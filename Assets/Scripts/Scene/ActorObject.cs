using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using System.Linq;

public class ActorObject : SceneBase
{
    public ExecuteAttack executeAttack;
    public delegate void ExecuteAttack();

    public Transform attackPos;

    public Collider2D obstacleCollider;

    [HideInInspector]
    public List<ActorObject> pets = new List<ActorObject>();//宠物
    [HideInInspector]
    public ActorObject master;//召唤物的主人
    [HideInInspector]
    public ActorObject deadTarget;//被谁杀死
    [HideInInspector]
    public ActorObject targetObject;//锁定对象
    [HideInInspector]
    public float beHitTime = 0;
    [HideInInspector]
    public ActorObject beHitObject;//被人攻击定对象
    [HideInInspector]
    public BehaviorTree behaviorTree;//AI行为树


    [HideInInspector]
    public Movement movement;
    [HideInInspector]
    public Rigidbody2D rigidbody2;
    [HideInInspector]
    public SpriteRenderer bodyRender;
    [HideInInspector]
    public CircleCollider2D circleCollider2D;
    [HideInInspector]
    public AnimationManager animationManager;

    [HideInInspector]
    public HeadBar headBar;//头顶血条
    [HideInInspector]
    public HeadSkill headSkill;//脚下释放元素的图标

    [HideInInspector]
    public bool IsDead;
    [HideInInspector]
    public bool isFrozen;
    [HideInInspector]
    public bool isDizzy;
    [HideInInspector]
    public bool IsDisappear;//免疫一切攻击 
    [HideInInspector]
    public bool isVisible = true; //战争迷雾使用
    [HideInInspector]
    public bool hasCreateMagic = false;

    public ActorData actorData;
    public SkillData currSkillData;

    public float terrainBuffTime;
    public float terrainEffectTime;

    public Vector3 terrainEffectLastPos;
    public int terrainEffectLastPosCount;

    public Dictionary<uint, Buff> buffList = new Dictionary<uint, Buff>();

    public bool isAI = false;
    public float speed = 1.0f;
    public Vector2 moveDirect;
    public bool attackState = true;

    private Transform mTrans;
    private Renderer[] mRenderers;

    private Vector3 diePos;

    private uint damageType;

    private float addSpeed = 1.0f;
    private float reduceSpeed = 1.0f;

    private bool isIceUp = false;
    private bool moveState = false;
    private bool showdamage = false;

    virtual protected void Awake()
    {
        mTrans = transform;
        movement = GetComponent<Movement>();
        rigidbody2 = GetComponent<Rigidbody2D>();
        mRenderers = GetComponentsInChildren<Renderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animationManager = GetComponent<AnimationManager>();
        bodyRender = GetComponentInChildren<SpriteRenderer>();
    }

    virtual protected void Start()
    {
        actorData.currShield = actorData.cfgVo.Shield;
        if(rigidbody2 != null) rigidbody2.drag = actorData.cfgVo.Mass;
    }

    protected void Update()
    {

        if (IsDead || isFrozen || isDizzy) return;

        if (moveState)
        {
            Vector3 speedVec = (Vector3)moveDirect.normalized * (moveDirect.magnitude / 50.0f);
            Vector3 offsetPos = speedVec * 0.018f * (attackState ? actorData.cfgVo.AtkMovingSpeed : actorData.cfgVo.MovingSpeed) * speed;
            transform.position += offsetPos;
        }

        if (Time.frameCount % 30 == 0 && !IsDead && behaviorTree != null && GameData.myself != null)
        {
            if (Vector2.Distance(mTrans.position, GameData.myself.transform.position) > 3.0f)
            {
                behaviorTree.DisableBehavior(true);
            }
            else
            {
                behaviorTree.EnableBehavior();
            }
        }
    }

    public void SetFrozen(bool bol)
    {
        isFrozen = bol;
        if (isFrozen)
        {
            animationManager.SetSpeed(0);
        }
        else
        {
            UpdateSpeed();
        }
    }

    public void SetDizzy(bool bol)
    {
        isDizzy = bol;
    }

    public void AttackState(bool value)
    {
        attackState = value;

        if (IsDead || isFrozen || isDizzy) return;

        if (attackState)
        {

        }
        else
        {
            GameData.myself.currSkillData = null;
            EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
        }
        GameData.elements.Clear();
        GameData.myself.currSkillData = null;
        EventCenter.DispatchEvent(EventEnum.ClearUseSkill);
    }

    public void MoveState(bool value)
    {
        if (isIceUp && moveState && !value)
        {
            rigidbody2.drag = 2;
            rigidbody2.velocity = moveDirect.normalized;
        }

        moveState = !(IsDead || isFrozen || isDizzy) && value;

        if (IsDead || isFrozen || isDizzy)
        {
            return;
        }

        if (attackState)
        {
            animationManager.Play(moveState ? AnimationName.Attack1 : AnimationName.AttackStand);
        }
        else
        {
            animationManager.Play(moveState ? AnimationName.Run : AnimationName.Idle);
        }
    }

    public void MoveDirect(Vector2 value)
    {
        if (IsDead || isFrozen || isDizzy)
        {
            return;
        }

        if (moveState)
        {
            if (isIceUp)
            {
                float v = value.magnitude;
                moveDirect = (moveDirect + value * 0.1f) / 2;
                moveDirect = moveDirect.normalized * v;
            }
            else
            {
                moveDirect = value;
            }
           
            if (!attackState)
            {
                UpdateDirect(moveDirect);
            }
        }
    }

    public void UpdateDirect(Vector2 value)
    {
        if (value.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (value.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void PlayAttackAnimation()
    {
        animationManager.Play((moveState || movement.arrive)? AnimationName.Attack1 : AnimationName.AttackStand);
    }

    public bool HasBuffType(BuffType buffType)
    {
        for (int i = 0; i< actorData.buffList.Count; i ++)
        {
            if ((BuffType)BuffCFG.items[actorData.buffList[i].ToString()].Type == BuffType.DeFrozen)
            {
                return true;
            }
        }
        return false;
    }

    public void AddBuff(ActorObject caster , string damageBuffStr, string damageAttributeStr)
    {
        if (IsDead || IsDisappear) return;
        string[] buffs = damageBuffStr.Split(',');
        for (int i = 0; i < buffs.Length; i ++)
        {
            if (string.IsNullOrEmpty(buffs[i])) continue;
            AddBuff(caster , uint.Parse(buffs[i]) , damageAttributeStr);
        }
    }

    virtual public void AddBuff(ActorObject caster , uint damageBuffId, string damageAttributeStr)
    {
        if (damageBuffId == 0 || IsDead || IsDisappear) return;
        BuffVo buffVo = BuffCFG.items[damageBuffId.ToString()];
        if (!actorData.buffList.Contains(damageBuffId))
        {
            DeleteBuff(GetSameBuff(damageBuffId));
            bool hasConflict = false;
            for (int i = 0; i < BuffConflictCFG.items.Count; i++)
            {
                if (BuffConflictCFG.items[i].BuffType1 == buffVo.Type)
                {
                    uint bufId = actorData.buffList.Where(vo => BuffCFG.items[vo.ToString()].Type == BuffConflictCFG.items[i].BuffType2).SingleOrDefault();
                    if (bufId != 0)
                    {
                        if (BuffConflictCFG.items[i].Type == 1)
                        {
                            damageBuffId = BuffConflictCFG.items[i].BuffId;
                            DeleteBuff(bufId);
                            if (!actorData.buffList.Contains(damageBuffId))
                            {
                                actorData.buffList.Add(damageBuffId);
                            }
                            hasConflict = true;
                            break;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            if (!hasConflict)
            {
                actorData.buffList.Add(damageBuffId);
            }
        }
        Buff buff;
        if (!buffList.TryGetValue(damageBuffId, out buff))
        {
            buff = gameObject.AddComponent<Buff>();
            buff.buffVo = BuffCFG.items[damageBuffId.ToString()];
            buff.target = this;
            buffList.Add(damageBuffId, buff); 
        }

        buff.damageAttributeStr = damageAttributeStr;
        buff.caster = caster;

        float addtime = 0;

        if (!string.IsNullOrEmpty(damageAttributeStr) && damageAttributeStr != "0")
        {
            string[] attributes = damageAttributeStr.Split(',');
            // 饰品攻击属性Buff时间增加
            if (caster != null && caster.actorData.decorations.Count > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    int count = caster.actorData.decorations.Count;
                    for (int j = 0; j < count; j++)
                    {
                        DecorationVo decorationVo = caster.actorData.decorations[j];
                        if ((DecorationType)decorationVo.Type == DecorationType.AttackStatusTimeAdd && decorationVo.Attr == uint.Parse(attributes[i]))
                        {
                            addtime += decorationVo.Parameter;
                        }
                    }
                }
            }

            // 自身饰品属性Buff时间减免
            if (actorData.decorations.Count > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    int count = actorData.decorations.Count;
                    for (int j = 0; j < count; j++)
                    {
                        DecorationVo decorationVo = actorData.decorations[j];
                        if ((DecorationType)decorationVo.Type == DecorationType.SelfStatusTimeReduce && decorationVo.Attr == uint.Parse(attributes[i]))
                        {
                            addtime -= decorationVo.Parameter;
                        }
                    }
                }
            }
        }

        buff.lifeTime = Time.time + buff.buffVo.Duration + addtime;
    }

    public void DeleteBuff(uint id)
    {
        if (id == 0) return;
        Buff buff;
        if (buffList.TryGetValue(id, out buff))
        {
            GameObject.Destroy(buff);
            buffList.Remove(id);
        }
        actorData.buffList.Remove(id);
    }

    public void RemoveAllBuff()
    {
        while (actorData.buffList.Count > 0)
        {
            DeleteBuff(actorData.buffList[0]);
        }
    }

    private uint GetSameBuff(uint id)
    {
        BuffVo buffVo = BuffCFG.items[id.ToString()];
        foreach (var buff in buffList)
        {
            if (buff.Value.buffVo.Group == buffVo.Group)
            {
                return buff.Key;
            }
        }
        return 0;
    }

    public void AddSpeed(float s)
    {
        if (IsDead) return;
        addSpeed += s;
        UpdateSpeed();
    }

    public void ReduceSpeed(float s)
    {
        if (IsDead) return;
        reduceSpeed -= s;
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        speed = addSpeed - (1 - reduceSpeed);
        speed = Mathf.Max(0 , speed);
        animationManager.SetSpeed(isFrozen ? 0 : speed);
    }

    public void AddHp(uint value)
    {
        if (IsDead || IsDisappear) return;
        if (actorData.currHp >= actorData.cfgVo.MaxHp) return;
        actorData.currHp = (uint)Mathf.Min(actorData.currHp + value , actorData.cfgVo.MaxHp);
        if(headBar != null) headBar.AddHp((int)value);
        UpdateHp();
    }

    public void ReduceHpFromBuff(ActorObject caster, uint damageValue, string damageAttributeStr)
    {
        if (IsDead || IsDisappear) return;
        string[] attributes = damageAttributeStr.Split(',');

        //饰品属性Buff伤害加成
        if (caster != null && caster.actorData.decorations.Count > 0)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                int count = caster.actorData.decorations.Count;
                for (int j = 0; j < count; j++)
                {
                    DecorationVo decorationVo = caster.actorData.decorations[j];
                    if ((DecorationType)decorationVo.Type == DecorationType.AttackStatusDamage && decorationVo.Attr == uint.Parse(attributes[i]))
                    {
                        damageValue = (uint)(damageValue * decorationVo.Parameter);
                    }
                }
            }
        }
        ReduceHp(caster , damageValue , "" , "" , Vector2.zero , true);
    }

    public void ReduceHpFormCastMagic(ActorObject caster, uint damageValue, string damageAttributeStr, string damageBuffStr, Vector2 direct, bool isFormBuff = false)
    {
        if (IsDead || IsDisappear) return;
        //天赋 投掷类伤害增加
        for (int i = 0; i < caster.actorData.talents.Count; i++)
        {
            if ((TalentType)caster.actorData.talents[i].Type == TalentType.AddCastDamage)
            {
                int random = Random.Range(0, 10000);
                if (random < caster.actorData.talents[i].Rate)
                {
                    damageValue = (uint)(damageValue * caster.actorData.talents[i].ParamValue);
                    break;
                }
            }
        }

        ReduceHp(caster, damageValue, damageAttributeStr , damageBuffStr , direct);
    }

    public void ReduceHp(ActorObject caster , uint damageValue , string damageAttributeStr , string damageBuffStr , Vector2 direct , bool isFormBuff = false)
    {
        if (IsDead || IsDisappear) return;

        damageType = 0;

        //不在同一个阵营中 锁定敌人  
        if (caster != null && GameData.friends.Contains(caster) && !GameData.friends.Contains(this) || !GameData.friends.Contains(caster) && GameData.friends.Contains(this))
        {
            beHitTime = Time.time; //锁定敌人时效
            beHitObject = caster;
        }

        //属性相克  计算伤害
        if (!string.IsNullOrEmpty(damageAttributeStr) && damageAttributeStr != "0")
        {
            float addPercent = 1.0f;
            for (int n = 0; n < actorData.talents.Count; n++)
            {
                if ((TalentType)actorData.talents[n].Type == TalentType.AttrDamageAdd)
                {
                    int random = Random.Range(0, 10000);
                    if (random < actorData.talents[n].Rate)
                    {
                        addPercent = actorData.talents[n].ParamValue;
                        break;
                    }
                }
            }

            string[] attributes = damageAttributeStr.Split(',');
            for (int i = 0; i < attributes.Length; i++)
            {
                var buff = AttributeConflictCFG.items.Where(vo => vo.Value.Atribute1.ToString() == attributes[i] && vo.Value.Atribute2.ToString() == actorData.cfgVo.Attribute.ToString()).SingleOrDefault().Value;
                if (buff != null)
                {
                    damageValue = (uint)(damageValue * buff.DamageBonus * addPercent);
                    damageType = 1;
                    break;
                }
            }

            // 饰品属性伤害加成
            if (caster != null && caster.actorData.decorations.Count > 0)
            {
                for (int i = 0;i < attributes.Length; i ++)
                {
                    int count = caster.actorData.decorations.Count;
                    for (int j = 0; j < count; j ++)
                    {
                        DecorationVo decorationVo = caster.actorData.decorations[j];
                        if ((DecorationType)decorationVo.Type == DecorationType.AttackAttrDamage && decorationVo.Attr == uint.Parse(attributes[i]))
                        {
                            damageValue = (uint)(damageValue * decorationVo.Parameter);
                        }
                    }
                }
            }
        }

        //Buff 击中上海计算
        if (!string.IsNullOrEmpty(damageBuffStr) && damageBuffStr != "0")
        {
            string[] buffs = damageBuffStr.Split(',');
            for (int i = 0; i < buffs.Length; i ++)
            {
                BuffVo vo = BuffCFG.items[buffs[i]];
                if(vo.DmgType == 1)
                {
                    if ((BuffType)vo.Type == BuffType.HitIce)
                    {
                        if(buffList.ContainsKey(vo.ParamValue))
                        {
                            damageValue = (uint)(damageValue * vo.Value);
                        }
                    }
                    else if ((BuffType)vo.Type == BuffType.HitBack)
                    {
                        if (rigidbody2 != null)
                        {
                            float power = (MassType)actorData.cfgVo.Mass == MassType.Low ? 3.0f : ((MassType)actorData.cfgVo.Mass == MassType.Middle ? 2.0f : 0.0f);
                            rigidbody2.drag = actorData.cfgVo.Mass * 10;
                            rigidbody2.velocity = direct * power * vo.Value;
                        }
                    }
                    else if((BuffType)vo.Type == BuffType.FireResist || (BuffType)vo.Type == BuffType.WaterResist || (BuffType)vo.Type == BuffType.EarthResist || (BuffType)vo.Type == BuffType.ElectricResist || (BuffType)vo.Type == BuffType.ColdResist)
                    {
                        if (buffList.ContainsKey(vo.ParamValue))
                        {
                            damageValue = (uint)(damageValue * vo.Value);
                        }
                    }
                }
            }
        }

        //天赋受直接攻击减免
        if (!isFormBuff)
        {
            for (int i = 0; i < actorData.talents.Count; i++)
            {
                if ((TalentType)actorData.talents[i].Type == TalentType.ReduceDamage)
                {
                    int random = Random.Range(0, 10000);
                    if (random < actorData.talents[i].Rate)
                    {
                        damageType = 2;
                        damageValue = (uint)(damageValue * actorData.talents[i].ParamValue);
                        break;
                    }
                }
            }
        }
       
        AddBuff(caster, damageBuffStr , damageAttributeStr);

        if (damageValue >= actorData.currHp)
        {
            deadTarget = caster;
            actorData.currHp = 0;
            UpdateHp();
            DamageDie();

            // 饰品 击杀 回复血量
            if (caster != null && caster.actorData.decorations.Count > 0)
            {
                int count = caster.actorData.decorations.Count;
                for (int i = 0; i < count; i++)
                {
                    DecorationVo decorationVo = caster.actorData.decorations[i];
                    if ((DecorationType)decorationVo.Type == DecorationType.KillTargetAddBlood)
                    {
                        caster.AddHp((uint)(caster.actorData.cfgVo.MaxHp * decorationVo.Parameter));
                    }
                }
            }
            //天赋 击杀怪物触发buff
            if (caster != null)
            {
                for (int i = 0; i < caster.actorData.talents.Count; i++)
                {
                    if ((TalentType)caster.actorData.talents[i].Type == TalentType.KillBuff)
                    {
                        int random = Random.Range(0, 10000);
                        if (random < caster.actorData.talents[i].Rate)
                        {
                            caster.AddBuff(caster, (uint)caster.actorData.talents[i].ParamValue, "");
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            actorData.currHp -= damageValue;
            UpdateHp();
            if (!showdamage)
            {
                showdamage = true;
                bodyRender.color = Color.red;
                Invoke("ShowDamageEffect", 0.3f);
            }
        }
        if (headBar != null) headBar.ReduceHp(-(int)(damageValue) , damageType);
    }

    public void UpdateHp()
    {
        if (headBar != null) headBar.UpdateHp(actorData.currHp, actorData.cfgVo.MaxHp);
        if ((ActorType)actorData.cfgVo.Type == ActorType.Boss) EventCenter.DispatchEvent(EventEnum.UpdateBossHp , (float)actorData.currHp/(float)actorData.cfgVo.MaxHp);
        UpdateAnger();
    }

    public void UpdateAnger()
    {
        if (headBar != null) headBar.UpdateEnergy(actorData.currAnger , actorData.cfgVo.RageMax , actorData.angerFull);
    }

    void ShowDamageEffect()
    {
        showdamage = false;
        if (buffList.ContainsKey(3))
        {
            bodyRender.color = new Color(0.03f, 0.62f , 0.94f , 1);
        }
        else
        {
            bodyRender.color = Color.white;
        }
    }

    public void Revie()
    {
        IsDead = false;
        if (actorData.uniqueId == GameData.myData.uniqueId)
        {
            DataManager.userData.IsDead = false;
        }
        bodyRender.sortingLayerName = "Scene";
        bodyRender.color = Color.white;
        actorData.currHp = actorData.cfgVo.MaxHp;
        UpdateHp();
        obstacleCollider.isTrigger = false;
        gameObject.SetActive(true);
        gameObject.transform.position = diePos;
        gameObject.transform.localScale = Vector3.one;
        GameData.allUnits.Add(this);
        GameData.friends.Add(this);
        if (behaviorTree != null) behaviorTree.EnableBehavior();
        if (circleCollider2D != null) circleCollider2D.isTrigger = false;
        if (headBar != null)
        {
            headBar.gameObject.SetActive(true);
            headBar.hpBar.gameObject.SetActive(true);
        }
        if (headSkill != null) headSkill.gameObject.SetActive(true);
        animationManager.Play(AnimationName.Idle);
    }

    public void DamageDie()
    {
        Die();
        diePos = transform.position;
        StartCoroutine(DelayRemove());
    }

    IEnumerator DelayRemove()
    {
        yield return null;
        animationManager.Play(AnimationName.Death);
        yield return new WaitForSeconds(1.0f);
        if (headBar != null) headBar.gameObject.SetActive(false);
        Over();
    }

    private void Over()
    {
        if (GameData.myData.uniqueId == actorData.uniqueId)
        {
            if (SceneManager.Instance.sceneType == SceneType.Home)
            {
                SceneManager.Instance.EnterHome();
            }
            else
            {
                WindowManager.Instance.OpenWindow(WindowKey.LevelResultView, new object[] { LevelResultEnum.Fail });
            }
        }
    }

    virtual protected void Die()
    {
        StopAllCoroutines();
        IsDead = true;
        RemoveAllBuff();
        bodyRender.sortingLayerName = "TerrainUp";
        bodyRender.color = Color.white;
        moveState = attackState = false;
        GameData.boss.Remove(this);
        GameData.enemys.Remove(this);
        GameData.friends.Remove(this);
        GameData.allUnits.Remove(this);
        addSpeed = reduceSpeed = 1.0f;
        UpdateSpeed();
        obstacleCollider.isTrigger = true;
        if (rigidbody2 != null) rigidbody2.velocity = Vector2.zero;
        SceneManager.Instance.RandomDrop(actorData.cfgVo.Drop, transform.position);

        if (master != null && master.pets.Contains(this))
        {
            master.pets.Remove(this);
        }

        if (actorData.uniqueId == GameData.myData.uniqueId)
        {
            DataManager.userData.IsDead = true;
        }

        if (behaviorTree != null) behaviorTree.DisableBehavior(true);
        if (circleCollider2D != null) circleCollider2D.isTrigger = true;
        if (headBar != null) headBar.hpBar.gameObject.SetActive(false);
        if (headSkill != null) headSkill.gameObject.SetActive(false);
        if (movement != null) movement.Stop();
    }

    void OnDestroy()
    {
        IsDead = true;


        GameData.boss.Remove(this);
        GameData.enemys.Remove(this);
        GameData.friends.Remove(this);
        GameData.allUnits.Remove(this);

        if (headBar != null)
        {
            GameObject.Destroy(headBar.gameObject);
        }
        if (headSkill != null)
        {
            GameObject.Destroy(headSkill.gameObject);
        }
    }

    public Vector3 currPos
    {
        get
        {
            return mTrans.position + (Vector3)circleCollider2D.offset;
        }
    }

    public bool isFly
    {
        get
        {
            return actorData.cfgVo.Fly == 1;
        }
    }
}
