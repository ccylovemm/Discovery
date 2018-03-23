using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class FindSkill : ActionBase
{
    public bool HasCD = false;

    public override TaskStatus OnUpdate()
    {
        SelectSkill();
        return TaskStatus.Success;
    }

    public void SelectSkill()
    {
        if ((ActorType)actorObject.targetObject.actorData.cfgVo.Type == ActorType.Boss)
        {
            if (SelectElementSkill(SkillElement.ThreeElement)) return;
            else if (SelectElementSkill(SkillElement.TwoElement)) return;
            else if (SelectElementSkill(SkillElement.OneElement)) return;
            else SelectElementSkill(SkillElement.ZeroElement);
        }
        else if ((ActorType)actorObject.targetObject.actorData.cfgVo.Type == ActorType.MonsterHard)
        {
            if (behaviorTree.targetCount == 1)
            {
                if (SelectElementSkill(SkillElement.TwoElement , true)) return;
                else if (SelectElementSkill(SkillElement.ThreeElement)) return;
                else if(SelectElementSkill(SkillElement.TwoElement)) return;
                else if (SelectElementSkill(SkillElement.OneElement)) return;
                else SelectElementSkill(SkillElement.ZeroElement);
            }
            else
            {
                if (SelectElementSkill(SkillElement.ThreeElement)) return;
                else if (SelectElementSkill(SkillElement.TwoElement)) return;
                else if (SelectElementSkill(SkillElement.OneElement)) return;
                else SelectElementSkill(SkillElement.ZeroElement);
            }
        }
        else if ((ActorType)actorObject.targetObject.actorData.cfgVo.Type == ActorType.Monster)
        {
            if (behaviorTree.targetCount == 1)
            {
                if (SelectElementSkill(SkillElement.OneElement , true)) return;
                else if (SelectElementSkill(SkillElement.TwoElement , true)) return;
                else if (SelectElementSkill(SkillElement.ThreeElement, true)) return;
                else if (SelectElementSkill(SkillElement.OneElement)) return;
                else SelectElementSkill(SkillElement.ZeroElement);
            }
            else if(behaviorTree.targetCount <= 3)
            {
                if (SelectElementSkill(SkillElement.TwoElement , true)) return;
                else if (SelectElementSkill(SkillElement.ThreeElement , true)) return;
                else if (SelectElementSkill(SkillElement.TwoElement)) return;
                else if (SelectElementSkill(SkillElement.OneElement)) return;
                else SelectElementSkill(SkillElement.ZeroElement);
            }
            else
            {
                if (SelectElementSkill(SkillElement.ThreeElement)) return;
                else if (SelectElementSkill(SkillElement.TwoElement)) return;
                else if (SelectElementSkill(SkillElement.OneElement)) return;
                else SelectElementSkill(SkillElement.ZeroElement);
            }
        }
        else if ((ActorType)actorObject.targetObject.actorData.cfgVo.Type == ActorType.Player || (ActorType)actorObject.targetObject.actorData.cfgVo.Type == ActorType.Pet)
        {
            if (SelectElementSkill(SkillElement.ThreeElement)) return;
            else if (SelectElementSkill(SkillElement.TwoElement)) return;
            else if (SelectElementSkill(SkillElement.OneElement)) return;
            else SelectElementSkill(SkillElement.ZeroElement);
        }
    }

    private bool SelectElementSkill(SkillElement skillType , bool conflict = false)
    {
        List<SkillData> skillList = actorObject.actorData.skills.Where(vo => (SkillElement)vo.skillVo.ComboType == skillType).ToList<SkillData>();

        int length = skillList.Count;

        if (length == 0)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < length; i ++)
            {
                if (HasCD && skillList[i].cdTime > Time.time) continue;
                string[] elements = skillList[i].skillVo.AttachElement.Split(',');
                for (int n = 0; n < elements.Length; n ++)
                {
                    if (AttributeConflictCFG.items.ContainsKey(elements[n] + actorObject.targetObject.actorData.cfgVo.Attribute))
                    {
                        actorObject.currSkillData = new SkillData(skillList[i].skillVo);
                        return true;
                    }
                }
            }
            if (conflict)
            {
                return false;
            }
            if (HasCD)
            {
                for (int i = skillList.Count - 1; i >= 0; i--)
                {
                    if (Time.time > skillList[i].cdTime)
                    {
                        actorObject.currSkillData = skillList[i];
                        return true;
                    }
                }
                return false;
            }
            else
            {
                actorObject.currSkillData = new SkillData(skillList[Random.Range(0, length)].skillVo);
            }
        }
        return true;
    }
}
