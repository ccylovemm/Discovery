using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class World1BossAppear : MonoBehaviour
{
    public GameObject bg;
    public GameObject head;
    public GameObject guang;
    public GameObject text;
    public GameObject black;

	void OnEnable ()
    {
        black.SetActive(false);
        guang.SetActive(false);
        text.SetActive(false);
        head.SetActive(false);
        bg.transform.localScale = new Vector3(1 , 0.2f , 1);
        text.transform.localPosition = new Vector3(-957 , -377 , 0);
        StartCoroutine(ShowAnimotion());	
	}

    IEnumerator ShowAnimotion()
    {
        bg.transform.DOScale(Vector3.one , 0.3f);
        yield return new WaitForSeconds(0.5f);
        head.SetActive(true);

        text.SetActive(true);
        black.SetActive(true);
        head.transform.DOShakePosition(4 , 2);
        text.transform.DOLocalMove(new Vector3(179 , -177, 0) , 0.2f , true);
        yield return new WaitForSeconds(0.3f);
        guang.SetActive(true);
        guang.transform.DOShakeScale(4 , 0.1f);
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }
}
