using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToTarget : ActionBase
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
    }
}
