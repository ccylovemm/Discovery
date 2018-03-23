//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ItemVo
{
	public uint Id; // id  (key)
	public uint Name; // 名称
	public uint Description; // 描述
	public uint Type; // 类型1
	public uint SubType; // 类型2
	public string ItemResources; // 资源
	public string ItemIcon; // 图标
	public string ParamValue; // 参数
}

public class ItemCFG : BaseCFG
{
	static public Dictionary<string , ItemVo> items = new Dictionary<string , ItemVo>();

	static private ItemCFG _instance = new ItemCFG();

	static public ItemCFG Instance
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

			ItemVo vo = new ItemVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Name = uint.Parse((string)data["Name"]);
			vo.Description = uint.Parse((string)data["Description"]);
			vo.Type = uint.Parse((string)data["Type"]);
			vo.SubType = uint.Parse((string)data["SubType"]);
			vo.ItemResources = (string)data["ItemResources"];
			vo.ItemIcon = (string)data["ItemIcon"];
			vo.ParamValue = (string)data["ParamValue"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
