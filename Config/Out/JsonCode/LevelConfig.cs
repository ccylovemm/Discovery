//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class LevelVo
{
	public uint MapId; // 地图Id  (key)
	public uint Level; // 关卡  (key)
	public string Desc; // 描述
	public string Reward; // 奖励
	public string SceneDrop; // 随机宝箱
	public uint AltarNum; // 祭坛数量
	public string AltarType; // 祭坛类型
	public string AltarRate; // 祭坛概率
	public uint MapStyle; // 地图库类型
	public string MonsterGroups; // 怪物组
	public string MonsterLevel; // 怪物组类型
	public uint WoodenDrop; // 木箱掉落
}

public class LevelCFG : BaseCFG
{
	static public Dictionary<string , LevelVo> items = new Dictionary<string , LevelVo>();

	static private LevelCFG _instance = new LevelCFG();

	static public LevelCFG Instance
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

			LevelVo vo = new LevelVo();
			vo.MapId = uint.Parse((string)data["MapId"]);
			vo.Level = uint.Parse((string)data["Level"]);
			vo.Desc = (string)data["Desc"];
			vo.Reward = (string)data["Reward"];
			vo.SceneDrop = (string)data["SceneDrop"];
			vo.AltarNum = uint.Parse((string)data["AltarNum"]);
			vo.AltarType = (string)data["AltarType"];
			vo.AltarRate = (string)data["AltarRate"];
			vo.MapStyle = uint.Parse((string)data["MapStyle"]);
			vo.MonsterGroups = (string)data["MonsterGroups"];
			vo.MonsterLevel = (string)data["MonsterLevel"];
			vo.WoodenDrop = uint.Parse((string)data["WoodenDrop"]);
			items.Add(vo.MapId.ToString() + vo.Level.ToString() , vo);
		}
	}
}
