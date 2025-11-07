using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TabuGameManager : MonoBehaviour
{
    [Header("Game Data")]
    public TabuCollection tabuCollection;
    [SerializeField] private string nextScene = "Auswahl";

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
    public TextMeshProUGUI mainTermText;
    public Button mainTermButton; // NEU: Button für Begriff-Tap
    public Transform tabuWordsContainer;
    public GameObject tabuWordPrefab;
    public TextMeshProUGUI timerText;
    public Image currentTeamImage;
    public TextMeshProUGUI scoreText;
    public Button correctButton;
    public Button skipButton;

    [Header("Result Screen")]
    public Image team1ImageResult;
    public TextMeshProUGUI team1ScoreText;
    public Image team2ImageResult;
    public TextMeshProUGUI team2ScoreText;
    public Image winnerTeamImage;
    public TextMeshProUGUI winnerText;
    public Button playAgainButton;
    public Button backToMenuButton;
    
    [Header("Team Icon Provider (NEU)")]
    [SerializeField] private TeamIconProvider teamIconProvider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip skipSound;
    public AudioClip timeUpSound;
    public AudioClip countdownSound;
    public AudioClip countdownStartSound;

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
    private List<TabuTerm> currentRoundTerms;
    private int currentTermIndex = 0;
    private float roundTimeLeft;
    private bool gameActive = false;
    private bool timerPaused = true; // NEU: Timer-Pausierung
    private bool termVisible = true; // NEU: Begriff-Sichtbarkeit-Tracking
    private int currentTeam = 1;
    private int team1Score = 0;
    private int team2Score = 0;
    private int correctTermsThisRound = 0;

    // Timer-Countdown Sounds
    private bool hasPlayedThreeSecondWarning = false;
    private bool hasPlayedTwoSecondWarning = false;
    private bool hasPlayedOneSecondWarning = false;

    // Aktuelle Sprache für Performance
    private LanguageSystem.Language currentLanguage = LanguageSystem.Language.German_Standard;

    // UI Pool für Tabu-Wörter
    private List<GameObject> tabuWordUIElements = new List<GameObject>();

    void Start()
    {
        InitializeLocalization();
        InitializeGame();

        // Event-Listener für Sprachwechsel
        LanguageSystem.OnLanguageChanged += OnLanguageChanged;
    }

    void OnDestroy()
    {
        LanguageSystem.OnLanguageChanged -= OnLanguageChanged;
    }

    void InitializeLocalization()
    {
        currentLanguage = LanguageSystem.Instance != null ?
            LanguageSystem.Instance.GetCurrentLanguage() : LanguageSystem.Language.German_Standard;
    }

    void OnLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;

        // Aktualisiere UI-Texte basierend auf aktuellem Zustand
        if (explanationUI.activeInHierarchy)
        {
            ShowExplanation();
        }

        if (gameplayUI.activeInHierarchy)
        {
            UpdateGameplayUI();
        }

        if (resultUI.activeInHierarchy)
        {
            UpdateResultsUI();
        }
    }

    void InitializeGame()
    {
        if (tabuCollection == null)
        {
            Debug.LogError("No tabu collection assigned!");
            return;
        }

        if (!tabuCollection.HasTermsForAllDifficulties())
        {
            Debug.LogWarning("Not all difficulty levels have terms assigned!");
        }
        
        // DEPRECATED: Legacy team images - jetzt über TeamIconProvider
        if (tabuCollection.Team1Image != null || tabuCollection.Team2Image != null)
        {
            Debug.LogWarning("TabuCollection team images sind DEPRECATED. Verwende TeamIconProvider stattdessen!");
        }

        SetupUI();
        
        // NEU: Team-Icons laden
        UpdateTeamIcons();
        
        ShowExplanation();
    }

    void SetupUI()
    {
        startButton.onClick.AddListener(StartCountdown);
        correctButton.onClick.AddListener(OnCorrectTerm);
        skipButton.onClick.AddListener(OnSkipTerm);
        playAgainButton.onClick.AddListener(RestartGame);
        backToMenuButton.onClick.AddListener(BackToMenu);
        
        // NEU: Begriff-Button für Tap-to-Start/Toggle
        if (mainTermButton != null)
        {
            mainTermButton.onClick.AddListener(OnTermTapped);
        }

        explanationUI.SetActive(true);
        countdownUI.SetActive(false);
        gameplayUI.SetActive(false);
        resultUI.SetActive(false);
    }

    // NEU: Team-Icons aktualisieren
    void UpdateTeamIcons()
    {
        if (teamIconProvider == null)
        {
            Debug.LogWarning("TeamIconProvider nicht zugewiesen! Verwende Fallback von TabuCollection.");
            SetupTeamIconsLegacy();
            return;
        }
        
        // Explanation Screen Icon
        UpdateTeamIconForImage(currentTeamImageExplanation, currentTeam);
        
        // Gameplay Icon
        UpdateTeamIconForImage(currentTeamImage, currentTeam);
        
        // Results Screen Icons
        if (team1ImageResult != null)
        {
            Sprite team1Icon = teamIconProvider.GetTeam1Icon();
            if (team1Icon != null)
                team1ImageResult.sprite = team1Icon;
        }
        
        if (team2ImageResult != null)
        {
            Sprite team2Icon = teamIconProvider.GetTeam2Icon();
            if (team2Icon != null)
                team2ImageResult.sprite = team2Icon;
        }
        
        Debug.Log($"Tabu Game Team Icons updated: Team1={teamIconProvider.GetTeam1Icon()?.name}, Team2={teamIconProvider.GetTeam2Icon()?.name}");
    }
    
    /// <summary>
    /// Aktualisiert ein einzelnes Team-Icon basierend auf aktuellem Team
    /// </summary>
    void UpdateTeamIconForImage(Image targetImage, int teamIndex)
    {
        if (targetImage == null || teamIconProvider == null) return;
        
        Sprite icon = teamIconProvider.GetTeamIcon(teamIndex - 1); // teamIndex ist 1-basiert, Provider ist 0-basiert
        if (icon != null)
        {
            targetImage.sprite = icon;
            targetImage.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// LEGACY: Fallback auf alte TabuCollection team images
    /// </summary>
    void SetupTeamIconsLegacy()
    {
        if (team1ImageResult && tabuCollection.Team1Image)
            team1ImageResult.sprite = tabuCollection.Team1Image;

        if (team2ImageResult && tabuCollection.Team2Image)
            team2ImageResult.sprite = tabuCollection.Team2Image;
            
        UpdateCurrentTeamImage(currentTeamImageExplanation);
        UpdateCurrentTeamImage(currentTeamImage);
    }

    void ShowExplanation()
    {
        DifficultyLevel currentTeamDifficulty = DifficultyLevel.Adults;
        if (GameDataManager.Instance != null)
        {
            currentTeamDifficulty = GameDataManager.Instance.GetTeamDifficulty(currentTeam - 1);
        }

        float adjustedDuration = tabuCollection.GetAdjustedRoundDuration(currentTeamDifficulty);

        // Lokalisierte Texte verwenden
        string explanationTitle = GetLocalizedText(explanationTitleLocalizedText, "Tabu - Erkläre den Begriff!");
        string explanationRules = GetLocalizedText(explanationRulesLocalizedText, 
            "Ein Spieler erklärt Begriffe, ohne die Tabu-Wörter zu verwenden. Das Team rät!");

        explanationText.text = $"<b>{explanationRules}";
    
        // NEU: Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImageExplanation, currentTeam);
        
        // Start Button
        if (startButton != null && startButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(startButtonLocalizedText, "Start");
        }
    }

    string GetLocalizedText(LocalizedText localizedText, string fallback)
    {
        if (localizedText != null)
            return localizedText.GetText(currentLanguage);
        return fallback;
    }

    // DEPRECATED: Legacy-Methode - wird durch UpdateTeamIconForImage ersetzt
    void UpdateCurrentTeamImage(Image targetImage)
    {
        if (targetImage == null) return;

        // Versuche zuerst TeamIconProvider
        if (teamIconProvider != null)
        {
            UpdateTeamIconForImage(targetImage, currentTeam);
            return;
        }

        // Legacy-Fallback
        if (currentTeam == 1 && tabuCollection.Team1Image)
        {
            targetImage.sprite = tabuCollection.Team1Image;
        }
        else if (currentTeam == 2 && tabuCollection.Team2Image)
        {
            targetImage.sprite = tabuCollection.Team2Image;
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

            if (audioSource)
            {
                if (i < 3 && countdownSound)
                {
                    audioSource.PlayOneShot(countdownSound);
                }
                else if (i == 3 && countdownStartSound)
                {
                    audioSource.PlayOneShot(countdownStartSound);
                }
            }

            yield return new WaitForSeconds(1f);
        }

        StartRound();
    }

    void StartRound()
    {
        countdownUI.SetActive(false);
        gameplayUI.SetActive(true);

        correctTermsThisRound = 0;

        DifficultyLevel currentTeamDifficulty = DifficultyLevel.Adults;
        if (GameDataManager.Instance != null)
        {
            currentTeamDifficulty = GameDataManager.Instance.GetTeamDifficulty(currentTeam - 1);
        }

        // Team 1 bekommt zufällige Terms
        if (currentTeam == 1)
        {
            currentRoundTerms = tabuCollection.GetRandomTerms(tabuCollection.TermsPerRound, currentTeamDifficulty);
        }
        // Team 2 bekommt andere Terms (wenn möglich)
        else
        {
            List<TabuTerm> team1Terms = tabuCollection.GetRandomTerms(tabuCollection.TermsPerRound, 
                GameDataManager.Instance.GetTeamDifficulty(0));
            currentRoundTerms = tabuCollection.GetRandomTermsExcluding(tabuCollection.TermsPerRound, 
                currentTeamDifficulty, team1Terms);
        }

        currentTermIndex = 0;
        roundTimeLeft = tabuCollection.GetAdjustedRoundDuration(currentTeamDifficulty);
        gameActive = true;
        timerPaused = true; // NEU: Timer startet pausiert

        hasPlayedThreeSecondWarning = false;
        hasPlayedTwoSecondWarning = false;
        hasPlayedOneSecondWarning = false;

        ShowCurrentTerm();
        UpdateGameplayUI();
    }

    void Update()
    {
        if (!gameActive) return;

        // NEU: Timer nur aktualisieren wenn nicht pausiert
        if (!timerPaused)
        {
            roundTimeLeft -= Time.deltaTime;
            CheckTimerCountdownSounds();

            if (roundTimeLeft <= 0)
            {
                EndRound();
            }

            UpdateTimerDisplay();
        }
    }

    void CheckTimerCountdownSounds()
    {
        if (audioSource && countdownSound)
        {
            if (roundTimeLeft <= 3f && roundTimeLeft > 2f && !hasPlayedThreeSecondWarning)
            {
                audioSource.PlayOneShot(countdownSound);
                hasPlayedThreeSecondWarning = true;
            }
            else if (roundTimeLeft <= 2f && roundTimeLeft > 1f && !hasPlayedTwoSecondWarning)
            {
                audioSource.PlayOneShot(countdownSound);
                hasPlayedTwoSecondWarning = true;
            }
            else if (roundTimeLeft <= 1f && roundTimeLeft > 0f && !hasPlayedOneSecondWarning)
            {
                audioSource.PlayOneShot(countdownSound);
                hasPlayedOneSecondWarning = true;
            }
        }
    }

    /// <summary>
    /// NEU: Wird aufgerufen wenn Spieler auf den Begriff tippt
    /// - Erstes Tap: Begriff ausblenden + Timer starten
    /// - Weiteres Tap: Begriff ein/ausblenden (Toggle) OHNE Timer zu pausieren
    /// </summary>
    void OnTermTapped()
    {
        if (!gameActive) return;

        // Haptic Feedback
        TriggerHapticFeedback();

        // FALL 1: Timer ist noch pausiert ? Erstes Tap (Spiel starten)
        if (timerPaused)
        {
            // Begriff ausblenden
            if (mainTermText != null)
            {
                mainTermText.gameObject.SetActive(false);
                termVisible = false;
            }

            // Timer starten
            timerPaused = false;

            // NEU: Correct Button einblenden
            UpdateButtonVisibility();

            Debug.Log($"Begriff ausgeblendet, Timer gestartet. Verbleibende Zeit: {Mathf.FloorToInt(roundTimeLeft)}s");
        }
        // FALL 2: Timer läuft bereits ? Toggle Begriff-Sichtbarkeit
        else
        {
            if (mainTermText != null)
            {
                termVisible = !termVisible;
                mainTermText.gameObject.SetActive(termVisible);

                Debug.Log($"Begriff {(termVisible ? "eingeblendet" : "ausgeblendet")} (Timer läuft weiter)");
            }
        }
    }

    /// <summary>
    /// NEU: Aktualisiert Button-Sichtbarkeit basierend auf Timer-Zustand
    /// Correct-Button wird nur angezeigt wenn Timer läuft
    /// </summary>
    void UpdateButtonVisibility()
    {
        if (correctButton != null)
        {
            correctButton.gameObject.SetActive(!timerPaused);
        }

        // Skip-Button ist immer sichtbar
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
        }
    }

    void ShowCurrentTerm()
    {
        if (currentTermIndex >= currentRoundTerms.Count)
        {
            // Shuffle verbleibende Terms
            for (int i = currentRoundTerms.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (currentRoundTerms[i], currentRoundTerms[randomIndex]) = (currentRoundTerms[randomIndex], currentRoundTerms[i]);
            }
            currentTermIndex = 0;
        }

        TabuTerm term = currentRoundTerms[currentTermIndex];
        
        // NEU: Begriff wieder einblenden und Button aktivieren
        if (mainTermText != null)
        {
            mainTermText.text = term.GetMainTerm(currentLanguage);
            mainTermText.gameObject.SetActive(true);
            termVisible = true; // NEU: Tracking aktualisieren
        }

        if (mainTermButton != null)
        {
            mainTermButton.interactable = true;
        }

        // NEU: Timer pausieren für nächsten Begriff
        timerPaused = true;

        // NEU: Button-Sichtbarkeit aktualisieren
        UpdateButtonVisibility();

        // Aktualisiere Tabu-Wörter
        UpdateTabuWordsUI(term);
    }

    void UpdateTabuWordsUI(TabuTerm term)
    {
        // Clear existing UI elements
        foreach (var element in tabuWordUIElements)
        {
            Destroy(element);
        }
        tabuWordUIElements.Clear();

        // Get tabu words for current language
        string[] tabuWords = term.GetTabuWords(currentLanguage);

        // Create UI elements for each tabu word
        foreach (string word in tabuWords)
        {
            if (string.IsNullOrEmpty(word)) continue;

            GameObject wordObj = Instantiate(tabuWordPrefab, tabuWordsContainer);
            TextMeshProUGUI wordText = wordObj.GetComponent<TextMeshProUGUI>();
            if (wordText != null)
            {
                wordText.text = word;
            }
            tabuWordUIElements.Add(wordObj);
        }
    }

    void OnCorrectTerm()
    {
        if (!gameActive) return;

        if (audioSource && correctSound)
        {
            audioSource.PlayOneShot(correctSound);
        }

        TriggerHapticFeedback();

        if (currentTeam == 1)
            team1Score++;
        else
            team2Score++;

        correctTermsThisRound++;
        currentRoundTerms.RemoveAt(currentTermIndex);

        if (currentTermIndex >= currentRoundTerms.Count)
        {
            currentTermIndex = 0;
        }

        if (correctTermsThisRound >= tabuCollection.TermsPerRound)
        {
            Debug.Log($"Erreicht! {correctTermsThisRound}/{tabuCollection.TermsPerRound} Begriffe erraten.");
            EndRound();
            return;
        }

        if (currentRoundTerms.Count == 0)
        {
            Debug.Log("Alle Begriffe erraten! Runde beendet.");
            EndRound();
            return;
        }

        ShowCurrentTerm();
        UpdateGameplayUI();
    }

    void OnSkipTerm()
    {
        if (!gameActive) return;

        if (audioSource && skipSound)
        {
            audioSource.PlayOneShot(skipSound);
        }

        TriggerHapticFeedback();

        TabuTerm skippedTerm = currentRoundTerms[currentTermIndex];
        currentRoundTerms.RemoveAt(currentTermIndex);
        currentRoundTerms.Add(skippedTerm);

        if (currentTermIndex >= currentRoundTerms.Count)
        {
            currentTermIndex = 0;
        }

        ShowCurrentTerm();
        UpdateGameplayUI();
    }

    void TriggerHapticFeedback()
    {
        #if UNITY_IOS || UNITY_ANDROID
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
        #endif
    }

    void EndRound()
    {
        gameActive = false;
        timerPaused = true; // NEU: Timer pausieren

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
        
        // NEU: Icons auf Results Screen aktualisieren
        UpdateTeamIcons();
        
        UpdateResultsUI();
        SaveResults();
    }

    void UpdateResultsUI()
    {
        // Scores mit lokalisierten Team-Labels
        string team1Label = GetLocalizedText(resultsTeam1LabelLocalizedText, "Team 1");
        string team2Label = GetLocalizedText(resultsTeam2LabelLocalizedText, "Team 2");

        team1ScoreText.text = $"{team1Label}: {team1Score}";
        team2ScoreText.text = $"{team2Label}: {team2Score}";

        if (team1Score > team2Score)
        {
            string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "gewinnt! ??");
            winnerText.text = winnerFormat;
            
            // NEU: Verwende TeamIconProvider
            if (winnerTeamImage && teamIconProvider != null)
            {
                Sprite team1Icon = teamIconProvider.GetTeam1Icon();
                if (team1Icon != null)
                {
                    winnerTeamImage.sprite = team1Icon;
                    winnerTeamImage.gameObject.SetActive(true);
                }
            }
            else if (winnerTeamImage && tabuCollection.Team1Image) // Legacy-Fallback
            {
                winnerTeamImage.sprite = tabuCollection.Team1Image;
                winnerTeamImage.gameObject.SetActive(true);
            }
        }
        else if (team2Score > team1Score)
        {
            string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "gewinnt! ??");
            winnerText.text = winnerFormat;
            
            // NEU: Verwende TeamIconProvider
            if (winnerTeamImage && teamIconProvider != null)
            {
                Sprite team2Icon = teamIconProvider.GetTeam2Icon();
                if (team2Icon != null)
                {
                    winnerTeamImage.sprite = team2Icon;
                    winnerTeamImage.gameObject.SetActive(true);
                }
            }
            else if (winnerTeamImage && tabuCollection.Team2Image) // Legacy-Fallback
            {
                winnerTeamImage.sprite = tabuCollection.Team2Image;
                winnerTeamImage.gameObject.SetActive(true);
            }
        }
        else
        {
            string tieText = GetLocalizedText(resultsTieLocalizedText, "Unentschieden! ??");
            winnerText.text = tieText;
            if (winnerTeamImage)
            {
                winnerTeamImage.gameObject.SetActive(false);
            }
        }
        
        // Back Button
        if (backToMenuButton != null && backToMenuButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            backToMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(backButtonLocalizedText, "Zurück");
        }
    }

    void SaveResults()
    {
        if (GameDataManager.Instance != null && tabuCollection != null)
        {
            GameDataManager.Instance.SaveRoomResult(
                tabuCollection.GetCollectionName(currentLanguage),
                tabuCollection.roomNumber,
                team1Score,
                team2Score,
                tabuCollection.TermsPerRound
            );
        }
    }

    void UpdateGameplayUI()
    {
        // NEU: Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImage, currentTeam);
        
        scoreText.text = $"{correctTermsThisRound}/{tabuCollection.TermsPerRound}";
        
        // NEU: Button-Sichtbarkeit aktualisieren
        UpdateButtonVisibility();
        
        // Buttons
        if (correctButton != null && correctButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            correctButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(correctButtonLocalizedText, "Richtig ?");
        }
        
        if (skipButton != null && skipButton.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            skipButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                GetLocalizedText(skipButtonLocalizedText, "Überspringen ?");
        }
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.FloorToInt(roundTimeLeft);
        timerText.text = seconds.ToString();
    }

    void RestartGame()
    {
        team1Score = 0;
        team2Score = 0;
        currentTeam = 1;
        correctTermsThisRound = 0;

        hasPlayedThreeSecondWarning = false;
        hasPlayedTwoSecondWarning = false;
        hasPlayedOneSecondWarning = false;

        resultUI.SetActive(false);
        
        // NEU: Icons neu laden (falls geändert)
        UpdateTeamIcons();
        
        ShowExplanation();
    }

    void BackToMenu()
    {
        SceneManager.LoadScene(nextScene);
    }
}