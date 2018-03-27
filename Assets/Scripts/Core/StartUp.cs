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
#if UNITY_IOS
        Advertisement.Initialize("1740028");

#elif UNITY_ANDROID
        Advertisement.Initialize("1740027");
#endif
        EventCenter.DispatchEvent(EventEnum.GameInitOver);
        UIManager.Instance.OpenView(WindowKey.UIRoot);
    }
}
