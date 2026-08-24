using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float hoverOffsetY = 25f;
    public float duration = 0.15f;
    
    [SerializeField] private Vector3 startLocalPos;
    private bool isLocalPosCaptured = false;
    private bool isHovered = false;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLocalPosCaptured)
        {
            startLocalPos = transform.localPosition;
            isLocalPosCaptured = true;
        }
        isHovered = true;
        transform.DOLocalMoveY(startLocalPos.y + hoverOffsetY, duration).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlayCardHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.DOLocalMoveY(startLocalPos.y, duration).SetEase(Ease.OutQuad);
        AudioManager.Instance.StopSfx();
    }
}