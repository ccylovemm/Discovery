using UnityEngine;

public class Bezier
{
    static public Vector3 CalculateCubicBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (1 - t)* (1 - t)* p1 + 2 * (1 - t) * t * p2 + t* t * p3;
    }
}
