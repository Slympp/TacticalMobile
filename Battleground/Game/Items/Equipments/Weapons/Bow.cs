using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Item/Weapon/Bow", order = 3)]
[System.Serializable]
public class Bow : Weapon {

    public SkillBaseConfig SkillBaseConfig = new SkillBaseConfig {
        Cost = 5,
        RangeType = RangeType.Circle,
        MinRange = 5,
        MaxRange = 7,
        AreaOfEffect = new List<AreaOfEffect> {
                            new AreaOfEffect {
                                Type = RangeType.Single,
                            }
        }
    };

    public override SkillBaseConfig GetSkillBaseConfig() {
        return SkillBaseConfig;
    }

    public override bool IsLeftHandOnly() {
        return true;
    }

    public override bool IsTwoHanded() {
        return true;
    }
}
