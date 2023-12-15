using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject_PCI : MonoBehaviour
{
    public bool isPathable;
    public bool isDestructable;
    public bool isInteractable;

    public SpriteRenderer spriteRenderer;
    public virtual void OnDamaged(Player_HJH player)
    {
        Debug.Log($"Object Damaged : {gameObject.name}");
    }
    public virtual void OnInteracted(Player_HJH player)
    {
        Debug.Log($"Object Interacting : {gameObject.name}");
    }
}
