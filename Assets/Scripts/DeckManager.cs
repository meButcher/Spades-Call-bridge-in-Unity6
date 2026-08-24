using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> fullDeck = new List<Card>();
    
    public void BuildDeck()
    {
        fullDeck.Clear();
        for (int s = 0; s < 4; s++)
        {
            for (int r = 2; r <= 14; r++)
            {
                fullDeck.Add(new Card((Suit)s, (Rank)r));
            }
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < fullDeck.Count; i++)
        {
            int randomIndex = Random.Range(i, fullDeck.Count);
            (fullDeck[i], fullDeck[randomIndex]) = (fullDeck[randomIndex], fullDeck[i]);
        }
    }
}
