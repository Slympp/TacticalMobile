using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillEffect : ScriptableObject {

    public int  ProcChance = 100;

    public virtual EffectType GetEffectType() {
        return EffectType.Null;
    }

    public virtual List<DamageLine>     GetDamage() { return null; }
    public virtual List<RecoveryLine>   GetRecovery() { return null; }
    public virtual List<Attribute>      GetAttributeModifier() { return null; }
    //public virtual List<Status>         GetStatus() { return null };
}

public enum EffectType {
    Damage,
    Recovery,
    AttributeModifier,
    Status,
    Null
}