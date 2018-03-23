using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : MonoBehaviour
{
    public bool isBomb = true;
    public float speed = 0.3f;
    public GameObject effectPrefab;
    public GameObject hitEffectPrefab;
    public GameObject hitEffectPrefab2;

    private Vector2 direct;
    private Vector3 castPos;
    private SkillLevelVo skillVo;
    private ActorObject caster;

    private float addPower;
    private float maxDistance;

    private void Awake()
    {
        GameObject.Instantiate(effectPrefab , transform);
        GameObject.Destroy(gameObject , 2.0f);
    }

    private void Update()
    {
        transform.position += (Vector3)direct.normalized * speed;
        if (Vector3.Distance(castPos, transform.position) > maxDistance * MapManager.textSize) 
        {
            if (SceneManager.Instance.TerrainIsHole(transform.position))
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                Bomb(null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActorObject actorObject = collision.gameObject.GetComponent<ActorObject>();

        if(actorObject != null && (actorObject.IsDead ||  actorObject == caster)) return;

        if (LayerUtil.IsDamageLayer(collision.gameObject.layer))
        {
            Bomb(actorObject);
        }
    }

    public void SetData(ActorObject actor , SkillLevelVo vo , Vector3 pos, Vector2 dir , float power = 0)
    {
        caster = actor;
        castPos = pos;
        skillVo = vo;
        direct = dir;
        addPower = power;

        maxDistance = skillVo.ShotRange + skillVo.ChargeRange * addPower;


        if (dir.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, Vector2.Angle(dir.normalized, Vector2.up));
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(dir.normalized, Vector2.up));
        }
    }

    private void Bomb(ActorObject actorObject)
    {
        if (hitEffectPrefab2 != null) GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab2, transform.position, Quaternion.identity), 3);
        if (hitEffectPrefab != null) GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab, transform.position, Quaternion.identity), 3);
        if (isBomb)
        {
            DamageTarget();
        }
        else
        {
            if (actorObject != null)
            {
                actorObject.ReduceHpFormCastMagic(caster , (uint)(skillVo.BaseDamage + addPower * skillVo.ChargeDamage) , skillVo.AttachElement, skillVo.Buff , direct.normalized * (1 + addPower));
            }
        }
        GameObject.Destroy(gameObject);
    }

    private void DamageTarget()
    {
        for (int i = GameData.allUnits.Count - 1; i >= 0; i--)
        {
            if (GameData.allUnits[i] == null || GameData.allUnits[i].IsDead || GameData.allUnits[i].IsDisappear) continue;
            Vector3 distance = GameData.allUnits[i].currPos - transform.position;
            if (distance.magnitude - GameData.allUnits[i].circleCollider2D.radius < skillVo.DamageRange * MapManager.textSize)
            {
                GameData.allUnits[i].ReduceHpFormCastMagic(caster ,(uint)(skillVo.BaseDamage + addPower * skillVo.ChargeDamage) , skillVo.AttachElement, skillVo.Buff , distance.normalized * (1 + addPower));
            }
        }

        Vector2 grid = MapManager.GetGrid(transform.position);
        for (int i = -4; i < 5; i++)
        {
            for (int j = -4; j < 5; j++)
            {
                Vector2 pos = new Vector2((int)grid.x + i, (int)grid.y + j);
                SmashWooden(pos);
            }
        }
    }

    private void SmashWooden(Vector2 pos)
    {
        if (Wooden.woodenPos.ContainsKey(pos))
        {
            Vector2 gridPos = MapManager.GetPos(pos);
            if (((Vector2)transform.position - gridPos).magnitude < skillVo.DamageRange * MapManager.textSize)
            {
                Wooden.woodenPos[pos].Smash();
            }
        }
    }
}
