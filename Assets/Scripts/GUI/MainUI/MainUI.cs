using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : Singleton<MainUI>
{
    public Text coinNum;
    public Joystick moveJoystick;

    private object[] currEventParamValues;
    private SceneEventType currSceneEventType;

    private void Awake()
    {
        if (moveJoystick != null)
        {
            moveJoystick.TouchEvent += MoveDirect;
            moveJoystick.TouchStateEvent += MoveState;
        }
    }

    private void OnEnable()
    {
        UpdateMoney();
        UpdateAttackBtn(null);
        EventCenter.AddEvent(EventEnum.UpdateMoney, UpdateMoney);
        EventCenter.AddEvent(EventEnum.ActionEvent, OnActionEvent);
        EventCenter.AddEvent(EventEnum.UpdateMainUIAttackBtn, UpdateAttackBtn);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.UpdateMoney, UpdateMoney);
        EventCenter.RemoveEvent(EventEnum.ActionEvent, OnActionEvent);
        EventCenter.RemoveEvent(EventEnum.UpdateMainUIAttackBtn, UpdateAttackBtn);
        ClearAcitonEvent();
    }

    public void UpdateAttackBtn(EventCenterData data)
    {
        if (GameData.myself == null) return;
    }

    public void UpdateMoney(EventCenterData data = null)
    {
        coinNum.text = GameData.gameCoin.ToString();
    }

    private void MoveDirect(Vector2 value)
    {
        if(GameData.myself != null) GameData.myself.MoveDirect(value);
    }

    private void MoveState(bool value)
    {
        if (GameData.myself != null) GameData.myself.MoveState(value);
    }

    public void OnActionEvent(EventCenterData data)
    {
        currEventParamValues = data.data as object[];
        currSceneEventType = (SceneEventType)currEventParamValues[0];
        if (currSceneEventType == SceneEventType.None)
        {
            ClearAcitonEvent();
            return;
        }
    }

    public void ProcessEvent()
    {
        if (currSceneEventType == SceneEventType.None)
        {
           
        }
    }

    public void Attack()
    {
        GameData.myself.animationManager.Play(AnimationName.Attack);
    }

    private void ClearAcitonEvent()
    {
        currSceneEventType = SceneEventType.None;
    }
}
