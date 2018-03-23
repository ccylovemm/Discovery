using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMagic : MagicBase
{
    public GameObject headEffect;

    public int count = 1;

    override protected void Start()
    {
        if (caster.pets.Count > 2)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            base.Start();
            Vector2 pos = MapManager.FindPosByRange(caster.transform.position, skillVo.ShotRange * MapManager.textSize, MapManager.textSize);
            SceneManager.Instance.CreateFriend(new ActorData((uint)skillVo.SkillValue), pos, caster);
            GameObject effect = GameObject.Instantiate(effectPrefab);
            effect.transform.position = pos;
            GameObject.Destroy(effect, skillVo.Duration);
            if (headEffect != null)
            {
                GameObject.Destroy(GameObject.Instantiate(headEffect, transform), 3);
            }
        }
    }
}
