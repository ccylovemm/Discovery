//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ParameterVo
{
	public uint Id; // 序号  (key)
	public string Value; // 数值
}

public class ParameterCFG : BaseCFG
{
	static public Dictionary<string , ParameterVo> items = new Dictionary<string , ParameterVo>();

	static private ParameterCFG _instance = new ParameterCFG();

	static public ParameterCFG Instance
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

			ParameterVo vo = new ParameterVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Value = (string)data["Value"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
