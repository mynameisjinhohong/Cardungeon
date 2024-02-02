using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_PCI : TileObject_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        if(player.keys > 2)
        {
            if (player.isMine)
            { 
                GamePlayManager.Instance.GameWin();
            }
            else
            {
                GamePlayManager.Instance.GameOver();
            }
        }
    }
}
