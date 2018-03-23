using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public List<SkillItem> skills;
    public List<MainUISkillItem> skillItems;
    public Joystick attackJoystick;
    public Joystick moveJoystick;
    public GameObject top;
    public GameObject dialogBtn;
    public GameObject pickupBtn;
    public MainTip mainTip;
    public Image carryItem;
    public GameObject carryItemBg;
    public Text coinNum;
    public Text diamondNum;
    public GameObject moneyObj;
    public HpBar bossHp;
    public List<GameObject> attackPressObjs = new List<GameObject>();
    public List<GameObject> attackNormalObjs = new List<GameObject>();

    private SceneBase eventItem;
    private SceneEventType currSceneEventType;
    private object[] currEventParamValues;
    private bool lockControl;

    static public MainUI Instance;

    private void Awake()
    {
        Instance = this;
        if (moveJoystick != null)
        {
            moveJoystick.TouchEvent += MoveDirect;
            moveJoystick.TouchStateEvent += MoveState;
        }

        if (attackJoystick != null)
        {
            attackJoystick.TouchEvent += AttackDirect;
            attackJoystick.TouchStateEvent += AttackState;
        }
    }

    private void OnEnable()
    {
        UpdateMoney();
        UpdateSkills();
        UpdateElements();
        UpdateCarryItem();
        UpdateAttackBtn(null);
        attackJoystick.gameObject.SetActive(true);
        if (SceneManager.Instance.sceneType == SceneType.Boss)
        {
            moneyObj.SetActive(false);
            bossHp.gameObject.SetActive(true);
            bossHp.SetData(1);
        }
        else
        {
            moneyObj.SetActive(true);
            bossHp.gameObject.SetActive(false);
        }

        EventCenter.AddEvent(EventEnum.ActionEvent, OnActionEvent);
        EventCenter.AddEvent(EventEnum.UpdateMoney, UpdateMoney);
        EventCenter.AddEvent(EventEnum.UpdateSkill, UpdateSkills);
        EventCenter.AddEvent(EventEnum.UpdateElement, UpdateElements);
        EventCenter.AddEvent(EventEnum.UpdateCarryItem, UpdateCarryItem);
        EventCenter.AddEvent(EventEnum.UpdateBossHp, UpdateBossHp);
        EventCenter.AddEvent(EventEnum.UpdateMainUIAttackBtn, UpdateAttackBtn);
        EventCenter.AddEvent(EventEnum.ResetAttackJoystickPos, UpdateAttackJoystickPos);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.ActionEvent, OnActionEvent);
        EventCenter.RemoveEvent(EventEnum.UpdateMoney, UpdateMoney);
        EventCenter.RemoveEvent(EventEnum.UpdateSkill, UpdateSkills);
        EventCenter.RemoveEvent(EventEnum.UpdateElement, UpdateElements);
        EventCenter.RemoveEvent(EventEnum.UpdateCarryItem, UpdateCarryItem);
        EventCenter.RemoveEvent(EventEnum.UpdateBossHp, UpdateBossHp);
        EventCenter.RemoveEvent(EventEnum.UpdateMainUIAttackBtn, UpdateAttackBtn);
        EventCenter.RemoveEvent(EventEnum.ResetAttackJoystickPos, UpdateAttackJoystickPos);
        ClearAcitonEvent();
    }

    public void UpdateAttackBtn(EventCenterData data)
    {
        if (GameData.myself == null) return;
        if (GameData.myself.currSkillData != null)
        {
            attackPressObjs[0].SetActive(true);
            attackPressObjs[1].SetActive(false);
            attackPressObjs[2].SetActive(false);
            attackPressObjs[3].SetActive(false);

            attackNormalObjs[0].SetActive(true);
            attackNormalObjs[1].SetActive(false);
            attackNormalObjs[2].SetActive(false);
            attackNormalObjs[3].SetActive(false);
        }
        else
        {
            attackPressObjs[0].SetActive(false);
            attackPressObjs[1].SetActive(GameData.myData.cfgId == 1);
            attackPressObjs[2].SetActive(GameData.myData.cfgId == 2);
            attackPressObjs[3].SetActive(GameData.myData.cfgId == 3);

            attackNormalObjs[0].SetActive(false);
            attackNormalObjs[1].SetActive(GameData.myData.cfgId == 1);
            attackNormalObjs[2].SetActive(GameData.myData.cfgId == 2);
            attackNormalObjs[3].SetActive(GameData.myData.cfgId == 3);
        }
    }

    public void UpdateAttackJoystickPos(EventCenterData data)
    {
        Vector2 dir = (Vector2)data.data;
        attackJoystick.joystickPoint.anchoredPosition = dir.normalized * 20;
    }

    public void LockControl(bool bol)
    {
        lockControl = bol;
        moveJoystick.Lock(bol);
        top.SetActive(!bol);
    }

    public void UpdateMoney(EventCenterData data = null)
    {
        coinNum.text = DataManager.userData.GoldCoin.ToString();
        diamondNum.text = DataManager.userData.Diamond.ToString();
    }

    public void UpdateBossHp(EventCenterData data)
    {
        if (SceneManager.Instance.sceneType == SceneType.Boss)
        {
            float value = (float)data.data;
            bossHp.SetData(value);
            bossHp.gameObject.SetActive(value != 0);
            moneyObj.SetActive(value == 0);
        }
    }

    public void OnPauseClick()
    {
        WindowManager.Instance.OpenWindow(WindowKey.PauseView);
    }

    public void OnUseItem()
    {
        carryItemBg.SetActive(false);
        carryItem.gameObject.SetActive(false);
        if (DataManager.userData.CarryId != 0)
        {
            ItemUtil.UseItem(DataManager.userData.CarryId);
            DataManager.userData.CarryId = 0;
        }
    }

    private void MoveDirect(Vector2 value)
    {
        if(GameData.myself != null) GameData.myself.MoveDirect(value);
    }

    private void MoveState(bool value)
    {
        if (GameData.myself != null) GameData.myself.MoveState(value);
    }

    private void AttackDirect(Vector2 value)
    {
        if (GameData.myself != null) GameData.myself.AttackDirect(value);
    }

    private void AttackState(bool value)
    {
        if (GameData.myself != null) GameData.myself.AttackState(value);
    }

    public void OnActionEvent(EventCenterData data)
    {
        if (GameData.myself != null) GameData.myself.AttackState(false);
        currEventParamValues = data.data as object[];
        currSceneEventType = (SceneEventType)currEventParamValues[0];
        if (currSceneEventType == SceneEventType.None)
        {
            ClearAcitonEvent();
            return;
        }
        else if (currSceneEventType == SceneEventType.PickUp)
        {
            pickupBtn.SetActive(true);
            dialogBtn.SetActive(false);
            eventItem = (SceneBase)currEventParamValues[1];
            DropItem dropItem = (DropItem)eventItem;
            if (dropItem.dropVo != null)
            {
                if (dropItem.dropVo.Type1 == 1)
                {
                    string str = "<b>" + LanguageManager.GetText(dropItem.itemVo.Name.ToString()) + "</b>\n";
                    str += LanguageManager.GetText(dropItem.itemVo.Description.ToString());
                    mainTip.SetMsg1(str);
                }
                else
                {
                    mainTip.Clear();
                }
            }
            else if (dropItem.itemVo != null)
            {
                string str = "<b>" + LanguageManager.GetText(dropItem.itemVo.Name.ToString()) + "</b>\n";
                str += LanguageManager.GetText(dropItem.itemVo.Description.ToString());
                mainTip.SetMsg1(str);
            }
        }
        else if (currSceneEventType == SceneEventType.NpcEvent)
        {
            pickupBtn.SetActive(false);
            dialogBtn.SetActive(true);
        }
        else if (currSceneEventType == SceneEventType.EnterMap)
        {
            pickupBtn.SetActive(false);
            dialogBtn.SetActive(true);
        }
        else if (currSceneEventType == SceneEventType.AltarEvent)
        {
            pickupBtn.SetActive(false);
            dialogBtn.SetActive(true);
        }
        attackJoystick.gameObject.SetActive(false);
    }

    public void ProcessEvent()
    {
        if (currSceneEventType == SceneEventType.None)
        {
           
        }
        else if (currSceneEventType == SceneEventType.PickUp)
        {
            DropItem dropItem = (DropItem)eventItem;
            if (dropItem.dropVo != null && dropItem.dropVo.Type1 == 2)
            {
                dropItem.Generate();
            }
            else if (dropItem.itemVo.Type == 2)//如果是元素
            {
                int index = GameData.myData.elements.IndexOf(dropItem.itemVo.SubType);
                if (index != -1)//相同元素
                {
                    SceneManager.Instance.RandomDropItem(dropItem.itemVo.SubType + 20000, 1, GameData.myself.transform.position);
                }
                else if (GameData.myData.elements.Count < 4)//可直接拾取
                {
                    GameData.myData.elements.Add(dropItem.itemVo.SubType);
                    DataManager.userData.Elements = GameData.myData.elements;
                }
                else //替换操作
                {
                    for (int i = 0; i < skillItems.Count; i++)
                    {
                        skillItems[i].SetReplace(true);
                    }
                    dialogBtn.SetActive(false);
                    pickupBtn.SetActive(false);
                    attackJoystick.gameObject.SetActive(true);
                    return;
                }
                UpdateElements();
                GameObject.Destroy(dropItem.gameObject);
            }
            else if ((ItemType)dropItem.itemVo.Type == ItemType.Item)//如果是物品
            {
                ItemUtil.GetItem(dropItem.itemVo.Id);
                GameObject.Destroy(dropItem.gameObject);
            }
            else if ((ItemType)dropItem.itemVo.Type == ItemType.Decoration)//如果是饰品
            {
                ItemUtil.GetDecoration(dropItem.itemVo.Id);
                GameObject.Destroy(dropItem.gameObject);
            }
            ClearAcitonEvent();
        }
        else if (currSceneEventType == SceneEventType.NpcEvent)
        {
            NpcEventType npcEventType = (NpcEventType)(uint)currEventParamValues[1];
            if (npcEventType == NpcEventType.UnLockAvatar)
            {
                WindowManager.Instance.OpenWindow(WindowKey.RoleView, new object[] {true , currEventParamValues[2] });
            }
            else if (npcEventType == NpcEventType.UpgradeAvatar)
            {
                WindowManager.Instance.OpenWindow(WindowKey.RoleView, new object[] {false});
            }
        //    ClearAcitonEvent();
        }
        else if(currSceneEventType == SceneEventType.EnterMap)
        {
            if (SceneManager.Instance.sceneType == SceneType.Home)
            {
                DataManager.userData.LevelCoin = 0;
                SceneManager.Instance.EnterScene();
            }
            else if (SceneManager.Instance.sceneType == SceneType.Boss)
            {
                if (LevelCFG.items.ContainsKey((DataManager.userData.Group + 1) + "" + 1))
                {
                    DataManager.userData.Group += 1;
                    DataManager.userData.GroupLevel = 1;
                }
                else
                {
                    DataManager.userData.Group = 1;
                    DataManager.userData.GroupLevel = 1;
                }
                DataManager.userData.EmployId = 0;
                GameData.myData.currHp = GameData.myData.cfgVo.MaxHp;
                DataManager.userData.Hp = GameData.myData.cfgVo.MaxHp;
                WindowManager.Instance.OpenWindow(WindowKey.LevelResultView, new object[] { LevelResultEnum.Victory });
            }
            else if(SceneManager.Instance.sceneType == SceneType.Level)
            {
                DataManager.userData.GroupLevel += 1;
                DataManager.userData.Hp = GameData.myData.currHp;
                if (GameData.employ != null) DataManager.userData.EmployHp = GameData.employ.actorData.currHp;

                if (!LevelCFG.items.ContainsKey(SceneManager.Instance.currLevelVo.MapId + "" + (SceneManager.Instance.currLevelVo.Level + 2)))
                {
                    SceneManager.Instance.EnterBoss();
                }
                else
                {
                    SceneManager.Instance.EnterScene();
                }
            }
            ClearAcitonEvent();
        }
        else if(currSceneEventType == SceneEventType.AltarEvent)
        {
            int altarId = (int)currEventParamValues[1];
            AltarSceneItem altarItem = (AltarSceneItem)currEventParamValues[2];
            if (altarItem.used)
            {
                EventCenter.DispatchEvent(EventEnum.ShowMsg, LanguageManager.GetText("xxxxxx"));
            }
            else
            {
                altarItem.Use();
                EventCenter.DispatchEvent(EventEnum.ShowMsg, altarId == 1 ? LanguageManager.GetText("xxxxxx") : (altarId == 2 ? LanguageManager.GetText("xxxxxx") : (altarId == 3 ? LanguageManager.GetText("xxxxxx") : LanguageManager.GetText("xxxxxx"))));
            }
        }
    }

    public void UpdateElements(EventCenterData data = null)
    {
        for (int i = 0; i< skillItems.Count; i ++)
        {
            skillItems[i].Clear();
            if (i < GameData.myData.elements.Count)
            {
                skillItems[i].SetData(GameData.myData.elements[i]);
            }
        }
    }

    public void UpdateSkills(EventCenterData data = null)
    {
        for(int i = 0; i < skills.Count; i ++)
        {
            skills[i].Clear();
        }

        Dictionary<SkillElement, string> skillDic = DataManager.userData.GetSkill;
        if (skillDic != null)
        {
            if (skillDic.ContainsKey(SkillElement.TwoElement))
            {
                skills[0].SetData(SkillCFG.items[skillDic[SkillElement.TwoElement]]);
            }
            if (skillDic.ContainsKey(SkillElement.ThreeElement))
            {
                skills[1].SetData(SkillCFG.items[skillDic[SkillElement.ThreeElement]]);
            }
            if (skillDic.ContainsKey(SkillElement.FourElement))
            {
                skills[2].SetData(SkillCFG.items[skillDic[SkillElement.FourElement]]);
            }
        }
    }

    public void UpdateCarryItem(EventCenterData data = null)
    {
        if (DataManager.userData.CarryId != 0)
        {
            ItemVo itemvo = ItemCFG.items[DataManager.userData.CarryId.ToString()];
            ResourceManager.Instance.LoadIcon(itemvo.ItemIcon, icon =>
            {
                carryItem.sprite = icon;
                carryItem.gameObject.SetActive(true);
                carryItemBg.SetActive(true);
            });
        }
        else
        {
            carryItemBg.SetActive(false);
            carryItem.gameObject.SetActive(false);
        }
    }

    public void ReplaceElement(uint replaceElement)
    {
        DropItem dropItem = (DropItem)eventItem;
        uint skillId = dropItem.itemVo.SubType;
        int index = GameData.myData.elements.IndexOf(replaceElement);
        SceneManager.Instance.RandomDropItem(replaceElement + 20000, 1, GameData.myself.transform.position);
        if (index != -1)
        {
            GameData.myData.elements.RemoveAt(index);
            GameData.myData.elements.Insert(index, skillId);
        }
        else
        {
            GameData.myData.elements.Add(skillId);
        }
        DataManager.userData.Elements = GameData.myData.elements;
        GameObject.Destroy(dropItem.gameObject);
        ClearAcitonEvent();
        UpdateElements();
    }

    private void ClearAcitonEvent()
    {
        eventItem = null;
        currSceneEventType = SceneEventType.None;
        mainTip.Clear();
        for (int i = 0; i < skillItems.Count; i++)
        {
            skillItems[i].SetReplace(false);
        }
        dialogBtn.SetActive(false);
        pickupBtn.SetActive(false);
        attackJoystick.gameObject.SetActive(true);
    }
}
