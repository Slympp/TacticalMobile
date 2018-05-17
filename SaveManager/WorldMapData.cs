using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapData {

    public List<Player>         Player;
    public float                Seed;
    public int                  Size;
    public WorldMapCellType[,]  WorldMapInfos;
    public bool[,]              DiscoveredMap;
    public Vector2              CurrentPos;
    public List<Item>           Inventory;
}
