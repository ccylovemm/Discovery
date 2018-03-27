using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack : MonoBehaviour
{
    private ActorObject actorObject;

	void Start ()
    {
        actorObject = GetComponentInParent<ActorObject>();
    }

    void ExecuteAttack(int v)
    {
        actorObject.ExecuteAttack();
    }
}
