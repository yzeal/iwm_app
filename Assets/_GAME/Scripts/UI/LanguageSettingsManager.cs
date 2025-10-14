using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Vereinfachter Manager für Sprachauswahl-Settings
/// Sprache wird sofort bei Auswahl übernommen
/// </summary>
public class LanguageSettingsManager : MonoBehaviour
{
    [Header("Language Selection UI")]
    public LanguageRadioGroup languageRadioGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;
    
    [Header("Action Buttons")]
    public Button backButton;
    
    [Header("Mobile UI Settings")]
    [SerializeField] private bool adaptToSafeArea = true;
    
    [Header("Localized UI Texts")]
    [SerializeField] private LocalizedText titleLocalizedText;
    [SerializeField] private LocalizedText instructionLocalizedText;
    [SerializeField] private LocalizedText backButtonLocalizedText;
    
    void Start()
    {
        // Mobile-spezifische Initialisierung
        if (adaptToSafeArea)
        {
            AdaptToSafeArea();
        }
        
        InitializeSettings();
        SetupUI();
        UpdateLocalizedTexts();
        
        // Event-Listener für Sprachwechsel
        LanguageSystem.OnLanguageChanged += OnLanguageChanged;
    }
    
    void OnDestroy()
    {
        LanguageSystem.OnLanguageChanged -= OnLanguageChanged;
    }
    
    void AdaptToSafeArea()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                Rect safeArea = Screen.safeArea;
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;
                
                canvasRect.anchorMin = anchorMin;
                canvasRect.anchorMax = anchorMax;
            }
        }
    }
    
    void InitializeSettings()
    {
        // Lade aktuelle Sprache
        Language currentLanguage = Language.German_Standard;
        
        if (GameDataManager.Instance != null)
        {
            currentLanguage = GameDataManager.Instance.CurrentLanguage;
        }
        else if (LanguageSystem.Instance != null)
        {
            currentLanguage = LanguageSystem.Instance.GetCurrentLanguage();
        }
        
        // Setze UI basierend auf aktueller Sprache
        languageRadioGroup.SetSelectedLanguage(currentLanguage);
        
        Debug.Log($"Language settings loaded: {currentLanguage}");
    }
    
    void SetupUI()
    {
        // Setup Button Events
        backButton.onClick.AddListener(BackToMenu);
        
        // Setup Language Group Events - Sprache wird sofort übernommen
        languageRadioGroup.OnLanguageChanged += OnLanguageSelectionChanged;
        
        // Mobile-optimierte Button-Größe
        AdaptButtonForMobile(backButton);
    }
    
    void UpdateLocalizedTexts()
    {
        Language currentLang = LanguageSystem.Instance != null ? 
            LanguageSystem.Instance.GetCurrentLanguage() : Language.German_Standard;
        
        // UI-Texte aktualisieren
        if (titleLocalizedText && titleText)
            titleText.text = titleLocalizedText.GetText(currentLang);
            
        if (instructionLocalizedText && instructionText)
            instructionText.text = instructionLocalizedText.GetText(currentLang);
        
        if (backButtonLocalizedText && backButton)
        {
            TextMeshProUGUI buttonText = backButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText) buttonText.text = backButtonLocalizedText.GetText(currentLang);
        }
    }
    
    void AdaptButtonForMobile(Button button)
    {
        if (button != null)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                Vector2 currentSize = buttonRect.sizeDelta;
                float minHeight = 50f;
                
                if (currentSize.y < minHeight)
                {
                    buttonRect.sizeDelta = new Vector2(currentSize.x, minHeight);
                }
            }
        }
    }
    
    void OnLanguageSelectionChanged(Language newLanguage)
    {
        // Sprache sofort übernehmen und speichern
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetLanguage(newLanguage);
        }
        else if (LanguageSystem.Instance != null)
        {
            LanguageSystem.Instance.SetLanguage(newLanguage);
        }
        
        Debug.Log($"Language immediately applied: {newLanguage}");
    }
    
    void OnLanguageChanged(Language newLanguage)
    {
        // UI-Texte aktualisieren wenn Sprache gewechselt wird
        UpdateLocalizedTexts();
    }
    
    void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    #region Debug Methods
    
    [ContextMenu("Test Language Switch")]
    void TestLanguageSwitch()
    {
        Language[] languages = { Language.German_Standard, Language.English_Standard, Language.German_Simple, Language.English_Simple };
        foreach (Language lang in languages)
        {
            languageRadioGroup.SelectLanguage(lang);
            Debug.Log($"Switched to: {lang}");
        }
    }
    
    #endregion
}