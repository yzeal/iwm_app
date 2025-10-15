using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Tabu Collection", menuName = "Game/Tabu Collection")]
public class TabuCollection : ScriptableObject
{
    [Header("Lokalisierte Collection Info")]
    [SerializeField] private LocalizedText localizedCollectionName;
    
    [Header("Legacy - Collection Info (DEPRECATED - use LocalizedText)")]
    [SerializeField] private string collectionName; // F�r R�ckw�rtskompatibilit�t
    public int roomNumber;

    [Header("Team Images")]
    public Sprite team1Image;
    public Sprite team2Image;

    [Header("Terms nach Schwierigkeitsgrad")]
    [SerializeField] private TabuTerm[] kidsTerms;
    [SerializeField] private TabuTerm[] bigKidsTerms;
    [SerializeField] private TabuTerm[] adultsTerms;

    [Header("Game Settings")]
    [Range(3, 10)]
    public int termsPerRound = 5;
    [Range(15f, 120f)]
    public float roundDuration = 60f;
    
    [Header("Zeit-Einstellungen")]
    public DifficultyTimeSettings timeSettings = new DifficultyTimeSettings();

    // ==================== PUBLIC GETTERS ====================

    /// <summary>
    /// Collection-Name f�r aktuelle Sprache abrufen
    /// </summary>
    public string GetCollectionName(LanguageSystem.Language language = LanguageSystem.Language.German_Standard)
    {
        // Neues Lokalisierungssystem bevorzugen
        if (localizedCollectionName != null)
        {
            return localizedCollectionName.GetText(language);
        }
        
        // Legacy-Fallback
        if (!string.IsNullOrEmpty(collectionName))
        {
            return collectionName;
        }
        
        return $"[MISSING COLLECTION NAME {roomNumber}]";
    }

    public float DefaultRoundDuration => roundDuration;
    public int TermsPerRound => termsPerRound;
    public DifficultyTimeSettings TimeSettings => timeSettings;
    public Sprite Team1Image => team1Image;
    public Sprite Team2Image => team2Image;

    // ==================== TERM SELECTION ====================

    /// <summary>
    /// Gibt Terms f�r einen bestimmten Schwierigkeitsgrad zur�ck
    /// </summary>
    public TabuTerm[] GetTermsForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsTerms ?? new TabuTerm[0],
            DifficultyLevel.BigKids => bigKidsTerms ?? new TabuTerm[0],
            DifficultyLevel.Adults => adultsTerms ?? new TabuTerm[0],
            _ => adultsTerms ?? new TabuTerm[0]
        };
    }

    /// <summary>
    /// Gibt zuf�llige Terms f�r einen Schwierigkeitsgrad zur�ck
    /// </summary>
    public List<TabuTerm> GetRandomTerms(int count, DifficultyLevel difficulty)
    {
        TabuTerm[] availableTerms = GetTermsForDifficulty(difficulty);

        if (availableTerms == null || availableTerms.Length == 0)
        {
            Debug.LogWarning($"Keine Terms f�r Schwierigkeitsgrad {difficulty} gefunden!");
            return new List<TabuTerm>();
        }

        // Shuffle und nehme die ersten 'count' Elemente
        List<TabuTerm> shuffled = availableTerms.OrderBy(x => Random.value).ToList();
        return shuffled.Take(Mathf.Min(count, shuffled.Count)).ToList();
    }

    /// <summary>
    /// Gibt zuf�llige Terms zur�ck mit Exklusionsliste (f�r Team 2)
    /// Falls nicht genug unique Terms verf�gbar, werden Duplikate erlaubt
    /// </summary>
    public List<TabuTerm> GetRandomTermsExcluding(int count, DifficultyLevel difficulty, List<TabuTerm> excludeTerms)
    {
        TabuTerm[] availableTerms = GetTermsForDifficulty(difficulty);

        if (availableTerms == null || availableTerms.Length == 0)
        {
            Debug.LogWarning($"Keine Terms f�r Schwierigkeitsgrad {difficulty} gefunden!");
            return new List<TabuTerm>();
        }

        // Filtere ausgeschlossene Terms
        List<TabuTerm> filteredTerms = availableTerms.Where(t => !excludeTerms.Contains(t)).ToList();

        // Falls nicht genug �brig sind, erlaube Duplikate
        if (filteredTerms.Count < count)
        {
            Debug.Log($"Nicht genug unique Terms ({filteredTerms.Count}/{count}). Duplikate erlaubt.");
            filteredTerms = availableTerms.ToList();
        }

        // Shuffle und nehme die ersten 'count' Elemente
        List<TabuTerm> shuffled = filteredTerms.OrderBy(x => Random.value).ToList();
        return shuffled.Take(Mathf.Min(count, shuffled.Count)).ToList();
    }

    /// <summary>
    /// Berechnet die angepasste Rundendauer basierend auf Schwierigkeitsgrad
    /// </summary>
    public float GetAdjustedRoundDuration(DifficultyLevel difficulty)
    {
        return roundDuration * timeSettings.GetTimeMultiplier(difficulty);
    }
    
    /// <summary>
    /// Gibt die Anzahl verf�gbarer Terms f�r einen Schwierigkeitsgrad zur�ck
    /// </summary>
    public int GetTermCountForDifficulty(DifficultyLevel difficulty)
    {
        return GetTermsForDifficulty(difficulty).Length;
    }

    // ==================== VALIDATION ====================

    /// <summary>
    /// Validierung: Pr�ft ob f�r alle Schwierigkeitsgrade Terms vorhanden sind
    /// </summary>
    public bool HasTermsForAllDifficulties()
    {
        return kidsTerms != null && kidsTerms.Length > 0 &&
               bigKidsTerms != null && bigKidsTerms.Length > 0 &&
               adultsTerms != null && adultsTerms.Length > 0;
    }

    /// <summary>
    /// Pr�ft ob alle Lokalisierungen vollst�ndig sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        // Collection-Name pr�fen
        if (localizedCollectionName == null || !localizedCollectionName.HasAllTranslations())
            return false;

        // Alle Terms pr�fen - .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var terms = GetTermsForDifficulty(difficulty);
            foreach (var term in terms)
            {
                if (term != null && !term.HasAllLocalizations())
                    return false;
            }
        }

        return true;
    }

    #region Legacy Support - f�r bestehenden Code
    
    /// <summary>
    /// Legacy-Property f�r R�ckw�rtskompatibilit�t
    /// </summary>
    [System.Obsolete("Use GetCollectionName(Language) instead")]
    public string CollectionName => GetCollectionName(LanguageSystem.Language.German_Standard);
    
    #endregion
    
    #region Editor Helper Methods
    
    [ContextMenu("Validate All Difficulties")]
    private void ValidateTerms()
    {
        Debug.Log($"Collection {GetCollectionName()} - Kids: {GetTermCountForDifficulty(DifficultyLevel.Kids)}, " +
                 $"BigKids: {GetTermCountForDifficulty(DifficultyLevel.BigKids)}, " +
                 $"Adults: {GetTermCountForDifficulty(DifficultyLevel.Adults)}");
    }

    [ContextMenu("Validate All Localizations")]
    private void ValidateLocalizations()
    {
        string status = $"Collection '{GetCollectionName()}' Localization Status:\n";
        status += $"Collection Name: {(localizedCollectionName != null && localizedCollectionName.HasAllTranslations() ? "?" : "?")}\n";

        // .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var terms = GetTermsForDifficulty(difficulty);
            int localizedCount = 0;
            foreach (var term in terms)
            {
                if (term != null && term.HasAllLocalizations()) 
                    localizedCount++;
            }
            status += $"{difficulty}: {localizedCount}/{terms.Length} terms localized\n";
        }

        Debug.Log(status, this);
    }

    [ContextMenu("Test: Get Random Terms (Kids, 5)")]
    private void TestGetRandomTermsKids()
    {
        var terms = GetRandomTerms(5, DifficultyLevel.Kids);
        Debug.Log($"Random Kids Terms ({terms.Count}):");
        foreach (var term in terms)
        {
            Debug.Log($"  - {term.GetMainTerm(LanguageSystem.Language.German_Standard)}");
        }
    }

    [ContextMenu("Test: Get Random Terms Excluding (BigKids, 3)")]
    private void TestGetRandomTermsExcluding()
    {
        var excludeList = GetRandomTerms(2, DifficultyLevel.BigKids);
        Debug.Log($"Excluded Terms ({excludeList.Count}):");
        foreach (var term in excludeList)
        {
            Debug.Log($"  - {term.GetMainTerm(LanguageSystem.Language.German_Standard)}");
        }

        var selectedTerms = GetRandomTermsExcluding(3, DifficultyLevel.BigKids, excludeList);
        Debug.Log($"Selected Terms ({selectedTerms.Count}):");
        foreach (var term in selectedTerms)
        {
            Debug.Log($"  - {term.GetMainTerm(LanguageSystem.Language.German_Standard)}");
        }
    }
    
    #endregion
}