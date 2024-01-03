using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox_PCI : Item_PCI
{
    private void Start()
    {
        // gameObject.layer = LayerMask.GetMask() 
    }

    public override void SetData(ItemData_PCI data)
    {
        try
        {
            _data = data;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Error ({data.itemName})");
        }
    }

    public override void OnInteracted(Player_HJH player)
    {
        if (player.isMine)
        {
            AudioPlayer.Instance.PlayClip(11);
            Animation();
        }
        _data.OnInteracted(player);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }
}
