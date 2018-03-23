//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class BuffConflictVo
{
	public uint BuffType1; // 新buff类型
	public uint BuffType2; // 旧buff类型
	public uint Type; // 相生相克类型
	public uint BuffId; // 形成BuffId
}

public class BuffConflictCFG : BaseCFG
{
	static public List<BuffConflictVo> items = new List<BuffConflictVo>();

	static private BuffConflictCFG _instance = new BuffConflictCFG();

	static public BuffConflictCFG Instance
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

			BuffConflictVo vo = new BuffConflictVo();
			vo.BuffType1 = uint.Parse((string)data["BuffType1"]);
			vo.BuffType2 = uint.Parse((string)data["BuffType2"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.BuffId = uint.Parse((string)data["BuffId"]);
			items.Add(vo);
		}
	}
}
