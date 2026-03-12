using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lesson Data/Make Sentence Data", fileName = "NewMakeSentence")]
public class MakeSentenceData : ScriptableObject
{
    [Header("Display")]
    [Tooltip("Header shown above the exercise, e.g. 'Задание 5. Он, на, идет...'")]
    public string taskTitle;

    [Tooltip("Translation or grammar hint shown to the right")]
    [TextArea] public string hint;

    [Header("Words")]
    [Tooltip("Words shown in the word bank (shuffled order)")]
    public List<string> shuffledWords;

    [Header("Answer")]
    [Tooltip("The exact correct sentence (words joined by single space)")]
    public List<string> correctSentences;
    
    public List<string> GetShuffled()
    {
        var copy = new List<string>(shuffledWords);
        for (int i = copy.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }
        return copy;
    }
}