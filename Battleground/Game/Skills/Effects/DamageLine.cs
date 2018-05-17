using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageLine {

    public int          MinValue;
    public int          MaxValue;
    public int          FinalValue { get { return Random.Range(MinValue, MaxValue + 1); } }

    public DamageType   DamageType;
    public ElementType  ElementType;
    public bool         IsLeech = false;
}

public enum DamageType {
    StrengthBased,
    IntelligenceBased,
    DexterityBased
}

public enum ElementType {
    None,
    Fire,
    Ice,
    Wind,
    Earth,
    Lightning,
    Water,
}

// Fire > Ice > Wind > Earth > Lightning > Water > Fire > ...
// Shadow <> Light
// Life, Arcane ?