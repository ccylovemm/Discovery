//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class NpcVo
{
	public uint Id; // 序号  (key)
	public string Icon; // 头像
	public string Name; // 名字
	public string Bubble; // 气泡内容
	public string Dialogue; // 对话详情
	public uint EventType; // 事件类型
	public string EventValue; // 事件参数
}

public class NpcCFG : BaseCFG
{
	static public Dictionary<string , NpcVo> items = new Dictionary<string , NpcVo>();

	static private NpcCFG _instance = new NpcCFG();

	static public NpcCFG Instance
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

			NpcVo vo = new NpcVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Icon = (string)data["Icon"];
			vo.Name = (string)data["Name"];
			vo.Bubble = (string)data["Bubble"];
			vo.Dialogue = (string)data["Dialogue"];
			vo.EventType = uint.Parse((string)data["EventType"]);
			vo.EventValue = (string)data["EventValue"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
