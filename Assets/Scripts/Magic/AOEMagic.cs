using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEMagic : MagicBase
{
    private bool plane;

    override protected void Awake()
    {
        base.Awake();
        GameObject.Instantiate(effectPrefab, effectDisplay.transform);
    }

    protected override void Start()
    {
        base.Start();
        if (caster.allAOEList.Count >= 3)//策划不需要配置数量  2018年1月12日13:33:17
        {
            GameObject.Destroy(caster.allAOEList[0].gameObject);
        }
        caster.allAOEList.Add(this);
    }

    override protected void Update()
    {
        if (plane) return;
        base.Update();
        effectDisplay.transform.position = (Vector2)caster.attackPos.position + effectDirect.normalized * skillVo.ShotRange * MapManager.textSize;
    }

    override public void MagicDestory()
    {
        plane = true;
        effectDisplay.GetComponentInChildren<AOE>().PlaneAOE(skillVo, caster);
        GameObject.Destroy(gameObject, skillVo.Duration);
    }

    override protected void OnDestroy()
    {
        base.OnDestroy();
        caster.allAOEList.Remove(this);
    }
}
