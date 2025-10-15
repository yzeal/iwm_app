using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewTabuTerm", menuName = "Game/Tabu Term")]
[Serializable]
public class TabuTerm : ScriptableObject
{
    [Header("Main Term (Lokalisiert)")]
    [SerializeField] private string mainTermGermanStandard;
    [SerializeField] private string mainTermEnglishStandard;
    [SerializeField] private string mainTermGermanSimple;
    [SerializeField] private string mainTermEnglishSimple;

    [Header("Tabu Words (Lokalisiert)")]
    [SerializeField] private string[] tabuWordsGermanStandard;
    [SerializeField] private string[] tabuWordsEnglishStandard;
    [SerializeField] private string[] tabuWordsGermanSimple;
    [SerializeField] private string[] tabuWordsEnglishSimple;

    [Header("Optional: Image")]
    [SerializeField] private Sprite termImage;

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Gibt den Hauptbegriff in der gewünschten Sprache zurück
    /// </summary>
    public string GetMainTerm(LanguageSystem.Language language)
    {
        string term = language switch
        {
            LanguageSystem.Language.German_Standard => mainTermGermanStandard,
            LanguageSystem.Language.English_Standard => mainTermEnglishStandard,
            LanguageSystem.Language.German_Simple => mainTermGermanSimple,
            LanguageSystem.Language.English_Simple => mainTermEnglishSimple,
            _ => mainTermGermanStandard
        };

        // Fallback-Hierarchie
        if (string.IsNullOrEmpty(term))
        {
            term = GetFallbackMainTerm(language);
        }

        return term;
    }

    /// <summary>
    /// Gibt die Tabu-Wörter in der gewünschten Sprache zurück
    /// </summary>
    public string[] GetTabuWords(LanguageSystem.Language language)
    {
        string[] words = language switch
        {
            LanguageSystem.Language.German_Standard => tabuWordsGermanStandard,
            LanguageSystem.Language.English_Standard => tabuWordsEnglishStandard,
            LanguageSystem.Language.German_Simple => tabuWordsGermanSimple,
            LanguageSystem.Language.English_Simple => tabuWordsEnglishSimple,
            _ => tabuWordsGermanStandard
        };

        // Fallback-Hierarchie
        if (words == null || words.Length == 0)
        {
            words = GetFallbackTabuWords(language);
        }

        return words;
    }

    /// <summary>
    /// Optional: Gibt das Bild für den Begriff zurück
    /// </summary>
    public Sprite GetTermImage()
    {
        return termImage;
    }

    // ==================== VALIDATION ====================

    /// <summary>
    /// Prüft, ob alle Lokalisierungen vorhanden sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        return !string.IsNullOrEmpty(mainTermGermanStandard) &&
               !string.IsNullOrEmpty(mainTermEnglishStandard) &&
               !string.IsNullOrEmpty(mainTermGermanSimple) &&
               !string.IsNullOrEmpty(mainTermEnglishSimple) &&
               tabuWordsGermanStandard != null && tabuWordsGermanStandard.Length > 0 &&
               tabuWordsEnglishStandard != null && tabuWordsEnglishStandard.Length > 0 &&
               tabuWordsGermanSimple != null && tabuWordsGermanSimple.Length > 0 &&
               tabuWordsEnglishSimple != null && tabuWordsEnglishSimple.Length > 0;
    }

    /// <summary>
    /// Editor Context Menu: Validierung
    /// </summary>
    [ContextMenu("Validate Localizations")]
    private void ValidateLocalizations()
    {
        if (HasAllLocalizations())
        {
            Debug.Log($"? TabuTerm '{name}' hat alle Lokalisierungen", this);
        }
        else
        {
            Debug.LogWarning($"? TabuTerm '{name}' hat fehlende Lokalisierungen!", this);
            
            if (string.IsNullOrEmpty(mainTermGermanStandard)) Debug.LogWarning("  - German Standard Main Term fehlt");
            if (string.IsNullOrEmpty(mainTermEnglishStandard)) Debug.LogWarning("  - English Standard Main Term fehlt");
            if (string.IsNullOrEmpty(mainTermGermanSimple)) Debug.LogWarning("  - German Simple Main Term fehlt");
            if (string.IsNullOrEmpty(mainTermEnglishSimple)) Debug.LogWarning("  - English Simple Main Term fehlt");
            
            if (tabuWordsGermanStandard == null || tabuWordsGermanStandard.Length == 0) 
                Debug.LogWarning("  - German Standard Tabu Words fehlen");
            if (tabuWordsEnglishStandard == null || tabuWordsEnglishStandard.Length == 0) 
                Debug.LogWarning("  - English Standard Tabu Words fehlen");
            if (tabuWordsGermanSimple == null || tabuWordsGermanSimple.Length == 0) 
                Debug.LogWarning("  - German Simple Tabu Words fehlen");
            if (tabuWordsEnglishSimple == null || tabuWordsEnglishSimple.Length == 0) 
                Debug.LogWarning("  - English Simple Tabu Words fehlen");
        }
    }

    // ==================== FALLBACK SYSTEM ====================

    private string GetFallbackMainTerm(LanguageSystem.Language language)
    {
        // Fallback-Hierarchie: Simple ? Standard, English ? German
        if (language == LanguageSystem.Language.German_Simple && !string.IsNullOrEmpty(mainTermGermanStandard))
            return mainTermGermanStandard;
        
        if (language == LanguageSystem.Language.English_Simple && !string.IsNullOrEmpty(mainTermEnglishStandard))
            return mainTermEnglishStandard;
        
        if ((language == LanguageSystem.Language.English_Standard || language == LanguageSystem.Language.English_Simple) 
            && !string.IsNullOrEmpty(mainTermGermanStandard))
            return mainTermGermanStandard;

        return mainTermGermanStandard ?? "[TERM MISSING]";
    }

    private string[] GetFallbackTabuWords(LanguageSystem.Language language)
    {
        // Fallback-Hierarchie: Simple ? Standard, English ? German
        if (language == LanguageSystem.Language.German_Simple && tabuWordsGermanStandard != null && tabuWordsGermanStandard.Length > 0)
            return tabuWordsGermanStandard;
        
        if (language == LanguageSystem.Language.English_Simple && tabuWordsEnglishStandard != null && tabuWordsEnglishStandard.Length > 0)
            return tabuWordsEnglishStandard;
        
        if ((language == LanguageSystem.Language.English_Standard || language == LanguageSystem.Language.English_Simple) 
            && tabuWordsGermanStandard != null && tabuWordsGermanStandard.Length > 0)
            return tabuWordsGermanStandard;

        return tabuWordsGermanStandard ?? new string[0];
    }
}