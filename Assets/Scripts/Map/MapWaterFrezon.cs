using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWaterFrezon : MonoBehaviour
{
    static public Dictionary<Vector2, MapWaterFrezon> frezonPos = new Dictionary<Vector2, MapWaterFrezon>();

    static public bool IsFrezon(Vector2 grid)
    {
        return frezonPos.ContainsKey(grid);
    }

    public GameObject frezonStatus1;
    public GameObject frezonStatus2;

    private float deFrezonTime;
    private float destoryTime;
    private Vector2 grid;

    private bool fireDefrezon = false;

    void Start()
    {
        deFrezonTime = Time.time + 0.2f;
        destoryTime = deFrezonTime + 0.5f;
        grid = MapManager.GetGrid(transform.position);
    }

    void Update()
    {
        if (Time.time > deFrezonTime)
        {
            frezonStatus1.SetActive(false);
            frezonStatus2.SetActive(true);
            if (Time.time > destoryTime)
            {
                frezonPos.Remove(grid);
                GameObject.Destroy(gameObject);
            }
        }
        else if (deFrezonTime - Time.time < 1.5f)
        {
            frezonStatus1.SetActive(false);
            frezonStatus2.SetActive(true);
        }
        else
        {
            frezonStatus1.SetActive(true);
            frezonStatus2.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        frezonPos.Remove(grid);
    }

    public void ReFrezon()
    {
        fireDefrezon = false;
        deFrezonTime += 0.2f;
        deFrezonTime = Mathf.Min(deFrezonTime , Time.time + 10);
        destoryTime = deFrezonTime + 0.5f;
    }

    public void FireDeFrezon()
    {
        if (fireDefrezon) return;
        fireDefrezon = true;
        deFrezonTime = Time.time;
        destoryTime = deFrezonTime + 0.2f;
    }
}
