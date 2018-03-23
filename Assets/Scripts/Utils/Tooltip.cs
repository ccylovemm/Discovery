using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : Singleton<Tooltip>
{
    public Text tip1;
    public Text tip2;
    public GameObject tipObj;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            HideTip();
        }
    }

    public void ShowTip(string str)
    {
        tip1.text = str;
        tip2.text = str;
        tipObj.SetActive(true);
        Vector2 pos = Vector2.one;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform , Input.mousePosition, canvas.worldCamera, out pos);
        tipObj.transform.localPosition = pos + Vector2.up * 60;
    }

    public void HideTip()
    {
        tipObj.SetActive(false);
    }
}
