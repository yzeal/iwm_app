using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Verwaltet den finalen Tour-Abschluss-Screen mit Gewinner/Verlierer-Anzeige
/// und Gesamtscores beider Teams.
/// Verwendet separate UIs für Gewinner/Verlierer und Unentschieden-Szenarien.
/// </summary>
public class TourResultsManager : MonoBehaviour
{
    [Header("Team Icon Provider")]
    [SerializeField] private TeamIconProvider teamIconProvider;

    [Header("UI GameObjects")]
    [Tooltip("UI für Gewinner/Verlierer-Szenario (wird aktiviert wenn Scores unterschiedlich)")]
    [SerializeField] private GameObject winnerLoserUI;
    [Tooltip("UI für Unentschieden-Szenario (wird aktiviert wenn Scores gleich)")]
    [SerializeField] private GameObject tieUI;

    [Header("Winner/Loser UI - Team Icons")]
    [SerializeField] private Image winnerTeamIcon;
    [SerializeField] private Image loserTeamIcon;

    [Header("Winner/Loser UI - Score Display")]
    [SerializeField] private TextMeshProUGUI winnerScoreText;
    [SerializeField] private LocalizedText winnerScorePrefixLocalizedText;
    [SerializeField] private TextMeshProUGUI loserScoreText;
    [SerializeField] private LocalizedText loserScorePrefixLocalizedText;

    [Header("Tie UI - Team Icons")]
    [SerializeField] private Image tieTeam1Icon;
    [SerializeField] private Image tieTeam2Icon;

    [Header("Tie UI - Score Display")]
    [SerializeField] private TextMeshProUGUI tieTeam1ScoreText;
    [SerializeField] private LocalizedText tieTeam1ScorePrefixLocalizedText;
    [SerializeField] private TextMeshProUGUI tieTeam2ScoreText;
    [SerializeField] private LocalizedText tieTeam2ScorePrefixLocalizedText;

    [Header("Debug Settings")]
    [Tooltip("Wenn aktiviert, werden simulierte Scores statt GameDataManager-Scores verwendet")]
    [SerializeField] private bool simulateResult = false;
    [Tooltip("Simulierter Score für Team 1 (nur wenn 'Simulate Result' aktiv)")]
    [SerializeField] private int simulatedScoreTeam1 = 100;
    [Tooltip("Simulierter Score für Team 2 (nur wenn 'Simulate Result' aktiv)")]
    [SerializeField] private int simulatedScoreTeam2 = 80;

    // Cached Language
    private LanguageSystem.Language currentLanguage = LanguageSystem.Language.German_Standard;

    // Cached Scores
    private int team1TotalScore = 0;
    private int team2TotalScore = 0;
    private int winnerTeamIndex = 0; // 0 = Team 1, 1 = Team 2
    private int loserTeamIndex = 1;
    private bool isTie = false;

    void Start()
    {
        InitializeLanguage();
        LoadFinalScores();
        UpdateUI();

        // Event-Listener für Sprachwechsel
        LanguageSystem.OnLanguageChanged += OnLanguageChanged;
    }

    void OnDestroy()
    {
        LanguageSystem.OnLanguageChanged -= OnLanguageChanged;
    }

    void InitializeLanguage()
    {
        currentLanguage = LanguageSystem.Instance != null ?
            LanguageSystem.Instance.GetCurrentLanguage() : LanguageSystem.Language.German_Standard;
    }

    void OnLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;
        UpdateScoreTexts();
    }

    /// <summary>
    /// Lädt finale Gesamtscores aus GameDataManager und ermittelt Gewinner/Verlierer
    /// Falls simulateResult aktiv ist, werden simulierte Scores verwendet
    /// </summary>
    void LoadFinalScores()
    {
        if (simulateResult)
        {
            // Verwende simulierte Scores
            team1TotalScore = simulatedScoreTeam1;
            team2TotalScore = simulatedScoreTeam2;
            Debug.Log($"TourResultsManager: SIMULIERTE SCORES verwendet - Team 1: {team1TotalScore}, Team 2: {team2TotalScore}");
        }
        else
        {
            // Verwende echte Scores aus GameDataManager
            if (GameDataManager.Instance == null)
            {
                Debug.LogWarning("TourResultsManager: GameDataManager nicht gefunden! Verwende 0-Scores.");
                team1TotalScore = 0;
                team2TotalScore = 0;
                return;
            }

            team1TotalScore = GameDataManager.Instance.GetTotalScoreForTeam(0); // Team 1 (0-basiert)
            team2TotalScore = GameDataManager.Instance.GetTotalScoreForTeam(1); // Team 2 (0-basiert)
            Debug.Log($"TourResultsManager: ECHTE SCORES aus GameDataManager - Team 1: {team1TotalScore}, Team 2: {team2TotalScore}");
        }

        // Ermittle Gewinner, Verlierer oder Unentschieden
        if (team1TotalScore > team2TotalScore)
        {
            winnerTeamIndex = 0; // Team 1 gewinnt
            loserTeamIndex = 1;
            isTie = false;
        }
        else if (team2TotalScore > team1TotalScore)
        {
            winnerTeamIndex = 1; // Team 2 gewinnt
            loserTeamIndex = 0;
            isTie = false;
        }
        else
        {
            // Unentschieden
            isTie = true;
            winnerTeamIndex = 0; // Egal, wird nicht verwendet
            loserTeamIndex = 1;
        }

        Debug.Log($"TourResultsManager: Ergebnis - {(isTie ? "Unentschieden!" : $"Gewinner: Team {winnerTeamIndex + 1}")}");
    }

    /// <summary>
    /// Aktualisiert alle UI-Elemente (aktiviert richtige UI + Icons + Texte)
    /// </summary>
    void UpdateUI()
    {
        // Aktiviere richtige UI basierend auf Ergebnis
        if (winnerLoserUI != null)
            winnerLoserUI.SetActive(!isTie);

        if (tieUI != null)
            tieUI.SetActive(isTie);

        // Update Icons und Texte
        if (isTie)
        {
            UpdateTieUI();
        }
        else
        {
            UpdateWinnerLoserUI();
        }
    }

    /// <summary>
    /// Aktualisiert Winner/Loser UI (Icons + Texte)
    /// </summary>
    void UpdateWinnerLoserUI()
    {
        if (teamIconProvider == null)
        {
            Debug.LogWarning("TourResultsManager: TeamIconProvider nicht zugewiesen!");
            return;
        }

        // Gewinner-Icon
        if (winnerTeamIcon != null)
        {
            Sprite winnerIcon = teamIconProvider.GetTeamIcon(winnerTeamIndex);
            if (winnerIcon != null)
            {
                winnerTeamIcon.sprite = winnerIcon;
                winnerTeamIcon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"TourResultsManager: Kein Icon für Gewinner-Team {winnerTeamIndex + 1} gefunden!");
                winnerTeamIcon.gameObject.SetActive(false);
            }
        }

        // Verlierer-Icon
        if (loserTeamIcon != null)
        {
            Sprite loserIcon = teamIconProvider.GetTeamIcon(loserTeamIndex);
            if (loserIcon != null)
            {
                loserTeamIcon.sprite = loserIcon;
                loserTeamIcon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"TourResultsManager: Kein Icon für Verlierer-Team {loserTeamIndex + 1} gefunden!");
                loserTeamIcon.gameObject.SetActive(false);
            }
        }

        // Score-Texte
        UpdateWinnerLoserScoreTexts();

        Debug.Log($"TourResultsManager: Winner/Loser UI aktualisiert - Gewinner: Team {winnerTeamIndex + 1}, Verlierer: Team {loserTeamIndex + 1}");
    }

    /// <summary>
    /// Aktualisiert Tie UI (Icons + Texte)
    /// </summary>
    void UpdateTieUI()
    {
        if (teamIconProvider == null)
        {
            Debug.LogWarning("TourResultsManager: TeamIconProvider nicht zugewiesen!");
            return;
        }

        // Team 1 Icon
        if (tieTeam1Icon != null)
        {
            Sprite team1Icon = teamIconProvider.GetTeamIcon(0);
            if (team1Icon != null)
            {
                tieTeam1Icon.sprite = team1Icon;
                tieTeam1Icon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("TourResultsManager: Kein Icon für Team 1 gefunden!");
                tieTeam1Icon.gameObject.SetActive(false);
            }
        }

        // Team 2 Icon
        if (tieTeam2Icon != null)
        {
            Sprite team2Icon = teamIconProvider.GetTeamIcon(1);
            if (team2Icon != null)
            {
                tieTeam2Icon.sprite = team2Icon;
                tieTeam2Icon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("TourResultsManager: Kein Icon für Team 2 gefunden!");
                tieTeam2Icon.gameObject.SetActive(false);
            }
        }

        // Score-Texte
        UpdateTieScoreTexts();

        Debug.Log($"TourResultsManager: Tie UI aktualisiert - Beide Teams haben {team1TotalScore} Punkte");
    }

    /// <summary>
    /// Aktualisiert Score-Texte (nur Text-Updates, kein UI-Switching)
    /// </summary>
    void UpdateScoreTexts()
    {
        if (isTie)
        {
            UpdateTieScoreTexts();
        }
        else
        {
            UpdateWinnerLoserScoreTexts();
        }
    }

    /// <summary>
    /// Aktualisiert Winner/Loser Score-Texte mit lokalisierten Präfixen
    /// </summary>
    void UpdateWinnerLoserScoreTexts()
    {
        // Gewinner-Score
        if (winnerScoreText != null)
        {
            string winnerPrefix = GetLocalizedText(winnerScorePrefixLocalizedText, "Gewinner");
            int winnerScore = winnerTeamIndex == 0 ? team1TotalScore : team2TotalScore;
            winnerScoreText.text = $"{winnerPrefix} {winnerScore}";
        }

        // Verlierer-Score
        if (loserScoreText != null)
        {
            string loserPrefix = GetLocalizedText(loserScorePrefixLocalizedText, "Zweiter Platz");
            int loserScore = loserTeamIndex == 0 ? team1TotalScore : team2TotalScore;
            loserScoreText.text = $"{loserPrefix} {loserScore}";
        }
    }

    /// <summary>
    /// Aktualisiert Tie Score-Texte mit lokalisierten Präfixen
    /// </summary>
    void UpdateTieScoreTexts()
    {
        // Team 1 Score
        if (tieTeam1ScoreText != null)
        {
            string team1Prefix = GetLocalizedText(tieTeam1ScorePrefixLocalizedText, "Team 1");
            tieTeam1ScoreText.text = $"{team1Prefix} {team1TotalScore}";
        }

        // Team 2 Score
        if (tieTeam2ScoreText != null)
        {
            string team2Prefix = GetLocalizedText(tieTeam2ScorePrefixLocalizedText, "Team 2");
            tieTeam2ScoreText.text = $"{team2Prefix} {team2TotalScore}";
        }
    }

    /// <summary>
    /// Hilfsmethode zum Abrufen lokalisierter Texte mit Fallback
    /// </summary>
    string GetLocalizedText(LocalizedText localizedText, string fallback)
    {
        if (localizedText != null)
            return localizedText.GetText(currentLanguage);
        return fallback;
    }

    #region Debug Methods

    /// <summary>
    /// Lädt Scores neu und aktualisiert UI (nützlich für Live-Testing im Editor)
    /// </summary>
    [ContextMenu("Refresh Scores")]
    void RefreshScores()
    {
        LoadFinalScores();
        UpdateUI();
        Debug.Log($"TourResultsManager: Scores aktualisiert - Team 1: {team1TotalScore}, Team 2: {team2TotalScore}, Tie: {isTie}");
    }

    /// <summary>
    /// Debug-Methode zum Testen mit simulierten Scores
    /// </summary>
    [ContextMenu("Test mit simulierten Scores")]
    void TestWithMockScores()
    {
        team1TotalScore = Random.Range(50, 150);
        team2TotalScore = Random.Range(50, 150);

        if (team1TotalScore > team2TotalScore)
        {
            winnerTeamIndex = 0;
            loserTeamIndex = 1;
            isTie = false;
        }
        else if (team2TotalScore > team1TotalScore)
        {
            winnerTeamIndex = 1;
            loserTeamIndex = 0;
            isTie = false;
        }
        else
        {
            isTie = true;
        }

        Debug.Log($"TourResultsManager TEST: Team 1: {team1TotalScore}, Team 2: {team2TotalScore}, Tie: {isTie}");
        UpdateUI();
    }

    /// <summary>
    /// Debug-Methode zum Testen eines Unentschiedens
    /// </summary>
    [ContextMenu("Test Unentschieden")]
    void TestTie()
    {
        int score = Random.Range(80, 120);
        team1TotalScore = score;
        team2TotalScore = score;
        isTie = true;

        Debug.Log($"TourResultsManager TEST TIE: Beide Teams haben {score} Punkte");
        UpdateUI();
    }

    /// <summary>
    /// Debug-Methode zum Testen eines klaren Gewinners
    /// </summary>
    [ContextMenu("Test Gewinner (Team 1)")]
    void TestWinnerTeam1()
    {
        team1TotalScore = Random.Range(100, 150);
        team2TotalScore = Random.Range(50, 99);
        winnerTeamIndex = 0;
        loserTeamIndex = 1;
        isTie = false;

        Debug.Log($"TourResultsManager TEST WINNER: Team 1: {team1TotalScore}, Team 2: {team2TotalScore}");
        UpdateUI();
    }

    /// <summary>
    /// Debug-Methode: Aktiviert Simulation mit aktuellen Inspector-Werten
    /// </summary>
    [ContextMenu("Apply Simulated Scores")]
    void ApplySimulatedScores()
    {
        if (!simulateResult)
        {
            Debug.LogWarning("TourResultsManager: 'Simulate Result' ist nicht aktiviert! Aktiviere es im Inspector.");
            return;
        }

        LoadFinalScores();
        UpdateUI();
        Debug.Log($"TourResultsManager: Simulierte Scores angewendet - Team 1: {simulatedScoreTeam1}, Team 2: {simulatedScoreTeam2}");
    }

    /// <summary>
    /// Debug-Methode: Setzt Simulation zurück und lädt echte Scores
    /// </summary>
    [ContextMenu("Reset to Real Scores")]
    void ResetToRealScores()
    {
        simulateResult = false;
        LoadFinalScores();
        UpdateUI();
        Debug.Log("TourResultsManager: Simulation deaktiviert, echte Scores geladen");
    }

    #endregion
}