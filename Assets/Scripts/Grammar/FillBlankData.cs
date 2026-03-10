using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fill Blank Data", fileName = "NewFillBlank")]
public class FillBlankData : ScriptableObject
{
    [Header("Display")]
    public string taskTitle;
    [TextArea] public string hint;

    [Header("Sentence")]
    [Tooltip("Используй ___ для каждого пропуска. Пример: 'Он ___ на поезде ___ вчера'")]
    [TextArea] public string sentenceWithBlanks;

    [Header("Answers")]
    [Tooltip("Правильные слова для пропусков — по порядку")]
    public List<string> correctAnswers;

    [Header("Word Bank")]
    [Tooltip("Все слова в банке (включая лишние/дистракторы)")]
    public List<string> wordBankWords;
}