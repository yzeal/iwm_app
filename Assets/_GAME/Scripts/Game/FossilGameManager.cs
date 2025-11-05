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
    [SerializeField] private LocalizedText controlsLocalizedText;
    [SerializeField] private LocalizedText timePerRoundLocalizedText;
    [SerializeField] private LocalizedText wordsPerRoundLocalizedText;
    [SerializeField] private LocalizedText difficultyLocalizedText;
    [SerializeField] private LocalizedText thisTeamPlayingLocalizedText;
    [SerializeField] private LocalizedText secondsLocalizedText;
    [SerializeField] private LocalizedText pointsLocalizedText;
    [SerializeField] private LocalizedText winnerLocalizedText;
    [SerializeField] private LocalizedText tieLocalizedText;

    [Header("Localized Instructions")]
    [SerializeField] private LocalizedText holdPhoneInstructionLocalizedText;
    [SerializeField] private LocalizedText teamExplainsInstructionLocalizedText;

    [Header("Localized Input Mode Texts")]
    [SerializeField] private LocalizedText tiltModeLocalizedText;
    [SerializeField] private LocalizedText touchModeLocalizedText;

    // Game State
    private List<FossilData> currentRoundFossils;
    private int currentFossilIndex = 0;
    private float roundTimeLeft;
    private bool gameActive = false;
    private int currentTeam = 1;
    private int team1Score = 0;
    private int team2Score = 0;
    private int correctFossilsThisRound = 0;

    // Timer-Countdown Sounds
    private bool hasPlayedThreeSecondWarning = false;
    private bool hasPlayedTwoSecondWarning = false;
    private bool hasPlayedOneSecondWarning = false;

    // Aktuelle Sprache für Performance
    private LanguageSystem.Language currentLanguage = LanguageSystem.Language.German_Standard;

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
        if (fossilCollection == null)
        {
            Debug.LogError("No fossil collection assigned!");
            return;
        }

        if (!fossilCollection.HasFossilsForAllDifficulties())
        {
            Debug.LogWarning("Not all difficulty levels have fossils assigned!");
        }
        
        // DEPRECATED: Legacy team images - jetzt über TeamIconProvider
        if (fossilCollection.team1Image != null || fossilCollection.team2Image != null)
        {
            Debug.LogWarning("FossilCollection team images sind DEPRECATED. Verwende TeamIconProvider stattdessen!");
        }

        SetupUI();
        
        // NEU: Team-Icons laden
        UpdateTeamIcons();
        
        ShowExplanation();
    }

    void SetupUI()
    {
        startButton.onClick.AddListener(StartCountdown);
        playAgainButton.onClick.AddListener(RestartGame);
        backToMenuButton.onClick.AddListener(BackToMenu);

        SetupInputHandlers();

        // ? KRITISCH: Input standardmäßig deaktivieren!
        fossilInputHandler.SetInputEnabled(false);

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
            Debug.LogWarning("TeamIconProvider nicht zugewiesen! Verwende Fallback von FossilCollection.");
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
        
        Debug.Log($"Fossil Game Team Icons updated: Team1={teamIconProvider.GetTeam1Icon()?.name}, Team2={teamIconProvider.GetTeam2Icon()?.name}");
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
    /// LEGACY: Fallback auf alte FossilCollection team images
    /// </summary>
    void SetupTeamIconsLegacy()
    {
        if (team1ImageResult && fossilCollection.team1Image)
            team1ImageResult.sprite = fossilCollection.team1Image;

        if (team2ImageResult && fossilCollection.team2Image)
            team2ImageResult.sprite = fossilCollection.team2Image;
            
        UpdateCurrentTeamImage(currentTeamImageExplanation);
        UpdateCurrentTeamImage(currentTeamImage);
    }

    void SetupInputHandlers()
    {
        fossilInputHandler.OnCorrectInput += OnCorrectFossil;
        fossilInputHandler.OnSkipInput += OnSkipFossil;
    }

    void ShowExplanation()
    {
        // Lokalisierte Input-Mode Info
        string inputInfo = GetLocalizedInputModeInfo();

        DifficultyLevel currentTeamDifficulty = DifficultyLevel.Adults;
        if (GameDataManager.Instance != null)
        {
            currentTeamDifficulty = GameDataManager.Instance.GetTeamDifficulty(currentTeam - 1);
        }

        float adjustedDuration = fossilCollection.GetAdjustedRoundDuration(currentTeamDifficulty);

        // Lokalisierte Texte verwenden
        string gameTitle = GetLocalizedText(gameTitleLocalizedText, GetDefaultGameTitle);
        string howToPlay = GetLocalizedText(howToPlayLocalizedText, GetDefaultHowToPlay);
        string controls = GetLocalizedText(controlsLocalizedText, GetDefaultControls);
        string timePerRound = GetLocalizedText(timePerRoundLocalizedText, GetDefaultTimePerRound);
        string wordsPerRound = GetLocalizedText(wordsPerRoundLocalizedText, GetDefaultWordsPerRound);
        string difficulty = GetLocalizedText(difficultyLocalizedText, GetDefaultDifficulty);
        string thisTeamPlaying = GetLocalizedText(thisTeamPlayingLocalizedText, GetDefaultThisTeamPlaying);
        string seconds = GetLocalizedText(secondsLocalizedText, GetDefaultSeconds);
        string holdPhoneInstruction = GetLocalizedText(holdPhoneInstructionLocalizedText, GetDefaultHoldPhoneInstruction);
        string teamExplainsInstruction = GetLocalizedText(teamExplainsInstructionLocalizedText, GetDefaultTeamExplainsInstruction);

        explanationText.text = $"<b>{gameTitle}</b>\n\n" +
                              $"<b>{howToPlay}:</b>\n" +
                              $"• {holdPhoneInstruction}\n" +
                              $"• {teamExplainsInstruction}\n\n" +
                              $"<b>{controls}:</b>\n" +
                              $"{inputInfo}\n\n" +
                              $"<b>{timePerRound}:</b> {adjustedDuration:F0} {seconds}\n" +
                              $"<b>{wordsPerRound}:</b> {fossilCollection.fossilsPerRound}\n" +
                              $"<b>{difficulty}:</b> {GetDifficultyDisplayName(currentTeamDifficulty)}\n\n" +
                              $"{thisTeamPlaying}:";
    
        // NEU: Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImageExplanation, currentTeam);
    }

    string GetLocalizedInputModeInfo()
    {
        bool usingAccelerometer = fossilInputHandler.IsUsingAccelerometer();

        if (usingAccelerometer)
        {
            return GetLocalizedText(tiltModeLocalizedText, GetDefaultTiltMode);
        }
        else
        {
            return GetLocalizedText(touchModeLocalizedText, GetDefaultTouchMode);
        }
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
            LanguageSystem.Language.English_Standard => "Fossil Charades",
            LanguageSystem.Language.English_Simple => "Fossil Game",
            LanguageSystem.Language.German_Simple => "Fossil-Raten",
            _ => "Fossilien-Stirnraten"
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

    string GetDefaultControls(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Controls",
            LanguageSystem.Language.English_Simple => "Controls",
            LanguageSystem.Language.German_Simple => "Steuerung",
            _ => "Steuerung"
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

    string GetDefaultWordsPerRound(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Words per round",
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

    string GetDefaultHoldPhoneInstruction(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Hold the phone to your forehead",
            LanguageSystem.Language.English_Simple => "Put phone on your head",
            LanguageSystem.Language.German_Simple => "Handy an die Stirn halten",
            _ => "Halte das Handy an deine Stirn"
        };
    }

    string GetDefaultTeamExplainsInstruction(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Your team explains the fossil to you",
            LanguageSystem.Language.English_Simple => "Your team tells you the word",
            LanguageSystem.Language.German_Simple => "Dein Team erklärt dir das Fossil",
            _ => "Dein Team erklärt dir das Fossil"
        };
    }

    string GetDefaultTiltMode(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Tilt mode - Tilt phone forward (correct) or backward (skip)",
            LanguageSystem.Language.English_Simple => "Tilt phone forward (correct) or back (skip)",
            LanguageSystem.Language.German_Simple => "Kippen-Modus - Handy nach vorne (richtig) oder zurück (überspringen)",
            _ => "Neige-Modus - Neige das Handy nach vorne (richtig) oder zurück (überspringen)"
        };
    }

    string GetDefaultTouchMode(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Touch mode - Tap left (skip) or right (correct)",
            LanguageSystem.Language.English_Simple => "Tap left (skip) or right (correct)",
            LanguageSystem.Language.German_Simple => "Touch-Modus - Tippe links (überspringen) oder rechts (richtig)",
            _ => "Touch-Modus - Tippe links (überspringen) oder rechts (richtig)"
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

        correctFossilsThisRound = 0;

        DifficultyLevel currentTeamDifficulty = DifficultyLevel.Adults;
        if (GameDataManager.Instance != null)
        {
            currentTeamDifficulty = GameDataManager.Instance.GetTeamDifficulty(currentTeam - 1);
        }

        FossilData[] availableFossils = fossilCollection.GetRandomFossils(fossilCollection.fossilsPerRound, currentTeamDifficulty);
        currentRoundFossils = new List<FossilData>(availableFossils);

        currentFossilIndex = 0;
        roundTimeLeft = fossilCollection.GetAdjustedRoundDuration(currentTeamDifficulty);
        gameActive = true;

        hasPlayedThreeSecondWarning = false;
        hasPlayedTwoSecondWarning = false;
        hasPlayedOneSecondWarning = false;

        fossilInputHandler.SetInputEnabled(true);
        fossilInputHandler.CalibrateDevice();

        ShowCurrentFossil();
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

    void ShowCurrentFossil()
    {
        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            for (int i = currentRoundFossils.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (currentRoundFossils[i], currentRoundFossils[randomIndex]) = (currentRoundFossils[randomIndex], currentRoundFossils[i]);
            }
            currentFossilIndex = 0;
        }

        FossilData fossil = currentRoundFossils[currentFossilIndex];
        fossilImage.sprite = fossil.fossilImage;
        fossilNameText.text = fossil.GetFossilName(currentLanguage);
    }

    void OnCorrectFossil()
    {
        if (!gameActive) return;

        if (audioSource && correctSound)
        {
            audioSource.PlayOneShot(correctSound);
        }

        if (currentTeam == 1)
            team1Score++;
        else
            team2Score++;

        correctFossilsThisRound++;
        currentRoundFossils.RemoveAt(currentFossilIndex);

        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            currentFossilIndex = 0;
        }

        if (correctFossilsThisRound >= fossilCollection.fossilsPerRound)
        {
            Debug.Log($"Erreicht! {correctFossilsThisRound}/{fossilCollection.fossilsPerRound} Fossilien erraten.");
            EndRound();
            return;
        }

        if (currentRoundFossils.Count == 0)
        {
            Debug.Log("Alle Fossilien erraten! Runde beendet.");
            EndRound();
            return;
        }

        ShowCurrentFossil();
        UpdateGameplayUI();
    }

    void OnSkipFossil()
    {
        if (!gameActive) return;

        if (audioSource && skipSound)
        {
            audioSource.PlayOneShot(skipSound);
        }

        FossilData skippedFossil = currentRoundFossils[currentFossilIndex];
        currentRoundFossils.RemoveAt(currentFossilIndex);
        currentRoundFossils.Add(skippedFossil);

        if (currentFossilIndex >= currentRoundFossils.Count)
        {
            currentFossilIndex = 0;
        }

        ShowCurrentFossil();
        UpdateGameplayUI();
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
            else if (winnerTeamImage && fossilCollection.team1Image) // Legacy-Fallback
            {
                winnerTeamImage.sprite = fossilCollection.team1Image;
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
            else if (winnerTeamImage && fossilCollection.team2Image) // Legacy-Fallback
            {
                winnerTeamImage.sprite = fossilCollection.team2Image;
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
        if (GameDataManager.Instance != null && fossilCollection != null)
        {
            GameDataManager.Instance.SaveRoomResult(
                fossilCollection.GetCollectionName(currentLanguage),
                fossilCollection.roomNumber,
                team1Score,
                team2Score,
                fossilCollection.fossilsPerRound
            );
        }
    }

    void UpdateGameplayUI()
    {
        // NEU: Verwende TeamIconProvider
        UpdateTeamIconForImage(currentTeamImage, currentTeam);
        scoreText.text = $"{correctFossilsThisRound}/{fossilCollection.fossilsPerRound}";
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
        correctFossilsThisRound = 0;

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