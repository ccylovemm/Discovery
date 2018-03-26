using UnityEngine;
using System.Collections.Generic;

public class MapWorldAsset : ScriptableObject
{
    public List<MapSceneAsset> mapScenes = new List<MapSceneAsset>();
}

public class MapSceneAsset
{
    public int offsetX;
    public int offsetY;
    public int mapSizeX;
    public int mapSizeY;
    public WorldType worldType;
    public List<MapLayerItem> layerItems = new List<MapLayerItem>();
    public List<MapEditorSortLayer> layers = new List<MapEditorSortLayer>();
}
