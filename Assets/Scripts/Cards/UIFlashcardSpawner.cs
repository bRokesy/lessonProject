using UnityEngine;

public class UIFlashcardSpawner : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private FlashcardDeckData deck;

    [Header("UI")]
    [SerializeField] private UIFlashcardFlip cardPrefab;
    [SerializeField] private Transform contentParent; // Content (GridLayoutGroup)

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (deck == null || cardPrefab == null || contentParent == null) return;

        // очистка контента
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var entry in deck.cards)
        {
            var card = Instantiate(cardPrefab, contentParent);
            card.SetData(entry.foreignWord, entry.translation, entry.image);
        }
    }
}