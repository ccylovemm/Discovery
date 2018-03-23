using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

public class CopyAssets
{
    static List<string> depends = new List<string>();

    [MenuItem("CopyAssets/Copy")]
    static public void Copy()
    {
        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Scripts/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Scripts/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Materials/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Materials/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Prefabs/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Prefabs/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Textures/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Textures/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Controller/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Controller/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Animation/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Animation/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Sounds/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Sounds/");
        }

        if (!Directory.Exists(Application.dataPath + "/ResourceAssets/Effect/Models/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ResourceAssets/Effect/Models/");
        }

        foreach (Object res in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string path = AssetDatabase.GetAssetPath(res);
            if (path.Contains(".prefab"))
            {
                SetDepends(path);
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("CopyAssets/Clear")]
    static public void Clear()
    {
        if (Directory.Exists(Application.dataPath + "/ResourceAssets/Effect"))
        {
            Directory.Delete(Application.dataPath + "/ResourceAssets/Effect", true);
        }
        AssetDatabase.Refresh();
    }

    static private void SetDepends(string filePath)
    {
        string[] dps = AssetDatabase.GetDependencies(filePath);
        for (int i = 0; i < dps.Length; i++)
        {
            if (!depends.Contains(dps[i]))
            {
                depends.Add(dps[i]);
                MoveFile(dps[i]);
                SetDepends(dps[i]);
            }
        }
    }

    static private void MoveFile(string filePath)
    {
        if (filePath.Contains("ResourceAssets/Effect")) return;
        filePath = filePath.Replace("Assets", "");
        string fileName = System.IO.Path.GetFileName(filePath);
        string extension = System.IO.Path.GetExtension(filePath);
        switch (extension)
        {
            case ".js":
            case ".cs":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Scripts/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Scripts/" + fileName + ".meta");
                break;
            case ".mat":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Materials/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Materials/" + fileName + ".meta");
                break;
            case ".prefab":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Prefabs/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Prefabs/" + fileName + ".meta");
                break;
            case ".png":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Textures/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Textures/" + fileName + ".meta");
                break;
            case ".controller":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Controller/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Controller/" + fileName + ".meta");
                break;
            case ".anim":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Animation/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Animation/" + fileName + ".meta");
                break;
            case ".wav":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Sounds/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Sounds/" + fileName + ".meta");
                break;
            case ".FBX":
                File.Move(Application.dataPath + filePath, Application.dataPath + "/ResourceAssets/Effect/Models/" + fileName);
                File.Move(Application.dataPath + filePath + ".meta", Application.dataPath + "/ResourceAssets/Effect/Models/" + fileName + ".meta");
                break;
            default:
                Debug.LogError("没有识别文明名");
                break;
        }
    }
}
