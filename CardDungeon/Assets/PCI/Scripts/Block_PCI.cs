using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_PCI : TileObject_PCI
{
    public List<Sprite> sprites = new List<Sprite>();
    private void Start()
    {
        int rand = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[rand];
    }
    public override void OnDamaged(Player_HJH player)
    {
        Destroy(gameObject);
    }
}
