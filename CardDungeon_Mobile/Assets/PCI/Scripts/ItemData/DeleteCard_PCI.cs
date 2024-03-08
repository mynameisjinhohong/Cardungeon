using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Delete Card", menuName = "Scriptable Object/Item/Create Delete Card Data")]
public class DeleteCard_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        if (player.isMine)
        {
            GamePlayManager.Instance.mainUi.DeleteStart();
        }
    }
}
