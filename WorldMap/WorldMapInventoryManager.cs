using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapInventoryManager : MonoBehaviour {

    public GameObject   InventoryUI;

    public Transform    Content;
    public GameObject   ItemInventory;

    private List<Item>  Inventory;

    private void Start() {
        Inventory = GetComponent<WorldMapManager>().WorldMapData.Inventory;
    }

    public void Toggle(bool b) {
        InventoryUI.SetActive(b);
        if (b)
            FilterInventoryBy(-1);
    }

    private void PopulateList(List<Item> itemList) {

        DestroyChildren(Content);

        foreach (Item item in itemList) {
            GameObject go = Instantiate(ItemInventory);
            go.transform.SetParent(Content, false);

            if (item.Icon != null) {
                GetChildrenByName(go.transform, "ItemIcon").GetComponent<Image>().sprite = item.Icon;
            }

            Text itemName = GetChildrenByName(go.transform, "ItemName").GetComponent<Text>();
            itemName.text = item.ItemName;
            itemName.color = GetColorNameByQuality(item.ItemQuality);
            GetChildrenByName(go.transform, "ItemQuantity").GetComponent<Text>().text = item.Quantity.ToString();
        }
    }

    public void FilterInventoryBy(int itemType) {

        List<Item> itemList;
        switch ((ItemType)itemType) {
            case ItemType.Weapon:
                itemList = FilterBy<Weapon>();
                break;

            case ItemType.Head:
                itemList = FilterBy<Head>();
                break;

            case ItemType.Body:
                itemList = FilterBy<Body>();
                break;

            case ItemType.Back:
                itemList = FilterBy<Back>();
                break;

            case ItemType.Accessory:
                itemList = FilterBy<Accessory>();
                break;

            case ItemType.Consumable:
                itemList = FilterBy<Consumable>();
                break;

            default:
                itemList = Inventory;
                break;
        }
        PopulateList(itemList);
    }

    private List<Item> FilterBy<T>() where T : Item {

        List<Item> filteredList = new List<Item>();

        foreach (Item item in Inventory) {
            if (item is T)
                filteredList.Add(item);
        }

        return filteredList;
    }

    private Color GetColorNameByQuality(ItemQuality quality) {

        switch (quality) {
            case ItemQuality.Magic:
                return Color.green;
            case ItemQuality.Rare:
                return Color.blue;
            case ItemQuality.Epic:
                return new Color(0.54f, 0.17f, 0.88f);
            case ItemQuality.Legendary:
                return new Color(0.91f, 0.7f, 0);
            default:
                return Color.white;
        }
    }

    private void DestroyChildren(Transform transform) {

        foreach (Transform t in transform)
            Destroy(t.gameObject);
    }

    private Transform GetChildrenByName(Transform transform, string childName) {

        foreach (Transform child in transform) {
            if (child.name == childName)
                return child;
        }
        return null;
    }

    
}

public enum ItemType {
    Weapon,
    Head,
    Body,
    Back,
    Accessory,
    Consumable
}
