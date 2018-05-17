using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour {

    // Testing purpose only
    public bool                         SkipBattlegrounds;             

    public GameObject                   WorldMapUI;
    public WorldMapData                 WorldMapData;

    private WorldMapBuilder             WorldMapBuilder;
    private WorldMapMoveButtonManager   WorldMapMoveButtonManager;
    private WorldMapInventoryManager    WorldMapInventoryManager;
    private WorldMapTeamManager         WorldMapTeamManager;

	void Awake () {

        if (SaveManager.CheckValidFileSave(Scene.WorldMap))
            WorldMapData = SaveManager.LoadWorldMap();
        else // Debug purpose only
            WorldMapData = new WorldMapData();

        WorldMapMoveButtonManager = GetComponent<WorldMapMoveButtonManager>();
        WorldMapBuilder = GetComponent<WorldMapBuilder>();
        WorldMapInventoryManager = GetComponent<WorldMapInventoryManager>();
        WorldMapTeamManager = GetComponent<WorldMapTeamManager>();
    }

    void Start() {
        WorldMapBuilder.Generate(WorldMapData);
        WorldMapMoveButtonManager.UpdateButtons();
    }

    public void OnApplicationQuit() {
        SaveManager.SaveWorldMap(WorldMapData);
    }

    public void Move(int x, int y) {

        int newX = (int)WorldMapData.CurrentPos.x + x;
        int newY = (int)WorldMapData.CurrentPos.y + y;
        WorldMapData.CurrentPos = new Vector2(newX, newY);
        WorldMapBuilder.UpdateDiscoveredMap(WorldMapData, newX, newY);

        switch (WorldMapData.WorldMapInfos[newX, newY]) {

            case WorldMapCellType.Battleground:
                WorldMapData.WorldMapInfos[newX, newY] = WorldMapCellType.Empty;
                GenerateBattleground(WorldMapBuilder.WorldMap[newX, newY]);
                break;

            case WorldMapCellType.Chest:
                WorldMapData.WorldMapInfos[newX, newY] = WorldMapCellType.Empty;
                GenerateChest();
                break;

            case WorldMapCellType.NewTeamMember:
                WorldMapData.WorldMapInfos[newX, newY] = WorldMapCellType.Empty;
                GenerateNewTeamMember();
                break;

            default:
                break;
        }

        if (WorldMapData.WorldMapInfos[newX, newY] != WorldMapCellType.City)
            WorldMapData.WorldMapInfos[newX, newY] = WorldMapCellType.Empty;

        WorldMapBuilder.Generate(WorldMapData);
        WorldMapMoveButtonManager.UpdateButtons();
    }

    public void GenerateBattleground(MapDataType mapDataType) {

        Debug.Log("Battleground found!");

        if (SkipBattlegrounds)
            return;

        BattlegroundData newBattleGround = new BattlegroundData();

        newBattleGround.Player = WorldMapData.Player;

        newBattleGround.Enemies = new List<Enemy>();
        newBattleGround.Enemies.Add(Resources.Load<GameObject>("Testing/EnemyKnight").GetComponent<Enemy>());
        newBattleGround.Enemies.Add(Resources.Load<GameObject>("Testing/EnemyRogue").GetComponent<Enemy>());
        newBattleGround.Enemies.Add(Resources.Load<GameObject>("Testing/EnemyWizard").GetComponent<Enemy>());
        
        newBattleGround.CurrentUnitId = 0;
        newBattleGround.MapInfos = new MapInfos();
        newBattleGround.MapInfos.Size = new Vector2(32, 32);

        newBattleGround.MapInfos.MapData = Resources.Load<ScriptableObject>("MapData/" + mapDataType.ToString()) as MapData;

        SaveManager.SaveWorldMap(WorldMapData);
        SaveManager.SaveBattleground(newBattleGround);

        SaveManager.LoadScene(Scene.Battleground);
    }

    public void GenerateChest() {

        Debug.Log("Chest found!");
    }

    public void GenerateNewTeamMember() {

        Debug.Log("NewTeamMember found!");
    }

    public void ToggleInventory(bool b) {

        WorldMapInventoryManager.Toggle(b);
        WorldMapUI.SetActive(!b);
        WorldMapBuilder.MapObject.SetActive(!b);
    }

    public void ToggleTeamManagement(bool b) {

        WorldMapTeamManager.Toggle(b);
        WorldMapUI.SetActive(!b);
        WorldMapBuilder.MapObject.SetActive(!b);
    }
}

