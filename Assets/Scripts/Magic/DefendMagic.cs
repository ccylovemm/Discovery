using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendMagic : MagicBase
{
    override protected void Awake()
    {
        base.Awake();
        GameObject.Instantiate(effectPrefab, effectDisplay.transform);
    }

    protected override void Start()
    {
        base.Start();
        GameObject.Destroy(effectDisplay, skillVo.Duration);
        GameObject.Destroy(gameObject, skillVo.Duration);
        effectDisplay.transform.position = caster.transform.position;
    }

    override protected void Update()
    {
        base.Update();
        effectDisplay.transform.position = caster.transform.position;
    }

    override public void MagicDestory()
    {

    }
}
