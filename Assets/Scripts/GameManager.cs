using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase { Dealing, Bidding, PlayingTricks, Scoring, GameOver }

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public UIManager uiManager;
    
    [Header("Game State")]
    public GamePhase currentPhase;

    [SerializeField] private int totalTargetScore = 500;
    
    public List<Player> players = new List<Player>();
    public int activePlayerIndex = 0;
    public int leadPlayerIndex = 0;
    
    
    [Header("Current Trick")]
    public List<Card> cardsOnTable = new List<Card>();
    public List<int> playerIndicesOnTable = new List<int>();
    public Suit leadSuit;
    public bool spadesBroken = false;
    public int trickCount = 0;
    
    
    void Start()
    {
        players.Clear(); 
        
        players.Add(new Player("You", true));
        players.Add(new Player("Player1", false));
        players.Add(new Player("Player2", false));
        players.Add(new Player("Player3", false));
        SetupNewRound();
    }

    #region Dealing Phase

    public void SetupNewRound()
    {
        //Initiation
        currentPhase = GamePhase.Dealing;
        spadesBroken = false;
        trickCount = 0;
        cardsOnTable.Clear();
        playerIndicesOnTable.Clear();
        
        //Deck Setup
        deckManager.BuildDeck();
        deckManager.ShuffleDeck();
        
        //Deal cards 
        int cardIndex = 0;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].hand.Clear();
            players[i].tricksWon = 0;
            players[i].currentBid = 0;
            for (int c = 0; c < 13; c++)
            {
                if (cardIndex < deckManager.fullDeck.Count)
                {
                    players[i].hand.Add(deckManager.fullDeck[cardIndex]);
                    cardIndex++;
                }
            }
        }
        
        StartBiddingPhase();
    }

    #endregion

    #region Bidding Phase

    void StartBiddingPhase()
    {
        currentPhase = GamePhase.Bidding;
        uiManager.ShowBiddingUI();
        
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isHuman)
            {
                players[i].currentBid = CalculateAIBid(players[i]);
            }
        }
        uiManager.ShowBiddingUI();
    }
    
    // AI CardPlay
    private Card SelectAICard(Player aiPlayer)
    {
        List<Card> legalCards = aiPlayer.hand.FindAll(c => IsLegalMove(aiPlayer, c));
        return legalCards[Random.Range(0, legalCards.Count)];
    }
    
    private int CalculateAIBid(Player player)
    {
        int estimatedTricks = 0;
        foreach (Card c in player.hand)
        {
            if (c.rank == Rank.Ace || c.rank == Rank.king) estimatedTricks++;
            else if (c.suit == Suit.Spades && c.rank >= Rank.ten) estimatedTricks++;
        }
        return Mathf.Max(1, estimatedTricks);
    }
    public void SubmitBid(int bidAmount)
    {
        players[0].currentBid = bidAmount;
        currentPhase = GamePhase.PlayingTricks;
        
        activePlayerIndex = leadPlayerIndex;

        ProcessTurn();
    }

    #endregion

    #region PlayingTricks Phase

     void ProcessTurn()
    {
        Player current = players[activePlayerIndex];
        uiManager.UpdateAllPlayerHandsUI();
        
        if (!current.isHuman)
        {
            StartCoroutine(AITurnRoutine(current));
        }
    }
    
    private IEnumerator AITurnRoutine(Player aiPlayer)
    {
        yield return new WaitForSeconds(1f);
        Card chosenCard = SelectAICard(aiPlayer);
        PlayCard(aiPlayer, chosenCard);
    }
   
    public bool PlayCard(Player player, Card card)
    {
        if (!IsLegalMove(player, card))
        {
            return false;
        }
        
        //Set leading suitt
        if (cardsOnTable.Count == 0)
        {
            leadSuit = card.suit;
        }
        if (card.suit == Suit.Spades)
        {
            spadesBroken = true;
        }
        
        // 1. Remove from hand
        player.hand.Remove(card);
        cardsOnTable.Add(card);
        playerIndicesOnTable.Add(activePlayerIndex);

        //Place on Board
        uiManager.UpdateAllPlayerHandsUI();
        uiManager.DisplayPlayedCardOnTable(activePlayerIndex, card);
        
        
        if (cardsOnTable.Count < 4)
        {
            activePlayerIndex = (activePlayerIndex + 1) % 4;
            ProcessTurn();
        }
        else
        {
            StartCoroutine(EvaluateTrickWinnerRoutine());
        }
        return true;
    }
   
    //Rule check
    public bool IsLegalMove(Player player, Card card)
    {
        // If lead player, cannot lead Spades until Spades are broken (unless player ONLY has Spades)
        if (cardsOnTable.Count == 0)
        {
            if (card.suit == Suit.Spades && !spadesBroken)
            {
                bool hasOnlySpades = player.hand.TrueForAll(c => c.suit == Suit.Spades);
                return hasOnlySpades;
            }
            return true;
        }
        // Must follow lead suit if player has it
        bool hasLeadSuit = player.hand.Exists(c => c.suit == leadSuit);
        if (hasLeadSuit && card.suit != leadSuit)
        {
            return false;
        }
        return true;
    }
    

    #endregion

    #region Scoring
     private IEnumerator EvaluateTrickWinnerRoutine()
    {
        
        yield return new WaitForSeconds(1.0f);
        
        int winningTableIndex = 0;
        Card winningCard = cardsOnTable[0];

        for (int i = 1; i < cardsOnTable.Count; i++)
        {
            Card current = cardsOnTable[i];
            if (current.suit == Suit.Spades && winningCard.suit != Suit.Spades)
            {
                winningCard = current;
                winningTableIndex = i;
            }
            else if (current.suit == Suit.Spades && winningCard.suit == Suit.Spades)
            {
                if (current.rank > winningCard.rank)
                {
                    winningCard = current;
                    winningTableIndex = i;
                }
            }
            else if (current.suit == leadSuit && winningCard.suit == leadSuit)
            {
                if (current.rank > winningCard.rank)
                {
                    winningCard = current;
                    winningTableIndex = i;
                }
            }
        }

        int winnerPlayerIndex = playerIndicesOnTable[winningTableIndex];
        players[winnerPlayerIndex].tricksWon++;
        
        AudioManager.Instance.PlayTrickWin();

        bool animationFinished = false;

        uiManager.AnimateTrickWinnerCollection(winnerPlayerIndex, () =>
        {
            animationFinished = true; 
        });
        
        yield return new WaitUntil(() => animationFinished);

        // reset board
        cardsOnTable.Clear();
        playerIndicesOnTable.Clear();
        trickCount++;

        // next round leader
        leadPlayerIndex = winnerPlayerIndex;
        activePlayerIndex = leadPlayerIndex;

        if (trickCount < 13)
        {
            ProcessTurn();
        }
        else
        {
            ScoreRound();
        }
    }

    void ScoreRound()
    {
        currentPhase = GamePhase.Scoring;
        
        foreach (Player player in players)
        {
            if (player.tricksWon >= player.currentBid)
            {
                int basePoints = player.currentBid * 10;
                int bags = player.tricksWon - player.currentBid; 
                player.totalScore += basePoints + bags;
            }
            else
            {
                player.totalScore -= player.currentBid * 10;
            }
        }
        
        int myTeamScore = players[0].totalScore + players[2].totalScore;
        int opponentTeamScore = players[1].totalScore + players[3].totalScore;

        int targetScore = totalTargetScore; 
        
        
        if (myTeamScore >= targetScore || opponentTeamScore >= targetScore)
        {
            currentPhase = GamePhase.GameOver;

            string winnerMsg = "";
            if (myTeamScore > opponentTeamScore)
            {
                winnerMsg = "Match Won";
            }
            else
            {
                winnerMsg = "Match Lost!";
            }
            

            uiManager.ShowGameOverUI(winnerMsg);
        }
        else
        {
            SetupNewRound();
        }
    }
    #endregion
}
