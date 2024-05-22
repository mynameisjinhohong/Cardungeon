using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vine_PCI : TileObject_PCI
{
    public float stunTime;
    public override void OnInteracted(Player_HJH player)
    {
        GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage(player.PlayerName.text + "¥‘¿Ã µ£ø° ∞…∑»Ω¿¥œ¥Ÿ", stunTime);
        GamePlayManager.Instance.playerDeck.StunGo(stunTime);
        player.StunOn(stunTime);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }
}