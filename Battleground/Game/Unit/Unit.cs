using LlockhamIndustries.Decals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Unit : MonoBehaviour {

    public string               UnitName;
    public Team                 Team;
    public List<SkillBase>      Skills;
    public AttributesSheet      AttributesSheet;
    public UnitEquipment        UnitEquipment;
    public HumanModelInfos      HumanModelInfos;
    //public List<Status>      Status;


    // UN-COMMENT after WorldMap implementation
    //public string           UnitName { get; set; }
    //public Team             Team { get; set; }
    //public List<SkillBase>  Skills { get; private set; }
    //public Statistics       Stats { get; private set; }
    //public UnitEquipment    Equipment { get; private set; }
    //public HumanModelInfos  ModelInfos { get; private set; }

    public int                  Id;
    public bool                 IsAlive { get; set; }
    public bool                 IsPlaying { get; set; } = false;
    public Cell                 CurrentCell { get; set; }
    public Rotation             CurrentRotation { get; set; } = Rotation.NULL;
    public List<int>            SkillsCooldowns { get; set; }
    public List<int>            SkillsUses { get; set; }

    public ActionState          ActionState { get; set; } = ActionState.Waiting;
    public int                  ActiveSkill { get; set; } = -1;
    public string               AnimationPrefix { get; set; } = "";

    public Slider               HealthSlider;
    public ProjectionRenderer   SelfProjection;

    public Animator             Animator { get; private set; }
    public ProjectionManager    ProjectionManager { protected get; set; }
    public CombatUIManager      CombatUIManager { protected get; set; }
    public ConfirmationWindow   ConfirmationWindow { private get; set; }
    public MapInfos             MapInfos { protected get; set; }
    private Movement            Movement;
    private GameObject          Model;

    public virtual void Init() {
        ActionState = ActionState.Waiting;
        if (SkillsCooldowns == null)
            SkillsCooldowns = new List<int>() { 0, 0, 0, 0, 0, 0 };

        IsAlive = (AttributesSheet.CurrentHealth != 0) ? true : false;

        Animator = transform.GetChild(0).GetComponent<Animator>();
        Movement = gameObject.AddComponent<Movement>();
        Movement.CombatUIManager = CombatUIManager;

        if (UnitEquipment != null)
            InitEquipment();

        if (SkillsUses == null)
            UpdateSkillsUses();
        UpdateSkillsAnimations();
    }

    // HUMAN
    private void InitEquipment() {

        AttributesSheet.EquipmentAttributes = new List<Attribute>();
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.Body);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.Head);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.Back);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.AccessoryA);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.AccessoryB);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.RightWeapon);
        AttributesSheet.AddEquipmentAttributes(UnitEquipment.LeftWeapon);

        if (UnitEquipment.RightWeapon != null)
            InitMainWeapon(UnitEquipment.RightWeapon);
        else if (UnitEquipment.LeftWeapon != null)
            InitMainWeapon(UnitEquipment.LeftWeapon);
        else {
            Sword punch = Resources.Load("Items/Equipments/Weapons/Sword/Punch/Punch") as Sword;
            SkillBase punchSkill = SkillBase.Create(punch.ItemName, punch.SkillIcon, punch.GetSkillBaseConfig());
            Skills[0] = punchSkill;
        }
    }

    private void InitMainWeapon(Weapon weapon) {

        SkillBaseConfig skillBaseConfig;

        if (weapon.AnimationPrefix != "") {
            AnimationPrefix = weapon.AnimationPrefix + " ";
            Animator.SetBool(AnimationPrefix + "Idle", true);
            Movement.AnimationPrefix = AnimationPrefix;
        }

        if ((skillBaseConfig = weapon.GetSkillBaseConfig()) != null) {
            SkillBase weaponSkill = SkillBase.Create(weapon.ItemName, weapon.SkillIcon, skillBaseConfig);
            Skills[0] = weaponSkill;
        }
    }

    // HUMAN
    public void LoadModel() {
        Model = HumanModelBuilder.BuildModel(HumanModelInfos, UnitEquipment);
        Model.transform.parent = this.transform;
    }

    public virtual IEnumerator Turn() { yield return null; }

    public void InitTurn() {

        ActiveSkill = -1;

        if (!IsPlaying) {
            AttributesSheet.UpdateAttributesDuration();
            UpdateSkillsCooldowns();
            UpdateSkillsUses();
        }

        IsPlaying = true;
        CombatUIManager.UpdateActionPoint(AttributesSheet.GetAttribute(AttributeType.ActionPoint));
        CombatUIManager.UpdateMovementPoint(AttributesSheet.GetAttribute(AttributeType.MovementPoint));

        SelfProjection.gameObject.SetActive(false);
        ActionState = ActionState.Waiting;
    }

    public bool Move(Cell targetCell, bool animate, int distance) {

        if (animate) {
            List<Cell> path = PathFinding.FindPath(MapInfos, CurrentCell, targetCell);
            if (path != null) {
                Movement.StartMovement(path, CurrentCell);
                ProjectionManager.SelectPath(path);
            }
        } else
            transform.position = targetCell.GetVector3Position();

        if (CurrentCell != null)
            CurrentCell.IsTaken = null;
        CurrentCell = targetCell;
        CurrentCell.IsTaken = this;

        if (distance > 0) {
            AttributesSheet.AddCostAttribute(new Attribute(AttributeType.MovementPoint, -distance, PropertyType.Value, 1));
            CombatUIManager.UpdateMovementPoint(AttributesSheet.GetAttribute(AttributeType.MovementPoint));
        }
        return true;
    }

    public void UpdateSkillsAnimations() {

        if (Skills != null)
            for (int i = 0; i < Skills.Count; i++) {
                if (Skills[i] != null && Skills[i].Animations != null)
                    for (int j = 0; j < Skills[i].Animations.Count; j++) {
                        Skills[i].Animations[j].Init();
                    }
            }
    }

    public void UpdateSkillsCooldowns() {

        if (Skills != null) {
            for (int i = 0; i < Skills.Count; i++) {
                if (Skills[i] != null && Skills[i].Cooldown != 0 && SkillsCooldowns[i] > 0)
                    SkillsCooldowns[i]--;
            }
        }
    }

    public void UpdateSkillsUses() {

        SkillsUses = new List<int>(6);
        if (Skills != null) {
            for (int i = 0; i < Skills.Count; i++)
                if (Skills[i] != null)
                    SkillsUses.Add(Skills[i].UsePerTurn);
                else
                    SkillsUses.Add(-1);
        }
    }

    public SkillBase GetSkill(int skillId) {
        if (Skills != null && skillId < Skills.Count) return Skills[skillId];
        return null;
    }

    public void SkillDisplayRange(int newActiveSkill) {

        // If click again on activeSkill, cancel DisplayRange
        if (newActiveSkill == ActiveSkill) {
            ConfirmationWindow.SetActive(false);
            ActionState = ActionState.Waiting;
            ActiveSkill = -1;
            return;
        } else
            ActiveSkill = newActiveSkill;

        int availableAttribute = AttributesSheet.GetAttribute((AttributeType)Skills[ActiveSkill].CostType);
        if (Skills[ActiveSkill].Cost <= availableAttribute) {

            SetState(ActionState.DisplayingRange, false);
            Skills[ActiveSkill].DisplayRange(ProjectionManager, CurrentCell, CurrentRotation);

        } else {
            Debug.Log("Can not use " + Skills[ActiveSkill].SkillName + ", " + availableAttribute + " " +
                        (AttributeType)Skills[ActiveSkill].CostType + " are available but need " +
                        Skills[ActiveSkill].Cost + " " + (AttributeType)Skills[ActiveSkill].CostType);
        }
    }

    public void SkillForceDisplayRange() {

        SetState(ActionState.DisplayingRange, false);
        Skills[ActiveSkill].DisplayRange(ProjectionManager, CurrentCell, CurrentRotation);
    }

    public void SkillDisplayAoe(Cell targetCell) {

        SetState(ActionState.DisplayingAoe, false);
        Skills[ActiveSkill].DisplayAreaOfEffect(ProjectionManager, targetCell, CurrentRotation);
        Skills[ActiveSkill].GetTargetsInAreaOfEffect(ProjectionManager, this);
    }

    public void SkillUse(Cell targetCell) {

        AttributesSheet.AddCostAttribute(new Attribute((AttributeType)Skills[ActiveSkill].CostType,
                        -Skills[ActiveSkill].Cost,
                        PropertyType.Value,
                        0));

        AttributesSheet.CurrentMana -= Skills[ActiveSkill].ManaCost;

        if (Skills[ActiveSkill].CostType == CostType.ActionPoint)
            CombatUIManager.UpdateActionPoint(AttributesSheet.GetAttribute(AttributeType.ActionPoint));
        else if (Skills[ActiveSkill].CostType == CostType.MovementPoint)
            CombatUIManager.UpdateMovementPoint(AttributesSheet.GetAttribute(AttributeType.MovementPoint));

        if (Skills[ActiveSkill].Cooldown != 0)
            SkillsCooldowns[ActiveSkill] = Skills[ActiveSkill].Cooldown;

        if (Skills[ActiveSkill].UsePerTurn != -1)
            SkillsUses[ActiveSkill]--;

        if (Skills[ActiveSkill].Animations != null && Skills[ActiveSkill].Animations.Count > 0) {

            SetState(ActionState.PlayingAnimation, false);
            CombatUIManager.SetActiveAllButtons(false, 0);
            for (int i = 0; i < Skills[ActiveSkill].Animations.Count; i++)
                StartCoroutine(Skills[ActiveSkill].Animations[i].Play(Animator, CurrentCell, targetCell, () => EndAnimation()));

        } else {

            Skills[ActiveSkill].GetEffect(AttributesSheet);
            ActiveSkill = -1;
            SetState(ActionState.Waiting, false);
            CombatUIManager.UpdateSkillsButtons();
        }

        CombatUIManager.UpdateManaBar();
    }

    public void EndAnimation() {

        Skills[ActiveSkill].GetEffect(AttributesSheet);
        ActiveSkill = -1;
        SetState(ActionState.Waiting, false);
        CombatUIManager.SetActiveAllButtons(true, AttributesSheet.GetAttribute(AttributeType.MovementPoint));
        CombatUIManager.UpdateSkillsButtons();
    }

    public void ApplyDamage(AttributesSheet casterAttributes, DamageLine line, bool isCritical, Weapon weapon = null) {

        float value = line.FinalValue;
        if (isCritical) value *= 1.5f + casterAttributes.GetCriticalDamageModifier();

        Debug.Log(UnitName + ": damaged " + value + " (" + line.DamageType + ")(" + line.ElementType + ")(" + isCritical + ")");
        int oldValue = AttributesSheet.CurrentHealth;
        AttributesSheet.CurrentHealth -= (int)value;
        Debug.Log(UnitName + ": " + oldValue + " => " + AttributesSheet.CurrentHealth);

        Animator.Play(AnimationPrefix + "Take Damage");
        CombatUIManager.UpdateHealthBar();
        UpdateHealthSlider();

        UpdateIsAlive();
    }

    public void ApplyRecovery(AttributesSheet casterAttributes, RecoveryLine line, bool isCritical) {
        float value = line.finalValue;
        if (isCritical) value *= 1.5f + casterAttributes.GetCriticalDamageModifier();

        Debug.Log(UnitName + ": recovered " + value + " (" + line.recoveryType + ")(" + isCritical + ")");
        if (line.recoveryType == RecoveryType.Health)
            AttributesSheet.CurrentHealth += (int)value;
        else if (line.recoveryType == RecoveryType.Mana)
            AttributesSheet.CurrentMana += (int)value;

        CombatUIManager.UpdateHealthBar();
        UpdateHealthSlider();
        CombatUIManager.UpdateManaBar();
    }

    public void ApplyAttributeModifier(Attribute attribute) {
        Debug.Log(UnitName + ": " + attribute.AttributeType + " modified (" + attribute.Value + ")(" + attribute.PropertyType + ")");
        int oldValue = AttributesSheet.GetAttribute(attribute.AttributeType);
        AttributesSheet.AddDynamicAttribute(attribute);
        Debug.Log(UnitName + ": " + oldValue + " => " + AttributesSheet.GetAttribute(attribute.AttributeType) + " (" + attribute.AttributeType + ")");

        CombatUIManager.UpdateHealthBar();
        UpdateHealthSlider();
        CombatUIManager.UpdateManaBar();
    }

    private void UpdateHealthSlider() {
        HealthSlider.value = AttributesSheet.CurrentHealth * 100 / AttributesSheet.MaxHealth;
    }

    public void UpdateIsAlive() {
        if (AttributesSheet.CurrentHealth == 0) {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die() {

        IsAlive = false;
        Animator.Play("Die");
        yield return new WaitForSeconds(1.2f);
        this.gameObject.SetActive(false);
        HealthSlider.transform.parent.gameObject.SetActive(false);
        yield return null;
    }

    public void UpdateRotation(Cell targetCell) {

        if (targetCell != null) {
            transform.LookAt(targetCell.GetVector3Position());

            float yAngle = transform.rotation.eulerAngles.y;

            if (yAngle > 315 || yAngle <= 45)
                yAngle = (float)Rotation.NE;
            else if (yAngle > 45 && yAngle <= 135)
                yAngle = (float)Rotation.SE;
            else if (yAngle > 135 && yAngle <= 225)
                yAngle = (float)Rotation.SO;
            else
                yAngle = (float)Rotation.NO;

            transform.rotation = Quaternion.Euler(new Vector3(0, yAngle, 0));
        }

        CurrentRotation = ((Rotation)(int)transform.eulerAngles.y);
    }

    public void SetState(ActionState state, bool reset) {

        if (state == ActionState.LookingForMovement || state == ActionState.Waiting)
            ActiveSkill = -1;
        if (reset && ActionState == state)
            ActionState = ActionState.Waiting;
        else
            ActionState = state;
    }
}

public enum Rotation {
    NE,
    SE = 90,
    SO = 180,
    NO = 270,
    NULL
}

public enum ActionState {
    Waiting,
    LookingForMovement,
    Moving,
    DisplayingRange,
    DisplayingAoe,
    PlayingAnimation,
    EndTurn
}