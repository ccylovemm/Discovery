using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorationItem : MonoBehaviour
{
    public Image itemIcon;
    public GameObject starRoot;
    public List<GameObject> stars = new List<GameObject>();

    public DecorationView decorationView;

    private GameObject selectIcon;
    private ItemVo currItemVo;

    public void SetData(ItemVo vo , GameObject select)
    {
        currItemVo = vo;
        selectIcon = select;
        if (DataManager.userData.Decorations.Contains(currItemVo.Id))
        {
            int level = DataManager.userData.GetDecorationLevel(vo.Id);
            starRoot.gameObject.SetActive(true);
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].SetActive(level > i);
            }
        }
        else
        {
            starRoot.gameObject.SetActive(false);
        }
       
        ResourceManager.Instance.LoadIcon(currItemVo.ItemIcon.ToString() , icon =>
        {
            itemIcon.color = DataManager.userData.Decorations.Contains(currItemVo.Id) ? Color.white : Color.gray;
            itemIcon.sprite = icon;
            itemIcon.gameObject.SetActive(true);
        });
    }

    public void Clear()
    {
        itemIcon.gameObject.SetActive(false);
        starRoot.gameObject.SetActive(false);
    }

    public void SelectItem()
    {
        if (currItemVo != null && DataManager.userData.Decorations.Contains(currItemVo.Id))
        {
            selectIcon.SetActive(true);
            selectIcon.transform.parent = transform;
            selectIcon.transform.SetAsFirstSibling();
            selectIcon.transform.localPosition = new Vector3(49 , -45 , 0);
            decorationView.SelectItem(currItemVo);
        }
    }
}
