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
    public Image currentTeamImageExplanation; // NEU: Team-Bild im Explanation Screen
    public Button startButton;
    
    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    
    [Header("Gameplay UI")]
    public Image fossilImage;
    public TextMeshProUGUI fossilNameText;
    public TextMeshProUGUI timerText;
    public Image currentTeamImage; // NEU: Ersetzt currentTeamText
    public TextMeshProUGUI scoreText;
    
    [Header("Input")]
    public FossilInputHandler fossilInputHandler;
    
    [Header("Result Screen")]
    public Image team1ImageResult; // NEU: Team 1 Bild
    public TextMeshProUGUI team1ScoreText;
    public Image team2ImageResult; // NEU: Team 2 Bild
    public TextMeshProUGUI team2ScoreText;
    public Image winnerTeamImage; // NEU: Gewinner Team Bild
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
        
        // Validiere Team-Images
        if (fossilCollection.team1Image == null || fossilCollection.team2Image == null)
        {
            Debug.LogWarning("Team images not assigned in FossilCollection!");
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
        SetupTeamImages();
        
        // Hide all UIs initially
        explanationUI.SetActive(true);
        countdownUI.SetActive(false);
        gameplayUI.SetActive(false);
        resultUI.SetActive(false);
    }
    
    void SetupTeamImages()
    {
        // Setze Team-Bilder in Result Screen (statisch)
        if (team1ImageResult && fossilCollection.team1Image)
            team1ImageResult.sprite = fossilCollection.team1Image;
            
        if (team2ImageResult && fossilCollection.team2Image)
            team2ImageResult.sprite = fossilCollection.team2Image;
    }
    
    void SetupInputHandlers()
    {
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
                              $"<b>Begriffe pro Runde:</b> {fossilCollection.fossilsPerRound}\n\n" +
                              $"Dieses Team ist jetzt dran:";
        
        // Aktualisiere Team-Bild im Explanation Screen
        UpdateCurrentTeamImage(currentTeamImageExplanation);
    }
    
    void UpdateCurrentTeamImage(Image targetImage)
    {
        if (targetImage == null) return;
        
        if (currentTeam == 1 && fossilCollection.team1Image)
        {
            targetImage.sprite = fossilCollection.team1Image;
        }
        else if (currentTeam == 2 && fossilCollection.team2Image)
        {
            targetImage.sprite = fossilCollection.team2Image;
        }
        
        targetImage.gameObject.SetActive(true);
    }
    
    void StartCountdown()
    {
        explanationUI.SetActive(false);
        countdownUI.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }
    
    IEnumerator CountdownCoroutine()
    {
        string[] countdownTexts = { "3", "2", "1", "!" };
        
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
        
        // Score-Texte (nur Zahlen, da Bilder separat angezeigt werden)
        team1ScoreText.text = $"{team1Score} Punkte";
        team2ScoreText.text = $"{team2Score} Punkte";
        
        // Gewinner-Logic
        if (team1Score > team2Score)
        {
            winnerText.text = "Gewinner!";
            if (winnerTeamImage && fossilCollection.team1Image)
            {
                winnerTeamImage.sprite = fossilCollection.team1Image;
                winnerTeamImage.gameObject.SetActive(true);
            }
        }
        else if (team2Score > team1Score)
        {
            winnerText.text = "Gewinner!";
            if (winnerTeamImage && fossilCollection.team2Image)
            {
                winnerTeamImage.sprite = fossilCollection.team2Image;
                winnerTeamImage.gameObject.SetActive(true);
            }
        }
        else
        {
            winnerText.text = "Unentschieden!";
            if (winnerTeamImage)
            {
                winnerTeamImage.gameObject.SetActive(false);
            }
        }
        
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
        // Aktualisiere aktuelles Team-Bild
        UpdateCurrentTeamImage(currentTeamImage);
        
        // Score-Text bleibt, aber ohne "Team 1"/"Team 2" Präfix
        scoreText.text = $"{team1Score} : {team2Score}";
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