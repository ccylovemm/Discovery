using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMagic : MagicBase
{
    protected override void Start()
    {
        base.Start();
        caster.animationManager.Play(AnimationName.Attack1);
        caster.executeAttack = ExecuteAttack;
        GameObject.Destroy(gameObject, Mathf.Min(caster.animationManager.GetCurrTime(), 2.0f));
    }

    private void ExecuteAttack()
    {
        GameObject.Instantiate(effectPrefab, caster.attackPos.position , Quaternion.identity).GetComponent<Missile>().SetData(caster, caster.targetObject , skillVo);
    }
}
