using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUtil
{
    static public void UseItem(uint id)
    {
        ItemVo itemVo = ItemCFG.items[id.ToString()];
        if ((ItemType)itemVo.Type == ItemType.Item)
        {
            if ((ItemSubType)itemVo.SubType == ItemSubType.AddHp)
            {
                uint hpNum = uint.Parse(ItemCFG.items[id.ToString()].ParamValue);
                // 饰品 击杀 回复血量
                if (GameData.myself.actorData.decorations.Count > 0)
                {
                    int count = GameData.myself.actorData.decorations.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DecorationVo decorationVo = GameData.myself.actorData.decorations[i];
                        if ((DecorationType)decorationVo.Type == DecorationType.UseBloodAdd)
                        {
                            hpNum += (uint)(hpNum * decorationVo.Parameter);
                        }
                    }
                }
                GameData.myself.AddHp(hpNum);
            }
            else if ((ItemSubType)itemVo.SubType == ItemSubType.DeBuff)
            {
                string[] buffs = itemVo.ParamValue.Split(',');
                for (int i = 0; i < buffs.Length; i ++)
                {
                    GameData.myself.DeleteBuff(uint.Parse(buffs[i]));
                }
            }
        }
    }

    static public void CostItem(uint id , int num)
    {
        if (id == 1)
        {
            DataManager.userData.GoldCoin -= num;
        }
        else if (id == 2)
        {
            DataManager.userData.Diamond -= num;
        }
    }

    static public void GetItem(uint id)
    {
        if (DataManager.userData.CarryId != 0)
        {
            SceneManager.Instance.RandomDropItem(DataManager.userData.CarryId, 1, GameData.myself.transform.position);
        }
        DataManager.userData.CarryId = id;
        EventCenter.DispatchEvent(EventEnum.UpdateCarryItem);
    }

    static public void GetDecoration(uint id)
    {
        if (DataManager.userData.Decorations.Contains(id))
        {
            SceneManager.Instance.RandomDropItem(id , 1, GameData.myself.transform.position);
            return;
        }
        DataManager.userData.SetDecoration(id);
        int level = DataManager.userData.GetDecorationLevel(id);
        if (level <= 1)
        {
            DataManager.userData.SetDecorationLevel(id , 1);
        }
        GameData.myData.FreshDecorations();
    }

    static public uint GetItemNum(uint id)
    {
        int num = 0;
        if (id == 1)
        {
            num = DataManager.userData.GoldCoin;
        }
        else if (id == 2)
        {
            num = DataManager.userData.Diamond;
        }
        return (uint)num;
    }
}
