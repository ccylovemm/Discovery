using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorType
{
    Player = 1,
    Monster = 2,
    MonsterHard = 3,
    Boss = 4,
    Pet = 5,
}

public enum LevelResultEnum
{
    Victory,
    Fail,
}

public enum MapGridType
{
    Walkable,
    Notwalkable
}

public enum MapSceneItemType
{
    None = 0,
    SceneItem = 2,
    Drop = 3,
}

public enum BuffType
{
    None = 0,
    Damage = 1,
    Wet = 2,
    Cold = 3,
    Frozen = 4,
    Speed = 5,
    Steam = 6,
    DeFrozen = 7,
    FireResist = 8,
    WaterResist = 9,
    EarthResist = 10,
    ElectricResist = 11,
    ColdResist = 12,
    HitIce = 13,
    AttackIce = 14,
    Shock = 15,
    Drug = 16,
    Dizzy = 17,
    Chaos = 18,
    Muddy = 19, //泥泞
    HitBack = 20,
    HitBreak = 21,
    HitNud = 22,
}

public enum SkillType
{
    Lighting = 1,
    Blow = 2,
    Line = 3,
    Cast = 4,
    Shield = 5,
    Bomb = 6,
    Aoe = 7,
    Strike = 8,
    Defend = 9,
    Arrow = 10,
    Pet = 11,
    RangeDamage =12,
    Speed = 13,
    SelfLife = 14,
    NearAttack = 101,
    FarAttack = 102,
    Bump = 103,
}

public enum SceneEventType
{
    None = 0,
    PickUp = 1,
    NpcEvent = 2,
    EnterMap = 3,
    AltarEvent = 4,
}

public enum NpcEventType
{
    None = 0,
    UnLockAvatar = 1,
    UpgradeAvatar = 2,
}

public enum ItemType
{
    Money = 1,
    Element = 2,
    Item = 3,
    Decoration = 4,
}

public enum ItemSubType
{
    AddHp = 1,
    AddAnger = 2,
    DeBuff = 3,
}

public enum SkillElement
{
    ZeroElement = 0,
    OneElement = 1,
    TwoElement = 2,
    ThreeElement = 3,
    FourElement = 4,
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

public enum MapRoomType
{
    Birth = 1,
    Level = 2,
    Trans = 3,
    BossLevel = 4,
}

public enum SceneType
{
    Home,
    Level,
    Boss,
}

public enum MassType
{
    Low = 1,
    Middle = 2,
    High = 3,
}

public enum Platform
{
    GooglePlay,
}

public enum DecorationType
{
    AttackAttrDamage = 1,
    AttackStatusDamage = 2,
    AttackStatusTimeAdd = 3,
    SelfStatusTimeReduce = 4,
    KillTargetAddBlood = 5,
    UseBloodAdd = 6,
}

public enum TalentType
{
    ReduceDamage,
    AddCastDamage,
    KillBuff,
    AngerAddSpeed,
    AttrDamageAdd,
}

public enum CastType
{
    Instant = 1,
    MouseUp = 2,
    TimeOver = 3,
}


