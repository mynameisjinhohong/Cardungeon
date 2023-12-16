using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PCI : TileObject_PCI
{
    ItemData_PCI _data;
    bool isRandom;

    public void SetData(ItemData_PCI data)
    {
        try {
            _data = data;
            int rand = UnityEngine.Random.Range(0, data.sprites.Count);
            if (rand == 0) isRandom = true;
            spriteRenderer.sprite = data.sprites[rand];
        }catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Error ({data.itemName})");
        }
    }

    public override void OnInteracted(Player_HJH player)
    {
        if (isRandom)
        {
            AudioPlayer.Instance.PlayClip(11);
        }
        _data.OnInteracted(player);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }
}
