using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeadBar : MonoBehaviour
{
    public HpBar hpBar;
    public GameObject MsgTxt1;
    public GameObject MsgTxt2;
    public GameObject MsgTxt3;
    public GameObject MsgTxt4;

    public Transform target;

    void LateUpdate()
    {
        Vector3 pos = CameraManager.Instance.mainCamera.WorldToScreenPoint(target.position);
        transform.position = pos;
    }

    public void UpdateHp(uint curr , uint max)
    {
        hpBar.SetData((float)curr / (float)max);
    }

    virtual public void UpdateEnergy(uint curr, uint max , bool isFull)
    {

    }

    public void AddHp(int value)
    {
        if (value == 0) return;
        ShowMsg((value > 0 ? "+" : "") + value.ToString(), MsgTxt4);
    }

    public void ReduceHp(int value)
    {
        if (value == 0) return;
        value = Random.Range((int)(value * 0.8f) , (int)(value * 1.2f));
        ShowMsg(value.ToString() , MsgTxt1);
    }

    private void ShowMsg(string msg , GameObject txt)
    {
        GameObject obj = GameObject.Instantiate(txt, transform);
        obj.GetComponent<Text>().text = msg;
        GameObject.Destroy(obj, 0.5f);
        DOTween.To(() => Vector3.zero , x => obj.transform.localPosition = x, Vector3.up * 30 , 0.4f);
    }
}
