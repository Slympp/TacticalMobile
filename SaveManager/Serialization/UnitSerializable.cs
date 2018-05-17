using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitSerializable {

    public int                          Id;
    public string                       UnitName;
    public Team                         Team;
    public List<int>                    Skills;
    public AttributesSheet              AttributesSheet;
    public UnitEquipmentSerializable    UnitEquipment;
    public HumanModelInfos              HumanModelInfos;

    public bool                         IsAlive;
    public bool                         IsPlaying;
    public int                          CurrentCellX;
    public int                          CurrentCellZ;
    public Rotation                     CurrentRotation;
    public List<int>                    SkillsCooldowns;
    public List<int>                    SkillsUses;
}
