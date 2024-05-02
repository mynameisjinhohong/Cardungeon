using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Get Attack Card", menuName = "Scriptable Object/Item/Create Get AttackCard Data")]
public class GetAttackCard_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        if (player.isMine)
        {
            int cardIdx = Random.Range(6, 9);
            GamePlayManager.Instance.mainUi.CardSelect(cardIdx);
        }
    }
}
