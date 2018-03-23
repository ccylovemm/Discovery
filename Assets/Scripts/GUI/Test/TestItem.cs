﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestItem : MonoBehaviour
{
    public TestView testView;
    public List<Image> iconList = new List<Image>();

    private SkillVo currSkillVo;

    public void SetData(SkillVo skillVo)
    {
        currSkillVo = skillVo;
        string[] elements = currSkillVo.Command.Split(',');
        for (int i = 0; i< iconList.Count; i ++)
        {
            if (i >= elements.Length)
            {
                iconList[i].gameObject.SetActive(false);
            }
            else
            {
                iconList[i].gameObject.SetActive(true);
                ResourceManager.Instance.LoadIcon("Icon_Element_" + elements[i], icon =>
                {
                    iconList[i].sprite = icon;
                });
            }
        }
    }

    public void SelectItem()
    {
        testView.SelectItem(currSkillVo);
    }
}
