using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public BuffVo buffVo;
    public ActorObject target;
    public ActorObject caster;
    public string damageAttributeStr = "";

    public float lifeTime;

    private GameObject effect;
    private float checkTime;
    private Vector3 lastPos = Vector3.zero;

    private bool isDestory = false;

    private void Start()
    {
        ExcuteBuff();
        ShowDisplay();
    }


    private void Update()
    {
        if ((BuffType)buffVo.Type == BuffType.Damage || (BuffType)buffVo.Type == BuffType.Drug)
        {
            if (Time.time >= checkTime)
            {
                checkTime = Time.time + buffVo.Interval;
                target.ReduceHpFromBuff(caster , (uint)buffVo.Value , damageAttributeStr);
            }
        }
        else if ((BuffType)buffVo.Type == BuffType.Wet)
        {
            if (Time.time >= checkTime && Vector2.Distance(target.transform.position , lastPos) > 0.15f)
            {
                checkTime = Time.time + 0.05f;
                Vector3 direct = target.transform.position - lastPos;
                lastPos = target.transform.position;
                if (!SceneManager.Instance.TerrainIn(lastPos))
                {
                    ResourceManager.Instance.LoadAsset("resourceassets/terrainEffect.assetbundle", ab =>
                    {
                        GameObject go = ((GameObject)GameObject.Instantiate(ab.LoadAsset("footprintWet.prefab")));
                        go.transform.position = lastPos;
                        go.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(direct.normalized, direct.y > 0 ? Vector2.up : Vector2.down));
                        GameObject.Destroy(go , 2);
                    });
                }
            }
        }

        if (Time.time >= lifeTime)
        {
            if ((BuffType)buffVo.Type == BuffType.Frozen)
            {
                target.AddBuff(caster , buffVo.ParamValue , damageAttributeStr);
            }
            target.DeleteBuff(buffVo.Id);
        }
    }

    private void OnDestroy()
    {
        isDestory = true;
        DissBuff();
        if (effect != null)
        {
            GameObject.Destroy(effect);
        }
    }

    private void ExcuteBuff()
    {
        if ((BuffType)buffVo.Type == BuffType.Cold)
        {
            target.bodyRender.color = new Color(0.03f, 0.62f, 0.94f, 1);
            target.ReduceSpeed(buffVo.Value);
        }
        else if ((BuffType)buffVo.Type == BuffType.Muddy)
        {
            target.ReduceSpeed(buffVo.Value);
        }
        else if ((BuffType)buffVo.Type == BuffType.Shock || (BuffType)buffVo.Type == BuffType.Frozen || (BuffType)buffVo.Type == BuffType.DeFrozen)
        {
            target.SetFrozen(true);
        }
        else if((BuffType)buffVo.Type == BuffType.Dizzy)
        {
            target.SetDizzy(true);
        }
        else if ((BuffType)buffVo.Type == BuffType.Speed)
        {
            target.AddSpeed(buffVo.Value);
        }
    }

    private void DissBuff()
    {
        if ((BuffType)buffVo.Type == BuffType.Cold || (BuffType)buffVo.Type == BuffType.Muddy)
        {
            target.bodyRender.color = Color.white;
            target.ReduceSpeed(buffVo.Value * -1);
        }
        else if ((BuffType)buffVo.Type == BuffType.Frozen)
        {
            if (!target.HasBuffType(BuffType.DeFrozen))
            {
                target.SetFrozen(false);
            }
        }
        else if ((BuffType)buffVo.Type == BuffType.Shock || (BuffType)buffVo.Type == BuffType.DeFrozen)
        {
            target.SetFrozen(false);
        }
        else if ((BuffType)buffVo.Type == BuffType.Dizzy)
        {
            target.SetDizzy(false);
        }
        else if ((BuffType)buffVo.Type == BuffType.Speed)
        {
            target.AddSpeed(buffVo.Value * -1);
        }
    }

    private void ShowDisplay()
    {
        if (!string.IsNullOrEmpty(buffVo.Display))
        {
            ResourceManager.Instance.LoadAsset("resourceassets/buff.assetbundle", ab =>
            {
                if (isDestory) return;
                effect = (GameObject)GameObject.Instantiate(ab.LoadAsset(buffVo.Display + ".prefab"), transform);
            });
        }
    }
}
