using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBase : MonoBehaviour
{
    public GameObject directObj;
    public GameObject shotEffect;
    public GameObject hitEffect;
    public GameObject effectPrefab;

    [HideInInspector]
    public ActorObject caster;

    public SkillLevelVo skillVo;

    protected float damageTime;

    protected Vector2 effectDirect;
    protected Vector2 direct;

    protected MusicPlayer musicPlayer;

    protected GameObject hitEffect_;
    protected GameObject directObj_;
    protected GameObject shotEffect_;
    protected GameObject effectDisplay;

    protected ActorObject target;


    virtual protected void Awake()
    {
        shotEffect_ = new GameObject();
        hitEffect_ = new GameObject();
        effectDisplay = new GameObject();
    }

    virtual protected void Start()
    {
        if (skillVo.Duration > 0) GameObject.Destroy(gameObject, skillVo.Duration);
        musicPlayer = SoundManager.Instance.GetMuiscPlayer();
        musicPlayer.Play(skillVo.MusicStart, false);
        musicPlayer.Play(skillVo.MusicLoop, true, false);
        LoadEffect();
        AutoLockTarget();
        effectDirect = direct;
        PlayerAttackAnimation();
        if(caster == GameData.myself)
        {
            EventCenter.DispatchEvent(EventEnum.ResetAttackJoystickPos, direct);
        }
    }

    virtual protected void PlayerAttackAnimation()
    {
        caster.PlayAttackAnimation();
    }

    virtual protected void Update()
    {
        if (caster.isAI)
        {
            AutoLockTarget();

            if (direct.x > 0)
            {
                caster.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (direct.x < 0)
            {
                caster.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }

        shotEffect_.transform.position = (Vector2)caster.attackPos.position + effectDirect.normalized * 0.04f;

        for (int i = 0; i < 30; i ++)
        {
            effectDirect += (direct - effectDirect).normalized * 0.004f;
        }
    }

    virtual public void UpdateDirect(Vector2 value)
    {
        direct = value;
    }

    virtual public void MagicDestory() 
    {
        GameObject.Destroy(directObj_);
        GameObject.Destroy(gameObject);
    }

    virtual protected void OnDestroy()
    {
        if (musicPlayer != null)
        {
            musicPlayer.Play(skillVo == null ? "" : skillVo.MusicOver, false, true);
            musicPlayer.use = false;
        }
        if (caster != null && caster.magicBase == this)
        {
            caster.ClearMagic();
        }
        GameObject.Destroy(effectDisplay);
        GameObject.Destroy(shotEffect_);
        GameObject.Destroy(hitEffect_);
    }

    protected void AutoLockTarget()
    {
        FindNearEnemy();
        if (target != null)
        {
            direct = target.currPos - caster.currPos;
        }
        else
        {
            direct = caster.transform.eulerAngles.y == 0 ? Vector2.right : Vector2.left;
        }
    }

    protected void LoadEffect()
    {
        if (shotEffect != null)
        {
            GameObject.Instantiate(shotEffect, Vector3.zero, Quaternion.identity, shotEffect_.transform);
        }

        if (hitEffect != null)
        {
            GameObject.Instantiate(hitEffect, Vector3.zero, Quaternion.identity, hitEffect_.transform);
        }
    }

    protected void FindNearEnemy()
    {
        float minDistance = 5 * MapManager.textSize;
        List<ActorObject> enemys = GameData.GetTarget(caster);
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i].IsDead || enemys[i].IsDisappear) continue;
            float distance = CommonUtil.Distance(caster, enemys[i]);
            if (minDistance > distance)
            {
                target = enemys[i];
                minDistance = distance;
            }
        }
    }

    protected void FindEnemy()
    {
        target = null;
        List<ActorObject> enemys = GameData.GetTarget(caster);
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i].IsDead || enemys[i].IsDisappear) continue;
            float distance = CommonUtil.Distance(caster , enemys[i]);
            if (distance < skillVo.ShotRange)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(caster.transform.position, enemys[i].transform.position - caster.transform.position, distance, LayerUtil.WallMasks());
                if (raycastHit2D.collider == null)
                {
                    target = enemys[i];
                    break;
                }
            }
        }
    }
}
