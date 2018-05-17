using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap {

    private static float    Decay = 0.1f;
    private static float    Momentum = 0.3f;
    private static int      mapSizeX;
    private static int      mapSizeY;

    public static float[,] GetInfluenceMap(MapInfos mapInfos, Team teamToSeek) {

        mapSizeX = (int)mapInfos.Size.x;
        mapSizeY = (int)mapInfos.Size.y;
        float[,] influences = new float[mapSizeX, mapSizeY];
        float[,] influencesBuffer = new float[mapSizeX, mapSizeY];

        SetUnits(influences, influencesBuffer, mapInfos.Map, teamToSeek);

        int cpt = 0;
        do {
            UpdateInfluenceMap(influences, influencesBuffer, mapInfos.Map);
            UpdateInfluenceBuffer(influences, influencesBuffer);

            PrintMap(influences);

            Debug.Log("iteration n°" + cpt);
            cpt++;
        } while (!PropagationIsComplete(influences, mapInfos.Map));

        RemoveTakenCells(influences, mapInfos.Map);

        return influences;
    }

    private static void PrintMap(float[,] map) {

        string txt = "";
        for (int x = 0; x < mapSizeX; x++) {
            string line = "";
            for (int y = 0; y < mapSizeY; y++) {
                line += map[x, y].ToString() + " ";
            }
            txt += line + "\n";
        }
        Debug.Log(txt);
    }

    private static void RemoveTakenCells(float[,] influences, Cell[,] map) {

        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
                if (map[x, y].IsTaken != null)
                    influences[x, y] = -1;
    }

    private static bool PropagationIsComplete(float[,] influences, Cell[,] map) {

        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
                if (map[x, y].IsWalkable && influences[x, y] == 0)
                    return false;
        return true;
    }

    private static void SetUnits(float[,] influences, float [,] influencesBuffer, Cell[,] map, Team teamToSeek) {

        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {

                if (map[x, y].IsTaken != null && map[x, y].IsTaken.Team == teamToSeek) {
                    influences[x, y] = map[x, y].IsTaken.AttributesSheet.CurrentHealth;
                    influencesBuffer[x, y] = map[x, y].IsTaken.AttributesSheet.CurrentHealth;
                }
            }
        }
    }

    private static void UpdateInfluenceMap(float[,] influences, float[,] influencesBuffer, Cell[,] map) {

        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {

                float maxInf = 0.0f;
                float minInf = 0.0f;

                if (map[x, y].IsWalkable == false) {
                    influences[x, y] = 0;
                    continue;
                }

                InfluenceCell[] neighbors = GetNeighbors(map, x, y);
                foreach (InfluenceCell n in neighbors) {

                    float inf = influencesBuffer[n.x, n.y] * Mathf.Exp(-Decay);
                    maxInf = Mathf.Max(inf, maxInf);
                    minInf = Mathf.Min(inf, minInf);
                }

                if (Mathf.Abs(minInf) > maxInf) {
                    influences[x, y] = Mathf.Lerp(influencesBuffer[x, y], minInf, Momentum);
                } else {
                    influences[x, y] = Mathf.Lerp(influencesBuffer[x, y], maxInf, Momentum);
                }
            }
        }
    }

    private static void UpdateInfluenceBuffer(float[,] influences, float[,] influencesBuffer) {

        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
                influencesBuffer[x, y] = influences[x, y];
    }

    private static InfluenceCell[] GetNeighbors(Cell[,] map, int x, int y) {

        List<InfluenceCell> retVal = new List<InfluenceCell>();

        if (x > 0 && map[x - 1, y].IsWalkable && Mathf.Abs(map[x, y].Y - map[x - 1, y].Y) < 2)
            retVal.Add(new InfluenceCell(x - 1, y));

        if (x < mapSizeX - 1 && map[x + 1, y].IsWalkable && Mathf.Abs(map[x, y].Y - map[x + 1, y].Y) < 2)
            retVal.Add(new InfluenceCell(x + 1, y));

        if (y > 0 && map[x, y - 1].IsWalkable && Mathf.Abs(map[x, y].Y - map[x, y - 1].Y) < 2)
            retVal.Add(new InfluenceCell(x, y - 1));

        if (y < mapSizeY - 1 && map[x, y + 1].IsWalkable && Mathf.Abs(map[x, y].Y - map[x, y + 1].Y) < 2)
            retVal.Add(new InfluenceCell(x, y + 1));

        return retVal.ToArray();
    }
}

public struct InfluenceCell {
    public int x;
    public int y;

    public InfluenceCell(int _x, int _y) {
        x = _x;
        y = _y;
    }
}