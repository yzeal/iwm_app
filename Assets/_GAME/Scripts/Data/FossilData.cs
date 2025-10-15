using UnityEngine;

[System.Serializable]
public class FossilData
{
    [Header("Lokalisierter Fossil-Name")]
    [SerializeField] private LocalizedText localizedFossilName;
    
    [Header("Legacy - Fossil Name (DEPRECATED - use LocalizedText)")]
    [SerializeField] private string fossilName; // Für Rückwärtskompatibilität
    
    [Header("Fossil Visual")]
    public Sprite fossilImage;
    
    [Header("Lokalisierte Beschreibung (Optional)")]
    [SerializeField] private LocalizedText localizedDescription;
    
    [Header("Legacy - Description (DEPRECATED - use LocalizedText)")]
    [TextArea(2, 3)]
    [SerializeField] private string description; // Für Rückwärtskompatibilität

    /// <summary>
    /// Konstruktor für Legacy-Support
    /// </summary>
    public FossilData(string name, Sprite image)
    {
        fossilName = name;
        fossilImage = image;
    }

    /// <summary>
    /// Konstruktor für Lokalisierung
    /// </summary>
    public FossilData(LocalizedText localizedName, Sprite image)
    {
        localizedFossilName = localizedName;
        fossilImage = image;
    }

    /// <summary>
    /// Fossil-Name für aktuelle Sprache abrufen
    /// </summary>
    public string GetFossilName(LanguageSystem.Language language = LanguageSystem.Language.German_Standard)
    {
        // Neues Lokalisierungssystem bevorzugen
        if (localizedFossilName != null)
        {
            return localizedFossilName.GetText(language);
        }
        
        // Legacy-Fallback
        if (!string.IsNullOrEmpty(fossilName))
        {
            return fossilName;
        }
        
        return "[MISSING FOSSIL NAME]";
    }

    /// <summary>
    /// Fossil-Beschreibung für aktuelle Sprache abrufen
    /// </summary>
    public string GetDescription(LanguageSystem.Language language = LanguageSystem.Language.German_Standard)
    {
        // Neues Lokalisierungssystem bevorzugen
        if (localizedDescription != null)
        {
            return localizedDescription.GetText(language);
        }
        
        // Legacy-Fallback
        if (!string.IsNullOrEmpty(description))
        {
            return description;
        }
        
        return ""; // Beschreibung ist optional
    }

    /// <summary>
    /// Prüft ob Lokalisierung verfügbar ist
    /// </summary>
    public bool HasLocalization()
    {
        return localizedFossilName != null;
    }

    /// <summary>
    /// Prüft ob alle Lokalisierungen vollständig sind
    /// </summary>
    public bool HasAllLocalizations()
    {
        if (localizedFossilName == null || !localizedFossilName.HasAllTranslations())
            return false;

        // Beschreibung ist optional
        if (localizedDescription != null && !localizedDescription.HasAllTranslations())
            return false;

        return true;
    }

    #region Legacy Properties für Rückwärtskompatibilität

    /// <summary>
    /// Legacy-Property für Rückwärtskompatibilität
    /// </summary>
    [System.Obsolete("Use GetFossilName(Language) instead")]
    public string FossilName => GetFossilName(LanguageSystem.Language.German_Standard);

    #endregion

    #region Validation Methods

    /// <summary>
    /// Zeigt Lokalisierungsstatus für Editor
    /// </summary>
    [ContextMenu("Validate Localizations")]
    private void ValidateLocalizations()
    {
        string status = $"Fossil Localization Status:\n";
        status += $"Name: {(localizedFossilName != null ? "?" : "?")}\n";
        status += $"Description: {(localizedDescription != null ? "?" : "Optional")}\n";
        
        if (localizedFossilName != null)
        {
            status += $"Name Complete: {(localizedFossilName.HasAllTranslations() ? "?" : "?")}\n";
        }

        Debug.Log(status);
    }

    #endregion
}