using UnityEngine;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class StartUp : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;

        VersionManager.Instance.CheckVersion(LoadConfig);
    }

    void LoadConfig()
    {
        CfgFiles.Init();
        int fileCount = CfgFiles.files.Count;
        ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", ab =>
        {
            foreach(var cfg in CfgFiles.files)
            {
                cfg.Value.Read(ab.LoadAsset<TextAsset>(cfg.Key + ".json").text);
                fileCount--;
                if (fileCount == 0)
                {
                    GameInit();
                }
            }           
        });
    }

    void GameInit()
    {
        DataManager.Init();
#if UNITY_IOS
        Advertisement.Initialize("1740028");

        AppsFlyer.setAppsFlyerKey("DTnFrbYC5f3pAdkT6o4XYJ");
		AppsFlyer.setAppID("1360985835");
		AppsFlyer.trackAppLaunch();

#elif UNITY_ANDROID
        Advertisement.Initialize("1740027");

        AppsFlyer.init("DTnFrbYC5f3pAdkT6o4XYJ");
		AppsFlyer.setAppID ("com.PandaGames.ComboMage"); 
#endif
        AudioListener.pause = !DataManager.userData.IsSound;

        EventCenter.DispatchEvent(EventEnum.GameInitOver);
     //   WindowManager.Instance.OpenWindow(WindowKey.MainUI);
    }
}
