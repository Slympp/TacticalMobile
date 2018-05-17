using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedWorldMapData {

    public List<UnitSerializable>       Player;
    public float                        Seed;
    public int                          Size;
    public WorldMapCellType[,]          WorldMapInfos;
    public bool[,]                      DiscoveredMap;
    public int                          CurrentPosX;
    public int                          CurrentPosY;

    public List<string>                 InventoryItemId;
    public List<int>                    InventoryItemQuantity;
}
