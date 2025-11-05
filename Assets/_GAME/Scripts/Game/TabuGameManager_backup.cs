using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manager für das Tabu-Minispiel
/// Analog zu FossilGameManager, aber mit Tabu-Mechanik und Button-Steuerung
/// Teams spielen abwechselnd, erklären Begriffe ohne Tabu-Wörter zu verwenden
/// </summary>
public class TabuGameManager_backup : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private TabuCollection tabuCollection;
    [SerializeField] private string nextScene = "Auswahl";

    [Header("UI References - Explanation Screen")]
    [SerializeField] private GameObject explanationScreen;
    [SerializeField] private TextMeshProUGUI explanationTitleText;
    [SerializeField] private TextMeshProUGUI explanationRulesText;
    [SerializeField] private Image explanationTeamImage;
    [SerializeField] private TextMeshProUGUI explanationDifficultyText;
    [SerializeField] private Button startButton;

    [Header("UI References - Countdown Screen")]
    [SerializeField] private GameObject countdownScreen;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("UI References - Gameplay Screen")]
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private Image currentTeamImage; // NEU: Team-Icon im Gameplay
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI mainTermText;
    [SerializeField] private TextMeshProUGUI tabuWordsHeaderText;
    [SerializeField] private Transform tabuWordsContainer;
    [SerializeField] private GameObject tabuWordPrefab; // Prefab für einzelne Tabu-Wörter (TextMeshProUGUI)
    [SerializeField] private TextMeshProUGUI scoreDisplayText;
    [SerializeField] private Button correctButton;
    [SerializeField] private Button skipButton;

    [Header("UI References - Results Screen")]
    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private TextMeshProUGUI resultsTeam1ScoreText;
    [SerializeField] private TextMeshProUGUI resultsTeam2ScoreText;
    [SerializeField] private TextMeshProUGUI resultsWinnerText;
    [SerializeField] private Image resultsTeam1Image;
    [SerializeField] private Image resultsTeam2Image;
    [SerializeField] private Image winnerTeamImage;
    [SerializeField] private Button resultsBackButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip countdownBeepClip;
    [SerializeField] private AudioClip countdownFinalClip;
    [SerializeField] private AudioClip timerWarningClip;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip skipClip;

    [Header("Localized Texts")]
    [SerializeField] private LocalizedText explanationTitleLocalizedText;
    [SerializeField] private LocalizedText explanationRulesLocalizedText;
    [SerializeField] private LocalizedText startButtonLocalizedText;
    [SerializeField] private LocalizedText timerLabelLocalizedText;
    [SerializeField] private LocalizedText tabuWordsHeaderLocalizedText;
    [SerializeField] private LocalizedText correctButtonLocalizedText;
    [SerializeField] private LocalizedText skipButtonLocalizedText;
    [SerializeField] private LocalizedText resultsTeam1LabelLocalizedText;
    [SerializeField] private LocalizedText resultsTeam2LabelLocalizedText;
    [SerializeField] private LocalizedText resultsWinnerLocalizedText;
    [SerializeField] private LocalizedText resultsTieLocalizedText;
    [SerializeField] private LocalizedText backButtonLocalizedText;

    // Game State
    private int currentTeam = 1; // 1 oder 2
    private int team1Score = 0;
    private int team2Score = 0;
    private float remainingTime;
    private bool isRoundActive = false;
    private bool hasPlayedWarning = false;

    // Term Management
    private Queue<TabuTerm> currentTermQueue;
    private List<TabuTerm> team1UsedTerms = new List<TabuTerm>();
    private TabuTerm currentTerm;

    // Difficulty
    private DifficultyLevel team1Difficulty;
    private DifficultyLevel team2Difficulty;

    // Language
    private LanguageSystem.Language currentLanguage;

    // Tabu Word UI Pool
    private List<TextMeshProUGUI> tabuWordTextPool = new List<TextMeshProUGUI>();

    // ==================== UNITY LIFECYCLE ====================

    private void Start()
    {
        InitializeGame();
        SubscribeToLanguageChanges();
        ShowExplanationScreen();
    }

    private void OnDestroy()
    {
        UnsubscribeFromLanguageChanges();
    }

    private void Update()
    {
        if (isRoundActive && gameplayScreen.activeSelf)
        {
            UpdateTimer();
        }
    }

    // ==================== INITIALIZATION ====================

    private void InitializeGame()
    {
        // Lade Team-Schwierigkeitsgrade
        team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(1);
        team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(2);

        // Aktuelle Sprache
        currentLanguage = LanguageSystem.Instance.GetCurrentLanguage();

        // Scores zurücksetzen
        team1Score = 0;
        team2Score = 0;
        currentTeam = 1;

        // UI Setup
        SetupButtons();
        HideAllScreens();
    }

    private void SetupButtons()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        if (correctButton != null)
            correctButton.onClick.AddListener(OnCorrectButtonClicked);

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkipButtonClicked);

        if (resultsBackButton != null)
            resultsBackButton.onClick.AddListener(OnBackButtonClicked);
    }

    // ==================== LANGUAGE SYSTEM ====================

    private void SubscribeToLanguageChanges()
    {
        LanguageSystem.OnLanguageChanged += OnLanguageChanged;
    }

    private void UnsubscribeFromLanguageChanges()
    {
        LanguageSystem.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;
        UpdateAllUITexts();
    }

    private void UpdateAllUITexts()
    {
        if (explanationScreen.activeSelf)
            UpdateExplanationUI();
        
        if (gameplayScreen.activeSelf)
            UpdateGameplayUI();
        
        if (resultsScreen.activeSelf)
            UpdateResultsUI();
    }

    private string GetLocalizedText(LocalizedText localizedText, string fallback)
    {
        if (localizedText != null)
            return localizedText.GetText(currentLanguage);
        return fallback;
    }

    // ==================== SCREEN MANAGEMENT ====================

    private void HideAllScreens()
    {
        if (explanationScreen != null) explanationScreen.SetActive(false);
        if (countdownScreen != null) countdownScreen.SetActive(false);
        if (gameplayScreen != null) gameplayScreen.SetActive(false);
        if (resultsScreen != null) resultsScreen.SetActive(false);
    }

    private void ShowExplanationScreen()
    {
        HideAllScreens();
        explanationScreen.SetActive(true);
        UpdateExplanationUI();
    }

    private void UpdateExplanationUI()
    {
        DifficultyLevel difficulty = currentTeam == 1 ? team1Difficulty : team2Difficulty;
        Sprite teamImage = currentTeam == 1 ? tabuCollection.Team1Image : tabuCollection.Team2Image;

        // Title
        if (explanationTitleText != null)
            explanationTitleText.text = GetLocalizedText(explanationTitleLocalizedText, "Tabu - Erkläre den Begriff!");

        // Rules
        if (explanationRulesText != null)
            explanationRulesText.text = GetLocalizedText(explanationRulesLocalizedText, 
                "Ein Spieler erklärt Begriffe, ohne die Tabu-Wörter zu verwenden. Das Team rät!");

        // Team Image
        if (explanationTeamImage != null && teamImage != null)
            explanationTeamImage.sprite = teamImage;

        // Difficulty
        if (explanationDifficultyText != null)
            explanationDifficultyText.text = GetDifficultyDisplayName(difficulty);

        // Start Button
        if (startButton != null)
        {
            var buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = GetLocalizedText(startButtonLocalizedText, "Start");
        }
    }

    private string GetDifficultyDisplayName(DifficultyLevel difficulty)
    {
        return currentLanguage switch
        {
            LanguageSystem.Language.German_Standard or LanguageSystem.Language.German_Simple => difficulty switch
            {
                DifficultyLevel.Kids => "Kinder",
                DifficultyLevel.BigKids => "Große Kinder",
                DifficultyLevel.Adults => "Erwachsene",
                _ => "Unbekannt"
            },
            _ => difficulty.ToString()
        };
    }

    // ==================== COUNTDOWN ====================

    private void OnStartButtonClicked()
    {
        PlayHapticFeedback();
        StartCoroutine(ShowCountdownAndStartRound());
    }

    private IEnumerator ShowCountdownAndStartRound()
    {
        HideAllScreens();
        countdownScreen.SetActive(true);

        string[] countdownValues = { "3", "2", "1", "!" };

        for (int i = 0; i < countdownValues.Length; i++)
        {
            if (countdownText != null)
                countdownText.text = countdownValues[i];

            if (audioSource != null)
            {
                if (i < 3 && countdownBeepClip != null)
                    audioSource.PlayOneShot(countdownBeepClip);
                else if (i == 3 && countdownFinalClip != null)
                    audioSource.PlayOneShot(countdownFinalClip);
            }

            yield return new WaitForSeconds(1f);
        }

        StartGameplayRound();
    }

    // ==================== GAMEPLAY ====================

    private void StartGameplayRound()
    {
        HideAllScreens();
        gameplayScreen.SetActive(true);

        // Terms vorbereiten
        PrepareTermsForCurrentTeam();

        // Timer starten
        DifficultyLevel difficulty = currentTeam == 1 ? team1Difficulty : team2Difficulty;
        remainingTime = tabuCollection.DefaultRoundDuration * tabuCollection.TimeSettings.GetTimeMultiplier(difficulty);
        hasPlayedWarning = false;
        isRoundActive = true;

        // Ersten Begriff anzeigen
        ShowNextTerm();
        UpdateGameplayUI();
    }

    private void PrepareTermsForCurrentTeam()
    {
        DifficultyLevel difficulty = currentTeam == 1 ? team1Difficulty : team2Difficulty;

        List<TabuTerm> selectedTerms;

        if (currentTeam == 1)
        {
            // Team 1: Zufällige Auswahl
            selectedTerms = tabuCollection.GetRandomTerms(tabuCollection.TermsPerRound, difficulty);
            team1UsedTerms = new List<TabuTerm>(selectedTerms);
        }
        else
        {
            // Team 2: Versuche andere Terms als Team 1
            selectedTerms = tabuCollection.GetRandomTermsExcluding(tabuCollection.TermsPerRound, difficulty, team1UsedTerms);
        }

        currentTermQueue = new Queue<TabuTerm>(selectedTerms);
    }

    private void ShowNextTerm()
    {
        if (currentTermQueue.Count == 0)
        {
            // Alle Begriffe durch ? Runde beenden
            EndRound();
            return;
        }

        currentTerm = currentTermQueue.Dequeue();
        UpdateTermDisplay();
    }

    private void UpdateTermDisplay()
    {
        if (currentTerm == null) return;

        // Hauptbegriff
        if (mainTermText != null)
            mainTermText.text = currentTerm.GetMainTerm(currentLanguage);

        // Tabu-Wörter
        DisplayTabuWords(currentTerm.GetTabuWords(currentLanguage));
    }

    private void DisplayTabuWords(string[] tabuWords)
    {
        // Clear alte Tabu-Wörter
        foreach (var textObj in tabuWordTextPool)
        {
            if (textObj != null)
                textObj.gameObject.SetActive(false);
        }

        // Erstelle/Reaktiviere Tabu-Wort-Texte
        for (int i = 0; i < tabuWords.Length; i++)
        {
            TextMeshProUGUI textObj;

            if (i < tabuWordTextPool.Count)
            {
                textObj = tabuWordTextPool[i];
                textObj.gameObject.SetActive(true);
            }
            else
            {
                // Erstelle neues Text-Objekt
                GameObject newObj = Instantiate(tabuWordPrefab, tabuWordsContainer);
                textObj = newObj.GetComponent<TextMeshProUGUI>();
                tabuWordTextPool.Add(textObj);
            }

            textObj.text = tabuWords[i];
        }
    }

    private void UpdateGameplayUI()
    {
        // NEU: Team-Image aktualisieren
        UpdateCurrentTeamImage();

        // Score Display
        int currentScore = currentTeam == 1 ? team1Score : team2Score;
        int maxTerms = tabuCollection.TermsPerRound;
        
        if (scoreDisplayText != null)
            scoreDisplayText.text = $"{currentScore}/{maxTerms}";

        // Tabu Words Header
        if (tabuWordsHeaderText != null)
            tabuWordsHeaderText.text = GetLocalizedText(tabuWordsHeaderLocalizedText, "Tabu-Wörter:");

        // Buttons
        if (correctButton != null)
        {
            var buttonText = correctButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = GetLocalizedText(correctButtonLocalizedText, "Richtig ?");
        }

        if (skipButton != null)
        {
            var buttonText = skipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = GetLocalizedText(skipButtonLocalizedText, "Überspringen ?");
        }
    }

    /// <summary>
    /// NEU: Team-Image im Gameplay Screen aktualisieren
    /// </summary>
    private void UpdateCurrentTeamImage()
    {
        if (currentTeamImage == null) return;

        Sprite teamSprite = currentTeam == 1 ? tabuCollection.Team1Image : tabuCollection.Team2Image;

        if (teamSprite != null)
        {
            currentTeamImage.sprite = teamSprite;
            currentTeamImage.gameObject.SetActive(true);
        }
        else
        {
            currentTeamImage.gameObject.SetActive(false);
        }
    }

    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = $"{seconds}s";

            // Farbe ändern bei wenig Zeit
            if (seconds <= 10)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }

        // Audio-Warnung bei letzten 3 Sekunden
        if (remainingTime <= 3f && remainingTime > 0f && !hasPlayedWarning)
        {
            if (audioSource != null && timerWarningClip != null)
                audioSource.PlayOneShot(timerWarningClip);
            hasPlayedWarning = true;
        }

        // Zeit abgelaufen
        if (remainingTime <= 0f)
        {
            EndRound();
        }
    }

    // ==================== BUTTON CALLBACKS ====================

    private void OnCorrectButtonClicked()
    {
        PlayHapticFeedback();
        
        if (audioSource != null && correctClip != null)
            audioSource.PlayOneShot(correctClip);

        // Punkt hinzufügen
        if (currentTeam == 1)
            team1Score++;
        else
            team2Score++;

        UpdateGameplayUI();
        ShowNextTerm();
    }

    private void OnSkipButtonClicked()
    {
        PlayHapticFeedback();
        
        if (audioSource != null && skipClip != null)
            audioSource.PlayOneShot(skipClip);

        // Begriff recyclen (ans Ende der Queue)
        if (currentTerm != null)
            currentTermQueue.Enqueue(currentTerm);

        ShowNextTerm();
    }

    private void EndRound()
    {
        isRoundActive = false;

        if (currentTeam == 1)
        {
            // Team 1 fertig ? Team 2 dran
            currentTeam = 2;
            ShowExplanationScreen();
        }
        else
        {
            // Beide Teams fertig ? Results
            ShowResultsScreen();
        }
    }

    // ==================== RESULTS SCREEN ====================

    private void ShowResultsScreen()
    {
        HideAllScreens();
        resultsScreen.SetActive(true);
        UpdateResultsUI();
    }

    private void UpdateResultsUI()
    {
        // Scores
        if (resultsTeam1ScoreText != null)
            resultsTeam1ScoreText.text = $"{GetLocalizedText(resultsTeam1LabelLocalizedText, "Team 1")}: {team1Score}";

        if (resultsTeam2ScoreText != null)
            resultsTeam2ScoreText.text = $"{GetLocalizedText(resultsTeam2LabelLocalizedText, "Team 2")}: {team2Score}";

        // Team Images
        if (resultsTeam1Image != null && tabuCollection.Team1Image != null)
            resultsTeam1Image.sprite = tabuCollection.Team1Image;

        if (resultsTeam2Image != null && tabuCollection.Team2Image != null)
            resultsTeam2Image.sprite = tabuCollection.Team2Image;

        // Winner (mit Gewinner-Icon)
        if (resultsWinnerText != null)
        {
            if (team1Score > team2Score)
            {
                // Team 1 gewinnt
                string winnerText = GetLocalizedText(resultsWinnerLocalizedText, "gewinnt! ??");
                resultsWinnerText.text = winnerText;

                // Gewinner-Icon anzeigen
                if (winnerTeamImage != null && tabuCollection.Team1Image != null)
                {
                    winnerTeamImage.sprite = tabuCollection.Team1Image;
                    winnerTeamImage.gameObject.SetActive(true);
                }
            }
            else if (team2Score > team1Score)
            {
                // Team 2 gewinnt
                string winnerText = GetLocalizedText(resultsWinnerLocalizedText, "gewinnt! ??");
                resultsWinnerText.text = winnerText;

                // Gewinner-Icon anzeigen
                if (winnerTeamImage != null && tabuCollection.Team2Image != null)
                {
                    winnerTeamImage.sprite = tabuCollection.Team2Image;
                    winnerTeamImage.gameObject.SetActive(true);
                }
            }
            else
            {
                // Unentschieden
                resultsWinnerText.text = GetLocalizedText(resultsTieLocalizedText, "Unentschieden! ??");

                // Gewinner-Icon ausblenden
                if (winnerTeamImage != null)
                    winnerTeamImage.gameObject.SetActive(false);
            }
        }

        // Back Button
        if (resultsBackButton != null)
        {
            var buttonText = resultsBackButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = GetLocalizedText(backButtonLocalizedText, "Zurück");
        }
    }

    private void OnBackButtonClicked()
    {
        PlayHapticFeedback();
        // Navigation zurück zum Hauptmenü (analog zu FossilGameManager)
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }

    // ==================== HELPER METHODS ====================

    private void PlayHapticFeedback()
    {
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    // ==================== EDITOR HELPER ====================

#if UNITY_EDITOR
    [ContextMenu("Test: Start Team 1 Round")]
    private void TestStartTeam1()
    {
        currentTeam = 1;
        InitializeGame();
        ShowExplanationScreen();
    }

    [ContextMenu("Test: Skip to Results")]
    private void TestResults()
    {
        team1Score = 3;
        team2Score = 5;
        ShowResultsScreen();
    }
#endif
}