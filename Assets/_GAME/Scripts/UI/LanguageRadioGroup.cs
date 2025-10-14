using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    [SerializeField] private bool enableDebugLogs = true; // Auf true gesetzt für Debugging
    
    private Language selectedLanguage = Language.German_Standard;
    private Button[] allButtons;
    
    public event Action<Language> OnLanguageChanged;
    
    void Start()
    {
        // Button-Array initialisieren
        allButtons = new Button[] 
        { 
            germanStandardButton, 
            englishStandardButton, 
            germanSimpleButton, 
            englishSimpleButton 
        };
        
        // Validierung
        ValidateButtonReferences();
        
        // Setup durchführen
        SetupButtons();
        
        if (enableDebugLogs)
            Debug.Log("LanguageRadioGroup initialized in Start()");
    }
    
    void ValidateButtonReferences()
    {
        if (germanStandardButton == null)
            Debug.LogError("LanguageRadioGroup: German Standard Button nicht zugewiesen!");
        if (englishStandardButton == null)
            Debug.LogError("LanguageRadioGroup: English Standard Button nicht zugewiesen!");
        if (germanSimpleButton == null)
            Debug.LogError("LanguageRadioGroup: German Simple Button nicht zugewiesen!");
        if (englishSimpleButton == null)
            Debug.LogError("LanguageRadioGroup: English Simple Button nicht zugewiesen!");
            
        if (unselectedSprite == null)
            Debug.LogWarning("LanguageRadioGroup: Unselected Sprite nicht zugewiesen - verwende Color-Fallback");
        if (selectedSprite == null)
            Debug.LogWarning("LanguageRadioGroup: Selected Sprite nicht zugewiesen - verwende Color-Fallback");
    }
    
    void SetupButtons()
    {
        // Event-Listeners hinzufügen mit Fehlerprüfung
        if (germanStandardButton != null)
        {
            germanStandardButton.onClick.AddListener(() => SelectLanguage(Language.German_Standard));
            if (enableDebugLogs)
                Debug.Log("German Standard Button onClick event registered");
        }
        
        if (englishStandardButton != null)
        {
            englishStandardButton.onClick.AddListener(() => SelectLanguage(Language.English_Standard));
            if (enableDebugLogs)
                Debug.Log("English Standard Button onClick event registered");
        }
        
        if (germanSimpleButton != null)
        {
            germanSimpleButton.onClick.AddListener(() => SelectLanguage(Language.German_Simple));
            if (enableDebugLogs)
                Debug.Log("German Simple Button onClick event registered");
        }
        
        if (englishSimpleButton != null)
        {
            englishSimpleButton.onClick.AddListener(() => SelectLanguage(Language.English_Simple));
            if (enableDebugLogs)
                Debug.Log("English Simple Button onClick event registered");
        }
        
        // Mobile-optimierte Touch-Targets
        AdaptButtonsForMobile();
        
        // Initial Selection
        UpdateButtonVisuals();
        
        if (enableDebugLogs)
            Debug.Log($"LanguageRadioGroup setup complete. Initial language: {selectedLanguage}");
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
        if (enableDebugLogs)
            Debug.Log($"SelectLanguage called with: {language}");
        
        if (selectedLanguage != language)
        {
            selectedLanguage = language;
            UpdateButtonVisuals();
            
            // Event auslösen
            if (OnLanguageChanged != null)
            {
                OnLanguageChanged.Invoke(selectedLanguage);
                if (enableDebugLogs)
                    Debug.Log($"OnLanguageChanged event invoked with: {selectedLanguage}");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning("OnLanguageChanged event has no subscribers!");
            }
            
            // Audio Feedback
            PlaySelectionSound();
            
            // Haptic Feedback für Mobile
            TriggerHapticFeedback();
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log($"Language {language} was already selected - no change");
        }
    }
    
    public void SetSelectedLanguage(Language language)
    {
        selectedLanguage = language;
        UpdateButtonVisuals();
        
        if (enableDebugLogs)
            Debug.Log($"SetSelectedLanguage (without event): {language}");
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
    
    [ContextMenu("Debug Button Setup")]
    void DebugButtonSetup()
    {
        Debug.Log("=== LanguageRadioGroup Debug Info ===");
        Debug.Log($"German Standard Button: {(germanStandardButton != null ? "OK" : "MISSING")}");
        Debug.Log($"English Standard Button: {(englishStandardButton != null ? "OK" : "MISSING")}");
        Debug.Log($"German Simple Button: {(germanSimpleButton != null ? "OK" : "MISSING")}");
        Debug.Log($"English Simple Button: {(englishSimpleButton != null ? "OK" : "MISSING")}");
        Debug.Log($"OnLanguageChanged subscribers: {(OnLanguageChanged != null ? OnLanguageChanged.GetInvocationList().Length : 0)}");
        Debug.Log($"Current selected language: {selectedLanguage}");
    }
    
    [ContextMenu("Test Button Interaction")]
    void TestButtonInteraction()
    {
        Debug.Log("=== Button Interaction Test ===");
        
        Button[] buttons = { germanStandardButton, englishStandardButton, germanSimpleButton, englishSimpleButton };
        string[] names = { "German Standard", "English Standard", "German Simple", "English Simple" };
        
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                Debug.Log($"{names[i]}:");
                Debug.Log($"  - Interactable: {buttons[i].interactable}");
                Debug.Log($"  - onClick listeners: {buttons[i].onClick.GetPersistentEventCount()}");
                
                // Prüfe Raycast Target
                Image img = buttons[i].GetComponent<Image>();
                if (img != null)
                    Debug.Log($"  - Raycast Target: {img.raycastTarget}");
                
                // Prüfe CanvasGroup
                CanvasGroup cg = buttons[i].GetComponent<CanvasGroup>();
                if (cg != null)
                    Debug.Log($"  - CanvasGroup blocks raycasts: {cg.blocksRaycasts}, interactable: {cg.interactable}");
            }
        }
        
        // Prüfe EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        Debug.Log($"EventSystem present: {eventSystem != null}");
        
        // Prüfe Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Canvas found: {canvas.name}");
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            Debug.Log($"GraphicRaycaster present: {raycaster != null}");
        }
    }
    
    #endregion
}