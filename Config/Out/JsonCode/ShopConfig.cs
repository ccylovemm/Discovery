//************************************************************
// Auto Generated Code By ExcelTool
// Copyright(c) Cao ChunYang  All rights reserved.
//************************************************************

using System;
using System.Collections.Generic;
using MiniJSON;

public class ShopVo
{
	public uint Id; // 商品id  (key)
	public uint LinkItems; // 关联道具
	public uint GoodsType; // 商品类型
	public uint CostType; // 货币类型
	public uint GoodsPrice; // 商品价格
	public uint Discount; // 折扣
}

public class ShopCFG : BaseCFG
{
	static public Dictionary<string , ShopVo> items = new Dictionary<string , ShopVo>();

	static private ShopCFG _instance = new ShopCFG();

	static public ShopCFG Instance
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

			ShopVo vo = new ShopVo();
			vo.Id = uint.Parse((string)data["Id"]);
			vo.LinkItems = uint.Parse((string)data["LinkItems"]);
			vo.GoodsType = uint.Parse((string)data["GoodsType"]);
			vo.CostType = uint.Parse((string)data["CostType"]);
			vo.GoodsPrice = uint.Parse((string)data["GoodsPrice"]);
			vo.Discount = uint.Parse((string)data["Discount"]);
			items.Add(vo.Id.ToString() , vo);
		}
	}
}
