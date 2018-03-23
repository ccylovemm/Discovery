using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMagicBase : MagicBase
{
    protected List<ParticleSystem> lightnings = new List<ParticleSystem>();
    protected List<Vector2> linePosList = new List<Vector2>();
    protected List<ActorObject> findTargets = new List<ActorObject>();

    override protected void Update()
    {
        base.Update();

        if (Time.time > damageTime)
        {
            damageTime = Time.time + skillVo.Interval;
            FindEnemy(effectDirect);
            DoDamage();
        }
        UpdateEffect();
    }

    protected void FindEnemy(Vector2 direct)
    {
        findTargets.Clear();
        Vector3 srcPos = caster.currPos;
        findTargets.Add(caster);
        bool isCaster = true;
        while (true)
        {
            ActorObject target = GetTarget(srcPos, direct, isCaster);
            isCaster = false;
            if (target == null) break;
            findTargets.Add(target);
            Vector3 targetPos = target.currPos;
            direct = targetPos - srcPos;
            srcPos = targetPos;
        }
    }

    virtual protected void UpdateEffect()
    {
        int i = 0;
        if (findTargets.Count < 2)
        {
            if (i >= lightnings.Count)
            {
                lightnings.Add(GameObject.Instantiate(effectPrefab).GetComponentInChildren<ParticleSystem>());
            }
            lightnings[i].gameObject.SetActive(true);
            lightnings[i].transform.position = caster.attackPos.position + (Vector3)effectDirect.normalized * 0.04f;
            ParticleSystem.MainModule main = lightnings[i].main;
            main.startRotation = (Mathf.PI / 180) * (effectDirect.y < 0 ? (360 - Vector2.Angle(effectDirect, Vector2.left)) : Vector2.Angle(effectDirect, Vector2.left));
            i++;
        }
        else
        {
            for (i = 0; i < findTargets.Count - 1; i++)
            {
                if (i >= lightnings.Count)
                {
                    lightnings.Add(GameObject.Instantiate(effectPrefab).GetComponentInChildren<ParticleSystem>());
                }
                lightnings[i].gameObject.SetActive(true);

                if (i == 0)
                {
                    effectDirect = findTargets[i + 1].currPos - findTargets[i].attackPos.position;
                    lightnings[i].transform.position = findTargets[i].attackPos.position + (Vector3)effectDirect.normalized * 0.04f;
                }
                else
                {
                    effectDirect = findTargets[i + 1].currPos - findTargets[i].currPos;
                    lightnings[i].transform.position = findTargets[i].currPos;
                }
                if (hitEffect != null)
                {
                    GameObject.Destroy(GameObject.Instantiate(hitEffect, findTargets[i + 1].transform), 1.5f);
                }
                ParticleSystem.MainModule main = lightnings[i].main;
                main.startRotation = (Mathf.PI / 180) * (effectDirect.y < 0 ? (360 - Vector2.Angle(effectDirect, Vector2.left)) : Vector2.Angle(effectDirect, Vector2.left));
            }
        }
        for(; i < lightnings.Count; i ++)
        {
            lightnings[i].gameObject.SetActive(false);
        }

        shotEffect_.transform.position = caster.attackPos.position + (Vector3)effectDirect.normalized * 0.04f;
    }

    protected void DoDamage()
    {
        for (int i = 0; i < findTargets.Count; i++)
        {
            if (findTargets[i] == null || findTargets[i].IsDead || findTargets[i] == caster) continue;
            findTargets[i].ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , findTargets[i].currPos - caster.currPos);
        }
    }

    public override void MagicDestory()
    {
        base.MagicDestory();
        for (int i = 0; i < lightnings.Count; i ++)
        {
            GameObject.Destroy(lightnings[i].gameObject);
        }
    }

    protected ActorObject GetTarget(Vector2 srcPos, Vector2 norVec, bool isCaster = false)
    {
        ActorObject target = null;

        float d = float.MaxValue;
        float shotRange = (isCaster ? skillVo.ShotRange : skillVo.SkillValue) * MapManager.textSize;
        int count = GameData.allUnits.Count;
        for (int i = 0; i < count; i++)
        {
            if (findTargets.Contains(GameData.allUnits[i])) continue;
            Vector2 dir = (Vector2)GameData.allUnits[i].currPos - srcPos;
            float distance = dir.magnitude;
            if (distance < shotRange)
            {
                float angle = Mathf.Acos(Vector2.Dot(norVec.normalized, dir.normalized)) * Mathf.Rad2Deg;
                if (angle < skillVo.Angle || norVec.normalized == dir.normalized)
                {
                    if (distance < d)
                    {
                        RaycastHit2D raycastHit2D = Physics2D.Raycast(srcPos, (Vector2)GameData.allUnits[i].currPos - srcPos, distance , LayerUtil.WallMasks());
                        if (raycastHit2D.collider == null)
                        {
                            d = distance;
                            target = GameData.allUnits[i];
                        }
                    }
                }
            }
        }
        return target;
    }
}

