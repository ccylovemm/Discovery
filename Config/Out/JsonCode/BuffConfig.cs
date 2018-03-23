//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class BuffVo
{
	public uint Id; // 序号  (key)
	public uint Group; // BUFF组
	public uint Type; // 类型
	public uint DmgType; // 伤害类型
	public string Name; // 名字
	public string Display; // 表现
	public float Duration; // 持续时间
	public float Interval; // 间隔时间
	public float Value; // 数值
	public uint ParamValue; // 额外的参数
}

public class BuffCFG : BaseCFG
{
	static public Dictionary<string , BuffVo> items = new Dictionary<string , BuffVo>();

	static private BuffCFG _instance = new BuffCFG();

	static public BuffCFG Instance
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

			BuffVo vo = new BuffVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Group = uint.Parse((string)data["Group"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.DmgType = uint.Parse((string)data["DmgType"]);
			vo.Name = (string)data["Name"];
			vo.Display = (string)data["Display"];
			vo.Duration = float.Parse((string)data["Duration"]);
			vo.Interval = float.Parse((string)data["Interval"]);
			vo.Value = float.Parse((string)data["Value"]);
			vo.ParamValue = uint.Parse((string)data["ParamValue"]);
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
