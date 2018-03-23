using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class ActionBase : Action
{
    protected BehaviorTree behaviorTree;

    public override void OnAwake()
    {
        base.OnAwake();
        behaviorTree = (BehaviorTree)Owner;
    }

    public ActorObject actorObject
    {
        get
        {
            return behaviorTree.actorObject;
        }
    }

    public Movement movement
    {
        get
        {
            return behaviorTree.movement;
        }
    }

    public Vector3 originPos
    {
        get
        {
            return behaviorTree.originPos;
        }
    }
}
