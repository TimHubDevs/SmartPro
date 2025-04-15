using DG.Tweening;
using UnityEngine;

public class AnimateOKButton : MonoBehaviour
{
    [SerializeField] private RectTransform okButton;
    [SerializeField] private float offscreenY = -200f;
    [SerializeField] private float visibleY = 80f;

    private void Start()
    {
        okButton.anchoredPosition = new Vector2(okButton.anchoredPosition.x, offscreenY);
    }

    public void ShowOkButtonAnimated()
    {
        okButton.DOKill();
        okButton.localScale = Vector3.one;
        
        okButton.gameObject.SetActive(true);
        okButton.DOAnchorPosY(visibleY, 0.5f).SetEase(Ease.OutBack);
    }

    public void AnimateOKFeedback()
    {
        okButton.DOKill();
        okButton.localScale = Vector3.one;
        okButton.DOPunchScale(Vector3.one * 0.03f, 0.3f, 8, 1);
    }

    public void HideOkButton()
    {
        okButton.DOKill();
        okButton.localScale = Vector3.one;
        
        okButton.DOAnchorPosY(offscreenY, 0.4f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            okButton.gameObject.SetActive(false);
        });;
    }

    public void ClickOKButton()
    {
        okButton.DOKill();
        okButton.localScale = Vector3.one;
        
        okButton.DOPunchScale(Vector3.one * 0.1f, 0.25f, 6, 1).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            HideOkButton();
        });
    }
}