//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class AttributeConflictVo
{
	public uint Atribute1; // 攻击方  (key)
	public uint Atribute2; // 受击方  (key)
	public float DamageBonus; // 伤害加成
}

public class AttributeConflictCFG : BaseCFG
{
	static public Dictionary<string , AttributeConflictVo> items = new Dictionary<string , AttributeConflictVo>();

	static private AttributeConflictCFG _instance = new AttributeConflictCFG();

	static public AttributeConflictCFG Instance
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

			AttributeConflictVo vo = new AttributeConflictVo();
			vo.Atribute1 = uint.Parse((string)data["Atribute1"]);
			vo.Atribute2 = uint.Parse((string)data["Atribute2"]);
			vo.DamageBonus = float.Parse((string)data["DamageBonus"]);
			items.Add(vo.Atribute1.ToString() + vo.Atribute2.ToString() , vo);
		}
	}
}
