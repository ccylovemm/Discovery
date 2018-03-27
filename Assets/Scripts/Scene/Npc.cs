using UnityEngine;

public class Npc : SceneBase
{
    public int npcId;
    private NpcVo npcVo;

    private void Awake()
    {
        npcVo = NpcCFG.items[npcId.ToString()];
    }

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
                EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.NpcEvent, npcVo.EventType, npcVo.EventValue});
                if (!string.IsNullOrEmpty(npcVo.Dialogue))
                {
                    EventCenter.DispatchEvent(EventEnum.ShowDialogue, new object[] { true, transform.position, LanguageManager.GetText(npcVo.Name) , LanguageManager.GetText(npcVo.Bubble) });
                }
            }
        }
    }

    public override void CancelAction()
    {
        if (actionItem == this)
        {
            actionItem = null;
            EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.None });
            if (!string.IsNullOrEmpty(npcVo.Dialogue))
            {
                EventCenter.DispatchEvent(EventEnum.ShowDialogue, new object[] { false });
            }
        }
    }
}
