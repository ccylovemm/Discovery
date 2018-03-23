using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ShopView : MonoBehaviour
{
    public Image npcIcon;
    public Text npcDialog;
    public Text npcName;
    public Text goodsName;
    public Text goodsDesc;
    public Text coinNum;
    public Text diamondNum;
    public Text buyText;
    public Button buyButton;

    public Text itemTabTxt;

    public RectTransform selectTrans;

    public RectTransform rectTransform;
    public GameObject shopItemPrefab;
    public List<ShopItem> shopList = new List<ShopItem>();
    public Grid shopGrid;

    private ShopVo currShopVo;
    private ItemVo currItemVo;
    private uint currType;

    private void Awake()
    {
        itemTabTxt.text = LanguageManager.GetText("100026");
    }

    private void OnEnable()
    {
        OnUpdateMoney();
        EventCenter.AddEvent(EventEnum.ShopSelectItem , OnSelectItem);
        EventCenter.AddEvent(EventEnum.UpdateMoney, OnUpdateMoney);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.ShopSelectItem, OnSelectItem);
        EventCenter.RemoveEvent(EventEnum.UpdateMoney, OnUpdateMoney);
    }

    private void SetParameters(object[] args)
    {
        NpcVo npcVo = NpcCFG.items[(string)args[0]];
        npcName.text = LanguageManager.GetText(npcVo.Name);
        npcDialog.text = LanguageManager.GetText(npcVo.Dialogue);
        SelectItem();
    }

    public void SelectItem()
    {
        SelectShopType(2);
    }

    public void SelectShopType(uint type , bool select = true)
    {
        currType = type;
        List<ShopVo> shopVoList = new List<ShopVo>();
        ShopCFG.items.Foreach(vo =>
        {
            if (vo.Value.GoodsType == type)
            {
                shopVoList.Add(vo.Value);
            }
        });

        for (int i = 0; i < shopList.Count; i ++)
        {
            shopList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < shopVoList.Count; i++)
        {
            if (i >= shopList.Count)
            {
                shopList.Add(GameObject.Instantiate(shopItemPrefab, shopGrid.transform).GetComponent<ShopItem>());
            }
            shopList[i].gameObject.SetActive(true);
            shopList[i].SetData(shopVoList[i] , selectTrans);
        }
        if(select) shopList[0].OnSelectItem();
        rectTransform.sizeDelta = new Vector2(1, Mathf.CeilToInt((float)shopVoList.Count / shopGrid.lineCount) * shopGrid.width);
        shopGrid.ResetPosition();
    }

    public void OnSelectItem(EventCenterData data)
    {
        currShopVo = data.data as ShopVo;
        currItemVo = ItemCFG.items[currShopVo.LinkItems.ToString()];
        UpdateGoodsInfo();
    }

    private void UpdateGoodsInfo()
    {
        goodsName.text = LanguageManager.GetText(currItemVo.Name.ToString());
        goodsDesc.text = LanguageManager.GetText(currItemVo.Description.ToString());
        if ((ItemType)currItemVo.Type == ItemType.Item)
        {
            if (DataManager.userData.CarryId == currItemVo.Id)
            {
                buyText.text = LanguageManager.GetText("100022");
                buyButton.enabled = false;
            }
            else
            {
                buyText.text = LanguageManager.GetText("100024");
                buyButton.enabled = true;
            }
        }
    }

    public void BuyGoods()
    {
        int cost =Mathf.CeilToInt(currShopVo.GoodsPrice * (currShopVo.Discount / 10000.0f));
        if (currShopVo.CostType == 1)
        {
            if (DataManager.userData.GoldCoin >= cost)
            {
                BuySuccess();
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210048"));
                DataManager.userData.GoldCoin -= cost;
            }
            else
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg , LanguageManager.GetText("210046"));
            }
        }
        else if (currShopVo.CostType == 2)
        {
            if (DataManager.userData.Diamond >= cost)
            {
                BuySuccess();
                DataManager.userData.Diamond -= cost;
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210048"));
            }
            else
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210049"));
            }
        }
    }

    private void BuySuccess()
    {
        if ((ItemType)currItemVo.Type == ItemType.Item)
        {
            ItemUtil.GetItem(currItemVo.Id);
        }
        SelectShopType(currType , false);
    }

    public void OnUpdateMoney(EventCenterData data = null)
    {
        coinNum.text = DataManager.userData.GoldCoin.ToString();
        diamondNum.text = DataManager.userData.Diamond.ToString();
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.ShopView);
    }
}
