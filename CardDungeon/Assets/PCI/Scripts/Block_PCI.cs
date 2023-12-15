using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_PCI : TileObject_PCI
{
    public override void OnDamaged(Player_HJH player)
    {
        Destroy(gameObject);
    }
}
