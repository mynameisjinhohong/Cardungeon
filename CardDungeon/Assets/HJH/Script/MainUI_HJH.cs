using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI_HJH : MonoBehaviour
{
    public Image playerIcon;
    public TMP_Text playerNickName;
    public Image[] hpBar;
    public Image[] mpBar;

    public Sprite hpSprite;
    public Sprite mpSprite;
    public Sprite emptySprite;

    public TMP_Text deckCardAmount;
    public Image mpCoolTime;
    public Image reRollButton;

    public Player_HJH myPlayer;
    public PlayerDeck_HJH playerDeck;

    public ToastMsgContainer toastMsgContainer;

    //카드 클릭했을 때 요소들
    public GameObject bigCard;
    public TMP_Text bigCardMp;
    public TMP_Text bigCardName;
    public TMP_Text bigCardDescribe;
    public Image itemImage;

    //덱 리스트
    public GameObject deckList;
    public GameObject dectContent;
    public GameObject cardPrefab;
    public TMP_Text endText;
    List<GameObject> enforceList;
    int idx = 0;
    // Start is called before the first frame update
    void Start()
    {
        //서버랑 통신해서 닉네임이랑 아이콘 바꾸는거 해야됨
        
    }

    // Update is called once per frame
    void Update()
    {
        int deckAmount = 0;
        deckAmount += playerDeck.deck.Count;
        deckAmount += playerDeck.hand.Count;
        deckAmount += playerDeck.trash.Count;
        deckCardAmount.text = "사용한 카드 " + playerDeck.trash.Count + "/" + deckAmount;
        if (bigCard.activeInHierarchy)
        {
            if(Input.GetMouseButtonDown(0))
            {
                BigCardOff();
            }
        }
    }

    public void ReNewHp()
    {
        for(int i =0; i<hpBar.Length; i++)
        {
            if(myPlayer.HP > i)
            {
                hpBar[i].sprite = hpSprite;
            }
            else
            {
                hpBar[i].sprite = emptySprite;
            }
        }

    }

    public void ReNewMp()
    {
        for (int i = 0; i < mpBar.Length; i++)
        {
            if (myPlayer.Mp > i)
            {
                mpBar[i].sprite = mpSprite;
            }
            else
            {
                mpBar[i].sprite = emptySprite;
            }
        }
    }

    public void Reroll(float reRollCool)
    {
        reRollButton.raycastTarget = false;
        StartCoroutine(RerollButton(reRollCool));
    }
    IEnumerator RerollButton(float reRollCool)
    {
        float currentTime = 0;
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;
            reRollButton.fillAmount = currentTime/reRollCool;
            if(currentTime/reRollCool > 1)
            {
                reRollButton.raycastTarget = true;
                break;
            }
        }
    }

    public void BigCardOn(int cardIdx)
    {
        bigCard.SetActive(true);
        if(cardIdx > 0)
        {
            bigCard.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[cardIdx].bigCardType;
            bigCardMp.text = CardManager.Instance.cardList.cards[cardIdx].useMP.ToString();
            bigCardName.text = CardManager.Instance.cardList.cards[cardIdx].cardName;
            bigCardDescribe.text = CardManager.Instance.cardList.cards[cardIdx].description;
            itemImage.sprite = CardManager.Instance.cardList.cards[cardIdx].itemImage;
        }
        else
        {
            bigCard.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-cardIdx].enforceBigCard;
            bigCardMp.text = CardManager.Instance.cardList.cards[-cardIdx].useMP.ToString();
            bigCardName.text = CardManager.Instance.cardList.cards[-cardIdx].cardName + "+";
            bigCardName.color = Color.yellow;
            bigCardDescribe.text = CardManager.Instance.cardList.cards[-cardIdx].description;
            bigCardDescribe.color = Color.yellow;
            itemImage.sprite = CardManager.Instance.cardList.cards[-cardIdx].itemImage;
        }
    }

    public void BigCardOff()
    {
        bigCard.SetActive(false);
    }
    public void EnforceOn()
    {
        enforceList = new List<GameObject>();
        idx = 0;
        for (int i = 0; i < playerDeck.hand.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            enforceList.Add(card);
            card.GetComponent<Button>().interactable = true;
            card.GetComponent<BigCard_HJH>().idx = idx;
            idx++;
            if (playerDeck.hand[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].itemImage;
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].itemImage;
            }

        }
        for (int i = 0; i < playerDeck.deck.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.GetComponent<Button>().interactable = true;
            card.GetComponent<BigCard_HJH>().idx = idx;
            idx++;
            if (playerDeck.deck[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
            }

        }
        for (int i = 0; i < playerDeck.trash.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.GetComponent<Button>().interactable = true;
            idx++;
            if (playerDeck.trash[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName + "+";
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
            }
        }
        for (int i = 0; i < dectContent.transform.childCount; i++)
        {
            if (i == 0)
            {
                dectContent.transform.GetChild(i).GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                dectContent.transform.GetChild(i).GetChild(4).gameObject.SetActive(false);
            }
        }
        dectContent.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        idx = 0;
        endText.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
        endText.transform.parent.GetComponent<Button>().onClick.AddListener(EnforceEnd);
        endText.text = "선택하기";
        deckList.SetActive(true);
    }

    public void EnforceEnd()
    {
        for(int i =0; i< dectContent.transform.childCount; i++)
        {
            if (dectContent.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
            {
                idx = i;
            }
        }
        Debug.Log(idx);
        if(idx > playerDeck.hand.Count + playerDeck.deck.Count)
        {
            playerDeck.deck[idx - playerDeck.hand.Count - playerDeck.deck.Count] *= -1;
        }
        else if(idx > playerDeck.hand.Count)
        {
            playerDeck.deck[idx - playerDeck.hand.Count] *= -1;
        }
        else
        {
            playerDeck.hand[idx] *= -1;
        }
        deckList.SetActive(false);
        playerDeck.HandVisible();
    }

    public void DeckListOn()
    {
        for(int i =0; i< playerDeck.hand.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.hand[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        for (int i =0; i<playerDeck.deck.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.deck[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        for(int i =0; i<playerDeck.trash.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.trash[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        endText.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
        endText.transform.parent.GetComponent<Button>().onClick.AddListener(DeckListOff);
        endText.text = "돌아가기";
        deckList.SetActive(true);
    }

    public void DeckListOff()
    {
        deckList.SetActive(false);
    }
}
