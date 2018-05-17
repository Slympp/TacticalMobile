using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapBuilder : MonoBehaviour {

    public MapDataType[,]   WorldMap;

    public Material         hellMaterial;
    public Material         desertMaterial;
    public Material         plainMaterial;
    public Material         snowMaterial;
    public Material         cityMaterial;
    public Material         unknownMaterial;

    public Sprite           Cursor;
    public Sprite           City;
    public Sprite           Unknown;

    private float           Zoom = 0.13f;
    public GameObject      MapObject;

    public void Generate(WorldMapData worldMapData) {

        if (worldMapData.Seed == 0) {
            worldMapData.Seed = Random.Range(1, 10000);
        }

        WorldMap = new MapDataType[worldMapData.Size, worldMapData.Size];

        for (int x = 0; x < worldMapData.Size; x++) {
            for (int y = 0; y < worldMapData.Size; y++) {

                Vector2 pos = Zoom * (new Vector2(x, y));
                float noise = Mathf.PerlinNoise(pos.x + worldMapData.Seed, pos.y + worldMapData.Seed);

                if (noise < 0.25f) WorldMap[x, y] = MapDataType.Hell;
                else if (noise < 0.5f) WorldMap[x, y] = MapDataType.Desert;
                else if (noise < 0.75f) WorldMap[x, y] = MapDataType.Plain;
                else WorldMap[x, y] = MapDataType.Snow;
            }
        }

        Populate(worldMapData);
        Build(worldMapData);
    }

    private void Populate(WorldMapData worldMapData) {

        if (worldMapData.WorldMapInfos == null) {
            worldMapData.WorldMapInfos = new WorldMapCellType[worldMapData.Size, worldMapData.Size];

            worldMapData.DiscoveredMap = new bool[worldMapData.Size, worldMapData.Size];

            for (int x = 0; x < worldMapData.Size; x++) {
                for (int y = 0; y < worldMapData.Size; y++) {

                    int cellType = Random.Range(0, 100);
                    if (cellType < 5) worldMapData.WorldMapInfos[x, y] = WorldMapCellType.City;
                    else if (cellType < 15) worldMapData.WorldMapInfos[x, y] = WorldMapCellType.Chest;
                    else if (cellType < 25) worldMapData.WorldMapInfos[x, y] = WorldMapCellType.NewTeamMember;
                    else worldMapData.WorldMapInfos[x, y] = WorldMapCellType.Battleground;

                    worldMapData.DiscoveredMap[x, y] = false;
                }
            }

            Vector2 initialPos = new Vector2();
            initialPos.x = Random.Range(0, worldMapData.Size);
            initialPos.y = Random.Range(0, worldMapData.Size);
            worldMapData.WorldMapInfos[(int)initialPos.x, (int)initialPos.y] = WorldMapCellType.City;
            worldMapData.CurrentPos = initialPos;
            UpdateDiscoveredMap(worldMapData, (int)initialPos.x, (int)initialPos.y);
        }
    }

    public void UpdateDiscoveredMap(WorldMapData worldMapData, int x, int y) {

        worldMapData.DiscoveredMap[x, y] = true;

        if (x - 1 >= 0)
            worldMapData.DiscoveredMap[x - 1, y] = true;
        if (x + 1 < worldMapData.Size)
            worldMapData.DiscoveredMap[x + 1, y] = true;
        if (y - 1 >= 0)
            worldMapData.DiscoveredMap[x, y - 1] = true;
        if (y + 1 < worldMapData.Size)
            worldMapData.DiscoveredMap[x, y + 1] = true;

    }

    private void Build(WorldMapData worldMapData) {

        if (MapObject != null)
            Destroy(MapObject);

        MapObject = new GameObject("WorldMap");

        for (int x = 0; x < worldMapData.Size; x++) {
            for (int y = 0; y < worldMapData.Size; y++) {

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

                plane.transform.parent = MapObject.transform;
                plane.transform.position = new Vector3(x * 10, 0, y * 10);

                MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
                if (worldMapData.DiscoveredMap[x, y]) {

                    AddSprite(x, y, plane.transform, worldMapData.CurrentPos, worldMapData.WorldMapInfos[x, y]);

                    if (worldMapData.WorldMapInfos[x, y] == WorldMapCellType.City) {
                        planeRenderer.material = cityMaterial;
                    } else {

                        if (WorldMap[x, y] == MapDataType.Desert)
                            planeRenderer.material = desertMaterial;
                        else if (WorldMap[x, y] == MapDataType.Hell)
                            planeRenderer.material = hellMaterial;
                        else if (WorldMap[x, y] == MapDataType.Plain)
                            planeRenderer.material = plainMaterial;
                        else if (WorldMap[x, y] == MapDataType.Snow)
                            planeRenderer.material = snowMaterial;
                    }
                } else
                    planeRenderer.material = unknownMaterial;
            }
        }
    }

    private void AddSprite(int x, int y, Transform transform, Vector2 currentPos, WorldMapCellType cellType) {

        if (x == currentPos.x && y == currentPos.y)
            BuildSprite(transform, Cursor, "cursor");
        else if (cellType == WorldMapCellType.City)
            BuildSprite(transform, City, "city");
        else if (cellType != WorldMapCellType.Empty)
            BuildSprite(transform, Unknown, "unknown");
    }

    private void BuildSprite(Transform parent, Sprite sprite, string name) {

        GameObject go = new GameObject(name);
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.eulerAngles = new Vector3(90, 0, 0);
        go.transform.localScale = new Vector3(7, 7, 1);
        go.AddComponent<SpriteRenderer>().sprite = sprite;
    }
}

public enum WorldMapCellType {
    City,
    Battleground,
    Chest,
    NewTeamMember,
    Empty
}

public enum MapDataType {
    Desert,
    Plain,
    Hell,
    Snow,
}