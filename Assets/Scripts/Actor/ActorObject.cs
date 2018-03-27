using System.Collections;
using UnityEngine;

public class ActorObject : SceneBase
{
    public Transform attackPos;
    public Collider2D obstacleCollider;

    [HideInInspector]
    public HeadBar headBar;//头顶血条
    [HideInInspector]
    public ActorObject targetObject;//锁定对象
    [HideInInspector]
    public NavMeshAgent2D navMeshAgent2D;
    [HideInInspector]
    public AnimationManager animationManager;

    [HideInInspector]
    public bool IsDead;

    public ActorData actorData;

    protected Transform mTrans;
    protected ActionStatus actionStatus = ActionStatus.Idle;

    private SpriteRenderer bodyRender;
    private bool showdamage = false;

    virtual protected void Awake()
    {
        mTrans = transform;
        navMeshAgent2D = GetComponent<NavMeshAgent2D>();
        animationManager = GetComponent<AnimationManager>();
        bodyRender = GetComponentInChildren<SpriteRenderer>();
    }

    virtual protected void Start()
    {
        ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
        {
            headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("HeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
            headBar.target = transform.Find("HeadPos");
            UpdateHp();
        });
    }

    protected void UpdateDirect(Vector2 value)
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

    public void ExecuteAttack()
    {
        if (targetObject != null && !targetObject.IsDead && Vector3.Distance(transform.position, targetObject.transform.position) < actorData.cfgVo.FindRange * MapManager.textSize)
        {
            targetObject.ReduceHp(actorData.cfgVo.Attack);
        }
    }

    public void ReduceHp(uint damageValue)
    {
        if (IsDead) return;

        if (damageValue >= actorData.currHp)
        {
            actorData.currHp = 0;
            UpdateHp();
            Die();
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
        if (headBar != null) headBar.ReduceHp(-(int)(damageValue));
    }

    public void UpdateHp()
    {
        if (headBar != null) headBar.UpdateHp(actorData.currHp, actorData.cfgVo.MaxHp);
    }

    private void ShowDamageEffect()
    {
        showdamage = false;
        bodyRender.color = Color.white;
    }

    private void Die()
    {
        StopAllCoroutines();
        IsDead = true;
        bodyRender.sortingLayerName = "TerrainUp";
        bodyRender.color = Color.white;
        GameData.enemys.Remove(this);
        obstacleCollider.isTrigger = true;
        if (navMeshAgent2D != null) navMeshAgent2D.Stop();
        if (headBar != null) headBar.hpBar.gameObject.SetActive(false);
        StartCoroutine(DelayRemove());
    }

    IEnumerator DelayRemove()
    {
        yield return null;
        animationManager.Play(AnimationName.Dead);
        yield return new WaitForSeconds(1.0f);
        if (headBar != null) headBar.gameObject.SetActive(false);
        GameObject.Destroy(gameObject , 10);
    }
}
