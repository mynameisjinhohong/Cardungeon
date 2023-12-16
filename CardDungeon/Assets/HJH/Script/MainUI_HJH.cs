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
    // Start is called before the first frame update
    void Start()
    {
        //������ ����ؼ� �г����̶� ������ �ٲٴ°� �ؾߵ�
        
    }

    // Update is called once per frame
    void Update()
    {
        int deckAmount = 0;
        deckAmount += playerDeck.deck.Count;
        deckAmount += playerDeck.hand.Count;
        deckAmount += playerDeck.trash.Count;
        deckCardAmount.text = "����� ī�� " + playerDeck.trash.Count + "/" + deckAmount;
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

    public void Reroll()
    {
        reRollButton.raycastTarget = false;
        StartCoroutine(RerollButton());
    }
    IEnumerator RerollButton()
    {
        float currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
        }
    }
}
