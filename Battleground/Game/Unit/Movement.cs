using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public string           AnimationPrefix { private get; set; } = "";
    public CombatUIManager  CombatUIManager { private get; set; }

    private Unit            Unit;

    private int             WaypointId;
    private List<Cell>      Path;
    private Cell            CurrentWaypoint;
    private Cell            NextWaypoint;

    private float           MoveSpeed = 4f;

    private float           JumpMaxTime = 0.6f;
    private float           JumpProgress = 0f;
    private IEnumerator     ActiveJumpCoroutine;


    private void Start() {
        Unit = GetComponent<Unit>();
    }

    void Update () {
		
        if (Unit.ActionState == ActionState.Moving) {

            transform.LookAt(new Vector3(NextWaypoint.X, transform.position.y, NextWaypoint.Z));

            if (CurrentWaypoint.Y != NextWaypoint.Y)
                Jump(NextWaypoint.GetVector3Position(), 1.25f, JumpMaxTime);
            else
                MoveTowards();

            if (transform.position == NextWaypoint.GetVector3Position()) {
                CurrentWaypoint = NextWaypoint;

                if (WaypointId != Path.Count - 1)
                    NextWaypoint = Path[++WaypointId];
                else
                    EndMovement();
            }
        }
    }

    private void MoveTowards() {
        Unit.Animator.SetBool(AnimationPrefix + "Run", true);
        transform.position = Vector3.MoveTowards(transform.position, NextWaypoint.GetVector3Position(), MoveSpeed * Time.smoothDeltaTime);
    }

    public void Jump(Vector3 destination, float maxHeight, float time) {
        if (ActiveJumpCoroutine == null) {
            JumpProgress = 0f;
            ActiveJumpCoroutine = JumpCoroutine(destination, maxHeight, time);
            StartCoroutine(ActiveJumpCoroutine);
        }
    }

    private IEnumerator JumpCoroutine(Vector3 destination, float maxHeight, float time) {
        var startPos = transform.position;
        Unit.Animator.SetBool(AnimationPrefix + "Run", false);
        Unit.Animator.Play(AnimationPrefix + "Jump Without Root Motion");
        while (JumpProgress <= 1.0f) {
            JumpProgress += Time.deltaTime / time;
            float height = Mathf.Sin(Mathf.PI * JumpProgress) * maxHeight;

            if (height < 0f)
                height = 0f;

            transform.position = Vector3.Lerp(startPos, destination, JumpProgress) + Vector3.up * height;
            yield return null;
        }
        transform.position = destination;
        ActiveJumpCoroutine = null;
        yield break;
    }

    public void StartMovement(List<Cell> path, Cell currentCell) {

        if (path != null) {
            this.Path = path;
            WaypointId = 0;
            CurrentWaypoint = currentCell;
            NextWaypoint = path[WaypointId];
            Unit.ActionState = ActionState.Moving;
            CombatUIManager.SetActiveAllButtons(false, 0);
        }
    }

    private void EndMovement() {

        Unit.Animator.SetBool(AnimationPrefix + "Run", false);
        Path = null;
        WaypointId = 0;
        CurrentWaypoint = null;
        NextWaypoint = null;
        Unit.ActionState = ActionState.Waiting;
        Unit.UpdateRotation(null);
        CombatUIManager.SetActiveAllButtons(true, Unit.AttributesSheet.GetAttribute(AttributeType.MovementPoint));
        CombatUIManager.UpdateSkillsButtons();
    }
}
