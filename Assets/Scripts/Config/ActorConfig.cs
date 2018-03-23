//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ActorVo
{
	public uint Id; // 序号  (key)
	public uint Level; // 等级  (key)
	public uint Type; // 类型
	public uint LevelUp; // 升级需求
	public string Name; // 名字
	public string Desc; // 描述
	public string Characteristic; // 特征
	public uint Mass; // 质量
	public uint Fly; // 飞行
	public string ResName; // 资源路径
	public string Talent; // 天赋
	public string Skills; // 携带技能
	public uint RageMax; // 怒气值
	public uint RageCost; // 怒气消耗
	public float Scale; // 尺寸
	public string Elements; // 初始持有元素
	public string Attribute; // 弱点属性
	public uint MaxHp; // 血量
	public uint Shield; // 护甲
	public uint Attack; // 攻击
	public uint Defence; // 防御
	public float MoveSpeed; // 巡逻速度
	public float MovingSpeed; // 移动速度
	public float AtkMovingSpeed; // 攻击移动速度
	public float AttackInterval; // 攻击间隔
	public string AIRes; // AI资源
	public float PatrolRange; // 巡逻范围
	public float FindRange; // 索敌范围
	public uint Drop; // 掉落
	public uint EmployCoin; // 雇佣消耗金币
	public uint UnlockType; // 解锁类型
	public string UnlockValue; // 解锁参数
}

public class ActorCFG : BaseCFG
{
	static public Dictionary<string , ActorVo> items = new Dictionary<string , ActorVo>();

	static private ActorCFG _instance = new ActorCFG();

	static public ActorCFG Instance
	{
		get
		{
			return _instance;
		}
	}

	override public void Read(string str)
	{
		List<object> jsons = Json.Deserialize(str) as List<object>;
		for (int i = 0; i < jsons.Count; i ++)
		{
			Dictionary<string , object> data = jsons[i] as Dictionary<string , object>;

			ActorVo vo = new ActorVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Level = uint.Parse((string)data["Level"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.LevelUp = uint.Parse((string)data["LevelUp"]);
			vo.Name = (string)data["Name"];
			vo.Desc = (string)data["Desc"];
			vo.Characteristic = (string)data["Characteristic"];
			vo.Mass = uint.Parse((string)data["Mass"]);
			vo.Fly = uint.Parse((string)data["Fly"]);
			vo.ResName = (string)data["ResName"];
			vo.Talent = (string)data["Talent"];
			vo.Skills = (string)data["Skills"];
			vo.RageMax = uint.Parse((string)data["RageMax"]);
			vo.RageCost = uint.Parse((string)data["RageCost"]);
			vo.Scale = float.Parse((string)data["Scale"]);
			vo.Elements = (string)data["Elements"];
			vo.Attribute = (string)data["Attribute"];
			vo.MaxHp = uint.Parse((string)data["MaxHp"]);
			vo.Shield = uint.Parse((string)data["Shield"]);
			vo.Attack = uint.Parse((string)data["Attack"]);
			vo.Defence = uint.Parse((string)data["Defence"]);
			vo.MoveSpeed = float.Parse((string)data["MoveSpeed"]);
			vo.MovingSpeed = float.Parse((string)data["MovingSpeed"]);
			vo.AtkMovingSpeed = float.Parse((string)data["AtkMovingSpeed"]);
			vo.AttackInterval = float.Parse((string)data["AttackInterval"]);
			vo.AIRes = (string)data["AIRes"];
			vo.PatrolRange = float.Parse((string)data["PatrolRange"]);
			vo.FindRange = float.Parse((string)data["FindRange"]);
			vo.Drop = uint.Parse((string)data["Drop"]);
			vo.EmployCoin = uint.Parse((string)data["EmployCoin"]);
			vo.UnlockType = uint.Parse((string)data["UnlockType"]);
			vo.UnlockValue = (string)data["UnlockValue"];
			items.Add(vo.Id.ToString() + vo.Level.ToString() , vo);
		}
	}
}
