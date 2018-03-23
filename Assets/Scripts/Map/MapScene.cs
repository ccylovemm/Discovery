using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MapScene : MonoBehaviour
{
    public int mapSizeX;
    public int mapSizeY;

    public MapWorldResource worldResource;

    public int offsetX;
    public int offsetY;
    
    public List<Vector2> randonBirthPos = new List<Vector2>();
    public List<Vector2> randonAltarPos = new List<Vector2>();
    public List<Vector2> randonChestPos = new List<Vector2>();
    public List<Vector2> randonMonsterPos = new List<Vector2>();
    public List<AppointAltar> appointAltars = new List<AppointAltar>();
    public List<AppointMonster> appointMonsters = new List<AppointMonster>();

    public List<Vector2> walkableList = new List<Vector2>();
    public List<MapLayerItem> layerItems = new List<MapLayerItem>();
    public List<MapEditorSortLayer> layers = new List<MapEditorSortLayer>();

    public bool isShowPath = false;

    private List<Rect> rects = new List<Rect>();
    private List<int> m_triangles = new List<int>();
    private List<Vector2> m_uv = new List<Vector2>();
    private List<Vector2> posList = new List<Vector2>();
    private List<Vector3> m_vertices = new List<Vector3>();
    private List<Vector2> offsetList = new List<Vector2>();

    private GameObject sceneMeshObj;
    private Vector2 texelSize;

    private List<GameObject> colliderObjs = new List<GameObject>();
    private List<GameObject> generatePrefabItems = new List<GameObject>();
    private List<GameObject> generateMountainsItems = new List<GameObject>();

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, .2f);

        for (int i = 0; i <= mapSizeX; i++)
        {
            Gizmos.DrawLine(new Vector3((i + offsetX) * MapManager.textSize, offsetY * MapManager.textSize, 0), new Vector3((i + offsetX) * MapManager.textSize, (mapSizeY + offsetY) * MapManager.textSize, 0));
        }

        for (int i = 0; i <= mapSizeY; i++)
        {
            Gizmos.DrawLine(new Vector3(offsetX * MapManager.textSize, (i + offsetY) * MapManager.textSize, 0), new Vector3((mapSizeX + offsetX) * MapManager.textSize, (i + offsetY) * MapManager.textSize, 0));
        }
        if(isShowPath)
        {
            Gizmos.color = new Color(1f, 0.5f, 1f, 0.5f);
            for (int i = 0; i < walkableList.Count; i++)
            {
                Gizmos.DrawCube(MapManager.GetPos((int)walkableList[i].x + offsetX, (int)walkableList[i].y + offsetY), new Vector3(MapManager.textSize, MapManager.textSize, 0));
            }
        }
        Gizmos.color = new Color(0, 1.0f, 0, 0.4f);
        for (int i = 0; i < randonBirthPos.Count; i++)
        {
            Gizmos.DrawCube(MapManager.GetPos((int)randonBirthPos[i].x + offsetX, (int)randonBirthPos[i].y + offsetY), Vector3.one * 0.01f * (i + 10));
        }
        Gizmos.color = new Color(0.9f, 0, 00.9f, 0.4f);
        for (int i = 0; i < randonChestPos.Count; i++)
        {
            Gizmos.DrawCube(MapManager.GetPos((int)randonChestPos[i].x + offsetX, (int)randonChestPos[i].y + offsetY), Vector3.one * 0.01f * (i + 10));
        }
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.4f);
        for (int i = 0; i < randonAltarPos.Count; i++)
        {
            Gizmos.DrawCube(MapManager.GetPos((int)randonAltarPos[i].x + offsetX, (int)randonAltarPos[i].y + offsetY) + new Vector2(MapManager.textSize / 2.0f, MapManager.textSize / 2.0f) , new Vector3(2 * MapManager.textSize , 2 * MapManager.textSize, 0));
        }
        Gizmos.color = new Color(0.3f, 0.8f, 0.2f, 0.4f);
        for (int i = 0; i < appointAltars.Count; i++)
        {
            Gizmos.DrawCube(MapManager.GetPos((int)appointAltars[i].pos.x + offsetX, (int)appointAltars[i].pos.y + offsetY) + new Vector2(MapManager.textSize / 2.0f, MapManager.textSize / 2.0f), new Vector3(2 * MapManager.textSize, 2 * MapManager.textSize, 0));
        }
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.4f);
        for (int i = 0; i < randonMonsterPos.Count; i++)
        {
            Gizmos.DrawSphere(MapManager.GetPos((int)randonMonsterPos[i].x + offsetX, (int)randonMonsterPos[i].y + offsetY), 0.1f + i * 0.01f);
        }
        Gizmos.color = new Color(0.3f, 0.8f, 0.2f, 0.4f);
        for (int i = 0; i < appointMonsters.Count; i++)
        {
            Gizmos.DrawSphere(MapManager.GetPos((int)appointMonsters[i].pos.x + offsetX, (int)appointMonsters[i].pos.y + offsetY), 0.1f + i * 0.01f);
        }
    }
#endif

    public void Clear(bool bol= true)
    {
        rects.Clear();
        m_triangles.Clear();
        m_uv.Clear();
        posList.Clear();
        m_vertices.Clear();
        offsetList.Clear();

        int count = colliderObjs.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.DestroyImmediate(colliderObjs[i]);
        }
        colliderObjs.Clear();
        count = generatePrefabItems.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.DestroyImmediate(generatePrefabItems[i]);
        }
        generatePrefabItems.Clear();
        count = generateMountainsItems.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.DestroyImmediate(generateMountainsItems[i]);
        }
        generateMountainsItems.Clear();
        GameObject.DestroyImmediate(sceneMeshObj);
        if(bol) GameObject.DestroyImmediate(gameObject);
    }

    public void Clone(MapSceneAsset mapSceneAsset)
    {
        mapSizeX = mapSceneAsset.mapSizeX;
        mapSizeY = mapSceneAsset.mapSizeY;
        worldResource = mapSceneAsset.worldResource;
        layerItems.Clear();
        int count = mapSceneAsset.layerItems.Count;
        for (int i = 0; i < count; i++)
        {
            MapLayerItem newItem = new MapLayerItem();
            MapLayerItem mapLayerItem = mapSceneAsset.layerItems[i];
            newItem.posList = new List<int>(mapLayerItem.posList);
            newItem.items = new List<MapResourceItem>(mapLayerItem.items);
            layerItems.Add(newItem);
        }

        appointAltars.Clear();
        count = mapSceneAsset.appointAltars.Count;
        for (int i = 0; i < count; i++)
        {
            AppointAltar newAltar = new AppointAltar();
            AppointAltar appointAltar = mapSceneAsset.appointAltars[i];
            newAltar.pos = appointAltar.pos;
            newAltar.groups = new List<int>(appointAltar.groups);
            appointAltars.Add(newAltar);
        }

        appointMonsters.Clear();
        count = mapSceneAsset.appointMonsters.Count;
        for (int i = 0; i < count; i++)
        {
            AppointMonster newMonster = new AppointMonster();
            AppointMonster appointMonster = mapSceneAsset.appointMonsters[i];
            newMonster.pos = appointMonster.pos;
            newMonster.groups = new List<int>(appointMonster.groups);
            appointMonsters.Add(newMonster);
        }

        layers = new List<MapEditorSortLayer>(mapSceneAsset.layers);
        walkableList = new List<Vector2>(mapSceneAsset.walkableList);
        randonBirthPos = new List<Vector2>(mapSceneAsset.randonBirthPos);
        randonChestPos = new List<Vector2>(mapSceneAsset.randonChestPos);
        randonAltarPos = new List<Vector2>(mapSceneAsset.randonAltarPos);
        randonMonsterPos = new List<Vector2>(mapSceneAsset.randonMonsterPos);
    }

    public void CreateMonster()
    {
        string level = SceneManager.Instance.currLevelVo.MonsterLevel;

        while (appointMonsters.Count > 0)
        {
            if (appointMonsters[0].groups.Count > 0)
            {
                Vector2 pos = appointMonsters[0].pos;
                int group = appointMonsters[0].groups[Random.Range(0, appointMonsters[0].groups.Count)];
                if (MonsterGroupCFG.items.ContainsKey(group.ToString()))
                {
                    MonsterGroupVo groupVo = MonsterGroupCFG.items[group.ToString()];
                    object o = groupVo.GetType().GetField(level).GetValue(groupVo);
                    SceneManager.Instance.CreateMonster(pos + new Vector2(offsetX, offsetY), System.Convert.ToString(o));

                }
            }
            appointMonsters.RemoveAt(0);
        }
    }

    public void CreateAppointAltar()
    {
        while (appointAltars.Count > 0)
        {
            int group = appointAltars[0].groups[Random.Range(0, appointAltars[0].groups.Count)];
            if (AltarCFG.items.ContainsKey(group.ToString()))
            {
                SceneManager.Instance.CreateAltar((uint)group , appointAltars[0].pos + new Vector2(offsetX, offsetY));

            }
            appointAltars.RemoveAt(0);
        }
    }

    public void UpdateNavPath()
    {
        walkableList.Clear();
        List<int> nonWalkableList = new List<int>();
        for (int i = 0; i < layerItems.Count; i ++)
        {
            for (int j = 0; j < layerItems[i].items.Count; j ++)
            {
                Vector2 p = new Vector2(layerItems[i].posList[j] / 1000, layerItems[i].posList[j] % 1000);
                if (layerItems[i].items[j].isPath)
                {
                    if(nonWalkableList.IndexOf(layerItems[i].posList[j]) == -1 && walkableList.IndexOf(p) == -1)
                    {
                        walkableList.Add(p);
                    }
                }
                else
                {
                    if (nonWalkableList.IndexOf(layerItems[i].posList[j]) == -1)
                    {
                        nonWalkableList.Add(layerItems[i].posList[j]);
                    }
                    if (walkableList.IndexOf(p) != -1)
                    {
                        walkableList.Remove(p);
                    }
                }
            }
        }
    }

    public void UpdateCollider()
    {
        for (int i = 0; i < colliderObjs.Count; i++)
        {
            GameObject.DestroyImmediate(colliderObjs[i]);
        }
        colliderObjs.Clear();

        int index = layers.IndexOf(MapEditorSortLayer.Floor1);
        Dictionary<MapEditorItemType, List<int>> itemType = new Dictionary<MapEditorItemType, List<int>>();
        int count = layerItems[index].items.Count;

        for (int i = 0; i < count; i++)
        {
            MapEditorItemType mapEditorItemType = layerItems[index].items[i].itemType;
            if (mapEditorItemType == MapEditorItemType.None || mapEditorItemType == MapEditorItemType.Deco) continue;
            if (itemType.ContainsKey(mapEditorItemType))
            {
                itemType[mapEditorItemType].Add(layerItems[index].posList[i]);
            }
            else
            {
                itemType.Add(mapEditorItemType, new List<int>());
                itemType[mapEditorItemType].Add(layerItems[index].posList[i]);
            }
        }

        Dictionary<MapEditorItemType, List<List<int>>> colliders = new Dictionary<MapEditorItemType, List<List<int>>>();
        foreach (var items in itemType)
        {
            List<int> allPos = items.Value;
            while (allPos.Count > 0)
            {
                List<int> list = new List<int>();
                list.Add(allPos[0]);
                allPos.RemoveAt(0);
                List<int> close = new List<int>();
                while (list.Count > 0)
                {
                    int x = list[0] / 1000;
                    int y = list[0] % 1000;
                    int pos1 = (x - 1) * 1000 + y;
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = (x + 1) * 1000 + y;
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = x * 1000 + (y - 1);
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = x * 1000 + (y + 1);
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    if (close.IndexOf(list[0]) == -1)
                    {
                        close.Add(list[0]);
                    }
                    list.RemoveAt(0);
                }
                if (colliders.ContainsKey(items.Key))
                {
                    colliders[items.Key].Add(close);
                }
                else
                {
                    colliders.Add(items.Key, new List<List<int>>());
                    colliders[items.Key].Add(close);
                }
            }
        }

        foreach (var collider in colliders)
        {
            List<List<int>> posList = collider.Value;
            int c = posList.Count;
            for (int i = 0; i < c; i++)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector2[]> polygons = new List<Vector2[]>();
                List<Collider2DEdge> edges = new List<Collider2DEdge>();
                int cc = posList[i].Count;
                for (int j = 0; j < cc; j++)
                {
                    int x = posList[i][j] / 1000;
                    int y = posList[i][j] % 1000;

                    count = vertices.Count / 4;
                    vertices.AddRange(new Vector3[4]);
                    vertices[count * 4] = new Vector3(x, y);
                    vertices[count * 4 + 1] = new Vector3(x + 1, y);
                    vertices[count * 4 + 2] = new Vector3(x + 1, y + 1);
                    vertices[count * 4 + 3] = new Vector3(x, y + 1);

                    if (posList[i].IndexOf(x * 1000 + (y + 1)) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 2], vertices[count * 4 + 3]);
                    }

                    if (posList[i].IndexOf(x * 1000 + (y - 1)) == -1)
                    {
                        AddEdge(edges, vertices[count * 4], vertices[count * 4 + 1]);
                    }

                    if (posList[i].IndexOf((x - 1) * 1000 + y) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 3], vertices[count * 4]);
                    }

                    if (posList[i].IndexOf((x + 1) * 1000 + y) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 1], vertices[count * 4 + 2]);
                    }
                }

                GameObject go = new GameObject();
                PolygonCollider2D polygonCollider2D = go.AddComponent<PolygonCollider2D>();

                if (collider.Key == MapEditorItemType.Hole)
                {
                    polygonCollider2D.isTrigger = true;
                    go.AddComponent<MapTerrainFall>();
                    go.layer = LayerUtil.LayerToHole();
                }
                else if (collider.Key == MapEditorItemType.Water)
                {
                    polygonCollider2D.isTrigger = true;
                    MapTerrainBuff mapTerrainBuff = go.AddComponent<MapTerrainBuff>();
                    mapTerrainBuff.buffId = 2;
                    mapTerrainBuff.terrainType = MapEditorItemType.Water;
                }
                else if (collider.Key == MapEditorItemType.Lava)
                {
                    polygonCollider2D.isTrigger = true;
                    MapTerrainBuff mapTerrainBuff = go.AddComponent<MapTerrainBuff>();
                    mapTerrainBuff.buffId = 1;
                    mapTerrainBuff.terrainType = MapEditorItemType.Lava;
                }
                else if (collider.Key == MapEditorItemType.Mud)
                {
                    polygonCollider2D.isTrigger = true;
                    MapTerrainBuff mapTerrainBuff = go.AddComponent<MapTerrainBuff>();
                    mapTerrainBuff.buffId = 29;
                    mapTerrainBuff.terrainType = MapEditorItemType.Mud;
                }
                else if (collider.Key == MapEditorItemType.DeepWater)
                {
                    polygonCollider2D.isTrigger = true;
                    go.AddComponent<MapTerrainFall>();
                    go.layer = LayerUtil.LayerToHole();
                }
                else if (collider.Key == MapEditorItemType.Wall || collider.Key == MapEditorItemType.StoneWall)
                {
                    go.layer = LayerUtil.LayerToObstacle();
                }

                colliderObjs.Add(go);
                PolygoniseEdges(polygons, edges);
                polygonCollider2D.pathCount = polygons.Count;
                for (int n = 0; n < polygons.Count; n++)
                {
                    polygonCollider2D.SetPath(n, polygons[n]);
                }
            }
        }
    }

    public void UpdateMesh()
    {
        rects.Clear();
        m_triangles.Clear();
        m_uv.Clear();
        posList.Clear();
        m_vertices.Clear();
        offsetList.Clear();

        for (int i = 0; i < 7; i++)
        {
            int index = layers.IndexOf((MapEditorSortLayer)i);
            if (index != -1)
            {
                Dictionary<int, MapResourceItem> mapGird = new Dictionary<int, MapResourceItem>();
                for (int n = 0; n < layerItems[index].items.Count; n++)
                {
                    mapGird.Add(layerItems[index].posList[n], layerItems[index].items[n]);
                }
                int count = layerItems[index].items.Count;
                for (int j = 0; j < count; j++)
                {
                    int x = layerItems[index].posList[j] / 1000;
                    int y = layerItems[index].posList[j] % 1000;
                    if (!layerItems[index].items[j].isPrefab)
                    {
                        List<MapSprite> list = GetSpriteList(x + offsetX, y + offsetY, mapGird);
                        int random = Random.Range(0, 10000);
                        MapSprite mapSprite = list.Where(s => random < s.sRate).FirstOrDefault();
                        rects.Add(mapSprite.sprite.rect);
                        posList.Add(new Vector2((x + offsetX) * MapManager.textSize + MapManager.textSize / 2.0f, (y + offsetY) * MapManager.textSize + MapManager.textSize / 2.0f));
                        offsetList.Add(new Vector2(mapSprite.offsetX, mapSprite.offsetY) * MapManager.textSize * 0.05f);
                    }
                }
            }
        }
        if (Application.isPlaying)
        {
            CombineMesh();
        }
        else
        {
#if UNITY_EDITOR
            CombineMeshFromEditor();
#endif
        }
    }

    public void CreateScene(Dictionary<int, MapResourceItem> mapAllGird = null)
    {
        Clear(false);
        for (int i = 0; i < 10; i++)
        {
            int index = layers.IndexOf((MapEditorSortLayer)i);
            if (index != -1)
            {
                MapLayerItem mapLayerItem = layerItems[index];

                Dictionary<int, MapResourceItem> mapGird = new Dictionary<int, MapResourceItem>();

                if (i != 0 || mapAllGird == null)
                {
                    int c = mapLayerItem.items.Count;
                    for (int n = 0; n < c; n++)
                    {
                        mapGird.Add(mapLayerItem.posList[n] + offsetX * 1000 + offsetY, mapLayerItem.items[n]);
                    }
                }
                else
                {
                    mapGird = mapAllGird;
                }

                int count = mapLayerItem.items.Count;
                for (int j = 0; j < count; j++)
                {
                    int x = mapLayerItem.posList[j] / 1000;
                    int y = mapLayerItem.posList[j] % 1000;
                    if (mapLayerItem.items[j].isPrefab)
                    {
                        GameObject go = GameObject.Instantiate(mapLayerItem.items[j].gameObject);
                        go.transform.position = MapManager.GetPos(x + offsetX, y + offsetY);
                        generatePrefabItems.Add(go);
                    }
                    else
                    {
                        List<MapSprite> list = GetSpriteList(x + offsetX, y + offsetY, mapGird);
                        int random = Random.Range(0 , 10000);
                        MapSprite mapSprite = list.Where(s => random < s.sRate).FirstOrDefault();
                        rects.Add(mapSprite.sprite.rect);
                        posList.Add(new Vector2((x + offsetX) * MapManager.textSize + MapManager.textSize / 2.0f, (y + offsetY) * MapManager.textSize + MapManager.textSize / 2.0f));
                        offsetList.Add(new Vector2(mapSprite.offsetX, mapSprite.offsetY) * MapManager.textSize * 0.05f);
                    }
                }
            }
        }
        if (Application.isPlaying)
        {
            CombineMesh();
        }
        else
        {
#if UNITY_EDITOR
            CombineMeshFromEditor();
#endif
        }
    }

    public void UpdateTile(Vector2 pos, MapResourceItem selectedTile)
    {
        if (selectedTile.isPrefab)
        {
            GameObject go = GameObject.Instantiate(selectedTile.gameObject);
            go.transform.position = MapManager.GetPos((int)pos.x + offsetX, (int)pos.y + offsetY);
            generatePrefabItems.Add(go);
        }
        else
        {
            UpdateMesh();
        }
    }


    void AddEdge(List<Collider2DEdge> edges, Vector2 p1, Vector2 p2)
    {
        int index = edges.Count;
        Collider2DEdge thisEdge = new Collider2DEdge(p1, p2);
        for (int i = 0; i < index; i++)
        {
            if (Approximately(edges[i].p1, p1))
            {
                edges[i].p1Connections.Add(new Collider2DEdgeConnection(index, true));
                thisEdge.p1Connections.Add(new Collider2DEdgeConnection(i, true));
            }
            else if (Approximately(edges[i].p1, p2))
            {
                edges[i].p1Connections.Add(new Collider2DEdgeConnection(index, false));
                thisEdge.p2Connections.Add(new Collider2DEdgeConnection(i, true));
            }
            else if (Approximately(edges[i].p2, p1))
            {
                edges[i].p2Connections.Add(new Collider2DEdgeConnection(index, true));
                thisEdge.p1Connections.Add(new Collider2DEdgeConnection(i, false));
            }
            else if (Approximately(edges[i].p2, p2))
            {
                edges[i].p2Connections.Add(new Collider2DEdgeConnection(index, false));
                thisEdge.p2Connections.Add(new Collider2DEdgeConnection(i, false));
            }
        }
        edges.Add(thisEdge);
    }

    bool Approximately(Vector2 v1, Vector2 v2)
    {
        float tolerance = 0.05f;
        return (v1.x > v2.x - tolerance && v1.x < v2.x + tolerance && v1.y > v2.y - tolerance && v1.y < v2.y + tolerance);
    }

    void PolygoniseEdges(List<Vector2[]> polygons, List<Collider2DEdge> edges)
    {
        if (edges.Count == 0)
            return;
        List<Vector2> currentPolygon = new List<Vector2>();
        currentPolygon.Add((edges[0].p1 + new Vector2(offsetX , offsetY)) * MapManager.textSize);
        bool isP2End = true;
        List<int> edgeIndices = new List<int>(edges.Count);
        int count = edges.Count;
        for (int i = 0; i < count; i++)
        {
            edgeIndices.Add(i);
        }
        int index = 0;
        while (edgeIndices.Count > 0)
        {
            edgeIndices.Remove(index);

            if ((isP2End && edges[index].p2Connections.Count == 0) || (!isP2End && edges[index].p1Connections.Count == 0))
            {
                // no loop found polygon is complete
                RemoveEdgeConnections(edges, index);
                polygons.Add(currentPolygon.ToArray());
                currentPolygon.Clear();
                if (edgeIndices.Count == 0)
                    break;
                index = edgeIndices[0];
                currentPolygon.Add((edges[index].p1 + new Vector2(offsetX, offsetY)) * MapManager.textSize);
                isP2End = true;
                continue;
            }

            Vector3 thisPoint = isP2End ? edges[index].p2 : edges[index].p1;
            int newIndex = isP2End ? edges[index].p2Connections[0].index : edges[index].p1Connections[0].index;
            bool newIsP2End = isP2End ? edges[index].p2Connections[0].toP1 : edges[index].p1Connections[0].toP1;
            if (isP2End && newIsP2End)
            {
                if ((!Mathf.Approximately(edges[index].p1.x, thisPoint.x) || !Mathf.Approximately(thisPoint.x, edges[newIndex].p2.x)) &&
                    (!Mathf.Approximately(edges[index].p1.y, thisPoint.y) || !Mathf.Approximately(thisPoint.y, edges[newIndex].p2.y)))
                    currentPolygon.Add(((Vector2)thisPoint + new Vector2(offsetX, offsetY)) * MapManager.textSize);
            }
            if (isP2End && !newIsP2End)
            {
                if ((!Mathf.Approximately(edges[index].p1.x, thisPoint.x) || !Mathf.Approximately(thisPoint.x, edges[newIndex].p1.x)) &&
                    (!Mathf.Approximately(edges[index].p1.y, thisPoint.y) || !Mathf.Approximately(thisPoint.y, edges[newIndex].p1.y)))
                    currentPolygon.Add(((Vector2)thisPoint + new Vector2(offsetX, offsetY)) * MapManager.textSize);
            }
            if (!isP2End && newIsP2End)
            {
                if ((!Mathf.Approximately(edges[index].p2.x, thisPoint.x) || !Mathf.Approximately(thisPoint.x, edges[newIndex].p2.x)) &&
                    (!Mathf.Approximately(edges[index].p2.y, thisPoint.y) || !Mathf.Approximately(thisPoint.y, edges[newIndex].p2.y)))
                    currentPolygon.Add(((Vector2)thisPoint + new Vector2(offsetX, offsetY)) * MapManager.textSize);
            }
            if (!isP2End && !newIsP2End)
            {
                if ((!Mathf.Approximately(edges[index].p2.x, thisPoint.x) || !Mathf.Approximately(thisPoint.x, edges[newIndex].p1.x)) &&
                    (!Mathf.Approximately(edges[index].p2.y, thisPoint.y) || !Mathf.Approximately(thisPoint.y, edges[newIndex].p1.y)))
                    currentPolygon.Add(((Vector2)thisPoint + new Vector2(offsetX, offsetY)) * MapManager.textSize);
            }
            RemoveEdgeConnections(edges, index);

            index = newIndex;
            isP2End = newIsP2End;
        }
        return;
    }

    void RemoveEdgeConnections(List<Collider2DEdge> edges, int index)
    {
        int count = edges[index].p2Connections.Count;
        for (int i = 0; i < count; i++)
        {
            int j = edges[index].p2Connections[i].index;
            if (edges[index].p2Connections[i].toP1)
            {
                int c = edges[j].p1Connections.Count;
                for (int k = 0; k < c; k++)
                {
                    if (edges[j].p1Connections[k].index == index)
                    {
                        edges[j].p1Connections.RemoveAt(k);
                        break;
                    }
                }
            }
            else
            {
                int c = edges[j].p2Connections.Count;
                for (int k = 0; k < edges[j].p2Connections.Count; k++)
                {
                    if (edges[j].p2Connections[k].index == index)
                    {
                        edges[j].p2Connections.RemoveAt(k);
                        break;
                    }
                }
            }
        }
        count = edges[index].p1Connections.Count;
        for (int i = 0; i < count; i++)
        {
            int j = edges[index].p1Connections[i].index;
            if (edges[index].p1Connections[i].toP1)
            {
                int c = edges[j].p1Connections.Count;
                for (int k = 0; k < c; k++)
                {
                    if (edges[j].p1Connections[k].index == index)
                    {
                        edges[j].p1Connections.RemoveAt(k);
                        break;
                    }
                }
            }
            else
            {
                int c = edges[j].p2Connections.Count;
                for (int k = 0; k < c; k++)
                {
                    if (edges[j].p2Connections[k].index == index)
                    {
                        edges[j].p2Connections.RemoveAt(k);
                        break;
                    }
                }
            }
        }
    }

    public List<MapSprite> GetSpriteList(int i, int j, Dictionary<int, MapResourceItem> girdInfo)
    {
        MapResourceItem item = girdInfo[i * 1000 + j];

        bool grid1 = girdInfo.ContainsKey((i - 1) * 1000 + j);
        bool grid2 = girdInfo.ContainsKey((i + 1) * 1000 + j);
        bool grid3 = girdInfo.ContainsKey(i * 1000 + (j - 1));
        bool grid4 = girdInfo.ContainsKey(i * 1000 + (j + 1));
        bool grid5 = girdInfo.ContainsKey((i - 1) * 1000 + (j + 1));
        bool grid6 = girdInfo.ContainsKey((i + 1) * 1000 + (j + 1));
        bool grid7 = girdInfo.ContainsKey((i - 1) * 1000 + (j - 1));
        bool grid8 = girdInfo.ContainsKey((i + 1) * 1000 + (j - 1));

        bool wall1 = item.itemType == MapEditorItemType.Wall && !grid1;
        bool wall2 = item.itemType == MapEditorItemType.Wall && !grid2;
        bool wall3 = item.itemType == MapEditorItemType.Wall && !grid3;
        bool wall4 = item.itemType == MapEditorItemType.Wall && !grid4;
        bool wall5 = item.itemType == MapEditorItemType.Wall && !grid5;
        bool wall6 = item.itemType == MapEditorItemType.Wall && !grid6;
        bool wall7 = item.itemType == MapEditorItemType.Wall && !grid7;
        bool wall8 = item.itemType == MapEditorItemType.Wall && !grid8;

        bool left = wall1 || grid1 && (girdInfo[(i - 1) * 1000 + j].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i - 1) * 1000 + j].itemType == MapEditorItemType.DeepWater);
        bool right = wall2 || grid2 && (girdInfo[(i + 1) * 1000 + j].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i + 1) * 1000 + j].itemType == MapEditorItemType.DeepWater);
        bool down = wall3 || grid3 && (girdInfo[i * 1000 + (j - 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[i * 1000 + (j - 1)].itemType == MapEditorItemType.DeepWater);
        bool up = wall4 || grid4 && (girdInfo[i * 1000 + (j + 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[i * 1000 + (j + 1)].itemType == MapEditorItemType.DeepWater);
        bool leftUp = wall5 || grid5 && (girdInfo[(i - 1) * 1000 + (j + 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i - 1) * 1000 + (j + 1)].itemType == MapEditorItemType.DeepWater);
        bool rightUp = wall6 || grid6 && (girdInfo[(i + 1) * 1000 + (j + 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i + 1) * 1000 + (j + 1)].itemType == MapEditorItemType.DeepWater);
        bool leftDown = wall7 || grid7 && (girdInfo[(i - 1) * 1000 + (j - 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i - 1) * 1000 + (j - 1)].itemType == MapEditorItemType.DeepWater);
        bool rightDown = wall8 || grid8 && (girdInfo[(i + 1) * 1000 + (j - 1)].itemType == item.itemType || item.itemType == MapEditorItemType.Water && girdInfo[(i + 1) * 1000 + (j - 1)].itemType == MapEditorItemType.DeepWater);


        if (item.isNine)
        {
            if (!left && !right && !down && !up)
            {
                //单个
                return item.single;
            }
            else if (!left && right && !down && !up)
            {
                //向左单个
                return item.singleLeft;
            }
            else if (left && !right && !down && !up)
            {
                //向右单个
                return item.singleRight;
            }
            else if (!left && !right && down && !up)
            {
                //向上单个
                return item.singleUp;
            }
            else if (!left && !right && !down && up)
            {
                //向下单个
                return item.singleDown;
            }
            else if (left && right && !down && !up)
            {
                //横向双通
                return item.pass1_1;
            }
            else if (!left && !right && down && up)
            {
                //竖向双通
                return item.pass1_2;
            }
            else if (left && right && !down && up && !leftUp && !rightUp)
            {
                //上三通
                return item.pass2_1;
            }
            else if (left && right && down && !up && !leftDown && !rightDown)
            {
                //下三通
                return item.pass2_2;
            }
            else if (left && !right && down && up && !leftUp && !leftDown)
            {
                //左三通
                return item.pass2_3;
            }
            else if (!left && right && down && up && !rightUp && !rightDown)
            {
                //右三通
                return item.pass2_4;
            }
            else if (left && right && down && up && !leftUp && !rightUp && !leftDown && !rightDown)
            {
                //上下左右四通
                return item.passAll;
            }
            else if (!left && right && down && !up && !rightDown)
            {
                //左上拐角
                return item.corner1_1;
            }
            else if (left && !right && down && !up && !leftDown)
            {
                //右上拐角
                return item.corner1_2;
            }
            else if (!left && right && !down && up && !rightUp)
            {
                //左下拐角 
                return item.corner1_3;
            }
            else if (left && !right && !down && up && !leftUp)
            {
                //右下拐角
                return item.corner1_4;
            }
            else if (!left && right && down && !up && rightDown)
            {
                //左外上角
                return item.out_1;
            }
            else if (left && !right && down && !up && leftDown)
            {
                //右外上角
                return item.out_2;
            }
            else if (!left && right  && !down && up && rightUp)
            {
                //左外下角
                return item.out_3;
            }
            else if (left && !right && !down && up && leftUp)
            {
                //右外下角
                return item.out_4;
            }
            else if (leftDown && left && leftUp && down && up && !rightDown && right && rightUp)
            {
                //左上内角
                return item.in_1;
            }
            else if (!leftDown && left && leftUp && down && up && rightDown && right && rightUp)
            {
                //右上内角
                return item.in_2;
            }
            else if (leftDown && left && leftUp && down && up && rightDown && right && !rightUp)
            {
                //左下内角
                return item.in_3;
            }
            else if (leftDown && left && !leftUp && down && up && rightDown && right && rightUp)
            {
                //右下内角
                return item.in_4;
            }
            else if (left && right && down && !up && rightDown && leftDown)
            {
                //上边
                return item.up;
            }
            else if (left && right && !down && up && rightUp && leftUp)
            {
                //下边
                return item.down;
            }
            else if (!left && right && down && up && rightDown && rightUp)
            {
                //左边
                return item.left;
            }
            else if (left && !right && down && up && leftDown && leftUp)
            {
                //右边
                return item.right;
            }
            else if (!leftDown && left && leftUp && down && up && rightDown && right && !rightUp)
            {
                //左上右下对角
                return item.angle1_1;
            }
            else if (leftDown && left && !leftUp && down && up && !rightDown && right && rightUp)
            {
                //左下右上对角
                return item.angle1_2;
            }
            else if (leftDown && left && !leftUp && down && up && rightDown && right && !rightUp)
            {
                //上凸
                return item.pass3_1;
            }
            else if (!leftDown && left && leftUp && down && up && !rightDown && right && rightUp)
            {
                //下凸
                return item.pass3_2;
            }
            else if (!leftDown && left && !leftUp && down && up && rightDown && right && rightUp)
            {
                //左凸
                return item.pass3_3;
            }
            else if (leftDown && left && leftUp && down && up && !rightDown && right && !rightUp)
            {
                //右凸
                return item.pass3_4;
            }
            else if (!left && down && up && rightDown && right && !rightUp)
            {
                //上凸靠左
                return item.pass4_1;
            }
            else if (leftDown && left && !leftUp && down && up && !right)
            {
                //上凸靠右
                return item.pass4_2;
            }
            else if (!left && down && up && !rightDown && right && rightUp)
            {
                //下凸靠左
                return item.pass4_3;
            }
            else if (!leftDown && left && leftUp && down && up && !right)
            {
                //下凸靠右
                return item.pass4_4;
            }
            else if (!leftDown && left && down && !up && rightDown && right)
            {
                //左凸靠上
                return item.pass4_5;
            }
            else if (left && !down && up && right && rightUp)
            {
                //左凸靠下
                return item.pass4_6;
            }
            else if (leftDown && left  && down  && !up && !rightDown && right)
            {
                //右凸靠上
                return item.pass4_7;
            }
            else if (left && leftUp && !down && up && right && !rightUp)
            {
                //右凸靠下
                return item.pass4_8;
            }
            else if (!leftDown && left && !leftUp && !down && up && right && rightDown && !rightUp)
            {
                //左上通两路
                return item.pass5_1;
            }
            else if (leftDown && left && !leftUp && down && up && right && !rightDown && !rightUp)
            {
                //右上通两路
                return item.pass5_2;
            }
            else if (!leftDown && left && !leftUp && down && up && right && !rightDown && rightUp)
            {
                //左下通两路
                return item.pass5_3;
            }
            else if (!leftDown && left && leftUp && down && up && right && !rightDown && !rightUp)
            {
                //右下通两路
                return item.pass5_4;
            }
            else
            {
                //中心
                return item.center;
            }
        }
        else
        {
            return item.normalList;
        }
    }

#if UNITY_EDITOR
    public void CombineMeshFromEditor()
    {
        sceneMeshObj = GameObject.Find("SceneMeshCombine");
        if (sceneMeshObj == null)
        {
            sceneMeshObj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ResourceAssets/Prefabs/Map/World" + (int)worldResource + ".prefab"));
            sceneMeshObj.name = "SceneMeshCombine";
        }

        MeshRenderer m_meshRenderer = sceneMeshObj.GetComponent<MeshRenderer>();
        m_meshRenderer.sortingLayerName = "TerrainDown";
        m_meshRenderer.sortingOrder = 0;

        texelSize = m_meshRenderer.sharedMaterial.mainTexture.texelSize;
        int count = rects.Count;
        for (int i = 0; i < count; i++)
        {
            CombineTileToMesh(rects[i], posList[i], offsetList[i]);
        }

        MeshFilter m_meshFilter = sceneMeshObj.GetComponent<MeshFilter>();
        m_meshFilter.sharedMesh = new Mesh();
        m_meshFilter.sharedMesh.vertices = m_vertices.ToArray();
        m_meshFilter.sharedMesh.uv = m_uv.ToArray();
        m_meshFilter.sharedMesh.triangles = m_triangles.ToArray();
    }
#endif

    public void CombineMesh()
    {
        ResourceManager.Instance.LoadAsset("resourceassets/map.assetbundle", map =>
        {
            sceneMeshObj = GameObject.Instantiate((GameObject)map.LoadAsset("World" + (int)worldResource + ".prefab"));
            sceneMeshObj.name = "SceneMeshCombine";
            MeshRenderer m_meshRenderer = sceneMeshObj.GetComponent<MeshRenderer>();
            m_meshRenderer.sortingLayerName = "TerrainDown";
            m_meshRenderer.sortingOrder = 0;

            texelSize = m_meshRenderer.sharedMaterial.mainTexture.texelSize;
            int count = rects.Count;
            for (int i = 0; i < count; i++)
            {
                CombineTileToMesh(rects[i], posList[i], offsetList[i]);
            }

            MeshFilter m_meshFilter = sceneMeshObj.GetComponent<MeshFilter>();
            m_meshFilter.sharedMesh = new Mesh();
            m_meshFilter.sharedMesh.vertices = m_vertices.ToArray();
            m_meshFilter.sharedMesh.uv = m_uv.ToArray();
            m_meshFilter.sharedMesh.triangles = m_triangles.ToArray();
        });
    }

    private void CombineTileToMesh(Rect rect, Vector2 pos, Vector2 offset)
    {
        float px0 = pos.x - rect.width * 0.005f + offset.x;
        float py0 = pos.y - rect.width * 0.005f + offset.y;
        float px1 = px0 + rect.width * 0.01f;
        float py1 = py0 + rect.height * 0.01f;

        int vertexIdx = m_vertices.Count;

        m_vertices.Add(new Vector3(px0, py0, 0));
        m_vertices.Add(new Vector3(px1, py0, 0));
        m_vertices.Add(new Vector3(px0, py1, 0));
        m_vertices.Add(new Vector3(px1, py1, 0));

        m_triangles.Add(vertexIdx + 3);
        m_triangles.Add(vertexIdx + 0);
        m_triangles.Add(vertexIdx + 2);
        m_triangles.Add(vertexIdx + 0);
        m_triangles.Add(vertexIdx + 3);
        m_triangles.Add(vertexIdx + 1);

        float u0 = rect.xMin * texelSize.x;
        float v0 = rect.yMin * texelSize.y;
        float u1 = rect.xMax * texelSize.x;
        float v1 = rect.yMax * texelSize.y;

        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(u0, v0);
        uvs[1] = new Vector2(u1, v0);
        uvs[2] = new Vector2(u0, v1);
        uvs[3] = new Vector2(u1, v1);

        for (int i = 0; i < 4; ++i)
        {
            m_uv.Add(uvs[i]);
        }
    }
}

[System.Serializable]
public class MapLayerItem
{
    public List<int> posList = new List<int>();
    public List<MapResourceItem> items = new List<MapResourceItem>();
}

[System.Serializable]
public class AppointAltar
{
    public Vector2 pos;
    public List<int> groups = new List<int>();
}

[System.Serializable]
public class AppointMonster
{
    public Vector2 pos;
    public List<int> groups = new List<int>();
}

public struct Collider2DEdgeConnection
{
    public int index;
    public bool toP1;
    public Collider2DEdgeConnection(int index, bool toP1)
    {
        this.index = index;
        this.toP1 = toP1;
    }
}

public struct Collider2DEdge
{
    public Vector2 p1;
    public Vector2 p2;
    public List<Collider2DEdgeConnection> p1Connections;
    public List<Collider2DEdgeConnection> p2Connections;
    public bool processed;
    public Collider2DEdge(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p1Connections = new List<Collider2DEdgeConnection>();
        this.p2Connections = new List<Collider2DEdgeConnection>();
        this.processed = false;
    }
}