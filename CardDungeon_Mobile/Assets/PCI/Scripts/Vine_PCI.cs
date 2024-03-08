using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vine_PCI : TileObject_PCI
{
    public int stunTime;
    public override void OnInteracted(Player_HJH player)
    {
        GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage("µ£¿¡ °É·È½À´Ï´Ù", 3.0f);
        GamePlayManager.Instance.playerDeck.StunGo(stunTime);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }
}