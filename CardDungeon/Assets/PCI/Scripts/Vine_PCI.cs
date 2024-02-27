using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine_PCI : TileObject_PCI
{
    public int stunTime;
    public override void OnInteracted(Player_HJH player)
    {
        GamePlayManager.Instance.playerDeck.StunGo(stunTime);
        base.OnInteracted(player);
    }
}