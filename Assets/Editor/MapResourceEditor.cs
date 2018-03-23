using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class MapResourceEditor : EditorWindow
{
    [MenuItem("MapEditor/资源编辑")]

    static void EditorResource()
    {
        EditorWindow.GetWindow(typeof(MapResourceEditor));
    }

    private List<MapResourceItem> mapResouceList = new List<MapResourceItem>();
    private Vector2 scrollPos = Vector2.zero;

    private void OnEnable()
    {
        MapResourceAsset mpPrefabAsset = (MapResourceAsset)AssetDatabase.LoadAssetAtPath("Assets/ResourceAssets/resources.asset", typeof(MapResourceAsset));
        if (mpPrefabAsset != null)
        {
            mapResouceList = mpPrefabAsset.items;
        }
        if (mapResouceList == null)
        {
            mapResouceList = new List<MapResourceItem>();
        }
    }

    public void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存数据"))
        {
            SaveDataAsset();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("资源列表 数量:" + mapResouceList.Count);
        if (GUILayout.Button("添加资源"))
        {
            mapResouceList.Insert(0, null);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < mapResouceList.Count; i++)
        {
            if (false && mapResouceList[i] != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("显示层级");
                mapResouceList[i].layer = (MapEditorSortLayer)EditorGUILayout.EnumPopup(mapResouceList[i].layer);
                GUILayout.Label("物件类型");
                GUILayout.BeginVertical();
                mapResouceList[i].itemType = (MapEditorItemType)EditorGUILayout.EnumPopup(mapResouceList[i].itemType);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();;
            mapResouceList[i] = (MapResourceItem)EditorGUILayout.ObjectField(mapResouceList[i], typeof(MapResourceItem), true, GUILayout.Width(200), GUILayout.Height(20));

            if (GUILayout.Button("删除" , GUILayout.Width(100), GUILayout.Height(20)))
            {
                mapResouceList.RemoveAt(i);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    void SaveDataAsset()
    {
        MapResourceAsset newData = ScriptableObject.CreateInstance<MapResourceAsset>();
        newData.items = mapResouceList;
        AssetDatabase.CreateAsset(newData, "Assets/ResourceAssets/resources.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
