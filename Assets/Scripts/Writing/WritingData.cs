using UnityEngine;

[CreateAssetMenu(menuName = "Lesson Data/Writing Data", fileName = "NewWritingData")]
public class WritingData : ScriptableObject
{
    [Header("Display")]
    public string lessonTitle;

    [Header("Correct Answers")]
    [Tooltip("Все допустимые варианты правильного ответа")]
    public string[] correctWords;

    [Header("Audio")]
    [Tooltip("Аудиоклипы слова (будет выбран случайный или последовательный)")]
    public AudioClip[] wordClips;
}