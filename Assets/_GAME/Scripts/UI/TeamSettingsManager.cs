using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TeamSettingsManager : MonoBehaviour
{
    [Header("Team 1 UI (Bottom)")]
    public Image team1Icon;
    public TextMeshProUGUI team1Label;
    public DifficultyRadioGroup team1DifficultyGroup;
    
    [Header("Team 2 UI (Top)")]
    public Image team2Icon;
    public TextMeshProUGUI team2Label;
    public DifficultyRadioGroup team2DifficultyGroup;
    
    [Header("Action Buttons")]
    public Button applyButton;
    public Button backButton;
    public Button resetButton;
    
    [Header("Team Icons (Default)")]
    public Sprite defaultTeam1Icon;
    public Sprite defaultTeam2Icon;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip appliedSound; // Separater Sound für "Gespeichert" Feedback
    
    [Header("Mobile UI Settings")]
    [SerializeField] private bool adaptToSafeArea = true; // Für iOS Notch/Safe Area
    
    void Start()
    {
        // Mobile-spezifische Initialisierung
        if (adaptToSafeArea)
        {
            AdaptToSafeArea();
        }
        
        InitializeSettings();
        SetupUI();
    }
    
    /// <summary>
    /// Passt das UI an die Safe Area von mobilen Geräten an
    /// </summary>
    void AdaptToSafeArea()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                // Hole Safe Area (wichtig für iPhone mit Notch)
                Rect safeArea = Screen.safeArea;
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;
                
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;
                
                canvasRect.anchorMin = anchorMin;
                canvasRect.anchorMax = anchorMax;
                
                Debug.Log($"Safe Area angepasst: {safeArea}");
            }
        }
    }
    
    void InitializeSettings()
    {
        // Lade aktuelle Schwierigkeitsgrade vom GameDataManager
        DifficultyLevel team1Difficulty = DifficultyLevel.Adults;
        DifficultyLevel team2Difficulty = DifficultyLevel.Adults;
        
        if (GameDataManager.Instance != null)
        {
            team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(0);
            team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(1);
        }
        
        // Setze UI basierend auf aktuellen Einstellungen
        team1DifficultyGroup.SetSelectedDifficulty(team1Difficulty);
        team2DifficultyGroup.SetSelectedDifficulty(team2Difficulty);
        
        // Setze Team-Icons (Standard oder aus GameDataManager falls verfügbar)
        SetupTeamIcons();
        
        // Setze Team-Labels
        team1Label.text = "Team 1";
        team2Label.text = "Team 2";
        
        Debug.Log($"Settings loaded: Team1={team1Difficulty}, Team2={team2Difficulty}");
    }
    
    void SetupTeamIcons()
    {
        // Verwende Standard-Icons erstmal - später können wir diese dynamisch laden
        if (team1Icon && defaultTeam1Icon)
            team1Icon.sprite = defaultTeam1Icon;
            
        if (team2Icon && defaultTeam2Icon)
            team2Icon.sprite = defaultTeam2Icon;
            
        // Mobile-optimierte Icon-Größen
        if (team1Icon) AdaptIconForMobile(team1Icon);
        if (team2Icon) AdaptIconForMobile(team2Icon);
    }
    
    void AdaptIconForMobile(Image icon)
    {
        // Stelle sicher, dass Icons auf mobilen Geräten gut sichtbar sind
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        if (iconRect != null)
        {
            // Mindestgröße für Touch-Targets auf Mobile (empfohlen: 44pt auf iOS, 48dp auf Android)
            float minSize = 60f; // Unity Units
            Vector2 currentSize = iconRect.sizeDelta;
            
            if (currentSize.x < minSize || currentSize.y < minSize)
            {
                iconRect.sizeDelta = new Vector2(Mathf.Max(currentSize.x, minSize), Mathf.Max(currentSize.y, minSize));
            }
        }
    }
    
    void SetupUI()
    {
        // Setup Button Events
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMenu);
        resetButton.onClick.AddListener(ResetToDefaults);
        
        // Setup Radio Group Events
        team1DifficultyGroup.OnDifficultyChanged += OnTeam1DifficultyChanged;
        team2DifficultyGroup.OnDifficultyChanged += OnTeam2DifficultyChanged;
        
        // Mobile-optimierte Button-Größen
        AdaptButtonsForMobile();
    }
    
    void AdaptButtonsForMobile()
    {
        Button[] buttons = { applyButton, backButton, resetButton };
        
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    // Mindest-Touch-Target-Größe für Mobile
                    Vector2 currentSize = buttonRect.sizeDelta;
                    float minHeight = 50f; // Mindesthöhe für Touch-Targets
                    
                    if (currentSize.y < minHeight)
                    {
                        buttonRect.sizeDelta = new Vector2(currentSize.x, minHeight);
                    }
                }
            }
        }
    }
    
    void OnTeam1DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 1 difficulty changed to: {newDifficulty}");
        
        // Mobile Haptic Feedback (falls gewünscht)
        TriggerHapticFeedback();
    }
    
    void OnTeam2DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 2 difficulty changed to: {newDifficulty}");
        
        // Mobile Haptic Feedback (falls gewünscht)
        TriggerHapticFeedback();
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
    
    void ApplySettings()
    {
        PlayButtonSound();
        
        DifficultyLevel team1Difficulty = team1DifficultyGroup.GetSelectedDifficulty();
        DifficultyLevel team2Difficulty = team2DifficultyGroup.GetSelectedDifficulty();
        
        // Speichere Einstellungen im GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetTeamDifficulties(team1Difficulty, team2Difficulty);
            Debug.Log($"Settings applied: Team1={team1Difficulty}, Team2={team2Difficulty}");
        }
        
        // Feedback für den Nutzer
        ShowAppliedFeedback();
        
        // Haptic Feedback für erfolgreiche Aktion
        TriggerHapticFeedback();
    }
    
    void ShowAppliedFeedback()
    {
        // Visuelles Feedback - Button kurz grün färben
        StartCoroutine(AppliedFeedbackCoroutine());
        
        // Audio Feedback
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
        buttonText.text = "Gespeichert!";
        
        yield return new WaitForSeconds(1.5f); // Etwas länger für Mobile
        
        applyButton.GetComponent<Image>().color = originalColor;
        buttonText.text = originalText;
    }
    
    void ResetToDefaults()
    {
        PlayButtonSound();
        
        // Setze beide Teams auf Adults (Standard)
        team1DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Adults);
        team2DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Adults);
        
        Debug.Log("Settings reset to defaults (Adults)");
        
        // Haptic Feedback
        TriggerHapticFeedback();
    }
    
    void BackToMenu()
    {
        PlayButtonSound();
        
        // Gehe zurück zum Hauptmenü (Szene 0)
        SceneManager.LoadScene(0);
    }
    
    void PlayButtonSound()
    {
        if (audioSource && buttonClickSound)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    #region Mobile Platform Detection
    
    bool IsMobile()
    {
        return Application.platform == RuntimePlatform.Android || 
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
    
    bool IsIOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }
    
    bool IsAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Print Current Settings")]
    void PrintCurrentSettings()
    {
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"Current Settings: Team1={GameDataManager.Instance.GetTeamDifficulty(0)}, " +
                     $"Team2={GameDataManager.Instance.GetTeamDifficulty(1)}");
        }
    }
    
    [ContextMenu("Test Mobile Adaptations")]
    void TestMobileAdaptations()
    {
        AdaptToSafeArea();
        AdaptButtonsForMobile();
        Debug.Log($"Mobile adaptations applied. Platform: {Application.platform}");
    }
    
    [ContextMenu("Set Mixed Difficulties")]
    void DebugSetMixed()
    {
        team1DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Kids);
        team2DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Adults);
    }
    
    #endregion
}