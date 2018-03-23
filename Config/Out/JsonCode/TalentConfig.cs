//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class TalentVo
{
	public uint Id; // 天赋Id  (key)
	public uint Type; // 天赋类型
	public string Name; // 天赋名字
	public string Icon; // 天赋图标
	public float ParamValue; // 参数值
	public uint Rate; // 概率
	public string Desc; // 天赋描述
}

public class TalentCFG : BaseCFG
{
	static public Dictionary<string , TalentVo> items = new Dictionary<string , TalentVo>();

	static private TalentCFG _instance = new TalentCFG();

	static public TalentCFG Instance
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

			TalentVo vo = new TalentVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.Name = (string)data["Name"];
			vo.Icon = (string)data["Icon"];
			vo.ParamValue = float.Parse((string)data["ParamValue"]);
			vo.Rate = uint.Parse((string)data["Rate"]);
			vo.Desc = (string)data["Desc"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
