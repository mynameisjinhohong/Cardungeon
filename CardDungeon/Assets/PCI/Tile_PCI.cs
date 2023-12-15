using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_PCI : MonoBehaviour
{
    private List<TileObject_PCI> onTileObjects = new List<TileObject_PCI>();
    private bool isPathable;

    public void AddTileObject(TileObject_PCI obj)
    {
        onTileObjects.Add(obj);
    }
}
