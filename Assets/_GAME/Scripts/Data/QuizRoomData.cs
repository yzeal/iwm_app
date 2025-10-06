using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Room", menuName = "Quiz/Room Data")]
public class QuizRoomData : ScriptableObject
{
    [Header("Raum Information")]
    public string roomName;
    public int roomNumber;
    
    [Header("Quiz Fragen nach Schwierigkeitsgrad")]
    [SerializeField] private QuizQuestion[] kidsQuestions;
    [SerializeField] private QuizQuestion[] bigKidsQuestions;
    [SerializeField] private QuizQuestion[] adultsQuestions;
    
    [Header("UI Einstellungen")]
    public Color roomColor = Color.blue;
    public Sprite roomIcon;
    
    [Header("Zeit-Einstellungen (optional)")]
    public DifficultyTimeSettings timeSettings = new DifficultyTimeSettings();
    
    /// <summary>
    /// Gibt die Fragen für den angegebenen Schwierigkeitsgrad zurück
    /// </summary>
    public QuizQuestion[] GetQuestionsForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsQuestions ?? new QuizQuestion[0],
            DifficultyLevel.BigKids => bigKidsQuestions ?? new QuizQuestion[0],
            DifficultyLevel.Adults => adultsQuestions ?? new QuizQuestion[0],
            _ => adultsQuestions ?? new QuizQuestion[0]
        };
    }
    
    /// <summary>
    /// Gibt eine zufällige Frage für den angegebenen Schwierigkeitsgrad zurück
    /// </summary>
    public QuizQuestion GetRandomQuestionForDifficulty(DifficultyLevel difficulty)
    {
        var questions = GetQuestionsForDifficulty(difficulty);
        if (questions.Length == 0) return null;
        
        int randomIndex = Random.Range(0, questions.Length);
        return questions[randomIndex];
    }
    
    /// <summary>
    /// Gibt die Anzahl verfügbarer Fragen für einen Schwierigkeitsgrad zurück
    /// </summary>
    public int GetQuestionCountForDifficulty(DifficultyLevel difficulty)
    {
        return GetQuestionsForDifficulty(difficulty).Length;
    }
    
    /// <summary>
    /// Validierung: Prüft ob für alle Schwierigkeitsgrade Fragen vorhanden sind
    /// </summary>
    public bool HasQuestionsForAllDifficulties()
    {
        return kidsQuestions != null && kidsQuestions.Length > 0 &&
               bigKidsQuestions != null && bigKidsQuestions.Length > 0 &&
               adultsQuestions != null && adultsQuestions.Length > 0;
    }
    
    #region Editor Helper Methods
    
    [ContextMenu("Validate All Difficulties")]
    private void ValidateQuestions()
    {
        Debug.Log($"Room {roomName} - Kids: {GetQuestionCountForDifficulty(DifficultyLevel.Kids)}, " +
                 $"BigKids: {GetQuestionCountForDifficulty(DifficultyLevel.BigKids)}, " +
                 $"Adults: {GetQuestionCountForDifficulty(DifficultyLevel.Adults)}");
    }
    
    #endregion
}