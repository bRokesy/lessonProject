using System.Collections;
using UnityEngine;

public class UIFlashcardSpawner : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private FlashcardDeckData deck;

    [Header("UI")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform contentParent;

    public Transform ContentParent => contentParent;

    void Start()
    {
        if (deck != null) SpawnAll();
    }

    public void LoadDeck(FlashcardDeckData newDeck)
    {
        deck = newDeck;
        // Используем корутину чтобы Destroy старых карточек успел выполниться
        StartCoroutine(SpawnNextFrame());
    }

    IEnumerator SpawnNextFrame()
    {
        // Уничтожить старые
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // Ждём кадр — Destroy выполняется в конце кадра
        yield return null;

        SpawnAll();
    }

    public void SpawnAll()
    {
        if (deck == null || cardPrefab == null || contentParent == null) return;

        // Очистить синхронно (для первого вызова из Start)
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        foreach (var entry in deck.cards)
        {
            var card = Instantiate(cardPrefab, contentParent);
            card.SetActive(false);
            card.GetComponent<UIFlashcardFlip>()?.SetData(entry.foreignWord, entry.translation, entry.image);
        }

        // Уведомить менеджер после спавна
        GetComponent<FlashcardDeckManager>()?.OnDeckLoaded();
    }
}