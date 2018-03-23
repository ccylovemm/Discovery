using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;

public class MapGenerateEditor : EditorWindow
{
    [MenuItem("MapEditor/地图生成")]
    static void RandMap()
    {
        EditorWindow.GetWindow(typeof(MapGenerateEditor));
    }

    public enum MapTool
    {
        Drag,
        Draw,
        Fill,
        Actor,
        Clear,
    }

    private GUIContent[] toolIcons_;
    public GUIContent[] toolIcons
    {
        get
        {
            if (toolIcons_ == null)
            {
                toolIcons_ = new GUIContent[] { EditorGUIUtility.IconContent("ViewToolMove", "Drag"), EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "Draw") , EditorGUIUtility.IconContent("editicon.sml", "Fill"), EditorGUIUtility.IconContent("AvatarInspector/BodySilhouette", "Actor"), EditorGUIUtility.IconContent("TreeEditor.Trash", "Clear") };
            }
            return toolIcons_;
        }
    }

    private MapTool toolSelected;
    private Vector2 scrollPos;
    private List<MapResourceItem> mapResouceList = new List<MapResourceItem>();

    private int selectIndex;
    private MapResourceItem selectedTile;
    private MapScene mapInfo;

    private int mapSizeX = 20;
    private int mapSizeY = 20;
    private MapWorldResource worldResource = MapWorldResource.World1;

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

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        mapInfo = GameObject.FindObjectOfType<MapScene>();
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        if (mapInfo == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("地图宽度：");
            mapSizeX = EditorGUILayout.IntField(mapSizeX);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("地图高度：");
            mapSizeY = EditorGUILayout.IntField(mapSizeY);
            GUILayout.EndHorizontal();
            worldResource = (MapWorldResource)EditorGUILayout.EnumPopup(worldResource, GUILayout.Width(100));
            if (GUILayout.Button("创建地形", GUILayout.Width(120), GUILayout.Height(30)))
            {
                mapInfo = new GameObject("Terrain").AddComponent<MapScene>();
                mapInfo.mapSizeX = mapSizeX;
                mapInfo.mapSizeY = mapSizeY;
                mapInfo.worldResource = worldResource;
            }

            if (GUILayout.Button("加载场景", GUILayout.Width(120), GUILayout.Height(30)))
            {
                LoadSceneAsset();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            toolSelected = (MapTool)GUILayout.Toolbar((int)toolSelected, toolIcons, GUILayout.Width(160), GUILayout.Height(30));

            if (GUILayout.Button("加载场景", GUILayout.Width(90), GUILayout.Height(30)))
            {
                LoadSceneAsset();
            }

            if (GUILayout.Button("保存场景", GUILayout.Width(90), GUILayout.Height(30)))
            {
                EditorApplication.SaveScene(EditorApplication.currentScene);
                SaveSceneAsset();
            }

            if (GUILayout.Button("清除场景", GUILayout.Width(90), GUILayout.Height(30)))
            {
                mapInfo.Clear();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorGUIUtility.IconContent("RotateTool"), GUILayout.Width(90), GUILayout.Height(30)))
            {
                RotateMap();
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("Mirror"), GUILayout.Width(90), GUILayout.Height(30)))
            {
                MirrorMap1();
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("Mirror"), GUILayout.Width(90), GUILayout.Height(30)))
            {
                MirrorMap2();
            }
            GUILayout.EndHorizontal();

            mapInfo.isShowPath = GUILayout.Toggle(mapInfo.isShowPath , "是否显示路径");

            if (toolSelected == MapTool.Actor)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("随机怪物点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 1;
                }
                if (GUILayout.Button("指定怪物点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 2;
                }
                if (GUILayout.Button("指定宝箱点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 3;
                }
                if (GUILayout.Button("指定出生点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 4;
                }
                if (GUILayout.Button("随机祭坛点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 5;
                }
                if (GUILayout.Button("指定祭坛点", GUILayout.Width(70), GUILayout.Height(40)))
                {
                    selectIndex = 6;
                }
               
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos);

                GUILayout.Label("随机怪物", GUILayout.Width(100));
                GUILayout.BeginVertical();//xxxxxx
                GUILayout.Label("随机怪物点" ,GUILayout.Width(100));
                for (int i = 0; i < mapInfo.randonMonsterPos.Count; i ++)
                {
                    GUILayout.BeginHorizontal();///222222
                    GUILayout.Label("x: " + mapInfo.randonMonsterPos[i].x , GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.randonMonsterPos[i].y , GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.randonMonsterPos.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();////22222
                }
                GUILayout.EndVertical();//xxxxx

                GUILayout.Space(20);

                GUILayout.Label("指定怪物", GUILayout.Width(100));
                GUILayout.BeginVertical();
                GUILayout.Label("指定怪物点", GUILayout.Width(100));
                for (int i = 0; i < mapInfo.appointMonsters.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("x: " + mapInfo.appointMonsters[i].pos.x, GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.appointMonsters[i].pos.y, GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.appointMonsters.RemoveAt(i);
                        break;
                    }

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("怪物组", GUILayout.Width(100));
                    if (GUILayout.Button("添加", GUILayout.Width(100)))
                    {
                        mapInfo.appointMonsters[i].groups.Add(0);
                    }
                    GUILayout.EndHorizontal();
                    
                    for (int j = 0; j < mapInfo.appointMonsters[i].groups.Count; j++)
                    {
                        GUILayout.BeginHorizontal();
                        mapInfo.appointMonsters[i].groups[j] = EditorGUILayout.IntField(mapInfo.appointMonsters[i].groups[j], GUILayout.Width(80));
                        if (GUILayout.Button("删除", GUILayout.Width(40)))
                        {
                            mapInfo.appointMonsters[i].groups.RemoveAt(j);
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(20);

                GUILayout.Label("随机宝箱", GUILayout.Width(100));
                GUILayout.BeginHorizontal(); //111111
                GUILayout.BeginVertical();//xxxxxx
                GUILayout.Label("随机宝箱点", GUILayout.Width(100));
                for (int i = 0; i < mapInfo.randonChestPos.Count; i++)
                {
                    GUILayout.BeginHorizontal();///222222
                    GUILayout.Label("x: " + mapInfo.randonChestPos[i].x, GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.randonChestPos[i].y, GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.randonChestPos.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();////22222
                }

                GUILayout.Space(20);

                GUILayout.Label("随机出生", GUILayout.Width(100));
                GUILayout.BeginHorizontal(); //111111
                GUILayout.BeginVertical();//xxxxxx
                GUILayout.Label("随机出生点", GUILayout.Width(100));
                for (int i = 0; i < mapInfo.randonBirthPos.Count; i++)
                {
                    GUILayout.BeginHorizontal();///222222
                    GUILayout.Label("x: " + mapInfo.randonBirthPos[i].x, GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.randonBirthPos[i].y, GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.randonBirthPos.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();////22222
                }

                GUILayout.Space(20);

                GUILayout.Label("随机祭坛", GUILayout.Width(100));
                GUILayout.BeginHorizontal(); //111111
                GUILayout.BeginVertical();//xxxxxx
                GUILayout.Label("随机祭坛点", GUILayout.Width(100));
                for (int i = 0; i < mapInfo.randonAltarPos.Count; i++)
                {
                    GUILayout.BeginHorizontal();///222222
                    GUILayout.Label("x: " + mapInfo.randonAltarPos[i].x, GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.randonAltarPos[i].y, GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.randonAltarPos.RemoveAt(i);
                        break;
                    }
                    GUILayout.EndHorizontal();////22222
                }

                GUILayout.Space(20);

                GUILayout.Label("指定祭坛", GUILayout.Width(100));
                GUILayout.BeginVertical();
                GUILayout.Label("指定祭坛点", GUILayout.Width(100));
                for (int i = 0; i < mapInfo.appointAltars.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("x: " + mapInfo.appointAltars[i].pos.x, GUILayout.Width(40));
                    GUILayout.Label("y: " + mapInfo.appointAltars[i].pos.y, GUILayout.Width(40));
                    if (GUILayout.Button("删除", GUILayout.Width(40)))
                    {
                        mapInfo.appointAltars.RemoveAt(i);
                        break;
                    }

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("祭坛类型", GUILayout.Width(100));
                    if (GUILayout.Button("添加", GUILayout.Width(100)))
                    {
                        mapInfo.appointAltars[i].groups.Add(0);
                    }
                    GUILayout.EndHorizontal();

                    for (int j = 0; j < mapInfo.appointAltars[i].groups.Count; j++)
                    {
                        GUILayout.BeginHorizontal();
                        mapInfo.appointAltars[i].groups[j] = EditorGUILayout.IntField(mapInfo.appointAltars[i].groups[j], GUILayout.Width(80));
                        if (GUILayout.Button("删除", GUILayout.Width(40)))
                        {
                            mapInfo.appointAltars[i].groups.RemoveAt(j);
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(20);

                GUILayout.EndScrollView();
            }
            else
            {
                
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("更新碰撞", GUILayout.Width(90), GUILayout.Height(30)))
                {
                    mapInfo.UpdateCollider();
                }

                if (GUILayout.Button("更新路径", GUILayout.Width(90), GUILayout.Height(30)))
                {
                    mapInfo.UpdateNavPath();
                }

                GUILayout.EndHorizontal();
                
                if (GUILayout.Button("获取物件", GUILayout.Width(60), GUILayout.Height(20)))
                {
                    MapResourceAsset mapPrefabAsset = (MapResourceAsset)AssetDatabase.LoadAssetAtPath("Assets/ResourceAssets/resources.asset", typeof(MapResourceAsset));
                    if (mapPrefabAsset != null)
                    {
                        mapResouceList = mapPrefabAsset.items;
                    }
                }

                scrollPos = GUILayout.BeginScrollView(scrollPos);
                if (mapResouceList != null)
                {
                    GUILayout.BeginVertical();
                    int count = 0;
                    for (int i = 0; i < mapResouceList.Count; i++)
                    {
                        if (mapResouceList[i].worldResource != MapWorldResource.None && mapResouceList[i].worldResource != worldResource) continue;
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
                GUILayout.EndScrollView();
            }
        }

        GUILayout.EndVertical();
    }


    private Vector2 lastPos = Vector2.zero;

    public void OnSceneGUI(SceneView sceneView)
    {
        if (mapInfo == null) return;
        Vector3 mousepos = Event.current.mousePosition;
        mousepos.y = sceneView.camera.pixelHeight - mousepos.y;
        mousepos = sceneView.camera.ScreenToWorldPoint(mousepos);
        Vector2 grid = MapManager.GetGrid(mousepos.x, mousepos.y);
        if (grid.x < 0 || grid.y < 0 || grid.x >= mapInfo.mapSizeX || grid.y >= mapInfo.mapSizeY) return;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (Event.current.type == EventType.MouseDown)
        {
            if (toolSelected == MapTool.Draw)
            {
                AddTile(grid, selectedTile);
                ClearStoneWallAndEdge();
                GenerateStoneWallAndEdge();
            }
            else if (toolSelected == MapTool.Fill)
            {
                FillTile(grid, selectedTile);
            }
            else if (toolSelected == MapTool.Actor)
            {
                if (selectIndex == 1)
                {
                    if (!mapInfo.randonMonsterPos.Contains(grid))
                    {
                        mapInfo.randonMonsterPos.Add(grid);
                    }
                }
                else if (selectIndex == 2)
                {
                    for (int i = 0; i < mapInfo.appointMonsters.Count; i ++)
                    {
                        if (mapInfo.appointMonsters[i].pos == grid)
                        {
                            return;
                        }
                    }
                    AppointMonster monster = new AppointMonster();
                    monster.pos = grid;
                    mapInfo.appointMonsters.Add(monster);
                }
                else if(selectIndex == 3)
                {
                    if (!mapInfo.randonChestPos.Contains(grid))
                    {
                        mapInfo.randonChestPos.Add(grid);
                    }
                }
                else if (selectIndex == 4)
                {
                    if (!mapInfo.randonBirthPos.Contains(grid))
                    {
                        mapInfo.randonBirthPos.Add(grid);
                    }
                }
                else if (selectIndex == 5)
                {
                    if (!mapInfo.randonAltarPos.Contains(grid))
                    {
                        mapInfo.randonAltarPos.Add(grid);
                    }
                }
                else if (selectIndex == 6)
                {
                    for (int i = 0; i < mapInfo.appointAltars.Count; i++)
                    {
                        if (mapInfo.appointAltars[i].pos == grid)
                        {
                            return;
                        }
                    }
                    AppointAltar atlar = new AppointAltar();
                    atlar.pos = grid;
                    mapInfo.appointAltars.Add(atlar);
                }
                mapInfo.UpdateMesh();
            }
            else if (toolSelected == MapTool.Clear)
            {
                Clear(grid);
            }
        }
        else if (Event.current.type == EventType.MouseDrag)
        {
            if (lastPos == grid) return;
            lastPos = grid;
            if (toolSelected == MapTool.Draw)
            {
                if(!selectedTile.isPrefab)
                {
                    AddTile(grid, selectedTile);
                    ClearStoneWallAndEdge();
                    GenerateStoneWallAndEdge();
                }
            }
            else if (toolSelected == MapTool.Clear)
            {
                Clear(grid);
            }
        }
    }

    public void Clear(Vector2 grid)
    {
        for (int i = 10; i > -1; i--)
        {
            int index = mapInfo.layers.IndexOf((MapEditorSortLayer)i);
            if (index != -1)
            {
                int n = mapInfo.layerItems[index].posList.IndexOf((int)grid.x * 1000 + (int)grid.y);
                if (n != -1)
                {
                    MapResourceItem item = mapInfo.layerItems[index].items[n];
                    mapInfo.layerItems[index].posList.RemoveAt(n);
                    mapInfo.layerItems[index].items.RemoveAt(n);
                    if (item.isPrefab)
                    {
                        mapInfo.CreateScene();
                    }
                    else
                    {
                        mapInfo.UpdateMesh();
                    }
                    break;
                }
            }
        }
    }

    void AddTile(Vector2 pos, MapResourceItem selectedTile)
    {
        AddTile_((int)pos.x * 1000 + (int)pos.y, selectedTile);
        mapInfo.UpdateTile(pos , selectedTile);
    }

    public void FillTile(Vector2 fillPos, MapResourceItem selectedTile)
    {
        int index = mapInfo.layers.IndexOf(selectedTile.layer);
        List<int> allPos = index == -1 ? new List<int>() : mapInfo.layerItems[index].posList;
        List<int> list = new List<int>();
        list.Add((int)fillPos.x * 1000 + (int)fillPos.y);
        List<int> close = new List<int>();
        while (list.Count > 0)
        {
            int x = list[0] / 1000;
            int y = list[0] % 1000;
            int x1 = x - 1;
            int x2 = x + 1;
            int y1 = y - 1;
            int y2 = y + 1;

            int pos1 = 0;

            if (x1 > -1)
            {
                pos1 = x1 * 1000 + y;
                if (!allPos.Contains(pos1) && !close.Contains(pos1) && !list.Contains(pos1))
                {
                    list.Add(pos1);
                    allPos.Remove(pos1);
                }
            }

            if (x2 < mapSizeX)
            {
                pos1 = x2 * 1000 + y;
                if (!allPos.Contains(pos1) && !close.Contains(pos1) && !list.Contains(pos1))
                {
                    list.Add(pos1);
                    allPos.Remove(pos1);
                }
            }

            if (y1 > -1)
            {
                pos1 = x * 1000 + y1;
                if (!allPos.Contains(pos1) && !close.Contains(pos1) && !list.Contains(pos1))
                {
                    list.Add(pos1);
                    allPos.Remove(pos1);
                }
            }

            if (y2 < mapSizeY)
            {
                pos1 = x * 1000 + y2;
                if (!allPos.Contains(pos1) && !close.Contains(pos1) && !list.Contains(pos1))
                {
                    list.Add(pos1);
                    allPos.Remove(pos1);
                }
            }

            if (!close.Contains(list[0]))
            {
                close.Add(list[0]);
            }
            list.RemoveAt(0);
        }
        for (int i = 0; i < close.Count; i++)
        {
            AddTile_(close[i], selectedTile);
        }
        mapInfo.CreateScene();
    }

    public void AddTile_(int pos, MapResourceItem selectedTile)
    {
        int index = mapInfo.layers.IndexOf(selectedTile.layer);
        if (index != -1)
        {
            int i = mapInfo.layerItems[index].posList.IndexOf(pos);
            if (i != -1)
            {
                mapInfo.layerItems[index].items[i] = selectedTile;
            }
            else
            {
                mapInfo.layerItems[index].posList.Add(pos);
                mapInfo.layerItems[index].items.Add(selectedTile);
            }
        }
        else
        {
            mapInfo.layers.Add(selectedTile.layer);
            mapInfo.layerItems.Add(new MapLayerItem());
            mapInfo.layerItems[mapInfo.layerItems.Count - 1].posList.Add(pos);
            mapInfo.layerItems[mapInfo.layerItems.Count - 1].items.Add(selectedTile);
        }
    }

    void RotateMap()
    {
        for (int i = 0; i < mapInfo.randonBirthPos.Count; i++)
        {
            mapInfo.randonBirthPos[i] = new Vector2(mapInfo.randonBirthPos[i].y, mapInfo.mapSizeX - mapInfo.randonBirthPos[i].x - 1);
        }
        for (int i = 0; i < mapInfo.randonAltarPos.Count; i++)
        {
            mapInfo.randonAltarPos[i] = new Vector2(mapInfo.randonAltarPos[i].y, mapInfo.mapSizeX - mapInfo.randonAltarPos[i].x - 1);
        }
        for (int i = 0; i < mapInfo.randonChestPos.Count; i++)
        {
            mapInfo.randonChestPos[i] = new Vector2(mapInfo.randonChestPos[i].y, mapInfo.mapSizeX - mapInfo.randonChestPos[i].x - 1);
        }
        for (int i = 0; i < mapInfo.randonMonsterPos.Count; i ++)
        {
            mapInfo.randonMonsterPos[i] = new Vector2(mapInfo.randonMonsterPos[i].y , mapInfo.mapSizeX - mapInfo.randonMonsterPos[i].x - 1);
        }
        for (int i = 0; i < mapInfo.walkableList.Count; i++)
        {
            mapInfo.walkableList[i] = new Vector2(mapInfo.walkableList[i].y, mapInfo.mapSizeX - mapInfo.walkableList[i].x - 1);
        }
        for (int i = 0; i < mapInfo.appointAltars.Count; i++)
        {
            mapInfo.appointAltars[i].pos = new Vector2(mapInfo.appointAltars[i].pos.y, mapInfo.mapSizeX - mapInfo.appointAltars[i].pos.x - 1);
        }
        for (int i = 0; i < mapInfo.appointMonsters.Count; i++)
        {
            mapInfo.appointMonsters[i].pos = new Vector2(mapInfo.appointMonsters[i].pos.y, mapInfo.mapSizeX - mapInfo.appointMonsters[i].pos.x - 1);
        }
        for (int i = 0; i < mapInfo.layerItems.Count; i ++)
        {
            for (int n = 0; n < mapInfo.layerItems[i].posList.Count; n ++)
            {
                int x = mapInfo.layerItems[i].posList[n] / 1000;
                int y = mapInfo.layerItems[i].posList[n] % 1000;
                mapInfo.layerItems[i].posList[n] = y * 1000 + mapInfo.mapSizeX - x - 1;
            }
        }
        ClearStoneWallAndEdge();
        GenerateStoneWallAndEdge();
        int temp = mapInfo.mapSizeX;
        mapInfo.mapSizeX = mapInfo.mapSizeY;
        mapInfo.mapSizeY = temp;
        mapInfo.CreateScene();
    }

    void MirrorMap1()
    {
        for (int i = 0; i < mapInfo.randonBirthPos.Count; i++)
        {
            mapInfo.randonBirthPos[i] = new Vector2(mapInfo.mapSizeX - mapInfo.randonBirthPos[i].x - 1, mapInfo.randonBirthPos[i].y);
        }
        for (int i = 0; i < mapInfo.randonAltarPos.Count; i++)
        {
            mapInfo.randonAltarPos[i] = new Vector2(mapInfo.mapSizeX - mapInfo.randonAltarPos[i].x - 1, mapInfo.randonAltarPos[i].y);
        }
        for (int i = 0; i < mapInfo.randonChestPos.Count; i++)
        {
            mapInfo.randonChestPos[i] = new Vector2(mapInfo.mapSizeX - mapInfo.randonChestPos[i].x - 1, mapInfo.randonChestPos[i].y);
        }
        for (int i = 0; i < mapInfo.randonMonsterPos.Count; i++)
        {
            mapInfo.randonMonsterPos[i] = new Vector2(mapInfo.mapSizeX - mapInfo.randonMonsterPos[i].x - 1, mapInfo.randonMonsterPos[i].y);
        }
        for (int i = 0; i < mapInfo.walkableList.Count; i++)
        {
            mapInfo.walkableList[i] = new Vector2(mapInfo.mapSizeX - mapInfo.walkableList[i].x - 1, mapInfo.walkableList[i].y);
        }
        for (int i = 0; i < mapInfo.appointAltars.Count; i++)
        {
            mapInfo.appointAltars[i].pos = new Vector2(mapInfo.mapSizeX - mapInfo.appointAltars[i].pos.x - 1, mapInfo.appointAltars[i].pos.y);
        }
        for (int i = 0; i < mapInfo.appointMonsters.Count; i++)
        {
            mapInfo.appointMonsters[i].pos = new Vector2(mapInfo.mapSizeX - mapInfo.appointMonsters[i].pos.x - 1, mapInfo.appointMonsters[i].pos.y);
        }
        for (int i = 0; i < mapInfo.layerItems.Count; i++)
        {
            for (int n = 0; n < mapInfo.layerItems[i].posList.Count; n++)
            {
                int x = mapInfo.layerItems[i].posList[n] / 1000;
                int y = mapInfo.layerItems[i].posList[n] % 1000;
                mapInfo.layerItems[i].posList[n] = (mapInfo.mapSizeX - x - 1) * 1000 + y;
            }
        }
        mapInfo.CreateScene();
    }

    void MirrorMap2()
    {
        for (int i = 0; i < mapInfo.randonBirthPos.Count; i++)
        {
            mapInfo.randonBirthPos[i] = new Vector2(mapInfo.randonBirthPos[i].x, mapInfo.mapSizeY - mapInfo.randonBirthPos[i].y - 1);
        }
        for (int i = 0; i < mapInfo.randonAltarPos.Count; i++)
        {
            mapInfo.randonAltarPos[i] = new Vector2(mapInfo.randonAltarPos[i].x, mapInfo.mapSizeY - mapInfo.randonAltarPos[i].y - 1);
        }
        for (int i = 0; i < mapInfo.randonChestPos.Count; i++)
        {
            mapInfo.randonChestPos[i] = new Vector2(mapInfo.randonChestPos[i].x, mapInfo.mapSizeY - mapInfo.randonChestPos[i].y - 1);
        }
        for (int i = 0; i < mapInfo.randonMonsterPos.Count; i++)
        {
            mapInfo.randonMonsterPos[i] = new Vector2(mapInfo.randonMonsterPos[i].x , mapInfo.mapSizeY - mapInfo.randonMonsterPos[i].y - 1);
        }
        for (int i = 0; i < mapInfo.walkableList.Count; i++)
        {
            mapInfo.walkableList[i] = new Vector2(mapInfo.walkableList[i].x, mapInfo.mapSizeY - mapInfo.walkableList[i].y - 1);
        }
        for (int i = 0; i < mapInfo.appointAltars.Count; i++)
        {
            mapInfo.appointAltars[i].pos = new Vector2(mapInfo.appointAltars[i].pos.x, mapInfo.mapSizeY - mapInfo.appointAltars[i].pos.y - 1);
        }
        for (int i = 0; i < mapInfo.appointMonsters.Count; i++)
        {
            mapInfo.appointMonsters[i].pos = new Vector2(mapInfo.appointMonsters[i].pos.x, mapInfo.mapSizeY - mapInfo.appointMonsters[i].pos.y - 1);
        }
        for (int i = 0; i < mapInfo.layerItems.Count; i++)
        {
            for (int n = 0; n < mapInfo.layerItems[i].posList.Count; n++)
            {
                int x = mapInfo.layerItems[i].posList[n] / 1000;
                int y = mapInfo.layerItems[i].posList[n] % 1000;
                mapInfo.layerItems[i].posList[n] = x * 1000 + (mapInfo.mapSizeY - y - 1);
            }
        }
        mapInfo.CreateScene();
    }

    void ClearStoneWallAndEdge()
    {
        for (int i = 0; i < mapInfo.layerItems.Count; i ++)
        {
            MapLayerItem mapLayerItem = mapInfo.layerItems[i];
            for (int j = mapLayerItem.items.Count - 1; j >= 0; j --)
            {
                if (mapLayerItem.items[j].itemType == MapEditorItemType.StoneWall)
                {
                    mapLayerItem.items[j] = mapLayerItem.items[j].replaceItem;
                }
                else if(mapLayerItem.items[j].itemType == MapEditorItemType.Edge)
                {
                    mapLayerItem.items.RemoveAt(j);
                    mapLayerItem.posList.RemoveAt(j);
                }
            }
        }
    }

    void GenerateStoneWallAndEdge()
    {
        int index = mapInfo.layers.IndexOf(MapEditorSortLayer.Floor1);
        MapLayerItem mapLayerItem = mapInfo.layerItems[index];
        for (int i = 0; i < mapLayerItem.items.Count ; i++)
        {
            if (mapLayerItem.items[i].itemType == MapEditorItemType.Wall)
            {
                int x = mapLayerItem.posList[i] / 1000;
                int y = mapLayerItem.posList[i] % 1000;
                if (y == 0) continue;
                index = mapLayerItem.posList.IndexOf(x * 1000 + y - 1);
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
                int x = mapLayerItem.posList[i] / 1000;
                int y = mapLayerItem.posList[i] % 1000;

                int index1 = mapLayerItem.posList.IndexOf(x * 1000 + y + 1);
                int index2 = mapLayerItem.posList.IndexOf((x - 1) * 1000 + y);
                int index3 = mapLayerItem.posList.IndexOf((x + 1) * 1000 + y);

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
                    index = mapInfo.layers.IndexOf(MapEditorSortLayer.Floor2);
                    MapLayerItem mapLayerEdgeItem = null;
                    if (index == -1)
                    {
                        mapInfo.layers.Add(MapEditorSortLayer.Floor2);
                        mapLayerEdgeItem = new MapLayerItem();
                        mapInfo.layerItems.Add(mapLayerEdgeItem);
                    }
                    else
                    {
                        mapLayerEdgeItem = mapInfo.layerItems[index];
                    }
                    index = mapLayerEdgeItem.posList.IndexOf(x * 1000 + y);
                    if (index == -1)
                    {
                        mapLayerEdgeItem.posList.Add(x * 1000 + y);
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
        filePath = EditorUtility.SaveFilePanel("保存", filePath, "", "asset");
        if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
        mapInfo.UpdateNavPath();
        MapSceneAsset newData = ScriptableObject.CreateInstance<MapSceneAsset>();
        newData.Clone(mapInfo);
        AssetDatabase.CreateAsset(newData, "Assets" + filePath.Replace(Application.dataPath, ""));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void LoadSceneAsset()
    {
        filePath = EditorUtility.OpenFilePanel("打开", filePath, "asset");
        if (string.IsNullOrEmpty(filePath) || !filePath.Contains(".asset")) return;
        MapSceneAsset mapPrefabAsset = (MapSceneAsset)AssetDatabase.LoadAssetAtPath("Assets" + filePath.Replace(Application.dataPath , ""), typeof(MapSceneAsset));
        if (mapPrefabAsset != null)
        {
            if (mapInfo == null)
            {
                mapInfo = new GameObject("Terrain").AddComponent<MapScene>();
            }
            mapInfo.Clone(mapPrefabAsset);
            mapInfo.CreateScene();
        }
    }
}
