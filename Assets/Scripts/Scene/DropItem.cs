using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropItem : SceneBase
{
    public ItemVo itemVo;

    private void OnDisable()
    {
        CancelAction();
    }

    private void Update()
    {
        if (Time.frameCount % 8 != 0) return;

        if (Vector2.Distance(transform.position, GameData.myself.transform.position)> 0.1f)
        {
            if (actionItem == this)
            {
                CancelAction();
            }
        }
        else
        {
            if (actionItem == null || Vector2.Distance(actionItem.transform.position, GameData.myself.transform.position) > Vector2.Distance(transform.position, GameData.myself.transform.position))
            {
                if (actionItem != null) actionItem.CancelAction();
                actionItem = this;
                EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.PickUp, this });
            }
        }
    }

    public override void CancelAction()
    {
        if (actionItem == this)
        {
            actionItem = null;
            EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.None });
        }
    }
}
