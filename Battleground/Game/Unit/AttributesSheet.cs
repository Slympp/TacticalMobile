using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributesSheet {

    public int MaxHealth;
    public int MaxMana;

    [SerializeField]
    private int _CurrentHealth;

    public int CurrentHealth {
        get { return _CurrentHealth; }
        set { _CurrentHealth = (value < 0 ? 0 : (value > MaxHealth) ? MaxHealth : value); }
    }

    [SerializeField]
    private int _CurrentMana;

    public int CurrentMana {
        get { return _CurrentMana; }
        set { _CurrentMana = (value < 0 ? 0 : (value > MaxMana) ? MaxMana : value); }
    }

    [SerializeField]
    public List<Attribute>      StaticAttributes;       // Base statistics
    [SerializeField]
    public List<Attribute>      EquipmentAttributes;    // Statistics provided by equipments
    [SerializeField]
    private List<Attribute>     CostsAttributes;        // Legit Action/Movement-points costs
    [SerializeField]
    private List<Attribute>     DynamicAttributes;      // Buffs/Debuffs

    public int GetAttribute(AttributeType type) {

        if (CostsAttributes == null)
            CostsAttributes = new List<Attribute>();
        if (DynamicAttributes == null)
            DynamicAttributes = new List<Attribute>();

        float finalValue = 0;
        foreach (Attribute stat in StaticAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Value)
                finalValue += stat.Value;
        }
        foreach (Attribute stat in EquipmentAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Value)
                finalValue += stat.Value;
        }
        foreach (Attribute stat in CostsAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Value)
                finalValue += stat.Value;
        }
        foreach (Attribute stat in DynamicAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Value)
                finalValue += stat.Value;
        }

        float mult = 1;
        foreach (Attribute stat in EquipmentAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Multiplicative)
                mult += (((float)stat.Value / 100));
        }
        foreach (Attribute stat in StaticAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Multiplicative)
                mult += (((float)stat.Value / 100));
        }
        foreach (Attribute stat in DynamicAttributes) {
            if (stat.AttributeType == type && stat.PropertyType == PropertyType.Multiplicative) {
                mult += (((float)stat.Value / 100));
            }
        }
        finalValue *= mult;
        return finalValue < 0 ? 0 : Mathf.FloorToInt(finalValue);
    }

    public void AddCostAttribute(Attribute attribute) {
        CostsAttributes.Add(attribute);
    }

    public int GetCostAttribute(AttributeType attributeType) {

        if (CostsAttributes == null)
            return -1;

        int ret_value = 0;
        foreach (Attribute attribute in CostsAttributes) {
            if (attribute.AttributeType == attributeType)
                ret_value += attribute.Value;
        }
        return ret_value;
    }

    public void AddDynamicAttribute(Attribute attribute) {
        DynamicAttributes.Add(attribute);
    }

    public void AddEquipmentAttributes(Equipment equipment) {

        if (equipment != null && equipment.Attributes != null)
            foreach (Attribute stat in equipment.Attributes)
                EquipmentAttributes.Add(stat);
    }

    public void UpdateAttributesDuration() {

        for (int i = 0; i < DynamicAttributes.Count; i++) {
            DynamicAttributes[i].Duration--;
            if (DynamicAttributes[i].Duration <= 0) {
                DynamicAttributes.Remove(DynamicAttributes[i]);
                i--;
            }
        }
    }

    public void RemoveCosts() {
        CostsAttributes.Clear();
    }

    #region FORMULAS

    #region HEAVY

    public int GetStrengthBasedDamage(int initialValue) {
        return 0;
    }

    #endregion

    #region MEDIUM

    public int GetDexterityBasedDamage(int initialValue) {
        return 0;
    }

    public int GetCriticalChanceModifier() {

        // 10 AGI = +1% critical chance
        return (GetAttribute(AttributeType.Agility) % 10);
    }

    public float GetCriticalDamageModifier() {

        // 1 AGI = +0.5% critical damage
        return (GetAttribute(AttributeType.Agility) * 0.005f);
    }

    public int GetDodgeChance() {
        return 0;
    }

    #endregion

    #region LIGHT

    public int GetIntelligenceBasedDamage(int initialValue) {
        return 0;
    }

    #endregion

    public int GetInitiative() {

        return (GetAttribute(AttributeType.Initiative) +
                (GetAttribute(AttributeType.Intelligence) * 1) +
                (GetAttribute(AttributeType.Strength) * 2) +
                (GetAttribute(AttributeType.Dexterity) * 3));
    }

    #endregion
}