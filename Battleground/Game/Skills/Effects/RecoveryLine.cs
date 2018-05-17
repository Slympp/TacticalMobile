using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecoveryLine {

    public int minValue;
    public int maxValue;
    public int finalValue { get { return Random.Range(minValue, maxValue + 1); } }

    public RecoveryType recoveryType;
}

public enum RecoveryType {
    Health,
    Mana,
}