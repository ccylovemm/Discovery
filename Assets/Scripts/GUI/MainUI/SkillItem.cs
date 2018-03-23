using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    public List<Image> elements = new List<Image>();

    private SkillVo currSkillVo;

    public void SetData(SkillVo skillVo)
    {
        currSkillVo = skillVo;
        string[] str = currSkillVo.Command.Split(',');
        int count = str.Length;
        for (int i = 0; i < count; i++)
        {
            ResourceManager.Instance.LoadIcon("Icon_Element_" + str[i], icon =>
            {
                elements[i].gameObject.SetActive(true);
                elements[i].sprite = icon;
            });
        }
    }

    public void Clear()
    {
        for (int i = 0; i < elements.Count; i ++)
        {
            elements[i].gameObject.SetActive(false);
        }
    }

    public void ShowTip()
    {
        
    }
}
