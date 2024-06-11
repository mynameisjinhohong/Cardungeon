using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chaser : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer chaserSR;

    void Start()
    {
        Debug.Log("추격자 생성대기 시작");
        StartCoroutine(WaitChase());
    }

    IEnumerator WaitChase()
    {
        Debug.Log("추격 시작");
        
        while (true)
        {
            if (!BackendManager.Instance.isInitialize)
            {
                yield break; // 코루틴 종료
            }
            
            int ranIdx = Random.Range(1, 5);

            GamePlayManager.Instance.CardGo(9, ranIdx);
            
            yield return new WaitForSeconds(2f);
            
            if (!BackendManager.Instance.isInitialize)
            {
                yield break; // 코루틴 종료
            }
            
            GamePlayManager.Instance.CardGo(9, 8);

            yield return new WaitForSeconds(2f);
        }
    }
}
