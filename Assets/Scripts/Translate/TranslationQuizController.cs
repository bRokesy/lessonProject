using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranslationQuizController : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string foreignWord;
        public string correctTranslation;
        public string[] options;
    }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI wordText;
    [SerializeField] private Transform optionsParent;
    [SerializeField] private OptionButtonUI optionButtonPrefab;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Behavior")]
    [SerializeField] private float delayBeforeNext = 1.2f;

    private List<OptionButtonUI> spawned = new();
    private int index = 0;
    private bool answered = false;

    // ✅ МАССИВ ЗАПОЛНЕН ЗДЕСЬ
    private Question[] questions =
    {
        new Question
        {
            foreignWord = "apple",
            correctTranslation = "яблоко",
            options = new [] { "яблоко", "груша", "банан", "апельсин" }
        },
        new Question
        {
            foreignWord = "dog",
            correctTranslation = "собака",
            options = new [] { "кошка", "собака", "птица", "рыба" }
        },
        new Question
        {
            foreignWord = "house",
            correctTranslation = "дом",
            options = new [] { "школа", "квартира", "дом", "магазин" }
        },
        new Question
        {
            foreignWord = "car",
            correctTranslation = "машина",
            options = new [] { "велосипед", "самолёт", "машина", "поезд" }
        },
        new Question
        {
            foreignWord = "water",
            correctTranslation = "вода",
            options = new [] { "молоко", "сок", "вода", "чай" }
        }
    };

    private void Start()
    {
        ShowQuestion();
    }

    private void ShowQuestion()
    {
		if (optionButtonPrefab == null)
		{
    	Debug.LogError("optionButtonPrefab НЕ назначен в инспекторе!");
   		return;
		}
		if (optionsParent == null)
		{
    	Debug.LogError("optionsParent НЕ назначен в инспекторе!");
   		return;
		}
        ClearOptions();
        answered = false;

        if (index >= questions.Length)
        {
            Finish();
            return;
        }

        var q = questions[index];

        wordText.text = q.foreignWord;

        if (progressText)
            progressText.text = $"{index + 1}/{questions.Length}";

        foreach (var opt in q.options)
        {
            var btn = Instantiate(optionButtonPrefab);
			btn.transform.SetParent(optionsParent, false);
			btn.Setup(opt, OnOptionClicked);
			spawned.Add(btn);
        }
    }

    private void OnOptionClicked(OptionButtonUI clicked)
    {
        if (answered) return;
        answered = true;

        var q = questions[index];

        bool isCorrect = clicked.Value.Trim().ToLower() ==
                         q.correctTranslation.Trim().ToLower();

        if (isCorrect) clicked.SetCorrect();
        else clicked.SetWrong();

        foreach (var btn in spawned)
        {
            btn.SetInteractable(false);

            if (btn.Value.Trim().ToLower() ==
                q.correctTranslation.Trim().ToLower())
                btn.SetCorrect();
        }

        StartCoroutine(NextAfterDelay());
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNext);
        index++;
        ShowQuestion();
    }

    private void Finish()
    {
        ClearOptions();
        wordText.text = "Готово! ✅";

        if (progressText)
            progressText.text = $"{questions.Length}/{questions.Length}";
    }

    private void ClearOptions()
    {
        foreach (var btn in spawned)
            if (btn != null)
                Destroy(btn.gameObject);

        spawned.Clear();
    }
}