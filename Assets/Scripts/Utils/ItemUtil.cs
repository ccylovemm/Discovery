using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUtil
{
    static public void CostItem(uint id , uint num)
    {
        if (id == 1)
        {
            GameData.gameCoin -= num;
        }
    }

    static public uint GetItemNum(uint id)
    {
        uint num = 0;
        if (id == 1)
        {
            num = GameData.gameCoin;
        }
        return num;
    }
}
