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
    [SerializeField] private LocalizedText gameTitleLocalizedText;
    [SerializeField] private LocalizedText howToPlayLocalizedText;
    [SerializeField] private LocalizedText rulesLocalizedText;
    [SerializeField] private LocalizedText explainWithoutTabuWordsLocalizedText;
    [SerializeField] private LocalizedText timePerRoundLocalizedText;
    [SerializeField] private LocalizedText termsPerRoundLocalizedText;
    [SerializeField] private LocalizedText difficultyLocalizedText;
    [SerializeField] private LocalizedText thisTeamPlayingLocalizedText;
    [SerializeField] private LocalizedText secondsLocalizedText;
    [SerializeField] private LocalizedText pointsLocalizedText;
    [SerializeField] private LocalizedText winnerLocalizedText;
    [SerializeField] private LocalizedText tieLocalizedText;

    // Game State
    private List<TabuTerm> currentRoundTerms;
    private int currentTermIndex = 0;
    private float roundTimeLeft;
    private bool gameActive = false;
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
        string gameTitle = GetLocalizedText(gameTitleLocalizedText, GetDefaultGameTitle);
        string howToPlay = GetLocalizedText(howToPlayLocalizedText, GetDefaultHowToPlay);
        string rules = GetLocalizedText(rulesLocalizedText, GetDefaultRules);
        string explainWithoutTabu = GetLocalizedText(explainWithoutTabuWordsLocalizedText, GetDefaultExplainWithoutTabu);
        string timePerRound = GetLocalizedText(timePerRoundLocalizedText, GetDefaultTimePerRound);
        string termsPerRound = GetLocalizedText(termsPerRoundLocalizedText, GetDefaultTermsPerRound);
        string difficulty = GetLocalizedText(difficultyLocalizedText, GetDefaultDifficulty);
        string thisTeamPlaying = GetLocalizedText(thisTeamPlayingLocalizedText, GetDefaultThisTeamPlaying);
        string seconds = GetLocalizedText(secondsLocalizedText, GetDefaultSeconds);

        explanationText.text = $"<b>{gameTitle}</b>\n\n" +
                              $"<b>{howToPlay}:</b>\n" +
                              $"<b>{rules}:</b>\n" +
                              $"• {explainWithoutTabu}\n\n" +
                              $"<b>{timePerRound}:</b> {adjustedDuration:F0} {seconds}\n" +
                              $"<b>{termsPerRound}:</b> {tabuCollection.TermsPerRound}\n" +
                              $"<b>{difficulty}:</b> {GetDifficultyDisplayName(currentTeamDifficulty)}\n\n" +
                              $"{thisTeamPlaying}:";
    
        // NEU: Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImageExplanation, currentTeam);
    }

    string GetLocalizedText(LocalizedText localizedText, System.Func<LanguageSystem.Language, string> fallbackFunction)
    {
        return localizedText != null ? localizedText.GetText(currentLanguage) : fallbackFunction(currentLanguage);
    }

    string GetDifficultyDisplayName(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => currentLanguage switch
            {
                LanguageSystem.Language.English_Standard => "Kids",
                LanguageSystem.Language.English_Simple => "Easy",
                LanguageSystem.Language.German_Simple => "Einfach",
                _ => "Kids"
            },
            DifficultyLevel.BigKids => currentLanguage switch
            {
                LanguageSystem.Language.English_Standard => "Big Kids",
                LanguageSystem.Language.English_Simple => "Medium",
                LanguageSystem.Language.German_Simple => "Mittel",
                _ => "BigKids"
            },
            DifficultyLevel.Adults => currentLanguage switch
            {
                LanguageSystem.Language.English_Standard => "Adults",
                LanguageSystem.Language.English_Simple => "Hard",
                LanguageSystem.Language.German_Simple => "Schwer",
                _ => "Adults"
            },
            _ => "Adults"
        };
    }

    #region Default Fallback Functions

    string GetDefaultGameTitle(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Tabu",
            LanguageSystem.Language.English_Simple => "Word Game",
            LanguageSystem.Language.German_Simple => "Wörter-Spiel",
            _ => "Tabu"
        };
    }

    string GetDefaultHowToPlay(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "How to play",
            LanguageSystem.Language.English_Simple => "How to play",
            LanguageSystem.Language.German_Simple => "So wird gespielt",
            _ => "Wie gespielt wird"
        };
    }

    string GetDefaultRules(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Rules",
            LanguageSystem.Language.English_Simple => "Rules",
            LanguageSystem.Language.German_Simple => "Regeln",
            _ => "Regeln"
        };
    }

    string GetDefaultExplainWithoutTabu(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Explain the main term without using the tabu words",
            LanguageSystem.Language.English_Simple => "Explain without using the forbidden words",
            LanguageSystem.Language.German_Simple => "Erkläre ohne die verbotenen Wörter",
            _ => "Erkläre den Hauptbegriff ohne die Tabu-Wörter zu verwenden"
        };
    }

    string GetDefaultTimePerRound(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Time per round",
            LanguageSystem.Language.English_Simple => "Time",
            LanguageSystem.Language.German_Simple => "Zeit pro Runde",
            _ => "Zeit pro Runde"
        };
    }

    string GetDefaultTermsPerRound(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Terms per round",
            LanguageSystem.Language.English_Simple => "Words",
            LanguageSystem.Language.German_Simple => "Wörter pro Runde",
            _ => "Begriffe pro Runde"
        };
    }

    string GetDefaultDifficulty(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Difficulty",
            LanguageSystem.Language.English_Simple => "Level",
            LanguageSystem.Language.German_Simple => "Schwierigkeit",
            _ => "Schwierigkeitsgrad"
        };
    }

    string GetDefaultThisTeamPlaying(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "This team is playing now",
            LanguageSystem.Language.English_Simple => "This team plays now",
            LanguageSystem.Language.German_Simple => "Dieses Team ist dran",
            _ => "Dieses Team ist jetzt dran"
        };
    }

    string GetDefaultSeconds(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "seconds",
            LanguageSystem.Language.English_Simple => "seconds",
            LanguageSystem.Language.German_Simple => "Sekunden",
            _ => "Sekunden"
        };
    }

    string GetDefaultPoints(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Points",
            LanguageSystem.Language.English_Simple => "Points",
            LanguageSystem.Language.German_Simple => "Punkte",
            _ => "Punkte"
        };
    }

    string GetDefaultWinner(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Winner!",
            LanguageSystem.Language.English_Simple => "Winner!",
            LanguageSystem.Language.German_Simple => "Gewinner!",
            _ => "Gewinner!"
        };
    }

    string GetDefaultTie(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "It's a tie!",
            LanguageSystem.Language.English_Simple => "Tie!",
            LanguageSystem.Language.German_Simple => "Unentschieden!",
            _ => "Unentschieden!"
        };
    }

    #endregion

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

        hasPlayedThreeSecondWarning = false;
        hasPlayedTwoSecondWarning = false;
        hasPlayedOneSecondWarning = false;

        ShowCurrentTerm();
        UpdateGameplayUI();
    }

    void Update()
    {
        if (!gameActive) return;

        roundTimeLeft -= Time.deltaTime;
        CheckTimerCountdownSounds();

        if (roundTimeLeft <= 0)
        {
            EndRound();
        }

        UpdateTimerDisplay();
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
        mainTermText.text = term.GetMainTerm(currentLanguage);

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
        string pointsLabel = GetLocalizedText(pointsLocalizedText, GetDefaultPoints);

        team1ScoreText.text = $"{team1Score} {pointsLabel}";
        team2ScoreText.text = $"{team2Score} {pointsLabel}";

        if (team1Score > team2Score)
        {
            string winnerFormat = GetLocalizedText(winnerLocalizedText, GetDefaultWinner);
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
            string winnerFormat = GetLocalizedText(winnerLocalizedText, GetDefaultWinner);
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
            string tieText = GetLocalizedText(tieLocalizedText, GetDefaultTie);
            winnerText.text = tieText;
            if (winnerTeamImage)
            {
                winnerTeamImage.gameObject.SetActive(false);
            }
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