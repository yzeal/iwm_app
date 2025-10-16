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
    [SerializeField] private GameObject timerContainer; // Can be hidden when timer is disabled
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownSound1;
    [SerializeField] private AudioClip countdownSound2;
    [SerializeField] private AudioClip countdownSound3;
    [SerializeField] private AudioClip timerWarningSound;
    [SerializeField] private AudioClip foundSound;
    [SerializeField] private AudioClip hintSound;
    [SerializeField] private AudioClip giveUpSound;

    [Header("Localized Texts (17 assets needed)")]
    [SerializeField] private LocalizedText explanationTitleLocalizedText;
    [SerializeField] private LocalizedText explanationRulesLocalizedText;
    [SerializeField] private LocalizedText startButtonLocalizedText;
    [SerializeField] private LocalizedText roundCounterLocalizedText; // "Runde {0}/{1}"
    [SerializeField] private LocalizedText timerLabelLocalizedText;
    [SerializeField] private LocalizedText possiblePointsLocalizedText; // "Mögliche Punkte: {0}"
    [SerializeField] private LocalizedText foundButtonLocalizedText;
    [SerializeField] private LocalizedText hintButtonLocalizedText; // "{0} verfügbar, -{1} Punkt"
    [SerializeField] private LocalizedText allHintsUsedLocalizedText;
    [SerializeField] private LocalizedText giveUpButtonLocalizedText;
    [SerializeField] private LocalizedText giveUpPopupLocalizedText;
    [SerializeField] private LocalizedText timeUpPopupLocalizedText;
    [SerializeField] private LocalizedText earnedPointsLocalizedText; // "+{0} Punkte"
    [SerializeField] private LocalizedText nextTeamButtonLocalizedText;
    [SerializeField] private LocalizedText resultsWinnerLocalizedText; // "Team {0} gewinnt!"
    [SerializeField] private LocalizedText resultsTieLocalizedText;
    [SerializeField] private LocalizedText backButtonLocalizedText;

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
    private int currentRound = 0; // 0-indexed
    private int team1Score = 0;
    private int team2Score = 0;

    private List<PuzzlePiece> team1Puzzles;
    private List<PuzzlePiece> team2Puzzles;
    private PuzzlePiece currentPuzzle;

    private int currentHintLevel = 0; // 0-2 (3 images total)
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
        // Subscribe to language changes (static event - access via class, not instance)
        LanguageSystem.OnLanguageChanged += HandleLanguageChanged;

        // Get current language using method (not property)
        currentLanguage = LanguageSystem.Instance.GetCurrentLanguage();

        // Get team difficulties
        if (GameDataManager.Instance != null)
        {
            team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(1);
            team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(2);
        }
        else
        {
            team1Difficulty = DifficultyLevel.Kids;
            team2Difficulty = DifficultyLevel.Kids;
            Debug.LogWarning("GameDataManager not found! Using default difficulty.");
        }

        // Setup button listeners
        SetupButtonListeners();

        // Initialize game
        InitializeGame();

        // Start with explanation for Team 1
        ShowExplanationScreen();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events (static event - access via class, not instance)
        LanguageSystem.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void Update()
    {
        // Update timer during gameplay
        if (currentState == GameState.Gameplay && useTimer && roundTimer > 0f)
        {
            roundTimer -= Time.deltaTime;

            // Update timer UI
            UpdateTimerUI();

            // Play warning sound at 3 seconds
            if (!timerWarningPlayed && roundTimer <= 3f && roundTimer > 0f)
            {
                PlayTimerWarning();
                timerWarningPlayed = true;
            }

            // Time's up
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

        // Reset scores
        team1Score = 0;
        team2Score = 0;
        currentTeam = 1;
        currentRound = 0;

        // Load puzzles for both teams
        LoadPuzzlesForTeams();

        // Hide all screens
        HideAllScreens();

        Debug.Log($"Puzzle Game initialized. Team 1: {team1Difficulty}, Team 2: {team2Difficulty}");
    }

    private void LoadPuzzlesForTeams()
    {
        int roundsPerTeam = puzzleCollection.roundsPerTeam;

        // Get puzzles for Team 1
        team1Puzzles = puzzleCollection.GetRandomPuzzles(roundsPerTeam, team1Difficulty);

        // Get puzzles for Team 2 (excluding Team 1's puzzles)
        team2Puzzles = puzzleCollection.GetRandomPuzzlesExcluding(roundsPerTeam, team2Difficulty, team1Puzzles);

        Debug.Log($"Loaded {team1Puzzles.Count} puzzles for Team 1, {team2Puzzles.Count} puzzles for Team 2");
    }

    private void SetupButtonListeners()
    {
        // Explanation Screen
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        // Gameplay Screen
        if (foundButton != null)
            foundButton.onClick.AddListener(OnFoundButtonClicked);

        if (hintButton != null)
            hintButton.onClick.AddListener(OnHintButtonClicked);

        if (giveUpButton != null)
            giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);

        // Solution Screen
        if (nextTeamButton != null)
            nextTeamButton.onClick.AddListener(OnNextTeamButtonClicked);

        // Results Screen
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);

        // Popups
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

        UpdateExplanationUI();

        PlayHapticFeedback();
    }

    private void ShowCountdownScreen()
    {
        currentState = GameState.Countdown;
        HideAllScreens();

        if (countdownScreen != null)
            countdownScreen.SetActive(true);

        UpdateCountdownUI();

        StartCoroutine(CountdownSequence());
    }

    private void ShowGameplayScreen()
    {
        currentState = GameState.Gameplay;
        HideAllScreens();

        if (gameplayScreen != null)
            gameplayScreen.SetActive(true);

        // Setup current puzzle
        SetupCurrentPuzzle();

        // Start timer
        StartRoundTimer();

        UpdateGameplayUI();

        PlayHapticFeedback();
    }

    private void ShowSolutionScreen(int earnedPoints)
    {
        currentState = GameState.Solution;
        HideAllScreens();

        if (solutionScreen != null)
            solutionScreen.SetActive(true);

        UpdateSolutionUI(earnedPoints);

        PlayHapticFeedback();
    }

    private void ShowResultsScreen()
    {
        currentState = GameState.Results;
        HideAllScreens();

        if (resultsScreen != null)
            resultsScreen.SetActive(true);

        UpdateResultsUI();

        PlayHapticFeedback();
    }

    // ============================================
    // UI UPDATES
    // ============================================

    private void UpdateExplanationUI()
    {
        // Team image
        if (explanationTeamImage != null && puzzleCollection != null)
        {
            explanationTeamImage.sprite = puzzleCollection.GetTeamImage(currentTeam);
        }

        // Title
        if (explanationTitleText != null)
        {
            explanationTitleText.text = GetLocalizedText(explanationTitleLocalizedText, "Bilderrätsel");
        }

        // Rules
        if (explanationRulesText != null)
        {
            explanationRulesText.text = GetLocalizedText(explanationRulesLocalizedText,
                "Finde das Exponat im Raum anhand des Bildausschnitts!\n\n" +
                "• Du kannst 2x einen Hinweis anfordern (größerer Bildausschnitt)\n" +
                "• Jeder Hinweis kostet 1 Punkt\n" +
                "• Maximal 3 Punkte pro Rätsel");
        }

        // Start button
        if (startButton != null && startButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(startButtonLocalizedText, "Start");
        }
    }

    private void UpdateCountdownUI()
    {
        // Team image
        if (countdownTeamImage != null && puzzleCollection != null)
        {
            countdownTeamImage.sprite = puzzleCollection.GetTeamImage(currentTeam);
        }
    }

    private void UpdateGameplayUI()
    {
        // Team image
        if (gameplayTeamImage != null && puzzleCollection != null)
        {
            gameplayTeamImage.sprite = puzzleCollection.GetTeamImage(currentTeam);
        }

        // Team score (just number)
        if (teamScoreText != null)
        {
            int currentScore = currentTeam == 1 ? team1Score : team2Score;
            teamScoreText.text = currentScore.ToString();
        }

        // Round counter (only if multiple rounds)
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

        // Timer (show/hide based on useTimer)
        if (timerContainer != null)
        {
            timerContainer.SetActive(useTimer);
        }

        UpdateTimerUI();

        // Puzzle image
        if (puzzleImage != null && currentPuzzle != null)
        {
            puzzleImage.sprite = currentPuzzle.GetHintImage(currentHintLevel);
        }

        // Possible points
        if (possiblePointsText != null)
        {
            string pointsFormat = GetLocalizedText(possiblePointsLocalizedText, "Mögliche Punkte: {0}");
            possiblePointsText.text = string.Format(pointsFormat, currentPossiblePoints);
        }

        // Found button
        if (foundButton != null && foundButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            foundButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(foundButtonLocalizedText, "Gefunden!");
        }

        // Hint button
        UpdateHintButtonUI();

        // Give up button
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
            // Show button with remaining hints
            hintButton.gameObject.SetActive(true);

            if (hintButtonText != null)
            {
                string hintFormat = GetLocalizedText(hintButtonLocalizedText, "{0} verfügbar, -{1} Punkt");
                hintButtonText.text = string.Format(hintFormat, hintsRemaining, puzzleCollection.hintPenalty);
            }
        }
        else
        {
            // Replace button with text
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

        // Solution image
        if (solutionImage != null)
        {
            solutionImage.sprite = currentPuzzle.solutionImage;
        }

        // Exhibit name
        if (exhibitNameText != null)
        {
            exhibitNameText.text = currentPuzzle.GetExhibitName(currentLanguage);
        }

        // Exhibit description
        if (exhibitDescriptionText != null)
        {
            string description = currentPuzzle.GetExhibitDescription(currentLanguage);
            exhibitDescriptionText.text = description;

            // Hide if empty
            exhibitDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(description));
        }

        // Earned points
        if (earnedPointsText != null)
        {
            string pointsFormat = GetLocalizedText(earnedPointsLocalizedText, "+{0} Punkte");
            earnedPointsText.text = string.Format(pointsFormat, earnedPoints);
        }

        // Next team button
        if (nextTeamButton != null && nextTeamButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            nextTeamButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(nextTeamButtonLocalizedText, "Nächstes Team");
        }
    }

    private void UpdateResultsUI()
    {
        // Team 1
        if (team1ResultImage != null && puzzleCollection != null)
        {
            team1ResultImage.sprite = puzzleCollection.GetTeamImage(1);
        }

        if (team1ScoreText != null)
        {
            team1ScoreText.text = team1Score.ToString();
        }

        // Team 2
        if (team2ResultImage != null && puzzleCollection != null)
        {
            team2ResultImage.sprite = puzzleCollection.GetTeamImage(2);
        }

        if (team2ScoreText != null)
        {
            team2ScoreText.text = team2Score.ToString();
        }

        // Winner
        if (winnerTeamImage != null && winnerText != null && puzzleCollection != null)
        {
            if (team1Score > team2Score)
            {
                winnerTeamImage.sprite = puzzleCollection.GetTeamImage(1);
                winnerTeamImage.gameObject.SetActive(true);

                string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "Team {0} gewinnt!");
                winnerText.text = string.Format(winnerFormat, 1);
            }
            else if (team2Score > team1Score)
            {
                winnerTeamImage.sprite = puzzleCollection.GetTeamImage(2);
                winnerTeamImage.gameObject.SetActive(true);

                string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "Team {0} gewinnt!");
                winnerText.text = string.Format(winnerFormat, 2);
            }
            else
            {
                winnerTeamImage.gameObject.SetActive(false);
                winnerText.text = GetLocalizedText(resultsTieLocalizedText, "Unentschieden!");
            }
        }

        // Back button
        if (backButton != null && backButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            backButton.GetComponentInChildren<TextMeshProUGUI>().text = GetLocalizedText(backButtonLocalizedText, "Zurück");
        }
    }

    // ============================================
    // GAMEPLAY LOGIC
    // ============================================

    private void SetupCurrentPuzzle()
    {
        // Get current puzzle for team
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

        // Reset hint state
        currentHintLevel = 0;
        hintsUsed = 0;
        currentPossiblePoints = puzzleCollection.startingPoints;

        Debug.Log($"Setup puzzle for Team {currentTeam}, Round {teamRound + 1}: {currentPuzzle.name}");
    }

    private int GetCurrentTeamRound()
    {
        // Calculate which round the current team is on
        // currentRound cycles through all rounds (both teams)
        // We need to map this to the team's specific round

        // Team 1 plays on even rounds (0, 2, 4, ...)
        // Team 2 plays on odd rounds (1, 3, 5, ...)

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

        // Award points
        AwardPoints(currentPossiblePoints);

        // Show solution
        ShowSolutionScreen(currentPossiblePoints);
    }

    private void OnHintButtonClicked()
    {
        if (hintsUsed >= 2) return; // Max 2 hints

        PlaySound(hintSound);
        PlayHapticFeedback();

        // Increase hint level
        currentHintLevel++;
        hintsUsed++;

        // Deduct points
        currentPossiblePoints -= puzzleCollection.hintPenalty;
        currentPossiblePoints = Mathf.Max(0, currentPossiblePoints);

        // Update UI
        UpdateGameplayUI();

        Debug.Log($"Hint used! Level: {currentHintLevel}, Remaining points: {currentPossiblePoints}");
    }

    private void OnGiveUpButtonClicked()
    {
        PlayHapticFeedback();

        // Show give up popup
        if (giveUpPopup != null)
        {
            giveUpPopup.SetActive(true);

            if (giveUpPopupText != null)
            {
                giveUpPopupText.text = GetLocalizedText(giveUpPopupLocalizedText,
                    "Sicher aufgeben?\nKeine Punkte für diese Runde!");
            }
        }
    }

    private void OnGiveUpConfirmed()
    {
        PlaySound(giveUpSound);
        PlayHapticFeedback();

        if (giveUpPopup != null)
            giveUpPopup.SetActive(false);

        // Award 0 points
        AwardPoints(0);

        // Show solution
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
        // Show time up popup
        if (timeUpPopup != null)
        {
            timeUpPopup.SetActive(true);

            if (timeUpPopupText != null)
            {
                timeUpPopupText.text = GetLocalizedText(timeUpPopupLocalizedText, "Zeit vorbei!");
            }
        }
    }

    private void OnTimeUpContinueClicked()
    {
        PlayHapticFeedback();

        if (timeUpPopup != null)
            timeUpPopup.SetActive(false);

        // Award 0 points
        AwardPoints(0);

        // Show solution
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

        // Advance round
        currentRound++;

        int totalRounds = puzzleCollection.roundsPerTeam * 2;

        if (currentRound >= totalRounds)
        {
            // Game over
            ShowResultsScreen();
        }
        else
        {
            // Switch team
            currentTeam = currentTeam == 1 ? 2 : 1;

            // Show explanation for next team
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
        // 3
        if (countdownText != null) countdownText.text = "3";
        PlaySound(countdownSound1);
        yield return new WaitForSeconds(1f);

        // 2
        if (countdownText != null) countdownText.text = "2";
        PlaySound(countdownSound2);
        yield return new WaitForSeconds(1f);

        // 1
        if (countdownText != null) countdownText.text = "1";
        PlaySound(countdownSound3);
        yield return new WaitForSeconds(1f);

        // Start gameplay
        ShowGameplayScreen();
    }

    private void OnBackButtonClicked()
    {
        PlayHapticFeedback();

        // Navigate back to main menu (adjust scene name as needed)
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // ============================================
    // LOCALIZATION
    // ============================================

    private void HandleLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;

        // Update all visible UI elements
        switch (currentState)
        {
            case GameState.Explanation:
                UpdateExplanationUI();
                break;
            case GameState.Gameplay:
                UpdateGameplayUI();
                break;
            case GameState.Solution:
                UpdateSolutionUI(0); // Points already awarded, just refresh
                break;
            case GameState.Results:
                UpdateResultsUI();
                break;
        }

        // Update popups if visible
        if (giveUpPopup != null && giveUpPopup.activeSelf)
        {
            if (giveUpPopupText != null)
            {
                giveUpPopupText.text = GetLocalizedText(giveUpPopupLocalizedText,
                    "Sicher aufgeben?\nKeine Punkte für diese Runde!");
            }
        }

        if (timeUpPopup != null && timeUpPopup.activeSelf)
        {
            if (timeUpPopupText != null)
            {
                timeUpPopupText.text = GetLocalizedText(timeUpPopupLocalizedText, "Zeit vorbei!");
            }
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