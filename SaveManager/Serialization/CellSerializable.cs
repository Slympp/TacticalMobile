using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellSerializable {

    public int      UnitId;
    public bool     IsWalkable;

    public int      X;
    public float    Y;
    public int      Z;
    public float    RealY;

    public string   MaterialName;
    public string   FillerName;
}
