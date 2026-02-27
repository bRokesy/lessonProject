using UnityEngine;

public class WordQuizModel : MonoBehaviour
{
    [Header("Correct Answers")]
    [SerializeField] private string[] correctWords;

    [Header("Audio Variants")]
    [SerializeField] private AudioClip[] wordClips;

    public string[] CorrectWords => correctWords;
    public AudioClip[] WordClips => wordClips;

    public void SetData(string[] words, AudioClip[] clips)
    {
        correctWords = words;
        wordClips = clips;
    }
}