using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Players Hand UI Panels")]
    public List<Transform> playerHandPanels = new List<Transform>();

    [Header("Table Card Slots")]
    public List<Transform> tableCardSlots = new List<Transform>();

    [Header("Player AvatarUIs")]
    public List<PlayerAvatarUI> playerAvatars = new List<PlayerAvatarUI>();

    [Header("Team Scores")]
    public TMP_Text yourTeamScoreText;
    public TMP_Text opponentTeamScoreText;
    
    [Header("Prefabs")]
    public GameObject cardPrefab;            
    public GameObject biddingButtonPrefab;   

    [Header("UI Panels")]
    public GameObject biddingPanel;
    public Transform biddingButtonContainer;
    public GameObject gameOverPanel;
    public TMP_Text winnerText;

    void Start()
    {
        CreateBiddingButtons(); 
        gameOverPanel.SetActive(false);
    }

    #region  BiddingUI

    void CreateBiddingButtons()
    {
        for (int i = 1; i <= 13; i++)
        {
            int bidValue = i;
            GameObject bidButtonGO = Instantiate(biddingButtonPrefab, biddingButtonContainer);
            TMP_Text txt = bidButtonGO.GetComponentInChildren<TMP_Text>();
            txt.text = bidValue.ToString();
            
            Button bidButton = bidButtonGO.GetComponent<Button>(); 
            bidButton.onClick.AddListener(() => 
            {
                OnBidSelected(bidValue);
                AudioManager.Instance.PlayButtonClick();
            });
        }
    }

    public void ShowBiddingUI()
    {
        biddingPanel.SetActive(true);
        UpdateAllPlayerHandsUI();
        ClearTableSlotsUI();
    }

    public void OnBidSelected(int bidAmount)
    {
        biddingPanel.SetActive(false);
        gameManager.SubmitBid(bidAmount);
    }

    #endregion
    
    public void UpdateAllPlayerHandsUI()
    {
        for (int i = 0; i < gameManager.players.Count; i++)
        {
            Transform targetPanel = playerHandPanels[i];
            foreach (Transform child in targetPanel) Destroy(child.gameObject);
            
            Player player = gameManager.players[i];
            foreach (Card card in player.hand)
            {
                GameObject cardGO = Instantiate(cardPrefab, targetPanel);
                CardUI cardUI = cardGO.GetComponent<CardUI>();
                cardUI.Setup(card, gameManager, player.isHuman);
            }
        }
    }
    
    public void DisplayPlayedCardOnTable(int playerIndex, Card card)
    {
        Transform slot = tableCardSlots[playerIndex];

        GameObject cardGO = Instantiate(cardPrefab, slot);
        CardUI cardUI = cardGO.GetComponent<CardUI>();
        cardUI.Setup(card, gameManager, true);
        
        CardAnimator.Instance.AnimateCardToSlot(cardGO.transform, slot);
    }

    public void AnimateTrickWinnerCollection(int winnerIndex, System.Action onComplete)
    {
        CardAnimator.Instance.AnimateCollectTrickToSlot(tableCardSlots, winnerIndex, 0.45f, onComplete);
        onComplete?.Invoke();
    }
    
    public void ClearTableSlotsUI()
    {
        foreach (Transform slot in tableCardSlots)
        {
            foreach (Transform child in slot) 
                Destroy(child.gameObject);
        }
    }
    

    void Update()
    {
        for (int i = 0; i < gameManager.players.Count; i++)
        {
            Player player = gameManager.players[i];
            PlayerAvatarUI avatar = playerAvatars[i];
            
            avatar.nameText.text = player.name;
            avatar.bidText.text = (gameManager.currentPhase == GamePhase.Bidding) ? "Bid: " : $"Bid: {player.currentBid}";
            avatar.tricksWonText.text = $"Tricks: {player.tricksWon}";
        }
        
        if (gameManager.players.Count >= 4)
        {
            int yourTeamScore = gameManager.players[0].totalScore + gameManager.players[2].totalScore;
            int opponentTeamScore = gameManager.players[1].totalScore + gameManager.players[3].totalScore;

            yourTeamScoreText.text = $"Us: {yourTeamScore}";
            opponentTeamScoreText.text = $"Them: {opponentTeamScore}";
        }
        
    }
    
    public void ShowGameOverUI(string winnerMessage)
    {
        gameOverPanel.SetActive(true);
        winnerText.text = winnerMessage;
    }
}