using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World1_BossBumpMagic : MagicBase
{
    public GameObject terrainMudEffect;
    public CircleCollider2D circleCollider;

    private Dictionary<Vector2, float> mudValue = new Dictionary<Vector2, float>();
    private List<ActorObject> list = new List<ActorObject>();

    override protected void Start()
    {
        circleCollider.radius = caster.circleCollider2D.radius;
        circleCollider.offset = caster.circleCollider2D.offset;

        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < SceneManager.Instance.mapRoom.mapInfo.randonMonsterPos.Count; i++)
        {
            list.Add(SceneManager.Instance.mapRoom.mapInfo.randonMonsterPos[i] + new Vector2(SceneManager.Instance.mapRoom.mapInfo.offsetX, SceneManager.Instance.mapRoom.mapInfo.offsetY));
        }
        Vector2 grid = MapManager.GetGrid(caster.transform.position);
        list.Remove(grid);
        Vector2 direct = GameData.myself.currPos - caster.currPos;
        float minAngle = float.MaxValue;
        Vector2 targetPos = Vector2.zero;
        for (int i = 0; i < list.Count; i ++)
        {
            Vector3 temVec = list[i] - grid;
            float angle = Mathf.Acos(Vector3.Dot(direct.normalized, temVec.normalized)) * Mathf.Rad2Deg;
            if (minAngle > angle)
            {
                minAngle = angle;
                targetPos = list[i];
            }
        }
        caster.movement.MoveTo(MapManager.GetPos(targetPos));
    }

    override protected void Update()
    {
        transform.position = caster.transform.position;
        if (caster.movement.arrive)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            Vector2 grid = MapManager.GetGrid(caster.transform.position);
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    Vector2 pos = new Vector2((int)grid.x + i, (int)grid.y + j);
                    Vector2 gridPos = MapManager.GetPos(pos);
                    Vector2 temVec = gridPos - (Vector2)caster.currPos;
                    float distance = temVec.magnitude;
                    if (distance < skillVo.ShotRange * MapManager.textSize)
                    {
                        MapMud mapMud = null;
                        if (MapMud.mudPos.TryGetValue(pos, out mapMud))
                        {
                            mapMud.ReMud();
                        }
                        else
                        {
                            if (!mudValue.ContainsKey(pos))
                            {
                                mudValue.Add(pos, 0);
                            }
                            mudValue[pos] += Mathf.Min(15, 8 * MapManager.textSize / distance);
                            if (mudValue[pos] > 100)
                            {
                                mudValue.Remove(pos);
                                mapMud = GameObject.Instantiate(terrainMudEffect, MapManager.GetPos(pos), Quaternion.identity).GetComponent<MapMud>();
                                MapMud.mudPos.Add(pos, mapMud);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null && actorObject != caster && !list.Contains(actorObject))
        {
            list.Add(actorObject);
            actorObject.ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff, actorObject.currPos - caster.currPos);
        }
    }
}
