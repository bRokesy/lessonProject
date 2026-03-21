using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lesson Data/Flashcard Deck Data", fileName = "FlashcardDeckData")]
public class FlashcardDeckData : ScriptableObject
{
    public List<FlashcardEntry> cards = new List<FlashcardEntry>();
}

[System.Serializable]
public class FlashcardEntry
{
    [TextArea]
    public string foreignWord;
    [TextArea]
    public string translation;
    public Sprite image;

    [Tooltip("Опционально — пример использования слова")]
    public string example;
}