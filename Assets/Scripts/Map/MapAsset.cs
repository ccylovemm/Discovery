using UnityEngine;
using System.Collections.Generic;

public class MapAsset : ScriptableObject
{
    public int sizeX;
    public int sizeY;
    public Vector2 brithPos;
    public Vector2 transPos;
    public List<int> mapdata = new List<int>();
}
