using LlockhamIndustries.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfos {

    public MapData                  MapData;
    public Vector2                  Size;

    public MapLayout                Layout;
    public Cell[,]                  Map;
    public ProjectionRenderer[,]    ProjectionMap;
}
