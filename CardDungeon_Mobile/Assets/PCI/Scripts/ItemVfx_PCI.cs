using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVfx_PCI : MonoBehaviour
{
    public SpriteRenderer sr;

    public void EndAnimation()
    {
        Destroy(gameObject);
    }
}
