using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Room", menuName = "Quiz/Room Data")]
public class QuizRoomData : ScriptableObject
{
    [Header("Lokalisierte Raum Information")]
    [SerializeField] private LocalizedText localizedRoomName;
    
    [Header("Legacy - Raum Information (DEPRECATED - use LocalizedText)")]
    [SerializeField] private string roomName; // Für Rückwärtskompatibilität
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
    /// Raum-Name für aktuelle Sprache abrufen
    /// </summary>
    public string GetRoomName(LanguageSystem.Language language = LanguageSystem.Language.German_Standard)
    {
        // Neues Lokalisierungssystem bevorzugen
        if (localizedRoomName != null)
        {
            return localizedRoomName.GetText(language);
        }
        
        // Legacy-Fallback
        if (!string.IsNullOrEmpty(roomName))
        {
            return roomName;
        }
        
        return $"[MISSING ROOM NAME {roomNumber}]";
    }
    
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

    /// <summary>
    /// Prüft ob alle Lokalisierungen vollständig sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        // Raum-Name prüfen
        if (localizedRoomName == null || !localizedRoomName.HasAllTranslations())
            return false;

        // Alle Fragen prüfen - .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var questions = GetQuestionsForDifficulty(difficulty);
            foreach (var question in questions)
            {
                if (!question.HasAllLocalizations())
                    return false;
            }
        }

        return true;
    }

    #region Legacy Properties für Rückwärtskompatibilität

    /// <summary>
    /// Legacy-Property für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetRoomName(Language) instead")]
    public string RoomName => GetRoomName(LanguageSystem.Language.German_Standard);

    #endregion
    
    #region Editor Helper Methods
    
    [ContextMenu("Validate All Difficulties")]
    private void ValidateQuestions()
    {
        Debug.Log($"Room {GetRoomName()} - Kids: {GetQuestionCountForDifficulty(DifficultyLevel.Kids)}, " +
                 $"BigKids: {GetQuestionCountForDifficulty(DifficultyLevel.BigKids)}, " +
                 $"Adults: {GetQuestionCountForDifficulty(DifficultyLevel.Adults)}");
    }

    [ContextMenu("Validate All Localizations")]
    private void ValidateLocalizations()
    {
        string status = $"Room '{GetRoomName()}' Localization Status:\n";
        status += $"Room Name: {(localizedRoomName != null && localizedRoomName.HasAllTranslations() ? "?" : "?")}\n";

        // .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var questions = GetQuestionsForDifficulty(difficulty);
            int localizedCount = 0;
            foreach (var question in questions)
            {
                if (question.HasAllLocalizations()) localizedCount++;
            }
            status += $"{difficulty}: {localizedCount}/{questions.Length} questions localized\n";
        }

        Debug.Log(status);
    }
    
    #endregion
}