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
	public uint Type; // 类型
	public string ResName; // 资源路径
	public string Skills; // 携带技能
	public uint MaxHp; // 血量
	public uint Attack; // 攻击
	public uint Defence; // 防御
	public float MoveSpeed; // 移动速度
	public float AttackInterval; // 攻击间隔
	public float AttackDistance; // 攻击距离
	public float FindRange; // 索敌范围
	public float PatrolRange; // 巡逻范围
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
			vo.Type = uint.Parse((string)data["Type"]);
			vo.ResName = (string)data["ResName"];
			vo.Skills = (string)data["Skills"];
			vo.MaxHp = uint.Parse((string)data["MaxHp"]);
			vo.Attack = uint.Parse((string)data["Attack"]);
			vo.Defence = uint.Parse((string)data["Defence"]);
			vo.MoveSpeed = float.Parse((string)data["MoveSpeed"]);
			vo.AttackInterval = float.Parse((string)data["AttackInterval"]);
			vo.AttackDistance = float.Parse((string)data["AttackDistance"]);
			vo.FindRange = float.Parse((string)data["FindRange"]);
			vo.PatrolRange = float.Parse((string)data["PatrolRange"]);
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
