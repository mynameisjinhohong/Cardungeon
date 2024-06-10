using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_PCI : TileObject_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        if(player.Keys > 2)
        {
            BackendManager.Instance.winUser = player.PlayerName.text;

            BackendManager.Instance.MatchEnd(true);
        }
    }
}
