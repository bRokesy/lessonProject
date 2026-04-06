using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    public static string CurrentLessonTitle;

    [Header("Уроки по порядку")]
    public List<LessonData> lessons;

    [Header("Timing")]
    public float nextExerciseDelay = 1.5f;

    [Header("Navigation UI (опционально)")]
    public TextMeshProUGUI progressLabel;
    public Button nextButton;
    public Button prevButton;
    public Slider progressBar;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
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
        nextButton?.onClick.AddListener(NextExerciseNoDelay);
        prevButton?.onClick.AddListener(PrevExercise);
    }

    // ─── Navigation ───────────────────────────────────────────────────────────

    public void NextExercise()
    {
        StartCoroutine(NextExerciseDelayed(nextExerciseDelay));
    }

    public void NextExerciseNoDelay()
    {
        StartCoroutine(NextExerciseDelayed(0f));
    }

    IEnumerator NextExerciseDelayed(float delay)
    {
        if (nextButton) nextButton.interactable = false;
        yield return new WaitForSeconds(delay);

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
        {
            var entry = lessons[currentLesson].GetExercise(currentExercise);
            if (entry != null) scenePanels.ShowOnly(entry.type);
        }
    }

    // ─── Load ─────────────────────────────────────────────────────────────────

    public void LoadCurrent()
    {
        if (lessons == null || lessons.Count == 0)
        {
            Debug.LogWarning("ProgressManager: список lessons пустой!");
            return;
        }

        NotifyLeave();

        var lesson = lessons[currentLesson];
        currentExercise = Mathf.Clamp(currentExercise, 0, Mathf.Max(0, lesson.Count - 1));

        var entry = lesson.GetExercise(currentExercise);
        if (entry == null)
        {
            Debug.LogWarning($"ProgressManager: упражнение {currentExercise} не найдено в {lesson.lessonName}");
            return;
        }

        if (!entry.IsValid())
        {
            Debug.LogWarning($"ProgressManager: поле данных не заполнено для типа {entry.type} в {lesson.lessonName}[{currentExercise}]");
            return;
        }

        CurrentLessonTitle = lesson.lessonName;
        UpdateUI(lesson);
        scenePanels?.ShowOnly(entry.type);

        Debug.Log($"ProgressManager: {lesson.lessonName} [{currentExercise + 1}/{lesson.Count}] тип: {entry.type}");

        switch (entry.type)
        {
            case LessonData.ExerciseType.FillBlank:
                FindAndLoad<FillBlankManager>(m => m.LoadExercise(entry.fillBlank));
                break;
            case LessonData.ExerciseType.MakeSentence:
                FindAndLoad<MakeSentenceManager>(m => m.LoadExercise(entry.makeSentence));
                break;
            case LessonData.ExerciseType.Translate:
                FindAndLoad<TranslationQuizManager>(m => m.LoadExercise(entry.translate));
                break;
            case LessonData.ExerciseType.Writing:
                FindAndLoad<WordQuizController>(m => m.LoadExercise(entry.writing));
                break;
            case LessonData.ExerciseType.Flashcards:
                FindAndLoad<UIFlashcardSpawner>(m => m.LoadDeck(entry.flashcards));
                break;
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    void NotifyLeave()
    {
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
        if (progressBar)
        {
            progressBar.maxValue = lesson.Count;
            progressBar.value = currentExercise + 1;
        }

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scenePanels = null; // сбросить — в новой сцене будет новый RegisterPanels
        StartCoroutine(LoadAfterFrame());
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}