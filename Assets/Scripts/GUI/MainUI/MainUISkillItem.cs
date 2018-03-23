using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUISkillItem : MonoBehaviour
{
    public Image icon;
    public Button button;
    public GameObject iconBg;
    public GameObject replaceObj;

    public MainUI mainUI;

    private uint element;
    private bool status = false;

    public void SetData(uint element_)
    {
        element = element_;
        ResourceManager.Instance.LoadIcon("btn_spell_" + element, image =>
        {
            icon.sprite = image;
            icon.gameObject.SetActive(true);
            
        });
        ResourceManager.Instance.LoadIcon("btn_spellClick_" + element, image =>
        {
            SpriteState spriteState = new SpriteState();
            spriteState.pressedSprite = image;
            button.spriteState = spriteState;
        });
        iconBg.SetActive(false);
    }

    public void SetReplace(bool replace)
    {
        status = replace;
        replaceObj.SetActive(replace);
    }

    public void OnClick()
    {
        if (!status)
        {
            if (element == 0 || GameData.elements.Count >= 4) return;
            SkillManager.AddElement(element);
            EventCenter.DispatchEvent(EventEnum.UseSkill);
        }
        else
        {
            status = false;
            mainUI.ReplaceElement(element);
        }
    }

    public void Clear()
    {
        element = 0;
        iconBg.SetActive(true);
        replaceObj.SetActive(false);
        icon.gameObject.SetActive(false);
    }
}
