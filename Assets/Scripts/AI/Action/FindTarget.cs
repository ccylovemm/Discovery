using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

public class FindTarget : ActionBase
{
    public override TaskStatus OnUpdate()
    {
        GetTarget();
        if (actorObject.targetObject == null)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }

    private void GetTarget()
    {
        if (actorObject.targetObject != null)
        {
            if (actorObject.targetObject.IsDead)
            {
                actorObject.targetObject = null;
            }
            else
            {
                float distance = CommonUtil.Distance(actorObject, actorObject.targetObject);
                distance = actorObject.master == null ? distance : Mathf.Min(distance, CommonUtil.Distance(actorObject.master, actorObject.targetObject));
                if (distance > actorObject.targetObject.actorData.cfgVo.FindRange * MapManager.textSize)
                {
                    actorObject.targetObject = null;
                }
            }
        }

        if (actorObject.targetObject == null)
        {
            List<ActorObject> allTargets = GameData.GetTarget(actorObject);
            float distance = float.MaxValue;
            int targetCount = 0;
            int count = allTargets.Count;
            for (int i = 0; i < count ; i++)
            {
                if(allTargets[i].IsDead || allTargets[i].IsDisappear) continue;
                float dis = CommonUtil.Distance(actorObject, allTargets[i]);
                dis = actorObject.master == null ? dis : Mathf.Min(dis, CommonUtil.Distance(actorObject.master, allTargets[i]));
                if (dis > actorObject.actorData.cfgVo.FindRange * MapManager.textSize) continue;
                targetCount ++;
                if (distance > dis)
                {
                    distance = dis;
                    actorObject.targetObject = allTargets[i];
                }
            }
            behaviorTree.targetCount = targetCount;
        }

        if (actorObject.targetObject == null)
        {
            actorObject.ClearMagic();
            if (Time.time - actorObject.beHitTime < 3.0f && actorObject.beHitObject != null && !actorObject.beHitObject.IsDead)
            {
                actorObject.targetObject = actorObject.beHitObject;
            }
        }
    }
}
