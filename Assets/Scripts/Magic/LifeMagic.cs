using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeMagic : MagicBase
{
    protected override void Start()
    {
        base.Start();
        caster.AddHp(skillVo.BaseDamage);
        GameObject.Destroy(GameObject.Instantiate(effectPrefab, caster.transform), skillVo.Duration);
        GameObject.Destroy(gameObject);
    }
}
