using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class Follow : ActionBase
{
    private float followTime;
    private Vector2 targetPos;

    public override TaskStatus OnUpdate()
    {
        if (actorObject.master == null) return TaskStatus.Failure;
        if (CommonUtil.Distance(actorObject , actorObject.master) > actorObject.actorData.cfgVo.PatrolRange * MapManager.textSize)
        {
            if (Time.time > followTime)
            {
                followTime = Time.time + 1.0f;
                targetPos = MapManager.FindPosByRange(actorObject.master.currPos, 3 * MapManager.textSize, 0);
                if (targetPos != Vector2.zero)
                {
                    movement.MoveTo(targetPos);
                }
            }
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
