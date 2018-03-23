using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMagic : MagicBase
{
    override protected void Start()
    {
        base.Start();
        caster.animationManager.Play(AnimationName.Attack1);
        caster.executeAttack = ExecuteAttack;
        float time = Mathf.Min(caster.animationManager.GetCurrTime() , 2.0f);
        GameObject.Destroy(gameObject, time);
    }

    void ExecuteAttack()
    {
        if (caster.IsDead || caster.targetObject.IsDead) return;
        if (CommonUtil.Distance(caster , caster.targetObject) > caster.currSkillData.skillVo.Distance * MapManager.textSize) return;
        caster.targetObject.ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , caster.targetObject.currPos - caster.currPos);
    }
}
