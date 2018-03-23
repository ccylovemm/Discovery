using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMagic : MagicBase
{
    public GameObject shield1;
    public GameObject shield2;

    public int angle = 30;

    private int shieldCount = 4;
    private List<GameObject> shields = new List<GameObject>();

    override protected void Start()
    {
        base.Start();

        for (int i = 0; i < shieldCount; i++)
        {
            shields.Add(GameObject.Instantiate(shield1));
        }

        for (int i = 0; i < shieldCount; i++)
        {
            shields[i].transform.position = caster.transform.position + Quaternion.AngleAxis(((1 - 4 + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized * skillVo.ShotRange * 0.15f;
        }
    }

    override protected void Update()
    {
        base.Update();
        for (int i = 0; i < shieldCount; i++)
        {
            shields[i].transform.position = caster.transform.position + Quaternion.AngleAxis(((1 - 4 + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized * skillVo.ShotRange * 0.15f;
        }
    }

    override public void MagicDestory()
    {
        for(int i = 0; i < shieldCount; i ++)
        {
            GameObject.Destroy(shields[i]);
        }
        shields.Clear();
        for (int i = 0; i < shieldCount; i++)
        {
            GameObject.Destroy(GameObject.Instantiate(shield2, caster.transform.position + Quaternion.AngleAxis(((1 - 4 + 2 * i) / 2.0f) * angle, Vector3.forward) * effectDirect.normalized * skillVo.ShotRange * 0.15f, Quaternion.identity) , skillVo.Duration);   
        }
        GameObject.Destroy(gameObject);
    }
}
