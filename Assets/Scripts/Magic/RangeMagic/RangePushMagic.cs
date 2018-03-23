using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePushMagic : MagicBase
{
    public GameObject capacityEffect1;
    public GameObject capacityEffect2;

    public GameObject effectLevel1;
    public GameObject effectLevel2;
    public GameObject effectLevel3;

    protected GameObject capacityEffect_;

    private float pressTime;
    private bool capacityFull = false;

    protected override void Awake()
    {
        base.Awake();
        pressTime = Time.time;
        GameObject.Destroy(gameObject , 10);
    }

    protected override void Start()
    {
        base.Start();
        capacityEffect_ = GameObject.Instantiate(capacityEffect1 , caster.transform);
    }

    protected override void Update()
    {
        base.Update();
        if (!capacityFull && Time.time - pressTime >= skillVo.ChargeTime)
        {
            capacityFull = true;
            GameObject.Destroy(capacityEffect_);
            capacityEffect_ = GameObject.Instantiate(capacityEffect2, caster.transform);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameObject.Destroy(capacityEffect_);
    }

    public override void MagicDestory()
    {
        base.MagicDestory();
        pressTime = Time.time - pressTime;
        GameObject.Destroy(GameObject.Instantiate(pressTime < 1.0f ? effectLevel1 : (pressTime > skillVo.ChargeTime ? effectLevel3 : effectLevel2), caster.transform.position, Quaternion.identity), 3.0f);
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
                GameData.allUnits[i].ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff, distance.normalized * (1 + Mathf.Min(pressTime / skillVo.ChargeTime , 1)));
            }
        }
    }
}
