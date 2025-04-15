using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private RectTransform slot;
    [SerializeField] private Image slotIcon;
    [SerializeField] private GameObject sidePanel;
    [SerializeField] private float flyDuration = 0.5f;

    public void AssignBoostSprite(RectTransform boostClone)
    {
        if (!sidePanel.activeInHierarchy)
            sidePanel.SetActive(true);

        Vector2 localPos = GetLocalPositionIn(boostClone.parent as RectTransform, slot);
        
        boostClone.DOAnchorPos(localPos, flyDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            boostClone.gameObject.SetActive(false);

            // Бамп-ефект на слоті
            slotIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 1);

            // Міняємо спрайт слота
            slotIcon.sprite = boostClone.gameObject.GetComponentInChildren<Image>().sprite;
        });
    }
    
    private Vector2 GetLocalPositionIn(RectTransform targetSpace, RectTransform worldSpaceRect)
    {
        Vector3 worldPos = worldSpaceRect.position;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetSpace, worldPos, null, out localPoint);
        return localPoint;
    }
}
