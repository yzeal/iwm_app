using UnityEngine;

[CreateAssetMenu(fileName = "New Fossil Collection", menuName = "FossilGame/Fossil Collection")]
public class FossilCollection : ScriptableObject
{
    [Header("Lokalisierte Collection Info")]
    [SerializeField] private LocalizedText localizedCollectionName;
    
    [Header("Legacy - Collection Info (DEPRECATED - use LocalizedText)")]
    [SerializeField] private string collectionName; // Für Rückwärtskompatibilität
    public int roomNumber;

    [Header("Team Images")]
    public Sprite team1Image;
    public Sprite team2Image;

    [Header("Fossils nach Schwierigkeitsgrad")]
    [SerializeField] private FossilData[] kidsFossils;
    [SerializeField] private FossilData[] bigKidsFossils;
    [SerializeField] private FossilData[] adultsFossils;

    [Header("Game Settings")]
    [Range(3, 10)]
    public int fossilsPerRound = 5;
    [Range(15f, 120f)]
    public float roundDuration = 30f;
    
    [Header("Zeit-Einstellungen")]
    public DifficultyTimeSettings timeSettings = new DifficultyTimeSettings();

    /// <summary>
    /// Collection-Name für aktuelle Sprache abrufen
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

    /// <summary>
    /// Gibt die Fossilien für den angegebenen Schwierigkeitsgrad zurück
    /// </summary>
    public FossilData[] GetFossilsForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsFossils ?? new FossilData[0],
            DifficultyLevel.BigKids => bigKidsFossils ?? new FossilData[0],
            DifficultyLevel.Adults => adultsFossils ?? new FossilData[0],
            _ => adultsFossils ?? new FossilData[0]
        };
    }

    /// <summary>
    /// Gibt zufällige Fossilien für den angegebenen Schwierigkeitsgrad zurück
    /// </summary>
    public FossilData[] GetRandomFossils(int count, DifficultyLevel difficulty)
    {
        var availableFossils = GetFossilsForDifficulty(difficulty);
        
        if (availableFossils.Length <= count)
            return availableFossils;

        // Fisher-Yates Shuffle für zufällige Auswahl
        FossilData[] shuffled = new FossilData[availableFossils.Length];
        System.Array.Copy(availableFossils, shuffled, availableFossils.Length);

        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        FossilData[] result = new FossilData[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }
    
    /// <summary>
    /// Berechnet die angepasste Rundendauer basierend auf Schwierigkeitsgrad
    /// </summary>
    public float GetAdjustedRoundDuration(DifficultyLevel difficulty)
    {
        return roundDuration * timeSettings.GetTimeMultiplier(difficulty);
    }
    
    /// <summary>
    /// Gibt die Anzahl verfügbarer Fossilien für einen Schwierigkeitsgrad zurück
    /// </summary>
    public int GetFossilCountForDifficulty(DifficultyLevel difficulty)
    {
        return GetFossilsForDifficulty(difficulty).Length;
    }
    
    /// <summary>
    /// Validierung: Prüft ob für alle Schwierigkeitsgrade Fossilien vorhanden sind
    /// </summary>
    public bool HasFossilsForAllDifficulties()
    {
        return kidsFossils != null && kidsFossils.Length > 0 &&
               bigKidsFossils != null && bigKidsFossils.Length > 0 &&
               adultsFossils != null && adultsFossils.Length > 0;
    }

    /// <summary>
    /// Prüft ob alle Lokalisierungen vollständig sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        // Collection-Name prüfen
        if (localizedCollectionName == null || !localizedCollectionName.HasAllTranslations())
            return false;

        // Alle Fossilien prüfen - .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var fossils = GetFossilsForDifficulty(difficulty);
            foreach (var fossil in fossils)
            {
                if (!fossil.HasAllLocalizations())
                    return false;
            }
        }

        return true;
    }

    #region Legacy Support - für bestehenden Code
    
    /// <summary>
    /// Legacy-Property für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetCollectionName(Language) instead")]
    public string CollectionName => GetCollectionName(LanguageSystem.Language.German_Standard);
    
    /// <summary>
    /// Legacy-Methode für Rückwärtskompatibilität - verwendet Adults-Fossilien
    /// </summary>
    [System.Obsolete("Use GetRandomFossils(count, difficulty) instead")]
    public FossilData[] GetRandomFossils(int count)
    {
        return GetRandomFossils(count, DifficultyLevel.Adults);
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    [ContextMenu("Validate All Difficulties")]
    private void ValidateFossils()
    {
        Debug.Log($"Collection {GetCollectionName()} - Kids: {GetFossilCountForDifficulty(DifficultyLevel.Kids)}, " +
                 $"BigKids: {GetFossilCountForDifficulty(DifficultyLevel.BigKids)}, " +
                 $"Adults: {GetFossilCountForDifficulty(DifficultyLevel.Adults)}");
    }

    [ContextMenu("Validate All Localizations")]
    private void ValidateLocalizations()
    {
        string status = $"Collection '{GetCollectionName()}' Localization Status:\n";
        status += $"Collection Name: {(localizedCollectionName != null && localizedCollectionName.HasAllTranslations() ? "?" : "?")}\n";

        // .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
        {
            var fossils = GetFossilsForDifficulty(difficulty);
            int localizedCount = 0;
            foreach (var fossil in fossils)
            {
                if (fossil.HasAllLocalizations()) localizedCount++;
            }
            status += $"{difficulty}: {localizedCount}/{fossils.Length} fossils localized\n";
        }

        Debug.Log(status);
    }
    
    #endregion
}