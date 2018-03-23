using UnityEngine;

public class HoleIn : SceneBase
{
    private void OnDisable()
    {
        CancelAction();
    }

    private void Update()
    {
        if (Time.frameCount % 8 != 0) return;
        if (GameData.myself == null) return;
        if (Vector2.Distance(transform.position, GameData.myself.currPos) - size > 0.02f)
        {
            if (actionItem == this)
            {
                CancelAction();
            }
        }
        else
        {
            if (actionItem == null || Vector2.Distance(actionItem.transform.position, GameData.myself.currPos) - actionItem.size > Vector2.Distance(transform.position, GameData.myself.currPos) - size)
            {
                if (actionItem != null) actionItem.CancelAction();
                actionItem = this;
                EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.EnterMap });
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

