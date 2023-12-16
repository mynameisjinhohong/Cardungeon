using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Key", menuName = "Scriptable Object/Item/Create Key Data")]
public class Key_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        player.keys++;
        base.OnInteracted(player);
    }
}
