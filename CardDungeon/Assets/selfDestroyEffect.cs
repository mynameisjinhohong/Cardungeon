using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestroyEffect : MonoBehaviour
{
    public void EffectStart(float time)
    {
        Destroy(gameObject, time);
    }
}
