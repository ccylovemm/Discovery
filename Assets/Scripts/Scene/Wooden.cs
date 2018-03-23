using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wooden : MonoBehaviour
{ 
    static public Dictionary<Vector2, Wooden> woodenPos = new Dictionary<Vector2, Wooden>();
    static public Dictionary<Vector2, Wooden> fireWoodenPos = new Dictionary<Vector2, Wooden>();

    public GameObject fireEffect;
    public GameObject deFireEffect;
    public GameObject fireOverEffect;
    public GameObject smashEffect;

    private Vector2 gridPos;
    private float fireTime;
    private bool isFire = false;
    private GameObject fireEffect_;

    private Dictionary<uint, int> actorList = new Dictionary<uint, int>();
    private Dictionary<Vector2, int> woodens = new Dictionary<Vector2, int>();

    private void Start()
    {
        gridPos = MapManager.GetGrid(transform.position);
        woodenPos.Add(gridPos, this);
    }

    private void Update()
    {
        if (isFire)
        {
            FireNearWooden();
            FireNearActor();
            if (Time.time - fireTime > 3)
            {
                FireOver();
            }
        }
    }

    private void FireNearWooden()
    {
        for (int i = -1; i < 2; i ++)
        {
            for (int j = -1; j < 2; j ++)
            {
                if (i == 0 && j == 0) continue;
                Vector2 pos = new Vector2(gridPos.x + i, gridPos.y + j);
                if (Wooden.woodenPos.ContainsKey(pos) && !Wooden.fireWoodenPos.ContainsKey(pos))
                {
                    if (!woodens.ContainsKey(pos))
                    {
                        woodens.Add(pos, 100);
                    }
                    woodens[pos] -= 1;
                    if (woodens[pos] < 0)
                    {
                        woodens.Remove(pos);
                        Wooden.woodenPos[pos].Fire();
                    }
                }
            }
        }        
    }

    private void FireNearActor()
    {
        for (int i = 0; i < GameData.allUnits.Count; i ++)
        {
            ActorObject actorObject = GameData.allUnits[i];
            if ((actorObject.currPos - transform.position).magnitude - actorObject.circleCollider2D.radius < MapManager.textSize * 1)
            {
                if (!actorList.ContainsKey(actorObject.actorData.uniqueId))
                {
                    actorList.Add(actorObject.actorData.uniqueId, 100);
                }
                actorList[actorObject.actorData.uniqueId] -= 1;
                if (actorList[actorObject.actorData.uniqueId] < 0)
                {
                    actorObject.AddBuff(null , 1 , "");
                }
            }
        }
    }

    private void OnDestroy()
    {
        woodens.Clear();
        actorList.Clear();
        woodenPos.Remove(gridPos);
        fireWoodenPos.Remove(gridPos);
    }

    public void Fire()
    {
        isFire = true;
        fireTime = Time.time;
        fireWoodenPos.Add(gridPos, this);
        fireEffect_ = GameObject.Instantiate(fireEffect , transform);
    }

    public void DeFire()
    {
        woodens.Clear();
        actorList.Clear();
        if (isFire)
        {
            GameObject.Destroy(fireEffect_);
            GameObject.Destroy(GameObject.Instantiate(deFireEffect, transform) , 0.5f);
        }
        isFire = false;
        fireWoodenPos.Remove(gridPos);
    }

    public void FireOver()
    {
        GameObject.Destroy(GameObject.Instantiate(fireOverEffect, transform.position, Quaternion.identity), 2);
        Break();
    }

    public void Smash()
    {
        GameObject.Destroy(GameObject.Instantiate(smashEffect, transform.position, Quaternion.identity), 2);
        Break();
        Drop();
    }

    private void Break()
    {
        woodenPos.Remove(gridPos);
        fireWoodenPos.Remove(gridPos);
        GameObject.Destroy(fireEffect_);
        GameObject.Destroy(gameObject);
        MapManager.SetPathData(gridPos);
        MapManager.SetPathHoleData(gridPos);
    }

    private void Drop()
    {
        SceneManager.Instance.RandomDrop(SceneManager.Instance.currLevelVo.WoodenDrop , transform.position);
    }
}
