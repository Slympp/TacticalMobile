using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapMoveButtonManager : MonoBehaviour {

    public Button MoveLeft;
    public Button MoveRight;
    public Button MoveDown;
    public Button MoveUp;

    private WorldMapManager WorldMapManager;
    private WorldMapData    WorldMapData;

    public void Awake() {
        WorldMapManager = GetComponent<WorldMapManager>();
        WorldMapData = WorldMapManager.WorldMapData;
    }

    public void UpdateButtons() {

        MoveLeft.interactable = (WorldMapData.CurrentPos.x - 1 >= 0) ? true : false;
        MoveRight.interactable = (WorldMapData.CurrentPos.x + 1 < WorldMapData.Size) ? true : false;
        MoveDown.interactable = (WorldMapData.CurrentPos.y - 1 >= 0) ? true : false;
        MoveUp.interactable = (WorldMapData.CurrentPos.y + 1 < WorldMapData.Size) ? true : false;
    }

    public void Move(int moveDirection) {

        switch ((MoveDirection)moveDirection) {
            case MoveDirection.Left:
                WorldMapManager.Move(-1, 0);
                break;
            case MoveDirection.Right:
                WorldMapManager.Move(1, 0);
                break;
            case MoveDirection.Down:
                WorldMapManager.Move(0, -1);
                break;
            case MoveDirection.Up:
                WorldMapManager.Move(0, 1);
                break;

            default:
                break;
        }
    }
}

public enum MoveDirection {
    Left = 0,
    Right,
    Down,
    Up
}