using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMagic : MagicBase
{
    static public List<Bomb> allBombList = new List<Bomb>();

    public int angle = 30;
    private List<Bomb> effectList = new List<Bomb>();
    private bool plane;
    private int count;
    private bool isDestroy = false;

    override protected void Start()
    {
        if (caster.allBombList.Count >= 3)//策划不需要配置数量  2018年1月12日13:33:17
        {
            GameObject.Destroy(caster.allBombList[0].gameObject);
        }
        caster.allBombList.Add(this);

        if (caster.isAI)
        {
            if (skillVo.Duration > 0) GameObject.Destroy(gameObject, skillVo.Duration);
        }
        musicPlayer = SoundManager.Instance.GetMuiscPlayer();
        musicPlayer.Play(skillVo.MusicStart, false);
        musicPlayer.Play(skillVo.MusicLoop, true, false);
        LoadEffect();
        AutoLockTarget();
        effectDirect = direct;
        caster.PlayAttackAnimation();

        count = (int)skillVo.SkillValue;
        for (int i = 0; i < count; i ++)
        {
            Bomb bomb = GameObject.Instantiate(effectPrefab).GetComponent<Bomb>();
            bomb.bombMagic = this;
            effectList.Add(bomb);
            allBombList.Add(bomb);
        }
    }

    override protected void Update()
    {
        if (plane) return;
        base.Update();
        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].transform.position = caster.transform.position + Quaternion.AngleAxis(((1 - count + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized * skillVo.ShotRange * MapManager.textSize;
        }
    }

    override public void MagicDestory()
    {
        plane = true;
        for (int i = 0; i < effectList.Count; i++)
        {
            effectList[i].PlaneBomb(skillVo , caster);
        }
    }

    override protected void OnDestroy()
    {
        isDestroy = true;
        base.OnDestroy();
        caster.allBombList.Remove(this);
        for (int i = 0; i < effectList.Count; i ++)
        {
            BombMagic.allBombList.Remove(effectList[i]);
            GameObject.Destroy(effectList[i].gameObject);
        }
    }

    public void Bomb(Bomb bomb)
    {
        if (isDestroy) return;
        effectList.Remove(bomb);
        if (effectList.Count == 0)
        {
            base.MagicDestory();
        }
    }
}
