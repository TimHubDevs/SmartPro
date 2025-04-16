using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private RectTransform slot;
    [SerializeField] private Image slotIcon;
    [SerializeField] private float flyDuration = 0.5f;
    [SerializeField] private RectTransform boostClone;
    [Header("Side Panel")]
    [SerializeField] private RectTransform sidePanel;
    [SerializeField] private RectTransform arrowButton;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float hiddenOffsetX = 210f;
    [SerializeField] private CanvasGroup boostPanelGroup;
    
    private bool isPanelVisible = true;
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private Vector2 slotLocalPosition;
    
    private void Start()
    {
        shownPosition = sidePanel.anchoredPosition;
        hiddenPosition = shownPosition + new Vector2(hiddenOffsetX, 0);

        slotLocalPosition = GetLocalPositionIn(boostClone.parent as RectTransform, slot);;
    }
    
    public void AssignBoostSprite(RectTransform boostClone)
    {
        if (!isPanelVisible)
            TogglePanel();

        boostClone.DOAnchorPos(slotLocalPosition, flyDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            boostClone.gameObject.SetActive(false);

            slotIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 1);

            slotIcon.sprite = boostClone.gameObject.GetComponentInChildren<Image>().sprite;
            
            TogglePanel();
            
            DOVirtual.DelayedCall(slideDuration, () =>
            {
                boostPanelGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    boostPanelGroup.gameObject.SetActive(false);
                });
            });
        });
    }
    
    private Vector2 GetLocalPositionIn(RectTransform targetSpace, RectTransform worldSpaceRect)
    {
        Vector3 worldPos = worldSpaceRect.TransformPoint(worldSpaceRect.rect.center);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetSpace, worldPos, null, out Vector2 localPoint);
        return localPoint;
    }
    
    public void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;

        Vector2 targetPos = isPanelVisible ? shownPosition : hiddenPosition;
        float targetRotation = isPanelVisible ? 180f : 0f;

        sidePanel.DOAnchorPos(targetPos, slideDuration).SetEase(Ease.InOutCubic);
        Sequence arrowSeq = DOTween.Sequence();

        arrowSeq.Append(arrowButton.DOPunchScale(Vector3.one * 0.1f, 0.2f, 6, 1));
        arrowSeq.Join(arrowButton.DORotate(new Vector3(0, 0, targetRotation), slideDuration).SetEase(Ease.InOutCubic));
    }
}
