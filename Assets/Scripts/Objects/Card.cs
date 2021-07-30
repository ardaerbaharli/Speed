using System;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool IsSliding { get; set; }
    public bool IsDummy { get; set; }
    public string CardName { get; set; }
    public int ID { get; set; }
    public int Value;
    public string Suit;

    public static bool IsEqual(Card a, Card b)
    {
        if (a.CardName == b.CardName && 
            a.Value == b.Value &&
            a.Suit == b.Suit &&
            a.ID == b.ID)
            return true;
        else return false;
    }
}
