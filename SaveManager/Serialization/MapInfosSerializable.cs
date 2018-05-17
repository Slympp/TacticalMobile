using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfosSerializable {

	public MapDataType              MapDataType;
    public int                      SizeX;
    public int                      SizeY;
    public MapLayout                Layout;
    public CellSerializable[,]      Map;
    public List<string>             Materials;
}
