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
    public GameObject tileEffect;
    public List<GameObject> tileEffects;
    #region �巡�� �� ���

    void ChildRayCast(bool onOff)
    {
        GetComponent<Image>().raycastTarget = onOff;
        transform.GetChild(0).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(1).GetComponent<TMP_Text>().raycastTarget = onOff;
        transform.GetChild(2).GetComponent<Image>().raycastTarget = onOff;
    }
    void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        defaultPos = transform.position;
        ChildRayCast(false);
        switch (Mathf.Abs(cardIdx))
        {
            case 1:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 2, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                }
                break;
            case 2:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 2, playerPos.y, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                }
                break;
            case 3:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tileEffect.transform.position = new Vector3(playerPos.x - 2, playerPos.y, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x - 1, playerPos.y, 0);
                }
                break;
            case 4:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y -2, 0);
                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y - 1, 0);
                }
                break;
            case 5:
                if (cardIdx < 0)
                {
                    for(int i =-3; i<4; i++)
                    {
                        for(int j = -3; j<4; j++)
                        {
                            if(i==0&& j == 0)
                            {
                                continue;
                            }
                            GameObject tile = Instantiate(tileEffect);
                            tileEffects.Add(tile);
                            Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                            tile.transform.position = new Vector3(playerPos.x +i , playerPos.y + j, 0);
                        }
                    }

                }
                else
                {
                    for (int i = -2; i < 3; i++)
                    {
                        for (int j = -2; j < 3; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }
                            GameObject tile = Instantiate(tileEffect);
                            tileEffects.Add(tile);
                            Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                            tile.transform.position = new Vector3(playerPos.x + i, playerPos.y + j, 0);
                            Debug.Log(tile.transform.position);
                        }
                    }
                }
                break;
            case 6:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                    GameObject tile2 = Instantiate(tileEffect);
                    tileEffects.Add(tile2);
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + -1, 0);

                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y + 1, 0);
                    GameObject tile2 = Instantiate(tileEffect);
                    tileEffects.Add(tile2);
                    tile.transform.position = new Vector3(playerPos.x, playerPos.y - 1, 0);
                }
                break;
            case 7:
                if (cardIdx < 0)
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                    GameObject tile2 = Instantiate(tileEffect);
                    tileEffects.Add(tile2);
                    tile.transform.position = new Vector3(playerPos.x - 1, playerPos.y , 0);

                }
                else
                {
                    GameObject tile = Instantiate(tileEffect);
                    tileEffects.Add(tile);
                    Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                    tile.transform.position = new Vector3(playerPos.x + 1, playerPos.y, 0);
                    GameObject tile2 = Instantiate(tileEffect);
                    tileEffects.Add(tile2);
                    tile.transform.position = new Vector3(playerPos.x - 1, playerPos.y, 0);
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
                        GameObject tile = Instantiate(tileEffect);
                        tileEffects.Add(tile);
                        Vector3 playerPos = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.position;
                        tile.transform.position = new Vector3(playerPos.x + i, playerPos.y + j, 0);
                    }
                }
                break;
        }

    }
    void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        transform.position = currentPos;
    }
    void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        for(int i =0; i< tileEffects.Count; i++)
        {
            Destroy(tileEffects[i]);
        }
        tileEffects.Clear();
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (!playerDeck.UseCard(handIdx))
            {
                transform.position = defaultPos;
                ChildRayCast(true);
            }
            else
            {
                Instantiate(cardEffect, transform);
                ChildRayCast(true);
            }
        }
        else
        {
            transform.position = defaultPos;
            GetComponent<Image>().raycastTarget = true;
            ChildRayCast(true);
        }
    }

    public void OnClick()
    {
        ChildRayCast(true);
        playerDeck.mainUi.BigCardOn(cardIdx);
    }
    #endregion
}
