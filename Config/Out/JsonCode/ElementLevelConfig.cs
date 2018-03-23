//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ElementLevelVo
{
	public uint Id; // 序号  (key)
	public uint Level; // 等级  (key)
	public uint Damage; // 伤害
	public uint CostCoin; // 升级消耗
	public string Desc; // 描述
}

public class ElementLevelCFG : BaseCFG
{
	static public Dictionary<string , ElementLevelVo> items = new Dictionary<string , ElementLevelVo>();

	static private ElementLevelCFG _instance = new ElementLevelCFG();

	static public ElementLevelCFG Instance
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

			ElementLevelVo vo = new ElementLevelVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Level = uint.Parse((string)data["Level"]);
			vo.Damage = uint.Parse((string)data["Damage"]);
			vo.CostCoin = uint.Parse((string)data["CostCoin"]);
			vo.Desc = (string)data["Desc"];
			items.Add(vo.Id.ToString() + vo.Level.ToString() , vo);
		}
	}
}
