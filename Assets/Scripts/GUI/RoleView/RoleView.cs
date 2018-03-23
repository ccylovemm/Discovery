using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleView : MonoBehaviour
{
    public Text roleTip;
    public Text roleName;
    public Image roleIcon;
    public List<GameObject> stars = new List<GameObject>();

    public GameObject subView1;
    public GameObject subView2;

    //sub1

    public Text text1;
    public Text text2;

    public Text maxHp;
    public Text speedTxt;
    public Text massTxt;
    public Image elementIcon;

    public Text talentSkillName;
    public Image talentSkillIcon;

    public Text passiveSkill1Name;
    public Image passiveSkill1Icon;
    public Text passiveSkill2Name;
    public Image passiveSkill2Icon;

    public GameObject heroObj;

    public Text unlockBtnTxt;

    //sub2

    public Text upgradeBtnTxt;
    public GameObject upgradeBtn;

    public GameObject selectIcon;
    public RectTransform rectTransform;
    public GameObject roleLevelItemPrefab;
    public List<RoleLevelItem> roleLevelList = new List<RoleLevelItem>();
    public Grid roleLevelGird;
    public ScrollRect scrollRect;

    private ActorVo currActorVo;

    private GameObject hero;

    private string tip1 = "";
    private string tip2 = "";
    private string tip3 = "";

    private void Start()
    {
        text1.text = LanguageManager.GetText("xxxxx");
        text2.text = LanguageManager.GetText("xxxxx");

        upgradeBtnTxt.text = LanguageManager.GetText("210044");
    }

    private void SetParameters(object[] args)
    {
        bool isUnlock = (bool)args[0];
        uint roleId = isUnlock ? (uint.Parse((string)args[1])) : GameData.myData.cfgId;
        subView1.SetActive(isUnlock);
        subView2.SetActive(!isUnlock);
        UpdateData(roleId);
    }

    private void UpdateData(uint id)
    {
        uint level = DataManager.userData.actor.level;
        currActorVo = ActorCFG.items[id + "" + level];

        roleTip.text = LanguageManager.GetText(currActorVo.Desc);
        roleName.text = LanguageManager.GetText(currActorVo.Name);
        ResourceManager.Instance.LoadIcon("HeadIcon" + currActorVo.Id, sprite =>
        {
            roleIcon.sprite = sprite;
        });

        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(currActorVo.Level > i);
        }

        //sub1

        maxHp.text = currActorVo.MaxHp.ToString();
        speedTxt.text = currActorVo.MoveSpeed.ToString();
        massTxt.text = currActorVo.Mass.ToString();

        GameObject.Destroy(hero);

        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            hero = GameObject.Instantiate((GameObject)model.LoadAsset("Hero" + currActorVo.Id + "_IdleImage.prefab") , heroObj.transform);
        });

        string[] elements = currActorVo.Elements.Split(',');
        ResourceManager.Instance.LoadIcon("Icon_Element_" + elements[0], sprite =>
        {
           elementIcon.sprite = sprite;
        });

        string[] str = currActorVo.Skills.Split(',');
        string[] skills = str[0].Split('_');
        SkillLevelVo skillVo = SkillLevelCFG.items[skills[0] + "" + skills[1]];
        tip1 = LanguageManager.GetText(skillVo.Description);
        talentSkillName.text = LanguageManager.GetText(skillVo.Name);
        ResourceManager.Instance.LoadIcon(skillVo.Icon, sprite =>
        {
            talentSkillIcon.sprite = sprite;
        });

        string[] talents = currActorVo.Talent.Split(',');
        TalentVo talentVo = TalentCFG.items[talents[0]];
        tip2 = LanguageManager.GetText(talentVo.Desc);
        passiveSkill1Name.text = LanguageManager.GetText(talentVo.Name);
        ResourceManager.Instance.LoadIcon(talentVo.Icon, sprite =>
        {
            passiveSkill1Icon.sprite = sprite;
        });
        if (talents.Length > 1)
        {
            talentVo = TalentCFG.items[talents[1]];
            tip3 = LanguageManager.GetText(talentVo.Desc);
            passiveSkill2Name.text = LanguageManager.GetText(talentVo.Name);
            ResourceManager.Instance.LoadIcon(talentVo.Icon, sprite =>
            {
                passiveSkill2Icon.sprite = sprite;
                passiveSkill2Icon.gameObject.SetActive(true);
            });
        }
        else
        {
            passiveSkill2Name.text = "";
            passiveSkill2Icon.gameObject.SetActive(false);
        }
       
        unlockBtnTxt.text = LanguageManager.GetText(DataManager.userData.HasUnlockActor(currActorVo.Id) ? "210009" : "210052");

        //sub2

        Clear();
        List<ActorVo> actorList = new List<ActorVo>();
        ActorCFG.items.Foreach(vo => { if (vo.Value.Id == id) { actorList.Add(vo.Value); } });
        int index = 0;
        for (int i = 0; i < actorList.Count; i++)
        {
            if (i >= roleLevelList.Count)
            {
                roleLevelList.Add(GameObject.Instantiate(roleLevelItemPrefab, roleLevelGird.transform).GetComponent<RoleLevelItem>());
            }
            roleLevelList[i].gameObject.SetActive(true);
            roleLevelList[i].SetData(actorList[i], selectIcon);
            if (actorList[i].Level == level)
            {
                index = i;
            }
        }
        roleLevelList[index].SelectItem();
        rectTransform.sizeDelta = new Vector2(1, Mathf.CeilToInt((float)roleLevelList.Count / roleLevelGird.lineCount) * roleLevelGird.width);
        roleLevelGird.ResetPosition();
        scrollRect.verticalNormalizedPosition = 1.0f - (float)(index + 1) / actorList.Count;
        upgradeBtn.SetActive(ActorCFG.items.ContainsKey(currActorVo.Id + "" + (currActorVo.Level + 1)));
    }

    public void UpgradeLevel()
    {
        if (currActorVo.LevelUp > DataManager.userData.GoldCoin)
        {
            EventCenter.DispatchEvent(EventEnum.ShowMsg , LanguageManager.GetText("210046"));
        }
        else
        {
            DataManager.userData.UpgradeActorLevel();
            GameData.FreshData();
            UpdateData(currActorVo.Id);
            EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("210047"));
        }
    }

    public void Unlock()
    {
        if (DataManager.userData.HasUnlockActor(currActorVo.Id))
        {
            DataManager.userData.Clear();
            DataManager.userData.ActorId = currActorVo.Id;
            DataManager.userData.FreshData(true);
            EventCenter.DispatchEvent(EventEnum.UpdateElement);
            EventCenter.DispatchEvent(EventEnum.UpdateAvatar);
            Close();
        }
        else
        {
            DataManager.userData.UnlockActor(currActorVo.Id);
            UpdateData(currActorVo.Id);
        }
    }

    public void ShowSkill1Tip()
    {
        Tooltip.Instance.ShowTip(tip1);
    }

    public void ShowSkill2Tip()
    {
        Tooltip.Instance.ShowTip(tip2);
    }

    public void ShowSkill3Tip()
    {
        Tooltip.Instance.ShowTip(tip3);
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.RoleView);
    }

    private void Clear()
    {
        for (int i = 0; i < roleLevelList.Count; i ++)
        {
            roleLevelList[i].gameObject.SetActive(true);
        }
    }
}
