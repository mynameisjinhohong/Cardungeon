using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData_PCI : ScriptableObject
{
    public string itemName;
    public int itemIdx;
    public string description;
    public Sprite image;
    public int amount;

    public List<Sprite> sprites = new List<Sprite>();
    public virtual void OnInteracted(Player_HJH player)
    {
        Debug.Log($"Item Activated : {itemName}({player})");
    }
}
