using UnityEngine;
using System.Collections.Generic;

public class MapSceneAsset : ScriptableObject
{
    public int offsetX;
    public int offsetY;
    public int mapSizeX;
    public int mapSizeY;
    public MapWorldResource worldResource;
    public List<MapLayerItem> layerItems = new List<MapLayerItem>();
    public List<MapEditorSortLayer> layers = new List<MapEditorSortLayer>();
}
