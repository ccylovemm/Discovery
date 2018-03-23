using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMagicBase : MagicBase
{
    protected override void Start()
    {
        base.Start();
        effectDisplay.transform.position = caster.transform.position;
        GameObject.Instantiate(effectPrefab, effectDisplay.transform);
        GameObject.Destroy(effectDisplay , skillVo.Duration);
        DoDamage();
    }

    private void DoDamage()
    {
        for (int i = 0; i < GameData.allUnits.Count; i++)
        {
            if (GameData.allUnits[i] == null || GameData.allUnits[i] == caster || GameData.allUnits[i].IsDead || GameData.allUnits[i].IsDisappear) continue;
            Vector2 distance = GameData.allUnits[i].transform.position - caster.transform.position;
            if (distance.magnitude < skillVo.DamageRange * MapManager.textSize)
            {
                GameData.allUnits[i].ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , distance);
            }
        }
    }
}
