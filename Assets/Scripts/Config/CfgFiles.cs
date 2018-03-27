//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************
using System;
using System.Collections.Generic;

public class CfgFiles{
	static public Dictionary<String , BaseCFG> files = new Dictionary<String,BaseCFG>();

	static public void Init()
	{
		files["Actor"] = ActorCFG.Instance;
		files["Item"] = ItemCFG.Instance;
		files["Language"] = LanguageCFG.Instance;
		files["Npc"] = NpcCFG.Instance;
		files["Parameter"] = ParameterCFG.Instance;
		files["SkillLevel"] = SkillLevelCFG.Instance;
	}
}
