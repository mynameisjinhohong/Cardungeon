using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HpPotion", menuName = "Scriptable Object/Item/Create HpPotion Data")]
public class HPPotion_PCI : ItemData_PCI
{
    public int value;
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        player.HP += value;
    }
}
