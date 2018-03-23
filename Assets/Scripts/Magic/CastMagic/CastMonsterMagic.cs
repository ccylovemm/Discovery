using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastMonsterMagic : MagicBase
{
    public int angle = 30;

    protected override void Start()
    {
        base.Start();
        caster.animationManager.Play(AnimationName.Attack1);
        caster.executeAttack = ExecuteAttack;
        GameObject.Destroy(gameObject, Mathf.Min(caster.animationManager.GetCurrTime(), 2.0f));
    }

    override protected void Update()
    {
        FindEnemy();

        if (target == null)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        effectDirect = target.transform.position - caster.transform.position;

        if (effectDirect.x > 0)
        {
            caster.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (effectDirect.x < 0)
        {
            caster.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        effectDirect = target.attackPos.position - caster.currPos;
    }

    private void ExecuteAttack()
    {
        if (skillVo.SkillValue == 1)
        {
            GameObject.Instantiate(effectPrefab, caster.currPos, Quaternion.identity).GetComponent<Cast>().SetData(caster, skillVo, caster.currPos, effectDirect);
        }
        else
        {
            for (int i = 0; i < skillVo.SkillValue; i++)
            {
                Vector2 dir = Quaternion.AngleAxis(((1 - skillVo.SkillValue + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized;
                Vector3 originPos = caster.attackPos.position + (Vector3)dir * 0.1f;
                GameObject.Instantiate(effectPrefab, originPos, Quaternion.identity).GetComponent<Cast>().SetData(caster, skillVo, originPos, dir);
            }
        }
    }

    protected void FindEnemy()
    {
        target = caster.targetObject != null && !caster.targetObject.IsDead ? caster.targetObject : null;
        if (target == null)
        {
            float minDistance = float.MaxValue;
            List<ActorObject> enemys = GameData.GetTarget(caster);
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i].IsDead || enemys[i].IsDisappear) continue;
                float distance = CommonUtil.Distance(caster, enemys[i]);
                if (distance < skillVo.ShotRange && distance < minDistance)
                {
                    minDistance = distance;
                    target = enemys[i];
                }
            }
        }
    }
}
