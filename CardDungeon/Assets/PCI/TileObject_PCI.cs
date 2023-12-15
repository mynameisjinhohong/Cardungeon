using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject_PCI : MonoBehaviour
{
    public bool isPathable;
    public bool isDestructable;
    public bool isInteractable;
    public virtual void OnDamaged()
    {
        Debug.Log($"Object Damaged : {gameObject.name}");
    }
    public virtual void OnInteracted()
    {
        Debug.Log($"Object Interacting : {gameObject.name}");
    }
}
