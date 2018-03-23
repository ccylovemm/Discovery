using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowColdMagic : BlowMagicBase
{
    public GameObject terrainFrezonEffect;

    private Dictionary<Vector2, float> waterValue = new Dictionary<Vector2, float>();

    override protected void Update()
    {
        Vector2 grid = MapManager.GetGrid(caster.transform.position);
        for (int i = -4; i < 5; i++)
        {
            for (int j = -4; j < 5; j++)
            {
                Vector2 pos = new Vector2((int)grid.x + i, (int)grid.y + j);
                FrezonWater(pos);
            }
        }
        base.Update();
    }

    private void FrezonWater(Vector2 pos)
    {
        Vector2 gridPos = MapManager.GetPos(pos);
        if (SceneManager.Instance.TerrainIsWater(gridPos))
        {
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
                        MapWaterFrezon mapWaterFrezon = null;
                        if (MapWaterFrezon.frezonPos.TryGetValue(pos, out mapWaterFrezon))
                        {
                            mapWaterFrezon.ReFrezon();
                        }
                        else
                        {
                            if (!waterValue.ContainsKey(pos))
                            {
                                waterValue.Add(pos, 0);
                            }
                            waterValue[pos] += Mathf.Min(15, 8 * MapManager.textSize / distance);
                            if (waterValue[pos] > 100)
                            {
                                waterValue.Remove(pos);
                                mapWaterFrezon = GameObject.Instantiate(terrainFrezonEffect, MapManager.GetPos(pos), Quaternion.identity).GetComponent<MapWaterFrezon>();
                                MapWaterFrezon.frezonPos.Add(pos, mapWaterFrezon);
                            }
                        }
                    }
                }
            }
        }
    }
}
