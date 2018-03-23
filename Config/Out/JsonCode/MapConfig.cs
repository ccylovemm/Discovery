//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class MapVo
{
	public uint Id; // id  (key)
	public uint SizeX; // 地图宽度
	public uint SizeY; // 地图高度
	public string Desc; // 描述
}

public class MapCFG : BaseCFG
{
	static public Dictionary<string , MapVo> items = new Dictionary<string , MapVo>();

	static private MapCFG _instance = new MapCFG();

	static public MapCFG Instance
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

			MapVo vo = new MapVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.SizeX = uint.Parse((string)data["SizeX"]);
			vo.SizeY = uint.Parse((string)data["SizeY"]);
			vo.Desc = (string)data["Desc"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
