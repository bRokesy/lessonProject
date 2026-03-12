using UnityEngine;

public class WordQuizModel : MonoBehaviour
{
    [Header("Data (ScriptableObject)")]
    [SerializeField] private WritingData writingData;
    
    [Header("Fallback (если нет WritingData)")]
    [SerializeField] private string[] correctWords;
    [SerializeField] private AudioClip[] wordClips;

    public string[] CorrectWords => writingData != null ? writingData.correctWords : correctWords;
    public AudioClip[] WordClips => writingData != null ? writingData.wordClips : wordClips;
    
    public void LoadData(WritingData data)
    {
        writingData = data;
    }
}