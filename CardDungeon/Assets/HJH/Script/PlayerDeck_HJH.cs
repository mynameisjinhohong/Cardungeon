using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck_HJH : MonoBehaviour
{
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

    }

    public void Reroll()
    {
        for(int i =0; i< hand.Count; i++)
        {
            int a = hand[0];
            hand.RemoveAt(0);
            trash.Add(a);
        }
        DrawFirst();
    }

    public void TrashToDeck()
    {
        for(int i =0; i<trash.Count; i++)
        {
            int a = trash[0];
            trash.RemoveAt(0);
            deck.Add(a);
        }
    }
}
