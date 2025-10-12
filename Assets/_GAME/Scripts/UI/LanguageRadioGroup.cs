using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Radio-Button-System für Sprachauswahl
/// Basiert auf DifficultyRadioGroup aber für Language-Enum
/// </summary>
public class LanguageRadioGroup : MonoBehaviour
{
    [Header("Language Buttons")]
    [SerializeField] private Button germanStandardButton;
    [SerializeField] private Button englishStandardButton;
    [SerializeField] private Button germanSimpleButton;
    [SerializeField] private Button englishSimpleButton;
    
    [Header("Button Sprites")]
    [SerializeField] private Sprite unselectedSprite;
    [SerializeField] private Sprite selectedSprite;
    
    [Header("Language Icons (Optional)")]
    [SerializeField] private Sprite germanIcon;
    [SerializeField] private Sprite englishIcon;
    [SerializeField] private Sprite simpleIcon;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectionSound;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;
    
    private Language selectedLanguage = Language.German_Standard;
    private Button[] allButtons;
    
    public event Action<Language> OnLanguageChanged;
    
    void Awake()
    {
        allButtons = new Button[] 
        { 
            germanStandardButton, 
            englishStandardButton, 
            germanSimpleButton, 
            englishSimpleButton 
        };
        
        SetupButtons();
    }
    
    void SetupButtons()
    {
        // Event-Listeners hinzufügen
        if (germanStandardButton) germanStandardButton.onClick.AddListener(() => SelectLanguage(Language.German_Standard));
        if (englishStandardButton) englishStandardButton.onClick.AddListener(() => SelectLanguage(Language.English_Standard));
        if (germanSimpleButton) germanSimpleButton.onClick.AddListener(() => SelectLanguage(Language.German_Simple));
        if (englishSimpleButton) englishSimpleButton.onClick.AddListener(() => SelectLanguage(Language.English_Simple));
        
        // Mobile-optimierte Touch-Targets
        AdaptButtonsForMobile();
        
        // Initial Selection
        UpdateButtonVisuals();
    }
    
    void AdaptButtonsForMobile()
    {
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    // Mindest-Touch-Target-Größe für Mobile
                    Vector2 currentSize = buttonRect.sizeDelta;
                    float minSize = 60f; // Mindestgröße für Touch-Targets
                    
                    if (currentSize.x < minSize || currentSize.y < minSize)
                    {
                        buttonRect.sizeDelta = new Vector2(
                            Mathf.Max(currentSize.x, minSize), 
                            Mathf.Max(currentSize.y, minSize)
                        );
                    }
                }
            }
        }
    }
    
    public void SelectLanguage(Language language)
    {
        if (selectedLanguage != language)
        {
            selectedLanguage = language;
            UpdateButtonVisuals();
            OnLanguageChanged?.Invoke(selectedLanguage);
            
            // Audio Feedback
            PlaySelectionSound();
            
            // Haptic Feedback für Mobile
            TriggerHapticFeedback();
            
            if (enableDebugLogs)
                Debug.Log($"Language selected: {language}");
        }
    }
    
    public void SetSelectedLanguage(Language language)
    {
        selectedLanguage = language;
        UpdateButtonVisuals();
    }
    
    public Language GetSelectedLanguage()
    {
        return selectedLanguage;
    }
    
    void UpdateButtonVisuals()
    {
        UpdateButtonVisual(germanStandardButton, selectedLanguage == Language.German_Standard);
        UpdateButtonVisual(englishStandardButton, selectedLanguage == Language.English_Standard);
        UpdateButtonVisual(germanSimpleButton, selectedLanguage == Language.German_Simple);
        UpdateButtonVisual(englishSimpleButton, selectedLanguage == Language.English_Simple);
    }
    
    void UpdateButtonVisual(Button button, bool isSelected)
    {
        if (button == null) return;
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null && unselectedSprite != null && selectedSprite != null)
        {
            buttonImage.sprite = isSelected ? selectedSprite : unselectedSprite;
        }
        
        // Optional: Color-based feedback als Fallback
        if (buttonImage != null && (unselectedSprite == null || selectedSprite == null))
        {
            buttonImage.color = isSelected ? Color.green : Color.white;
        }
    }
    
    void PlaySelectionSound()
    {
        if (audioSource && selectionSound)
        {
            audioSource.PlayOneShot(selectionSound);
        }
    }
    
    void TriggerHapticFeedback()
    {
        // Haptic Feedback für iOS/Android
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
    
    #region Public Helper Methods
    
    /// <summary>
    /// Aktiviert/Deaktiviert bestimmte Sprachen (z.B. wenn nicht alle verfügbar sind)
    /// </summary>
    public void SetLanguageAvailability(Language language, bool available)
    {
        Button targetButton = language switch
        {
            Language.German_Standard => germanStandardButton,
            Language.English_Standard => englishStandardButton,
            Language.German_Simple => germanSimpleButton,
            Language.English_Simple => englishSimpleButton,
            _ => null
        };
        
        if (targetButton != null)
        {
            targetButton.interactable = available;
            
            // Visuelle Anpassung für deaktivierte Buttons
            Image buttonImage = targetButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = available ? Color.white : Color.gray;
            }
        }
    }
    
    /// <summary>
    /// Alle verfügbaren Sprachen aktivieren
    /// </summary>
    public void EnableAllLanguages()
    {
        foreach (Language lang in System.Enum.GetValues(typeof(Language)))
        {
            SetLanguageAvailability(lang, true);
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Test All Languages")]
    void TestAllLanguages()
    {
        foreach (Language lang in System.Enum.GetValues(typeof(Language)))
        {
            Debug.Log($"Testing language: {lang}");
            SelectLanguage(lang);
        }
    }
    
    [ContextMenu("Set to German")]
    void DebugSetGerman()
    {
        SelectLanguage(Language.German_Standard);
    }
    
    [ContextMenu("Set to English")]
    void DebugSetEnglish()
    {
        SelectLanguage(Language.English_Standard);
    }
    
    #endregion
}