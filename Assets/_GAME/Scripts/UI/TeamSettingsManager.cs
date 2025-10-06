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
    
    void Start()
    {
        InitializeSettings();
        SetupUI();
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
    }
    
    void OnTeam1DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 1 difficulty changed to: {newDifficulty}");
    }
    
    void OnTeam2DifficultyChanged(DifficultyLevel newDifficulty)
    {
        PlayButtonSound();
        Debug.Log($"Team 2 difficulty changed to: {newDifficulty}");
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
    }
    
    void ShowAppliedFeedback()
    {
        // Visuelles Feedback - Button kurz grün färben oder ähnliches
        StartCoroutine(AppliedFeedbackCoroutine());
    }
    
    System.Collections.IEnumerator AppliedFeedbackCoroutine()
    {
        Color originalColor = applyButton.GetComponent<Image>().color;
        applyButton.GetComponent<Image>().color = Color.green;
        
        TextMeshProUGUI buttonText = applyButton.GetComponentInChildren<TextMeshProUGUI>();
        string originalText = buttonText.text;
        buttonText.text = "Gespeichert!";
        
        yield return new WaitForSeconds(1f);
        
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
    
    [ContextMenu("Set Mixed Difficulties")]
    void DebugSetMixed()
    {
        team1DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Kids);
        team2DifficultyGroup.SetSelectedDifficulty(DifficultyLevel.Adults);
    }
    
    #endregion
}