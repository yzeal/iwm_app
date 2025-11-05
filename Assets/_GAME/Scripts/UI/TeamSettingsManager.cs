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
    public TeamIconRadioGroup team1IconGroup; // NEU
    
    [Header("Team 2 UI (Top)")]
    public Image team2Icon;
    public TextMeshProUGUI team2Label;
    public DifficultyRadioGroup team2DifficultyGroup;
    public TeamIconRadioGroup team2IconGroup; // NEU
    
    [Header("Action Buttons")]
    public Button applyButton;
    public Button backButton;
    
    [Header("Team Icons (Default) - DEPRECATED")]
    public Sprite defaultTeam1Icon;
    public Sprite defaultTeam2Icon;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip appliedSound;
    
    [Header("Mobile UI Settings")]
    [SerializeField] private bool adaptToSafeArea = true;
    
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
        int team1IconIndex = 0;
        int team2IconIndex = 0;
        
        if (GameDataManager.Instance != null)
        {
            team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(0);
            team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(1);
            team1IconIndex = GameDataManager.Instance.GetTeamIconIndex(0);
            team2IconIndex = GameDataManager.Instance.GetTeamIconIndex(1);
        }
        
        // Setze UI basierend auf aktuellen Einstellungen
        team1DifficultyGroup.SetSelectedDifficulty(team1Difficulty);
        team2DifficultyGroup.SetSelectedDifficulty(team2Difficulty);
        
        // NEU: Setze Icon-Auswahl
        if (team1IconGroup != null)
            team1IconGroup.SetSelectedIcon(team1IconIndex);
        if (team2IconGroup != null)
            team2IconGroup.SetSelectedIcon(team2IconIndex);
        
        // Setze Team-Labels
        team1Label.text = "Team 1";
        team2Label.text = "Team 2";
        
        Debug.Log($"Settings loaded: Team1={team1Difficulty} (Icon {team1IconIndex}), Team2={team2Difficulty} (Icon {team2IconIndex})");
    }
    
    void SetupUI()
    {
        // Setup Button Events
        applyButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(BackToMenu);
        
        // Setup Radio Group Events
        team1DifficultyGroup.OnDifficultyChanged += OnTeam1DifficultyChanged;
        team2DifficultyGroup.OnDifficultyChanged += OnTeam2DifficultyChanged;
        
        // NEU: Setup Icon Group Events
        if (team1IconGroup != null)
            team1IconGroup.OnIconChanged += OnTeam1IconChanged;
        if (team2IconGroup != null)
            team2IconGroup.OnIconChanged += OnTeam2IconChanged;
        
        // Mobile-optimierte Button-Größen
        AdaptButtonsForMobile();
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
                    // Mindest-Touch-Target-Größe für Mobile
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
    
    void OnTeam1DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 1 difficulty changed to: {newDifficulty}");
        TriggerHapticFeedback();
    }
    
    void OnTeam2DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 2 difficulty changed to: {newDifficulty}");
        TriggerHapticFeedback();
    }
    
    // NEU: Icon-Change-Handler
    void OnTeam1IconChanged(int newIconIndex)
    {
        PlayButtonSound();
        Debug.Log($"Team 1 icon changed to: {newIconIndex}");
        TriggerHapticFeedback();
    }
    
    void OnTeam2IconChanged(int newIconIndex)
    {
        PlayButtonSound();
        Debug.Log($"Team 2 icon changed to: {newIconIndex}");
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
        
        // NEU: Icon-Indizes holen
        int team1IconIndex = team1IconGroup != null ? team1IconGroup.GetSelectedIconIndex() : 0;
        int team2IconIndex = team2IconGroup != null ? team2IconGroup.GetSelectedIconIndex() : 0;
        
        // Speichere Einstellungen im GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SetTeamDifficulties(team1Difficulty, team2Difficulty);
            GameDataManager.Instance.SetTeamIconIndices(team1IconIndex, team2IconIndex);
            Debug.Log($"Settings applied: Team1={team1Difficulty} (Icon {team1IconIndex}), Team2={team2Difficulty} (Icon {team2IconIndex})");
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
        
        yield return new WaitForSeconds(1.5f);
        
        applyButton.GetComponent<Image>().color = originalColor;
        buttonText.text = originalText;
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
            Debug.Log($"Current Settings: Team1={GameDataManager.Instance.GetTeamDifficulty(0)} (Icon {GameDataManager.Instance.GetTeamIconIndex(0)}), " +
                     $"Team2={GameDataManager.Instance.GetTeamDifficulty(1)} (Icon {GameDataManager.Instance.GetTeamIconIndex(1)})");
        }
    }
    
    [ContextMenu("Test Mobile Adaptations")]
    void TestMobileAdaptations()
    {
        AdaptToSafeArea();
        AdaptButtonsForMobile();
        Debug.Log($"Mobile adaptations applied. Platform: {Application.platform}");
    }
    
    #endregion
}