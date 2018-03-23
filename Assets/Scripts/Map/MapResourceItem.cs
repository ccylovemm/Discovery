using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapResourceItem : MonoBehaviour
{
    public bool isPrefab = true;
    public bool isNine = false;
    public bool isPath = true;

    public MapResourceItem replaceItem;
    public MapResourceItem replaceEdgeUp;
    public MapResourceItem replaceEdgeLeft;
    public MapResourceItem replaceEdgeRight;
    public MapResourceItem replaceEdgeSingleUp;
    public MapResourceItem replaceEdgeLeftUp;
    public MapResourceItem replaceEdgeRightUp;
    public MapResourceItem replaceEdgeVer;

    public MapEditorSortLayer layer;
    public MapEditorItemType itemType;
    public MapWorldResource worldResource;

    public List<MapSprite> currList = new List<MapSprite>();

    public List<MapSprite> normalList = new List<MapSprite>();

    public List<MapSprite> out_1 = new List<MapSprite>();
    public List<MapSprite> out_2 = new List<MapSprite>();
    public List<MapSprite> out_3 = new List<MapSprite>();
    public List<MapSprite> out_4 = new List<MapSprite>();

    public List<MapSprite> in_1 = new List<MapSprite>();
    public List<MapSprite> in_2 = new List<MapSprite>();
    public List<MapSprite> in_3 = new List<MapSprite>();
    public List<MapSprite> in_4 = new List<MapSprite>();

    public List<MapSprite> corner1_1 = new List<MapSprite>();
    public List<MapSprite> corner1_2 = new List<MapSprite>();
    public List<MapSprite> corner1_3 = new List<MapSprite>();
    public List<MapSprite> corner1_4 = new List<MapSprite>();


    public List<MapSprite> angle1_1 = new List<MapSprite>();
    public List<MapSprite> angle1_2 = new List<MapSprite>();

    public List<MapSprite> up = new List<MapSprite>();
    public List<MapSprite> left = new List<MapSprite>();
    public List<MapSprite> right = new List<MapSprite>();
    public List<MapSprite> down = new List<MapSprite>();
    public List<MapSprite> center = new List<MapSprite>();

    public List<MapSprite> single = new List<MapSprite>();

    public List<MapSprite> singleUp = new List<MapSprite>();
    public List<MapSprite> singleDown = new List<MapSprite>();
    public List<MapSprite> singleLeft = new List<MapSprite>();
    public List<MapSprite> singleRight = new List<MapSprite>();

    public List<MapSprite> pass1_1 = new List<MapSprite>();
    public List<MapSprite> pass1_2 = new List<MapSprite>();

    public List<MapSprite> pass2_1 = new List<MapSprite>();
    public List<MapSprite> pass2_2 = new List<MapSprite>();
    public List<MapSprite> pass2_3 = new List<MapSprite>();
    public List<MapSprite> pass2_4 = new List<MapSprite>();

    public List<MapSprite> pass3_1 = new List<MapSprite>();
    public List<MapSprite> pass3_2 = new List<MapSprite>();
    public List<MapSprite> pass3_3 = new List<MapSprite>();
    public List<MapSprite> pass3_4 = new List<MapSprite>();

    public List<MapSprite> pass4_1 = new List<MapSprite>();
    public List<MapSprite> pass4_2 = new List<MapSprite>();
    public List<MapSprite> pass4_3 = new List<MapSprite>();
    public List<MapSprite> pass4_4 = new List<MapSprite>();
    public List<MapSprite> pass4_5 = new List<MapSprite>();
    public List<MapSprite> pass4_6 = new List<MapSprite>();
    public List<MapSprite> pass4_7 = new List<MapSprite>();
    public List<MapSprite> pass4_8 = new List<MapSprite>();

    public List<MapSprite> pass5_1 = new List<MapSprite>();
    public List<MapSprite> pass5_2 = new List<MapSprite>();
    public List<MapSprite> pass5_3 = new List<MapSprite>();
    public List<MapSprite> pass5_4 = new List<MapSprite>();

    public List<MapSprite> passAll = new List<MapSprite>();
}

[System.Serializable]
public class MapSprite
{
    public int sRate = 10000;
    public float offsetX;
    public float offsetY;
    public Sprite sprite;
}