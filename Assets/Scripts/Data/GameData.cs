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
  
    //上线初始化 切换角色时刷新数据
    static public void FreshData(bool fresh = false) //fresh = true 切换角色时
    {
        myData = new ActorData(DataManager.userData.ActorId);
        if (fresh || DataManager.userData.IsDead)
        {
            DataManager.userData.Clear();
        }
        else
        {
            if (DataManager.userData.Hp != 0)
            {
                myData.currHp = DataManager.userData.Hp;
            }
            else
            {
                DataManager.userData.Hp = myData.currHp;
            }

            if (DataManager.userData.Elements.Count != 0)
            {
                myData.elements = DataManager.userData.Elements;
            }
            else
            {
                DataManager.userData.Elements = myData.elements;
            }
        }
        SkillManager.FreshSkillLevel();
        SkillManager.LoadAllSkill();
    }

    static public List<ActorObject> GetTarget(ActorObject master)
    {
        if ((ActorType)master.actorData.cfgVo.Type == ActorType.Player || (ActorType)master.actorData.cfgVo.Type == ActorType.Pet)
        {
            return enemys;
        }
        return friends;
    }
}