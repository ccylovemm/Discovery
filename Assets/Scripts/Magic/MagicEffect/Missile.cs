using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject effectPrefab;
    public GameObject hitEffectPrefab;

    private float passTime;
    private float speed = 0.01f;
    private ActorObject caster;
    private ActorObject target;
    private SkillLevelVo skillVo;

    private void Awake()
    {
        passTime = Time.time;
        GameObject.Instantiate(effectPrefab, transform);
    }

    private void Update()
    {
        if (target == null) return;
        if (Time.time - passTime > skillVo.SkillValue || target.IsDead)
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab, transform.position, Quaternion.identity), 2);
        }
        Vector3 direct = target.currPos - transform.position;
        transform.position += direct.normalized * speed;
    }

    public void SetData(ActorObject actor, ActorObject target_, SkillLevelVo vo)
    {
        caster = actor;
        target = target_;
        skillVo = vo;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerUtil.IsWall(collision.gameObject.layer))
        {
            GameObject.Destroy(gameObject);
            GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab, transform.position ,Quaternion.identity), 2);
        }
        else
        {
            ActorObject actorObject = collision.gameObject.GetComponent<ActorObject>();
            if (actorObject == target)
            {
                actorObject.ReduceHp(caster, skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff, (target.currPos - transform.position).normalized);
                GameObject.Destroy(GameObject.Instantiate(hitEffectPrefab, actorObject.transform), 2);
                GameObject.Destroy(gameObject);
            }
        }
    }
}
