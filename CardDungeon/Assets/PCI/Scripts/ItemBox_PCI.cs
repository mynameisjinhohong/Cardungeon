using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox_PCI : TileObject_PCI
{
    ItemData_PCI _data;

    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        AudioPlayer.Instance.PlayClip(11);
        Animation();
        _data.OnInteracted(player);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }

    private void Animation()
    {

    }
}
