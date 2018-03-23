//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class SkillVo
{
	public string Command; // 元素组合  (key)
	public uint ComboType; // 组合类型
	public uint SkillId; // 技能
}

public class SkillCFG : BaseCFG
{
	static public Dictionary<string , SkillVo> items = new Dictionary<string , SkillVo>();

	static private SkillCFG _instance = new SkillCFG();

	static public SkillCFG Instance
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

			SkillVo vo = new SkillVo();
			vo.Command = (string)data["Command"];
			vo.ComboType = uint.Parse((string)data["ComboType"]);
			vo.SkillId = uint.Parse((string)data["SkillId"]);
			items.Add(vo.Command.ToString() , vo);
		}
	}
}
