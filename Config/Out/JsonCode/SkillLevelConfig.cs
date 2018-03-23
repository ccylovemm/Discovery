//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class SkillLevelVo
{
	public uint Id; // 序号  (key)
	public uint Level; // 等级  (key)
	public string Name; // 技能名
	public string Description; // 技能描述
	public string LvDescription; // 升级描述
	public uint Type; // 技能类型
	public uint ComboType; // 组合类型
	public uint CastType; // 施法方式
	public string Icon; // 技能图标
	public string ResName; // 技能
	public float CD; // 冷却时间
	public float Distance; // 技能出手距离
	public float ShotRange; // 射程
	public float ChargeRange; // 蓄力追加射程
	public float DamageRange; // 伤害范围
	public float SkillValue; // 技能参数
	public uint BaseDamage; // 基础伤害
	public uint ChargeDamage; // 蓄力追加伤害
	public float ChargeTime; // 最大蓄力时间
	public float Angle; // 角度
	public float Interval; // 间隔
	public float Duration; // 持续
	public string AttachElement; // 附加元素
	public string Buff; // 击中buff
	public string CastEffect; // 出手特效
	public string HitEffect; // 击中特效
	public string MusicStart; // 出手音效
	public string MusicLoop; // 持续音效
	public string MusicOver; // 结束音效
}

public class SkillLevelCFG : BaseCFG
{
	static public Dictionary<string , SkillLevelVo> items = new Dictionary<string , SkillLevelVo>();

	static private SkillLevelCFG _instance = new SkillLevelCFG();

	static public SkillLevelCFG Instance
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

			SkillLevelVo vo = new SkillLevelVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.Level = uint.Parse((string)data["Level"]);
			vo.Name = (string)data["Name"];
			vo.Description = (string)data["Description"];
			vo.LvDescription = (string)data["LvDescription"];
			vo.Type = uint.Parse((string)data["Type"]);
			vo.ComboType = uint.Parse((string)data["ComboType"]);
			vo.CastType = uint.Parse((string)data["CastType"]);
			vo.Icon = (string)data["Icon"];
			vo.ResName = (string)data["ResName"];
			vo.CD = float.Parse((string)data["CD"]);
			vo.Distance = float.Parse((string)data["Distance"]);
			vo.ShotRange = float.Parse((string)data["ShotRange"]);
			vo.ChargeRange = float.Parse((string)data["ChargeRange"]);
			vo.DamageRange = float.Parse((string)data["DamageRange"]);
			vo.SkillValue = float.Parse((string)data["SkillValue"]);
			vo.BaseDamage = uint.Parse((string)data["BaseDamage"]);
			vo.ChargeDamage = uint.Parse((string)data["ChargeDamage"]);
			vo.ChargeTime = float.Parse((string)data["ChargeTime"]);
			vo.Angle = float.Parse((string)data["Angle"]);
			vo.Interval = float.Parse((string)data["Interval"]);
			vo.Duration = float.Parse((string)data["Duration"]);
			vo.AttachElement = (string)data["AttachElement"];
			vo.Buff = (string)data["Buff"];
			vo.CastEffect = (string)data["CastEffect"];
			vo.HitEffect = (string)data["HitEffect"];
			vo.MusicStart = (string)data["MusicStart"];
			vo.MusicLoop = (string)data["MusicLoop"];
			vo.MusicOver = (string)data["MusicOver"];
			items.Add(vo.Id.ToString() + vo.Level.ToString() , vo);
		}
	}
}
