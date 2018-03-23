//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class DecorationVo
{
	public uint Id; // 饰品Id  (key)
	public uint Level; // 等级  (key)
	public uint CostCoin; // 升级消耗金币
	public uint Type; // 类型
	public uint Attr; // 属性
	public float Parameter; // 参数值
	public uint Description; // 描述
}

public class DecorationCFG : BaseCFG
{
	static public Dictionary<string , DecorationVo> items = new Dictionary<string , DecorationVo>();

	static private DecorationCFG _instance = new DecorationCFG();

	static public DecorationCFG Instance
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

			DecorationVo vo = new DecorationVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Level = uint.Parse((string)data["Level"]);
			vo.CostCoin = uint.Parse((string)data["CostCoin"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.Attr = uint.Parse((string)data["Attr"]);
			vo.Parameter = float.Parse((string)data["Parameter"]);
			vo.Description = uint.Parse((string)data["Description"]);
			items.Add(vo.Id.ToString() + vo.Level.ToString() , vo);
		}
	}
}
