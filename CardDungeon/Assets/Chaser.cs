using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chaser : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer chaserSR;
    private Coroutine canMoveCor;

    public void Chase(bool isOnOff)
    {
        gameObject.SetActive(isOnOff);

        if (isOnOff)
        {
            if (canMoveCor == null)
            {
                canMoveCor = StartCoroutine(RanMoveCor());
            } 
        }
        else
        {
            StopCoroutine(canMoveCor);   
        }
    }
    IEnumerator RanMoveCor()
    {
        while (true)
        {
            int ranIdx = Random.Range(1, 5);

            GamePlayManager.Instance.CardGo(9, ranIdx);
            
            yield return new WaitForSeconds(2f);
            
            GamePlayManager.Instance.CardGo(9, 8);

            yield return new WaitForSeconds(2f);
        }
    }
}
