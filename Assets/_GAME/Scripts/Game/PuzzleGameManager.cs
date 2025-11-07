using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the Picture Puzzle minigame.
/// Players find exhibits in the room based on image excerpts.
/// Teams play alternating rounds with hint system and scoring.
/// </summary>
public class PuzzleGameManager : MonoBehaviour
{
    [Header("Puzzle Collection")]
    [SerializeField] private PuzzleCollection puzzleCollection;
    [SerializeField] private string nextScene = "Auswahl";

    [Header("UI Screens")]
    [SerializeField] private GameObject explanationScreen;
    [SerializeField] private GameObject countdownScreen;
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private GameObject solutionScreen;
    [SerializeField] private GameObject resultsScreen;
    
    [Header("Explanation Screen UI")]
    [SerializeField] private Image explanationTeamImage;
    [SerializeField] private TextMeshProUGUI explanationTitleText;
    [SerializeField] private TextMeshProUGUI explanationRulesText;
    [SerializeField] private Button startButton;
    
    [Header("Countdown Screen UI")]
    [SerializeField] private Image countdownTeamImage;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    [Header("Gameplay Screen UI")]
    [SerializeField] private Image gameplayTeamImage;
    [SerializeField] private TextMeshProUGUI teamScoreText;
    [SerializeField] private TextMeshProUGUI roundCounterText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerContainer;
    [SerializeField] private Image puzzleImage;
    [SerializeField] private TextMeshProUGUI possiblePointsText;
    [SerializeField] private Button foundButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintButtonText;
    [SerializeField] private Button giveUpButton;
    
    [Header("Solution Screen UI")]
    [SerializeField] private Image solutionImage;
    [SerializeField] private TextMeshProUGUI exhibitNameText;
    [SerializeField] private TextMeshProUGUI exhibitDescriptionText;
    [SerializeField] private TextMeshProUGUI earnedPointsText;
    [SerializeField] private Button nextTeamButton;
    
    [Header("Results Screen UI")]
    [SerializeField] private Image team1ResultImage;
    [SerializeField] private TextMeshProUGUI team1ScoreText;
    [SerializeField] private Image team2ResultImage;
    [SerializeField] private TextMeshProUGUI team2ScoreText;
    [SerializeField] private Image winnerTeamImage;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button backButton;
    
    [Header("Popup UI")]
    [SerializeField] private GameObject giveUpPopup;
    [SerializeField] private TextMeshProUGUI giveUpPopupText;
    [SerializeField] private Button giveUpConfirmButton;
    [SerializeField] private Button giveUpCancelButton;
    
    [SerializeField] private GameObject timeUpPopup;
    [SerializeField] private TextMeshProUGUI timeUpPopupText;
    [SerializeField] private Button timeUpContinueButton;
    
    [Header("Team Icon Provider (NEU)")]
    [SerializeField] private TeamIconProvider teamIconProvider;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownSound1;
    [SerializeField] private AudioClip countdownSound2;
    [SerializeField] private AudioClip countdownSound3;
    [SerializeField] private AudioClip timerWarningSound;
    [SerializeField] private AudioClip foundSound;
    [SerializeField] private AudioClip hintSound;
    [SerializeField] private AudioClip giveUpSound;
    
    [Header("Localized Texts (21 assets needed)")]
    [SerializeField] private LocalizedText explanationTitleLocalizedText;
    [SerializeField] private LocalizedText explanationRulesLocalizedText;
    [SerializeField] private LocalizedText startButtonLocalizedText;
    [SerializeField] private LocalizedText roundCounterLocalizedText;
    [SerializeField] private LocalizedText timerLabelLocalizedText;
    [SerializeField] private LocalizedText possiblePointsLocalizedText;
    [SerializeField] private LocalizedText foundButtonLocalizedText;
    [SerializeField] private LocalizedText hintButtonLocalizedText;
    [SerializeField] private LocalizedText hintButtonLabelLocalizedText;
    [SerializeField] private LocalizedText allHintsUsedLocalizedText;
    [SerializeField] private LocalizedText giveUpButtonLocalizedText;
    [SerializeField] private LocalizedText giveUpPopupLocalizedText;
    [SerializeField] private LocalizedText timeUpPopupLocalizedText;
    [SerializeField] private LocalizedText earnedPointsLocalizedText;
    [SerializeField] private LocalizedText nextTeamButtonLocalizedText;
    [SerializeField] private LocalizedText resultsWinnerLocalizedText;
    [SerializeField] private LocalizedText resultsTieLocalizedText;
    [SerializeField] private LocalizedText backButtonLocalizedText;
    [SerializeField] private LocalizedText giveUpConfirmButtonLocalizedText;
    [SerializeField] private LocalizedText giveUpCancelButtonLocalizedText;
    [SerializeField] private LocalizedText timeUpContinueButtonLocalizedText;
    
    // ============================================
    // PRIVATE VARIABLES
    // ============================================
    
    private enum GameState
    {
        Explanation,
        Countdown,
        Gameplay,
        Solution,
        Results
    }
    
    private GameState currentState;
    private int currentTeam = 1; // 1 or 2
    private int currentRound = 0;
    private int team1Score = 0;
    private int team2Score = 0;
    
    private List<PuzzlePiece> team1Puzzles;
    private List<PuzzlePiece> team2Puzzles;
    private PuzzlePiece currentPuzzle;
    
    private int currentHintLevel = 0;
    private int hintsUsed = 0;
    private int currentPossiblePoints;
    
    private float roundTimer;
    private float roundDuration;
    private bool useTimer;
    private bool timerWarningPlayed;
    
    private DifficultyLevel team1Difficulty;
    private DifficultyLevel team2Difficulty;
    
    private LanguageSystem.Language currentLanguage;
    
    // ============================================
    // UNITY LIFECYCLE
    // ============================================
    
    private void Start()
    {
        LanguageSystem.OnLanguageChanged += HandleLanguageChanged;
        
        currentLanguage = LanguageSystem.Instance.GetCurrentLanguage();
        
        if (GameDataManager.Instance != null)
        {
            team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(0); // NEU: 0-basiert (war 1)
            team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(1); // NEU: 0-basiert (war 2)
        }
        else
        {
            team1Difficulty = DifficultyLevel.Kids;
            team2Difficulty = DifficultyLevel.Kids;
            Debug.LogWarning("GameDataManager not found! Using default difficulty.");
        }
        
        SetupButtonListeners();
        InitializeGame();
        
        // NEU: Team-Icons laden
        UpdateTeamIcons();
        
        ShowExplanationScreen();
    }
    
    private void OnDestroy()
    {
        LanguageSystem.OnLanguageChanged -= HandleLanguageChanged;
    }
    
    private void Update()
    {
        if (currentState == GameState.Gameplay && useTimer && roundTimer > 0f)
        {
            roundTimer -= Time.deltaTime;
            
            UpdateTimerUI();
            
            if (!timerWarningPlayed && roundTimer <= 3f && roundTimer > 0f)
            {
                PlayTimerWarning();
                timerWarningPlayed = true;
            }
            
            if (roundTimer <= 0f)
            {
                roundTimer = 0f;
                OnTimeUp();
            }
        }
    }
    
    // ============================================
    // INITIALIZATION
    // ============================================
    
    private void InitializeGame()
    {
        if (puzzleCollection == null)
        {
            Debug.LogError("PuzzleCollection not assigned!");
            return;
        }
        
        // DEPRECATED: Legacy team images - jetzt über TeamIconProvider
        if (puzzleCollection.team1Image != null || puzzleCollection.team2Image != null)
        {
            Debug.LogWarning("PuzzleCollection team images sind DEPRECATED. Verwende TeamIconProvider stattdessen!");
        }
        
        team1Score = 0;
        team2Score = 0;
        currentTeam = 1;
        currentRound = 0;
        
        LoadPuzzlesForTeams();
        HideAllScreens();
        
        Debug.Log($"Puzzle Game initialized. Team 1: {team1Difficulty}, Team 2: {team2Difficulty}");
    }
    
    private void LoadPuzzlesForTeams()
    {
        int roundsPerTeam = puzzleCollection.roundsPerTeam;
        
        team1Puzzles = puzzleCollection.GetRandomPuzzles(roundsPerTeam, team1Difficulty);
        team2Puzzles = puzzleCollection.GetRandomPuzzlesExcluding(roundsPerTeam, team2Difficulty, team1Puzzles);
        
        Debug.Log($"Loaded {team1Puzzles.Count} puzzles for Team 1, {team2Puzzles.Count} puzzles for Team 2");
    }
    
    // NEU: Team-Icons aktualisieren
    private void UpdateTeamIcons()
    {
        if (teamIconProvider == null)
        {
            Debug.LogWarning("TeamIconProvider nicht zugewiesen! Verwende Fallback von PuzzleCollection.");
            SetupTeamIconsLegacy();
            return;
        }
        
        // Explanation Screen Icon
        UpdateTeamIconForImage(explanationTeamImage, currentTeam);
        
        // Countdown Screen Icon
        UpdateTeamIconForImage(countdownTeamImage, currentTeam);
        
        // Gameplay Icon
        UpdateTeamIconForImage(gameplayTeamImage, currentTeam);
        
        // Results Screen Icons
        if (team1ResultImage != null)
        {
            Sprite team1Icon = teamIconProvider.GetTeam1Icon();
            if (team1Icon != null)
                team1ResultImage.sprite = team1Icon;
        }
        
        if (team2ResultImage != null)
        {
            Sprite team2Icon = teamIconProvider.GetTeam2Icon();
            if (team2Icon != null)
                team2ResultImage.sprite = team2Icon;
        }
        
        Debug.Log($"Puzzle Game Team Icons updated: Team1={teamIconProvider.GetTeam1Icon()?.name}, Team2={teamIconProvider.GetTeam2Icon()?.name}");
    }
    
    /// <summary>
    /// Aktualisiert ein einzelnes Team-Icon basierend auf aktuellem Team
    /// </summary>
    private void UpdateTeamIconForImage(Image targetImage, int teamNumber)
    {
        if (targetImage == null || teamIconProvider == null) return;
        
        Sprite icon = teamIconProvider.GetTeamIcon(teamNumber - 1); // teamNumber ist 1-basiert, Provider ist 0-basiert
        if (icon != null)
        {
            targetImage.sprite = icon;
            targetImage.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// LEGACY: Fallback auf alte PuzzleCollection team images
    /// </summary>
    private void SetupTeamIconsLegacy()
    {
        if (team1ResultImage && puzzleCollection.team1Image)
            team1ResultImage.sprite = puzzleCollection.team1Image;

        if (team2ResultImage && puzzleCollection.team2Image)
            team2ResultImage.sprite = puzzleCollection.team2Image;
            
        UpdateCurrentTeamImageLegacy(explanationTeamImage);
        UpdateCurrentTeamImageLegacy(countdownTeamImage);
        UpdateCurrentTeamImageLegacy(gameplayTeamImage);
    }
    
    /// <summary>
    /// DEPRECATED: Legacy-Methode - wird durch UpdateTeamIconForImage ersetzt
    /// </summary>
    private void UpdateCurrentTeamImageLegacy(Image targetImage)
    {
        if (targetImage == null) return;

        // Versuche zuerst TeamIconProvider
        if (teamIconProvider != null)
        {
            UpdateTeamIconForImage(targetImage, currentTeam);
            return;
        }

        // Legacy-Fallback
        if (currentTeam == 1 && puzzleCollection.team1Image)
        {
            targetImage.sprite = puzzleCollection.team1Image;
        }
        else if (currentTeam == 2 && puzzleCollection.team2Image)
        {
            targetImage.sprite = puzzleCollection.team2Image;
        }

        targetImage.gameObject.SetActive(true);
    }
    
    private void SetupButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
        
        if (foundButton != null)
            foundButton.onClick.AddListener(OnFoundButtonClicked);
        
        if (hintButton != null)
            hintButton.onClick.AddListener(OnHintButtonClicked);
        
        if (giveUpButton != null)
            giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);
        
        if (nextTeamButton != null)
            nextTeamButton.onClick.AddListener(OnNextTeamButtonClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
        
        if (giveUpConfirmButton != null)
            giveUpConfirmButton.onClick.AddListener(OnGiveUpConfirmed);
        
        if (giveUpCancelButton != null)
            giveUpCancelButton.onClick.AddListener(OnGiveUpCancelled);
        
        if (timeUpContinueButton != null)
            timeUpContinueButton.onClick.AddListener(OnTimeUpContinueClicked);
    }
    
    // ============================================
    // SCREEN MANAGEMENT
    // ============================================
    
    private void HideAllScreens()
    {
        if (explanationScreen != null) explanationScreen.SetActive(false);
        if (countdownScreen != null) countdownScreen.SetActive(false);
        if (gameplayScreen != null) gameplayScreen.SetActive(false);
        if (solutionScreen != null) solutionScreen.SetActive(false);
        if (resultsScreen != null) resultsScreen.SetActive(false);
        if (giveUpPopup != null) giveUpPopup.SetActive(false);
        if (timeUpPopup != null) timeUpPopup.SetActive(false);
    }
    
    private void ShowExplanationScreen()
    {
        currentState = GameState.Explanation;
        HideAllScreens();
        
        if (explanationScreen != null)
            explanationScreen.SetActive(true);
        
        // NEU: Icons für aktuelles Team aktualisieren
        UpdateTeamIconForImage(explanationTeamImage, currentTeam);
        
        StartCoroutine(UpdateExplanationUIDelayed());
        
        PlayHapticFeedback();
    }

    private IEnumerator UpdateExplanationUIDelayed()
    {
        yield return new WaitForEndOfFrame();
        UpdateExplanationUI();
    }
    
    private void ShowCountdownScreen()
    {
        currentState = GameState.Countdown;
        HideAllScreens();
        
        if (countdownScreen != null)
            countdownScreen.SetActive(true);
        
        // NEU: Icons für aktuelles Team aktualisieren
        UpdateTeamIconForImage(countdownTeamImage, currentTeam);
        
        UpdateCountdownUI();
        
        StartCoroutine(CountdownSequence());
    }
    
    private void ShowGameplayScreen()
    {
        currentState = GameState.Gameplay;
        HideAllScreens();
        
        if (gameplayScreen != null)
            gameplayScreen.SetActive(true);
        
        SetupCurrentPuzzle();
        StartRoundTimer();
        
        // NEU: Icons für aktuelles Team aktualisieren
        UpdateTeamIconForImage(gameplayTeamImage, currentTeam);
        
        StartCoroutine(UpdateGameplayUIDelayed());
        
        PlayHapticFeedback();
    }
    
    private IEnumerator UpdateGameplayUIDelayed()
    {
        yield return new WaitForEndOfFrame();
        UpdateGameplayUI();
    }
    
    private void ShowSolutionScreen(int earnedPoints)
    {
        currentState = GameState.Solution;
        HideAllScreens();
        
        if (solutionScreen != null)
            solutionScreen.SetActive(true);
        
        StartCoroutine(UpdateSolutionUIDelayed(earnedPoints));
        
        PlayHapticFeedback();
    }

    private IEnumerator UpdateSolutionUIDelayed(int earnedPoints)
    {
        yield return new WaitForEndOfFrame();
        UpdateSolutionUI(earnedPoints);
    }
    
    private void ShowResultsScreen()
    {
        currentState = GameState.Results;
        HideAllScreens();
        
        if (resultsScreen != null)
            resultsScreen.SetActive(true);
        
        // NEU: Icons auf Results Screen aktualisieren
        UpdateTeamIcons();

        SaveResults();

        StartCoroutine(UpdateResultsUIDelayed());
        
        PlayHapticFeedback();
    }

    /// <summary>
    /// Speichert die Ergebnisse des Spiels im GameDataManager
    /// </summary>
    private void SaveResults()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("GameDataManager nicht gefunden! Ergebnisse können nicht gespeichert werden.");
            return;
        }

        if (puzzleCollection == null)
        {
            Debug.LogError("PuzzleCollection fehlt! Kann roomNumber nicht abrufen.");
            return;
        }

        string collectionName = puzzleCollection.GetCollectionName(currentLanguage);
        int roomNumber = puzzleCollection.roomNumber;
        int totalRounds = puzzleCollection.roundsPerTeam; // Anzahl Runden pro Team

        GameDataManager.Instance.SaveRoomResult(
            collectionName,
            roomNumber,
            team1Score,
            team2Score,
            totalRounds * 2 // Total Questions = Runden pro Team * 2 Teams
        );

        Debug.Log($"Puzzle results saved: {collectionName} (Room {roomNumber}) - Team1: {team1Score}, Team2: {team2Score}");
    }

    private IEnumerator UpdateResultsUIDelayed()
    {
        yield return new WaitForEndOfFrame();
        UpdateResultsUI();
    }
    
    // ============================================
    // UI UPDATES
    // ============================================
    
    private void UpdateExplanationUI()
    {
        // Team image - NEU: bereits in ShowExplanationScreen() aktualisiert
        
        if (explanationTitleText != null)
        {
            explanationTitleText.text = GetLocalizedText(explanationTitleLocalizedText, "Bilderrätsel");
        }
        
        if (explanationRulesText != null && puzzleCollection != null)
        {
            string rulesFormat = GetLocalizedText(explanationRulesLocalizedText, 
                "Finde das Exponat im Raum anhand des Bildausschnitts!\n\n" +
                "• Du kannst {0}x einen Hinweis anfordern (größerer Bildausschnitt)\n" +
                "• Jeder Hinweis kostet {1} Punkt\n" +
                "• Maximal {2} Punkte pro Rätsel");
            
            explanationRulesText.text = string.Format(rulesFormat, 
                2,
                puzzleCollection.hintPenalty, 
                puzzleCollection.startingPoints);
        }
        
        if (startButton != null && startButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(startButtonLocalizedText, "Start");
        }
    }
    
    private void UpdateCountdownUI()
    {
        // Team image - NEU: bereits in ShowCountdownScreen() aktualisiert
    }
    
    private void UpdateGameplayUI()
    {
        // Team image - NEU: bereits in ShowGameplayScreen() aktualisiert
        
        if (teamScoreText != null)
        {
            int currentScore = currentTeam == 1 ? team1Score : team2Score;
            teamScoreText.text = currentScore.ToString();
        }
        
        if (roundCounterText != null)
        {
            int totalRounds = puzzleCollection.roundsPerTeam;
            if (totalRounds > 1)
            {
                string counterFormat = GetLocalizedText(roundCounterLocalizedText, "Runde {0}/{1}");
                roundCounterText.text = string.Format(counterFormat, GetCurrentTeamRound() + 1, totalRounds);
                roundCounterText.gameObject.SetActive(true);
            }
            else
            {
                roundCounterText.gameObject.SetActive(false);
            }
        }
        
        if (timerContainer != null)
        {
            timerContainer.SetActive(useTimer);
        }
        
        UpdateTimerUI();
        
        if (puzzleImage != null && currentPuzzle != null)
        {
            puzzleImage.sprite = currentPuzzle.GetHintImage(currentHintLevel);
        }
        
        if (possiblePointsText != null)
        {
            string pointsFormat = GetLocalizedText(possiblePointsLocalizedText, "Mögliche Punkte: {0}");
            possiblePointsText.text = string.Format(pointsFormat, currentPossiblePoints);
        }
        
        if (foundButton != null && foundButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            foundButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(foundButtonLocalizedText, "Gefunden!");
        }
        
        UpdateHintButtonUI();
        
        if (giveUpButton != null && giveUpButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            giveUpButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(giveUpButtonLocalizedText, "Aufgeben");
        }
    }
    
    private void UpdateHintButtonUI()
    {
        if (hintButton == null) return;
        
        int hintsRemaining = 2 - hintsUsed;
        
        if (hintsRemaining > 0)
        {
            hintButton.gameObject.SetActive(true);
            
            if (hintButton.GetComponentInChildren<TextMeshProUGUI>() != null)
            {
                hintButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                    GetLocalizedText(hintButtonLocalizedText, "Hinweis anzeigen");
            }
            
            if (hintButtonText != null && puzzleCollection != null)
            {
                string labelFormat = GetLocalizedText(hintButtonLabelLocalizedText, "{0} verfügbar, -{1} Punkt");
                hintButtonText.text = string.Format(labelFormat, hintsRemaining, puzzleCollection.hintPenalty);
                hintButtonText.gameObject.SetActive(true);
            }
        }
        else
        {
            hintButton.gameObject.SetActive(false);
            
            if (hintButtonText != null)
            {
                hintButtonText.text = GetLocalizedText(allHintsUsedLocalizedText, "Alle Hinweise benutzt");
                hintButtonText.gameObject.SetActive(true);
            }
        }
    }
    
    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        
        if (useTimer)
        {
            int seconds = Mathf.CeilToInt(roundTimer);
            timerText.text = seconds.ToString();
        }
        else
        {
            timerText.text = "";
        }
    }
    
    private void UpdateSolutionUI(int earnedPoints)
    {
        if (currentPuzzle == null) return;
        
        if (solutionImage != null)
        {
            solutionImage.sprite = currentPuzzle.solutionImage;
        }
        
        if (exhibitNameText != null)
        {
            exhibitNameText.text = currentPuzzle.GetExhibitName(currentLanguage);
        }
        
        if (exhibitDescriptionText != null)
        {
            string description = currentPuzzle.GetExhibitDescription(currentLanguage);
            exhibitDescriptionText.text = description;
            exhibitDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(description));
        }
        
        if (earnedPointsText != null)
        {
            string pointsFormat = GetLocalizedText(earnedPointsLocalizedText, "+{0} Punkte");
            earnedPointsText.text = string.Format(pointsFormat, earnedPoints);
        }
        
        if (nextTeamButton != null && nextTeamButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            nextTeamButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(nextTeamButtonLocalizedText, "Nächstes Team");
        }
    }
    
    private void UpdateResultsUI()
    {
        // Team 1 - NEU: bereits in ShowResultsScreen() aktualisiert
        if (team1ScoreText != null)
        {
            team1ScoreText.text = team1Score.ToString();
        }
        
        // Team 2 - NEU: bereits in ShowResultsScreen() aktualisiert
        if (team2ScoreText != null)
        {
            team2ScoreText.text = team2Score.ToString();
        }
        
        // Winner
        if (winnerTeamImage != null && winnerText != null)
        {
            if (team1Score > team2Score)
            {
                // NEU: Verwende TeamIconProvider
                if (teamIconProvider != null)
                {
                    Sprite team1Icon = teamIconProvider.GetTeam1Icon();
                    if (team1Icon != null)
                    {
                        winnerTeamImage.sprite = team1Icon;
                        winnerTeamImage.gameObject.SetActive(true);
                    }
                }
                else if (puzzleCollection != null && puzzleCollection.team1Image != null) // Legacy-Fallback
                {
                    winnerTeamImage.sprite = puzzleCollection.team1Image;
                    winnerTeamImage.gameObject.SetActive(true);
                }
                
                string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "Team {0} gewinnt!");
                winnerText.text = string.Format(winnerFormat, 1);
            }
            else if (team2Score > team1Score)
            {
                // NEU: Verwende TeamIconProvider
                if (teamIconProvider != null)
                {
                    Sprite team2Icon = teamIconProvider.GetTeam2Icon();
                    if (team2Icon != null)
                    {
                        winnerTeamImage.sprite = team2Icon;
                        winnerTeamImage.gameObject.SetActive(true);
                    }
                }
                else if (puzzleCollection != null && puzzleCollection.team2Image != null) // Legacy-Fallback
                {
                    winnerTeamImage.sprite = puzzleCollection.team2Image;
                    winnerTeamImage.gameObject.SetActive(true);
                }
                
                string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "Team {0} gewinnt!");
                winnerText.text = string.Format(winnerFormat, 2);
            }
            else
            {
                winnerTeamImage.gameObject.SetActive(false);
                winnerText.text = GetLocalizedText(resultsTieLocalizedText, "Unentschieden!");
            }
        }
        
        if (backButton != null && backButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(backButtonLocalizedText, "Zurück");
        }
    }
    
    // =================================-----------
    // GAMEPLAY LOGIC
    // ============================================
    
    private void SetupCurrentPuzzle()
    {
        List<PuzzlePiece> teamPuzzles = currentTeam == 1 ? team1Puzzles : team2Puzzles;
        int teamRound = GetCurrentTeamRound();
        
        if (teamRound >= 0 && teamRound < teamPuzzles.Count)
        {
            currentPuzzle = teamPuzzles[teamRound];
        }
        else
        {
            Debug.LogError($"Invalid team round: {teamRound}");
            return;
        }
        
        currentHintLevel = 0;
        hintsUsed = 0;
        currentPossiblePoints = puzzleCollection.startingPoints;
        
        Debug.Log($"Setup puzzle for Team {currentTeam}, Round {teamRound + 1}: {currentPuzzle.name}");
    }
    
    private int GetCurrentTeamRound()
    {
        if (currentTeam == 1)
        {
            return currentRound / 2;
        }
        else
        {
            return (currentRound - 1) / 2;
        }
    }
    
    private void StartRoundTimer()
    {
        DifficultyLevel currentDifficulty = currentTeam == 1 ? team1Difficulty : team2Difficulty;
        
        roundDuration = puzzleCollection.GetAdjustedRoundDuration(currentDifficulty);
        useTimer = puzzleCollection.UseTimerForDifficulty(currentDifficulty);
        
        if (useTimer)
        {
            roundTimer = roundDuration;
            timerWarningPlayed = false;
        }
        else
        {
            roundTimer = 0f;
        }
        
        Debug.Log($"Round timer: {roundDuration}s, UseTimer: {useTimer}");
    }
    
    private void OnFoundButtonClicked()
    {
        PlaySound(foundSound);
        PlayHapticFeedback();
        
        AwardPoints(currentPossiblePoints);
        ShowSolutionScreen(currentPossiblePoints);
    }
    
    private void OnHintButtonClicked()
    {
        if (hintsUsed >= 2) return;
        
        PlaySound(hintSound);
        PlayHapticFeedback();
        
        currentHintLevel++;
        hintsUsed++;
        
        currentPossiblePoints -= puzzleCollection.hintPenalty;
        currentPossiblePoints = Mathf.Max(0, currentPossiblePoints);
        
        UpdateGameplayUI();
        
        Debug.Log($"Hint used! Level: {currentHintLevel}, Remaining points: {currentPossiblePoints}");
    }
    
    private void OnGiveUpButtonClicked()
    {
        PlayHapticFeedback();
        
        if (giveUpPopup != null)
        {
            giveUpPopup.SetActive(true);
            StartCoroutine(UpdateGiveUpPopupTextDelayed());
        }
    }

    private IEnumerator UpdateGiveUpPopupTextDelayed()
    {
        yield return new WaitForEndOfFrame();
        
        if (giveUpPopupText != null)
        {
            giveUpPopupText.text = GetLocalizedText(giveUpPopupLocalizedText, 
                "Sicher aufgeben?\nKeine Punkte für diese Runde!");
        }
        
        if (giveUpConfirmButton != null && giveUpConfirmButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            giveUpConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(giveUpConfirmButtonLocalizedText, "Ja, aufgeben");
        }
        
        if (giveUpCancelButton != null && giveUpCancelButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            giveUpCancelButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(giveUpCancelButtonLocalizedText, "Abbrechen");
        }
    }
    
    private void OnGiveUpConfirmed()
    {
        PlaySound(giveUpSound);
        PlayHapticFeedback();
        
        if (giveUpPopup != null)
            giveUpPopup.SetActive(false);
        
        AwardPoints(0);
        ShowSolutionScreen(0);
    }
    
    private void OnGiveUpCancelled()
    {
        PlayHapticFeedback();
        
        if (giveUpPopup != null)
            giveUpPopup.SetActive(false);
    }
    
    private void OnTimeUp()
    {
        if (timeUpPopup != null)
        {
            timeUpPopup.SetActive(true);
            StartCoroutine(UpdateTimeUpPopupTextDelayed());
        }
    }

    private IEnumerator UpdateTimeUpPopupTextDelayed()
    {
        yield return new WaitForEndOfFrame();
        
        if (timeUpPopupText != null)
        {
            timeUpPopupText.text = GetLocalizedText(timeUpPopupLocalizedText, "Zeit vorbei!");
        }
        
        if (timeUpContinueButton != null && timeUpContinueButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            timeUpContinueButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(timeUpContinueButtonLocalizedText, "Weiter");
        }
    }

    private void OnTimeUpContinueClicked()
    {
        PlayHapticFeedback();

        if (timeUpPopup != null)
            timeUpPopup.SetActive(false);

        AwardPoints(0);
        ShowSolutionScreen(0);
    }

    private void AwardPoints(int points)
    {
        if (currentTeam == 1)
        {
            team1Score += points;
        }
        else
        {
            team2Score += points;
        }
        
        Debug.Log($"Team {currentTeam} earned {points} points. Total: {(currentTeam == 1 ? team1Score : team2Score)}");
    }
    
    // ============================================
    // ROUND PROGRESSION
    // ============================================
    
    private void OnNextTeamButtonClicked()
    {
        PlayHapticFeedback();
        
        currentRound++;
        
        int totalRounds = puzzleCollection.roundsPerTeam * 2;
        
        if (currentRound >= totalRounds)
        {
            ShowResultsScreen();
        }
        else
        {
            currentTeam = currentTeam == 1 ? 2 : 1;
            ShowExplanationScreen();
        }
    }
    
    private void OnStartButtonClicked()
    {
        PlayHapticFeedback();
        ShowCountdownScreen();
    }
    
    private IEnumerator CountdownSequence()
    {
        if (countdownText != null) countdownText.text = "3";
        PlaySound(countdownSound1);
        yield return new WaitForSeconds(1f);
        
        if (countdownText != null) countdownText.text = "2";
        PlaySound(countdownSound2);
        yield return new WaitForSeconds(1f);
        
        if (countdownText != null) countdownText.text = "1";
        PlaySound(countdownSound3);
        yield return new WaitForSeconds(1f);
        
        ShowGameplayScreen();
    }
    
    private void OnBackButtonClicked()
    {
        PlayHapticFeedback();
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
    
    // ============================================
    // LOCALIZATION
    // ============================================
    
    private void HandleLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;
        
        switch (currentState)
        {
            case GameState.Explanation:
                UpdateExplanationUI();
                break;
            case GameState.Gameplay:
                UpdateGameplayUI();
                break;
            case GameState.Solution:
                UpdateSolutionUI(0);
                break;
            case GameState.Results:
                UpdateResultsUI();
                break;
        }
        
        if (giveUpPopup != null && giveUpPopup.activeSelf)
        {
            StartCoroutine(UpdateGiveUpPopupTextDelayed());
        }
        
        if (timeUpPopup != null && timeUpPopup.activeSelf)
        {
            StartCoroutine(UpdateTimeUpPopupTextDelayed());
        }
    }
    
    private string GetLocalizedText(LocalizedText localizedText, string fallback)
    {
        if (localizedText != null)
        {
            return localizedText.GetText(currentLanguage);
        }
        
        return fallback;
    }
    
    // ============================================
    // AUDIO & HAPTICS
    // ============================================
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void PlayTimerWarning()
    {
        if (audioSource != null && timerWarningSound != null)
        {
            audioSource.PlayOneShot(timerWarningSound);
        }
    }
    
    private void PlayHapticFeedback()
    {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}