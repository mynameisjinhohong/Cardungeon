using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DarkFog", menuName = "Scriptable Object/Item/Create DarkFog Data")]

public class DarkFog_PCI : ItemData_PCI
{
    public override void OnInteracted(Player_HJH player)
    {
        base.OnInteracted(player);
        player.DarkOn();
    }
}
