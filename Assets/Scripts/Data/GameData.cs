using System;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

public class GameData
{
    static public uint gameCoin;

    static public Player myself;
    static public ActorData myData;

    //敌军所有单位
    static public List<ActorObject> enemys = new List<ActorObject>();
}