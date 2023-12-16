using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_PCI : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    public List<TileObject_PCI> onTileObjects = new List<TileObject_PCI>();

    private void Start()
    {
        int rand = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[rand];
    }
    public void AddTileObject(TileObject_PCI obj)
    {
        onTileObjects.Add(obj);
        onTileObjects.Sort((TileObject_PCI lhs, TileObject_PCI rhs) =>
        {
            if(lhs.sortOrder < rhs.sortOrder)
            {
                return 1;
            }else if(lhs.sortOrder == rhs.sortOrder)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        });
        obj.tile = this;
        Render();
    }

    public void Render()
    {
        if (onTileObjects.Count == 0) return;
        foreach(var e in onTileObjects)
        {
            e.spriteRenderer.enabled = false;
        }
        onTileObjects[0].spriteRenderer.enabled = true;
    }

    public void RemoveTileObject(TileObject_PCI obj)
    {
        if(onTileObjects.Contains(obj))
            onTileObjects.Remove(obj);
        Render();
    }

    public bool IsPahtable()
    {
        foreach (var e in onTileObjects)
        {
            if (!e.isPathable) return false;
        }
        return true;
    }

    public bool IsDestructable()
    {
        foreach(var e in onTileObjects)
        {
            if (e.isDestructable) return true;
        }
        return false;
    }

    public bool IsInteractable()
    {
        foreach(var e in onTileObjects)
        {
            if (e.isInteractable) return true;
        }
        return false;
    }

    public void OnDamaged(Player_HJH player)
    {
        foreach(var e in onTileObjects)
        {
            if (e.isDestructable)
            {
                onTileObjects.Remove(e);
                e.OnDamaged(player);
                Render();
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
                Render();
                break;
            }
        }
    }
}
