using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FossilGameManager : MonoBehaviour
{
    [Header("Game Data")]
    public FossilCollection fossilCollection;
    
    [Header("UI References")]
    public GameObject explanationUI;
    public GameObject countdownUI;
    public GameObject gameplayUI;
    public GameObject resultUI;
    
    [Header("Explanation Screen")]
    public TextMeshProUGUI explanationText;
    public Button startButton;
    
    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    
    [Header("Gameplay UI")]
    public Image fossilImage;
    public TextMeshProUGUI fossilNameText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI currentTeamText;
    public TextMeshProUGUI scoreText;
    
    [Header("Input")]
    public FossilInputHandler fossilInputHandler;
    
    [Header("Result Screen")]
    public TextMeshProUGUI team1ScoreText;
    public TextMeshProUGUI team2ScoreText;
    public TextMeshProUGUI winnerText;
    public Button playAgainButton;
    public Button backToMenuButton;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip skipSound;
    public AudioClip timeUpSound;
    public AudioClip countdownSound;
    
    // Game State
    private List<FossilData> currentRoundFossils;
    private int currentFossilIndex = 0;
    private float roundTimeLeft;
    private bool gameActive = false;
    private int currentTeam = 1; // 1 oder 2
    private int team1Score = 0;
    private int team2Score = 0;
    
    void Start()
    {
        InitializeGame();
    }
    
    void InitializeGame()
    {
        if (fossilCollection == null || fossilCollection.fossils.Length == 0)
        {
            Debug.LogError("No fossil collection assigned!");
            return;
        }
        
        SetupUI();
        ShowExplanation();
    }
    
    void SetupUI()
    {
        startButton.onClick.AddListener(StartCountdown);
        playAgainButton.onClick.AddListener(RestartGame);
        backToMenuButton.onClick.AddListener(BackToMenu);
        
        SetupInputHandlers();
        
        // Hide all UIs initially
        explanationUI.SetActive(true);
        countdownUI.SetActive(false);
        gameplayUI.SetActive(false);
        resultUI.SetActive(false);
    }
    
    void SetupInputHandlers()
    {
        // Nur noch ein Input Handler
        fossilInputHandler.OnCorrectInput += OnCorrectFossil;
        fossilInputHandler.OnSkipInput += OnSkipFossil;
    }
    
    void ShowExplanation()
    {
        string inputInfo = fossilInputHandler.GetInputModeInfo();
        
        explanationText.text = $"<size=28><b>Fossilien-Stirnraten</b></size>\n\n" +
                              $"<b>Wie gespielt wird:</b>\n" +
                              $"• Halte das Handy an deine Stirn\n" +
                              $"• Dein Team erklärt dir das Fossil\n\n" +
                              $"<b>Steuerung:</b>\n" +
                              $"{inputInfo}\n\n" +
                              $"<b>Zeit pro Runde:</b> {fossilCollection.roundDuration} Sekunden\n" +
                              $"<b>Begriffe pro Runde:</b> {fossilCollection.fossilsPerRound}\n\n" +  // <- KORRIGIERT: fossilsPerRound statt fossilsPerRund
                              $"Team {currentTeam} ist als erstes dran!";
    }
    
    void StartCountdown()
    {
        explanationUI.SetActive(false);
        countdownUI.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }
    
    IEnumerator CountdownCoroutine()
    {
        string[] countdownTexts = { "3", "2", "1", "LOS!" };
        
        for (int i = 0; i < countdownTexts.Length; i++)
        {
            countdownText.text = countdownTexts[i];
            
            if (audioSource && countdownSound)
            {
                audioSource.PlayOneShot(countdownSound);
            }
            
            yield return new WaitForSeconds(1f);
        }
        
        StartRound();
    }
    
    void StartRound()
    {
        countdownUI.SetActive(false);
        gameplayUI.SetActive(true);
        
        // Bereite Fossilien für diese Runde vor
        currentRoundFossils = new List<FossilData>(fossilCollection.GetRandomFossils(fossilCollection.fossilsPerRound));
        currentFossilIndex = 0;
        roundTimeLeft = fossilCollection.roundDuration;
        gameActive = true;
        
        // Enable Input
        fossilInputHandler.SetInputEnabled(true);
        fossilInputHandler.CalibrateDevice();
        
        ShowCurrentFossil();
        UpdateUI();
    }
    
    void Update()
    {
        if (!gameActive) return;
        
        roundTimeLeft -= Time.deltaTime;
        
        if (roundTimeLeft <= 0)
        {
            EndRound();
        }
        
        UpdateTimerDisplay();
    }
    
    void ShowCurrentFossil()
    {
        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            // Alle Fossilien durch - neue zufällige Auswahl
            currentRoundFossils.AddRange(fossilCollection.GetRandomFossils(fossilCollection.fossilsPerRound));
        }
        
        FossilData fossil = currentRoundFossils[currentFossilIndex];
        fossilImage.sprite = fossil.fossilImage;
        fossilNameText.text = fossil.fossilName;
    }
    
    void OnCorrectFossil()
    {
        if (!gameActive) return;
        
        if (audioSource && correctSound)
        {
            audioSource.PlayOneShot(correctSound);
        }
        
        // Punkt für aktuelles Team
        if (currentTeam == 1)
            team1Score++;
        else
            team2Score++;
            
        // Nächstes Fossil
        currentFossilIndex++;
        ShowCurrentFossil();
        UpdateUI();
    }
    
    void OnSkipFossil()
    {
        if (!gameActive) return;
        
        if (audioSource && skipSound)
        {
            audioSource.PlayOneShot(skipSound);
        }
        
        // Fossil ans Ende der Liste setzen
        FossilData skippedFossil = currentRoundFossils[currentFossilIndex];
        currentRoundFossils.RemoveAt(currentFossilIndex);
        currentRoundFossils.Add(skippedFossil);
        
        // Falls wir am Ende der Liste waren, Index korrigieren
        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            currentFossilIndex = 0;
        }
        
        ShowCurrentFossil();
        UpdateUI();
    }
    
    void EndRound()
    {
        gameActive = false;
        fossilInputHandler.SetInputEnabled(false);
        
        if (audioSource && timeUpSound)
        {
            audioSource.PlayOneShot(timeUpSound);
        }
        
        // Wechsel zum anderen Team oder zeige Ergebnisse
        if (currentTeam == 1)
        {
            currentTeam = 2;
            ShowExplanation();
            explanationUI.SetActive(true);
            gameplayUI.SetActive(false);
        }
        else
        {
            ShowResults();
        }
    }
    
    void ShowResults()
    {
        gameplayUI.SetActive(false);
        resultUI.SetActive(true);
        
        team1ScoreText.text = $"Team 1: {team1Score} Punkte";
        team2ScoreText.text = $"Team 2: {team2Score} Punkte";
        
        if (team1Score > team2Score)
            winnerText.text = "Team 1 gewinnt!";
        else if (team2Score > team1Score)
            winnerText.text = "Team 2 gewinnt!";
        else
            winnerText.text = "Unentschieden!";
            
        // Speichere Ergebnisse
        SaveResults();
    }
    
    void SaveResults()
    {
        if (GameDataManager.Instance != null && fossilCollection != null)
        {
            GameDataManager.Instance.SaveRoomResult(
                fossilCollection.collectionName,
                fossilCollection.roomNumber,
                team1Score,
                team2Score,
                fossilCollection.fossilsPerRound
            );
        }
    }
    
    void UpdateUI()
    {
        currentTeamText.text = $"Team {currentTeam}";
        scoreText.text = $"Team 1: {team1Score} | Team 2: {team2Score}";
    }
    
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(roundTimeLeft / 60);
        int seconds = Mathf.FloorToInt(roundTimeLeft % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
    
    void RestartGame()
    {
        team1Score = 0;
        team2Score = 0;
        currentTeam = 1;
        
        resultUI.SetActive(false);
        ShowExplanation();
    }
    
    void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}