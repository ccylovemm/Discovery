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
            List<SkillVo> skill2List = new List<SkillVo>();
            List<SkillVo> skill3List = new List<SkillVo>();
            List<SkillVo> skill4List = new List<SkillVo>();
            SkillCFG.items.Foreach(vo => {
                if ((SkillElement)vo.Value.ComboType == SkillElement.TwoElement)
                {
                    skill2List.Add(vo.Value);
                }
                else if ((SkillElement)vo.Value.ComboType == SkillElement.ThreeElement)
                {
                    skill3List.Add(vo.Value);
                }
                else if ((SkillElement)vo.Value.ComboType == SkillElement.FourElement)
                {
                    skill4List.Add(vo.Value);
                }
            });

            randomSkillList.Clear();
            if (Random.Range(0, 10000) < 1000 * GameData.myData.elements.Count)
            {
                randomSkillList.Insert(0, skill4List[Random.Range(0, skill4List.Count)]);
            }

            if (Random.Range(0, 10000) < 2000 * GameData.myData.elements.Count)
            {
                randomSkillList.Insert(0 , skill3List[Random.Range(0, skill3List.Count)]);
            }

            for(int i = randomSkillList.Count; i < 3; i ++)
            {
                int index = Random.Range(0, skill2List.Count);
                randomSkillList.Insert(0, skill2List[index]);
                skill2List.RemoveAt(index);
            }
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
