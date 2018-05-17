using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    public GameObject       ActionsButtons;
    public Button           NewGameButton;
    public Button           LoadGameButton;
    public Button           AchievementsButton;

    public GameObject       NewGameConfirmationPopup;
    public Button           NewGameConfirmationPopupValid;
    public Button           NewGameConfirmationPopupCancel;

	void Start () {

        Application.targetFrameRate = 60;

        NewGameConfirmationPopupValid.onClick.AddListener(CreateNewGame);
        NewGameConfirmationPopupCancel.onClick.AddListener(() => ToggleNewGameConfirmationPopup(false));

        bool worldMapSaveExist = false;
        if ((worldMapSaveExist = SaveManager.CheckValidFileSave(Scene.WorldMap))) {
            NewGameButton.onClick.AddListener(() => ToggleNewGameConfirmationPopup(true));
        } else
            NewGameButton.onClick.AddListener(CreateNewGame);

        if ((LoadGameButton.interactable = SaveManager.CheckValidFileSave(Scene.Battleground))) {
            LoadGameButton.onClick.AddListener(() => SaveManager.LoadScene(Scene.Battleground));
        } else if ((LoadGameButton.interactable = worldMapSaveExist)) {
            LoadGameButton.onClick.AddListener(() => SaveManager.LoadScene(Scene.WorldMap));
        }
	}

    private void CreateNewGame() {
        SaveManager.LoadScene(Scene.CharacterCreation, true);
    }

    private void ToggleNewGameConfirmationPopup(bool b) {
        ActionsButtons.SetActive(!b);
        NewGameConfirmationPopup.SetActive(b);
    }
}
