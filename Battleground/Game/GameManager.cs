using LlockhamIndustries.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MapManager))]
public class GameManager : MonoBehaviour {

    public GameObject           UnitProjectionTemplate;

    public GameObject           Canvas;
    public GameObject           HealthSliderTemplate;

    private List<Unit>          InitiativeList;

    private MapManager          MapManager;
    private ProjectionManager   ProjectionManager;
    private CameraManager       CameraManager;
    private CombatUIManager     CombatUIManager;
    private TargetingManager    TargetingManager;

    private BattlegroundData    BattlegroundData;

    void Awake () {

        MapManager = GetComponent<MapManager>();
        CameraManager = GetComponent<CameraManager>();
        ProjectionManager = GetComponent<ProjectionManager>();
        CombatUIManager = GameObject.Find("CombatUI").GetComponent<CombatUIManager>();
        TargetingManager = GetComponent<TargetingManager>();

        if (SaveManager.CheckValidFileSave(Scene.Battleground)) {
            BattlegroundData = SaveManager.LoadBattleground();
        }

        MapManager.GenerateMaps(BattlegroundData.MapInfos);
	}

    void Start() {

        LoadGame();

        if (BattlegroundData.Player.Count > 0 && BattlegroundData.Enemies.Count > 0)
            StartCoroutine(StartGame());
    }

    private void OnApplicationQuit() {

        SaveManager.SaveBattleground(BattlegroundData);
    }

    public void LoadGame() {

        ProjectionManager.MapInfos = BattlegroundData.MapInfos;
        TargetingManager.MapInfos = BattlegroundData.MapInfos;
        TargetingManager.ProjectionManager = ProjectionManager;

        int unitId = 0;
        
        
        LoadTeam(BattlegroundData.Player, Team.Player, ref unitId);
        LoadTeam(BattlegroundData.Enemies, Team.Enemies, ref unitId);

        CameraManager.InitBackground(BattlegroundData.MapInfos.MapData.Background);
        CameraManager.CenterMainCamera(new Vector3((BattlegroundData.MapInfos.Size.x / 2), 0, (BattlegroundData.MapInfos.Size.y / 2)));
        CameraManager.CenterTacticalCamera(new Vector3((BattlegroundData.MapInfos.Size.x / 2), 0, (BattlegroundData.MapInfos.Size.y / 2)));
    }

    public void EndGame(Team lastTeamAlive) {

        if (lastTeamAlive == Team.Player) {
            // TODO: 
            // 1 - Update WorldMapSave with Players from the combat
            // 2 - Delete BattlegroundSave

            SaveManager.DeleteBattlegroundSave();
            SaveManager.LoadScene(Scene.WorldMap);
            
        } else {
            //Display GameOver screen
            SaveManager.LoadScene(Scene.MainMenu, true);
        }
    }

    private void LoadTeam<T>(List<T> team, Team teamName, ref int unitId) where T : Unit {

        GameObject teamObject = new GameObject(teamName.ToString());
        teamObject.transform.parent = this.transform;

        for (int i = 0; i < team.Count; i++) {
            team[i].transform.parent = teamObject.transform;
            team[i].CombatUIManager = CombatUIManager;
            team[i].ProjectionManager = ProjectionManager;
            team[i].ConfirmationWindow = TargetingManager.ConfirmationWindow;
            team[i].MapInfos = BattlegroundData.MapInfos;

            team[i].LoadModel();

            team[i].Init();
            team[i].Id = unitId++;

            InstantiateSelfProjection(team[i], ((int)teamName == 0) ? Color.blue : Color.red);
            team[i].HealthSlider = InstantiateHealthSlider(team[i].transform);
            team[i].HealthSlider.value = team[i].AttributesSheet.CurrentHealth;
            CameraManager.HealthSliders.Add(team[i].HealthSlider.transform.parent.GetComponent<UIAnchorTransform>());

            SetPosition(team[i]);
            SetRotation(team[i]);

            if (!team[i].IsAlive) {
                team[i].gameObject.SetActive(false);
                team[i].HealthSlider.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void SetPosition(Unit unit) {

        if (unit.CurrentCell == null || unit.CurrentCell.IsTaken == null) {
            int x, z;
            int minY = (((int)BattlegroundData.MapInfos.Size.y / 2) * (int)unit.Team + 2);
            int maxY = (((int)BattlegroundData.MapInfos.Size.y / 2) * (1 + (int)unit.Team)) - 2;
            do {
                x = (int)Random.Range(2, BattlegroundData.MapInfos.Size.x - 2);
                z = (int)Random.Range(minY, maxY);
            } while (BattlegroundData.MapInfos.Map[x, z].IsWalkable == false || BattlegroundData.MapInfos.Map[x, z].IsTaken != null);

            unit.Move(BattlegroundData.MapInfos.Map[x, z], false, 0);
        } else
            unit.Move(unit.CurrentCell, false, 0);
    }

    private void SetRotation(Unit unit) {

        if (unit.CurrentRotation == Rotation.NULL)
            unit.CurrentRotation = (unit.Team == Team.Player ? Rotation.NE : Rotation.SO);

        unit.transform.eulerAngles = new Vector3(0, (float)unit.CurrentRotation, 0);
        unit.UpdateRotation(null);
    }
    
    // TODO: move to CombatUIManager
    private void InstantiateSelfProjection(Unit unit, Color c) {

        GameObject go = Instantiate(UnitProjectionTemplate, unit.transform);
        unit.SelfProjection = go.GetComponent<ProjectionRenderer>();
        unit.SelfProjection.SetColor(0, c);
        unit.SelfProjection.UpdateProperties();
    }

    // TODO: move to CombatUIManager
    private Slider InstantiateHealthSlider(Transform unit) {

        GameObject go = Instantiate(HealthSliderTemplate, Canvas.transform.Find("HealthSliders").transform);
        UIAnchorTransform anchor = go.GetComponent<UIAnchorTransform>();
        anchor.myCanvas = Canvas.GetComponent<RectTransform>();
        anchor.objectToFollow = unit;
        return go.transform.Find("HealthBar").GetComponent<Slider>();
    }

    private IEnumerator StartGame() {

        Team    teamAlive = Team.All;
        int     turn = 1;
        int     i = BattlegroundData.CurrentUnitId;

        while (teamAlive == Team.All) {

            InitiativeList = GetInitiativeList();

            while (i < InitiativeList.Count) {

                if (!InitiativeList[i].gameObject.activeSelf || !InitiativeList[i].IsAlive) {
                    i++;
                    continue;
                }

                if ((teamAlive = CheckIfTeamIsDead()) != Team.All) {
                    Debug.Log("Fin du game, " + teamAlive.ToString() + " win");
                    EndGame(teamAlive);
                    yield return null;
                }

                BattlegroundData.CurrentUnitId = i;

                CombatUIManager.CurrentUnit = InitiativeList[i];
                TargetingManager.CurrentUnit = InitiativeList[i];

                CameraManager.CenterMainCamera(InitiativeList[i].CurrentCell.GetVector3Position());
                CameraManager.CenterTacticalCamera(InitiativeList[i].CurrentCell.GetVector3Position());
                CameraManager.FollowedUnit = InitiativeList[i];

                yield return StartCoroutine(InitiativeList[i].Turn());
                i++;
            }
            i = 0;
            turn++;
        }
        yield return null;
    }

    private Team CheckIfTeamIsDead() {

        bool a = true, b = true;

        foreach (Unit unit in BattlegroundData.Player) {
            if (unit.gameObject.activeSelf && unit.IsAlive) a = false;
        }
        foreach (Unit unit in BattlegroundData.Enemies) {
            if (unit.gameObject.activeSelf && unit.IsAlive) b = false;
        }

        if (a && b) return Team.None;
        else if (a) return Team.Enemies;
        else if (b) return Team.Player;
        else return Team.All;
    }

    private List<Unit> GetInitiativeList() {
        List<Unit> list = new List<Unit>();

        for (int i = 0; i < BattlegroundData.Player.Count; i++)
            if (BattlegroundData.Player[i].gameObject.activeSelf)
                list.Add(BattlegroundData.Player[i]);
        for (int i = 0; i < BattlegroundData.Enemies.Count; i++)
            if (BattlegroundData.Enemies[i].gameObject.activeSelf)
                list.Add(BattlegroundData.Enemies[i]);

        list.Sort((p1, p2) => p1.AttributesSheet.GetAttribute(AttributeType.Initiative).CompareTo(p2.AttributesSheet.GetAttribute(AttributeType.Initiative)));
        list.Reverse();

        return list;
    }
}

public enum Team {
    Player,
    Enemies,
    All,
    None
}