using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedBattlegroundData {

    public List<UnitSerializable>   Player;
    public List<UnitSerializable>   Enemies;
    public MapInfosSerializable     MapInfos;
    public int CurrentUnitId;
}
