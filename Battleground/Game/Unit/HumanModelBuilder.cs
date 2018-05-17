using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanModelBuilder {

    public static GameObject BuildModel(HumanModelInfos modelInfos, UnitEquipment unitEquipment) {

        GameObject model = GameObject.Instantiate((GameObject)Resources.Load("Body/Template", typeof(GameObject)));

        if (modelInfos == null) {
            modelInfos = new HumanModelInfos();
            modelInfos.Randomize();
        }

        AddHair(model, modelInfos);
        AddBody(model, modelInfos, unitEquipment.Body);
        AddHead(model, unitEquipment.Head);
        AddBack(model, unitEquipment.Back);
        AddWeapons(model, unitEquipment.RightWeapon, unitEquipment.LeftWeapon);

        return model;
    }

    private static void AddHair(GameObject model, HumanModelInfos modelInfos) {

        GameObject go;
        string path = "Hair/" + (modelInfos.Gender).ToString() +
                    "/" + "Hair " + (modelInfos.Gender).ToString() + " " + ((modelInfos.HairType < 10) ? "0" + modelInfos.HairType.ToString() : modelInfos.HairType.ToString()) +
                    " " + (modelInfos.HairColor).ToString();
        go = GameObject.Instantiate((GameObject)Resources.Load(path, typeof(GameObject)));
        go.transform.parent = GetPart(model, "RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigNeck/RigHead/Dummy Prop Head");
        go.transform.localPosition = Vector3.zero;
    }
    
    private static void AddBody(GameObject model, HumanModelInfos modelInfos, Body body) {

        Material bodyMaterial;
        if (body == null) {
            bodyMaterial = (Material)Resources.Load("Body/" + (modelInfos.Gender).ToString() + "/" +
                (modelInfos.Gender).ToString() + " " + (modelInfos.SkinTone).ToString() + " Naked", typeof(Material));
        } else if (modelInfos.Gender == Gender.Male)
            bodyMaterial = body.MaleSkinStone[(int)modelInfos.SkinTone];
        else if (modelInfos.Gender == Gender.Female)
            bodyMaterial = body.FemaleSkinTone[(int)modelInfos.SkinTone];
        else
            bodyMaterial = body.OtherSkinTone;

        GetPart(model, "Base").GetComponent<SkinnedMeshRenderer>().material = bodyMaterial;

        if (body != null && body.SpecialEffect != null) {
            GameObject go = GameObject.Instantiate(body.SpecialEffect);
            go.transform.parent = model.transform;
        }
    }

    private static void AddHead(GameObject model, Head head) {

        if (head != null) {
            GameObject go = GameObject.Instantiate(head.Skin);
            go.transform.parent = GetPart(model, "RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigNeck/RigHead/Dummy Prop Head");
            go.transform.localPosition = Vector3.zero;
        }
    }

    private static void AddBack(GameObject model, Back back) {

        if (back != null) {
            GameObject go = GameObject.Instantiate(back.Skin);
            go.transform.parent = GetPart(model, "RigPelvis/RigSpine1/RigSpine2/RigRibcage/Dummy Prop Back");
            go.transform.localPosition = Vector3.zero;
        }
    }

    private static void AddWeapons(GameObject model, Weapon weaponR, Weapon weaponL) {

        if (weaponR != null) {
            GameObject go = GameObject.Instantiate(weaponR.Skin);
            go.transform.parent = GetPart(model, "RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigRArm1/RigRArm2/RigRArmPalm/Dummy Prop Right");
            go.transform.localPosition = Vector3.zero;
        }

        if (weaponL != null) {
            GameObject go = GameObject.Instantiate(weaponL.Skin);
            go.transform.parent = GetPart(model, "RigPelvis/RigSpine1/RigSpine2/RigRibcage/RigLArm1/RigLArm2/RigLArmPalm/Dummy Prop Left");
            go.transform.localPosition = Vector3.zero;
        }
    }

    private static Transform GetPart(GameObject model, string path) {
        return model.transform.Find(path);
    }
}
