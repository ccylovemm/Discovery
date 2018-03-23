using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMud : MonoBehaviour
{
    static public Dictionary<Vector2, MapMud> mudPos = new Dictionary<Vector2, MapMud>();

    static public bool IsMud(Vector2 grid)
    {
        return mudPos.ContainsKey(grid);
    }

    public GameObject mudStatus1;
    public GameObject mudStatus2;

    private float deMudTime;
    private float destoryTime;
    private Vector2 grid;

    private bool waterWash = false;

    void Start()
    {
        deMudTime = Time.time + 0.2f;
        destoryTime = deMudTime + 0.5f;
        grid = MapManager.GetGrid(transform.position);
    }

    void Update()
    {
        if (Time.time > deMudTime)
        {
            mudStatus1.SetActive(false);
            mudStatus2.SetActive(true);
            if (Time.time > destoryTime)
            {
                mudPos.Remove(grid);
                GameObject.Destroy(gameObject);
            }
        }
        else if (deMudTime - Time.time < 1.5f)
        {
            mudStatus1.SetActive(false);
            mudStatus2.SetActive(true);
        }
        else
        {
            mudStatus1.SetActive(true);
            mudStatus2.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        mudPos.Remove(grid);
    }

    public void ReMud()
    {
        waterWash = false;
        deMudTime += 0.1f;
        deMudTime = Mathf.Min(deMudTime, Time.time + 3);
        destoryTime = deMudTime + 0.5f;
    }

    public void WaterWash()
    {
        if (waterWash) return;
        waterWash = true;
        deMudTime = Time.time;
        destoryTime = deMudTime + 0.2f;
    }
}
