using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

    public static DatabasePreset    skillsDatabasePreset;
    public static DatabasePreset    itemsDatabasePreset;
    public static DatabasePreset    materialsDatabasePreset;

    private static string           BattlegroundSaveFile = "/battleground.dat";
    private static string           WorldMapSaveFile = "/worldmap.dat";

    private static bool             created = false;

    void Awake() {

        if (!created) {
            BattlegroundSaveFile = BattlegroundSaveFile.Insert(0, Application.persistentDataPath);
            WorldMapSaveFile = WorldMapSaveFile.Insert(0, Application.persistentDataPath);

            skillsDatabasePreset = Resources.Load<ScriptableObject>("Databases/SkillsPreset") as DatabasePreset;
            itemsDatabasePreset = Resources.Load<ScriptableObject>("Databases/ItemsPreset") as DatabasePreset;
            materialsDatabasePreset = Resources.Load<ScriptableObject>("Databases/MaterialsPreset") as DatabasePreset;

            DontDestroyOnLoad(transform.gameObject);
            created = true;
        } else {
            Debug.Log("SaveManager awake");
        }
    }

    public static void LoadScene(Scene scene, bool destroySaves = false) {

        if (destroySaves) {
            DeleteSaves();
        }

        SceneManager.LoadScene((int)scene);
    }

    public static void SaveWorldMap(WorldMapData worldMapData) {

        FileStream fs = File.Create(WorldMapSaveFile);
        BinaryFormatter bf = new BinaryFormatter();

        SerializedWorldMapData gameData = new SerializedWorldMapData();

        gameData.Player = DataSerialization.Serialize(worldMapData.Player);

        gameData.Seed = worldMapData.Seed;
        gameData.WorldMapInfos = worldMapData.WorldMapInfos;
        gameData.DiscoveredMap = worldMapData.DiscoveredMap;
        gameData.CurrentPosX = (int)worldMapData.CurrentPos.x;
        gameData.CurrentPosY = (int)worldMapData.CurrentPos.y;
        gameData.Size = worldMapData.Size;

        gameData.InventoryItemId = new List<string>();
        gameData.InventoryItemQuantity = new List<int>();
        foreach (Item item in worldMapData.Inventory) {
            gameData.InventoryItemId.Add(item.ItemId.ToString());
            gameData.InventoryItemQuantity.Add(item.Quantity);
        }

        bf.Serialize(fs, gameData);
        fs.Close();
    }

    public static WorldMapData LoadWorldMap() { 

        if (File.Exists(WorldMapSaveFile)) {

            FileStream fs = File.Open(WorldMapSaveFile, FileMode.Open);

            if (fs == null || fs.Length == 0) {
                Debug.Log("Failed to load WorldMapSaveFile (null or empty)");
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            SerializedWorldMapData gameData = (SerializedWorldMapData)bf.Deserialize(fs);
            fs.Close();

            WorldMapData worldMapData = new WorldMapData();
            Dictionary<string, string> skillsDb = DataSerialization.ReadDatabasePathById(skillsDatabasePreset);
            Dictionary<string, string> itemsDb = DataSerialization.ReadDatabasePathById(itemsDatabasePreset);

            worldMapData.Player = DataSerialization.Deserialize<Player>(gameData.Player, skillsDb, itemsDb);

            worldMapData.Seed = gameData.Seed;
            worldMapData.WorldMapInfos = gameData.WorldMapInfos;
            worldMapData.DiscoveredMap = gameData.DiscoveredMap;
            worldMapData.CurrentPos = new Vector2(gameData.CurrentPosX, gameData.CurrentPosY);
            worldMapData.Size = gameData.Size;

            worldMapData.Inventory = new List<Item>();
            for (int i = 0; i < gameData.InventoryItemId.Count; i++) {
                Item newItem = ScriptableObject.CreateInstance<Item>();
                worldMapData.Inventory.Add(DataSerialization.DeserializeItem(newItem, gameData.InventoryItemId[i], itemsDb));
                newItem.Quantity = gameData.InventoryItemQuantity[i];
            }

            return worldMapData;

        } else {
            Debug.Log("(Load) WorldMapSaveFile doesn't exist!");
            return null;
        }
    }

    public static void SaveBattleground(BattlegroundData battlegroundData) {

        FileStream fs = File.Create(BattlegroundSaveFile);
        BinaryFormatter bf = new BinaryFormatter();

        SerializedBattlegroundData gameData = new SerializedBattlegroundData();

        gameData.Player = DataSerialization.Serialize(battlegroundData.Player);
        gameData.Enemies = DataSerialization.Serialize(battlegroundData.Enemies);
        gameData.MapInfos = DataSerialization.Serialize(battlegroundData.MapInfos);
        gameData.CurrentUnitId = battlegroundData.CurrentUnitId;

        bf.Serialize(fs, gameData);
        fs.Close();
    }

    public static BattlegroundData LoadBattleground() {

        if (File.Exists(BattlegroundSaveFile)) {

            FileStream fs = File.Open(BattlegroundSaveFile, FileMode.Open);

            if (fs == null || fs.Length == 0) {
                Debug.Log("Failed to load BattlegroundSaveFile (null or empty)");
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            SerializedBattlegroundData gameData = (SerializedBattlegroundData)bf.Deserialize(fs);
            fs.Close();

            BattlegroundData battlegroundData = new BattlegroundData();
            Dictionary<string, string> skillsDb = DataSerialization.ReadDatabasePathById(skillsDatabasePreset);
            Dictionary<string, string> itemsDb = DataSerialization.ReadDatabasePathById(itemsDatabasePreset);


            battlegroundData.Player = DataSerialization.Deserialize<Player>(gameData.Player, skillsDb, itemsDb);
            battlegroundData.Enemies = DataSerialization.Deserialize<Enemy>(gameData.Enemies, skillsDb, itemsDb);

            battlegroundData.MapInfos = DataSerialization.Deserialize(gameData.MapInfos, materialsDatabasePreset, battlegroundData.Player, battlegroundData.Enemies);

            if (battlegroundData.MapInfos.Map != null) {
                SetCurrentCell(battlegroundData.Player, gameData.Player, battlegroundData.MapInfos.Map);
                SetCurrentCell(battlegroundData.Enemies, gameData.Enemies, battlegroundData.MapInfos.Map);
            }
            battlegroundData.CurrentUnitId = gameData.CurrentUnitId;

            return battlegroundData;

        } else {
            Debug.Log("(Load) BattlegroundSaveFile doesn't exist!");
            return null;
        }
    }

    private static void SetCurrentCell<T>(List<T> team, List<UnitSerializable> gameDataTeam, Cell[,] Map) where T : Unit {

        for (int i = 0; i < team.Count; i++)
             team[i].CurrentCell = Map[gameDataTeam[i].CurrentCellX, gameDataTeam[i].CurrentCellZ];
    }

    public static void DeleteBattlegroundSave() {

        if (File.Exists(BattlegroundSaveFile)) {
            File.Delete(BattlegroundSaveFile);
            Debug.Log("BattlegroundSaveFile deleted");
        }
    }

    public static void DeleteSaves() {

        if (File.Exists(WorldMapSaveFile)) {
            File.Delete(WorldMapSaveFile);
            Debug.Log("WorldMapSaveFile deleted");
        }

        DeleteBattlegroundSave();
    }

    public static bool CheckValidFileSave(Scene scene) {

        string pathToCheck;
        if (scene == Scene.WorldMap)
            pathToCheck = WorldMapSaveFile;
        else if (scene == Scene.Battleground)
            pathToCheck = BattlegroundSaveFile;
        else {
            Debug.Log("Invalid sceneSaveFile to check: " + scene);
            return false;
        }

        if (File.Exists(pathToCheck)) {

            FileStream fs = File.Open(pathToCheck, FileMode.Open);
            if (fs != null && fs.Length != 0) {
                fs.Close();
                return true;
            }
            fs.Close();
        }
        return false;
    }
}

public enum Scene {
    MainMenu = 0,
    CharacterCreation = 1,
    WorldMap = 2,
    Battleground = 3,
    Null
}