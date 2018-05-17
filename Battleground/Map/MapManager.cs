using System.Collections;
using System.Collections.Generic;
using OrbCreationExtensions;
using UnityEngine;
using LlockhamIndustries.Decals;

public class MapManager : MonoBehaviour {

    public Vector2      MinSize;
    public Vector2      MaxSize;

    public GameObject   CellTemplate;
    public GameObject   ClickableCellTemplate;

    public void GenerateMaps(MapInfos mapInfos) {
        
        GenerateMap(mapInfos);
        GenerateClickableMap(mapInfos);
    }

    private void GenerateMap(MapInfos mapInfos) {

        GameObject MapObject = new GameObject("Map");
        MapObject.layer = 8;
        MapObject.transform.parent = this.transform;

        mapInfos.ProjectionMap = new ProjectionRenderer[(int)mapInfos.Size.x, (int)mapInfos.Size.y];

        if (mapInfos.Map == null) {

            mapInfos.Layout = new MapLayout(mapInfos);
            mapInfos.Map = new Cell[(int)mapInfos.Size.x, (int)mapInfos.Size.y];

            for (int y = 0; y < mapInfos.Layout.SizeY; y++) {
                for (int x = 0; x < mapInfos.Layout.SizeX; x++) {

                    if (mapInfos.Layout.Layout[x, y] != (int)BlocHeight.Empty) {
                        mapInfos.Map[x, y] = new Cell(x, y, mapInfos.Layout.Layout[x, y], MapObject.transform, CellTemplate,
                                            mapInfos.MapData.BlocTypes[mapInfos.Layout.Layout[x, y]].Variations[Random.Range(0, mapInfos.MapData.BlocTypes[mapInfos.Layout.Layout[x, y]].Variations.Length - 1)],
                                            mapInfos.MapData.BlocTypes[mapInfos.Layout.Layout[x, y]].Variations[mapInfos.MapData.BlocTypes[mapInfos.Layout.Layout[x, y]].Variations.Length - 1]);

                        mapInfos.Map[x, y].BuildCell(MapObject.transform, CellTemplate);
                    } else
                        mapInfos.Map[x, y] = new Cell(x, y);
                }
            }

        } else {

            for (int y = 0; y < mapInfos.Layout.SizeY; y++)
                for (int x = 0; x < mapInfos.Layout.SizeX; x++)
                    if (mapInfos.Layout.Layout[x, y] != (int)BlocHeight.Empty)
                        mapInfos.Map[x, y].BuildCell(MapObject.transform, CellTemplate);
        }

        MapObject.CombineMeshes();
        MapObject.GetSimplifiedMeshInBackground(1f, true, 1f, res => MapObject.GetComponent<MeshCollider>().sharedMesh = res);
    }

    private void GenerateClickableMap(MapInfos mapInfos) {

        GameObject ClickableMap = new GameObject("MapClickable");
        ClickableMap.transform.parent = this.transform;

        for (int y = 0; y < mapInfos.Layout.SizeY; y++) {
            for (int x = 0; x < mapInfos.Layout.SizeX; x++) {

                if (mapInfos.Layout.Layout[x, y] != (int)BlocHeight.Empty) {
                    GameObject projection = Instantiate(ClickableCellTemplate);

                    projection.transform.parent = ClickableMap.transform;
                    projection.transform.position = projection.transform.position + mapInfos.Map[x, y].GetVector3Position();
                    mapInfos.ProjectionMap[x, y] = projection.GetComponent<ProjectionRenderer>();
                    mapInfos.ProjectionMap[x, y].enabled = false;

                    float height = (int)mapInfos.Map[x, y].Y;
                    BoxCollider collider = projection.GetComponent<BoxCollider>();
                    collider.center = new Vector3(0, 0, 0.4f + (height / 2));
                    collider.size = new Vector3(1.1f, 1.1f, 1.1f * height);
                }
            }
        }
    }
}
