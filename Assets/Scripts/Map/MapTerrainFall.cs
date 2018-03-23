using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrainFall : MonoBehaviour
{
    private List<Vector2> actorPos = new List<Vector2>();
    private List<ActorObject> actorList = new List<ActorObject>();

    private void Update()
    {
        int count = actorList.Count;
        for (int i = count - 1; i > -1; i --)
        {
            if (SceneManager.Instance.TerrainIsFall(actorList[i].transform.position))
            {
                actorList[i].DropDie(actorPos[i]);
                actorPos.RemoveAt(i);
                actorList.RemoveAt(i);
            }
            else
            {
                actorPos[i] = actorList[i].transform.position;
            }
        }
    }

    private void OnDestroy()
    {
        actorList.Clear();
        actorList = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActorObject actorObject = collision.GetComponent<ActorObject>();
        if (actorObject != null && !actorObject.IsDead && !actorObject.isFly)
        {
            actorList.Add(actorObject);
            actorPos.Add(actorObject.transform.position);
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
                actorPos.RemoveAt(index);
                actorList.RemoveAt(index);
            }
        }
    }
}
