using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Get Move Card", menuName = "Scriptable Object/Item/Create Get MoveCard Data")]
public class GetMoveCard_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        if (player.isMine)
        {
            int cardIdx = Random.Range(1, 6);
            GamePlayManager.Instance.playerDeck.deck.Add(cardIdx);
        }
    }
}
