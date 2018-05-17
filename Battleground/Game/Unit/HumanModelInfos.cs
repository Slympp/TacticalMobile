using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanModelInfos {

    public Gender       Gender;
    public SkinTone     SkinTone;
    [Tooltip("HairType should always be > 0")]
    public int          HairType = 1;
    public HairColor    HairColor;

    public void Randomize() {

        Gender = (Gender)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Gender)).Length);
        SkinTone = (SkinTone)UnityEngine.Random.Range(0, Enum.GetNames(typeof(SkinTone)).Length - 1);
        HairType = UnityEngine.Random.Range(1, 16 + 1);
        HairColor = (HairColor)UnityEngine.Random.Range(0, Enum.GetNames(typeof(HairColor)).Length);
    }
}

public enum Gender {
    Male,
    Female,
}

public enum SkinTone {
    White,
    Brown,
    Black,
    Other,
}

public enum HairColor {
    Black,
    Blonde,
    Brown,
    Cyan,
    Grey,
    Purple,
    Red,
}