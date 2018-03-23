using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pit : MonoBehaviour
{
    public float delayTime = 1.0f;
    public float showTime = 0.2f;

    private SpriteRenderer render;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        DOTween.To(() => 1.0f, x => render.color = new Color(1, 1, 1, x), 0.0f, showTime).OnComplete(() => { GameObject.Destroy(gameObject); }).SetDelay(delayTime);
        GameObject.Destroy(gameObject , delayTime + showTime);
    }
}
