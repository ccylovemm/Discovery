using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweenAlpha2 : MonoBehaviour
{
    private SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        DOTween.To(() => 1.0f, x => sprite.color = new Color(1, 1, 1, x), 0.0f, 1.0f);
    }
}
