using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class Lock : ActionBase
{
    public bool HasCD = false;

    public override TaskStatus OnUpdate()
    {
        if (actorObject.IsDead || actorObject.isFrozen || actorObject.isDizzy)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
