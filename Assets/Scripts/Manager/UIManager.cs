using UnityEngine;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, GameObject> viewDic = new Dictionary<string, GameObject>();

    public void OpenView(string key)
    {
        GameObject wind;
        if (viewDic.TryGetValue(key, out wind))
        {
            wind.SetActive(true);
        }
        else
        {
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", ab =>
            {
                wind = (GameObject)GameObject.Instantiate(ab.LoadAsset(key + ".prefab") , transform);
                viewDic[key] = wind;
            });
        }
    }

    public void CloseView(string key)
    {
        GameObject o;
        if (viewDic.TryGetValue(key, out o))
        {
            o.SetActive(false);
        }
    }

    public bool HasView(string key)
    {
        return viewDic.ContainsKey(key);
    }
}