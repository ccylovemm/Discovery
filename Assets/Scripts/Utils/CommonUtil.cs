using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtil
{
    static public float Distance(ActorObject a , ActorObject b)
    {
        return (a.currPos - b.currPos).magnitude - a.circleCollider2D.radius - b.circleCollider2D.radius;
    }
}
