using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarItem : MonoBehaviour
{
    public Text skillName;
    public Grid skillElemetnGrid;
    public List<Image> skillElements;
    public Grid skillBgGrid;
    public List<GameObject> skillBgs;
    public Grid addIconGrid;
    public List<GameObject> addIcons;
    public Text skillDesc;
    public Text replaceBtnTxt;

    private SkillVo currSkillVo;
    private SkillLevelVo currSkillLevelVo;

    private void Start()
    {
        replaceBtnTxt.text = LanguageManager.GetText("210002");
    }

    public void SetData(SkillVo skillVo)
    {
        Clear();

        currSkillVo = skillVo;
        currSkillLevelVo = SkillLevelCFG.items[currSkillVo.SkillId + "" + 1];
        skillName.text = LanguageManager.GetText(currSkillLevelVo.Name);
        skillDesc.text = LanguageManager.GetText(currSkillLevelVo.Description);
        string[] str = currSkillVo.Command.Split(',');
        int count = str.Length;
        for (int i = 0; i < count; i ++)
        {
            ResourceManager.Instance.LoadIcon("Icon_Element_" + str[i], icon =>
            {
                skillElements[i].gameObject.SetActive(true);
                skillElements[i].sprite = icon;
            });
            if (i < addIcons.Count)
            {
                addIcons[i].SetActive(count - 1 > i);
            }
            skillBgs[i].SetActive(true);
        }
        skillElemetnGrid.ResetPosition();
        addIconGrid.ResetPosition();
        skillBgGrid.ResetPosition();
    }

    private void Clear()
    {
        for (int i = 0; i < skillElements.Count; i ++)
        {
            skillElements[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < skillBgs.Count; i ++)
        {
            skillBgs[i].SetActive(false);
        }

        for (int i = 0; i < addIcons.Count; i++)
        {
            addIcons[i].SetActive(false);
        }
    }

    public void ReplaceSkill()
    {
        DataManager.userData.SetSkill((SkillElement)currSkillVo.ComboType , currSkillVo.Command);
        EventCenter.DispatchEvent(EventEnum.UpdateSkill);
        WindowManager.Instance.CloseWindow(WindowKey.AltarView);
    }
}
