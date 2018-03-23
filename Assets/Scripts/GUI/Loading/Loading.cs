using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Loading : Singleton<Loading>
{
    public GameObject startGame;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.GameInitOver , OnGameInitOver);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.GameInitOver, OnGameInitOver);
    }

    public void OnGameInitOver(EventCenterData data)
    {
        startGame.SetActive(true);
    }

    public void OnSingleClick()
    {
        if (UIManager.Instance.HasView(WindowKey.UIRoot))
        {
            startGame.SetActive(false);
            SceneManager.Instance.Enter();
        }
    }

    public void FadeDisable(TweenCallback callBack)
    {
        DOTween.To(() => 1.0f, x => canvasGroup.alpha = x, 0.0f, 0.7f).SetEase(Ease.InExpo).OnComplete( () => { callBack(); gameObject.SetActive(false); });
    }

    public void Reset(bool reload = false)
    {
        startGame.SetActive(reload);
        gameObject.SetActive(true);
        canvasGroup.alpha = 1;
    }
}
