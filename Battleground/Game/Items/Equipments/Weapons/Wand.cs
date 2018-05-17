using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wand", menuName = "Item/Weapon/Wand", order = 4)]
[System.Serializable]
public class Wand : Weapon {

    public SkillBaseConfig SkillBaseConfig = new SkillBaseConfig {
        Cost = 3,
        RangeType = RangeType.Circle,
        MinRange = 4,
        MaxRange = 6,
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
