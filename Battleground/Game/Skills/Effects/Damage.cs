using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Damage", menuName = "Skill/Effect/Damage", order = 1)]
public class Damage : SkillEffect {

    public List<DamageLine> DamageLine;

    public override EffectType GetEffectType() {
        return EffectType.Damage;
    }

    public override List<DamageLine> GetDamage() {
        return DamageLine;
    }
}