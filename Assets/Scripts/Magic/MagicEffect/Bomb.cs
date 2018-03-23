using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour
{
    public GameObject bomb;
    public GameObject bombPlane;
    public GameObject shadow;
    public GameObject bombEffect;
    public GameObject bombPit;
    public CircleCollider2D cirCollider2D;

    public BombMagic bombMagic;

    [HideInInspector]
    public bool bombed;
    private bool plane;

    private SkillLevelVo skillVo;
    private ActorObject caster;

    private void OnDestroy()
    {
        bombMagic.Bomb(this);
    }

    public void PlaneBomb(SkillLevelVo skillVo_ , ActorObject caster_)
    {
        skillVo = skillVo_;
        caster = caster_;
        StartCoroutine(PlaneBomb_());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (plane && LayerUtil.IsNotAvailable(collision.gameObject.layer))
        {
            BombMagic.allBombList.Remove(this);
            GameObject.Destroy(gameObject);
            return;
        }

        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null && !actorObject.isFly)
        {
            ShowBomb();
        }
    }

    public void ShowBomb()
    {
        if (!plane || bombed) return;
        bombed = true;
        DamageTarget();
        bombPlane.SetActive(false);
        Invoke("BombOther", 0.2f);
        BombMagic.allBombList.Remove(this);
        GameObject.Destroy(GameObject.Instantiate(bombPit, bomb.transform.position, Quaternion.identity) , 2.0f);
        GameObject.Destroy(GameObject.Instantiate(bombEffect, bomb.transform.position, Quaternion.identity), 2.0f);
        GameObject.Destroy(gameObject , 0.3f);
    }

    private void BombOther()
    {
        for(int i = 0; i< BombMagic.allBombList.Count; i ++)
        {
            if (BombMagic.allBombList[i].bombed) continue;
            if (Vector2.Distance(BombMagic.allBombList[i].transform.position, transform.position) < skillVo.DamageRange * MapManager.textSize)
            {
                BombMagic.allBombList[i].ShowBomb();
            }
        }
    }

    private void DamageTarget()
    {
        for (int i = GameData.allUnits.Count - 1; i >= 0; i--)
        {
            if (GameData.allUnits[i] == null || GameData.allUnits[i].IsDead || GameData.allUnits[i].IsDisappear || GameData.allUnits[i].isFly) continue;
            Vector3 distance = GameData.allUnits[i].currPos - bomb.transform.position;
            if (distance.magnitude - GameData.allUnits[i].circleCollider2D.radius < skillVo.DamageRange * MapManager.textSize)
            {
                GameData.allUnits[i].ReduceHp(caster , skillVo.BaseDamage, skillVo.AttachElement , skillVo.Buff , distance);
            }
        }
    }

    IEnumerator PlaneBomb_()
    {
        while (Vector2.Distance(bomb.transform.position , shadow.transform.position) > 0.022f)
        {
            yield return null;
            bomb.transform.position += Vector3.down * 0.02f;
        }
        yield return null;
        bomb.SetActive(false);
        shadow.SetActive(false);
        bombPlane.SetActive(true);
        cirCollider2D.enabled = true;
        plane = true;
    }
}
