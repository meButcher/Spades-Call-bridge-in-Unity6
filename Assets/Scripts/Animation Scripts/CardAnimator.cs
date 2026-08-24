using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CardAnimator : MonoBehaviour
{
    public static CardAnimator Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AnimateCardToSlot(Transform cardTransform, Transform targetSlot)
    {
            cardTransform.SetParent(targetSlot);
            cardTransform.localPosition = Vector3.zero;
            AudioManager.Instance.PlayCardPlace();
    }
    
    public void AnimateCollectTrickToSlot(List<Transform> allTableSlots, int winnerIndex, float duration = 0.45f, System.Action onComplete = null)
    {
        Transform winningSlot = allTableSlots[winnerIndex];
        List<Transform> cardsToMove = new List<Transform>();
        
        Transform winningCard = winningSlot.GetChild(0);

        for (int i = 0; i < allTableSlots.Count; i++)
        {
            if (i != winnerIndex)
            {
                cardsToMove.Add(allTableSlots[i].GetChild(0));
            }
        }

        Sequence slideSequence = DOTween.Sequence();
        
        foreach (Transform card in cardsToMove)
        {
            slideSequence.Join(card.DOMove(winningSlot.position, duration).SetEase(Ease.OutCubic));
            card.SetParent(winningSlot);
        }
        winningCard.SetAsLastSibling();
        
        slideSequence.AppendInterval(0.3f);
        
        slideSequence.OnComplete(() =>
        {
            foreach (Transform slot in allTableSlots)
            {
                foreach (Transform child in slot) 
                    Destroy(child.gameObject);
            }
            onComplete?.Invoke();
        });
    }
}