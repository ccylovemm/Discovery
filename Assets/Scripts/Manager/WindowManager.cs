﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<string, GameObject> windowDic = new Dictionary<string, GameObject>();

    public void OpenWindow(string key, System.Object[] parameters = null)
    {
        GameObject wind;
        if (windowDic.TryGetValue(key, out wind))
        {
            wind.SetActive(true);
            wind.BroadcastMessage("SetParameters", parameters, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", ab =>
            {
                wind = (GameObject)GameObject.Instantiate(ab.LoadAsset(key +".prefab") , transform);
                windowDic[key] = wind;
                wind.BroadcastMessage("SetParameters", parameters, SendMessageOptions.DontRequireReceiver);
            });
        }
    }

    public void OpenBossAppear(string key)
    {
        GameObject wind;
        if (windowDic.TryGetValue(key, out wind))
        {
            wind.SetActive(true);
        }
        else
        {
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", ab =>
            {
                wind = (GameObject)GameObject.Instantiate(ab.LoadAsset(key + ".prefab"));
                wind.transform.parent = transform.parent;
                wind.transform.localScale = Vector3.one;
                wind.transform.localPosition = new Vector3(0, 0, wind.transform.position.z);
                windowDic[key] = wind;
            });
        }
    }

    public void CloseWindow(string key)
    {
        GameObject o;
        if (windowDic.TryGetValue(key, out o))
        {
            o.SetActive(false);
        }
    }

    public void CloseAllWindow()
    {
        foreach (GameObject o in windowDic.Values)
        {
            o.gameObject.SetActive(false);
        }
    }
}