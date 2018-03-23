//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class MapStyleVo
{
	public uint Type; // id
	public string ResName; // 资源
}

public class MapStyleCFG : BaseCFG
{
	static public List<MapStyleVo> items = new List<MapStyleVo>();

	static private MapStyleCFG _instance = new MapStyleCFG();

	static public MapStyleCFG Instance
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

			MapStyleVo vo = new MapStyleVo();
			vo.Type = uint.Parse((string)data["Type"]);
			vo.ResName = (string)data["ResName"];
			items.Add(vo);
		}
	}
}
