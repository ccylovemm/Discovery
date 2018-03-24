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
    public int offsetX;
    public int offsetY;

    public int mapSizeX;
    public int mapSizeY;

    public MapWorldResource worldResource;

    public List<MapLayerItem> layerItems = new List<MapLayerItem>();
    public List<MapEditorSortLayer> layers = new List<MapEditorSortLayer>();

    public GameObject sceneMeshObj;
    public List<GameObject> colliderObjs = new List<GameObject>();
    public List<GameObject> generatePrefabItems = new List<GameObject>();

    private List<Rect> rects = new List<Rect>();
    private List<int> m_triangles = new List<int>();
    private List<Vector2> m_uv = new List<Vector2>();
    private List<Vector2> posList = new List<Vector2>();
    private List<Vector3> m_vertices = new List<Vector3>();
    private List<Vector2> offsetList = new List<Vector2>();

    private Vector2 texelSize;

    private Dictionary<int, MapResourceItem> mapGird = new Dictionary<int, MapResourceItem>();

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
    }
#endif

    public void FreshMapGrid()
    {
        mapGird.Clear();
        int index = layers.IndexOf(MapEditorSortLayer.Floor1);
        if(index != -1)
        {
            MapLayerItem mapLayerItem = layerItems[index];
            for(int i = 0; i < mapLayerItem.items.Count; i ++)
            {
                mapGird.Add(mapLayerItem.posList[i] , mapLayerItem.items[i]);
            }
        }
    }

    public void AddGrid(int pos, MapResourceItem selectedTile)
    {
        int index = layers.IndexOf(selectedTile.layer);
        if (index != -1)
        {
            int i = layerItems[index].posList.IndexOf(pos);
            if (i != -1)
            {
                layerItems[index].items[i] = selectedTile;
            }
            else
            {
                layerItems[index].posList.Add(pos);
                layerItems[index].items.Add(selectedTile);
            }
        }
        else
        {
            layers.Add(selectedTile.layer);
            layerItems.Add(new MapLayerItem());
            layerItems[layerItems.Count - 1].posList.Add(pos);
            layerItems[layerItems.Count - 1].items.Add(selectedTile);
        }
        if(selectedTile.layer == MapEditorSortLayer.Floor1)
        {
            if (mapGird.ContainsKey(pos))
            {
                mapGird[pos] = selectedTile;
            }
            else
            {
                mapGird.Add(pos , selectedTile);
            }
        }
    }

    public void ClearGrid(Vector2 grid)
    {
        for (int i = 3; i > -1; i--)
        {
            int index = layers.IndexOf((MapEditorSortLayer)i);
            if (index != -1)
            {
                int n = layerItems[index].posList.IndexOf((int)grid.x * 10000 + (int)grid.y);
                if (n != -1)
                {
                    MapResourceItem item = layerItems[index].items[n];
                    layerItems[index].posList.RemoveAt(n);
                    layerItems[index].items.RemoveAt(n);
                    UpdateMap();
                    break;
                }
            }
        }
    }

    public void UpdateMap()
    {
        rects.Clear();
        m_triangles.Clear();
        m_uv.Clear();
        posList.Clear();
        m_vertices.Clear();
        offsetList.Clear();

        int count = generatePrefabItems.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.DestroyImmediate(generatePrefabItems[i]);
        }
        generatePrefabItems.Clear();

        for (int i = 0; i < 3; i++)
        {
            int index = layers.IndexOf((MapEditorSortLayer)i);
            if (index != -1)
            {
                MapLayerItem mapLayerItem = layerItems[index];
                count = mapLayerItem.items.Count;
                for (int j = 0; j < count; j++)
                {
                    int x = mapLayerItem.posList[j] / 10000;
                    int y = mapLayerItem.posList[j] % 10000;
                    if (mapLayerItem.items[j].isPrefab)
                    {
                        GameObject go = GameObject.Instantiate(mapLayerItem.items[j].gameObject);
                        go.transform.position = MapManager.GetPos(x, y);
                        generatePrefabItems.Add(go);
                    }
                    else
                    {
                        List<MapSprite> list = new List<MapSprite>();
                        if (mapLayerItem.items[j].isNine)
                        {
                            list = GetSpriteList(x, y, mapGird);
                        }
                        else
                        {
                            list = mapLayerItem.items[j].normalList;
                        }
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

    public void Clear()
    {
        m_uv.Clear();
        rects.Clear();
        posList.Clear();
        offsetList.Clear();
        m_vertices.Clear();
        m_triangles.Clear();

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

        GameObject.DestroyImmediate(sceneMeshObj);
        GameObject.DestroyImmediate(gameObject);
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
                    int x = list[0] / 10000;
                    int y = list[0] % 10000;
                    int pos1 = (x - 1) * 10000 + y;
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = (x + 1) * 10000 + y;
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = x * 10000 + (y - 1);
                    if (allPos.IndexOf(pos1) != -1)
                    {
                        list.Add(pos1);
                        allPos.Remove(pos1);
                    }
                    pos1 = x * 10000 + (y + 1);
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
                    int x = posList[i][j] / 10000;
                    int y = posList[i][j] % 10000;

                    count = vertices.Count / 4;
                    vertices.AddRange(new Vector3[4]);
                    vertices[count * 4] = new Vector3(x, y);
                    vertices[count * 4 + 1] = new Vector3(x + 1, y);
                    vertices[count * 4 + 2] = new Vector3(x + 1, y + 1);
                    vertices[count * 4 + 3] = new Vector3(x, y + 1);

                    if (posList[i].IndexOf(x * 10000 + (y + 1)) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 2], vertices[count * 4 + 3]);
                    }

                    if (posList[i].IndexOf(x * 10000 + (y - 1)) == -1)
                    {
                        AddEdge(edges, vertices[count * 4], vertices[count * 4 + 1]);
                    }

                    if (posList[i].IndexOf((x - 1) * 10000 + y) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 3], vertices[count * 4]);
                    }

                    if (posList[i].IndexOf((x + 1) * 10000 + y) == -1)
                    {
                        AddEdge(edges, vertices[count * 4 + 1], vertices[count * 4 + 2]);
                    }
                }

                GameObject go = new GameObject();
                PolygonCollider2D polygonCollider2D = go.AddComponent<PolygonCollider2D>();

                go.layer = LayerUtil.LayerToObstacle();

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

    private void AddEdge(List<Collider2DEdge> edges, Vector2 p1, Vector2 p2)
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

    private bool Approximately(Vector2 v1, Vector2 v2)
    {
        float tolerance = 0.05f;
        return (v1.x > v2.x - tolerance && v1.x < v2.x + tolerance && v1.y > v2.y - tolerance && v1.y < v2.y + tolerance);
    }

    private void PolygoniseEdges(List<Vector2[]> polygons, List<Collider2DEdge> edges)
    {
        if (edges.Count == 0)
            return;
        List<Vector2> currentPolygon = new List<Vector2>();
        currentPolygon.Add((edges[0].p1 + new Vector2(offsetX, offsetY)) * MapManager.textSize);
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

    private void RemoveEdgeConnections(List<Collider2DEdge> edges, int index)
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

    private List<MapSprite> GetSpriteList(int i, int j, Dictionary<int, MapResourceItem> girdInfo)
    {
        MapResourceItem item = girdInfo[i * 10000 + j];

        bool grid1 = girdInfo.ContainsKey((i - 1) * 10000 + j);
        bool grid2 = girdInfo.ContainsKey((i + 1) * 10000 + j);
        bool grid3 = girdInfo.ContainsKey(i * 10000 + (j - 1));
        bool grid4 = girdInfo.ContainsKey(i * 10000 + (j + 1));
        bool grid5 = girdInfo.ContainsKey((i - 1) * 10000 + (j + 1));
        bool grid6 = girdInfo.ContainsKey((i + 1) * 10000 + (j + 1));
        bool grid7 = girdInfo.ContainsKey((i - 1) * 10000 + (j - 1));
        bool grid8 = girdInfo.ContainsKey((i + 1) * 10000 + (j - 1));

        bool wall1 = item.itemType == MapEditorItemType.Wall && !grid1;
        bool wall2 = item.itemType == MapEditorItemType.Wall && !grid2;
        bool wall3 = item.itemType == MapEditorItemType.Wall && !grid3;
        bool wall4 = item.itemType == MapEditorItemType.Wall && !grid4;
        bool wall5 = item.itemType == MapEditorItemType.Wall && !grid5;
        bool wall6 = item.itemType == MapEditorItemType.Wall && !grid6;
        bool wall7 = item.itemType == MapEditorItemType.Wall && !grid7;
        bool wall8 = item.itemType == MapEditorItemType.Wall && !grid8;

        bool left = wall1 || grid1 && (girdInfo[(i - 1) * 10000 + j].itemType == item.itemType);
        bool right = wall2 || grid2 && (girdInfo[(i + 1) * 10000 + j].itemType == item.itemType);
        bool down = wall3 || grid3 && (girdInfo[i * 10000 + (j - 1)].itemType == item.itemType);
        bool up = wall4 || grid4 && (girdInfo[i * 10000 + (j + 1)].itemType == item.itemType);
        bool leftUp = wall5 || grid5 && (girdInfo[(i - 1) * 10000 + (j + 1)].itemType == item.itemType);
        bool rightUp = wall6 || grid6 && (girdInfo[(i + 1) * 10000 + (j + 1)].itemType == item.itemType);
        bool leftDown = wall7 || grid7 && (girdInfo[(i - 1) * 10000 + (j - 1)].itemType == item.itemType);
        bool rightDown = wall8 || grid8 && (girdInfo[(i + 1) * 10000 + (j - 1)].itemType == item.itemType);


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
        sceneMeshObj = GameObject.Find("MapTerrain" + name.Replace("Map" , ""));
        if (sceneMeshObj == null)
        {
            sceneMeshObj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ResourceAssets/Prefabs/Map/World" + (int)worldResource + ".prefab"));
            sceneMeshObj.name = "MapTerrain" + name.Replace("Map", "");
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