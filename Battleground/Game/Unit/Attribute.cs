using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attribute {

    public AttributeType    AttributeType;
    public int              Value;
    public PropertyType     PropertyType;
    public int              Duration = -1;

    public Attribute(AttributeType attributeType, int value, PropertyType propertyType, int duration) {
        AttributeType = attributeType;
        Value = value;
        PropertyType = propertyType;
        Duration = duration;
    }
}

public enum AttributeType {
    ActionPoint,
    MovementPoint,
    Initiative,
    Strength,
    Constitution,
    Intelligence,
    Wisdom,
    Dexterity,
    Agility,
    PhysicalArmor,
    MagicalArmor,
    DodgeChance
}

public enum PropertyType {
    Value,
    Multiplicative
}
