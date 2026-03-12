using UnityEngine;

[RequireComponent(typeof(WordQuizModel))]
[RequireComponent(typeof(WordQuizView))]
[RequireComponent(typeof(AudioSource))]
public class WordQuizController : MonoBehaviour
{
    public enum PlayMode { Random, Sequential }

    [SerializeField] private PlayMode playMode = PlayMode.Random;

    private WordQuizModel model;
    private WordQuizView view;
    private AudioSource audioSource;
    private int currentClipIndex = 0;

    private void Awake()
    {
        model = GetComponent<WordQuizModel>();
        view  = GetComponent<WordQuizView>();
        audioSource = GetComponent<AudioSource>();

        view.InputField.onEndEdit.AddListener(CheckAnswer);
    }

    private void OnDestroy()
    {
        view.InputField.onEndEdit.RemoveListener(CheckAnswer);
    }
    
    public void LoadExercise(WritingData data)
    {
        model.LoadData(data);
        currentClipIndex = 0;
        view.ResetView();
    }

    public void PlayWord()
    {
        var clips = model.WordClips;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = playMode == PlayMode.Random
            ? clips[Random.Range(0, clips.Length)]
            : clips[currentClipIndex++ % clips.Length];

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void CheckAnswer(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            view.ResetView();
            return;
        }

        string user = Normalize(userInput);
        bool isCorrect = false;

        foreach (var word in model.CorrectWords)
        {
            if (Normalize(word) == user)
            {
                isCorrect = true;
                break;
            }
        }

        if (isCorrect)
        {
            view.SetCorrect();
            
            ProgressManager.Instance.NextExercise();
        }
        else
        {
            view.SetWrong();
        }
    }

    private string Normalize(string input) =>
        System.Text.RegularExpressions.Regex
            .Replace(input.ToLower().Trim(), @"\s+", " ");
}