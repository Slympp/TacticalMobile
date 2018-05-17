using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public override IEnumerator Turn() {

        InitTurn();
        Debug.Log("(Player) " + UnitName + " turn");

        CombatUIManager.RefreshUI();
        CombatUIManager.SetActiveAllButtons(true, AttributesSheet.GetAttribute(AttributeType.MovementPoint));
        CombatUIManager.UpdateSkillsButtons();

        while (ActionState != ActionState.EndTurn && IsAlive) {

            switch (ActionState) {

                case ActionState.LookingForMovement:
                    if (AttributesSheet.GetAttribute(AttributeType.MovementPoint) != 0) {
                        if (ProjectionManager.SelectionState != SelectionState.AvailableMovement)
                            ProjectionManager.SelectAvailableMovement(CurrentCell, AttributesSheet.GetAttribute(AttributeType.MovementPoint), SelectionState.AvailableMovement);
                    } else {
                        Debug.Log("Can't move, no movementPoint available");
                        ActionState = ActionState.Waiting;
                    }
                    break;

                case ActionState.Waiting:
                    if (ProjectionManager.SelectionState != SelectionState.CurrentCell &&
                        ProjectionManager.SelectionState != SelectionState.OtherAvailableMovement)
                        ProjectionManager.SelectCurrentCell(CurrentCell, Team);
                    break;

                default:
                    break;
            }
            yield return new WaitForFixedUpdate();
        }
        AttributesSheet.RemoveCosts();
        ProjectionManager.ClearProjections();
        SelfProjection.gameObject.SetActive(true);
        IsPlaying = false;
        yield return null;
    }
}
