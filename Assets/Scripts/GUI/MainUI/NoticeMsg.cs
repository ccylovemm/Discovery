using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class NoticeMsg : MonoBehaviour {

	public GameObject MsgTxt;

	private bool isShowing = false;
	private float lastTime = 0.0f;
	private List<string> msgStrList = new List<string>();

	void OnEnable()
	{
        EventCenter.AddEvent(EventEnum.ShowMsg, OnShowMsg);
	}

	void OnDisable()
	{
		isShowing = false;
        EventCenter.RemoveEvent(EventEnum.ShowMsg, OnShowMsg);
    }

	void Update()
	{
		if(Time.frameCount % 8 == 0)return;
		if(msgStrList.Count == 0)return;
		if (Time.time - lastTime > 0.3) 
		{
			PopMsg(msgStrList[0]);
			msgStrList.RemoveAt(0);
		}
	}

	public void OnShowMsg(EventCenterData data)
	{
        string str = data.data as string;
        if (Time.time - lastTime > 0.3)
		{
			PopMsg(str);
		}
		else
		{
			if(msgStrList.Count > 0)
			{
				if(msgStrList.Where(s => s.Equals(str)).Count<string>() <= 1)
				{
					msgStrList.Add(str);
				}
			}
			else
			{
				msgStrList.Add(str);
			}
		}
	}

    private void PopMsg(string str, uint type = 1)
    {
        lastTime = Time.time;
        GameObject obj = GameObject.Instantiate(MsgTxt, transform);
        obj.GetComponent<Text>().text = str;
        DOTween.To(() => 0, x => obj.transform.localPosition = new Vector3(0, x, 0), 50, 0.8f);
        GameObject.Destroy(obj , 1.2f);
	}
}
