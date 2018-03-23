using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrainBuff : MonoBehaviour
{
    public int buffId;
    public MapEditorItemType terrainType;
    private List<ActorObject> actorList = new List<ActorObject>();

    private GameObject terrainSurfaceEffect;

    private void Start()
    {
        ResourceManager.Instance.LoadAsset("resourceassets/terrainEffect.assetbundle", effect =>
        {
            if (terrainType == MapEditorItemType.Water)
            {
                terrainSurfaceEffect = (GameObject)effect.LoadAsset("waterWave.prefab");
            }
        });
    }

    private void Update()
    {
        DamageTarget();
    }

    private void OnDestroy()
    {
        actorList.Clear();
        actorList = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null && !actorObject.isFly)
        {
            actorList.Add(actorObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null)
        {
            int index = actorList.IndexOf(actorObject);
            if (index != -1)
            {
                actorList.RemoveAt(index);
            } 
        }
    }

    private void DamageTarget()
    {
        for (int i = actorList.Count - 1; i >= 0; i --)
        {
            if (terrainType == MapEditorItemType.Water && MapWaterFrezon.IsFrezon(MapManager.GetGrid(actorList[i].transform.position)))
            {
                continue;
            }

            if (actorList[i].IsDisappear || actorList[i].IsDead)
            {
                continue;
            }
           
            if (SceneManager.Instance.TerrainIn(actorList[i].transform.position , terrainType))
            {
                if (Time.time > actorList[i].terrainBuffTime)
                {
                    actorList[i].AddBuff(null , buffId.ToString() , "");
                    actorList[i].terrainBuffTime = Time.time + 0.5f;
                    if(terrainSurfaceEffect != null)
                    {
                        GameObject.Destroy(GameObject.Instantiate(terrainSurfaceEffect, actorList[i].transform.position , Quaternion.identity) , 1);
                    }
                }
            }
        }
    }
}
