using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Get Skill Card", menuName = "Scriptable Object/Item/Create Get SkillCard Data")]
public class GetSkillCard_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        if (player.isMine)
        {
            int cardIdx = 9;
            GamePlayManager.Instance.playerDeck.deck.Add(cardIdx);
        }
    }
}
