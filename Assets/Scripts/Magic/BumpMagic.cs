using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpMagic : MagicBase
{
    public GameObject dizzyEffect;
    public CircleCollider2D circleCollider;

    private GameObject effectPrefab_;

    private bool hasBump = false;
    private bool isBump = false;

    override protected void Start()
    {
        circleCollider.radius = caster.circleCollider2D.radius;
        circleCollider.offset = caster.circleCollider2D.offset;
        caster.animationManager.Play(AnimationName.ReadyAttack);
        StartCoroutine(StartAttack());
    }

    override protected void Update()
    {
        transform.position = caster.transform.position;
    }

    IEnumerator StartAttack()
    {
        FindTarget();
        caster.UpdateDirect(target.transform.position - caster.transform.position);
        yield return new WaitForSeconds(1.5f);

        if (caster.IsDead)
        {
            GameObject.Destroy(gameObject);
        }
        else if (target == null || target.IsDead)
        {
            caster.animationManager.Play(AnimationName.Idle);
            GameObject.Destroy(gameObject);
        }
        else
        { 
            Vector2 grid = MapManager.GetGrid(target.transform.position);
            if (MapManager.mapPathData.ContainsKey(grid))
            {
                Vector2 d = target.currPos - caster.currPos;
                caster.UpdateDirect(d);
                caster.animationManager.Play(AnimationName.Bump);
                hasBump = true;
                float longPath = skillVo.ShotRange * MapManager.textSize;
                float speed = 0.01f;
                effectPrefab_ = GameObject.Instantiate(effectPrefab , transform);
                while (true)
                {
                    if (isBump)
                    {
                        break;
                    }
                    yield return null;
                    caster.transform.position += (Vector3)d.normalized * speed;
                    longPath -= speed;
                    speed += 0.002f;
                    if (longPath <= 0)
                    {
                        hasBump = false;
                        caster.animationManager.Play(AnimationName.Idle);
                        yield return new WaitForSeconds(0.5f);
                        GameObject.Destroy(effectPrefab_);
                        GameObject.Destroy(gameObject);
                        break;
                    }
                }
            }
            else
            {
                GameObject.Destroy(effectPrefab_);
                GameObject.Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.IsDamageLayer(collision.gameObject.layer))
        {
            if (hasBump)
            {
                ActorObject actorObject = collision.GetComponent<ActorObject>();
                if (actorObject != null)
                {
                    if (actorObject.IsDead || actorObject.IsDisappear) return;
                    actorObject.ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff, actorObject.currPos - caster.currPos);
                }
                isBump = true;
                hasBump = false;
                GameObject.Destroy(effectPrefab_);
                StartCoroutine(BumpOver(actorObject == null));
            }
        }
    }

    IEnumerator BumpOver(bool dizzy)
    {
        if (dizzy)
        {
            GameObject.Destroy(GameObject.Instantiate(dizzyEffect, transform) , 3.0f);
            caster.animationManager.Play(AnimationName.Dizzy);
            yield return new WaitForSeconds(3.0f);
        }
        caster.animationManager.Play(AnimationName.Idle);
        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(gameObject);
    }

    void FindTarget()
    {
        float distance = skillVo.ShotRange;
        List<ActorObject> allTargets = GameData.GetTarget(caster);
        for (int i = 0; i < allTargets.Count; i++)
        {
            if (allTargets[i].IsDead || allTargets[i].IsDisappear) continue;
            float dis = Vector2.Distance(caster.transform.position, allTargets[i].transform.position);
            if (distance > dis)
            {
                target = allTargets[i];
                distance = dis;
            }
        }
    }
}
