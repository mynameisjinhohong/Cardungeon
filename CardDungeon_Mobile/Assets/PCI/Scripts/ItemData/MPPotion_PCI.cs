using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MpPotion", menuName = "Scriptable Object/Item/Create MpPotion Data")]
public class MPPotion_PCI : ItemData_PCI
{
    public int value;

    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        player.Mp += value;
    }
}
