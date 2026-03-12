using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Универсальный контейнер урока.
/// Создаётся через: Right-click → Create → Lesson Data → Lesson
/// </summary>
[CreateAssetMenu(menuName = "Lesson Data/Lesson", fileName = "NewLesson")]
public class LessonData : ScriptableObject
{
    public enum ExerciseType
    {
        FillBlank,
        MakeSentence,
        Translate,
        Writing,
        Flashcards
    }

    [Header("Info")]
    public string lessonName;
    public ExerciseType exerciseType;

    [Header("Exercises — заполни только нужный список")]
    public List<FillBlankData>      fillBlankExercises;
    public List<MakeSentenceData>   makeSentenceExercises;
    public List<TranslateData>      translateExercises;
    public List<WritingData>        writingExercises;
    public List<FlashcardDeckData>  flashcardDecks;
    
    public int Count
    {
        get
        {
            switch (exerciseType)
            {
                case ExerciseType.FillBlank:    return fillBlankExercises?.Count    ?? 0;
                case ExerciseType.MakeSentence: return makeSentenceExercises?.Count ?? 0;
                case ExerciseType.Translate:    return translateExercises?.Count    ?? 0;
                case ExerciseType.Writing:      return writingExercises?.Count      ?? 0;
                case ExerciseType.Flashcards:   return flashcardDecks?.Count        ?? 0;
                default: return 0;
            }
        }
    }
}