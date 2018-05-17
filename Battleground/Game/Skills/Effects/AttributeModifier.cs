using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "AttributeModifier", menuName = "Skill/Effect/AttributeModifier", order = 3)]
public class AttributeModifier : SkillEffect {

    public List<Attribute> AttributeModifiers;

    public override EffectType GetEffectType() {
        return EffectType.AttributeModifier;
    }

    public override List<Attribute> GetAttributeModifier() {
        return AttributeModifiers;
    }
}