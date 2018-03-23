using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMagic : MagicBase
{
    public bool isAddHp;
    protected float beamLength;
    protected float maxBeamLength;
    protected float currBeamLength;
    protected LineRenderer lineRenderer;

    private float passTime;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GameObject.Instantiate(effectPrefab).GetComponent<LineRenderer>(); ;
        lineRenderer.sortingLayerID = SortingLayer.NameToID("Scene");
        passTime = Time.time;
    }

    protected override void Start()
    {
        base.Start();
        maxBeamLength = skillVo.ShotRange * MapManager.textSize;
        lineRenderer.sortingOrder = (int)(caster.transform.position.y * -100) + 5;
    }

    override protected void Update()
    {
        base.Update();
        UpdateLine();
    }

    protected void UpdateLine()
    {
        currBeamLength += 0.2f;

        currBeamLength = Mathf.Min(maxBeamLength, currBeamLength);

        if (Time.time - passTime > 2.0f)
        {
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
        }

        Vector2 originPos = (Vector2)caster.attackPos.position + effectDirect.normalized * 0.04f;
        RaycastHit2D[] hitPoints = Physics2D.RaycastAll(caster.attackPos.position, effectDirect, currBeamLength, LayerUtil.DamageMasks());
        beamLength = currBeamLength;

        for (int i = 0; i < hitPoints.Length; i++)
        {
            if (hitPoints[i].transform.gameObject == caster.gameObject) continue;
            ActorObject actorObject = hitPoints[i].collider.GetComponent<ActorObject>();
            if (actorObject != null && actorObject.IsDead) continue;
            Vector2 ldir = hitPoints[i].point - originPos;
            beamLength = ldir.magnitude;
            if (actorObject != null && !actorObject.IsDisappear)
            {
                if (Time.time > damageTime)
                {
                    damageTime = Time.time + skillVo.Interval;
                    if (isAddHp)
                    {
                        actorObject.AddHp(skillVo.BaseDamage);
                    }
                    else
                    {
                        actorObject.ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , Time.time - passTime > 2.0f ? ldir : Vector2.zero);
                    }
                }
            }
            break;
        }

        lineRenderer.SetPosition(0, originPos);
        lineRenderer.SetPosition(1, originPos + effectDirect.normalized * beamLength);
        lineRenderer.materials[0].SetTextureScale("_MainTex", new Vector2(beamLength * 0.1f, 1f));
        lineRenderer.materials[0].SetTextureOffset("_MainTex", new Vector2(-1 * (Time.time % 1.0f) + 1, 0f));
        hitEffect_.transform.position = originPos + effectDirect.normalized * beamLength;
    }

    public override void MagicDestory()
    {
        base.MagicDestory();
        GameObject.Destroy(lineRenderer.gameObject);
    }
}
