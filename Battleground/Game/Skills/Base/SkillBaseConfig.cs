using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBaseConfig {

    public int                  Cost;

    [Tooltip("How many turns before the skill can be cast again ?" +
            "\n(0 = no cooldown, 1 = next turn)")]
    public int                  Cooldown = 0;
    [Tooltip("How many times the skill can be cast during the current turn ?" +
            "\n(-1 = no per turn limitation)")]
    public int                  UsePerTurn = -1;
    [Tooltip("% of chances that the effect is critical (* 1.5)")]
    public int                  CriticalChance = 0;

    [Header("--- Range ---")]
    public RangeType            RangeType = RangeType.Single;
    public int                  MinRange = 1;
    public int                  MaxRange = 1;
    [Tooltip("Currently not working")]
    public bool                 NeedLineOfSight = false;
    public bool                 NeedPathToTarget = false;

    [Header("--- Effects ---")]
    public List<SkillEffect>   SkillEffects;

    [Header("--- Area of Effect ---")]
    public List<AreaOfEffect>   AreaOfEffect;

    [Header("--- Animations ---")]
    public List<SkillAnimation> Animations;
}
