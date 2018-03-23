using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [AddComponentMenu("Behavior Designer/Behavior Tree")]
    public class BehaviorTree : Behavior
    {
        public Movement movement;
        public Vector3 originPos;
        public ActorObject actorObject;
        public int targetCount = 0;

        private void Awake()
        {
            originPos = transform.position;
            movement = GetComponent<Movement>();
            actorObject = GetComponent<ActorObject>();
        }
    }
}