using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Equipment {

    public Sprite           SkillIcon;
    public GameObject       Skin;
    public string           AnimationPrefix = "";
    public WeaponType       WeaponType;

    public virtual SkillBaseConfig  GetSkillBaseConfig() {
        return null;
    }

    public virtual bool IsTwoHanded() {
        return false;
    }

    public virtual bool IsLeftHandOnly() {
        return false;
    }
}

public enum WeaponType {
    Sword,
    Axe,
    Mace,
    Dagger,
    THSword,
    THAxe,
    Spear,
    Shield,
    Bow,
    Crossbow,
    Wand,
    All
}