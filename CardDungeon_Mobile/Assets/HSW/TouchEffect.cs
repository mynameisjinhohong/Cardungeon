using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TouchEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        
        Destroy(this.gameObject);
    }
}