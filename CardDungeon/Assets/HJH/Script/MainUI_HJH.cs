using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI_HJH : MonoBehaviour
{
    public Image playerIcon;
    public Image playerBG;
    public Image[] hpBar;
    public Image[] mpBar;

    public Sprite hpSprite;
    public Sprite mpSprite;
    public Sprite emptySprite;

    public TMP_Text deckCardAmount;
    public TMP_Text trashCardAmount;

    public Image mpCoolTime;
    public Image reRollButton;
    public Image reRollCoolTime;

    public Player_HJH myPlayer;
    public PlayerDeck_HJH playerDeck;

    public ToastMsgContainer toastMsgContainer;
    public GameObject winImage;

    //카드 클릭했을 때 요소들
    public GameObject bigCard;
    public Image bigCardImg;
    public TMP_Text bigCardMp;
    public TMP_Text bigCardName;
    public TMP_Text bigCardDescribe;
    public Image itemImage;

    //덱 리스트
    public GameObject deckList;
    public GameObject dectContent;
    public GameObject cardPrefab;
    public TMP_Text endText;
    int idx = 0;

    public Sprite[] icons;
    bool firstSet = false;

    public GameObject gameOver;

    public List<Sprite> ToonList;

    public Image Toon;

    public GameObject ToonBG;

    //강화 or 삭제
    public GameObject allList;
    public GameObject threeList;
    public GameObject twoList;
    public GameObject oneList;
    public GameObject activeButton;
    int cardSize = 0;
    //큰 미니맵 작은 미니맵
    public GameObject bigMinimap;
    public GameObject smallMinimap;

    public bool reRollNow = false;

    public List<GameObject> keys = new List<GameObject>();

    private void OnEnable()
    {
        myPlayer.KeysOnValueChanged += SetKeysUI;
    }

    private void OnDisable()
    {
        myPlayer.KeysOnValueChanged -= SetKeysUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ToonStart());
    }

    IEnumerator ToonStart()
    {
        ToonBG.SetActive(true);

        yield return new WaitForSeconds(4.5f);

        GetComponent<Canvas>().sortingOrder = 200;

        ToonBG.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstSet)
        {
            if (myPlayer != null)
            {
                playerIcon.sprite = icons[GamePlayManager.Instance.myIdx];
                firstSet = true;
            }

        }
        int deckAmount = 0;
        deckAmount += playerDeck.deck.Count;
        deckAmount += playerDeck.hand.Count;
        deckAmount += playerDeck.trash.Count;

        trashCardAmount.text = playerDeck.trash.Count.ToString();
        deckCardAmount.text = playerDeck.deck.Count.ToString();

        if (bigCard.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                BigCardOff();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (deckList.activeInHierarchy)
            {
                DeckListOff();
            }
            else
            {
                DeckListOn();
            }
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (deckList.activeInHierarchy)
            {
                DeckListOff();
            }
            else
            {
                TrashDeckListOn();
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MinimapChange();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            playerDeck.ButtonReroll();
        }
    }

    public void ReNewHp()
    {
        for (int i = 0; i < hpBar.Length; i++)
        {
            if (myPlayer.HP > i)
            {
                hpBar[i].sprite = hpSprite;
            }
            else
            {
                hpBar[i].sprite = emptySprite;
            }
        }

    }

    public void MinimapChange()
    {
        if (bigMinimap.activeInHierarchy)
        {
            bigMinimap.SetActive(false);
            smallMinimap.SetActive(true);
        }
        else
        {
            bigMinimap.SetActive(true);
            smallMinimap.SetActive(false);
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
        reRollNow = true;
        StartCoroutine(RerollButton(reRollCool));
    }
    IEnumerator RerollButton(float reRollCool)
    {
        float currentTime = 0;
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;
            reRollButton.fillAmount = currentTime / reRollCool;
            reRollCoolTime.fillAmount = currentTime / reRollCool;
            if (currentTime / reRollCool > 1)
            {
                reRollButton.raycastTarget = true;
                reRollNow = false;
                break;
            }
        }
    }

    public void BigCardOn(int cardIdx)
    {
        bigCard.SetActive(true);
        if (cardIdx > 0)
        {
            bigCardImg.sprite = CardManager.Instance.cardList.cards[cardIdx].bigCardType;
            bigCardMp.text = CardManager.Instance.cardList.cards[cardIdx].useMP.ToString();
            bigCardName.text = CardManager.Instance.cardList.cards[cardIdx].cardName;
            bigCardDescribe.text = CardManager.Instance.cardList.cards[cardIdx].description;
            itemImage.sprite = CardManager.Instance.cardList.cards[cardIdx].itemImage;
        }
        else
        {
            bigCardImg.sprite = CardManager.Instance.cardList.cards[-cardIdx].enforceBigCard;
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
    #region 카드 강화 삭제

    public int[] RandomCard(bool del)
    {
        List<int> canCardList = new List<int>();
        int all = playerDeck.hand.Count + playerDeck.deck.Count + playerDeck.trash.Count;
        for (int i = 0; i < all; i++)
        {
            if (i >= playerDeck.hand.Count + playerDeck.deck.Count)
            {
                if (playerDeck.trash[i - playerDeck.hand.Count - playerDeck.deck.Count] > 0)
                {
                    canCardList.Add(i);
                }
            }
            else if (i >= playerDeck.hand.Count)
            {
                if (playerDeck.deck[i - playerDeck.hand.Count] > 0)
                {
                    canCardList.Add(i);
                }
            }
            else
            {
                if (playerDeck.hand[i] > 0)
                {
                    canCardList.Add(i);
                }
            }

        }
        int[] ran;
        Debug.Log(canCardList.Count);
        if (canCardList.Count == 1)
        {
            ran = new int[1];
            ran[0] = canCardList[0];
            cardSize = 1;
        }
        else if (canCardList.Count == 2)
        {
            ran = new int[2];
            ran[0] = canCardList[0];
            ran[1] = canCardList[1];
            cardSize = 2;
        }
        else if(canCardList.Count >= 3)
        {
            ran = new int[3];
            int a = Random.Range(0, canCardList.Count - 2);
            ran[0] = canCardList[a];
            a = Random.Range(a + 1, canCardList.Count - 1);
            ran[1] = canCardList[a];
            a = Random.Range(a + 1, canCardList.Count);
            ran[2] = canCardList[a];
            cardSize = 3;
        }
        else
        {
            ran = null;
        }
        return ran;
    }
    public void EnforceOn()
    {
        int[] ran = RandomCard(false);
        Debug.Log(ran);
        if(ran != null)
        {
            allList.SetActive(true);
            switch (ran.Length)
            {
                case 1:
                    threeList.SetActive(false);
                    twoList.SetActive(false);
                    oneList.SetActive(true);
                    break;
                case 2:
                    threeList.SetActive(false);
                    twoList.SetActive(true);
                    oneList.SetActive(false);
                    break;
                case 3:
                    threeList.SetActive(true);
                    twoList.SetActive(false);
                    oneList.SetActive(false);
                    break;
            }
            for (int i = 0; i < ran.Length; i++)
            {
                GameObject card = threeList.transform.GetChild(i).gameObject;
                switch (ran.Length)
                {
                    case 1:
                        card = oneList.transform.GetChild(i).gameObject;
                        break;
                    case 2:
                        card = twoList.transform.GetChild(i).gameObject;
                        break;
                    case 3:
                        card = threeList.transform.GetChild(i).gameObject;
                        break;
                }
                card.GetComponent<Button>().interactable = true;
                card.GetComponent<BigCard_HJH>().idx = ran[i];
                card.GetComponent<BigCard_HJH>().imOn = false;
                card.transform.GetChild(4).gameObject.SetActive(false);
                card.transform.GetChild(5).gameObject.SetActive(false);
                if (ran[i] >= playerDeck.hand.Count + playerDeck.deck.Count)
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].bigCardType;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].useMP.ToString();
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].cardName;
                    card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].description;
                    card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].itemImage;
                    GameObject card2 = card.transform.GetChild(5).gameObject;
                    card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].bigCardType;
                    card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].useMP.ToString();
                    card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].cardName;
                    card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].description;
                    card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].itemImage;

                }
                else if (ran[i] >= playerDeck.hand.Count)
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].bigCardType;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].useMP.ToString();
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].cardName;
                    card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].description;
                    card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].itemImage;
                    GameObject card2 = card.transform.GetChild(5).gameObject;
                    card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].bigCardType;
                    card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].useMP.ToString();
                    card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].cardName;
                    card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].description;
                    card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].itemImage;

                }
                else
                {
                    card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].bigCardType;
                    card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].useMP.ToString();
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].cardName;
                    card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].description;
                    card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].itemImage;
                    GameObject card2 = card.transform.GetChild(5).gameObject;
                    card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].bigCardType;
                    card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].useMP.ToString();
                    card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].cardName;
                    card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].description;
                    card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].itemImage;
                }
            }
            idx = 0;
            activeButton.GetComponent<Button>().onClick.RemoveAllListeners();
            activeButton.GetComponent<Button>().onClick.AddListener(EnforceEnd);
            activeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "카드 강화하기";
            allList.SetActive(true);
        }
        else
        {
            myPlayer.EnforceTextOn();
        }

    }
    public void EnforceOff()
    {
        allList.SetActive(false);
    }
    public void EnforceEnd()
    {
        switch (cardSize)
        {
            case 1:
                for (int i = 0; i < 1; i++)
                {
                    if (oneList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = oneList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    if (twoList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = twoList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    if (threeList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = threeList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
        }
        if (idx >= playerDeck.hand.Count + playerDeck.deck.Count)
        {
            if (playerDeck.trash[idx - playerDeck.hand.Count - playerDeck.deck.Count] > 0)
            {
                playerDeck.trash[idx - playerDeck.hand.Count - playerDeck.deck.Count] *= -1;
            }
        }
        else if (idx >= playerDeck.hand.Count)
        {
            if (playerDeck.deck[idx - playerDeck.hand.Count] > 0)
            {
                playerDeck.deck[idx - playerDeck.hand.Count] *= -1;
            }
        }
        else
        {
            if (playerDeck.hand[idx] > 0)
            {
                playerDeck.hand[idx] *= -1;
            }
        }
        allList.SetActive(false);
        playerDeck.HandVisible();
    }
    public void DeleteStart()
    {
        int[] ran = RandomCard(true);
        allList.SetActive(true);
        switch (ran.Length)
        {
            case 1:
                threeList.SetActive(false);
                twoList.SetActive(false);
                oneList.SetActive(true);
                break;
            case 2:
                threeList.SetActive(false);
                twoList.SetActive(true);
                oneList.SetActive(false);
                break;
            case 3:
                threeList.SetActive(true);
                twoList.SetActive(false);
                oneList.SetActive(false);
                break;
        }
        for (int i = 0; i < ran.Length; i++)
        {
            GameObject card = threeList.transform.GetChild(i).gameObject;
            switch (ran.Length)
            {
                case 1:
                    card = oneList.transform.GetChild(i).gameObject;
                    break;
                case 2:
                    card = twoList.transform.GetChild(i).gameObject;
                    break;
                case 3:
                    card = threeList.transform.GetChild(i).gameObject;
                    break;
            }
            card.GetComponent<Button>().interactable = true;
            card.GetComponent<BigCard_HJH>().idx = ran[i];
            card.GetComponent<BigCard_HJH>().imOn = false;
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.transform.GetChild(5).gameObject.SetActive(false);
            if (ran[i] >= playerDeck.hand.Count + playerDeck.deck.Count)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].itemImage;
                GameObject card2 = card.transform.GetChild(5).gameObject;
                card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].bigCardType;
                card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].useMP.ToString();
                card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].cardName;
                card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].description;
                card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[ran[i] - playerDeck.hand.Count - playerDeck.deck.Count]].itemImage;

            }
            else if (ran[i] >= playerDeck.hand.Count)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].itemImage;
                GameObject card2 = card.transform.GetChild(5).gameObject;
                card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].bigCardType;
                card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].useMP.ToString();
                card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].cardName;
                card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].description;
                card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[ran[i] - playerDeck.hand.Count]].itemImage;

            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].cardName;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].description;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].itemImage;
                GameObject card2 = card.transform.GetChild(5).gameObject;
                card2.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].bigCardType;
                card2.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].useMP.ToString();
                card2.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].cardName;
                card2.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].description;
                card2.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[ran[i]]].itemImage;
            }
        }
        idx = 0;
        activeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        activeButton.GetComponent<Button>().onClick.AddListener(DeleteEnd);
        activeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "카드 삭제하기";
        allList.SetActive(true);
    }

    public void DeleteEnd()
    {
        switch (cardSize)
        {
            case 1:
                for (int i = 0; i < 1; i++)
                {
                    if (oneList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = oneList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    if (twoList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = twoList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    if (threeList.transform.GetChild(i).GetComponent<BigCard_HJH>().imOn)
                    {
                        idx = threeList.transform.GetChild(i).GetComponent<BigCard_HJH>().idx;
                    }
                }
                break;
        }

        if (idx >= playerDeck.hand.Count + playerDeck.deck.Count)
        {
            playerDeck.trash.RemoveAt(idx - playerDeck.hand.Count - playerDeck.deck.Count);
        }
        else if (idx >= playerDeck.hand.Count)
        {
            playerDeck.deck.RemoveAt(idx - playerDeck.hand.Count);
        }
        else
        {
            playerDeck.hand.RemoveAt(idx);

        }
        allList.SetActive(false);
        playerDeck.HandVisible();
    }


    #endregion
    #region 덱,묘지 리스트
    public void FullDeckListOn()
    {
        for (int i = 0; i < dectContent.transform.childCount; i++)
        {
            Destroy(dectContent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < playerDeck.hand.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.hand[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.hand[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.hand[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.hand[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.hand[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.hand[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.hand[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.hand[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        for (int i = 0; i < playerDeck.deck.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.deck[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        for (int i = 0; i < playerDeck.trash.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.trash[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        endText.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
        endText.transform.parent.GetComponent<Button>().onClick.AddListener(DeckListOff);
        endText.text = "돌아가기";
        deckList.SetActive(true);
    }

    public void TrashDeckListOn()
    {
        for (int i = 0; i < dectContent.transform.childCount; i++)
        {
            Destroy(dectContent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < playerDeck.trash.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.trash[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.trash[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.trash[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }

        }
        endText.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();
        endText.transform.parent.GetComponent<Button>().onClick.AddListener(DeckListOff);
        endText.text = "돌아가기";
        deckList.SetActive(true);
    }

    public void DeckListOn()
    {
        for (int i = 0; i < dectContent.transform.childCount; i++)
        {
            Destroy(dectContent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < playerDeck.deck.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, dectContent.transform);
            card.transform.GetChild(4).gameObject.SetActive(false);
            card.GetComponent<Button>().interactable = false;
            if (playerDeck.deck[i] > 0)
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].bigCardType;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].cardName;
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.white;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[playerDeck.deck[i]].itemImage;
                card.transform.GetChild(4).gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].enforceBigCard;
                card.transform.GetChild(0).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].useMP.ToString();
                card.transform.GetChild(1).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].cardName + "+";
                card.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(2).GetComponent<TMP_Text>().text = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].description;
                card.transform.GetChild(2).GetComponent<TMP_Text>().color = Color.yellow;
                card.transform.GetChild(3).GetComponent<Image>().sprite = CardManager.Instance.cardList.cards[-playerDeck.deck[i]].itemImage;
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
    #endregion
    public void GameOver()
    {
        gameOver.SetActive(true);

    }
    public void GotoLobby()
    {
        SceneManager.LoadScene(0);
    }
    public void LookAround()
    {
        float disTance = float.MaxValue;
        int idx = 0;
        for (int i = 0; i < GamePlayManager.Instance.players.Count; i++)
        {
            Transform myPos = myPlayer.transform;
            if (i != GamePlayManager.Instance.myIdx)
            {
                if ((GamePlayManager.Instance.players[i].transform.position - myPos.position).magnitude < disTance)
                {
                    disTance = (GamePlayManager.Instance.players[i].transform.position - myPos.position).magnitude;
                    idx = i;
                }
            }
        }
        Camera.main.transform.SetParent(GamePlayManager.Instance.players[idx].transform);
    }

    private void SetKeysUI(int value)
    {
        try
        {
            if (value < 3)
                keys[value - 1].SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
