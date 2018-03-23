//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class LanguageVo
{
	public string Id; // 编号  (key)
	public string ContentCN; // 中文
	public string ContentEN; // 英文
}

public class LanguageCFG : BaseCFG
{
	static public Dictionary<string , LanguageVo> items = new Dictionary<string , LanguageVo>();

	static private LanguageCFG _instance = new LanguageCFG();

	static public LanguageCFG Instance
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

			LanguageVo vo = new LanguageVo();
			vo.Id = (string)data["Id"];
			vo.ContentCN = (string)data["ContentCN"];
			vo.ContentEN = (string)data["ContentEN"];
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
