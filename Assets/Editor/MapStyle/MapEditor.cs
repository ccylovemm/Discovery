using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;

public class MapEditor : EditorWindow
{
    [MenuItem("MapEditor/地图样式")]
    static void ShowMap()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }

    public enum MapTool
    {
        None,
        Level,
        Birth,
        Trans,
        Clear,
    }

    private GUIContent[] toolIcons_;
    public GUIContent[] toolIcons
    {
        get
        {
            if (toolIcons_ == null)
            {
                toolIcons_ = new GUIContent[] { EditorGUIUtility.IconContent("ViewToolMove", "None"), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "Level"), EditorGUIUtility.IconContent("lightMeter/greenLight", "Birth"), EditorGUIUtility.IconContent("lightMeter/redLight", "Trans"), EditorGUIUtility.IconContent("TreeEditor.Trash", "Clear") };
            }
            return toolIcons_;
        }
    }
    private int sizeX = 5;
    private int sizeY = 5;
    private MapTool toolSelected;
    private int selectIndex;
    private MapResourceItem mapResourceItem;
    private MapAssetScene mapAssetScene;

    private string filePath_ = "";
    private string filePath
    {
        get
        {
            if (filePath_ == "")
            {
                filePath_ = Application.dataPath + "/ResourceAssets/MapStyleConfig/";
            }
            return filePath_;
        }
        set
        {
            filePath_ = value;
        }
    }

    private void OnEnable()
    {
        MapResourceItem mapPrefabAsset = (MapResourceItem)AssetDatabase.LoadAssetAtPath("Assets/Editor/MapStyle/MapEditorResource.prefab", typeof(MapResourceItem));
        if (mapPrefabAsset != null)
        {
            mapResourceItem = mapPrefabAsset;
        }
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        mapAssetScene = GameObject.FindObjectOfType<MapAssetScene>();
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        if (mapAssetScene == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("宽度：");
            sizeX = EditorGUILayout.IntField(sizeX);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("高度：");
            sizeY = EditorGUILayout.IntField(sizeY);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("创建地图样式", GUILayout.Width(120), GUILayout.Height(30)))
            {
                mapAssetScene = new GameObject("MapAsset").AddComponent<MapAssetScene>();
                mapAssetScene.sizeX = sizeX;
                mapAssetScene.sizeY = sizeY;
                mapAssetScene.mapdata.Clear();
                for (int i = 0; i < sizeX; i ++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        mapAssetScene.mapdata.Add(0);
                    }
                }
            }

            if (GUILayout.Button("加载样式", GUILayout.Width(120), GUILayout.Height(30)))
            {
                LoadMapAsset();
            }
        }
        else
        {
            GUILayout.Label("    空          关卡         出生点     传送门       清除");
            toolSelected = (MapTool)GUILayout.Toolbar((int)toolSelected, toolIcons, GUILayout.Width(280), GUILayout.Height(30));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("加载样式", GUILayout.Width(90), GUILayout.Height(30)))
            {
                LoadMapAsset();
            }

            if (GUILayout.Button("保存样式", GUILayout.Width(90), GUILayout.Height(30)))
            {
                SaveMapAsset();
            }

            if (GUILayout.Button("清除样式", GUILayout.Width(90), GUILayout.Height(30)))
            {
                mapAssetScene.Clear();
            }

            GUILayout.EndHorizontal();
            if (mapResourceItem != null)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < mapResourceItem.normalList.Count; i++)
                {
                    if (i % 7 == 0)
                    {
                        GUILayout.BeginHorizontal();
                    }
                    Sprite sprite = mapResourceItem.normalList[i].sprite;
                    var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
                    croppedTexture.SetPixels(pixels);
                    croppedTexture.Apply();
                    if (GUILayout.Button(croppedTexture, GUILayout.Width(60), GUILayout.Height(60)))
                    {
                        selectIndex = i;
                        if (toolSelected == MapTool.Clear)
                        {
                            toolSelected = MapTool.Level;
                        }
                    }
                    if ((i + 1) % 7 == 0)
                    {
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
        }

        GUILayout.EndVertical();
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        if (mapAssetScene == null) return;
        Vector3 mousepos = Event.current.mousePosition;
        mousepos.y = sceneView.camera.pixelHeight - mousepos.y;
        mousepos = sceneView.camera.ScreenToWorldPoint(mousepos);
        Vector2 grid = MapManager.GetGrid(mousepos.x, mousepos.y);
        if (grid.x < 0 || grid.y < 0 || grid.x >= mapAssetScene.sizeX || grid.y >= mapAssetScene.sizeY) return;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Event.current.type == EventType.MouseDown)
        {
            if (toolSelected == MapTool.Level)
            {
                AddLevel(grid);
            }
            else if (toolSelected == MapTool.Birth)
            {
                SetBirth(grid + Vector2.one);
            }
            else if (toolSelected == MapTool.Trans)
            {
                SetTrans(grid + Vector2.one);
            }
            else if (toolSelected == MapTool.Clear)
            {
                Clear(grid);
            }
        }
    }

    public void AddLevel(Vector2 pos)
    {
        mapAssetScene.mapdata[(int)pos.x + (int)pos.y * mapAssetScene.sizeY] = selectIndex + 1;
        mapAssetScene.UpdateMesh();
    }

    public void SetBirth(Vector2 pos)
    {
        mapAssetScene.brithPos = pos;
    }

    public void SetTrans(Vector2 pos)
    {
        mapAssetScene.transPos = pos;
    }

    public void Clear(Vector2 pos)
    {
        if (mapAssetScene.brithPos == pos + Vector2.one)
        {
            mapAssetScene.brithPos = Vector2.zero;
        }
        if (mapAssetScene.transPos == pos + Vector2.one)
        {
            mapAssetScene.transPos = Vector2.zero;
        }
        else
        {
            mapAssetScene.mapdata[(int)pos.x + (int)pos.y * mapAssetScene.sizeY] = 0;
            mapAssetScene.UpdateMesh();
        }
    }

    void SaveMapAsset()
    {
        filePath = EditorUtility.SaveFilePanel("保存", filePath, "", "asset");
        if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
        MapAsset newData = ScriptableObject.CreateInstance<MapAsset>();
        newData.sizeX = mapAssetScene.sizeY;
        newData.sizeY = mapAssetScene.sizeY;
        newData.brithPos = mapAssetScene.brithPos;
        newData.transPos = mapAssetScene.transPos;
        for (int i = 0; i < mapAssetScene.mapdata.Count; i ++)
        {
            newData.mapdata.Add(mapAssetScene.mapdata[i]);
        }
        AssetDatabase.CreateAsset(newData, "Assets" + filePath.Replace(Application.dataPath, ""));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void LoadMapAsset()
    {
        filePath = EditorUtility.OpenFilePanel("打开", filePath, "asset");
        if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
        MapAsset mapPrefabAsset = (MapAsset)AssetDatabase.LoadAssetAtPath("Assets" + filePath.Replace(Application.dataPath, ""), typeof(MapAsset));
        if (mapPrefabAsset != null)
        {
            if (mapAssetScene == null)
            {
                mapAssetScene = new GameObject("MapAsset").AddComponent<MapAssetScene>();
            }
            mapAssetScene.sizeX = mapPrefabAsset.sizeX;
            mapAssetScene.sizeY = mapPrefabAsset.sizeY;
            mapAssetScene.brithPos = mapPrefabAsset.brithPos;
            mapAssetScene.transPos = mapPrefabAsset.transPos;
            mapAssetScene.mapdata.Clear();
            for (int i = 0; i < mapPrefabAsset.mapdata.Count; i++)
            {
                mapAssetScene.mapdata.Add(mapPrefabAsset.mapdata[i]);
            }
            mapAssetScene.UpdateMesh();
        }
    }
}
