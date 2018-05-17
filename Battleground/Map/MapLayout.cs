using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapLayout {

    public float    SizeX { get; private set; }
    public float    SizeY { get; private set; }
    public int[,]   Layout { get; private set; }

    public MapLayout(MapInfos mapInfos) {

        SizeX = (mapInfos.Size.x <= 0 ? 1 : mapInfos.Size.x);
        SizeY = (mapInfos.Size.y <= 0 ? 1 : mapInfos.Size.y);
        Layout = new int[(int)SizeX, (int)SizeY];

        do { FillLayout(mapInfos); } while (!mapIsPlayable(mapInfos));
    }

    private void FillLayout(MapInfos mapInfos) {

        float randomNoise = Random.Range(0, 10000);

        float maxRay = Mathf.Sqrt(Mathf.Pow((0 - SizeX / 2), 2) + Mathf.Pow((0 - SizeY / 2), 2)) * 0.85f;
        float minRay = maxRay * 0.9f;

        for (int x = 0; x < SizeX; x++) {

            double ray = Random.Range(minRay, maxRay);
            for (int y = 0; y < SizeY; y++) {

                if (GetDistanceFromCenter(x, y) < ray) {

                    Vector2 pos = mapInfos.MapData.Zoom * (new Vector2(x, y));
                    float noise = Mathf.PerlinNoise(pos.x + randomNoise, pos.y + randomNoise);

                    if (noise < mapInfos.MapData.BlocHeight[0]) Layout[x, y] = (int)BlocHeight.NonWalkable;
                    else if (noise < mapInfos.MapData.BlocHeight[1]) Layout[x, y] = (int)BlocHeight.SeaLevel;
                    else if (noise < mapInfos.MapData.BlocHeight[2]) Layout[x, y] = (int)BlocHeight.Ground;
                    else if (noise < mapInfos.MapData.BlocHeight[3]) Layout[x, y] = (int)BlocHeight.Middle;
                    else Layout[x, y] = (int)BlocHeight.Top;
                } else
                    Layout[x, y] = (int)BlocHeight.Empty;
                
            }
        }
    }

    // Check if layout is good enough to be played
    private bool mapIsPlayable(MapInfos mapInfos) {

        // Check % of non-walkable cells
        if (mapInfos.MapData != null && mapInfos.MapData.NonWalkableCheck) {
            float validCells = 0;
            float nonWalkableCells = 0;

            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++) {
                    if (Layout[i, j] != -1) validCells++;
                    if (Layout[i, j] == 0) nonWalkableCells++;
                }

            float percentNonWalkables = nonWalkableCells / validCells;
            if (percentNonWalkables > mapInfos.MapData.MaxNonWalkable || percentNonWalkables < mapInfos.MapData.MinNonWalkable) return false;
        }

        return true;
    }

    private double GetDistanceFromCenter(int x, int y) {

        return (Mathf.Sqrt(Mathf.Pow((x - SizeX / 2), 2) + Mathf.Pow((y - SizeY / 2), 2)));
    }
}
