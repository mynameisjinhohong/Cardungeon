using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDeck_HJH : MonoBehaviour
{

    public MainUI_HJH mainUi;
    //카드의 idx를 가지고 있도록
    public List<int> deck;
    public List<int> trash;
    public List<int> hand;
    public int firstHandCount = 5;
    public int fullHandCount = 7;
    public GameObject[] cards;
    // Start is called before the first frame update
    void Start()
    {
        SuffelDeck();
        DrawFirst();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    #region 덱 관리 관련 스크립트
    public void HandVisible() //핸드 업데이트 해주는 함수
    {
        for(int i = 0; i<cards.Length; i++)
        {
            if(i < hand.Count)
            {
                GameObject card = cards[i];
                card.SetActive(true);
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].cardType;
                card.GetComponent<Card_HJH>().handIdx = i;
                card.GetComponent<Card_HJH>().cardIdx = hand[i];
                card.GetComponent<Card_HJH>().playerDeck = this;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].useMP.ToString(); //나중에 변경

            }
            else
            {
                GameObject card = cards[i];
                card.SetActive(false);
            }

        }
    }

    public void SuffelDeck()
    {
        int n = deck.Count;
        for(int i = 0; i < n; i++)
        {
            int idx = Random.Range(0, n);
            int a = deck[idx];
            deck[idx] = deck[i];
            deck[i] = a;
        }
    }

    public void DrawFirst()
    {
        for(int i =0; i<firstHandCount; i++)
        {
            if(deck.Count > 0)
            {
                int a = deck[0];
                deck.RemoveAt(0);
                hand.Add(a);
            }
            else
            {
                TrashToDeck();
                SuffelDeck();
                i--;
            }
        }
        HandVisible();
    }
    public void DrawOne()
    {
        if(deck.Count > 0)
        {
            if (hand.Count < fullHandCount)
            {
                int a = deck[0];
                deck.RemoveAt(0);
                hand.Add(a);
            }
        }
        else
        {
            TrashToDeck();
            SuffelDeck();
            DrawOne();
        }
        HandVisible();
    }

    public void Reroll()
    {
        int hd = hand.Count;
        for(int i =0; i< hd; i++)
        {
            int a = hand[0];
            hand.RemoveAt(0);
            trash.Add(a);
        }
        DrawFirst();
    }

    public void mpReroll()
    {
        int hd = hand.Count;
        for (int i = 0; i < hd; i++)
        {
            int a = hand[0];
            hand.RemoveAt(0);
            trash.Add(a);
        }
        DrawFirst();
    }


    public bool UseCard(int handIdx)
    {
        int a = hand[handIdx];
        if (GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp >= CardManager.Instance.cardList.cards[a].useMP)
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp -= CardManager.Instance.cardList.cards[a].useMP;
            GamePlayManager.Instance.CardGo(GamePlayManager.Instance.myIdx, a);
            hand.RemoveAt(handIdx);
            trash.Add(a);
            HandVisible();
            return true;
        }
        else
        {
            return false;
        }

    }

    public void TrashToDeck()
    {
        int tr = trash.Count;
        for(int i =0; i<tr; i++)
        {
            int a = trash[0];
            trash.RemoveAt(0);
            deck.Add(a);
        }
    }
    #endregion

}
