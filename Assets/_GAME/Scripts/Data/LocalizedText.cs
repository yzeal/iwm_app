using UnityEngine;
using System;

/// <summary>
/// ScriptableObject f�r einzelne lokalisierte Texte
/// Enth�lt �bersetzungen f�r alle unterst�tzten Sprachen mit Fallback-System
/// </summary>
[CreateAssetMenu(fileName = "New Localized Text", menuName = "Localization/Localized Text")]
public class LocalizedText : ScriptableObject
{
    [Header("Text Identifikation")]
    [SerializeField] private string textKey;
    [TextArea(2, 5)]
    [SerializeField] private string description = "Beschreibung des Texts f�r Editor";

    [Header("�bersetzungen")]
    [TextArea(2, 10)]
    [SerializeField] private string germanStandard = "";
    
    [TextArea(2, 10)]
    [SerializeField] private string englishStandard = "";
    
    [TextArea(2, 10)]
    [SerializeField] private string germanSimple = "";
    
    [TextArea(2, 10)]
    [SerializeField] private string englishSimple = "";

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    public string TextKey => textKey;

    /// <summary>
    /// Text f�r angegebene Sprache abrufen mit Fallback-System
    /// </summary>
    public string GetText(LanguageSystem.Language language = LanguageSystem.Language.German_Standard)
    {
        string text = GetDirectText(language);
        
        // Fallback-System wenn Text leer oder null
        if (string.IsNullOrEmpty(text))
        {
            text = TryFallback(language);
        }

        if (enableDebugLogs && string.IsNullOrEmpty(text))
        {
            Debug.LogWarning($"No text found for key '{textKey}' in language '{language}' or fallbacks");
        }

        return text ?? $"[MISSING: {textKey}]";
    }

    /// <summary>
    /// Direkten Text f�r Sprache abrufen (ohne Fallback)
    /// </summary>
    private string GetDirectText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.German_Standard => germanStandard,
            LanguageSystem.Language.English_Standard => englishStandard,
            LanguageSystem.Language.German_Simple => germanSimple,
            LanguageSystem.Language.English_Simple => englishSimple,
            _ => germanStandard
        };
    }

    /// <summary>
    /// Fallback-System durchlaufen
    /// </summary>
    private string TryFallback(LanguageSystem.Language originalLanguage)
    {
        // Erste Fallback-Stufe: Standard-Variante der Sprache
        if (LanguageSystem.Instance.IsSimpleLanguage(originalLanguage))
        {
            LanguageSystem.Language standardVariant = LanguageSystem.Instance.GetStandardVariant(originalLanguage);
            string standardText = GetDirectText(standardVariant);
            if (!string.IsNullOrEmpty(standardText))
            {
                if (enableDebugLogs)
                    Debug.Log($"Using standard variant fallback for '{textKey}': {originalLanguage} -> {standardVariant}");
                return standardText;
            }
        }

        // Zweite Fallback-Stufe: Allgemeine Fallback-Sprache
        LanguageSystem.Language fallbackLanguage = LanguageSystem.Instance.GetFallbackLanguage(originalLanguage);
        if (fallbackLanguage != originalLanguage)
        {
            string fallbackText = GetDirectText(fallbackLanguage);
            if (!string.IsNullOrEmpty(fallbackText))
            {
                if (enableDebugLogs)
                    Debug.Log($"Using general fallback for '{textKey}': {originalLanguage} -> {fallbackLanguage}");
                return fallbackText;
            }
        }

        // Letzte Fallback-Stufe: Immer Deutsch Standard
        if (originalLanguage != LanguageSystem.Language.German_Standard)
        {
            string germanText = GetDirectText(LanguageSystem.Language.German_Standard);
            if (!string.IsNullOrEmpty(germanText))
            {
                if (enableDebugLogs)
                    Debug.Log($"Using final fallback (German) for '{textKey}': {originalLanguage} -> German_Standard");
                return germanText;
            }
        }

        return null;
    }

    /// <summary>
    /// Pr�ft ob f�r eine Sprache eine �bersetzung vorhanden ist
    /// </summary>
    public bool HasTranslation(LanguageSystem.Language language)
    {
        return !string.IsNullOrEmpty(GetDirectText(language));
    }

    /// <summary>
    /// Pr�ft ob f�r alle Sprachen �bersetzungen vorhanden sind
    /// </summary>
    public bool HasAllTranslations()
    {
        return HasTranslation(LanguageSystem.Language.German_Standard) &&
               HasTranslation(LanguageSystem.Language.English_Standard) &&
               HasTranslation(LanguageSystem.Language.German_Simple) &&
               HasTranslation(LanguageSystem.Language.English_Simple);
    }

    /// <summary>
    /// Automatisch Text-Key aus Dateinamen setzen (Editor-Helper)
    /// </summary>
    [ContextMenu("Set Key From Name")]
    private void SetKeyFromName()
    {
        textKey = name;
    }

    /// <summary>
    /// Validierung: Zeige fehlende �bersetzungen
    /// </summary>
    [ContextMenu("Validate Translations")]
    private void ValidateTranslations()
    {
        // .NET Framework 4.7.1 kompatible Enum-Iteration
        foreach (LanguageSystem.Language lang in System.Enum.GetValues(typeof(LanguageSystem.Language)))
        {
            bool hasTranslation = HasTranslation(lang);
            Debug.Log($"'{textKey}' - {lang}: {(hasTranslation ? "?" : "?")}");
        }
    }
}