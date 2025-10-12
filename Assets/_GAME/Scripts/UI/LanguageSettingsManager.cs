using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager für Sprachauswahl-Settings
/// Ähnlich wie TeamSettingsManager aber für Sprachen
/// </summary>
public class LanguageSettingsManager : MonoBehaviour
{
    [Header("Language Selection UI")]
    public LanguageRadioGroup languageRadioGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;
    
    [Header("Current Language Display")]
    public TextMeshProUGUI currentLanguageLabel;
    public Image languageIcon;
    
    [Header("Action Buttons")]
    public Button applyButton;
    public Button backButton;
    
    [Header("Language Icons")]
    public Sprite germanIcon;
    public Sprite englishIcon;
    public Sprite simpleLanguageIcon;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip appliedSound;
    
    [Header("Mobile UI Settings")]
    [SerializeField] private bool adaptToSafeArea = true;
    
    [Header("Localized UI Texts")]
    [SerializeField] private LocalizedText titleLocalizedText;
    [SerializeField] private LocalizedText instructionLocalizedText;
    [SerializeField] private LocalizedText applyButtonLocalizedText;
    [SerializeField] private LocalizedText backButtonLocalizedText;
    [SerializeField] private LocalizedText currentLanguageLabelLocalizedText;
    
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
        UpdateCurrentLanguageDisplay(currentLanguage);
        
        Debug.Log($"Language settings loaded: {currentLanguage}");
    }
    
    void SetupUI()
    {
        // Setup Button Events
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMenu);
        
        // Setup Language Group Events
        languageRadioGroup.OnLanguageChanged += OnLanguageSelectionChanged;
        
        // Mobile-optimierte Button-Größen
        AdaptButtonsForMobile();
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
            
        if (applyButtonLocalizedText && applyButton)
        {
            TextMeshProUGUI buttonText = applyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText) buttonText.text = applyButtonLocalizedText.GetText(currentLang);
        }
        
        if (backButtonLocalizedText && backButton)
        {
            TextMeshProUGUI buttonText = backButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText) buttonText.text = backButtonLocalizedText.GetText(currentLang);
        }
        
        if (currentLanguageLabelLocalizedText && currentLanguageLabel)
            currentLanguageLabel.text = currentLanguageLabelLocalizedText.GetText(currentLang);
    }
    
    void AdaptButtonsForMobile()
    {
        Button[] buttons = { applyButton, backButton };
        
        foreach (Button button in buttons)
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
    }
    
    void OnLanguageSelectionChanged(Language newLanguage)
    {
        PlayButtonSound();
        UpdateCurrentLanguageDisplay(newLanguage);
        TriggerHapticFeedback();
        
        Debug.Log($"Language selection changed to: {newLanguage}");
    }
    
    void UpdateCurrentLanguageDisplay(Language language)
    {
        // Icon aktualisieren
        if (languageIcon)
        {
            languageIcon.sprite = GetLanguageIcon(language);
        }
        
        // Label aktualisieren (zeigt gewählte Sprache in dieser Sprache an)
        if (currentLanguageLabel)
        {
            string displayName = LanguageSystem.Instance != null ? 
                LanguageSystem.Instance.GetLanguageDisplayName(language) : language.ToString();
            currentLanguageLabel.text = $"Gewählt: {displayName}";
        }
    }
    
    Sprite GetLanguageIcon(Language language)
    {
        return language switch
        {
            Language.German_Standard => germanIcon,
            Language.English_Standard => englishIcon,
            Language.German_Simple => simpleLanguageIcon ?? germanIcon,
            Language.English_Simple => simpleLanguageIcon ?? englishIcon,
            _ => germanIcon
        };
    }
    
    void OnLanguageChanged(Language newLanguage)
    {
        // UI-Texte aktualisieren wenn Sprache gewechselt wird
        UpdateLocalizedTexts();
        UpdateCurrentLanguageDisplay(newLanguage);
    }
    
    void ApplySettings()
    {
        PlayButtonSound();
        
        Language selectedLanguage = languageRadioGroup.GetSelectedLanguage();
        
        // Speichere Sprache im GameDataManager (das synchronisiert automatisch mit LanguageSystem)
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetLanguage(selectedLanguage);
        }
        else if (LanguageSystem.Instance != null)
        {
            LanguageSystem.Instance.SetLanguage(selectedLanguage);
        }
        
        Debug.Log($"Language applied: {selectedLanguage}");
        
        // Feedback für den Nutzer
        ShowAppliedFeedback();
        TriggerHapticFeedback();
    }
    
    void ShowAppliedFeedback()
    {
        StartCoroutine(AppliedFeedbackCoroutine());
        
        if (audioSource && appliedSound)
        {
            audioSource.PlayOneShot(appliedSound);
        }
    }
    
    System.Collections.IEnumerator AppliedFeedbackCoroutine()
    {
        Color originalColor = applyButton.GetComponent<Image>().color;
        applyButton.GetComponent<Image>().color = Color.green;
        
        TextMeshProUGUI buttonText = applyButton.GetComponentInChildren<TextMeshProUGUI>();
        string originalText = buttonText.text;
        
        // Feedback-Text in aktueller Sprache
        Language currentLang = LanguageSystem.Instance.GetCurrentLanguage();
        string appliedText = currentLang switch
        {
            Language.English_Standard => "Applied!",
            Language.English_Simple => "Saved!",
            Language.German_Simple => "Gespeichert!",
            _ => "Gespeichert!"
        };
        
        buttonText.text = appliedText;
        
        yield return new WaitForSeconds(1.5f);
        
        applyButton.GetComponent<Image>().color = originalColor;
        buttonText.text = originalText;
    }
    
    void BackToMenu()
    {
        PlayButtonSound();
        SceneManager.LoadScene(0);
    }
    
    void PlayButtonSound()
    {
        if (audioSource && buttonClickSound)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    void TriggerHapticFeedback()
    {
        #if UNITY_IOS
        if (UnityEngine.iOS.Device.generation != UnityEngine.iOS.DeviceGeneration.iPhoneUnknown)
        {
            Handheld.Vibrate();
        }
        #elif UNITY_ANDROID
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
        #endif
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