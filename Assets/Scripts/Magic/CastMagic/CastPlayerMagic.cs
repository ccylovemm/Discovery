using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPlayerMagic : MagicBase
{
    public GameObject capacityEffect1;
    public GameObject capacityEffect2;

    protected GameObject capacityEffect_;

    private bool capacityFull = false;
    private float pressTime;
    private int angle = 30;

    override protected void Awake()
    {
        base.Awake();
        pressTime = Time.time;
    }

    protected override void Start()
    {
        base.Start();
        angle = (int)skillVo.Angle;
        directObj_ = GameObject.Instantiate(directObj);
        capacityEffect_ = GameObject.Instantiate(capacityEffect1, caster.transform);
    }

    override protected void Update()
    {
        base.Update();

        if (!capacityFull && Time.time - pressTime >= skillVo.ChargeTime)
        {
            capacityFull = true;
            GameObject.Destroy(capacityEffect_);
            capacityEffect_ = GameObject.Instantiate(capacityEffect2, caster.transform);
        }

        if (directObj_ != null)
        {
            directObj_.transform.position = caster.currPos + (Vector3)effectDirect.normalized * 0.1f;

            if (effectDirect.x > 0)
            {
                directObj_.transform.eulerAngles = new Vector3(0, 180, Vector2.Angle(effectDirect.normalized, Vector2.up));
            }
            else
            {
                directObj_.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(effectDirect.normalized, Vector2.up));
            }
        }
    }

    public override void MagicDestory()
    {
        base.MagicDestory();
        GameObject.Destroy(capacityEffect_);
        Fire();
    }

    protected void Fire()
    {
        if (skillVo.SkillValue == 1)
        {
            GameObject.Instantiate(effectPrefab, caster.currPos, Quaternion.identity).GetComponent<Cast>().SetData(caster, skillVo, caster.currPos, effectDirect, Mathf.Min(1.0f, (Time.time - pressTime) / skillVo.ChargeTime));
        }
        else
        {
            for (int i = 0; i < skillVo.SkillValue; i++)
            {
                Vector2 dir = Quaternion.AngleAxis(((1 - skillVo.SkillValue + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized;
                Vector3 originPos = caster.attackPos.position + (Vector3)dir * 0.1f;
                GameObject.Instantiate(effectPrefab, originPos, Quaternion.identity).GetComponent<Cast>().SetData(caster, skillVo, originPos, dir, Mathf.Min(1.0f , (Time.time - pressTime) / skillVo.ChargeTime));
            }
        }
    }
}
