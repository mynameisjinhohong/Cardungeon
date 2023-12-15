using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_PCI : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public List<TileObject_PCI> onTileObjects = new List<TileObject_PCI>();

    private void Start()
    {
        int rand = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[rand];
    }
    public void AddTileObject(TileObject_PCI obj)
    {
        onTileObjects.Add(obj);
    }

    public bool IsPahtable()
    {
        foreach (var e in onTileObjects)
        {
            if (!e.isPathable) return false;
        }
        return true;
    }

    public void OnDamaged(Player_HJH player)
    {
        foreach(var e in onTileObjects)
        {
            if (e.isDestructable)
            {
                onTileObjects.Remove(e);
                e.OnDamaged(player);
                break;
            }
        }
    }

    public void OnInteracted(Player_HJH player)
    {
        foreach(var e in onTileObjects)
        {
            if (e.isInteractable)
            {
                e.OnInteracted(player);
                break;
            }
        }
    }
}
