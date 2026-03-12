using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("Уроки по порядку")]
    public List<LessonData> lessons;

    [Header("Timing")]
    public float nextExerciseDelay = 1.5f;

    [Header("Navigation UI (опционально)")]
    public TextMeshProUGUI progressLabel;
    public Button nextButton;
    public Button prevButton;

    private int currentLesson   = 0;
    private int currentExercise = 0;
    private SceneUIPanels scenePanels;

    const string PREF_LESSON   = "progress_lesson";
    const string PREF_EXERCISE = "progress_exercise";

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLesson   = Mathf.Clamp(PlayerPrefs.GetInt(PREF_LESSON, 0), 0, Mathf.Max(0, lessons.Count - 1));
        currentExercise = PlayerPrefs.GetInt(PREF_EXERCISE, 0);
    }

    void Start()
    {
        BindButtons();
        StartCoroutine(LoadAfterFrame());
    }

    IEnumerator LoadAfterFrame()
    {
        yield return null;
        LoadCurrent();
    }

    void BindButtons()
    {
        nextButton?.onClick.RemoveAllListeners();
        prevButton?.onClick.RemoveAllListeners();
        nextButton?.onClick.AddListener(NextExercise);
        prevButton?.onClick.AddListener(PrevExercise);
    }

    // ─── Navigation ───────────────────────────────────────────────────────────

    public void NextExercise()
    {
        StartCoroutine(NextExerciseDelayed());
    }

    IEnumerator NextExerciseDelayed()
    {
        // Заблокировать кнопку на время задержки
        if (nextButton) nextButton.interactable = false;

        yield return new WaitForSeconds(nextExerciseDelay);

        var lesson = lessons[currentLesson];
        if (currentExercise < lesson.Count - 1)
            currentExercise++;
        else if (currentLesson < lessons.Count - 1)
        {
            currentLesson++;
            currentExercise = 0;
        }
        else { OnAllComplete(); yield break; }

        SaveProgress();
        LoadCurrent();
    }

    public void PrevExercise()
    {
        if (currentExercise > 0)
            currentExercise--;
        else if (currentLesson > 0)
        {
            currentLesson--;
            currentExercise = Mathf.Max(0, lessons[currentLesson].Count - 1);
        }

        SaveProgress();
        LoadCurrent();
    }

    public void ResetProgress()
    {
        currentLesson   = 0;
        currentExercise = 0;
        SaveProgress();
        LoadCurrent();
    }

    // ─── Panels ───────────────────────────────────────────────────────────────

    public void RegisterPanels(SceneUIPanels panels)
    {
        scenePanels = panels;
        if (lessons != null && lessons.Count > 0)
            scenePanels.ShowOnly(lessons[currentLesson].exerciseType);
    }

    // ─── Load ─────────────────────────────────────────────────────────────────

    public void LoadCurrent()
    {
        if (lessons == null || lessons.Count == 0)
        {
            Debug.LogWarning("ProgressManager: список lessons пустой!");
            return;
        }

        // Дать текущему контроллеру очистить UI перед уходом
        NotifyLeave();

        var lesson = lessons[currentLesson];
        currentExercise = Mathf.Clamp(currentExercise, 0, Mathf.Max(0, lesson.Count - 1));

        UpdateUI(lesson);
        scenePanels?.ShowOnly(lesson.exerciseType);

        Debug.Log($"ProgressManager: {lesson.lessonName}, упражнение {currentExercise + 1}/{lesson.Count}, тип: {lesson.exerciseType}");

        switch (lesson.exerciseType)
        {
            case LessonData.ExerciseType.FillBlank:
                FindAndLoad<FillBlankManager>(m => m.LoadExercise(lesson.fillBlankExercises[currentExercise]));
                break;
            case LessonData.ExerciseType.MakeSentence:
                FindAndLoad<MakeSentenceManager>(m => m.LoadExercise(lesson.makeSentenceExercises[currentExercise]));
                break;
            case LessonData.ExerciseType.Translate:
                FindAndLoad<TranslationQuizController>(m => m.LoadExercise(lesson.translateExercises[currentExercise]));
                break;
            case LessonData.ExerciseType.Writing:
                FindAndLoad<WordQuizController>(m => m.LoadExercise(lesson.writingExercises[currentExercise]));
                break;
            case LessonData.ExerciseType.Flashcards:
                FindAndLoad<UIFlashcardSpawner>(m => m.LoadDeck(lesson.flashcardDecks[currentExercise]));
                break;
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Уведомить текущий контроллер об уходе с упражнения.</summary>
    void NotifyLeave()
    {
        var controller = FindFirstObjectByType<MonoBehaviour>();
        // Ищем все IExerciseController в сцене и вызываем OnExerciseLeave
        foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mono is IExerciseController ec)
                ec.OnExerciseLeave();
        }
    }

    void FindAndLoad<T>(System.Action<T> action) where T : Object
    {
        var manager = FindFirstObjectByType<T>();
        if (manager != null)
            action(manager);
        else
            Debug.LogWarning($"ProgressManager: {typeof(T).Name} не найден в сцене.");
    }

    void UpdateUI(LessonData lesson)
    {
        if (progressLabel)
            progressLabel.text = $"{lesson.lessonName}  •  {currentExercise + 1} / {lesson.Count}";

        if (prevButton) prevButton.interactable = !(currentLesson == 0 && currentExercise == 0);
        if (nextButton) nextButton.interactable = !(currentLesson == lessons.Count - 1 && currentExercise == lesson.Count - 1);
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt(PREF_LESSON,   currentLesson);
        PlayerPrefs.SetInt(PREF_EXERCISE, currentExercise);
        PlayerPrefs.Save();
    }

    void OnAllComplete()
    {
        if (progressLabel) progressLabel.text = "Все уроки пройдены!";
        if (nextButton)    nextButton.interactable = false;
    }
}