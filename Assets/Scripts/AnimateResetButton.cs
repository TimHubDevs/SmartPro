using DG.Tweening;
using UnityEngine;

public class AnimateResetButton : MonoBehaviour
{
    [SerializeField] private Transform resetButton;

    public void AnimateButton()
    {
        resetButton.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(resetButton.DOPunchScale(Vector3.one * 0.1f, 0.25f, 6, 1));

        seq.Join(resetButton.DORotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic));

        seq.OnComplete(() => resetButton.rotation = Quaternion.identity);
    }
}
