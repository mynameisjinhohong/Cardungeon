using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeck_HJH : MonoBehaviour
{

    public MainUI_HJH mainUi;
    public float reRollCoolTime;
    //카드의 idx를 가지고 있도록
    public List<int> deck;
    public List<int> trash;
    public List<int> hand;
    public int firstHandCount = 5;
    public int fullHandCount = 7;
    public Transform deckPos;
    public RectTransform trashPos;
    public GameObject[] cards;
    public GameObject[] cardTrash;
    public Ease ease = Ease.OutQuart;
    // Start is called before the first frame update
    void Start()
    {
        SuffelDeck();
        DrawFirst();
    }


    // Update is called once per frame
    void Update()
    {
        if (!mainUi.reRollNow && hand.Count < 1)
        {
            Reroll();
        }

    }
    #region 덱 관리 관련 스크립트
    public void HandVisible() //핸드 업데이트 해주는 함수
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < hand.Count)
            {
                GameObject card = cards[i];
                card.SetActive(true);
                if (hand[i] > 0)
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].cardType;
                    card.GetComponent<Card_HJH>().handIdx = i;
                    card.GetComponent<Card_HJH>().cardIdx = hand[i];
                    card.GetComponent<Card_HJH>().playerDeck = this;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].cardName;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].useMP.ToString(); //나중에 변경
                    card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                    card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].itemImage;
                }
                else
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].enforceSmallCard;
                    card.GetComponent<Card_HJH>().handIdx = i;
                    card.GetComponent<Card_HJH>().cardIdx = hand[i];
                    card.GetComponent<Card_HJH>().playerDeck = this;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].cardName + "+";
                    card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.yellow;
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].useMP.ToString(); //나중에 변경
                    card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                    card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].itemImage;
                }
                Vector3 goPos = card.GetComponent<RectTransform>().anchoredPosition;
                card.GetComponent<RectTransform>().anchoredPosition = deckPos.GetComponent<RectTransform>().anchoredPosition;
                card.GetComponent<RectTransform>().DOAnchorPos(goPos, 1f).SetEase(ease);
                Color co = card.GetComponent<Image>().color;
                co.a = 0;
                card.GetComponent<Image>().color = co;
                card.transform.GetChild(2).GetComponent<Image>().color = co;
                card.GetComponent<Image>().DOFade(1.0f, 1f).SetEase(ease);
                card.transform.GetChild(2).GetComponent<Image>().DOFade(1.0f, 1f).SetEase(ease);
            }
            else
            {
                GameObject card = cards[i];
                card.SetActive(false);
            }

        }
    }
    public void RerollVisible(int su)
    {
        for (int i = 0; i < cardTrash.Length; i++)
        {
            if (i < su)
            {
                GameObject card = cardTrash[i];
                card.SetActive(true);
                if (hand[i] > 0)
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].cardType;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].cardName;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].useMP.ToString(); //나중에 변경
                    card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                    card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].itemImage;
                }
                else
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].enforceSmallCard;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].cardName + "+";
                    card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.yellow;
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].useMP.ToString(); //나중에 변경
                    card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                    card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].itemImage;
                }
                Vector3 goPos = card.GetComponent<RectTransform>().anchoredPosition;
                Color co = card.GetComponent<Image>().color;
                co.a = 1f;
                card.GetComponent<Image>().color = co;
                card.transform.GetChild(2).GetComponent<Image>().color = co;
                Sequence se = DOTween.Sequence(card)
                    .Append(card.GetComponent<RectTransform>().DOAnchorPos(trashPos.anchoredPosition, 1f).SetEase(ease))
                    .Join(card.GetComponent<Image>().DOFade(0.0f, 1f).SetEase(ease))
                    .Join(card.transform.GetChild(2).GetComponent<Image>().DOFade(0.0f, 1f).SetEase(ease))
                    .Join(card.transform.GetChild(0).GetComponent<TMP_Text>().DOFade(0.0f,1f).SetEase(ease))
                    .Join(card.transform.GetChild(1).GetComponent<TMP_Text>().DOFade(0.0f,1f).SetEase(ease))
                    .Append(DOTween.To(()=>0f, x => card.SetActive(false),0f,0f))
                    .Append(card.GetComponent<RectTransform>().DOAnchorPos(goPos, 1f).SetEase(ease));
                se.Play();
            }
            else
            {
                GameObject card = cardTrash[i];
                card.SetActive(false);
            }
        }
    }

    public void SuffelDeck()
    {
        int n = deck.Count;
        for (int i = 0; i < n; i++)
        {
            int idx = Random.Range(0, n);
            int a = deck[idx];
            deck[idx] = deck[i];
            deck[i] = a;
        }
    }

    public void DrawFirst()
    {
        for (int i = 0; i < firstHandCount; i++)
        {
            if (deck.Count > 0)
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
        if (deck.Count > 0)
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
    public void ButtonReroll()
    {
        if (mainUi.reRollNow)
        {
            mpReroll();
        }
        else
        {
            Reroll();
        }
    }

    public void Reroll()
    {
        int hd = hand.Count;
        RerollVisible(hd);
        for (int i = 0; i < hd; i++)
        {
            int a = hand[0];
            hand.RemoveAt(0);
            trash.Add(a);
        }
        mainUi.Reroll(reRollCoolTime);
        DrawFirst();
    }

    public void mpReroll()
    {

        if (GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp > 0)
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp--;
            int hd = hand.Count;
            RerollVisible(hd);
            for (int i = 0; i < hd; i++)
            {
                int a = hand[0];
                hand.RemoveAt(0);
                trash.Add(a);
            }
            DrawFirst();
        }
        else
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp--;
        }

    }


    public bool UseCard(int handIdx)
    {
        int a = hand[handIdx];

        if (GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp >= CardManager.Instance.cardList.cards[Mathf.Abs(a)].useMP && CardManager.Instance.OnCardCheck(GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx], a))
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp -= CardManager.Instance.cardList.cards[Mathf.Abs(a)].useMP;
            GamePlayManager.Instance.CardGo(GamePlayManager.Instance.myIdx, a);
            hand.RemoveAt(handIdx);
            trash.Add(a);
            HandVisible();
            return true;
        }
        else
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp -= CardManager.Instance.cardList.cards[Mathf.Abs(a)].useMP;
            return false;
        }

    }

    public void TrashToDeck()
    {
        int tr = trash.Count;
        for (int i = 0; i < tr; i++)
        {
            int a = trash[0];
            trash.RemoveAt(0);
            deck.Add(a);
        }
    }
    #endregion

}
