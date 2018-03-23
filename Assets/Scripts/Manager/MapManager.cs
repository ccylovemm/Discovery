using UnityEngine;
using System.Collections.Generic;

public class MapManager
{
    static public int mapSizeX;
    static public int mapSizeY;
    static public float textSize = 0.20f;
    static public Dictionary<Vector2, AStarNode> mapPathData = new Dictionary<Vector2, AStarNode>();
    static public Dictionary<Vector2, AStarNode> mapPathData_Hole = new Dictionary<Vector2, AStarNode>();
    static public MapSceneItemType[,] mapSceneData;

    static public void ClearPath()
    {
        mapPathData.Clear();
        mapPathData_Hole.Clear();
    }

    static public void SetPathData(Vector2 grid , bool path = true)
    {
        if (!mapPathData.ContainsKey(grid))
        {
            if (path)
            {
                mapPathData.Add(grid, new AStarNode(grid));
            }
        }
        else
        {
            if(!path)
            {
                mapPathData.Remove(grid);
            }
        }
    }

    static public void SetPathHoleData(Vector2 grid , bool path = true)
    {
        if (!mapPathData_Hole.ContainsKey(grid))
        {
            if (path)
            {
                mapPathData_Hole.Add(grid, new AStarNode(grid));
            }
        }
        else
        {
            if (!path)
            {
                mapPathData_Hole.Remove(grid);
            }
        }
    }

    static public Vector2 FindEscapePos(Vector2 originPos, Vector2 targetPos, float distance, float range , int tempCount = 100)
    {
        if (distance <= Vector3.Distance(originPos, targetPos))
        {
            return originPos;
        }
        float x = 0;
        float y = 0;
        Vector2 direct = originPos - targetPos;
        Vector2 srcPos1 = originPos + direct.normalized * (distance - direct.magnitude);
        Vector2 srcPos2 = srcPos1 + direct.normalized * range;
        float minX = Mathf.Min(srcPos1.x, srcPos2.x);
        float maxX = Mathf.Max(srcPos1.x, srcPos2.x);
        float minY = Mathf.Min(srcPos1.y, srcPos2.y);
        float maxY = Mathf.Max(srcPos1.y, srcPos2.y);
        Vector2 girdPos = Vector2.zero;
        do
        {
            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);
            girdPos = GetGrid(x , y);
            tempCount--;
        } while (!mapPathData.ContainsKey(girdPos) && tempCount != 0);
        if (tempCount == 0) return originPos;
        return new Vector2(x, y);
    }

    static public Vector2 FindPosByRange(Vector2 originPos, float maxRange, float minRange, int tempCount = 100)
    {
        float x = 0;
        float y = 0;
        float minX = originPos.x - maxRange;
        float maxX = originPos.x + maxRange;
        float minY = originPos.y - maxRange;
        float maxY = originPos.y + maxRange;

        Vector2 girdPos = Vector2.zero;
        do
        {
            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);
            girdPos = GetGrid(x, y);
            if (mapPathData.ContainsKey(girdPos))
            {
                break;
            }
            tempCount--;
        } while (tempCount != 0);
        if (tempCount == 0)
        {
            return Vector2.zero;
        }
        return new Vector2(x, y);
    }

    static public Vector2 FindGrid(int sizeX , int sizeY , int tempCount = 100)
    {
        int x, y = 0;
        do
        {
            x = Random.Range(1 , mapSizeX - 1);
            y = Random.Range(1 , mapSizeY - 1);
            bool match = true;
            for(int i = x; i < x + sizeX; i ++)
            {
                for(int j = y; j < y + sizeY; j ++)
                {
                    if(mapPathData.ContainsKey(new Vector2(x , y)) || mapSceneData[i, j] != MapSceneItemType.None)
                    {
                        match = false;
                        break;
                    }
                }
                if (!match) break;
            }
            if (match)
            {
                break;
            }
            tempCount--;
        } while (tempCount != 0);
        return new Vector2(x, y);
    }

    static public Vector2 FindGridByRange(Vector2 originPos, int maxRange, int minRange, int tempCount = 100)
    {
        int x = 0;
        int y = 0;
        Vector2 gridPos = GetGrid(originPos.x , originPos.y);
        int minX = (int)gridPos.x - maxRange;
        int maxX = (int)gridPos.x + maxRange;
        int minY = (int)gridPos.y - maxRange;
        int maxY = (int)gridPos.y + maxRange;
        do
        {
            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);
            if (mapPathData.ContainsKey(new Vector2(x , y)))
            {
                break;
            }
            tempCount--;
        } while (tempCount != 0);
        if (tempCount == 0)
        {
            EventCenter.DispatchEvent(EventEnum.ShowMsg , "Cannot Find Pos   FindGridByRange");
        }
        return new Vector2(x, y);
    }

    static public Vector3 FindDropPos(Vector3 originPos)
    {
        Vector3 pos = originPos;
        Vector2 gridPos = Vector2.zero;
        do
        {
            float x = Random.Range(-2.0f , 2.0f);
            float y = Random.Range(-2.0f , 2.0f);
            pos = new Vector3(x , y, 0.0f) * 0.1f;
            gridPos = GetGrid((pos + originPos).x , (pos + originPos).y);
        }
        while (pos.magnitude > 0.35f || pos.magnitude < 0.1f || !mapPathData.ContainsKey(gridPos));
        return pos + originPos;
    }

    static public Vector2 GetGrid(Vector2 pos)
    {
        return GetGrid(pos.x , pos.y);
    }

    static public Vector2 GetGrid(float x , float y)
    {
        int girdX = (int)(x / textSize);
        int girdY = (int)(y / textSize);
        return new Vector2(girdX , girdY);
    }

    static public Vector2 GetPos(Vector2 grid)
    {
        return new Vector2((grid.x + 0.5f) * textSize, (grid.y + 0.5f) * textSize);
    }

    static public Vector2 GetPos(int x, int y)
    {
        return new Vector2((x + 0.5f) * textSize, (y + 0.5f) * textSize);
    }

    static public List<Vector3> FindPath(Vector2 startPos , Vector2 endPos , bool isfly = false)
    {
        if (isfly)
        {
            if (!mapPathData_Hole.ContainsKey(startPos) || !mapPathData_Hole.ContainsKey(endPos)) return null;
        }
        else
        {
            if (!mapPathData.ContainsKey(startPos) || !mapPathData.ContainsKey(endPos)) return null;
        }
     
        List<AStarNode> openPath = new List<AStarNode>();
        List<AStarNode> ErrorPath = new List<AStarNode>();
        AStarNode startNode = isfly ? mapPathData_Hole[startPos] : mapPathData[startPos];
        AStarNode endNode = isfly ? mapPathData_Hole[endPos] : mapPathData[endPos];
        List<AStarNode> openNode = new List<AStarNode>();
        List<AStarNode> closeNode = new List<AStarNode>();
        openNode.Add(startNode);
        startNode.parent = null;
        while (openNode.Count > 0)
        {
            AStarNode currentNode = openNode[0];

            for (int i = 0; i < openNode.Count; i++)
            {
                if (openNode[i].fValue <= currentNode.fValue && openNode[i].hValue < currentNode.hValue)
                {
                    currentNode = openNode[i];
                }
            }
            openNode.Remove(currentNode);
            closeNode.Add(currentNode);
            if (currentNode == endNode)
            {
                List<Vector3> path = new List<Vector3>();
                while (currentNode.parent != null)
                {
                    path.Add(currentNode.mapPos);
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                return path;
            }
            openPath.Add(currentNode);

            List<AStarNode> OpenNodeList = GetOpenAroundNode(currentNode, closeNode , isfly ? mapPathData_Hole : mapPathData);
            foreach (var node in OpenNodeList)
            {
                int newCode = currentNode.gValue + GetDistanceToPos(currentNode, node);
                if (!openNode.Contains(node) || node.gValue > newCode)
                {
                    node.gValue = newCode;
                    node.hValue = GetDistanceToPos(node, endNode);
                    node.parent = currentNode;
                    if (!openNode.Contains(node))
                    {
                        openNode.Add(node);
                    }
                }
            }
        }
        return null;
    }

    static private List<AStarNode> GetOpenAroundNode(AStarNode currentNode, List<AStarNode> closeNode , Dictionary<Vector2, AStarNode> pathData)
    {
        List<AStarNode> OpenNodeList = new List<AStarNode>();

        Vector2 pos1 = new Vector2(currentNode.mapGird.x - 1, currentNode.mapGird.y);
        Vector2 pos2 = new Vector2(currentNode.mapGird.x + 1, currentNode.mapGird.y);
        Vector2 pos3 = new Vector2(currentNode.mapGird.x, currentNode.mapGird.y - 1);
        Vector2 pos4 = new Vector2(currentNode.mapGird.x, currentNode.mapGird.y + 1);

        Vector2 pos5 = new Vector2(currentNode.mapGird.x - 1, currentNode.mapGird.y + 1);
        Vector2 pos6 = new Vector2(currentNode.mapGird.x - 1, currentNode.mapGird.y - 1);
        Vector2 pos7 = new Vector2(currentNode.mapGird.x + 1, currentNode.mapGird.y + 1);
        Vector2 pos8 = new Vector2(currentNode.mapGird.x + 1, currentNode.mapGird.y - 1);

        if (pathData.ContainsKey(pos1) && !closeNode.Contains(pathData[pos1]))
        {
            OpenNodeList.Add(pathData[pos1]);
        }

        if (pathData.ContainsKey(pos2) && !closeNode.Contains(pathData[pos2]))
        {
            OpenNodeList.Add(pathData[pos2]);
        }

        if (pathData.ContainsKey(pos3) && !closeNode.Contains(pathData[pos3]))
        {
            OpenNodeList.Add(pathData[pos3]);
        }

        if (pathData.ContainsKey(pos4) && !closeNode.Contains(pathData[pos4]))
        {
            OpenNodeList.Add(pathData[pos4]);
        }

        if (pathData.ContainsKey(pos5) && pathData.ContainsKey(pos1) && pathData.ContainsKey(pos4) && !closeNode.Contains(pathData[pos5]))
        {
            OpenNodeList.Add(pathData[pos5]);
        }

        if (pathData.ContainsKey(pos6) && pathData.ContainsKey(pos1) && pathData.ContainsKey(pos3) && !closeNode.Contains(pathData[pos6]))
        {
            OpenNodeList.Add(pathData[pos6]);
        }

        if (pathData.ContainsKey(pos7) && pathData.ContainsKey(pos2) && pathData.ContainsKey(pos4) && !closeNode.Contains(pathData[pos7]))
        {
            OpenNodeList.Add(pathData[pos7]);
        }

        if (pathData.ContainsKey(pos8) && pathData.ContainsKey(pos2) && pathData.ContainsKey(pos3) && !closeNode.Contains(pathData[pos8]))
        {
            OpenNodeList.Add(pathData[pos8]);
        }

        return OpenNodeList;
    }

    static public int GetDistanceToPos(AStarNode StartNode, AStarNode EndNode)
    {
        int dx = Mathf.Abs((int)StartNode.mapGird.x - (int)EndNode.mapGird.x);
        int dy = Mathf.Abs((int)StartNode.mapGird.y - (int)EndNode.mapGird.y);

        int value = 0;
        if (dx > dy)
        {
            value = 14 * dy + Mathf.Abs(10 * (dx - dy));
        }
        else
        {
            value = 14 * dx + Mathf.Abs(10 * (dy - dx));
        }
        return 14 * (dx > dy ? dy : dx) + Mathf.Abs(10 * (dx - dy));
    }
}