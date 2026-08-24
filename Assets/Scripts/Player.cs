using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Player
{
    public string name;
    public bool isHuman;
    public List<Card> hand = new List<Card>();
    public int currentBid;
    public int tricksWon;
    public int totalScore;
    
    public Player(string name, bool isHuman)
    {
        this.name = name;
        this.isHuman = isHuman;
    }
}

[System.Serializable]
public class PlayerAvatarUI
{
    public TMP_Text nameText;
    public TMP_Text bidText;
    public TMP_Text tricksWonText;
}