using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUtil
{
    static public int WallMasks()
    {
        return LayerMask.GetMask("Obstacle", "SceneItemHigh");
    }

    static public int DamageMasks()
    {
        return LayerMask.GetMask("Obstacle" , "Player", "Actor" , "SceneItemHigh");
    }

    static public bool IsWall(int layer)
    {
        return layer == LayerMask.NameToLayer("Obstacle")
            || layer == LayerMask.NameToLayer("SceneItemHigh");
    }

    static public bool IsNotAvailable(int layer)
    {
        return layer == LayerMask.NameToLayer("Hole") 
            || layer == LayerMask.NameToLayer("Obstacle") 
            || layer == LayerMask.NameToLayer("SceneItemHigh");
    }

    static public bool IsDamageLayer(int layer)
    {
        return layer == LayerMask.NameToLayer("Obstacle")
            || layer == LayerMask.NameToLayer("Actor")
            || layer == LayerMask.NameToLayer("Player")
            || layer == LayerMask.NameToLayer("SceneItemHigh");
    }

    static public int LayerToActor()
    {
        return LayerMask.NameToLayer("Actor");
    }

    static public int LayerToPlayer()
    {
        return LayerMask.NameToLayer("Player");
    }

    static public int LayerToDefault()
    {
        return LayerMask.NameToLayer("Default");
    }

    static public int LayerToHole()
    {
        return LayerMask.NameToLayer("Hole");
    }

    static public int LayerToObstacle()
    {
        return LayerMask.NameToLayer("Obstacle");
    }
}
