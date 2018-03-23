using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBackMagic : MagicBase
{
    public GameObject capacityEffect;
    protected GameObject capacityEffect_;

    protected float castintervalTime;

    override protected void Awake()
    {
        base.Awake();
        castintervalTime = Time.time;
    }
   
    protected override void Start()
    {
        base.Start();
        directObj_ = GameObject.Instantiate(directObj);
    }

    override protected void Update()
    {
        base.Update();
        if (Time.time - castintervalTime > 1.0f && capacityEffect_ == null)
        {
            capacityEffect_ = GameObject.Instantiate(capacityEffect, caster.transform);
        }
    }

    public override void MagicDestory()
    {
        base.MagicDestory();
        GameObject.Destroy(capacityEffect_);
        DoDamage();
    }

    private void DoDamage()
    {
        List<ActorObject> enemys = GameData.GetTarget(caster);
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i] == null || enemys[i] == caster || enemys[i].IsDead || enemys[i].IsDisappear) continue;
            Vector2 distance = caster.currPos - enemys[i].currPos;
            if (distance.magnitude < skillVo.DamageRange * MapManager.textSize)
            {
                enemys[i].ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff, distance.normalized * (1 + Mathf.Min(1 , Time.time - castintervalTime / 5)));
            }
        }
    }
}
