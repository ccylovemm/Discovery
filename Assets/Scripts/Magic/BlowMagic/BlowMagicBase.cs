using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowMagicBase : MagicBase
{
    public float delayDamageTime = 0;

    private ParticleSystem[] particles;

    override protected void Awake()
    {
        base.Awake();
        damageTime = Time.time + delayDamageTime;
    }

    protected override void Start()
    {
        base.Start();

        particles = GameObject.Instantiate(effectPrefab, effectDisplay.transform).GetComponentsInChildren<ParticleSystem>();
        effectDisplay.transform.position = caster.attackPos.position + (Vector3)effectDirect.normalized * 0.04f;
        if (effectDirect.x > 0)
        {
            effectDisplay.transform.eulerAngles = new Vector3(0, 180, Vector2.Angle(effectDirect.normalized, Vector2.up));
        }
        else
        {
            effectDisplay.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(effectDirect.normalized, Vector2.up));
        }
    }

    override protected void OnDestroy()
    {
        if (musicPlayer != null)
        {
            musicPlayer.Play(skillVo == null ? "" : skillVo.MusicOver, false, true);
            musicPlayer.use = false;
        }
        if (caster != null && caster.magicBase == this)
        {
            caster.ClearMagic();
        }
        for(int i = 0; i < particles.Length; i ++)
        {
            particles[i].loop = false;
        }
        particles = null;
        GameObject.Destroy(effectDisplay , 2.0f);
        GameObject.Destroy(shotEffect_);
        GameObject.Destroy(hitEffect_);
    }

    override protected void Update()
    {
        base.Update();

        if (effectDirect.x > 0)
        {
            effectDisplay.transform.eulerAngles = new Vector3(0, 180, Vector2.Angle(effectDirect.normalized, Vector2.up));
        }
        else
        {
            effectDisplay.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(effectDirect.normalized, Vector2.up));
        }

        effectDisplay.transform.position = caster.attackPos.position + (Vector3)effectDirect.normalized * 0.04f;

        if (Time.time > damageTime)
        {
            damageTime = Time.time + skillVo.Interval;
            for (int i = 0; i < GameData.allUnits.Count; i ++)
            {
                if (GameData.allUnits[i] == null || GameData.allUnits[i] == caster || GameData.allUnits[i].IsDead || GameData.allUnits[i].IsDisappear) continue;
                Vector3 casterPos = caster.currPos;
                Vector3 targetPos = GameData.allUnits[i].currPos;
                float distance = CommonUtil.Distance(caster , GameData.allUnits[i]);
                if (distance < skillVo.ShotRange * MapManager.textSize)
                {
                    Vector3 temVec = targetPos - casterPos;
                    float angle = Mathf.Acos(Vector3.Dot(effectDirect.normalized, temVec.normalized)) * Mathf.Rad2Deg;
                    if (angle < skillVo.Angle || effectDirect.normalized == (Vector2)temVec.normalized)
                    {
                        RaycastHit2D raycastHit2D = Physics2D.Raycast(caster.currPos, targetPos - casterPos, distance , LayerUtil.WallMasks());
                        if (raycastHit2D.collider == null)
                        {
                            if (hitEffect != null)
                            {
                                GameObject.Destroy(GameObject.Instantiate(hitEffect, GameData.allUnits[i].transform), 1.5f);
                            }
                            GameData.allUnits[i].ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , temVec);
                        }
                    }
                }
            }
        }
    }
}
