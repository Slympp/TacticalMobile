using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Item/Weapon/Sword", order = 1)]
[System.Serializable]
public class Sword : Weapon {

    public SkillBaseConfig SkillBaseConfig = new SkillBaseConfig {
        Cost = 4,
        RangeType = RangeType.Circle,
        MinRange = 1,
        MaxRange = 1,
        AreaOfEffect = new List<AreaOfEffect> {
                            new AreaOfEffect {
                                Type = RangeType.Single,
                            }
        }
    };

    public override SkillBaseConfig GetSkillBaseConfig() {
        return SkillBaseConfig;
    }
}
