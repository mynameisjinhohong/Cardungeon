using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Type
    {
        Move,
        Attack,
        Skill,
    }
    public string cardName;
    public int useMP;
    public Sprite cardType;
    public Sprite bigCardType;
    public Sprite itemImage;
    public Sprite enforceBigCard;
    public Sprite enforceSmallCard;
    public Type type;
    [TextArea]
    public string description;
    public int cardIdx;
    [TextArea]
    public string enforceDescription;
}

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/CardData")]
public class CardObject_HJH : ScriptableObject
{
    public Card[] cards;
}
