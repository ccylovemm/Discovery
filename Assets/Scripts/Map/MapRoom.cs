using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom
{
    public int offsetX;
    public int offsetY;

    public MapScene mapInfo;
    public MapLevelVo currMapLevelVo;

    public void CreateScene(MapSceneAsset mapPrefabAsset, Dictionary<int, MapResourceItem> mapGird = null)
    {
        mapInfo = new GameObject("Terrain").AddComponent<MapScene>();
        mapInfo.offsetX = offsetX;
        mapInfo.offsetY = offsetY;
        mapInfo.Clone(mapPrefabAsset);
        mapInfo.CreateScene(mapGird);
        mapInfo.UpdateCollider();
        mapInfo.CreateMonster();

        int index = mapInfo.layers.IndexOf(MapEditorSortLayer.Floor1);
        int count = 0;
        if (index != -1)
        {
            MapLayerItem mapLayerItem = mapInfo.layerItems[index];
            count = mapLayerItem.items.Count;
            for (int i = 0; i < count; i++)
            {
                if (mapLayerItem.items[i].itemType == MapEditorItemType.Hole || mapLayerItem.items[i].itemType == MapEditorItemType.DeepWater)
                {
                    int pos = mapLayerItem.posList[i];
                    MapManager.SetPathHoleData(new Vector2(offsetX + pos / 1000, offsetY + pos % 1000));
                }
            }
        }

        count = mapInfo.walkableList.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 grid = mapInfo.walkableList[i] + new Vector2(offsetX, offsetY);
            MapManager.SetPathData(grid);
            MapManager.SetPathHoleData(grid);
        }
        if ((MapRoomType)currMapLevelVo.Type == MapRoomType.Birth || (MapRoomType)currMapLevelVo.Type == MapRoomType.BossLevel)
        {
            SceneManager.Instance.CreateActor(GameData.myData, ((mapInfo.randonBirthPos.Count > 0 ? mapInfo.randonBirthPos[Random.Range(0, mapInfo.randonBirthPos.Count)] : mapInfo.walkableList[Random.Range(0, mapInfo.walkableList.Count)]) + new Vector2(offsetX, offsetY)) * MapManager.textSize);
        }
        mapInfo.CreateAppointAltar();
    }
}
