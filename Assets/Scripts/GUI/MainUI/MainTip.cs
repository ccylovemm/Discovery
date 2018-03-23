using UnityEngine;
using UnityEngine.UI;

public class MainTip : MonoBehaviour
{
    public Text msg1Txt;
    public Text msg2Txt;
    public Text worldName;
    public Text levelName;

    public GameObject msg1;
    public GameObject msg2;
    public GameObject levelTip;

    private float missTime = 0;

    private void OnEnable()
    {
        EventCenter.AddEvent(EventEnum.EnterLevel, OnEnterLevel);
    }

    private void OnDisable()
    {
        EventCenter.RemoveEvent(EventEnum.EnterLevel, OnEnterLevel);
    }

    void Update()
    {
        if (Time.time > missTime)
        {
            msg1.SetActive(false);
            msg2.SetActive(false);
            levelTip.SetActive(false);
        }
    }

    public void SetMsg1(string str, float time = 100000.0f)
    {
        missTime = Time.time + time;
        msg1Txt.text = str;
        msg1.SetActive(true);
    }

    public void SetMsg2(string str , float time = 1.0f)
    {
        missTime = Time.time + time;
        msg2Txt.text = str;
        msg2.SetActive(true);
    }

    public void OnEnterLevel(EventCenterData data)
    {
        missTime = Time.time + 1.0f;
        levelTip.SetActive(true);
        worldName.text = LanguageManager.GetText(SceneManager.Instance.currLevelVo.Desc);
        levelName.text = DataManager.userData.Group + "-" + DataManager.userData.GroupLevel;
    }

    public void Clear()
    {
        msg1.SetActive(false);
        msg2.SetActive(false);
        levelTip.SetActive(false);
    }
}
