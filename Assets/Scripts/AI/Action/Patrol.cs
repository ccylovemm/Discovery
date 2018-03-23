using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class Patrol : ActionBase
{ 
    private float patroTime;

    public override TaskStatus OnUpdate()
    {
        if (Time.time > patroTime)
        {
            patroTime = Time.time + Random.Range(2.0f, 5.0f);
            Vector2 pos = MapManager.FindPosByRange(actorObject.master == null ? originPos : actorObject.master.transform.position, actorObject.actorData.cfgVo.PatrolRange * MapManager.textSize, MapManager.textSize);
            movement.MoveTo(pos, true);
        }
        return TaskStatus.Success;
    }
}
