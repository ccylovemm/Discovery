using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World1Boss : ActorObject
{
    public GameObject w1Boss_break;
    public GameObject w1Boss_digDown;
    public GameObject w1Boss_birth;
    public GameObject w1Boss_Stun;
    public GameObject w1Boss_mudRoll;
    public GameObject w1Boss_runDrop;

    private bool isBossDizzy = false;
    private GameObject mudRoll;
    private GameObject runDrop;

    override protected void Awake()
    {
        base.Awake();
        IsDisappear = true;
        gameObject.layer = LayerUtil.LayerToDefault();
    }

    override protected void Start()
    {
        base.Start();
        StartCoroutine(StartAI());
        StartCoroutine(ShowBossAppear());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if(other.tag == "Shield")
        {
            GameObject.Destroy(runDrop);
            ClearMagic();
            StopAllCoroutines();
            animationManager.Play(AnimationName.Idle);
            GameObject.Destroy(GameObject.Instantiate(w1Boss_Stun, transform), 3);
            StartCoroutine(ShieldDizzy());
        }
    }

    IEnumerator ShowBossAppear()
    {
        yield return new WaitForSeconds(0.5f);
        WindowManager.Instance.OpenBossAppear(WindowKey.World1BossAppear);
    }

    override protected void ReduceHp_(ActorObject caster , uint damageValue , bool isFromBuff =  false)
    {
        if (isBossDizzy)
        {
            base.ReduceHp_(caster, damageValue);
        }
        else
        {
            if (damageValue >= actorData.currShield)
            {
                GameObject.Destroy(mudRoll);
                GameObject.Destroy(runDrop);
                GameObject.Destroy(GameObject.Instantiate(w1Boss_break, transform), 3);
                isBossDizzy = true;
                ClearMagic();
                StopAllCoroutines();
                StartCoroutine(Dizzy());
            }
            base.ReduceHp_(caster, damageValue);
        }
    }

    public override void AddBuff(ActorObject caster, uint damageBuffId, string damageAttributeStr)
    {
        if (damageBuffId == 29)
        {
            return;
        }
        base.AddBuff(caster, damageBuffId, damageAttributeStr);
    }

    IEnumerator ShieldDizzy()
    {
        yield return null;
        yield return new WaitForSeconds(3.0f);
        while (isFrozen || isDizzy)
        {
            yield return null;
        }
        GameObject.Destroy(mudRoll);
        SkillManager.Instantiate(this, actorData.skills[2].skillVo);
        animationManager.Play("Diving");
        yield return new WaitForSeconds(animationManager.GetCurrTime());
        animationManager.Play("Empty");
        IsDisappear = true;
        gameObject.layer = LayerUtil.LayerToDefault();
        RemoveAllBuff();
        yield return new WaitForSeconds(1.0f);
        RandomPos();
        StartCoroutine(StartAI());
    }

    IEnumerator Dizzy()
    {
        yield return null;
        movement.Stop();
        while (isFrozen || isDizzy)
        {
            yield return null;
        }
        GameObject.Destroy(GameObject.Instantiate(w1Boss_Stun, transform), 3);
        animationManager.Play(AnimationName.Dizzy);
        yield return new WaitForSeconds(3.0f);
        while (isFrozen || isDizzy)
        {
            yield return null;
        }
        SkillManager.Instantiate(this, actorData.skills[2].skillVo);
        animationManager.Play("RunAway");
        yield return new WaitForSeconds(animationManager.GetCurrTime());
        animationManager.Play("Empty");
        IsDisappear = true;
        actorData.currShield = actorData.cfgVo.Shield;
        gameObject.layer = LayerUtil.LayerToDefault();
        RemoveAllBuff();
        yield return new WaitForSeconds(1.0f);
        RandomPos();
        isBossDizzy = false;
        StartCoroutine(StartAI());
    }

    IEnumerator StartAI()
    {
        while (!isBossDizzy)
        {
            yield return new WaitForSeconds(3.0f);
            GameObject.Destroy(GameObject.Instantiate(w1Boss_birth, transform), 3);
            yield return new WaitForSeconds(3.0f);
            gameObject.layer = LayerUtil.LayerToActor();
            IsDisappear = false;
            Vector2 direct = GameData.myself.currPos - currPos;
            if (direct.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (direct.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            SkillManager.Instantiate(this, actorData.skills[2].skillVo);
            animationManager.Play(AnimationName.Jump);
            yield return new WaitForSeconds(animationManager.GetCurrTime());
          
            if (actorData.currHp < actorData.cfgVo.MaxHp / 2 && Time.time > actorData.skills[1].cdTime)
            {
                int count = Random.Range(2 , 5);
                while (count > 0)
                {
                    IsDisappear = false;
                    mudRoll = GameObject.Instantiate(w1Boss_mudRoll, transform);
                    runDrop = GameObject.Instantiate(w1Boss_runDrop, transform);
                    currSkillData = actorData.skills[1];
                    SkillManager.Instantiate(this, currSkillData.skillVo);
                    while (hasCreateMagic || magicBase != null)
                    {
                        yield return null;
                    }
                    GameObject.Destroy(runDrop);
                    while (isFrozen || isDizzy)
                    {
                        yield return null;
                    }
                    GameObject.Destroy(mudRoll);
                    SkillManager.Instantiate(this, actorData.skills[2].skillVo);
                    animationManager.Play("Diving");
                    yield return new WaitForSeconds(animationManager.GetCurrTime());
                    animationManager.Play("Empty");
                    IsDisappear = true;
                    gameObject.layer = LayerUtil.LayerToDefault();
                    RemoveAllBuff();
                    count--;
                    yield return new WaitForSeconds(0.5f);
                }
                currSkillData.cdTime = Time.time + currSkillData.skillVo.CD;
            }
            else if(Time.time > actorData.skills[0].cdTime)
            {
                mudRoll = GameObject.Instantiate(w1Boss_mudRoll, transform);
                yield return new WaitForSeconds(1.0f);
                currSkillData = actorData.skills[0];
                SkillManager.Instantiate(this , currSkillData.skillVo);
                currSkillData.cdTime = Time.time + currSkillData.skillVo.CD;
                while (hasCreateMagic || magicBase != null)
                {
                    yield return null;
                }
                animationManager.Play(AnimationName.Idle);
                yield return new WaitForSeconds(5.0f);
                while (isFrozen || isDizzy)
                {
                    yield return null;
                }
                GameObject.Destroy(mudRoll);
                SkillManager.Instantiate(this, actorData.skills[2].skillVo);
                animationManager.Play("Diving");
                yield return new WaitForSeconds(animationManager.GetCurrTime());
                animationManager.Play("Empty");
                IsDisappear = true;
                gameObject.layer = LayerUtil.LayerToDefault();
                RemoveAllBuff();
            }
            yield return new WaitForSeconds(1.0f);
            RandomPos();
        }
    }

    private void RandomPos()
    {
        List<Vector2> list = new List<Vector2>();
        for(int i = 0; i < SceneManager.Instance.mapRoom.mapInfo.randonMonsterPos.Count; i ++)
        {
            list.Add(SceneManager.Instance.mapRoom.mapInfo.randonMonsterPos[i] + new Vector2(SceneManager.Instance.mapRoom.mapInfo.offsetX , SceneManager.Instance.mapRoom.mapInfo.offsetY));
        }
        Vector2 grid = MapManager.GetGrid(transform.position);
        list.Remove(grid);
        transform.position = MapManager.GetPos(list[Random.Range(0, list.Count)]);
    }
}
