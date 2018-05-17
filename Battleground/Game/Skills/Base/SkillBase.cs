using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/New Skill", order = 1)]
public class SkillBase : ScriptableObject {

    [Header("--- Base ---")]
    public string               SkillName;
    public string               SkillDescription = "Skill description";
    public int                  SkillId;
    public CostType             CostType;
    public int                  Cost;
    public int                  ManaCost = 0;
    public Sprite               Icon;

    [Header("-- Attributes --")]
    [Tooltip("Does the skill affect the caster ?")]
    public bool                 HitSelf = true;
    [Tooltip("Does the skill affect caster allies ?")]
    public bool                 HitAllies = true;
    [Tooltip("Does the skill affect caster enemies ?")]
    public bool                 HitEnemies = true;
    [Tooltip("How many turns before the skill can be cast again ?" +
            "\n(0 = no cooldown, 1 = next turn)")]
    public int                  Cooldown = 0;
    [Tooltip("How many times the skill can be cast during the current turn ?" +
            "\n(-1 = no per turn limitation)")]
    public int                  UsePerTurn = -1;
    [Tooltip("% of chances that the effect is critical (* 1.5)")]
    public int                  CriticalChance = 0;

    [Header("--- Range ---")]
    public RangeType            RangeType;
    public int                  MinRange;
    public int                  MaxRange;
    [Tooltip("Currently not working")]
    public bool                 NeedLineOfSight = false;
    [Tooltip("Currently not working (for future movement skills)")]
    public bool                 TargetMustBeEmpty = false;
    [Tooltip("Currently not working (for future movement skills)")]
    public bool                 NeedPathToTarget = false;
    public RelativeRotation[]   Rotation = { RelativeRotation.All };

    [Header("--- Area of Effect ---")]
    public List<AreaOfEffect> AreaOfEffect;
    public List<Unit> Targets { get; private set; }

    [Header("--- Effects ---")]
    [SerializeField]
    public List<SkillEffect>    SkillEffects;

    [Header("--- Animations ---")]
    public List<SkillAnimation> Animations;

    public static SkillBase Create(string itemName, Sprite skillIcon, SkillBaseConfig config) {
        return CreateInstance<SkillBase>().Init(itemName, skillIcon, config);
    }

    public SkillBase Init(string itemName, Sprite skillIcon, SkillBaseConfig config) {
        SkillName = itemName;
        Icon = skillIcon;

        CostType = CostType.ActionPoint;

        Cost = config.Cost;
        Cooldown = config.Cooldown;
        UsePerTurn = config.UsePerTurn;
        CriticalChance = config.CriticalChance;
        SkillEffects = config.SkillEffects;

        //if (config.SkillEffects != null) {
        //    SkillEffects = new List<SkillEffect>();
        //    for (int i = 0; i < config.SkillEffects.Count; i++)
        //        SkillEffects.Add(Damage.Create(config.SkillEffects[i]));
        //}

        RangeType = config.RangeType;
        MinRange = config.MinRange;
        MaxRange = config.MaxRange;
        NeedLineOfSight = config.NeedLineOfSight;
        NeedPathToTarget = config.NeedPathToTarget;
        AreaOfEffect = config.AreaOfEffect;
        Animations = config.Animations;
        
        return this;
    }

    public void DisplayRange(ProjectionManager pm, Cell currentCell, Rotation currentRotation) {

        pm.ClearProjections();
        if (Rotation.Length == 0)
            pm.SelectCells(RangeType, currentCell,
                            MinRange,
                            MaxRange,
                            NeedLineOfSight,
                            TargetMustBeEmpty,
                            NeedPathToTarget,
                            true,
                            false,
                            SelectionState.DisplaySkillRange);
        else {
            foreach (RelativeRotation relativeRotation in Rotation)
                pm.SelectCells(RangeType, currentCell,
                                MinRange,
                                MaxRange,
                                NeedLineOfSight,
                                TargetMustBeEmpty,
                                NeedPathToTarget,
                                true,
                                false,
                                SelectionState.DisplaySkillRange,
                                relativeRotation,
                                currentRotation);
        }
        pm.ColorProjections(pm.Selection, Color.cyan);
        pm.EnableProjections(pm.Selection, true);

        if (NeedLineOfSight) {
            pm.ColorProjections(pm.NotInLOS, Color.grey);
            pm.EnableProjections(pm.NotInLOS, true);
        }
    }

    public void DisplayAreaOfEffect(ProjectionManager pm, Cell targetCell, Rotation currentRotation) {

        pm.ClearProjections();
        foreach (SkillEffect effect in SkillEffects) {

            foreach (AreaOfEffect aoe in AreaOfEffect) {
                if (aoe.Rotation.Count == 0)
                    pm.SelectCells(aoe.Type, targetCell,
                                    aoe.MinSize,
                                    aoe.MaxSize,
                                    aoe.NeedLineOfSight,
                                    TargetMustBeEmpty,
                                    aoe.NeedPathToTarget,
                                    true,
                                    true,
                                    SelectionState.DisplaySkillAoe);
                else
                    foreach (RelativeRotation relativeRotation in aoe.Rotation)
                        pm.SelectCells(aoe.Type, targetCell,
                                        aoe.MinSize,
                                        aoe.MaxSize,
                                        aoe.NeedLineOfSight,
                                        TargetMustBeEmpty,
                                        aoe.NeedPathToTarget,
                                        true,
                                        true,
                                        SelectionState.DisplaySkillAoe,
                                        relativeRotation,
                                        currentRotation);
            }
        }
    }

    public void GetTargetsInAreaOfEffect(ProjectionManager pm, Unit caster) {

        Targets = new List<Unit>();
        foreach (Cell cell in pm.AreaOfEffect) {
            pm.ColorCell(cell.X, cell.Z, Color.magenta);
            if (cell.IsTaken) {

                if (!HitSelf && cell.IsTaken == caster)
                    continue;

                if (!HitAllies && cell.IsTaken.Team == caster.Team)
                    continue;

                if (!HitEnemies && cell.IsTaken.Team != caster.Team)
                    continue;

                Targets.Add(cell.IsTaken);
            }
        }
        pm.EnableProjections(pm.Selection, true);
    }

    public void GetEffect(AttributesSheet casterAttributes) {

            if (Targets.Count > 0) {
                foreach (Unit target in Targets) {

                    foreach (SkillEffect effect in SkillEffects) {

                        bool isCriticalHit = true;
                            if (CriticalChance != 100) {
                                int randomNb = Random.Range(0, 100);
                                isCriticalHit = randomNb < (CriticalChance + casterAttributes.GetCriticalChanceModifier()) ? true : false;
                            }

                            if (effect.ProcChance != 100) {
                                int randomNb = Random.Range(0, 100);
                                if (randomNb > effect.ProcChance)
                                    continue;
                            }

                            switch (effect.GetEffectType()) {
                                case EffectType.Damage:
                                    List<DamageLine> damageList = effect.GetDamage();
                                    foreach (DamageLine line in damageList)
                                        target.ApplyDamage(casterAttributes, line, isCriticalHit);
                                    break;
                                case EffectType.Recovery:
                                    List<RecoveryLine> recoveryList = effect.GetRecovery();
                                    foreach (RecoveryLine line in recoveryList)
                                        target.ApplyRecovery(casterAttributes, line, isCriticalHit);
                                    break;
                                case EffectType.AttributeModifier:
                                    List<Attribute> attributeList = effect.GetAttributeModifier();
                                    foreach (Attribute attribute in attributeList) {
                                        Attribute newAttribute = new Attribute(attribute.AttributeType,
                                                                                attribute.Value,
                                                                                attribute.PropertyType,
                                                                                attribute.Duration);
                                        target.ApplyAttributeModifier(newAttribute);
                                    }
                                   break;

                                //case EffectType.Buff:
                                //    List<Buff> buffList = effect.GetBuff();
                                //    foreach (Buff buff in buffList)
                                //        target.ApplyBuff(buff);
                                //    break;

                                // Fix loop so effect can be get even if no target
                                //case EffectType.Movement:
                                // caster.Move();
                                //break;

                                default:
                                    break;
                            }
                    }
            }
        }
    }
}