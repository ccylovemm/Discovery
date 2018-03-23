//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class AltarVo
{
	public uint Id; // id  (key)
	public string ResName; // 资源
	public string Value; // 元素概率
}

public class AltarCFG : BaseCFG
{
	static public Dictionary<string , AltarVo> items = new Dictionary<string , AltarVo>();

	static private AltarCFG _instance = new AltarCFG();

	static public AltarCFG Instance
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

			AltarVo vo = new AltarVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.ResName = (string)data["ResName"];
			vo.Value = (string)data["Value"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
