using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chaser : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer chaserSR;

    public void StartChase()
    {
        gameObject.SetActive(true);

        StartCoroutine(RanMove());
    }

    IEnumerator RanMove()
    {
        while (true)
        {
            int ranIdx = Random.Range(1, 5);

            GamePlayManager.Instance.CardGo(9, ranIdx);
            
            yield return new WaitForSeconds(1f);
            
            GamePlayManager.Instance.CardGo(9, 8);

            yield return new WaitForSeconds(1f);
        }
    }
}
