using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card_HJH : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int cardIdx; //� ī������
    public int handIdx; //�ڵ忡�� ���° ī������
    public PlayerDeck_HJH playerDeck;
    public Vector2 defaultPos;
    public GameObject cardEffect;
    public GameObject[] tileEffect; // 0 - move, 1 - attack
    public List<GameObject> tileEffects;
    #region �巡�� �� ���

    private void Update()
    {
    }

    void ChildRayCast(bool onOff)
    {
        GetComponent<Image>().raycastTarget = onOff;
        transform.GetChild(0).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(1).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(2).GetComponent<Image>().raycastTarget = onOff;
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        OnBeginUseCard();
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = currentPos;
    }

    public void OnBeginUseCard()
    {
        defaultPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        ChildRayCast(false);
        switch (Mathf.Abs(cardIdx))
        {
            case 1:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 2, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                }
                break;
            case 2:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 2, playerPos.y, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                }
                break;
            case 3:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tileEffect[0].transform.position = new Vector3(playerPos.x - 2, playerPos.y, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x - 1, playerPos.y, 0);
                }
                break;
            case 4:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y -2, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[0]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y - 1, 0);
                }
                break;
            case 5:
                if (cardIdx < 0)
                {
                    for(int i =-2; i<3; i++)
                    {
                        for(int j = -2; j<3; j++)
                        {
                            if(i==0&& j == 0)
                            {
                                continue;
                            }
                            GameObject tile = Instantiate(tileEffect[0]);
                            tileEffects.Add(tile);
                            Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                            tile.transform.position = new Vector3(playerPos.x +i , playerPos.y + j, 0);
                        }
                    }

                }
                else
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }
                            GameObject tile = Instantiate(tileEffect[0]);
                            tileEffects.Add(tile);
                            Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                            tile.transform.position = new Vector3(playerPos.x + i, playerPos.y + j, 0);
                        }
                    }
                }
                break;
            case 6:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                    GameObject tile2 = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile2);
                    tile2.transform.position = new Vector3(playerPos.x, playerPos.y - 1, 0);

                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                    GameObject tile2 = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile2);
                    tile2.transform.position = new Vector3(playerPos.x, playerPos.y - 1, 0);
                }
                break;
            case 7:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                    Debug.Log(tile.transform.position);
                    GameObject tile2 = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile2);
                    tile2.transform.position = new Vector3(playerPos.x - 1, playerPos.y , 0);
                    
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                    GameObject tile2 = Instantiate(tileEffect[1]);
                    tileEffects.Add(tile2);
                    tile2.transform.position = new Vector3(playerPos.x - 1, playerPos.y, 0);
                }
                break;
            case 8:
                for(int i =-1; i < 2; i++)
                {
                    for(int j = -1; j <2; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        GameObject tile = Instantiate(tileEffect[1]);
                        tileEffects.Add(tile);
                        Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                        tile.transform.position = new Vector3(playerPos.x + i, playerPos.y + j, 0);
                    }
                }
                break;
        }

    }

    public void EndUseCard(bool isDrag)
    {
        for(int i =0; i< tileEffects.Count; i++)
        {
            Destroy(tileEffects[i]);
        }
        tileEffects.Clear();

        if (isDrag)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && Mathf.Abs(((Vector2)gameObject.GetComponent<RectTransform>().anchoredPosition - defaultPos).magnitude) > 75)
            {
                if (!playerDeck.UseCard(handIdx))
                {
                    Debug.Log(handIdx + "번 사용처리");
                    gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPos;
                    ChildRayCast(true);
                }
                else
                {
                    Debug.Log(handIdx + "번 사용처리2");
                    gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPos;
                    Instantiate(cardEffect, transform);
                    ChildRayCast(true);
                }
            }
            else
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPos;
                GetComponent<Image>().raycastTarget = true;
                ChildRayCast(true);
            }
        }
        else
        {
            if (!playerDeck.UseCard(handIdx))
            {
                Debug.Log(handIdx + "번 사용처리");
                gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPos;
                ChildRayCast(true);
            }
            else
            {
                Debug.Log(handIdx + "번 사용처리2");
                gameObject.GetComponent<RectTransform>().anchoredPosition = defaultPos;
                Instantiate(cardEffect, transform);
                ChildRayCast(true);
            }
        }
    }
    
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        EndUseCard(true);
    }

    public void OnClick()
    {
        ChildRayCast(true);
        playerDeck.mainUi.BigCardOn(cardIdx);
    }
    #endregion
}
