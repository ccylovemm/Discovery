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
		files["Altar"] = AltarCFG.Instance;
		files["AttributeConflict"] = AttributeConflictCFG.Instance;
		files["Buff"] = BuffCFG.Instance;
		files["BuffConflict"] = BuffConflictCFG.Instance;
		files["Decoration"] = DecorationCFG.Instance;
		files["Drop"] = DropCFG.Instance;
		files["ElementConflict"] = ElementConflictCFG.Instance;
		files["ElementLevel"] = ElementLevelCFG.Instance;
		files["Item"] = ItemCFG.Instance;
		files["Language"] = LanguageCFG.Instance;
		files["Level"] = LevelCFG.Instance;
		files["Map"] = MapCFG.Instance;
		files["MapLevel"] = MapLevelCFG.Instance;
		files["MapStyle"] = MapStyleCFG.Instance;
		files["MonsterGroup"] = MonsterGroupCFG.Instance;
		files["Npc"] = NpcCFG.Instance;
		files["Parameter"] = ParameterCFG.Instance;
		files["Shop"] = ShopCFG.Instance;
		files["Skill"] = SkillCFG.Instance;
		files["SkillLevel"] = SkillLevelCFG.Instance;
		files["Talent"] = TalentCFG.Instance;
	}
}
