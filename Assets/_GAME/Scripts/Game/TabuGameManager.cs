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

    // NEU (19.11.2025): Team-Start-Reihenfolge umkehren
    [Header("Game Flow Settings")]
    [SerializeField] private bool team2StartsFirst = false;
    [Tooltip("Wenn aktiviert, beginnt Team 2 statt Team 1 das Spiel")]

    [Header("UI References")]
    public GameObject explanationUI;
    public GameObject countdownUI;
    public GameObject gameplayUI;
    public GameObject resultUI;

    [Header("Explanation Screen")]
    public TextMeshProUGUI explanationText;
    public Image currentTeamImageExplanation;
    [SerializeField] private TextMeshProUGUI explanationTeamNameText; // NEU (19.11.2025) - Optional
    public Button startButton;

    [Header("Countdown")]
    [SerializeField] private Image countdownTeamImage;
    [SerializeField] private TextMeshProUGUI countdownTeamNameText; // NEU (19.11.2025) - Optional
    public TextMeshProUGUI countdownText;

    [Header("Gameplay UI")]
    public TextMeshProUGUI mainTermText;
    public Button mainTermButton; // Button für Begriff-Tap (nur im Tap-to-Start-Modus)
    public GameObject startIndicatorIcon; // "Los!"-Icon (nur im Tap-to-Start-Modus)
    public Transform tabuWordsContainer;
    public GameObject tabuWordPrefab;
    public TextMeshProUGUI timerText;
    public Image currentTeamImage;
    [SerializeField] private TextMeshProUGUI gameplayTeamNameText; // NEU (19.11.2025) - Optional
    public TextMeshProUGUI scoreText;
    public Button correctButton;
    public Button skipButton;

    [Header("Result Screen")]
    public Image team1ImageResult;
    [SerializeField] private TextMeshProUGUI team1NameText; // NEU (19.11.2025) - Optional
    public TextMeshProUGUI team1ScoreText;
    public Image team2ImageResult;
    [SerializeField] private TextMeshProUGUI team2NameText; // NEU (19.11.2025) - Optional
    public TextMeshProUGUI team2ScoreText;
    public Image winnerTeamImage;
    [SerializeField] private TextMeshProUGUI winnerNameText; // NEU (19.11.2025) - Optional
    public TextMeshProUGUI winnerText;
    public Button playAgainButton;
    public Button backToMenuButton;
    
    [Header("Team Icon Provider")]
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
    private bool timerPaused = false; // Timer-Pausierung (nur wenn enableTapToStart = true)
    private bool termVisible = true; // Begriff-Sichtbarkeit-Tracking
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
            UpdateTeamNames();
        }

        if (gameplayUI.activeInHierarchy)
        {
            UpdateGameplayUI();
            UpdateTeamNames();
        }

        if (resultUI.activeInHierarchy)
        {
            UpdateResultsUI();
            UpdateTeamNames();
        }
    }

    void InitializeGame()
    {

        // NEU (19.11.2025): Team-Start-Reihenfolge basierend auf Checkbox
        currentTeam = team2StartsFirst ? 2 : 1;

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
        
        // Team-Icons laden
        UpdateTeamIcons();
        
        // NEU (19.11.2025): Modus-Logging
        Debug.Log($"Tabu-Modus: {(tabuCollection.EnableTapToStart ? "Tap-to-Start (07.11.2025)" : "Continuous Timer (Standard)")}");
        
        ShowExplanation();
    }

    void SetupUI()
    {
        startButton.onClick.AddListener(StartCountdown);
        correctButton.onClick.AddListener(OnCorrectTerm);
        skipButton.onClick.AddListener(OnSkipTerm);
        playAgainButton.onClick.AddListener(RestartGame);
        backToMenuButton.onClick.AddListener(BackToMenu);
        
        // Begriff-Button für Tap-to-Start/Toggle
        if (mainTermButton != null)
        {
            mainTermButton.onClick.AddListener(OnTermTapped);
        }

        explanationUI.SetActive(true);
        countdownUI.SetActive(false);
        gameplayUI.SetActive(false);
        resultUI.SetActive(false);
    }

    private void UpdateTeamNames()
    {
        if (teamIconProvider == null) return;

        // Aktuelles Team (für Explanation/Countdown/Gameplay)
        string currentTeamName = teamIconProvider.GetTeamIconNameText(currentTeam - 1, currentLanguage);

        // Explanation Screen - aktuelles Team
        if (explanationTeamNameText != null)
        {
            explanationTeamNameText.text = currentTeamName;
            explanationTeamNameText.gameObject.SetActive(true);
        }

        // Countdown Screen - aktuelles Team
        if (countdownTeamNameText != null)
        {
            countdownTeamNameText.text = currentTeamName;
            countdownTeamNameText.gameObject.SetActive(true);
        }

        // Gameplay Screen - aktuelles Team
        if (gameplayTeamNameText != null)
        {
            gameplayTeamNameText.text = currentTeamName;
            gameplayTeamNameText.gameObject.SetActive(true);
        }

        // Results Screen - beide Teams
        if (team1NameText != null)
        {
            team1NameText.text = teamIconProvider.GetTeam1IconNameText(currentLanguage);
            team1NameText.gameObject.SetActive(true);
        }

        if (team2NameText != null)
        {
            team2NameText.text = teamIconProvider.GetTeam2IconNameText(currentLanguage);
            team2NameText.gameObject.SetActive(true);
        }
    }

    // Team-Icons aktualisieren
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
    
        // Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImageExplanation, currentTeam);
        UpdateTeamNames();

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
        UpdateTeamIconForImage(countdownTeamImage, currentTeam);
        UpdateTeamNames();

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
        
        // NEU (19.11.2025): Timer-Start basierend auf Modus
        timerPaused = tabuCollection.EnableTapToStart; // Nur pausieren wenn Tap-to-Start aktiv

        hasPlayedThreeSecondWarning = false;
        hasPlayedTwoSecondWarning = false;
        hasPlayedOneSecondWarning = false;

        ShowCurrentTerm();
        UpdateGameplayUI();
    }

    void Update()
    {
        if (!gameActive) return;

        // Timer nur aktualisieren wenn nicht pausiert
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
    /// Wird aufgerufen wenn Spieler auf den Begriff tippt
    /// MODUS A (Tap-to-Start aktiv):
    ///   - Erstes Tap: Begriff ausblenden + Timer starten + "Los!"-Icon verstecken
    ///   - Weiteres Tap: Begriff ein/ausblenden (Toggle) OHNE Timer zu pausieren
    /// MODUS B (Tap-to-Start inaktiv):
    ///   - Begriff-Button ist deaktiviert (nicht interaktiv)
    /// </summary>
    void OnTermTapped()
    {
        if (!gameActive) return;
        
        // NEU (19.11.2025): Nur reagieren wenn Tap-to-Start-Modus aktiv ist
        if (!tabuCollection.EnableTapToStart)
        {
            Debug.LogWarning("OnTermTapped() aufgerufen, aber Tap-to-Start ist deaktiviert! Button sollte nicht interaktiv sein.");
            return;
        }

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

            // "Los!"-Icon ausblenden
            UpdateStartIndicatorVisibility();

            // Correct Button einblenden
            UpdateButtonVisibility();

            Debug.Log($"[Tap-to-Start Modus] Begriff ausgeblendet, Timer gestartet. Verbleibende Zeit: {Mathf.FloorToInt(roundTimeLeft)}s");
        }
        // FALL 2: Timer läuft bereits ? Toggle Begriff-Sichtbarkeit
        else
        {
            if (mainTermText != null)
            {
                termVisible = !termVisible;
                mainTermText.gameObject.SetActive(termVisible);

                Debug.Log($"[Tap-to-Start Modus] Begriff {(termVisible ? "eingeblendet" : "ausgeblendet")} (Timer läuft weiter)");
            }
        }
    }

    /// <summary>
    /// Aktualisiert "Los!"-Icon Sichtbarkeit
    /// - NUR sichtbar wenn Tap-to-Start aktiv UND Timer pausiert ist
    /// - Versteckt wenn Tap-to-Start inaktiv ODER Timer läuft
    /// </summary>
    void UpdateStartIndicatorVisibility()
    {
        if (startIndicatorIcon != null)
        {
            // NEU (19.11.2025): Nur anzeigen wenn Tap-to-Start aktiv UND Timer pausiert
            bool shouldShow = tabuCollection.EnableTapToStart && timerPaused;
            startIndicatorIcon.SetActive(shouldShow);
        }
    }

    /// <summary>
    /// Aktualisiert Button-Sichtbarkeit basierend auf Timer-Zustand und Modus
    /// MODUS A (Tap-to-Start aktiv):
    ///   - Correct-Button nur sichtbar wenn Timer läuft
    /// MODUS B (Tap-to-Start inaktiv):
    ///   - Correct-Button immer sichtbar
    /// </summary>
    void UpdateButtonVisibility()
    {
        if (correctButton != null)
        {
            // NEU (19.11.2025): Correct-Button basierend auf Modus
            bool correctVisible = tabuCollection.EnableTapToStart ? !timerPaused : true;
            correctButton.gameObject.SetActive(correctVisible);
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
        
        // Begriff einblenden und Button-State setzen
        if (mainTermText != null)
        {
            mainTermText.text = term.GetMainTerm(currentLanguage);
            mainTermText.gameObject.SetActive(true);
            termVisible = true;
        }

        // NEU (19.11.2025): Button nur aktivieren wenn Tap-to-Start aktiv ist
        if (mainTermButton != null)
        {
            mainTermButton.interactable = tabuCollection.EnableTapToStart;
        }

        // NEU (19.11.2025): Timer nur pausieren wenn Tap-to-Start aktiv ist
        if (tabuCollection.EnableTapToStart)
        {
            timerPaused = true;
        }

        // "Los!"-Icon und Button-Sichtbarkeit aktualisieren
        UpdateStartIndicatorVisibility();
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
            Debug.Log($"Ziel erreicht! {correctTermsThisRound}/{tabuCollection.TermsPerRound} Begriffe erraten.");
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
        timerPaused = true;

        if (audioSource && timeUpSound)
        {
            audioSource.PlayOneShot(timeUpSound);
        }

        // NEU (19.11.2025): Team wechseln
        int startingTeam = team2StartsFirst ? 2 : 1;
        int secondTeam = team2StartsFirst ? 1 : 2;

        if (currentTeam == startingTeam)
        {
            currentTeam = secondTeam;
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
        
        // Icons auf Results Screen aktualisieren
        UpdateTeamIcons();
        
        UpdateResultsUI();
        SaveResults();
    }

    void UpdateResultsUI()
    {

        // NEU (19.11.2025): Namen aktualisieren (optional)
        UpdateTeamNames();

        // NEU: Gewinner-Name anzeigen (optional)
        if (winnerNameText != null && teamIconProvider != null)
        {
            if (team1Score > team2Score)
            {
                winnerNameText.text = teamIconProvider.GetTeam1IconNameText(currentLanguage);
                winnerNameText.gameObject.SetActive(true);
            }
            else if (team2Score > team1Score)
            {
                winnerNameText.text = teamIconProvider.GetTeam2IconNameText(currentLanguage);
                winnerNameText.gameObject.SetActive(true);
            }
            else
            {
                winnerNameText.gameObject.SetActive(false);
            }
        }

        // Scores mit lokalisierten Team-Labels
        string team1Label = GetLocalizedText(resultsTeam1LabelLocalizedText, "Team 1");
        string team2Label = GetLocalizedText(resultsTeam2LabelLocalizedText, "Team 2");

        team1ScoreText.text = $"{team1Label}: {team1Score}";
        team2ScoreText.text = $"{team2Label}: {team2Score}";

        if (team1Score > team2Score)
        {
            string winnerFormat = GetLocalizedText(resultsWinnerLocalizedText, "gewinnt! ??");
            winnerText.text = winnerFormat;
            
            // Verwende TeamIconProvider
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
            
            // Verwende TeamIconProvider
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
        UpdateTeamNames();

        scoreText.text = $"{correctTermsThisRound}/{tabuCollection.TermsPerRound}";
        
        // Button-Sichtbarkeit aktualisieren
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
        currentTeam = team2StartsFirst ? 2 : 1;
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