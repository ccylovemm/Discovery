using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;

public class MapEditor : EditorWindow
{
    [MenuItem("MapEditor/地图生成")]
    static void RandMap()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }

    private MapTool toolSelected;
    private List<MapResourceItem> mapResouceList = new List<MapResourceItem>();

    private MapResourceItem selectedTile;
    private Vector2 lastPos = Vector2.zero;

    private MapScene currMapScene;
    private List<MapScene> mapSceneList = new List<MapScene>();

    private int offsetX = 0;
    private int offsetY = 0;

    private int mapSizeX = 20;
    private int mapSizeY = 20;

    private WorldType worldType = WorldType.World1;

    void OnFocus()
    {
        mapSceneList = GameObject.FindObjectsOfType<MapScene>().ToList<MapScene>();
        if (currMapScene == null && mapSceneList.Count > 0)
        {
            currMapScene = mapSceneList[0];
        }
        if (currMapScene != null)
        {
            FreshResource();
            currMapScene.FreshMapGrid();
        }
        Selection.selectionChanged = OnSelectObj;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        worldType = (WorldType)EditorGUILayout.EnumPopup(worldType, GUILayout.Width(100));
        if (GUILayout.Button("创建新的地图块", GUILayout.Width(100), GUILayout.Height(30)))
        {
            currMapScene = new GameObject("Map" + (mapSceneList.Count + 1)).AddComponent<MapScene>();
            currMapScene.offsetX = offsetX;
            currMapScene.offsetY = offsetY;
            currMapScene.mapSizeX = mapSizeX;
            currMapScene.mapSizeY = mapSizeY;
            currMapScene.worldType = worldType;
            mapSceneList.Add(currMapScene);
            FreshResource();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("地图宽度：" , GUILayout.Width(60));
        mapSizeX = EditorGUILayout.IntField(mapSizeX , GUILayout.Width(60));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("地图高度：", GUILayout.Width(60));
        mapSizeY = EditorGUILayout.IntField(mapSizeY, GUILayout.Width(60));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("地图偏移X：", GUILayout.Width(60));
        offsetX = EditorGUILayout.IntField(offsetX, GUILayout.Width(60));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("地图偏移Y：", GUILayout.Width(60));
        offsetY = EditorGUILayout.IntField(offsetY, GUILayout.Width(60));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginVertical();
        GUILayout.Label("当前选中的地图块: " + (currMapScene == null ? "无" : currMapScene.name), GUILayout.Width(200));

        if (currMapScene != null)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("地图宽度：" + currMapScene.mapSizeX, GUILayout.Width(100));
            GUILayout.Label("地图高度：" + currMapScene.mapSizeY, GUILayout.Width(100));
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("地图偏移X：", GUILayout.Width(60));
            currMapScene.offsetX = EditorGUILayout.IntField(currMapScene.offsetX, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("地图偏移Y：", GUILayout.Width(60));
            currMapScene.offsetY = EditorGUILayout.IntField(currMapScene.offsetY, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("更新物理碰撞", GUILayout.Width(90), GUILayout.Height(30)))
        {
            for(int i = 0; i < mapSceneList.Count; i ++)
            {
                mapSceneList[i].UpdateCollider();
            }
        }

        if (GUILayout.Button("清除当前地图块", GUILayout.Width(90), GUILayout.Height(30)))
        {
            if (currMapScene != null)
            {
                currMapScene.Clear();
            }
            mapSceneList.Remove(currMapScene);
            currMapScene = null;
            if (mapSceneList.Count > 0)
            {
                currMapScene = mapSceneList[0];
            }
        }

        if (GUILayout.Button("保存世界地图", GUILayout.Width(90), GUILayout.Height(30)))
        {
            EditorApplication.SaveScene(EditorApplication.currentScene);
            SaveSceneAsset();
        }

        if (GUILayout.Button("加载世界地图", GUILayout.Width(90), GUILayout.Height(30)))
        {
            LoadSceneAsset();
        }

        GUILayout.EndHorizontal();
        toolSelected = (MapTool)GUILayout.Toolbar((int)toolSelected, toolIcons, GUILayout.Width(160), GUILayout.Height(30));

        if (currMapScene != null && mapResouceList != null)
        {
            GUILayout.BeginVertical();
            int count = 0;
            for (int i = 0; i < mapResouceList.Count; i++)
            {
                if (mapResouceList[i].worldType != WorldType.None && mapResouceList[i].worldType != currMapScene.worldType) continue;
                if (count % 7 == 0)
                {
                    GUILayout.BeginHorizontal();
                }
                Sprite sprite = (mapResouceList[i].isPrefab ? (mapResouceList[i].GetComponentInChildren<SpriteRenderer>().sprite) : ((mapResouceList[i].isNine ? mapResouceList[i].center[0] : mapResouceList[i].normalList[0]).sprite));
                var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
                croppedTexture.SetPixels(pixels);
                croppedTexture.Apply();

                if (GUILayout.Button(croppedTexture, GUILayout.Width(60), GUILayout.Height(60)))
                {
                    selectedTile = mapResouceList[i];
                    if (toolSelected == MapTool.Clear)
                    {
                        toolSelected = MapTool.Draw;
                    }
                }
                if ((count + 1) % 7 == 0)
                {
                    GUILayout.EndHorizontal();
                }
                count++;
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    public void OnSelectObj()
    {
        if (Selection.activeGameObject != null)
        {
            MapScene mapScene = Selection.activeGameObject.GetComponent<MapScene>();
            if (mapScene != null)
            {
                currMapScene = mapScene;
                FreshResource();
            }
        }
    }

    private void FreshResource()
    {
        if (currMapScene == null)
        {
            mapResouceList.Clear();
        }
        else
        {
            MapResourceAsset mapPrefabAsset = (MapResourceAsset)AssetDatabase.LoadAssetAtPath("Assets/ResourceAssets/resources.asset", typeof(MapResourceAsset));
            if (mapPrefabAsset != null)
            {
                mapResouceList = new List<MapResourceItem>(mapPrefabAsset.items);
            }
        }
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        if (currMapScene == null) return;
        Vector3 mousepos = Event.current.mousePosition;
        mousepos.y = sceneView.camera.pixelHeight - mousepos.y;
        mousepos = sceneView.camera.ScreenToWorldPoint(mousepos);

        Vector2 grid = MapManager.GetGrid(mousepos.x, mousepos.y);
        if (grid.x < currMapScene.offsetX || grid.y < currMapScene.offsetY || grid.x >= currMapScene.mapSizeX + currMapScene.offsetX || grid.y >= currMapScene.mapSizeY + currMapScene.offsetY) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Event.current.type == EventType.MouseDown)
        {
            if (toolSelected == MapTool.Draw)
            {
                AddTile(grid, selectedTile);
                ClearStoneWallAndEdge();
                GenerateStoneWallAndEdge();
            }
            else if (toolSelected == MapTool.Clear)
            {
                currMapScene.ClearGrid(grid);
            }
        }
        else if (Event.current.type == EventType.MouseDrag)
        {
            if (lastPos == grid) return;
            lastPos = grid;
            if (toolSelected == MapTool.Draw)
            {
                if (!selectedTile.isPrefab)
                {
                    AddTile(grid, selectedTile);
                    ClearStoneWallAndEdge();
                    GenerateStoneWallAndEdge();
                }
            }
            else if (toolSelected == MapTool.Clear)
            {
                currMapScene.ClearGrid(grid);
            }
        }
    }

    void AddTile(Vector2 pos, MapResourceItem selectedTile)
    {
        currMapScene.AddGrid(((int)pos.x - currMapScene.offsetX) * 10000 + (int)pos.y - currMapScene.offsetY, selectedTile);
        currMapScene.UpdateMap();
    }

    void ClearStoneWallAndEdge()
    {
        for (int i = 0; i < currMapScene.layerItems.Count; i++)
        {
            MapLayerItem mapLayerItem = currMapScene.layerItems[i];
            for (int j = mapLayerItem.items.Count - 1; j >= 0; j--)
            {
                if (mapLayerItem.items[j].itemType == MapEditorItemType.StoneWall)
                {
                    mapLayerItem.items[j] = mapLayerItem.items[j].replaceItem;
                }
                else if (mapLayerItem.items[j].itemType == MapEditorItemType.Edge)
                {
                    mapLayerItem.items.RemoveAt(j);
                    mapLayerItem.posList.RemoveAt(j);
                }
            }
        }
    }

    void GenerateStoneWallAndEdge()
    {
        int index = currMapScene.layers.IndexOf(MapEditorSortLayer.Floor1);
        MapLayerItem mapLayerItem = currMapScene.layerItems[index];
        for (int i = 0; i < mapLayerItem.items.Count; i++)
        {
            if (mapLayerItem.items[i].itemType == MapEditorItemType.Wall)
            {
                int x = mapLayerItem.posList[i] / 10000;
                int y = mapLayerItem.posList[i] % 10000;
                if (y == 0) continue;
                index = mapLayerItem.posList.IndexOf(x * 10000 + y - 1);
                if (index != -1 && (mapLayerItem.items[index].itemType != MapEditorItemType.Wall && mapLayerItem.items[index].itemType != MapEditorItemType.StoneWall))
                {
                    mapLayerItem.items[i] = mapLayerItem.items[i].replaceItem;
                }
            }
        }

        for (int i = 0; i < mapLayerItem.items.Count; i++)
        {
            if (mapLayerItem.items[i].itemType != MapEditorItemType.Wall && mapLayerItem.items[i].itemType != MapEditorItemType.StoneWall)
            {
                int x = mapLayerItem.posList[i] / 10000;
                int y = mapLayerItem.posList[i] % 10000;

                int index1 = mapLayerItem.posList.IndexOf(x * 10000 + y + 1);
                int index2 = mapLayerItem.posList.IndexOf((x - 1) * 10000 + y);
                int index3 = mapLayerItem.posList.IndexOf((x + 1) * 10000 + y);

                bool wall1 = index1 != -1 && (mapLayerItem.items[index1].itemType == MapEditorItemType.Wall || mapLayerItem.items[index1].itemType == MapEditorItemType.StoneWall);
                bool wall2 = index2 != -1 && (mapLayerItem.items[index2].itemType == MapEditorItemType.Wall || mapLayerItem.items[index2].itemType == MapEditorItemType.StoneWall);
                bool wall3 = index3 != -1 && (mapLayerItem.items[index3].itemType == MapEditorItemType.Wall || mapLayerItem.items[index3].itemType == MapEditorItemType.StoneWall);

                MapResourceItem edge = null;
                if (wall1 && wall2 && wall3)
                {
                    edge = mapLayerItem.items[index1].replaceEdgeSingleUp;
                }
                else if (wall1 && wall2 && !wall3)
                {
                    edge = mapLayerItem.items[index1].replaceEdgeLeftUp;
                }
                else if (wall1 && !wall2 && wall3)
                {
                    edge = mapLayerItem.items[index1].replaceEdgeRightUp;
                }
                else if (wall1 && !wall2 && !wall3)
                {
                    edge = mapLayerItem.items[index1].replaceEdgeUp;
                }
                else if (!wall1 && wall2 && wall3)
                {
                    edge = mapLayerItem.items[index2].replaceEdgeVer;
                }
                else if (!wall1 && wall2 && !wall3)
                {
                    edge = mapLayerItem.items[index2].replaceEdgeLeft;
                }
                else if (!wall1 && !wall2 && wall3)
                {
                    edge = mapLayerItem.items[index3].replaceEdgeRight;
                }
                else
                {
                    edge = null;
                }
                if (edge != null)
                {
                    index = currMapScene.layers.IndexOf(MapEditorSortLayer.Floor2);
                    MapLayerItem mapLayerEdgeItem = null;
                    if (index == -1)
                    {
                        currMapScene.layers.Add(MapEditorSortLayer.Floor2);
                        mapLayerEdgeItem = new MapLayerItem();
                        currMapScene.layerItems.Add(mapLayerEdgeItem);
                    }
                    else
                    {
                        mapLayerEdgeItem = currMapScene.layerItems[index];
                    }
                    index = mapLayerEdgeItem.posList.IndexOf(x * 10000 + y);
                    if (index == -1)
                    {
                        mapLayerEdgeItem.posList.Add(x * 10000 + y);
                        mapLayerEdgeItem.items.Add(edge);
                    }
                    else
                    {
                        mapLayerEdgeItem.items[index] = edge;
                    }
                }
            }
        }
    }

    void SaveSceneAsset()
    {
        if(currMapScene != null)
        {
            filePath = EditorUtility.SaveFilePanel("保存", filePath, "", "asset");
            if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
            MapWorldAsset newData = ScriptableObject.CreateInstance<MapWorldAsset>();
            for(int i = 0; i < mapSceneList.Count; i ++)
            {
                MapSceneAsset mapSceneAsset = new MapSceneAsset();
                mapSceneAsset.offsetX = mapSceneList[i].offsetX;
                mapSceneAsset.offsetY = mapSceneList[i].offsetY;
                mapSceneAsset.mapSizeX = mapSceneList[i].mapSizeX;
                mapSceneAsset.mapSizeY = mapSceneList[i].mapSizeY;
                mapSceneAsset.worldType = mapSceneList[i].worldType;
                mapSceneAsset.layers = new List<MapEditorSortLayer>(mapSceneList[i].layers);

                for (int n = 0; n < mapSceneList[i].layerItems.Count; n++)
                {
                    mapSceneAsset.layerItems.Add(new MapLayerItem());
                    mapSceneAsset.layerItems[n].posList = new List<int>(mapSceneList[i].layerItems[n].posList);
                    mapSceneAsset.layerItems[n].items = new List<MapResourceItem>(mapSceneList[i].layerItems[n].items);
                }
                newData.mapScenes.Add(mapSceneAsset);
            }
            AssetDatabase.CreateAsset(newData, "Assets" + filePath.Replace(Application.dataPath, ""));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void LoadSceneAsset()
    {
        filePath = EditorUtility.OpenFilePanel("打开", filePath, "asset");
        if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
        MapWorldAsset mapWorldAsset = (MapWorldAsset)AssetDatabase.LoadAssetAtPath("Assets" + filePath.Replace(Application.dataPath, ""), typeof(MapWorldAsset));
        if (mapWorldAsset != null)
        {
            for (int i = 0; i < mapWorldAsset.mapScenes.Count; i ++)
            {
                currMapScene = new GameObject("Map" + (i + 1)).AddComponent<MapScene>();
                currMapScene.offsetX = mapWorldAsset.mapScenes[i].offsetX;
                currMapScene.offsetY = mapWorldAsset.mapScenes[i].offsetY;
                currMapScene.mapSizeX = mapWorldAsset.mapScenes[i].mapSizeX;
                currMapScene.mapSizeY = mapWorldAsset.mapScenes[i].mapSizeY;
                currMapScene.worldType = mapWorldAsset.mapScenes[i].worldType;
                currMapScene.layers = new List<MapEditorSortLayer>(mapWorldAsset.mapScenes[i].layers);
                for (int n = 0; n < mapWorldAsset.mapScenes[i].layerItems.Count; n++)
                {
                    currMapScene.layerItems.Add(new MapLayerItem());
                    currMapScene.layerItems[n].posList = new List<int>(mapWorldAsset.mapScenes[i].layerItems[n].posList);
                    currMapScene.layerItems[n].items = new List<MapResourceItem>(mapWorldAsset.mapScenes[i].layerItems[n].items);
                }
                mapSceneList.Add(currMapScene);
                currMapScene.FreshMapGrid();
                currMapScene.UpdateMap();
            }
        }
        FreshResource();
    }

    private string filePath_ = "";
    private string filePath
    {
        get
        {
            if (filePath_ == "")
            {
                filePath_ = Application.dataPath + "/ResourceAssets/MapConfig/";
            }
            return filePath_;
        }
        set
        {
            filePath_ = value;
        }
    }

    public enum MapTool
    {
        Drag,
        Draw,
        Clear,
    }

    private GUIContent[] toolIcons_;
    public GUIContent[] toolIcons
    {
        get
        {
            if (toolIcons_ == null)
            {
                toolIcons_ = new GUIContent[] { EditorGUIUtility.IconContent("ViewToolMove", "Drag"), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "Draw"), EditorGUIUtility.IconContent("TreeEditor.Trash", "Clear") };
            }
            return toolIcons_;
        }
    }
}
