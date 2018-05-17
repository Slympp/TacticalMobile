using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Body", menuName = "Item/Body", order = 2)]
[System.Serializable]
public class Body : Equipment {

    public Material[]  MaleSkinStone = new Material[3];
    public Material[]  FemaleSkinTone = new Material[3];
    public Material    OtherSkinTone;
    public GameObject  SpecialEffect;
}
