using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

public class BuildAssets
{
    static List<string> depends = new List<string>();

    static BuildTarget buildTarget;

    [MenuItem("BuildAssets/BuildWin")]
    static public void BuildWin()
    {
        buildTarget = BuildTarget.StandaloneWindows64;
        Build();
    }

    [MenuItem("BuildAssets/BuildAndroid")]
    static public void BuildAndroid()
    {
        buildTarget = BuildTarget.Android;
        Build();
    }

    [MenuItem("BuildAssets/BuildIos")]
    static public void BuildIos()
    {
        buildTarget = BuildTarget.iOS;
        Build();
    }

    static void Build()
    {
        DelBundles();
        DelBundleNames();
        SetBundleConfig();
        BuildBundles();
    }

    [MenuItem("BuildAssets/Clear")]
    static public void DelBundleNames()
    {
        string[] bundles = AssetDatabase.GetAllAssetBundleNames();
        for (var i = 0; i < bundles.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(bundles[i], true);
        }
    }

    static void DelBundles()
    {
        if (Directory.Exists(Application.streamingAssetsPath + "/resourceassets"))
        {
            Directory.Delete(Application.streamingAssetsPath + "/resourceassets", true);
        }
        Directory.CreateDirectory(Application.streamingAssetsPath + "/resourceassets");
    }

    static void SetBundleConfig()
    {
        SetBundleConfig("/ResourceAssets/Prefabs/Avatar/" , "*.prefab" , "resourceassets/avatar.assetbundle");
        SetBundleConfig("/ResourceAssets/Prefabs/Map/", "*.prefab", "resourceassets/map.assetbundle");
        SetBundleConfig("/ResourceAssets/Prefabs/TerrainEffect/", "*.prefab", "resourceassets/terrainEffect.assetbundle");
        SetBundleConfig("/ResourceAssets/Prefabs/GUI/", "*.prefab", "resourceassets/gui.assetbundle");
        SetBundleConfig("/ResourceAssets/Prefabs/Item/", "*.prefab", "resourceassets/item.assetbundle");
        SetBundleConfig("/ResourceAssets/Map/", "*.unity", "");
        SetBundleConfig("/ResourceAssets/Prefabs/Status/", "*.prefab", "resourceassets/buff.assetbundle");
        SetBundleConfig("/ResourceAssets/Prefabs/Magic/", "*.prefab", "");
        SetBundleConfig("/ResourceAssets/Config/", "*.json", "resourceassets/configassets.assetbundle");
        SetBundleConfig("/ResourceAssets/MapConfig/", "*.asset", "resourceassets/configassets.assetbundle");
        SetBundleConfig("/ResourceAssets/MapStyleConfig/", "*.asset", "resourceassets/configassets.assetbundle");
        SetBundleConfig("/ResourceAssets/AI/", "*.asset", "resourceassets/configassets.assetbundle");
        SetBundleConfig("/ResourceAssets/Sounds/", "*.mp3", "resourceassets/soundassets.assetbundle");
        SetBundleConfig("/ResourceAssets/Atlas", "*.png", "");
    }

    static void SetBundleConfig(string path , string name , string assetBundleName)
    {
        string[] files = Directory.GetFiles(Application.dataPath + path , name, SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string filePath = file.Replace("\\", "/");
            string assetPath = filePath.Replace(Application.dataPath + "/", "");

            string extenName = System.IO.Path.GetExtension(filePath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            assetPath = assetPath.Replace(fileName + extenName, "");

            if (string.IsNullOrEmpty(extenName) || extenName == ".meta")
            {
                continue;
            }
            string bundleFilePath = "Assets" + filePath.Replace(Application.dataPath, "");
            AssetImporter assetImporter = AssetImporter.GetAtPath(bundleFilePath);
            if (assetBundleName == "")
            {
                if (extenName == ".unity")
                {
                    assetImporter.assetBundleName = assetPath + fileName + ".unity3d";
                }
                else
                {
                    assetImporter.assetBundleName = assetPath + fileName + ".assetbundle";
                } 
            }
            else
            {
                assetImporter.assetBundleName = assetBundleName;
            }
            SetDepends(bundleFilePath);
        }
    }

    static void SetDepends(string filePath)
    {
        string[] dps = AssetDatabase.GetDependencies(filePath);
        for (int i = 0; i < dps.Length; i++)
        {
            if (dps[i] == filePath || dps[i].Contains(".cs") || dps[i].Contains(".js")) continue;
            if (!depends.Contains(dps[i]))
            {
                depends.Add(dps[i]);
                AssetImporter assetImporter = AssetImporter.GetAtPath(dps[i]);
                if(string.IsNullOrEmpty(assetImporter.assetBundleName)) assetImporter.assetBundleName = "resourceassets/alldependencies/" + dps[i];
                SetDepends(dps[i]);
            }
        }
    }

    static void BuildBundles()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath , BuildAssetBundleOptions.UncompressedAssetBundle,EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        SaveVersion();
    }

    static void SaveVersion()
    {
        string outstr = "VersionNumber|1" + "\n";
        string[] files = Directory.GetFiles(Application.streamingAssetsPath, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(".meta") || file.Contains("LocalVersion.txt")) continue;
            string filePath = file.Replace("\\", "/");
            string assetPath = filePath.Replace(Application.streamingAssetsPath + "/", "");
            outstr += assetPath + "|" + AssetDatabase.AssetPathToGUID("Assets/StreamingAssets/" + assetPath) + "\n";
        }
        string outfile = System.IO.Path.Combine(Application.streamingAssetsPath, "LocalVersion.txt");
        System.IO.File.WriteAllText(outfile, outstr, Encoding.ASCII);
    }
}
