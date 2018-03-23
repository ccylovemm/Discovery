using UnityEngine;
using System.Collections.Generic;

public class AltarSceneItem : SceneBase
{
    public int id;
    public bool used = false;
    public GameObject normalStatus;
    public GameObject usedStatus;

    private Vector3 altarPos;
    private List<SkillVo> randomSkillList = new List<SkillVo>();

    private void Start()
    {
        altarPos = transform.position + new Vector3(0.5f , 0.3f , 0) * MapManager.textSize;
    }

    private void OnDisable()
    {
        CancelAction();
    }

    public void Use()
    {
        if(!used)
        {
            used = true;
            normalStatus.SetActive(false);
            usedStatus.SetActive(true);
            List<SkillVo> skillList = new List<SkillVo>();
            SkillCFG.items.Foreach(vo => {
                if ((SkillElement)vo.Value.ComboType != SkillElement.OneElement)
                {
                    skillList.Add(vo.Value);
                }
            });

            randomSkillList.Clear();
            int index = Random.Range(0 , skillList.Count);
            randomSkillList.Add(skillList[index]);
            skillList.RemoveAt(index);
            index = Random.Range(0, skillList.Count);
            randomSkillList.Add(skillList[index]);
            skillList.RemoveAt(index);
            index = Random.Range(0, skillList.Count);
            randomSkillList.Add(skillList[index]);
            skillList.RemoveAt(index);
        }
        WindowManager.Instance.OpenWindow(WindowKey.AltarView, new object[] { randomSkillList });
    }

    private void Update()
    {
        if (GameData.myself == null) return;
        if (Time.frameCount % 8 != 0) return;

        if (Vector2.Distance(altarPos, GameData.myself.currPos) - size > 0.2f)
        {
            if (actionItem == this)
            {
                CancelAction();
            }
        }
        else
        {
            if (actionItem == null || Vector2.Distance(actionItem.transform.position, GameData.myself.currPos) - actionItem.size > Vector2.Distance(altarPos, GameData.myself.currPos) - size)
            {
                if (actionItem != null) actionItem.CancelAction();
                actionItem = this;
                EventCenter.DispatchEvent(EventEnum.ActionEvent, new object[] { SceneEventType.AltarEvent, id , this});
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
