using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMView : MonoBehaviour
{
    public Dropdown dropdown;

    private void Awake()
    {
       
        List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>();
        foreach (var level in LevelCFG.items)
        {
            datas.Add(new Dropdown.OptionData("关卡 " + level.Value.MapId + "_" + level.Value.Level));
        }
        dropdown.AddOptions(datas);
    }

    public void BackHome()
    {
        SceneManager.Instance.EnterHome();
    }

    public void ResetMap()
    {
        SceneManager.Instance.Enter();
    }

    public void ClearData()
    {
        DataManager.Clear();
        Application.Quit();
    }

    public void AddCoin()
    {
        DataManager.userData.GoldCoin += 1000;
    }

    public void ReduceCoin()
    {
        if (DataManager.userData.GoldCoin > 1000)
        {
            DataManager.userData.GoldCoin -= 1000;
        }
        else
        {
            DataManager.userData.GoldCoin = 0;
        }
    }

    public void AddDiamond()
    {
        DataManager.userData.Diamond += 1000;
    }

    public void ReduceDiamond()
    {
        if (DataManager.userData.Diamond > 1000)
        {
            DataManager.userData.Diamond -= 1000;
        }
        else
        {
            DataManager.userData.Diamond = 0;
        }
    }

    public void RandomDropElement()
    {
        SceneManager.Instance.RandomDropItem((uint)Random.Range(1 , 9) + 20000, 1, GameData.myself.transform.position);
    }

    public void OpenTestView()
    {
        WindowManager.Instance.OpenWindow(WindowKey.TestView);
    }

    public void DropPotion()
    {
        SceneManager.Instance.RandomDropItem((uint)Random.Range(30001 , 30004) , 1 , GameData.myself.transform.position);
    }

    public void GoToLevel()
    {
        string str = dropdown.captionText.text;
        if (!string.IsNullOrEmpty(str))
        {
            string[] s = str.Split(' ')[1].Split('_');
            uint group = uint.Parse(s[0]);
            uint level = uint.Parse(s[1]);
            DataManager.userData.Group = group;
            DataManager.userData.GroupLevel = level;
            SceneManager.Instance.Enter();
        }
    }

    public void Close()
    {
        WindowManager.Instance.CloseWindow(WindowKey.GMView);
    }
}
