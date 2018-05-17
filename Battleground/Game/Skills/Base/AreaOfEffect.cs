using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaOfEffect {
    public RangeType                Type;
    public int                      MinSize;
    public int                      MaxSize;
    public bool                     NeedLineOfSight = false;
    [Tooltip("Unstable")]
    public bool                     NeedPathToTarget = false;
    public List<RelativeRotation>   Rotation;
}

public enum CostType {
    ActionPoint,
    MovementPoint,
}