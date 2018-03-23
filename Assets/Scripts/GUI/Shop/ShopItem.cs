using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image itemIcon;
    public Image costIcon;
    public Text costNum;
    public Text saleNum;
    public Text ownTxt;
    public GameObject saleBg;

    private RectTransform selectIcon;
    private ShopVo currVo;
    private ItemVo currItemVo;

    public void SetData(ShopVo shopVo, RectTransform trans)
    {
        selectIcon = trans;
        currVo = shopVo;
        currItemVo = ItemCFG.items[shopVo.LinkItems.ToString()];
        ResourceManager.Instance.LoadIcon(currItemVo.ItemIcon, icon =>
        {
            itemIcon.sprite = icon;
        });
        ResourceManager.Instance.LoadIcon(ItemCFG.items[currVo.CostType.ToString()].ItemIcon, icon =>
        {
            costIcon.sprite = icon;
        });
        costNum.text = currVo.GoodsPrice.ToString();
        saleNum.text = (currVo.Discount >= 10000 ? "" : ("-" + (10000 - currVo.Discount) / 100 + "%"));
        saleBg.SetActive(saleNum.text != "");
        if ((ItemType)currItemVo.Type == ItemType.Item)
        {
            if (DataManager.userData.CarryId == currItemVo.Id)
            {
                ownTxt.text = LanguageManager.GetText("100022");
                costNum.gameObject.SetActive(false);
                costIcon.gameObject.SetActive(false);
            }
            else
            {
                ownTxt.text = "";
                costNum.gameObject.SetActive(true);
                costIcon.gameObject.SetActive(true);
            }
        }
    }

    public void OnSelectItem()
    {
        selectIcon.parent = transform;
        selectIcon.localPosition = new Vector3(42 , -54 , 0);
        EventCenter.DispatchEvent(EventEnum.ShopSelectItem, currVo);
    }
}
