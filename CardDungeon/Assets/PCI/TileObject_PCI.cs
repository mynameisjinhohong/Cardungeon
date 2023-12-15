using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject_PCI : MonoBehaviour
{
    protected  virtual void Interact()
    {
        Debug.Log($"Object Interacting : {gameObject.name}");
    }
}
