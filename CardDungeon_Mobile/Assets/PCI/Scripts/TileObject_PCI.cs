using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject_PCI : MonoBehaviour
{
    public bool isPathable;
    public bool isDestructable;
    public bool isInteractable;
    public int sortOrder;

    [HideInInspector]
    public Tile_PCI tile;

    public SpriteRenderer spriteRenderer;
    public virtual void OnDamaged(Player_HJH player)
    {
        Debug.Log($"Object Damaged : {gameObject.name}");
    }
    public virtual void OnInteracted(Player_HJH player)
    {
        Debug.Log($"Object Interacting : {gameObject.name}");
    }

    public void Show()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}