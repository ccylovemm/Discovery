using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AOE : MonoBehaviour
{
    public GameObject effect;
    public CircleCollider2D cirCollider2D;

    private ActorObject caster;
    private SkillLevelVo skillVo;
    private List<float> damageTimes = new List<float>();
    private List<ActorObject> actorList = new List<ActorObject>();
    private float damageInterval;

    private void Awake()
    {
        GameObject.Instantiate(effect, transform);
    }

    private void Update()
    {
        if (Time.time > damageInterval)
        {
            damageInterval = Time.time + 0.15f;
            DamageTarget();
        }
    }

    private void OnDestroy()
    {
        actorList.Clear();
        actorList = null;
    }

    public void PlaneAOE(SkillLevelVo vo , ActorObject caster_)
    {
        skillVo = vo;
        caster = caster_;
        cirCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null && !actorObject.isFly)
        {
            actorList.Add(actorObject);
            damageTimes.Add(Time.time);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null)
        {
            int index = actorList.IndexOf(actorObject);
            if (index != -1)
            {
                actorList.RemoveAt(index);
                damageTimes.RemoveAt(index);
            }
        }
    }

    private void DamageTarget()
    {
        for (int i = actorList.Count - 1; i >= 0; i--)
        {
            if (actorList[i].IsDead)
            {
                actorList.RemoveAt(i);
                damageTimes.RemoveAt(i);
                continue;
            }
            if (Vector2.Distance(transform.position, actorList[i].transform.position) > skillVo.DamageRange)
            {
                continue;
            }
            if (Time.time > damageTimes[i])
            {
                if (skillVo.SkillValue == 1)
                {
                    actorList[i].ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement, skillVo.Buff , Vector2.zero);
                }
                else
                {
                    actorList[i].AddHp(skillVo.BaseDamage);
                }
                damageTimes[i] = Time.time + skillVo.Interval;
            }
        }
    }
}
