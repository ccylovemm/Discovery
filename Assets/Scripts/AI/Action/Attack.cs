using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class Attack : ActionBase
{
    public bool canMove;
    private bool isAttack;
    private float attackInterval;

    public override TaskStatus OnUpdate()
    {
        if (isAttack)
        {
            if (!actorObject.hasCreateMagic && actorObject.magicBase == null)
            {
                isAttack = false;
                return TaskStatus.Success;
            }
            else if (canMove)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        else
        {
            if (Time.time < attackInterval)
            {
                return TaskStatus.Success;
            }
        }
        Vector2 direct = actorObject.targetObject.currPos - actorObject.currPos;
        float distance = CommonUtil.Distance(actorObject , actorObject.targetObject);
        if (!actorObject.targetObject.IsDead && distance <= actorObject.currSkillData.skillVo.Distance * MapManager.textSize)
        {
            if (!actorObject.hasCreateMagic && actorObject.magicBase == null)
            {
                isAttack = true;
                movement.Stop();
                actorObject.UpdateDirect(direct);
                attackInterval = Time.time + behaviorTree.actorObject.actorData.cfgVo.AttackInterval;
                SkillManager.Instantiate(actorObject, behaviorTree.actorObject.currSkillData.skillVo);
                behaviorTree.actorObject.currSkillData.cdTime = Time.time + behaviorTree.actorObject.currSkillData.skillVo.CD;
                return TaskStatus.Running;
            }
        }
        return TaskStatus.Success;
    }
}
