using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MapAssetScene : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    public Vector2 brithPos;
    public Vector2 transPos;
    public List<int> mapdata = new List<int>();

    private List<Rect> rects = new List<Rect>();
    private List<int> m_triangles = new List<int>();
    private List<Vector2> m_uv = new List<Vector2>();
    private List<Vector2> posList = new List<Vector2>();
    private List<Vector3> m_vertices = new List<Vector3>();
    private List<Vector2> offsetList = new List<Vector2>();

    private GameObject sceneMeshObj;
    private Vector2 texelSize;
    private MapResourceItem mapResourceItem;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, .2f);

        for (int i = 0; i <= sizeX; i++)
        {
            Gizmos.DrawLine(new Vector3(i * MapManager.textSize, 0, 0), new Vector3(i * MapManager.textSize, sizeY * MapManager.textSize, 0));
        }

        for (int i = 0; i <= sizeY; i++)
        {
            Gizmos.DrawLine(new Vector3(0, i * MapManager.textSize, 0), new Vector3(sizeX * MapManager.textSize, i * MapManager.textSize, 0));
        }

        Gizmos.color = new Color(0f, 1.0f, 0f, 0.5f);
        if (brithPos != Vector2.zero)
        {
            Gizmos.DrawCube((brithPos - Vector2.one * 0.5f) * MapManager.textSize, new Vector3(1 * MapManager.textSize, 1 * MapManager.textSize, 0));
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        if (transPos != Vector2.zero)
        {
            Gizmos.DrawCube((transPos - Vector2.one * 0.5f) * MapManager.textSize, new Vector3(1 * MapManager.textSize, 1 * MapManager.textSize, 0));
        }
    }

    public void Clear()
    {
        rects.Clear();
        m_triangles.Clear();
        m_uv.Clear();
        posList.Clear();
        m_vertices.Clear();
        offsetList.Clear();
        for (int i = 0; i < mapdata.Count; i ++)
        {
            mapdata[i] = 0;
        }
        brithPos = transPos = Vector2.zero;
        GameObject.DestroyImmediate(sceneMeshObj);
    }

#if UNITY_EDITOR
    public void UpdateMesh()
    {
        if (mapResourceItem == null)
        {
            mapResourceItem = (MapResourceItem)AssetDatabase.LoadAssetAtPath("Assets/Editor/MapStyle/MapEditorResource.prefab", typeof(MapResourceItem));
        }

        rects.Clear();
        m_triangles.Clear();
        m_uv.Clear();
        posList.Clear();
        m_vertices.Clear();
        offsetList.Clear();

        for(int i = 0; i < mapdata.Count; i ++)
        {
            int x = i % sizeX;
            int y = i / sizeX;
            if (mapdata[i] == 0 || mapdata[i] - 1 >= mapResourceItem.normalList.Count) continue;
            MapSprite mapSprite = mapResourceItem.normalList[mapdata[i] - 1];
            rects.Add(mapSprite.sprite.rect);
            posList.Add(new Vector2(x * MapManager.textSize + MapManager.textSize / 2.0f, y * MapManager.textSize + MapManager.textSize / 2.0f));
            offsetList.Add(Vector2.zero);
        }
        CombineMeshFromEditor();
    }

    private void CombineMeshFromEditor()
    {
        sceneMeshObj = GameObject.Find("MapMeshCombine");
        if (sceneMeshObj == null)
        {
            sceneMeshObj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/MapStyle/MapMeshCombine.prefab"));
            sceneMeshObj.name = "MapMeshCombine";
        }

        MeshRenderer m_meshRenderer = sceneMeshObj.GetComponent<MeshRenderer>();
        m_meshRenderer.sortingLayerName = "TerrainDown";
        m_meshRenderer.sortingOrder = 0;

        texelSize = m_meshRenderer.sharedMaterial.mainTexture.texelSize;
        for (int i = 0; i < rects.Count; i++)
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