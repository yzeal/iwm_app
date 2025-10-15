using UnityEngine;
using System;

/// <summary>
/// Zentrale Verwaltung der Mehrsprachigkeit mit Fallback-System
/// Singleton-Pattern für globalen Zugriff
/// </summary>
public class LanguageSystem : MonoBehaviour
{
    /// <summary>
    /// Verfügbare Sprachen im System
    /// </summary>
    [System.Serializable]
    public enum Language
    {
        German_Standard,    // Default
        English_Standard,
        German_Simple,
        English_Simple
    }

    private static LanguageSystem instance;
    public static LanguageSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LanguageSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("LanguageSystem");
                    instance = go.AddComponent<LanguageSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Current Language Settings")]
    [SerializeField] private Language currentLanguage = Language.German_Standard;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    public static event Action<Language> OnLanguageChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguageFromPrefs();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Aktuelle Sprache abrufen
    /// </summary>
    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }

    /// <summary>
    /// Sprache ändern und persistieren
    /// </summary>
    public void SetLanguage(Language newLanguage)
    {
        if (currentLanguage != newLanguage)
        {
            currentLanguage = newLanguage;
            SaveLanguageToPrefs();
            OnLanguageChanged?.Invoke(currentLanguage);
            
            if (enableDebugLogs)
                Debug.Log($"Language changed to: {newLanguage}");
        }
    }

    /// <summary>
    /// Fallback-Sprache für gegebene Sprache ermitteln
    /// </summary>
    public Language GetFallbackLanguage(Language language)
    {
        return language switch
        {
            Language.English_Standard => Language.German_Standard,
            Language.German_Simple => Language.German_Standard,
            Language.English_Simple => Language.English_Standard,
            Language.German_Standard => Language.German_Standard, // Keine weitere Fallback
            _ => Language.German_Standard
        };
    }

    /// <summary>
    /// Prüft ob eine Sprache eine "einfache" Variante ist
    /// </summary>
    public bool IsSimpleLanguage(Language language)
    {
        return language == Language.German_Simple || language == Language.English_Simple;
    }

    /// <summary>
    /// Gibt die Standard-Variante einer Sprache zurück
    /// </summary>
    public Language GetStandardVariant(Language language)
    {
        return language switch
        {
            Language.German_Simple => Language.German_Standard,
            Language.English_Simple => Language.English_Standard,
            _ => language
        };
    }

    /// <summary>
    /// Sprache laden aus PlayerPrefs
    /// </summary>
    private void LoadLanguageFromPrefs()
    {
        try
        {
            string savedLanguage = PlayerPrefs.GetString("SelectedLanguage", Language.German_Standard.ToString());
            if (Enum.TryParse<Language>(savedLanguage, out Language loadedLanguage))
            {
                currentLanguage = loadedLanguage;
            }
            else
            {
                currentLanguage = Language.German_Standard;
            }
        }
        catch (Exception e)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"Failed to load language from prefs: {e.Message}");
            currentLanguage = Language.German_Standard;
        }
    }

    /// <summary>
    /// Sprache speichern in PlayerPrefs
    /// </summary>
    private void SaveLanguageToPrefs()
    {
        try
        {
            PlayerPrefs.SetString("SelectedLanguage", currentLanguage.ToString());
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"Failed to save language to prefs: {e.Message}");
        }
    }

    /// <summary>
    /// Alle verfügbaren Sprachen abrufen
    /// </summary>
    public Language[] GetAllLanguages()
    {
        return (Language[])Enum.GetValues(typeof(Language));
    }

    /// <summary>
    /// Sprachenname für UI anzeigen
    /// </summary>
    public string GetLanguageDisplayName(Language language)
    {
        return language switch
        {
            Language.German_Standard => "Deutsch",
            Language.English_Standard => "English",
            Language.German_Simple => "Deutsch (Einfach)",
            Language.English_Simple => "English (Simple)",
            _ => "Unknown"
        };
    }
}