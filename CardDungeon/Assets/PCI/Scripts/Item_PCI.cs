using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

// TODO: 아이템 획득 연출,  아이템 상자와 분리하기

public class Item_PCI : TileObject_PCI
{
    [SerializeField]
    protected ItemData_PCI _data;
    public ItemVfx_PCI vfx;

    private void Start()
    {
        // gameObject.layer = LayerMask.GetMask() 
    }

    public virtual void SetData(ItemData_PCI data) {
        try
        {
            _data = data;
            spriteRenderer.sprite = _data.sprites[0];
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log($"Error ({data.itemName})");
        }
    }

    public override void OnInteracted(Player_HJH player)
    {
        if (player.isMine)
        {
            AudioPlayer.Instance.PlayClip(11);
            Animation();
        }
        _data.OnInteracted(player);
        tile.RemoveTileObject(this);
        Destroy(gameObject);
    }

    public void Animation()
    {
        ItemVfx_PCI v = Instantiate(vfx, transform.parent);
        v.sr.sprite = _data.image;
    }
}
