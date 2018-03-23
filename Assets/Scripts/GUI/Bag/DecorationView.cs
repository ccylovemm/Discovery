using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DecorationView : MonoBehaviour
{
    public GameObject subView1;
    public GameObject subView2;

    public Image itemIcon;
    public Text titleTxt;
    public Text itemName;
    public Text itemDesc;
    public Text currLvlDesc;
    public Text nextLvlDesc;
    public Text costNumTxt;
    public Text upgradeBtnTxt;

    public GameObject selectIcon;
    public GameObject starRoot;
    public List<GameObject> stars = new List<GameObject>();

    public RectTransform rectTransform;
    public GameObject decorationItemPrefab;
    public List<DecorationItem> itemList = new List<DecorationItem>();
    public Grid itemGrid;

    private ItemVo currItemVo;
    private DecorationVo currDecorationVo;


    private void Awake()
    {
        titleTxt.text = LanguageManager.GetText("210056");
        upgradeBtnTxt.text = LanguageManager.GetText("210044");
    }

    private void OnEnable()
    {
        Clear();
        UpdateList();
        subView1.SetActive(SceneManager.Instance.sceneType != SceneType.Home);
        subView2.SetActive(SceneManager.Instance.sceneType == SceneType.Home);
        itemList[0].SelectItem();
    }

    public void UpdateList()
    {
        List<ItemVo> voList = new List<ItemVo>();

        DataManager.userData.decorationLevel.Foreach(vo => voList.Add(ItemCFG.items[vo.Key.ToString()]));

        for (int i = 0; i < voList.Count; i++)
        {
            if (i >= itemList.Count)
            {
                itemList.Add(GameObject.Instantiate(decorationItemPrefab, itemGrid.transform).GetComponent<DecorationItem>());
            }
            itemList[i].gameObject.SetActive(true);
            itemList[i].SetData(voList[i] , selectIcon);
        }
        rectTransform.sizeDelta = new Vector2(1, Mathf.CeilToInt((float)voList.Count / itemGrid.lineCount) * itemGrid.width);
        itemGrid.ResetPosition();
    }

    public void SelectItem(ItemVo itemVo)
    {
        currItemVo = itemVo;
        currDecorationVo = DecorationCFG.items[itemVo.Id + "" + DataManager.userData.GetDecorationLevel(itemVo.Id)];

        itemName.text = LanguageManager.GetText(itemVo.Name.ToString());
        itemDesc.text = LanguageManager.GetText(itemVo.Description.ToString());
        currLvlDesc.text = LanguageManager.GetText(currDecorationVo.Description.ToString());
        if (DecorationCFG.items.ContainsKey(currDecorationVo.Id + "" + (currDecorationVo.Level + 1)))
        {
            nextLvlDesc.text = LanguageManager.GetText(DecorationCFG.items[currDecorationVo.Id + "" + (currDecorationVo.Level + 1)].Description.ToString());
        }
        else
        {
            nextLvlDesc.text = "";
        }

        costNumTxt.text = currDecorationVo.CostCoin.ToString();
        starRoot.SetActive(true);
        for (int i = 0; i < stars.Count; i ++)
        {
            stars[i].SetActive(currDecorationVo.Level > i);
        }

        ResourceManager.Instance.LoadIcon(currItemVo.ItemIcon.ToString(), icon =>
        {
            itemIcon.sprite = icon;
            itemIcon.gameObject.SetActive(true);
        });
    }

    public void UpgradeLevel()
    {
        if (currDecorationVo == null) return;
        if (DecorationCFG.items.ContainsKey(currDecorationVo.Id + "" + (currDecorationVo.Level + 1)))
        {
            if (currDecorationVo.CostCoin > DataManager.userData.GoldCoin)
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("100029"));
            }
            else
            {
                DataManager.userData.SetDecorationLevel(currDecorationVo.Id, (int)currDecorationVo.Level + 1);
                UpdateList();
                SelectItem(currItemVo);
                GameData.myData.FreshDecorations();
                DataManager.userData.GoldCoin -= (int)currDecorationVo.CostCoin;
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("100031"));
            }
        }
        else
        {
            EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("已达最大等级"));
        }
    }

    private void Clear()
    {
        starRoot.SetActive(false);
        selectIcon.SetActive(false);
        itemName.text = "";
        itemIcon.gameObject.SetActive(false);
        itemDesc.text = "";
        currLvlDesc.text = "";
        nextLvlDesc.text = "";
        costNumTxt.text = "0";
        for (int i = 0; i < itemList.Count; i ++)
        {
            itemList[i].Clear();
        }
    }
}
