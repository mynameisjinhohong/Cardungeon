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
        Debug.Log("�߰��� ������� ����");
        StartCoroutine(WaitChase());
    }

    IEnumerator WaitChase()
    {
        Debug.Log("�߰� ����");
        
        while (true)
        {
            if (!BackendManager.Instance.isInitialize)
            {
                yield break; // �ڷ�ƾ ����
            }
            
            int ranIdx = Random.Range(1, 5);

            GamePlayManager.Instance.CardGo(9, ranIdx);
            
            yield return new WaitForSeconds(2f);
            
            if (!BackendManager.Instance.isInitialize)
            {
                yield break; // �ڷ�ƾ ����
            }
            
            GamePlayManager.Instance.CardGo(9, 8);

            yield return new WaitForSeconds(2f);
        }
    }
}
