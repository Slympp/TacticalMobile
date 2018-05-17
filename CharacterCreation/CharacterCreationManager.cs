using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationManager : MonoBehaviour {

    public Text             GenderText;
    public Text             SkinToneText;
    public Text             HairTypeText;
    public Text             HairColorText;

    public Button           BackButton;
    public Button           NextButton;

    public GameObject       ModelRoot;

    private GameObject      Model;
    private HumanModelInfos ModelInfos;
    private UnitEquipment   UnitEquipment;

    private bool            NeedUpdateModel = true;

    private int             SkinToneMaxValue;
    private int             HairTypeMaxValue;
    private int             HairColorMaxValue;

	void Start () {

        UnitEquipment = new UnitEquipment();
        ModelInfos = new HumanModelInfos();

        RandomizeModel();

        SkinToneMaxValue = Enum.GetNames(typeof(SkinTone)).Length - 1;
        HairTypeMaxValue = 16 + 1;
        HairColorMaxValue = Enum.GetNames(typeof(HairColor)).Length;
    }
	
	void Update () {

        if (NeedUpdateModel) {
            if (Model != null)
                Destroy(Model);
            Model = HumanModelBuilder.BuildModel(ModelInfos, UnitEquipment);
            SetParent(Model.transform, ModelRoot.transform);

            NeedUpdateModel = false;
        }
	}

    private void SetParent(Transform child, Transform parent) {

        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    public void ChangeGender() {

        ModelInfos.Gender = ModelInfos.Gender == Gender.Male ? Gender.Female : Gender.Male;
        GenderText.text = ModelInfos.Gender.ToString();
        NeedUpdateModel = true;
    }

    public void ChangeSkinTone(int newValue) {

        if ((int)ModelInfos.SkinTone + newValue < 0)
            ModelInfos.SkinTone = (SkinTone)SkinToneMaxValue - 1;
        else if ((int)ModelInfos.SkinTone + newValue >= SkinToneMaxValue)
            ModelInfos.SkinTone = 0;
        else
            ModelInfos.SkinTone += newValue;

        SkinToneText.text = ModelInfos.SkinTone.ToString();
        NeedUpdateModel = true;
    }

    public void ChangeHairType(int newValue) {

        if ((int)ModelInfos.HairType + newValue < 1)
            ModelInfos.HairType = HairTypeMaxValue - 1;
        else if ((int)ModelInfos.HairType + newValue >= HairTypeMaxValue)
            ModelInfos.HairType = 1;
        else
            ModelInfos.HairType += newValue;

        HairTypeText.text = ModelInfos.HairType.ToString();
        NeedUpdateModel = true;
    }

    public void ChangeHairColor(int newValue) {

        if ((int)ModelInfos.HairColor + newValue < 0)
            ModelInfos.HairColor = (HairColor)HairColorMaxValue - 1;
        else if ((int)ModelInfos.HairColor + newValue >= HairColorMaxValue)
            ModelInfos.HairColor = 0;
        else
            ModelInfos.HairColor += newValue;

        HairColorText.text = ModelInfos.HairColor.ToString();
        NeedUpdateModel = true;
    }

    public void RandomizeModel() {
        ModelInfos.Randomize();

        GenderText.text = ModelInfos.Gender.ToString();
        SkinToneText.text = ModelInfos.SkinTone.ToString();
        HairTypeText.text = ModelInfos.HairType.ToString();
        HairColorText.text = ModelInfos.HairColor.ToString();

        NeedUpdateModel = true;
    }

    public void BackToMainScreen() {

        // TODO: Discard changes warning popup
        SaveManager.LoadScene(Scene.MainMenu);
    }

    public void GoToNextScreen() {

        WorldMapData worldMapData = new WorldMapData();

        worldMapData.Size = 10;
        worldMapData.Player = new List<Player>();
        worldMapData.Player.Add(Resources.Load<GameObject>("PlayerTemplate").GetComponent<Player>());
        worldMapData.Player.Add(Resources.Load<GameObject>("PlayerTemplateRogue").GetComponent<Player>());
        worldMapData.Player.Add(Resources.Load<GameObject>("PlayerTemplateWizard").GetComponent<Player>());
        worldMapData.Player[0].HumanModelInfos = ModelInfos;

        worldMapData.Inventory = new List<Item>();
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Weapons/Sword/CommonSword/CommonSword") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Weapons/Sword/MagicSword/MagicSword") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Weapons/Sword/RareSword/RareSword") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Weapons/Sword/EpicSword/EpicSword") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Weapons/Sword/LegendarySword/LegendarySword") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Head/HelmetKnight1Black") as Item);
        worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Equipments/Body/BodyKnight1Black") as Item);

        for (int i = 0; i < 10; i++)
            worldMapData.Inventory.Add(Resources.Load<ScriptableObject>("Items/Consumables/HealthPotion") as Item);

        SaveManager.SaveWorldMap(worldMapData);
        SaveManager.LoadScene(Scene.WorldMap);
    }
}
