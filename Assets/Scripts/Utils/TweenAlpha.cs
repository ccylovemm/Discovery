using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweenAlpha : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        DOTween.To(() => 1.0f , x => image.color = new Color(1, 1, 1, x) , 0.0f , 1.0f).SetLoops(-1 , LoopType.Yoyo);
    }
}
