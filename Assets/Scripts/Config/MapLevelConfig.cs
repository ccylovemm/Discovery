//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class MapLevelVo
{
	public uint World; // 世界
	public uint Type; // 类型
	public uint AreaType; // 区域类型
	public string ResName; // 资源名称
}

public class MapLevelCFG : BaseCFG
{
	static public List<MapLevelVo> items = new List<MapLevelVo>();

	static private MapLevelCFG _instance = new MapLevelCFG();

	static public MapLevelCFG Instance
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

			MapLevelVo vo = new MapLevelVo();
			vo.World = uint.Parse((string)data["World"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.AreaType = uint.Parse((string)data["AreaType"]);
			vo.ResName = (string)data["ResName"];
			items.Add(vo);
		}
	}
}
