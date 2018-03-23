//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ElementConflictVo
{
	public string Id; // 序号
}

public class ElementConflictCFG : BaseCFG
{
	static public List<ElementConflictVo> items = new List<ElementConflictVo>();

	static private ElementConflictCFG _instance = new ElementConflictCFG();

	static public ElementConflictCFG Instance
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

			ElementConflictVo vo = new ElementConflictVo();
			vo.Id = (string)data["Id"];
			items.Add(vo);
		}
	}
}
