using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear", menuName = "Item/Weapon/Spear", order = 2)]
[System.Serializable]
public class Spear : Weapon {

    public SkillBaseConfig SkillBaseConfig = new SkillBaseConfig {
        Cost = 5,
        RangeType = RangeType.Circle,
        MinRange = 1,
        MaxRange = 1,
        NeedPathToTarget = true,
        AreaOfEffect = new List<AreaOfEffect> {
                            new AreaOfEffect {
                                Type = RangeType.Line,
                                MinSize = 0,
                                MaxSize = 1,
                                NeedPathToTarget = true,
                                Rotation = new List<RelativeRotation> {
                                                RelativeRotation.Front
                                }
                            }
        }
    };

    public override SkillBaseConfig GetSkillBaseConfig() {
        return SkillBaseConfig;
    }

    public override bool IsTwoHanded() {
        return true;
    }
}
