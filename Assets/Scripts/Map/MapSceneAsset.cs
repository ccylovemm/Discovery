using UnityEngine;
using System.Collections.Generic;

public class MapSceneAsset : ScriptableObject
{
    public int mapSizeX;
    public int mapSizeY;
    public MapWorldResource worldResource;
    public List<Vector2> walkableList = new List<Vector2>();
    public List<MapLayerItem> layerItems = new List<MapLayerItem>();
    public List<MapEditorSortLayer> layers = new List<MapEditorSortLayer>();
    public List<Vector2> randonBirthPos = new List<Vector2>();
    public List<Vector2> randonChestPos = new List<Vector2>();
    public List<Vector2> randonAltarPos = new List<Vector2>();
    public List<Vector2> randonMonsterPos = new List<Vector2>();
    public List<int> randomMonsterGroups = new List<int>();
    public List<AppointAltar> appointAltars = new List<AppointAltar>();
    public List<AppointMonster> appointMonsters = new List<AppointMonster>();

    public void Clone(MapScene mapScene)
    {
        mapSizeX = mapScene.mapSizeX;
        mapSizeY = mapScene.mapSizeY;
        worldResource = mapScene.worldResource;
        walkableList.Clear();
        for (int i = 0; i < mapScene.walkableList.Count; i++)
        {
            walkableList.Add(mapScene.walkableList[i]);
        }
        layerItems.Clear();
        for (int i = 0; i < mapScene.layerItems.Count; i++)
        {
            layerItems.Add(new MapLayerItem());
            for (int j = 0; j < mapScene.layerItems[i].posList.Count; j++)
            {
                layerItems[i].items.Add(mapScene.layerItems[i].items[j]);
                layerItems[i].posList.Add(mapScene.layerItems[i].posList[j]);
            }
        }
        layers.Clear();
        for (int i = 0; i < mapScene.layers.Count; i ++)
        {
            layers.Add(mapScene.layers[i]);
        }
        randonBirthPos.Clear();
        for (int i = 0; i < mapScene.randonBirthPos.Count; i++)
        {
            randonBirthPos.Add(mapScene.randonBirthPos[i]);
        }
        randonAltarPos.Clear();
        for (int i = 0; i < mapScene.randonAltarPos.Count; i++)
        {
            randonAltarPos.Add(mapScene.randonAltarPos[i]);
        }
        randonChestPos.Clear();
        for (int i = 0; i < mapScene.randonChestPos.Count; i++)
        {
            randonChestPos.Add(mapScene.randonChestPos[i]);
        }
        randonMonsterPos.Clear();
        for (int i = 0; i < mapScene.randonMonsterPos.Count; i++)
        {
            randonMonsterPos.Add(mapScene.randonMonsterPos[i]);
        }
        appointAltars.Clear();
        for (int i = 0; i < mapScene.appointAltars.Count; i++)
        {
            appointAltars.Add(new AppointAltar());
            appointAltars[i].pos = mapScene.appointAltars[i].pos;
            for (int j = 0; j < mapScene.appointAltars[i].groups.Count; j++)
            {
                appointAltars[i].groups.Add(mapScene.appointAltars[i].groups[j]);
            }
        }

        appointMonsters.Clear();
        for (int i = 0; i < mapScene.appointMonsters.Count; i++)
        {
            appointMonsters.Add(new AppointMonster());
            appointMonsters[i].pos = mapScene.appointMonsters[i].pos;
            for (int j = 0; j < mapScene.appointMonsters[i].groups.Count; j++)
            {
                appointMonsters[i].groups.Add(mapScene.appointMonsters[i].groups[j]);
            }
        }
    }
}
