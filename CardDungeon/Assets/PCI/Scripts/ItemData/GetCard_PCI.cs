using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Get Card", menuName = "Scriptable Object/Item/Create Get Card Data")]
public class GetCard_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        if (player.isMine)
        {
            int cardIdx = Random.Range(1, CardManager.Instance.cardList.cards.Length);
            GamePlayManager.Instance.playerDeck.deck.Add(cardIdx);
        }
    }
}
