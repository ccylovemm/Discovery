using UnityEngine;
using System.Collections.Generic;

public class ObstacleTransparente : MonoBehaviour
{
    private SpriteRenderer lastRenderer;
    private float checkTime = 0;

    void Update()
    {
        if (Time.time < checkTime) return;
        checkTime = Time.time + 0.2f;
        RaycastHit2D hit = Physics2D.Linecast(transform.position , transform.position , LayerMask.GetMask("ObstacleTransparente"));
        if (hit.transform != null)
        {
            SpriteRenderer render = hit.transform.GetComponentInChildren<SpriteRenderer>();
            if (lastRenderer != null && lastRenderer != render)
            {
                lastRenderer.color = new Color(1, 1, 1, 1);
            }

            if (render != null)
            {
                render.color = new Color(1, 1, 1, 0.5f);
                lastRenderer = render;
            }
        }
        else
        {
            if (lastRenderer != null)
            {
                lastRenderer.color = new Color(1 , 1 , 1 , 1);
            }
            lastRenderer = null;
        }
    }
}