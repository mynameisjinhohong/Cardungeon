using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public string cardName;
    public int useMP;
    public Sprite cardType;
    public Sprite itemImage;
    public string description;
    public int cardIdx;
    public string enforceDescription;
}

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/CardData")]
public class CardObject_HJH : ScriptableObject
{
    public Card[] cards;
}
