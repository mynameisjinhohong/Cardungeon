using DG.Tweening;
using System.Collections;
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
    public List<Sprite> keyPadImageList;
    public List<Vector3> cardsAnc;
    public List<Vector3> cardsTrashAnc;
    public Ease ease = Ease.OutQuart;
    bool stun = false;
    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cardsAnc.Add(cards[i].GetComponent<RectTransform>().anchoredPosition);
        }
        for (int i = 0; i < cardTrash.Length; i++)
        {
            cardsTrashAnc.Add(cardTrash[i].GetComponent<RectTransform>().anchoredPosition);
        }
    }

    void Start()
    {
        SuffelDeck();
        DrawFirst(0);
        HandVisible();
    }


    // Update is called once per frame
    void Update()
    {
        if (!mainUi.reRollNow && hand.Count < 1)
        {
            Reroll();
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hand.Count <= 0) return;
            
            cards[0].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hand.Count <= 1) return;
            
            cards[1].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hand.Count <= 2) return;
            
            cards[2].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (hand.Count <= 3) return;
            
            cards[3].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (hand.Count <= 4) return;
            
            cards[4].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (hand.Count <= 5) return;
            
            cards[5].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (hand.Count <= 6) return;
            
            cards[6].GetComponent<Card_HJH>().OnBeginUseCard();
        }
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (hand.Count <= 0) return;
            
            cards[0].GetComponent<Card_HJH>().EndUseCard(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (hand.Count <= 1) return;
            
            cards[1].GetComponent<Card_HJH>().EndUseCard(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (hand.Count <= 2) return;
            
            cards[2].GetComponent<Card_HJH>().EndUseCard(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            if (hand.Count <= 3) return;
            
            cards[3].GetComponent<Card_HJH>().EndUseCard(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            if (hand.Count <= 4) return;
            
            cards[4].GetComponent<Card_HJH>().EndUseCard(false);

        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            if (hand.Count <= 5) return;
            
            cards[5].GetComponent<Card_HJH>().EndUseCard(false);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            if (hand.Count <= 6) return;
            
            cards[6].GetComponent<Card_HJH>().EndUseCard(false);
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
                    card.GetComponent<Card_HJH>().keyPadImage.sprite = keyPadImageList[i];
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
                    card.GetComponent<Card_HJH>().keyPadImage.gameObject.SetActive(false);
                }

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
                GameObject cardT = cardTrash[i];
                cardT.SetActive(true);
                if (hand[i] > 0)
                {
                    cardT.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].cardType;
                    cardT.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].cardName;
                    cardT.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
                    cardT.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[i]].useMP.ToString(); //나중에 변경
                    cardT.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                    cardT.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[i]].itemImage;
                }
                else
                {
                    cardT.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].enforceSmallCard;
                    cardT.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].cardName + "+";
                    cardT.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.yellow;
                    cardT.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[i]].useMP.ToString(); //나중에 변경
                    cardT.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                    cardT.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[i]].itemImage;
                }
                Vector3 goPos = cardsTrashAnc[i];
                Color co2 = cardT.GetComponent<Image>().color;
                co2.a = 1f;
                cardT.GetComponent<Image>().color = co2;
                cardT.transform.GetChild(2).GetComponent<Image>().color = co2;
                DG.Tweening.Sequence se = DOTween.Sequence(cardT)
                    .Append(cardT.GetComponent<RectTransform>().DOAnchorPos(trashPos.anchoredPosition, 1f).SetEase(ease))
                    .Join(cardT.GetComponent<Image>().DOFade(0.0f, 1f).SetEase(ease))
                    .Join(cardT.transform.GetChild(2).GetComponent<Image>().DOFade(0.0f, 1f).SetEase(ease))
                    .Join(cardT.transform.GetChild(0).GetComponent<TMP_Text>().DOFade(0.0f, 1f).SetEase(ease))
                    .Join(cardT.transform.GetChild(1).GetComponent<TMP_Text>().DOFade(0.0f, 1f).SetEase(ease))
                    .Append(DOTween.To(() => 0f, x => cardT.SetActive(false), 0f, 0f))
                    .Append(cardT.GetComponent<RectTransform>().DOAnchorPos(goPos, 1f).SetEase(ease));
                se.Play();
            }
            else
            {
                GameObject card = cardTrash[i];
                card.SetActive(false);
            }
        }
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
                Vector3 goCard = cardsAnc[i];
                card.GetComponent<RectTransform>().anchoredPosition = deckPos.GetComponent<RectTransform>().anchoredPosition;
                card.GetComponent<RectTransform>().DOAnchorPos(goCard, 1f).SetEase(ease);
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

    public void DrawFirst(int hd)
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
        RerollVisible(hd);
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
            else
            {
                return;
            }
        }
        else
        {
            TrashToDeck();
            SuffelDeck();
            DrawOne();
            return;
        }
        GameObject card = cards[hand.Count - 1];
        card.SetActive(true);
        if (hand[hand.Count - 1] > 0)
        {
            card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[hand.Count - 1]].cardType;
            card.GetComponent<Card_HJH>().handIdx = hand.Count - 1;
            card.GetComponent<Card_HJH>().cardIdx = hand[hand.Count - 1];
            card.GetComponent<Card_HJH>().playerDeck = this;
            card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[hand.Count - 1]].cardName;
            card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.white;
            card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[hand[hand.Count - 1]].useMP.ToString(); //나중에 변경
            card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
            card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[hand[hand.Count - 1]].itemImage;
        }
        else
        {
            card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[hand.Count - 1]].enforceSmallCard;
            card.GetComponent<Card_HJH>().handIdx = hand.Count - 1;
            card.GetComponent<Card_HJH>().cardIdx = hand[hand.Count - 1];
            card.GetComponent<Card_HJH>().playerDeck = this;
            card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[hand.Count - 1]].cardName + "+";
            card.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.yellow;
            card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-hand[hand.Count - 1]].useMP.ToString(); //나중에 변경
            card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
            card.transform.GetChild(2).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-hand[hand.Count - 1]].itemImage;
        }
        Vector3 goCard = card.GetComponent<RectTransform>().anchoredPosition;
        card.GetComponent<RectTransform>().anchoredPosition = deckPos.GetComponent<RectTransform>().anchoredPosition;
        card.GetComponent<RectTransform>().DOAnchorPos(goCard, 1f).SetEase(ease);
        Color co = card.GetComponent<Image>().color;
        co.a = 0;
        card.GetComponent<Image>().color = co;
        card.transform.GetChild(2).GetComponent<Image>().color = co;
        card.GetComponent<Image>().DOFade(1.0f, 1f).SetEase(ease);
        card.transform.GetChild(2).GetComponent<Image>().DOFade(1.0f, 1f).SetEase(ease);
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
        for (int i = 0; i < hd; i++)
        {
            int a = hand[0];
            hand.RemoveAt(0);
            trash.Add(a);
        }
        mainUi.Reroll(reRollCoolTime);
        DrawFirst(hd);
    }

    public void mpReroll()
    {
        if (GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp > 0)
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp--;
            int hd = hand.Count;
            for (int i = 0; i < hd; i++)
            {
                int a = hand[0];
                hand.RemoveAt(0);
                trash.Add(a);
            }
            DrawFirst(hd);
        }
        else
        {
            GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp--;
        }
    }


    public bool UseCard(int handIdx)
    {
        int a = hand[handIdx];
        if (GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].Mp >= CardManager.Instance.cardList.cards[Mathf.Abs(a)].useMP && CardManager.Instance.OnCardCheck(GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx], a) && !stun)
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
            if (stun)
            {
                GamePlayManager.Instance.mainUi.toastMsgContainer.AddMessage("덫에걸려 움직일수 없습니다", 3.0f);
                GameObject player = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].gameObject;
                Vector3 lo = GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx].transform.localPosition;
                DG.Tweening.Sequence se = DOTween.Sequence(GamePlayManager.Instance.players[GamePlayManager.Instance.myIdx])
                    .Append(player.transform.DOLocalMoveX(0.05f, 0.05f))
                    .Append(player.transform.DOLocalMoveX(-0.1f, 0.05f))
                    .Append(player.transform.DOLocalMoveX(0.1f, 0.05f))
                    .Append(player.transform.DOLocalMoveX(-0.05f, 0.05f))
                    .Append(player.transform.DOLocalMove(lo, 0.05f));
                se.Play();
            }
            AudioPlayer.Instance.PlayClip(12);
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

    public void StunGo(float time)
    {
        StartCoroutine(Stun(time));
    }

    IEnumerator Stun(float time)
    {
        stun = true;
        yield return new WaitForSeconds(time);
        stun = false;

    }

}
