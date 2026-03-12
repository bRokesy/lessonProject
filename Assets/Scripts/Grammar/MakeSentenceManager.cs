using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeSentenceManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform wordBank;
    public Transform answerZone;
    public TextMeshProUGUI taskLabel;
    public TextMeshProUGUI hintLabel;
    public Button checkButton;
    public Button resetButton;
    public TextMeshProUGUI feedbackText;

    [Header("Prefabs")]
    public GameObject wordChipPrefab;

    private List<DraggableWord> spawnedChips = new List<DraggableWord>();
    private MakeSentenceData currentData; 

    void Start()
    {
        checkButton.onClick.AddListener(CheckAnswer);
        resetButton.onClick.AddListener(ResetExercise);
    }

    public void LoadExercise(MakeSentenceData data)
    {
        currentData = data; // ← сохраняем

        feedbackText.text = "";
        taskLabel.text = data.taskTitle;
        hintLabel.text = data.hint;

        ClearAll();

        var shuffledData = data.GetShuffled();

        foreach (string word in shuffledData)
        {
            GameObject chip = Instantiate(wordChipPrefab, wordBank);
            DraggableWord dw = chip.GetComponent<DraggableWord>();
            dw.Init(word, wordBank, answerZone);
            spawnedChips.Add(dw);
        }
    }

    void ClearAll()
    {
        foreach (var chip in spawnedChips)
            if (chip != null) Destroy(chip.gameObject);
        spawnedChips.Clear();
    }

    public void CheckAnswer()
    {
        if (currentData == null) return;

        List<string> playerWords = new List<string>();

        foreach (Transform child in answerZone)
        {
            DraggableWord dw = child.GetComponent<DraggableWord>();
            if (dw != null)
                playerWords.Add(dw.Word);
        }

        string playerSentence = string.Join(" ", playerWords).Trim();
        bool correct = false;

        foreach (string sentence in currentData.correctSentences)
        {
            if (sentence == playerSentence)
            {
                correct = true;
                break;
            }
        }

        feedbackText.text = correct ? "Правильно!" : "Попробуйте ещё раз";
        feedbackText.color = correct ? Color.green : Color.red;

        if (correct)
        {
            ProgressManager.Instance.NextExercise();
        }
    }

    public void ResetExercise()
    {
        feedbackText.text = "";

        foreach (var chip in spawnedChips)
        {
            if (chip != null)
                chip.ReturnToBank();
        }
    }
}