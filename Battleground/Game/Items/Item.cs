using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject {

	public string       ItemName;
    public int          ItemId;
    public int          Quantity;
    public ItemQuality  ItemQuality;
    public Sprite       Icon;
}

public enum ItemQuality {
    Common,     // white
    Magic,      // green
    Rare,       // blue
    Epic,       // purple
    Legendary   // orange
}
