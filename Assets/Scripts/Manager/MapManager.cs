using UnityEngine;
using System.Collections.Generic;

public class MapManager
{
    static public float textSize = 0.20f;

    static public Vector2 GetGrid(Vector2 pos)
    {
        return GetGrid(pos.x, pos.y);
    }

    static public Vector2 GetGrid(float x, float y)
    {
        int girdX = (int)(x / textSize);
        int girdY = (int)(y / textSize);
        return new Vector2(girdX, girdY);
    }

    static public Vector2 GetPos(Vector2 grid)
    {
        return new Vector2((grid.x + 0.5f) * textSize, (grid.y + 0.5f) * textSize);
    }

    static public Vector2 GetPos(int x, int y)
    {
        return new Vector2((x + 0.5f) * textSize, (y + 0.5f) * textSize);
    }
}