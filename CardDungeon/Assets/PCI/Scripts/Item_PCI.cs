using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_PCI : TileObject_PCI
{
    ItemData_PCI _data;
    public Sprite sprite;

    public void SetData(ItemData_PCI data)
    {
        _data = data;
        //sprite = data.image;
    }

    public override void OnInteracted(Player_HJH player)
    {
        _data.OnInteracted(player);
        Destroy(gameObject);
    }
}
