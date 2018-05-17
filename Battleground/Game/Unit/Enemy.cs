using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {

    private AITurn AITurn;

    public override IEnumerator Turn() {

        InitTurn();
        Debug.Log("(Enemy) " + UnitName + " turn");

        CombatUIManager.RefreshUI();
        CombatUIManager.UpdateSkillsButtons();
        CombatUIManager.SetActiveAllButtons(false, 0);

        ProjectionManager.SelectCurrentCell(CurrentCell, Team);

        // GET MOST EFFICIENT TURN
        CombatUIManager.ToggleThinkingAI(true);
        yield return GetTurn();
        CombatUIManager.ToggleThinkingAI(false);

        // PLAY TURN
        if (AITurn != null && AITurn.Actions != null) {
           foreach(AIAction action in AITurn.Actions) {

                if (action is AIActionMovement) {
                    AIActionMovement movementAction = action as AIActionMovement;
                    Cell cellDestination = MapInfos.Map[(int)movementAction.Destination.x, (int)movementAction.Destination.y];
                    Debug.Log("Move from [" + CurrentCell.X + ", " + CurrentCell.Z + "] to [" +
                                              cellDestination.X + ", " + cellDestination.Z + "]");

                    // ProjectionManager.SelectAvailableMovement();
                    // yield return new WaitForSeconds(0.5f);

                    Move(cellDestination, true, Utils.GetDistanceBetweenCells(CurrentCell, cellDestination));
                    while (ActionState == ActionState.Moving)
                        yield return new WaitForEndOfFrame();

                } else if (action is AIActionUseSkill) {
                    AIActionUseSkill useSkillAction = action as AIActionUseSkill;
                    Debug.Log("Use " + useSkillAction.SpellId + " at [" + useSkillAction.SpellTarget.X + ", " +
                                                                          useSkillAction.SpellTarget.Z + "]");

                    SkillDisplayRange(useSkillAction.SpellId);
                    yield return new WaitForSeconds(0.5f);
                    SkillDisplayAoe(useSkillAction.SpellTarget);
                    yield return new WaitForSeconds(0.5f);

                    SkillUse(useSkillAction.SpellTarget);
                    while (ActionState == ActionState.PlayingAnimation)
                        yield return new WaitForEndOfFrame();
                }
            }
        }

        // END TURN
        AttributesSheet.RemoveCosts();
        ProjectionManager.ClearProjections();
        SelfProjection.gameObject.SetActive(true);
        IsPlaying = false;
        yield return null;
    }

    private IEnumerator GetTurn() {

        // Compare all availables turns
        // Get the most efficient one

        //AITurn = ComputeTurn();

        yield return new WaitForSeconds(2f);

        yield return null;
    }

    private AITurn ComputeTurn() {

        float[,] influenceMap = InfluenceMap.GetInfluenceMap(MapInfos, Team.Player);

        InfluenceMapDrawer drawer = GameObject.Find("InfluenceMapDrawer").GetComponent<InfluenceMapDrawer>();
        drawer.influenceMap = influenceMap;

        AITurn turn = new AITurn();
        turn.Actions = new List<AIAction>();

        AIActionMovement movement = new AIActionMovement();
        movement.GetHighestScore(influenceMap, CurrentCell.X, CurrentCell.Z, (int)MapInfos.Size.x, (int)MapInfos.Size.y, AttributesSheet.GetAttribute(AttributeType.MovementPoint));
        turn.Actions.Add(movement);

        /* while (enough AP to use a skill || enough MP to move) {

            // if (enough AP to use a skill) {

                - Get Available Skills

                if (health == low && has a healing skill)
                    Use healing skill
                else {
                    - Get potential targets in Range
                    - Get skill cellTarget
                    - Add potential kills to turn
                    - Add potential damage to turn
                }

            } else (Move) {
                GetInfluenceMap(Team.Player);
                AIActionMovement movement = new AIActionMovement();
                if (CurrentHealth > 50%) {
                    GetHighestScore();
                } else
                    GetLowestScore();
                turns.Action.Add(movement);
            }
        */
        return turn;
    }
}
