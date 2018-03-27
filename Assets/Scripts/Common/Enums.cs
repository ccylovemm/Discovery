using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType
{
    Player = 1,
    Monster = 2,
}
public enum SceneEventType
{
    None = 0,
    PickUp = 1,
    NpcEvent = 2,
}

public enum NpcEventType
{
    None = 0,
}

public enum ItemType
{
    Money = 1,
    Item = 2,
}

public enum WorldType
{
    None = 0,
    World1 = 1,
    World2 = 2,
    World3 = 3,
}

public enum MapEditorSortLayer
{
    Floor1,
    Floor2,
    Floor3,
}

public enum MapEditorItemType
{
    None,
    Wall,
    Water,
    DeepWater,
    Hole,
    Edge,
    StoneWall,
    Lava,
    Mud,
    Deco,
}

public enum ActionStatus
{
    Idle,
    Back,
    Follow,
    Attack,
}


