using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI-Komponente für automatische Text-Lokalisierung
/// Kann sowohl mit Text als auch TextMeshPro verwendet werden
/// </summary>
public class LocalizedTextComponent : MonoBehaviour
{
    [Header("Localization Settings")]
    [SerializeField] private LocalizedText localizedText;
    [SerializeField] private bool updateOnLanguageChange = true;

    [Header("Target Components (Auto-detected if empty)")]
    [SerializeField] private Text targetText;
    [SerializeField] private TextMeshProUGUI targetTextMeshPro;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    private void Awake()
    {
        // Auto-detect Text-Komponenten falls nicht manuell zugewiesen
        if (targetText == null)
            targetText = GetComponent<Text>();
        
        if (targetTextMeshPro == null)
            targetTextMeshPro = GetComponent<TextMeshProUGUI>();

        if (targetText == null && targetTextMeshPro == null)
        {
            Debug.LogWarning($"LocalizedTextComponent on {gameObject.name}: No Text or TextMeshPro component found!");
        }
    }

    private void Start()
    {
        UpdateText();
        
        if (updateOnLanguageChange)
        {
            LanguageSystem.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnDestroy()
    {
        if (updateOnLanguageChange)
        {
            LanguageSystem.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    /// <summary>
    /// Text aktualisieren basierend auf aktueller Sprache
    /// </summary>
    public void UpdateText()
    {
        if (localizedText == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"LocalizedTextComponent on {gameObject.name}: No LocalizedText assigned!");
            return;
        }

        Language currentLanguage = LanguageSystem.Instance.GetCurrentLanguage();
        string translatedText = localizedText.GetText(currentLanguage);

        SetText(translatedText);

        if (enableDebugLogs)
            Debug.Log($"Updated text for '{localizedText.TextKey}' to: {translatedText}");
    }

    /// <summary>
    /// Text setzen auf verfügbare Komponente
    /// </summary>
    private void SetText(string text)
    {
        if (targetTextMeshPro != null)
        {
            targetTextMeshPro.text = text;
        }
        else if (targetText != null)
        {
            targetText.text = text;
        }
    }

    /// <summary>
    /// LocalizedText zur Laufzeit ändern
    /// </summary>
    public void SetLocalizedText(LocalizedText newLocalizedText)
    {
        localizedText = newLocalizedText;
        UpdateText();
    }

    /// <summary>
    /// Event-Handler für Sprachwechsel
    /// </summary>
    private void OnLanguageChanged(Language newLanguage)
    {
        UpdateText();
    }

    /// <summary>
    /// Force Update (für Editor oder spezielle Situationen)
    /// </summary>
    [ContextMenu("Force Update Text")]
    public void ForceUpdateText()
    {
        UpdateText();
    }
}