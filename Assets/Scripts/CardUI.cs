using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public TMP_Text cardText_Rank_TL;
    public TMP_Text cardText_Rank_BR;
    public TMP_Text cardText_Suit;
    public Button button;
    public Image BackFace;
    public GameObject disabledVisualPanel;
    public CardHover hover;
    
    private Card _cardData;
    private GameManager _gameManager;
    private bool _isFaceUp;

    public void Setup(Card card, GameManager gm, bool faceUp = true)
    {
        _cardData = card;
        _gameManager = gm;
        _isFaceUp = faceUp;
        
        if (_isFaceUp)
        {
            BackFace.gameObject.SetActive(false);
            
            cardText_Rank_TL.text = GetRank(card);
            cardText_Rank_BR.text = GetRank(card);
            cardText_Suit.text = GetSuit(card);
            if (card.suit == Suit.Hearts || card.suit == Suit.Diamonds)
            {
                cardText_Rank_TL.color = Color.red;
                cardText_Rank_BR.color = Color.red;
                cardText_Suit.color = Color.red;
            }
            else
            {
                cardText_Rank_TL.color = Color.black;
                cardText_Rank_BR.color = Color.black;
                cardText_Suit.color = Color.black;
            }
        }
        else
        {
            BackFace.gameObject.SetActive(true);
        }
        
        bool isInHand = transform.parent.name.Contains("Hand Panel");
        hover.enabled = isInHand;
        
        //Valid playable card check
        bool isHumanTurn = (gm.currentPhase == GamePhase.PlayingTricks && gm.activePlayerIndex == 0);
        bool isValid = (gm.IsLegalMove(gm.players[0], card));
        if (faceUp && isHumanTurn)
        {
            button.interactable = isValid;
            disabledVisualPanel.SetActive(!isValid);
        }

        
        button.onClick.RemoveAllListeners();
        if (_isFaceUp)
        {
            button.onClick.AddListener(OnCardClicked);
        }
    }

    private void OnCardClicked()
    {
        Player human = _gameManager.players[0];
        if (_gameManager.currentPhase == GamePhase.PlayingTricks && _gameManager.activePlayerIndex == 0)
        {
            bool success = _gameManager.PlayCard(human, _cardData);
            if (success)
            {
                Destroy(gameObject);
            }
        }
    }

    private string GetRank(Card c)
    {
        string rankStr = "";

        if (c.rank == Rank.Ace)
        {
            rankStr = "A";
        }
        else if (c.rank == Rank.king)
        {
            rankStr = "K";
        }
        else if (c.rank == Rank.queen)
        {
            rankStr = "Q";
        }
        else if (c.rank == Rank.jack)
        {
            rankStr = "J";
        }
        else
        {
            rankStr = ((int)c.rank).ToString(); 
        }

        return rankStr;
    }

    private string GetSuit(Card c)
    {
        string suitStr = "";

        if (c.suit == Suit.Spades)
        {
            suitStr = "♠";
        }
        else if (c.suit == Suit.Hearts)
        {
            suitStr = "♥";
        }
        else if (c.suit == Suit.Diamonds)
        {
            suitStr = "♦";
        }
        else if (c.suit == Suit.Clubs)
        {
            suitStr = "♣";
        }
        
        return suitStr;
    }
}