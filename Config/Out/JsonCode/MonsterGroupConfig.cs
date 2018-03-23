//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class MonsterGroupVo
{
	public uint Id; // 序号  (key)
	public string Monsters1; // 组怪1
	public string Monsters2; // 组怪2
	public string Monsters3; // 组怪3
}

public class MonsterGroupCFG : BaseCFG
{
	static public Dictionary<string , MonsterGroupVo> items = new Dictionary<string , MonsterGroupVo>();

	static private MonsterGroupCFG _instance = new MonsterGroupCFG();

	static public MonsterGroupCFG Instance
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

			MonsterGroupVo vo = new MonsterGroupVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Monsters1 = (string)data["Monsters1"];
			vo.Monsters2 = (string)data["Monsters2"];
			vo.Monsters3 = (string)data["Monsters3"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
