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
            Debug.Log("¿òÁ÷ÀÓ");
            
            int ranIdx = Random.Range(1, 5);
            
            GamePlayManager.Instance.CardGo(9, ranIdx);
            
            OnChaserMove(ranIdx);
            
            animator.Play("Walk");
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    public bool OnChaserMove(int cardIdx)
    {
        bool ret = true;
        switch (Mathf.Abs(cardIdx))
        {
            case 1:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y + 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y + 1));
                }
                break;
            case 2:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x + 2, (int)transform.position.y));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x + 1, (int)transform.position.y));
                }
                break;
            case 3:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x - 2, (int)transform.position.y));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x - 1, (int)transform.position.y));
                }
                break;
            case 4:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y - 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y - 1));
                }
                break;
            case 5:
                if (cardIdx < 0)
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y - 2));
                }
                else
                {
                    ret = GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x, (int)transform.position.y - 1));
                }
                break;
            case 6:
                if (cardIdx < 0)
                {
                    ret = false;
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x + j, (int)transform.position.y + i)))
                            {
                                ret = true;
                            }
                        }
                    }
                }
                else
                {
                    ret = false;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (GamePlayManager.Instance.gameBoard.IsPathable(new Vector2Int((int)transform.position.x + j, (int)transform.position.y + i)))
                            {
                                ret = true;
                            }
                        }
                    }
                }
                break;
        }
        return ret;

    }
}
