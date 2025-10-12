using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    [Header("Lokalisierte Frage")]
    [SerializeField] private LocalizedText localizedQuestionText;
    
    [Header("Legacy - Frage (DEPRECATED - use LocalizedText)")]
    [TextArea(2, 4)]
    [SerializeField] private string questionText; // Für Rückwärtskompatibilität
    
    [Header("Lokalisierte Antworten (erste Antwort ist die richtige)")]
    [SerializeField] private LocalizedText[] localizedAnswers = new LocalizedText[4];
    
    [Header("Legacy - Antworten (DEPRECATED - use LocalizedText)")]
    [SerializeField] private string[] answers = new string[4]; // Für Rückwärtskompatibilität

    /// <summary>
    /// Frage-Text für aktuelle Sprache abrufen
    /// </summary>
    public string GetQuestionText(Language language = Language.German_Standard)
    {
        // Neues Lokalisierungssystem bevorzugen
        if (localizedQuestionText != null)
        {
            return localizedQuestionText.GetText(language);
        }
        
        // Legacy-Fallback
        if (!string.IsNullOrEmpty(questionText))
        {
            return questionText;
        }
        
        return "[MISSING QUESTION]";
    }

    /// <summary>
    /// Korrekte Antwort für aktuelle Sprache abrufen
    /// </summary>
    public string GetCorrectAnswer(Language language = Language.German_Standard)
    {
        return GetAnswer(0, language);
    }

    /// <summary>
    /// Antwort an bestimmtem Index für aktuelle Sprache abrufen
    /// </summary>
    public string GetAnswer(int index, Language language = Language.German_Standard)
    {
        if (index < 0 || index >= 4) return "[INVALID INDEX]";

        // Neues Lokalisierungssystem bevorzugen
        if (localizedAnswers != null && 
            index < localizedAnswers.Length && 
            localizedAnswers[index] != null)
        {
            return localizedAnswers[index].GetText(language);
        }
        
        // Legacy-Fallback
        if (answers != null && index < answers.Length && !string.IsNullOrEmpty(answers[index]))
        {
            return answers[index];
        }
        
        return $"[MISSING ANSWER {index}]";
    }

    /// <summary>
    /// Alle Antworten gemischt für aktuelle Sprache abrufen
    /// </summary>
    public string[] GetShuffledAnswers(Language language = Language.German_Standard)
    {
        string[] allAnswers = new string[4];
        for (int i = 0; i < 4; i++)
        {
            allAnswers[i] = GetAnswer(i, language);
        }

        // Fisher-Yates Shuffle
        for (int i = allAnswers.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (allAnswers[i], allAnswers[randomIndex]) = (allAnswers[randomIndex], allAnswers[i]);
        }
        
        return allAnswers;
    }
    
    /// <summary>
    /// Index der korrekten Antwort in gemischtem Array finden
    /// </summary>
    public int GetCorrectAnswerIndex(string[] shuffledAnswers, Language language = Language.German_Standard)
    {
        string correctAnswer = GetCorrectAnswer(language);
        for (int i = 0; i < shuffledAnswers.Length; i++)
        {
            if (shuffledAnswers[i] == correctAnswer)
                return i;
        }
        return -1;
    }

    #region Legacy Support Methods

    /// <summary>
    /// Legacy-Methode für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetCorrectAnswer(Language) instead")]
    public string GetCorrectAnswer() => GetCorrectAnswer(Language.German_Standard);

    /// <summary>
    /// Legacy-Methode für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetShuffledAnswers(Language) instead")]
    public string[] GetShuffledAnswers() => GetShuffledAnswers(Language.German_Standard);

    /// <summary>
    /// Legacy-Methode für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetCorrectAnswerIndex(shuffledAnswers, Language) instead")]
    public int GetCorrectAnswerIndex(string[] shuffledAnswers) => GetCorrectAnswerIndex(shuffledAnswers, Language.German_Standard);

    #endregion

    #region Validation Methods

    /// <summary>
    /// Prüft ob alle Lokalisierungen vorhanden sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        // Frage prüfen
        if (localizedQuestionText == null || !localizedQuestionText.HasAllTranslations())
            return false;

        // Antworten prüfen
        if (localizedAnswers == null || localizedAnswers.Length != 4)
            return false;

        foreach (var answer in localizedAnswers)
        {
            if (answer == null || !answer.HasAllTranslations())
                return false;
        }

        return true;
    }

    /// <summary>
    /// Zeigt Lokalisierungsstatus für Editor
    /// </summary>
    [ContextMenu("Validate Localizations")]
    private void ValidateLocalizations()
    {
        string status = $"Question Localization Status:\n";
        
        status += $"Question: {(localizedQuestionText != null ? "?" : "?")}\n";
        
        for (int i = 0; i < 4; i++)
        {
            bool hasAnswer = localizedAnswers != null && 
                           i < localizedAnswers.Length && 
                           localizedAnswers[i] != null;
            status += $"Answer {i}: {(hasAnswer ? "?" : "?")}\n";
        }

        Debug.Log(status);
    }

    #endregion
}