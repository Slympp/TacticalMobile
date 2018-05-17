using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LlockhamIndustries.Decals;

public class ProjectionManager : MonoBehaviour {

    [HideInInspector]
    public SelectionState               SelectionState  = SelectionState.Null;

    public List<ProjectionRenderer>     Selection { get; private set; }
    public List<ProjectionRenderer>     NotInLOS { get; private set; }
    public List<Cell>                   AreaOfEffect { get; private set; }
    public MapInfos                     MapInfos { private get; set; }

    //private List<ProjectionRenderer>    OutLOSSelection;
    private List<Cell>                  PathFindingResult;

	void Start () {
	}

    public void SelectCurrentCell(Cell currentCell, Team team) {

        ClearProjections();
        SelectCells(RangeType.Single, currentCell, 0, 0, false, false, false, false, false, SelectionState.CurrentCell);
        ColorProjections(Selection, GetColorFromTeam(team));
        EnableProjections(Selection, true);
    }

    public void SelectAvailableMovement(Cell currentCell, int movementPoint, SelectionState state) {

        ClearProjections();
        SelectCells(RangeType.Circle, currentCell, 1, movementPoint, false, true, true, false, false, state);
        ColorProjections(Selection, Color.green);
        EnableProjections(Selection, true);
    }

    public void SelectPath(List<Cell> path) {

        ClearProjections();
        foreach (Cell cell in path)
            SelectCell(cell, null, true, false, false);
        ColorProjections(Selection, Color.green);
        EnableProjections(Selection, true);
    }

    public void SelectCells(RangeType range, Cell origin, int minRange, int maxRange,
                            bool lineOfSight, bool mustBeEmpty, bool needPath, bool exceptLastCell,
                            bool addCellToAoe,
                            SelectionState state,
                            RelativeRotation relativeOrientation = RelativeRotation.None,
                            Rotation orientation = Rotation.NULL) {

        if (Selection == null)
            Selection = new List<ProjectionRenderer>();
        if (lineOfSight && NotInLOS == null)
            NotInLOS = new List<ProjectionRenderer>();
        if (addCellToAoe && AreaOfEffect == null)
            AreaOfEffect = new List<Cell>();

        switch (range) {
            case RangeType.Square:
                SelectSquare(origin, minRange, maxRange, lineOfSight, mustBeEmpty, needPath, exceptLastCell, addCellToAoe);
                break;

            case RangeType.Circle:
                SelectCircle(origin, minRange, maxRange, lineOfSight, mustBeEmpty, needPath, exceptLastCell, addCellToAoe);
                break;

            case RangeType.Line:
                List<Rotation> lineOriList = GetLineOrientationFromRelative(relativeOrientation, orientation);
                foreach (Rotation ori in lineOriList)
                    SelectLine(origin, minRange, maxRange, lineOfSight, mustBeEmpty, needPath, exceptLastCell, addCellToAoe, ori);
                break;

            case RangeType.Diagonal:
                List<Rotation> diagOriList = GetDiagOrientationFromRelative(relativeOrientation, orientation);
                foreach (Rotation ori in diagOriList)
                    SelectDiagonal(origin, minRange, maxRange, lineOfSight, mustBeEmpty, needPath, exceptLastCell, addCellToAoe, ori);
                break;

            case RangeType.Single:
                SelectCell(origin, null, mustBeEmpty, false, addCellToAoe);
                break;
        }
        SelectionState = state;
    }

    private void SelectCircle(Cell origin, int minRange, int maxRange, bool lineOfSight, bool mustBeEmpty, bool needPath, bool exceptLastCell, bool addCellToAoe) {

        for (int x = -maxRange; x <= maxRange; x++)
            for (int z = -maxRange; z <= maxRange; z++) {

                    if ((Mathf.Abs(x + z)) <= maxRange &&
                        (Mathf.Abs(x) + Mathf.Abs(z) >= minRange) &&
                        (Mathf.Abs(x) + Mathf.Abs(z) <= maxRange) &&
                        origin.X + x >= 0 && origin.X + x < MapInfos.Size.x &&
                        origin.Z + z >= 0 && origin.Z + z < MapInfos.Size.y) {

                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + x, origin.Z + z], exceptLastCell)) == null) || PathFindingResult.Count > maxRange /*origin.GetDistanceTo(MapInfos.Map[origin.X + x, origin.Z + z])*/))
                            continue;
                        SelectCell(MapInfos.Map[origin.X + x, origin.Z + z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
            }
    }

    private void SelectSquare(Cell origin, int minRange, int maxRange, bool lineOfSight, bool mustBeEmpty, bool needPath, bool exceptLastCell, bool addCellToAoe) {

        int absX;
        int absZ;
        for (int x = -maxRange; x <= maxRange; x++)
            for (int z = -maxRange; z <= maxRange; z++) {

                absX = Mathf.Abs(x);
                absZ = Mathf.Abs(z);
                if (((absX <= maxRange && absX >= minRange) || (absZ <= maxRange && absZ >= minRange)) &&
                        origin.X + x >= 0 && origin.X + x < MapInfos.Size.x &&
                        origin.Z + z >= 0 && origin.Z + z < MapInfos.Size.y) {

                    if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + x, origin.Z + z], exceptLastCell)) == null) || PathFindingResult.Count > origin.GetDistanceTo(MapInfos.Map[origin.X + x, origin.Z + z])))
                        continue;
                    SelectCell(MapInfos.Map[origin.X + x, origin.Z + z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                }
            }
    }

    private void SelectDiagonal(Cell origin, int minRange, int maxRange, bool lineOfSight, bool mustBeEmpty, bool needPath, bool exceptLastCell, bool addCellToAoe, Rotation orientation) {

        switch (orientation) {
            case Rotation.NE:
                for (int i = minRange; i <= maxRange; i++) {
                    if (origin.X + i >= 0 && origin.X + i < MapInfos.Size.x &&
                        origin.Z + i >= 0 && origin.Z + i < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + i, origin.Z + i], exceptLastCell)) == null) || PathFindingResult.Count > maxRange * 2))
                            continue;
                        SelectCell(MapInfos.Map[origin.X + i, origin.Z + i], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.SO:
                for (int i = minRange; i <= maxRange; i++) {
                    if (origin.X - i >= 0 && origin.X - i < MapInfos.Size.x &&
                        origin.Z - i >= 0 && origin.Z - i < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X - i, origin.Z - i], exceptLastCell)) == null) || PathFindingResult.Count > maxRange * 2))
                            continue;
                        SelectCell(MapInfos.Map[origin.X - i, origin.Z - i], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.NO:
                for (int i = minRange; i <= maxRange; i++) {
                    if (origin.X - i >= 0 && origin.X - i < MapInfos.Size.x &&
                        origin.Z + i >= 0 && origin.Z + i < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X - i, origin.Z + i], exceptLastCell)) == null) || PathFindingResult.Count > maxRange * 2))
                            continue;
                        SelectCell(MapInfos.Map[origin.X - i, origin.Z + i], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.SE:
                for (int i = minRange; i <= maxRange; i++) {
                    if (origin.X + i >= 0 && origin.X + i < MapInfos.Size.x &&
                        origin.Z - i >= 0 && origin.Z - i < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + i, origin.Z - i], exceptLastCell)) == null) || PathFindingResult.Count > maxRange * 2))
                            continue;
                        SelectCell(MapInfos.Map[origin.X + i, origin.Z - i], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SelectLine(Cell origin, int minRange, int maxRange, bool lineOfSight, bool mustBeEmpty, bool needPath, bool exceptLastCell, bool addCellToAoe, Rotation orientation) {

        switch (orientation) {
            case Rotation.SE:
                for (int x = minRange; x <= maxRange; x++) {
                    if (origin.X + x >= 0 && origin.X + x < MapInfos.Size.x) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + x, origin.Z], exceptLastCell)) == null) || PathFindingResult.Count > maxRange))
                            continue;
                        SelectCell(MapInfos.Map[origin.X + x, origin.Z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.NE:
                for (int z = minRange; z <= maxRange; z++) {
                    if (origin.Z + z >= 0 && origin.Z + z < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X, origin.Z + z], exceptLastCell)) == null) || PathFindingResult.Count > maxRange))
                            continue;
                        SelectCell(MapInfos.Map[origin.X, origin.Z + z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.NO:
                for (int x = -maxRange; x <= -minRange; x++) {
                    if (origin.X + x >= 0 && origin.X + x < MapInfos.Size.x) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X + x, origin.Z], exceptLastCell)) == null) || PathFindingResult.Count > maxRange))
                            continue;
                        SelectCell(MapInfos.Map[origin.X + x, origin.Z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            case Rotation.SO:
                for (int z = -maxRange; z <= -minRange; z++) {
                    if (origin.Z + z >= 0 && origin.Z + z < MapInfos.Size.y) {
                        if (needPath && (((PathFindingResult = PathFinding.FindPath(MapInfos, origin, MapInfos.Map[origin.X, origin.Z + z], exceptLastCell)) == null) || PathFindingResult.Count > maxRange))
                            continue;
                        SelectCell(MapInfos.Map[origin.X, origin.Z + z], origin, mustBeEmpty, lineOfSight, addCellToAoe);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SelectCell(Cell cell, Cell origin, bool mustBeEmpty, bool lineOfSight, bool addCellToAoe) {

        if (cell.IsWalkable == true) {
            if ((mustBeEmpty && cell.IsTaken != null) || Selection.Contains(MapInfos.ProjectionMap[cell.X, cell.Z]))
                return;
            if (lineOfSight) {
                RaycastHit hit;
                Vector3 startPos = origin.GetVector3PositionSimple() + new Vector3(0, 0.5f, 0);
                Vector3 endPos = cell.GetVector3PositionSimple() + new Vector3(0, 0.5f, 0);
                float dist = Vector3.Distance(endPos, startPos);
                Physics.Raycast(startPos, endPos - startPos, out hit, dist);
                if (hit.collider != null) {
                    if (hit.collider.name != "Collider" || (hit.collider.name == "Collider" && hit.distance + 0.6f < dist)) {
                        NotInLOS.Add(MapInfos.ProjectionMap[cell.X, cell.Z]);
                        return;
                    }
                }
            }
            if (addCellToAoe)
                AreaOfEffect.Add(cell);
            Selection.Add(MapInfos.ProjectionMap[cell.X, cell.Z]);
        }
    }

    public void ClearProjections() {

        if (Selection != null) {
            EnableProjections(Selection, false);
            Selection.Clear();
            SelectionState = SelectionState.Null;
        }
        if (NotInLOS != null) {
            EnableProjections(NotInLOS, false);
            NotInLOS.Clear();
        }
        if (AreaOfEffect != null)
            AreaOfEffect.Clear();
    }

    public void EnableProjections(List<ProjectionRenderer> projectionList, bool b) {

        foreach (ProjectionRenderer projection in projectionList) {
            projection.enabled = b;
        }
    }

    public void ColorProjections(List<ProjectionRenderer> projectionList, Color c) {

        foreach (ProjectionRenderer projection in projectionList) {
            projection.SetColor(0, c);
            projection.UpdateProperties();
        }
    }

    public void ColorCell(int x, int z, Color c) {
        MapInfos.ProjectionMap[x, z].SetColor(0, c);
        MapInfos.ProjectionMap[x, z].UpdateProperties();
    }

    public Color GetColorFromTeam(Team team) {

        switch (team) {

            case Team.Player:
                return Color.blue;

            case Team.Enemies:
                return Color.red;

            default:
                return Color.white;
        }
    }

    public List<Rotation> GetLineOrientationFromRelative(RelativeRotation relativeOrientation, Rotation current) {

        List<Rotation> ret = new List<Rotation>();

        switch (relativeOrientation) {
            case RelativeRotation.Front:
                ret.Add(current);
                break;

            case RelativeRotation.Back:
                switch (current) {
                    case Rotation.NE:
                        ret.Add(Rotation.SO);
                        break;
                    case Rotation.SO:
                        ret.Add(Rotation.NE);
                        break;
                    case Rotation.NO:
                        ret.Add(Rotation.SE);
                        break;
                    case Rotation.SE:
                        ret.Add(Rotation.NO);
                        break;
                    default:
                        ret.Add(Rotation.NULL);
                        break;
                }
                break;

            case RelativeRotation.Sides:
                switch (current) {
                    case Rotation.NE:
                        ret.Add(Rotation.SE);
                        ret.Add(Rotation.NO);
                        break;
                    case Rotation.SO:
                        ret.Add(Rotation.SE);
                        ret.Add(Rotation.NO);
                        break;
                    case Rotation.NO:
                        ret.Add(Rotation.NE);
                        ret.Add(Rotation.SO);
                        break;
                    case Rotation.SE:
                        ret.Add(Rotation.SO);
                        ret.Add(Rotation.NE);
                        break;
                    default:
                        ret.Add(Rotation.NULL);
                        break;
                }
                break;

            case RelativeRotation.All:
                ret.Add(Rotation.NE);
                ret.Add(Rotation.NO);
                ret.Add(Rotation.SE);
                ret.Add(Rotation.SO);
                break;

            default:
                break;
        }
        return ret;
    }

    public List<Rotation> GetDiagOrientationFromRelative(RelativeRotation relativeOrientation, Rotation current) {

        List<Rotation> ret = new List<Rotation>();

        switch (relativeOrientation) {
            case RelativeRotation.Front:
                switch (current) {
                    case Rotation.NE:
                        ret.Add(Rotation.NE);
                        ret.Add(Rotation.NO);
                        break;
                    case Rotation.SO:
                        ret.Add(Rotation.SO);
                        ret.Add(Rotation.SE);
                        break;
                    case Rotation.NO:
                        ret.Add(Rotation.SO);
                        ret.Add(Rotation.NO);
                        break;
                    case Rotation.SE:
                        ret.Add(Rotation.SE);
                        ret.Add(Rotation.NE);
                        break;
                    default:
                        ret.Add(Rotation.NULL);
                        break;
                }
                break;

            case RelativeRotation.Back:
                switch (current) {
                    case Rotation.NE:
                        ret.Add(Rotation.SO);
                        ret.Add(Rotation.SE);
                        break;
                    case Rotation.SO:
                        ret.Add(Rotation.NO);
                        ret.Add(Rotation.NE);
                        break;
                    case Rotation.NO:
                        ret.Add(Rotation.SE);
                        ret.Add(Rotation.NE);
                        break;
                    case Rotation.SE:
                        ret.Add(Rotation.SO);
                        ret.Add(Rotation.NO);
                        break;
                    default:
                        ret.Add(Rotation.NULL);
                        break;
                }
                break;

            case RelativeRotation.All:
                ret.Add(Rotation.NE);
                ret.Add(Rotation.NO);
                ret.Add(Rotation.SE);
                ret.Add(Rotation.SO);
                break;

            default:
                break;
        }
        return ret;
    }
}

public enum RelativeRotation {
    None,
    All,
    Front,
    Sides,
    Back
}

public enum RangeType {
    Single,
    Circle,
    Line,
    Square,
    Diagonal,
}

public enum SelectionState {
    DisplaySkillRange,
    DisplaySkillAoe,
    AvailableMovement,
    OtherAvailableMovement,
    CurrentCell,
    Null
}