using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingManager : MonoBehaviour {

    public ConfirmationWindow   ConfirmationWindow;

    public MapInfos             MapInfos { private get; set; }
    public ProjectionManager    ProjectionManager { private get; set; }
    public Unit                 CurrentUnit { private get; set; }

    private Cell                Cell;
    private Cell                TargetCell;

    void Start () {
    }
	
	void Update () {

        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
                if (hit.collider != null) {

                    Cell = MapInfos.Map[(int)hit.collider.transform.position.x, (int)hit.collider.transform.position.z];

                    switch (CurrentUnit.ActionState) {

                        case ActionState.Waiting:
                            if (Cell.IsTaken != null && ProjectionManager.SelectionState != SelectionState.OtherAvailableMovement) {
                                ProjectionManager.SelectAvailableMovement(Cell, Cell.IsTaken.AttributesSheet.GetAttribute(AttributeType.MovementPoint), SelectionState.OtherAvailableMovement);
                            }
                            break;

                        case ActionState.LookingForMovement:
                            int dist = Utils.GetDistanceToTarget(CurrentUnit, hit.collider.transform.position);
                            if (dist <= CurrentUnit.AttributesSheet.GetAttribute(AttributeType.MovementPoint) && ProjectionManager.Selection.Contains(MapInfos.ProjectionMap[Cell.X, Cell.Z]))
                                CurrentUnit.Move(Cell, true, dist);
                            break;

                        case ActionState.DisplayingRange:
                            if (ProjectionManager.Selection.Contains(MapInfos.ProjectionMap[Cell.X, Cell.Z])) {
                                CurrentUnit.UpdateRotation(Cell);
                                CurrentUnit.SkillDisplayAoe(Cell);
                                TargetCell = Cell;
                                ConfirmationWindow.CellToFollow = MapInfos.ProjectionMap[TargetCell.X, TargetCell.Z];
                                ConfirmationWindow.SetActive(true);
                            } else {
                                CurrentUnit.SetState(ActionState.Waiting, false);
                            }
                            break;
                    }
                }
        } else if (CurrentUnit != null && CurrentUnit.ActionState == ActionState.DisplayingAoe) {

            if (ConfirmationWindow.ConfirmationStatus == ConfirmationStatus.Valid) {
                CurrentUnit.SkillUse(TargetCell);
                ConfirmationWindow.SetActive(false);
            } else if (ConfirmationWindow.ConfirmationStatus == ConfirmationStatus.Cancel) {
                CurrentUnit.SetState(ActionState.DisplayingRange, false);
                CurrentUnit.SkillForceDisplayRange();
                ConfirmationWindow.SetActive(false);
            }

        } else if (CurrentUnit != null && CurrentUnit.ActionState == ActionState.EndTurn && ConfirmationWindow.enabled) {
            ConfirmationWindow.SetActive(false);
        } else if (ProjectionManager != null && ProjectionManager.SelectionState == SelectionState.OtherAvailableMovement)
            ProjectionManager.SelectionState = SelectionState.Null;
    }
}
