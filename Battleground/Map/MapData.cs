using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
[System.Serializable]
public class MapData : ScriptableObject {

    [System.Serializable]
    public class MaterialList {
        public Material[] Variations = new Material[] { null, null };
    }

    public MaterialList[] BlocTypes = new MaterialList[] { null, null, null, null, null };

    public float    Zoom = 0.1f;
    public bool     NonWalkableCheck = false;
    [HideInInspector]
    public float    MinNonWalkable = 0.1f;
    [HideInInspector]
    public float    MaxNonWalkable = 0.25f;
    [SerializeField]
    public float[]  BlocHeight = new float[4] {0.2f, 0.3f, 0.4f, 0.6f };
    public Sprite   Background;
}