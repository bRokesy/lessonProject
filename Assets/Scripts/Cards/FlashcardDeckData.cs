using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flashcards/Deck Data", fileName = "FlashcardDeckData")]
public class FlashcardDeckData : ScriptableObject
{
    public List<FlashcardEntry> cards = new List<FlashcardEntry>();
}

[System.Serializable]
public class FlashcardEntry
{
    public string foreignWord;
    public string translation;
    public Sprite image;
}