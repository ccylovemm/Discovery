﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowFireMagic : BlowMagicBase
{
    private Dictionary<Vector2, float> waterValue = new Dictionary<Vector2, float>();
    private Dictionary<Vector2, float> woodenValue = new Dictionary<Vector2, float>();

    override protected void Update()
    {
        Vector2 grid = MapManager.GetGrid(caster.transform.position);
        for (int i = -4; i < 5; i++)
        {
            for (int j = -4; j < 5; j++)
            {
                Vector2 pos = new Vector2((int)grid.x + i, (int)grid.y + j);
                MeltIce(pos);
                FireWooden(pos);
            }
        }
        base.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        waterValue.Clear();
    }

    private void MeltIce(Vector2 pos)
    {
        if (MapWaterFrezon.frezonPos.ContainsKey(pos))
        {
            Vector2 gridPos = MapManager.GetPos(pos);
            Vector2 temVec = gridPos - (Vector2)caster.currPos;

            float distance = temVec.magnitude;
            if (distance < skillVo.ShotRange * MapManager.textSize)
            {
                float angle = Mathf.Acos(Vector3.Dot(effectDirect.normalized, temVec.normalized)) * Mathf.Rad2Deg;
                if (angle < skillVo.Angle || effectDirect.normalized == (Vector2)temVec.normalized)
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(caster.currPos, temVec, temVec.magnitude, LayerUtil.WallMasks());
                    if (raycastHit2D.collider == null)
                    {
                        if (!waterValue.ContainsKey(pos))
                        {
                            waterValue.Add(pos, 100);
                        }
                        waterValue[pos] -= Mathf.Min(30, 15 * MapManager.textSize / distance);
                        if (waterValue[pos] < 0)
                        {
                            waterValue.Remove(pos);
                            MapWaterFrezon.frezonPos[pos].FireDeFrezon();
                        }
                    }
                }
            }
        }
    }

    private void FireWooden(Vector2 pos)
    {
        if (Wooden.woodenPos.ContainsKey(pos) && !Wooden.fireWoodenPos.ContainsKey(pos))
        {
            Vector2 gridPos = MapManager.GetPos(pos);
            Vector2 temVec = gridPos - (Vector2)caster.currPos;

            float distance = temVec.magnitude;
            if (distance < skillVo.ShotRange * MapManager.textSize)
            {
                float angle = Mathf.Acos(Vector3.Dot(effectDirect.normalized, temVec.normalized)) * Mathf.Rad2Deg;
                if (angle < skillVo.Angle || effectDirect.normalized == (Vector2)temVec.normalized)
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(caster.currPos, temVec, temVec.magnitude, LayerMask.GetMask("Wall", "Obstacle"));
                    if (raycastHit2D.collider == null)
                    {
                        if(!woodenValue.ContainsKey(pos))
                        {
                            woodenValue.Add(pos , 100);
                        }
                        woodenValue[pos] -= Mathf.Min(10, 5 * MapManager.textSize / distance);
                        if (woodenValue[pos] < 0)
                        {
                            woodenValue.Remove(pos);
                            Wooden.woodenPos[pos].Fire();
                        }
                    }
                }
            }
        }
    }
}