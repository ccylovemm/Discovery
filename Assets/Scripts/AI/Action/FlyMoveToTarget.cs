using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

public class FlyMoveToTarget : ActionBase
{
    private float moveTime;
    public override TaskStatus OnUpdate()
    {
        if (CommonUtil.Distance(actorObject.targetObject, actorObject) <= actorObject.currSkillData.skillVo.Distance * MapManager.textSize)
        {
            movement.Stop();
            return TaskStatus.Success;
        }
        else
        {
            if (Time.time > moveTime)
            {
                moveTime = Time.time + 1.3f;

                Vector2 grid = MapManager.GetGrid(actorObject.targetObject.transform.position);
                if (MapManager.mapPathData.ContainsKey(grid))
                {
                    List<Vector3> paths = MapManager.FindPath(MapManager.GetGrid(actorObject.transform.position), grid, actorObject.isFly);
                    if(paths != null && paths.Count > 0)
                    {
                        movement.FlyTo(paths.Count > 1 ? paths[1] : paths[0]);
                    }
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
    }
}
