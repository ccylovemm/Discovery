//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class DropVo
{
	public uint Id; // 编号  (key)
	public uint Type1; // 获得方式
	public uint Type2; // 随机方式
	public string ResName; // 地图掉落
	public string Reward; // 掉落
	public string Num; // 数量
	public string Rate; // 区间
}

public class DropCFG : BaseCFG
{
	static public Dictionary<string , DropVo> items = new Dictionary<string , DropVo>();

	static private DropCFG _instance = new DropCFG();

	static public DropCFG Instance
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

			DropVo vo = new DropVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Type1 = uint.Parse((string)data["Type1"]);
			vo.Type2 = uint.Parse((string)data["Type2"]);
			vo.ResName = (string)data["ResName"];
			vo.Reward = (string)data["Reward"];
			vo.Num = (string)data["Num"];
			vo.Rate = (string)data["Rate"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
