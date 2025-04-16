using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

public class BoostManager : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField]private GameObject[] boosts;
    [SerializeField] private Image[] boostIcons;
    [SerializeField] private GameObject[] boostHighlight;
    [SerializeField] private GameObject[] boostCheckmark;
    [SerializeField] private ParticleSystem[] boostVFX;
    
    [Header("Clone for Animation")]
    [SerializeField] private GameObject boostClone;
    [SerializeField] private Image cloneIcon;
    [SerializeField] private GameObject cloneHighlight;
    [SerializeField] private GameObject cloneCheckmark;
    [SerializeField] private ParticleSystem cloneVFX;
    
    [Header("Logic")]
    [Tooltip("Order must match BoostType enum")] [SerializeField]
    private Sprite[] boostSprites;

    [SerializeField] private Button okButton;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Color disabledColor;
    [SerializeField] private CanvasGroup boostPanelGroup;
    [SerializeField] private AnimateOKButton animateOkButton;

    private BoostType[] currentBoosts = new BoostType[3];
    private BoostType? selectedBoost = null;
    private readonly BoostType[] allBoosts = System.Enum.GetValues(typeof(BoostType)).Cast<BoostType>().ToArray();

    private void Start()
    {
        okButton.gameObject.SetActive(false);
        ResetBoosts();
    }

    public void ResetBoosts()
    {
        if (okButton.gameObject.activeInHierarchy)
        {
            okButton.interactable = false;
            animateOkButton.HideOkButton();
        }
        selectedBoost = null;

        //If we need just fast and easy job I know LINQ

        /*List<BoostType> newBoosts = new List<BoostType>();

        // Guarantee at least 2 new boosts
        List<BoostType> previousBoosts = currentBoosts.ToList();
        List<BoostType> nonRepeating = allBoosts.Except(previousBoosts).OrderBy(x => Random.value).ToList();
        newBoosts.AddRange(nonRepeating.Take(2));

        // Add a 3rd boost that is not included in the first two
        List<BoostType> remaining = allBoosts.Except(newBoosts).ToList();
        newBoosts.Add(remaining[Random.Range(0, remaining.Count)]);

        newBoosts = newBoosts.OrderBy(x => Random.value).ToList();

        currentBoosts = newBoosts.ToArray();
        */

        //If we need more optimized version for shuffle I used a Fisher–Yates Shuffle method
        List<BoostType> previous = new List<BoostType>(currentBoosts);
        List<BoostType> pool = new List<BoostType>();

        for (int i = 0; i < allBoosts.Length; i++)
        {
            if (!previous.Contains(allBoosts[i]))
                pool.Add(allBoosts[i]);
        }

        Shuffle(pool);
        List<BoostType> newBoosts = new List<BoostType>
        {
            pool[0], pool[1]
        };

        List<BoostType> remaining = new List<BoostType>();
        for (int i = 0; i < allBoosts.Length; i++)
        {
            if (!newBoosts.Contains(allBoosts[i]))
                remaining.Add(allBoosts[i]);
        }

        BoostType third = remaining[Random.Range(0, remaining.Count)];
        newBoosts.Add(third);

        Shuffle(newBoosts);
        currentBoosts = newBoosts.ToArray();

        for (int i = 0; i < boostIcons.Length; i++)
        {
            boosts[i].transform.DOKill();
            boosts[i].transform.localScale = Vector3.one;
            boostIcons[i].sprite = boostSprites[(int)currentBoosts[i]];
            boostIcons[i].SetNativeSize();
            boostHighlight[i].SetActive(false);
        }
    }

    public void SelectBoost(int index)
    {
        selectedBoost = currentBoosts[index];
        if (!okButton.gameObject.activeInHierarchy)
        {
            okButton.interactable = true;
            animateOkButton.ShowOkButtonAnimated();
        }
        else
        {
            animateOkButton.AnimateOKFeedback();
        }

        for (int i = 0; i < boostIcons.Length; i++)
        {
            bool isSelected  = i == index;
            boostHighlight[i].SetActive(isSelected);
            
            Transform boostTransform = boosts[i].transform;
            boostTransform.DOKill();

            if (isSelected)
            {
                boostTransform.DOScale(1.15f, 0.25f).SetEase(Ease.OutBack);
            }
            else
            {
                boostTransform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public void ConfirmBoost()
    {
        if (selectedBoost == null)
            return;

        int selectedIndex = -1;
        for (int i = 0; i < currentBoosts.Length; i++)
        {
            if (currentBoosts[i] == selectedBoost)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex == -1)
            return;

        titleText.color = disabledColor;
        descriptionText.color = disabledColor;
        
        GameObject originBoost = boosts[selectedIndex];
        originBoost.SetActive(false);
        boostPanelGroup.interactable = false;
        
        boostPanelGroup.DOFade(0f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            boostPanelGroup.gameObject.SetActive(false);
        });
        
        boostClone.SetActive(true);
        cloneIcon.sprite = boostSprites[(int)selectedBoost];
        cloneIcon.SetNativeSize();
        cloneCheckmark.SetActive(false);
        cloneHighlight.SetActive(true);
        
        RectTransform cloneRT = boostClone.GetComponent<RectTransform>();
        RectTransform originRT = originBoost.GetComponent<RectTransform>();
        cloneRT.anchoredPosition = originRT.anchoredPosition;
        cloneRT.localScale = Vector3.one;
        
        cloneCheckmark.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(cloneRT.DOAnchorPos(Vector3.zero, 0.5f).SetEase(Ease.OutCubic));
        seq.Join(cloneRT.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack));
        seq.AppendCallback(() =>
        {
            if (cloneVFX != null) cloneVFX.Play();
        });
        seq.AppendInterval(0.4f);
        seq.Append(cloneCheckmark.transform.DOScale(0f, 0.2f));
        seq.Append(cloneRT.DOScale(0.6f, 0.5f).SetEase(Ease.InCubic));
        seq.AppendCallback(() =>
        {
            cloneHighlight.SetActive(false);
            slotManager.AssignBoostSprite(cloneRT);
            titleText.gameObject.SetActive(false);
            descriptionText.gameObject.SetActive(false);
        });
    }
}