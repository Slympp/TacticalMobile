using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour {

    public Button[] SkillsButton;
    public Text[]   SkillsButtonText;
    public Image[]  SkillsButtonIcon;
    public Sprite   DefaultSkillButtonSprite;

    public Button   EndTurnButton;
    public Button   MovementButton;

    public Text     ActionPointsText;
    public Text     MovementPointsText;

    public Slider   HealthBar;
    public Text     HealthText;
    public Slider   ManaBar;
    public Text     ManaText;

    public bool             AIIsThinking { get; set; } = false;
    public RectTransform    ThinkingAIcon;

    public Unit     CurrentUnit { private get; set; }

    private void Update() {
        
        if (AIIsThinking) {
            ThinkingAIcon.Rotate(0f, 0f, 200 * Time.deltaTime);
        }
    }

    public void ToggleThinkingAI(bool b) {

        AIIsThinking = b;
        ThinkingAIcon.transform.parent.gameObject.SetActive(b);
    }

    public void RefreshUI() {

        UpdateHealthBar();
        UpdateManaBar();

        for (int i = 0; i < SkillsButton.Length; i++) {
            int tempInt = i;
            SkillsButton[i].onClick.RemoveAllListeners();
            SkillsButton[i].onClick.AddListener(() => CurrentUnit.SkillDisplayRange(tempInt));
        }

        MovementButton.onClick.RemoveAllListeners();
        MovementButton.onClick.AddListener(() => CurrentUnit.SetState(ActionState.LookingForMovement, true));
        EndTurnButton.onClick.RemoveAllListeners();
        EndTurnButton.onClick.AddListener(() => CurrentUnit.SetState(ActionState.EndTurn, false));
    }

    public void UpdateSkillsButtons(bool forceNotInteractable = false) {

        int availableAP = CurrentUnit.AttributesSheet.GetAttribute(AttributeType.ActionPoint);
        int availableMP = CurrentUnit.AttributesSheet.GetAttribute(AttributeType.MovementPoint);

        for (int i = 0; i < SkillsButton.Length; i++) {

            SkillBase skill = null;
            if (i < CurrentUnit.Skills.Count)
                skill = CurrentUnit.GetSkill(i);

            if (skill != null) {
                SkillsButtonIcon[i].sprite = skill.Icon;

                if (skill.CostType == CostType.ActionPoint)
                    SkillsButton[i].interactable = (availableAP >= skill.Cost) ? true : false;
                else if (skill.CostType == CostType.MovementPoint)
                    SkillsButton[i].interactable = (availableMP >= skill.Cost) ? true : false;

                if (skill.ManaCost > CurrentUnit.AttributesSheet.CurrentMana)
                    SkillsButton[i].interactable = false;

                if (skill.Cooldown > 0 && CurrentUnit.SkillsCooldowns[i] > 0) {
                    SkillsButton[i].interactable = false;
                    SkillsButtonText[i].text = CurrentUnit.SkillsCooldowns[i].ToString();
                } else
                    SkillsButtonText[i].text = "";

                if (skill.UsePerTurn != -1 && CurrentUnit.SkillsUses[i] == 0)
                    SkillsButton[i].interactable = false;

                if (forceNotInteractable)
                    SkillsButton[i].interactable = false;

            } else {
                SkillsButton[i].interactable = false;
                SkillsButton[i].onClick.RemoveAllListeners();
                SkillsButtonIcon[i].sprite = DefaultSkillButtonSprite;
            }
        }
    }

    public void UpdateHealthBar() {

        HealthText.text = CurrentUnit.AttributesSheet.CurrentHealth.ToString() + " / " + CurrentUnit.AttributesSheet.MaxHealth.ToString();
        HealthBar.value = CurrentUnit.AttributesSheet.CurrentHealth * 100 / CurrentUnit.AttributesSheet.MaxHealth;
    }

    public void UpdateManaBar() {

        ManaText.text = CurrentUnit.AttributesSheet.CurrentMana.ToString() + " / " + CurrentUnit.AttributesSheet.MaxMana.ToString();
        ManaBar.value = CurrentUnit.AttributesSheet.CurrentMana * 100 / CurrentUnit.AttributesSheet.MaxMana;
    }

    public void UpdateActionPoint(int AP) {

        ActionPointsText.text = AP.ToString();
    }

    public void UpdateMovementPoint(int MP) {

        MovementButton.interactable = (MP <= 0 ? false : true);
        MovementPointsText.text = MP.ToString();
    }

    public void SetActiveAllButtons(bool active, int movementPoints) {

        if (active)
            MovementButton.interactable = (movementPoints <= 0 ? false : true);
        else
            MovementButton.interactable = false;

        EndTurnButton.interactable = active;
        for (int i = 0; i < SkillsButton.Length; i++)
            SkillsButton[i].interactable = active;
    }
}
    
