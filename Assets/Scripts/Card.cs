using UnityEngine;

public enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades,
}

//Rank
public enum Rank
{
    two = 2,
    three = 3,
    four = 4,
    five = 5,
    six = 6,
    seven = 7,
    eight = 8,
    nine = 9,
    ten = 10,
    jack = 11,
    queen = 12,
    king = 13,
    Ace = 14,
}

[System.Serializable]
public class Card
{
   public Suit suit;
   public Rank rank;
   public Sprite cardImage;

   public Card(Suit s, Rank r, Sprite img = null)
   {
       suit = s;
       rank = r;
       cardImage = img;
   }
}
