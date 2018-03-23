using UnityEngine;

public class AStarNode
{
    public Vector2 mapGird;
    public Vector3 mapPos;
    public int gValue;
    public int hValue;

    public int i;
    public int index;

    public float fValue
    {
        get
        {
            return gValue + hValue;
        }
    }
    public AStarNode parent;

    public AStarNode(Vector2 grid)
    {
        mapGird = grid;
        mapPos = MapManager.GetPos(mapGird);
    }
}
