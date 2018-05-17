using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrbCreationExtensions;


public class V2WorldMapGenerator : MonoBehaviour {

    public GameObject       Tile;
    public int              Seed;
    public Vector2          Offset;
    public float            Scale = 150f;
    public int              Octaves;
    [Range(0, 1f)]
    public float            Persistance;
    public float            Lacunarity;
    [Range(0, 1f)]
    public float            HeightMultiplier;
    public AnimationCurve   HeightCurve;

    public bool             AutoUpdate = true;

    private Transform       MapObject;
    private Transform[,]    ChunkMap;

    private static int      ChunkSize = 8;

    public void GenerateMap() { 

        if (MapObject != null)
            DestroyImmediate(MapObject.gameObject);

        MapObject = new GameObject("Map").transform;
        ChunkMap = new Transform[3, 3];

        int posX = (int)Offset.x;
        int posY = (int)Offset.y;

        // CENTER
        ChunkMap[1, 1] = GenerateChunk(posX, posY);

        // SIDES
        ChunkMap[0, 1] = GenerateChunk(posX - 1, posY);
        ChunkMap[2, 1] = GenerateChunk(posX + 1, posY);
        ChunkMap[1, 0] = GenerateChunk(posX, posY - 1);
        ChunkMap[1, 2] = GenerateChunk(posX, posY + 1);

        // DIAGONALS
        ChunkMap[0, 0] = GenerateChunk(posX - 1, posY - 1);
        ChunkMap[2, 0] = GenerateChunk(posX + 1, posY - 1);
        ChunkMap[2, 2] = GenerateChunk(posX + 1, posY + 1);
        ChunkMap[0, 2] = GenerateChunk(posX - 1, posY + 1); 
    }

    public void MoveMap(MoveDirection direction) {
        
        switch (direction) {

            case MoveDirection.Up:

                DestroyImmediate(ChunkMap[0, 0].gameObject);
                DestroyImmediate(ChunkMap[1, 0].gameObject);
                DestroyImmediate(ChunkMap[2, 0].gameObject);

                ChunkMap[0, 0] = ChunkMap[0, 1]; 
                ChunkMap[1, 0] = ChunkMap[1, 1];
                ChunkMap[2, 0] = ChunkMap[2, 1];

                ChunkMap[0, 1] = ChunkMap[0, 2];
                ChunkMap[1, 1] = ChunkMap[1, 2];
                ChunkMap[2, 1] = ChunkMap[2, 2];

                Offset.y++;

                ChunkMap[0, 2] = GenerateChunk((int)Offset.x - 1, (int)Offset.y + 1);
                ChunkMap[1, 2] = GenerateChunk((int)Offset.x, (int)Offset.y + 1);
                ChunkMap[2, 2] = GenerateChunk((int)Offset.x + 1, (int)Offset.y + 1);

                break;

            case MoveDirection.Down:

                DestroyImmediate(ChunkMap[0, 2].gameObject);
                DestroyImmediate(ChunkMap[1, 2].gameObject);
                DestroyImmediate(ChunkMap[2, 2].gameObject);

                ChunkMap[0, 2] = ChunkMap[0, 1];
                ChunkMap[1, 2] = ChunkMap[1, 1];
                ChunkMap[2, 2] = ChunkMap[2, 1];

                ChunkMap[0, 1] = ChunkMap[0, 0];
                ChunkMap[1, 1] = ChunkMap[1, 0];
                ChunkMap[2, 1] = ChunkMap[2, 0];

                Offset.y--;

                ChunkMap[0, 0] = GenerateChunk((int)Offset.x - 1, (int)Offset.y - 1);
                ChunkMap[1, 0] = GenerateChunk((int)Offset.x, (int)Offset.y - 1);
                ChunkMap[2, 0] = GenerateChunk((int)Offset.x + 1, (int)Offset.y - 1);

                break;

            case MoveDirection.Right:

                DestroyImmediate(ChunkMap[0, 0].gameObject);
                DestroyImmediate(ChunkMap[0, 1].gameObject);
                DestroyImmediate(ChunkMap[0, 2].gameObject);

                ChunkMap[0, 0] = ChunkMap[1, 0];
                ChunkMap[0, 1] = ChunkMap[1, 1];
                ChunkMap[0, 2] = ChunkMap[1, 2];

                ChunkMap[1, 0] = ChunkMap[2, 0];
                ChunkMap[1, 1] = ChunkMap[2, 1];
                ChunkMap[1, 2] = ChunkMap[2, 2];

                Offset.x++;

                ChunkMap[2, 0] = GenerateChunk((int)Offset.x + 1, (int)Offset.y - 1);
                ChunkMap[2, 1] = GenerateChunk((int)Offset.x + 1 , (int)Offset.y);
                ChunkMap[2, 2] = GenerateChunk((int)Offset.x + 1, (int)Offset.y + 1);

                break;

            case MoveDirection.Left:

                DestroyImmediate(ChunkMap[2, 0].gameObject);
                DestroyImmediate(ChunkMap[2, 1].gameObject);
                DestroyImmediate(ChunkMap[2, 2].gameObject);

                ChunkMap[2, 0] = ChunkMap[1, 0];
                ChunkMap[2, 1] = ChunkMap[1, 1];
                ChunkMap[2, 2] = ChunkMap[1, 2];

                ChunkMap[1, 0] = ChunkMap[0, 0];
                ChunkMap[1, 1] = ChunkMap[0, 1];
                ChunkMap[1, 2] = ChunkMap[0, 2];

                Offset.x--;

                ChunkMap[0, 0] = GenerateChunk((int)Offset.x - 1, (int)Offset.y - 1);
                ChunkMap[0, 1] = GenerateChunk((int)Offset.x - 1, (int)Offset.y);
                ChunkMap[0, 2] = GenerateChunk((int)Offset.x - 1, (int)Offset.y + 1);

                break;

            default:
                break;
        }
    }

    private Transform GenerateChunk(int chunkX, int chunkY) {

        Transform chunkObject = new GameObject("Chunk[" + chunkX + "][" + chunkY + "]").transform;

        chunkObject.parent = MapObject;
        chunkObject.transform.position = new Vector3(chunkX * ChunkSize - (ChunkSize / 2),
                                                     0,
                                                     chunkY * ChunkSize - (ChunkSize / 2));

        float[,] noiseMap = GenerateChunkNoise(chunkX, chunkY);

        for (int x = 0; x < ChunkSize; x++) {
            for (int z = 0; z < ChunkSize; z++) { 

                float tileNoise = HeightCurve.Evaluate(noiseMap[x, z]);
                int tileHeight = Mathf.FloorToInt(tileNoise * HeightMultiplier * 10);
                int minHeight = Mathf.FloorToInt(GetAdjacentTilesMinNoise(noiseMap, x, z, chunkX, chunkY, tileNoise) * HeightMultiplier * 10);

                int y = tileHeight;
                do {
                    GameObject go = Instantiate(Tile);
                    go.transform.parent = chunkObject;
                    go.transform.localPosition = new Vector3(x, y, z);
                    y--;
                } while (y > minHeight);
            }
        }

        chunkObject.gameObject.CombineMeshes();
        chunkObject.gameObject.GetSimplifiedMeshInBackground(1f, true, 1f, res => chunkObject.GetComponent<MeshCollider>().sharedMesh = res);
        chunkObject.gameObject.AddComponent<MeshCollider>();
        return chunkObject;
    }

    private float     GetAdjacentTilesMinNoise(float[,] noiseMap, int x, int y, int chunkX, int chunkY, float noise) {

        float minHeight = noise;
        float evaluatedNoise;

        evaluatedNoise = (x > 0) ? HeightCurve.Evaluate(noiseMap[x - 1, y]) :
                                   HeightCurve.Evaluate(GetTileNoise(chunkX * ChunkSize - 1, chunkY * ChunkSize));
        if (evaluatedNoise < minHeight) minHeight = evaluatedNoise;

        evaluatedNoise = (x < ChunkSize - 1) ? HeightCurve.Evaluate(noiseMap[x + 1, y]) :
                                   HeightCurve.Evaluate(GetTileNoise(chunkX * ChunkSize + 1, chunkY * ChunkSize));
        if (evaluatedNoise < minHeight) minHeight = evaluatedNoise;

        evaluatedNoise = (y > 0) ? HeightCurve.Evaluate(noiseMap[x, y - 1]) :
                                   HeightCurve.Evaluate(GetTileNoise(chunkX * ChunkSize, chunkY * ChunkSize - 1));
        if (evaluatedNoise < minHeight) minHeight = evaluatedNoise;

        evaluatedNoise = (y < ChunkSize - 1) ? HeightCurve.Evaluate(noiseMap[x, y + 1]) :
                                   HeightCurve.Evaluate(GetTileNoise(chunkX * ChunkSize, chunkY * ChunkSize + 1));
        if (evaluatedNoise < minHeight) minHeight = evaluatedNoise;

        return minHeight;
    }

    private float[,] GenerateChunkNoise(int chunkX, int chunkY) {

        float[,] noiseMap = new float[ChunkSize, ChunkSize];

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                noiseMap[x, y] = GetTileNoise(chunkX * ChunkSize + x, chunkY * ChunkSize + y);
            }
        }
        return noiseMap;
    }

    private float GetTileNoise(int x, int y) {

        float amplitude = 1f;
        float frequency = 1f;
        float noiseHeight = 0f;

        for (int i = 0; i < Octaves; i++) {
            float tileX = x / Scale * frequency;
            float tileY = y / Scale * frequency;

            float perlinValue = Mathf.PerlinNoise(tileX + Seed, tileY + Seed);
            noiseHeight += perlinValue * amplitude;

            amplitude *= Persistance;
            frequency *= Lacunarity;
        }

        return (noiseHeight);
    }

    private void OnValidate() {

        if (Scale <= 0)
            Scale = 0.0001f;
        if (Lacunarity < 1)
            Lacunarity = 1;

        if (Octaves < 0)
            Octaves = 0;
        else if (Octaves > 24)
            Octaves = 24;
    }


}