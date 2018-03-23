using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class LevelResult : MonoBehaviour
{
    public Text title;
    public Text level;
    public Text timeTxt;
    public Text lootTxt;
    public Text revieBtnTxt;
    public Text adsBtnTxt;
    public Text leaveBtnTxt;
    public Text revieCostNum;
    public Image revieCostIcon;
    public GameObject revieBtn;
    public GameObject adsBtn;
    public Grid btnGrid;

    public LevelResultEnum result;

    private void Awake()
    {
        timeTxt.text = LanguageManager.GetText("210038");
        lootTxt.text = LanguageManager.GetText("210040");
        revieBtnTxt.text = LanguageManager.GetText("210042");
        adsBtnTxt.text = LanguageManager.GetText("Ads");
        leaveBtnTxt.text = LanguageManager.GetText("210043");
    }

    private void OnEnable()
    {
        btnGrid.gameObject.SetActive(false);
    }

    void SetParameters(object[] args)
    {
        result = (LevelResultEnum)args[0];
        if (result == LevelResultEnum.Victory)
        {
            title.text = LanguageManager.GetText("210031");
            level.text = LanguageManager.GetText("210032");
            revieBtn.SetActive(false);
            adsBtn.SetActive(false);
            revieCostNum.text = "";
            revieCostIcon.gameObject.SetActive(false);
        }
        else
        {
            title.text = LanguageManager.GetText("210035");
            level.text = SceneManager.Instance.currMapVo.Id + "-" + SceneManager.Instance.currLevelVo.Level;
            #if UNITY_IOS || UNITY_ANDROID
            adsBtn.SetActive(Advertisement.IsReady(ConfigData.AdsPlacementId));
            #else
            adsBtn.SetActive(false);
            #endif
            revieBtn.SetActive(true);
            revieCostNum.text = ParameterCFG.items["2"].Value;
            revieCostIcon.gameObject.SetActive(true);

            ResourceManager.Instance.LoadIcon("Icon_creditGoldUI", icon =>
            {
                revieCostIcon.sprite = icon;
            });
        }
        StartCoroutine(ShowBtns());
    }

    IEnumerator ShowBtns()
    {
        yield return new WaitForSeconds(0.5f);
        btnGrid.gameObject.SetActive(true);
        btnGrid.ResetPosition();
    }

    public void OnLeave()
    {
        if (result == LevelResultEnum.Fail)
        {
            GameData.myData.Reset();
            DataManager.userData.Clear();
            SkillManager.FreshSkillLevel();
            SceneManager.Instance.EnterHome();
        }
        else
        {
            SceneManager.Instance.EnterScene();
            WindowManager.Instance.CloseWindow(WindowKey.LevelResultView);
        }
    }

    public void OnRevie()
    {
        if (ItemUtil.GetItemNum(uint.Parse(ParameterCFG.items["1"].Value)) >= uint.Parse(ParameterCFG.items["2"].Value))
        {
            RevieActor();
        }
        else
        {
            EventCenter.DispatchEvent(EventEnum.ShowMsg ,  LanguageManager.GetText(ItemCFG.items[ParameterCFG.items["1"].Value].Name.ToString()) + LanguageManager.GetText("210050"));
        }
    }

    public void ShowAds()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (Advertisement.IsReady(ConfigData.AdsPlacementId))
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleShowResult;
            Advertisement.Show(ConfigData.AdsPlacementId, options);
        }
#endif
    }

#if UNITY_IOS || UNITY_ANDROID
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                RevieActor();
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                break;
        }
    }
#endif

    private void RevieActor()
    {
        GameData.myself.Revie();
        WindowManager.Instance.CloseWindow(WindowKey.LevelResultView);
    }
}
