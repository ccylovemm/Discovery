using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    public Text spellTxt;
    public Text backBtnTxt;

    private void Awake()
    {
        spellTxt.text = LanguageManager.GetText("210056");
        backBtnTxt.text = LanguageManager.GetText("210057");
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void OpenGM()
    {
        WindowManager.Instance.OpenWindow(WindowKey.GMView);
    }

    public void OpenBag()
    {
        WindowManager.Instance.OpenWindow(WindowKey.BagView);
    }

    public void OnMusicHandle()
    {
        DataManager.userData.IsSound = !DataManager.userData.IsSound;
        AudioListener.pause = !DataManager.userData.IsSound;
    }

    public void OnSoundHandle()
    {
        DataManager.userData.IsSound = !DataManager.userData.IsSound;
        AudioListener.pause = !DataManager.userData.IsSound;
    }

    public void OnShareHandle()
    {
      
    }

    public void OnCloseHandle()
    {
        WindowManager.Instance.CloseWindow(WindowKey.PauseView);
    }
}
