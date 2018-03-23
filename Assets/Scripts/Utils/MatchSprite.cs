using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSprite : MonoBehaviour
{
    public bool autoMatch = true;
    public SpriteRenderer render;
    public List<Vector2> posList = new List<Vector2>();

    private void Awake()
    {
        if (autoMatch)
        {
            render.transform.position = new Vector3(render.sprite.bounds.size.x / 2.0f, render.sprite.bounds.size.y / 2.0f);
        }
    }
}
