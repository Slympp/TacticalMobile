using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSerialization {

    public static MapInfosSerializable Serialize(MapInfos mapInfos) {

        MapInfosSerializable serializedData = new MapInfosSerializable();

        serializedData.MapDataType = (MapDataType)System.Enum.Parse(typeof(MapDataType), mapInfos.MapData.name);

        serializedData.SizeX = (int)mapInfos.Size.x;
        serializedData.SizeY = (int)mapInfos.Size.y;
        serializedData.Layout = mapInfos.Layout;

        if (mapInfos.Map != null) {
            serializedData.Materials = new List<string>();
            CellSerializable[,] serializedMap = new CellSerializable[serializedData.SizeX, serializedData.SizeY];
            for (int x = 0; x < serializedData.SizeX; x++)
                for (int y = 0; y < serializedData.SizeY; y++) {

                    serializedMap[x, y] = new CellSerializable();

                    if (mapInfos.Map[x, y].IsTaken != null)
                        serializedMap[x, y].UnitId = mapInfos.Map[x, y].IsTaken.Id;
                    else
                        serializedMap[x, y].UnitId = -1;

                    serializedMap[x, y].IsWalkable = mapInfos.Map[x, y].IsWalkable;
                    serializedMap[x, y].X = mapInfos.Map[x, y].X;
                    serializedMap[x, y].Y = mapInfos.Map[x, y].Y;
                    serializedMap[x, y].Z = mapInfos.Map[x, y].Z;
                    serializedMap[x, y].RealY = mapInfos.Map[x, y].RealY;

                    if (mapInfos.Map[x, y].Material != null) {
                        serializedMap[x, y].MaterialName = mapInfos.Map[x, y].Material.name;
                        if (!serializedData.Materials.Contains(serializedMap[x, y].MaterialName))
                            serializedData.Materials.Add(serializedMap[x, y].MaterialName);
                    }

                    if (mapInfos.Map[x, y].Filler != null) {
                        serializedMap[x, y].FillerName = mapInfos.Map[x, y].Filler.name;
                        if (!serializedData.Materials.Contains(serializedMap[x, y].FillerName))
                            serializedData.Materials.Add(serializedMap[x, y].FillerName);
                    }
                }

            serializedData.Map = serializedMap;
        } else
            serializedData.Map = null;
        
        return serializedData;
    }

    public static MapInfos Deserialize(MapInfosSerializable serializedData, DatabasePreset materialsPreset, List<Player> player, List<Enemy> enemies) {

        MapInfos mapInfos = new MapInfos();

        string path = "MapData/" + serializedData.MapDataType.ToString();
        mapInfos.MapData = Resources.Load<ScriptableObject>(path) as MapData;

        mapInfos.Size = new Vector2(serializedData.SizeX, serializedData.SizeY);
        mapInfos.Layout = serializedData.Layout;

        if (serializedData.Map != null) {
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            Dictionary<string, string> materialsDatabase = DataSerialization.ReadDatabasePathByName(materialsPreset);

            foreach (string name in serializedData.Materials) {

                if (materialsDatabase.ContainsKey(name)) {
                    materials.Add(name, Resources.Load<Material>(materialsDatabase[name]));
                } else
                    Debug.Log("Material " + name + " not found in DB");
            }

            Cell[,] newMap = new Cell[serializedData.SizeX, serializedData.SizeY];
            for (int x = 0; x < serializedData.SizeX; x++) {
                for (int y = 0; y < serializedData.SizeY; y++) {

                    newMap[x, y] = new Cell();

                    if (serializedData.Map[x, y].UnitId != -1)
                        newMap[x, y].IsTaken = GetUnitById(serializedData.Map[x, y].UnitId, player, enemies);

                    newMap[x, y].IsWalkable = serializedData.Map[x, y].IsWalkable;
                    newMap[x, y].X = serializedData.Map[x, y].X;
                    newMap[x, y].Y = serializedData.Map[x, y].Y;
                    newMap[x, y].Z = serializedData.Map[x, y].Z;
                    newMap[x, y].RealY = serializedData.Map[x, y].RealY;

                    if (serializedData.Map[x, y].MaterialName != null &&
                        serializedData.Map[x, y].MaterialName != "") {

                        if (materials.ContainsKey(serializedData.Map[x, y].MaterialName)) {
                            newMap[x, y].Material = materials[serializedData.Map[x, y].MaterialName];
                        } else
                            Debug.Log("Deserialization error, material " + serializedData.Map[x, y].MaterialName + " not found");
                    }

                    if (serializedData.Map[x, y].FillerName != null &&
                        serializedData.Map[x, y].FillerName != "") {

                        if (materials.ContainsKey(serializedData.Map[x, y].FillerName))
                            newMap[x, y].Filler = materials[serializedData.Map[x, y].FillerName];
                        else
                            Debug.Log("Deserialization error, material " + serializedData.Map[x, y].FillerName + " not found");
                    }
                }
            }
            mapInfos.Map = newMap;
        } else
            mapInfos.Map = null;

        return mapInfos;
    }

    private static Unit GetUnitById<A, B>(int id, List<A> player, List<B> enemies) where A : Unit
                                                                                    where B : Unit {

        if (id == -1)
            return null;

        foreach(Unit unit in player)
            if (unit.Id == id)
                return unit;

        foreach (Unit unit in enemies)
            if (unit.Id == id)
                return unit;

        Debug.Log("Deserialization error, unit n°" + id + " not found!");
        return null;
    }

    public static List<UnitSerializable> Serialize<T>(List<T> team) where T : Unit {

        List<UnitSerializable> serializedTeam = new List<UnitSerializable>();
        for (int i = 0; i < team.Count; i++)
            serializedTeam.Add(Serialize(team[i]));
        return serializedTeam;
    }

    public static List<T> Deserialize<T>(List<UnitSerializable> gameDataTeam, Dictionary<string, string> skillsDb, Dictionary<string, string> itemsDb) where T : Unit {

        List<T> deserializedTeam = new List<T>();
        for (int i = 0; i < gameDataTeam.Count; i++) {
            deserializedTeam.Add(Deserialize<T>(gameDataTeam[i], skillsDb, itemsDb));
        }
        return deserializedTeam;
    }

    public static UnitSerializable Serialize(Unit unit) {

        UnitSerializable serializedData = new UnitSerializable();

        serializedData.Id = unit.Id;
        serializedData.UnitName = unit.UnitName;
        serializedData.Team = unit.Team;

        // Size -1 because we don't need to save the weaponSkill (0)
        serializedData.Skills = new List<int>(unit.Skills.Count - 1);
        for (int i = 1; i < unit.Skills.Count; i++) {
            if (unit.Skills[i] != null) {
                serializedData.Skills.Add(unit.Skills[i].SkillId);
            } else
                serializedData.Skills.Add(0);
        }

        serializedData.AttributesSheet = unit.AttributesSheet;
        serializedData.UnitEquipment = Serialize(unit.UnitEquipment);
        serializedData.HumanModelInfos = unit.HumanModelInfos;
        serializedData.IsAlive = unit.IsAlive;
        serializedData.IsPlaying = unit.IsPlaying;

        if (unit.CurrentCell != null) {
            serializedData.CurrentCellX = unit.CurrentCell.X;
            serializedData.CurrentCellZ = unit.CurrentCell.Z;
        }

        serializedData.CurrentRotation = unit.CurrentRotation;
        serializedData.SkillsCooldowns = unit.SkillsCooldowns;
        serializedData.SkillsUses = unit.SkillsUses;

        return serializedData;
    }

    public static T Deserialize<T>(UnitSerializable serializedData, Dictionary<string, string> skillsDb, Dictionary<string, string> itemsDb) where T : Unit {

        GameObject go = new GameObject(serializedData.UnitName);
        T unit = go.AddComponent<T>();
        unit.Id = serializedData.Id;
        unit.UnitName = serializedData.UnitName;
        unit.Team = serializedData.Team;

        unit.Skills = new List<SkillBase>(serializedData.Skills.Count + 1);
        unit.Skills.Add(null);
        for (int i = 0; i < serializedData.Skills.Count; i++) {

            if (serializedData.Skills[i] != 0) {
                string key = serializedData.Skills[i].ToString();
                if (skillsDb.ContainsKey(key)) {
                    unit.Skills.Add(Resources.Load<ScriptableObject>(skillsDb[key]) as SkillBase);
                } else {
                    Debug.Log("Deserialize Unit: skillId " + serializedData.Skills[i] + " not found in DB");
                }
            }
        }

        unit.AttributesSheet = serializedData.AttributesSheet;
        unit.UnitEquipment = Deserialize(serializedData.UnitEquipment, itemsDb);
        unit.HumanModelInfos = serializedData.HumanModelInfos;
        unit.IsAlive = serializedData.IsAlive;
        unit.IsPlaying = serializedData.IsPlaying;
        unit.CurrentRotation = serializedData.CurrentRotation;
        unit.SkillsCooldowns = serializedData.SkillsCooldowns;
        unit.SkillsUses = serializedData.SkillsUses;
        return unit;
    }

    public static UnitEquipmentSerializable Serialize(UnitEquipment unitEquipment) {

        UnitEquipmentSerializable serializedData = new UnitEquipmentSerializable();

        serializedData.RightWeapon = (unitEquipment.RightWeapon == null) ? 0 : unitEquipment.RightWeapon.ItemId;
        serializedData.LeftWeapon = (unitEquipment.LeftWeapon == null) ? 0 : unitEquipment.LeftWeapon.ItemId;
        serializedData.Body = (unitEquipment.Body == null) ? 0 : unitEquipment.Body.ItemId;
        serializedData.Head = (unitEquipment.Head == null) ? 0 : unitEquipment.Head.ItemId;
        serializedData.Back = (unitEquipment.Back == null) ? 0 : unitEquipment.Back.ItemId;
        serializedData.AccessoryA = (unitEquipment.AccessoryA == null) ? 0 : unitEquipment.AccessoryA.ItemId;
        serializedData.AccessoryB = (unitEquipment.AccessoryB == null) ? 0 : unitEquipment.AccessoryB.ItemId;

        return serializedData;
    }

    public static UnitEquipment Deserialize(UnitEquipmentSerializable serializedData, Dictionary<string, string> itemsDb) {

        UnitEquipment unitEquipment = new UnitEquipment();

        unitEquipment.RightWeapon = DeserializeItem(unitEquipment.RightWeapon, serializedData.RightWeapon.ToString(), itemsDb);
        unitEquipment.LeftWeapon = DeserializeItem(unitEquipment.LeftWeapon, serializedData.LeftWeapon.ToString(), itemsDb);
        unitEquipment.Body = DeserializeItem(unitEquipment.Body, serializedData.Body.ToString(), itemsDb);
        unitEquipment.Head = DeserializeItem(unitEquipment.Head, serializedData.Head.ToString(), itemsDb);
        unitEquipment.Back = DeserializeItem(unitEquipment.Back, serializedData.Back.ToString(), itemsDb);
        unitEquipment.AccessoryA = DeserializeItem(unitEquipment.AccessoryA, serializedData.AccessoryA.ToString(), itemsDb);
        unitEquipment.AccessoryB = DeserializeItem(unitEquipment.AccessoryB, serializedData.AccessoryB.ToString(), itemsDb);

        return unitEquipment;
    }

    public static T DeserializeItem<T>(T item, string key, Dictionary<string, string> itemsDb) where T : Item {

        if (key != "0") {
            if (itemsDb.ContainsKey(key))
                item = Resources.Load<ScriptableObject>(itemsDb[key]) as T;
            else
                Debug.Log("Deserialize Item: itemId " + key + " not found in DB");
        } else
            item = null;
        return item;
    }

    public static Dictionary<string, string> ReadDatabasePathById(DatabasePreset preset) {

        Dictionary<string, string> dic = new Dictionary<string, string>();

        DatabaseElementList list = JsonUtility.FromJson<DatabaseElementList>(ReadDatabase(preset).text);

        foreach (DatabaseElement element in list.List) {
            if (element.Data[0] != "0")
                dic.Add(element.Data[0], element.Path);
        }

        return dic;
    }

    public static Dictionary<string, string> ReadDatabasePathByName(DatabasePreset preset) {

        Dictionary<string, string> dic = new Dictionary<string, string>();

        DatabaseElementList list = JsonUtility.FromJson<DatabaseElementList>(ReadDatabase(preset).text);

        foreach (DatabaseElement element in list.List) {
            dic.Add(element.Name, element.Path);
        }

        return dic;
    }

    public static TextAsset ReadDatabase(DatabasePreset preset) {
        TextAsset text = Resources.Load<TextAsset>("Databases/" + preset.Name);

        return text;
    }
}
