using System;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

public class GameData
{
    static public ActorData myData;
    static public ActorObject myself;
    static public ActorObject employ;

    static public List<uint> elements = new List<uint>();

    //所有单位
    static public List<ActorObject> allUnits = new List<ActorObject>();
    //友军所有单位
    static public List<ActorObject> friends = new List<ActorObject>();
    //敌军所有单位
    static public List<ActorObject> enemys = new List<ActorObject>();
    //敌军Boss单位
    static public List<ActorObject> boss = new List<ActorObject>();
  

    static public List<ActorObject> GetTarget(ActorObject master)
    {
        if ((ActorType)master.actorData.cfgVo.Type == ActorType.Player || (ActorType)master.actorData.cfgVo.Type == ActorType.Pet)
        {
            return enemys;
        }
        return friends;
    }
}