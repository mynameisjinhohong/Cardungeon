using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_PCI : TileObject_PCI
{
    private void Start()
    {
        isPathable = false;
        isDestructable = true;
        isInteractable = false;
    }
    public override void OnDamaged(Player_HJH player)
    {
        Destroy(gameObject);
    }
}
