using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
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
            attackJoystick.TouchStateEvent += AttackState;
        }
    }

    private void OnEnable()
    {
        UpdateMoney();
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
        EventCenter.AddEvent(EventEnum.UpdateBossHp, UpdateBossHp);
        EventCenter.AddEvent(EventEnum.UpdateMainUIAttackBtn, UpdateAttackBtn);
        EventCenter.AddEvent(EventEnum.ResetAttackJoystickPos, UpdateAttackJoystickPos);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.ActionEvent, OnActionEvent);
        EventCenter.RemoveEvent(EventEnum.UpdateMoney, UpdateMoney);
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

    private void MoveDirect(Vector2 value)
    {
        if(GameData.myself != null) GameData.myself.MoveDirect(value);
    }

    private void MoveState(bool value)
    {
        if (GameData.myself != null) GameData.myself.MoveState(value);
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
            else if ((ItemType)dropItem.itemVo.Type == ItemType.Item)//如果是物品
            {
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
            ClearAcitonEvent();
        }
    }

    private void ClearAcitonEvent()
    {
        eventItem = null;
        currSceneEventType = SceneEventType.None;
        mainTip.Clear();
        dialogBtn.SetActive(false);
        pickupBtn.SetActive(false);
        attackJoystick.gameObject.SetActive(true);
    }
}
