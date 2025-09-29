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
    public Image currentTeamImageExplanation;
    public Button startButton;
    
    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    
    [Header("Gameplay UI")]
    public Image fossilImage;
    public TextMeshProUGUI fossilNameText;
    public TextMeshProUGUI timerText;
    public Image currentTeamImage;
    public TextMeshProUGUI scoreText;
    
    [Header("Input")]
    public FossilInputHandler fossilInputHandler;
    
    [Header("Result Screen")]
    public Image team1ImageResult;
    public TextMeshProUGUI team1ScoreText;
    public Image team2ImageResult;
    public TextMeshProUGUI team2ScoreText;
    public Image winnerTeamImage;
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
    private int currentTeam = 1;
    private int team1Score = 0;
    private int team2Score = 0;
    
    // NEU: Tracking für korrekte Fossilien pro Runde
    private int correctFossilsThisRound = 0;
    
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
        
        explanationUI.SetActive(true);
        countdownUI.SetActive(false);
        gameplayUI.SetActive(false);
        resultUI.SetActive(false);
    }
    
    void SetupTeamImages()
    {
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
        
        // Reset für neue Runde
        correctFossilsThisRound = 0;
        currentRoundFossils = new List<FossilData>(fossilCollection.GetRandomFossils(fossilCollection.fossilsPerRound));
        currentFossilIndex = 0;
        roundTimeLeft = fossilCollection.roundDuration;
        gameActive = true;
        
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
        // GEÄNDERT: Keine endlose Fossil-Nachladung
        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            // Falls alle Fossilien durch sind, aber noch nicht genug erraten wurden
            // -> Mische die Liste neu und beginne von vorn
            for (int i = currentRoundFossils.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (currentRoundFossils[i], currentRoundFossils[randomIndex]) = (currentRoundFossils[randomIndex], currentRoundFossils[i]);
            }
            currentFossilIndex = 0;
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
        
        // Erhöhe Score für aktuelles Team
        if (currentTeam == 1)
            team1Score++;
        else
            team2Score++;
        
        // WICHTIG: Erhöhe Counter für korrekte Fossilien dieser Runde
        correctFossilsThisRound++;
        
        // GEÄNDERT: Entferne das korrekt erratene Fossil aus der Liste
        currentRoundFossils.RemoveAt(currentFossilIndex);
        
        // Index-Korrektur nach dem Entfernen
        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            currentFossilIndex = 0;
        }
        
        // Prüfe ob die maximale Anzahl erreicht wurde
        if (correctFossilsThisRound >= fossilCollection.fossilsPerRound)
        {
            Debug.Log($"Erreicht! {correctFossilsThisRound}/{fossilCollection.fossilsPerRound} Fossilien erraten.");
            EndRound();
            return;
        }
        
        // Prüfe ob noch Fossilien übrig sind
        if (currentRoundFossils.Count == 0)
        {
            Debug.Log("Alle Fossilien erraten! Runde beendet.");
            EndRound();
            return;
        }
        
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
        
        // GEÄNDERT: Fossil ans Ende der Liste setzen, aber in der Liste behalten
        FossilData skippedFossil = currentRoundFossils[currentFossilIndex];
        currentRoundFossils.RemoveAt(currentFossilIndex);
        currentRoundFossils.Add(skippedFossil); // Ans Ende anhängen
        
        // Index-Korrektur nach dem Verschieben
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
        
        team1ScoreText.text = $"{team1Score} Punkte";
        team2ScoreText.text = $"{team2Score} Punkte";
        
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
        UpdateCurrentTeamImage(currentTeamImage);
        
        // GEÄNDERT: Bessere Anzeige des Fortschritts
        int remainingFossils = currentRoundFossils.Count;
        scoreText.text = $"{correctFossilsThisRound} von {fossilCollection.fossilsPerRound} | Noch {remainingFossils} übrig | {team1Score} : {team2Score}";
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
        correctFossilsThisRound = 0;
        
        resultUI.SetActive(false);
        ShowExplanation();
    }
    
    void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}