using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BoostManager : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private Image[] boostIcons;

    [SerializeField] private GameObject[] boostHighlight;

    [Tooltip("Order must match BoostType enum")] [SerializeField]
    private Sprite[] boostSprites;

    [SerializeField] private Button resetButton;
    [SerializeField] private Button okButton;

    private BoostType[] currentBoosts = new BoostType[3];
    private BoostType? selectedBoost = null;
    private readonly BoostType[] allBoosts = System.Enum.GetValues(typeof(BoostType)).Cast<BoostType>().ToArray();

    private void Start()
    {
        resetButton.onClick.AddListener(ResetBoosts);
        okButton.gameObject.SetActive(false);
        ResetBoosts();
    }

    private void ResetBoosts()
    {
        okButton.gameObject.SetActive(false);
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

        // Fill the pool with new boosts only
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

        // Add a 3rd boost that is not included in the first two
        List<BoostType> remaining = new List<BoostType>();
        for (int i = 0; i < allBoosts.Length; i++)
        {
            if (!newBoosts.Contains(allBoosts[i]))
                remaining.Add(allBoosts[i]);
        }

        BoostType third = remaining[Random.Range(0, remaining.Count)];
        newBoosts.Add(third);

        // Last Shuffle
        Shuffle(newBoosts);
        currentBoosts = newBoosts.ToArray();

        for (int i = 0; i < boostIcons.Length; i++)
        {
            boostIcons[i].sprite = boostSprites[(int)currentBoosts[i]];
            boostIcons[i].SetNativeSize();
            boostHighlight[i].SetActive(false);
        }
    }

    public void SelectBoost(int index)
    {
        selectedBoost = currentBoosts[index];
        okButton.gameObject.SetActive(true);

        for (int i = 0; i < boostIcons.Length; i++)
        {
            bool active = i == index;
            boostHighlight[i].SetActive(active);
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
}