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

    //카드 클릭했을 때 요소들
    public GameObject bigCard;
    public TMP_Text bigCardMp;
    public TMP_Text bigCardName;
    public TMP_Text bigCardDescribe;
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
        bigCardMp.text = CardManager.Instance.cardList.cards[cardIdx].useMP.ToString();
        bigCardName.text = CardManager.Instance.cardList.cards[cardIdx].cardName;
        bigCardDescribe.text = CardManager.Instance.cardList.cards[cardIdx].description;
    }

    public void BigCardOff()
    {
        bigCard.SetActive(false);
    }
}
