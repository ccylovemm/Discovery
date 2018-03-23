using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class MoveAvoidToTarget : ActionBase
{
    private float moveTime;
    private float avoidTime;

    public override TaskStatus OnUpdate()
    {
        float distance = CommonUtil.Distance(actorObject, actorObject.targetObject);
        float shotRangge = actorObject.currSkillData.skillVo.Distance * MapManager.textSize;

        if (distance > shotRangge)
        {
            if (Time.time > moveTime)
            {
                moveTime = Time.time + 0.3f;
                Vector2 grid = MapManager.GetGrid(actorObject.targetObject.transform.position);
                if (MapManager.mapPathData.ContainsKey(grid))
                {
                    movement.MoveTo(actorObject.targetObject.transform.position);
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Failure;
                }
            }
            else
            {
                return TaskStatus.Success;
            }
        }
        else
        {
            if (Time.time > avoidTime)
            {
                avoidTime = Time.time + 1.0f;
                float avoidDistance = Mathf.Min(actorObject.currSkillData.skillVo.Distance, shotRangge * 0.9f);
                if (distance < avoidDistance)
                {
                    float dis = Mathf.Max(avoidDistance, shotRangge * 0.7f);
                    Vector2 pos = MapManager.FindEscapePos(actorObject.transform.position, actorObject.targetObject.transform.position, dis, shotRangge - dis);
                    if (pos == (Vector2)actorObject.transform.position)
                    {
                        pos = MapManager.FindPosByRange(actorObject.targetObject.transform.position, shotRangge, dis);
                    }
                    if (MapManager.mapPathData.ContainsKey(MapManager.GetGrid(pos)))
                    {
                        movement.MoveTo(pos, true);
                    }  
                }
            }
        }
        return TaskStatus.Success;
    }
}
