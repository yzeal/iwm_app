using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SplitScreenQuizManager : MonoBehaviour
{
    [Header("Quiz Data")]
    public QuizRoomData roomData;
    [SerializeField] private string nextScene = "Auswahl";

    [Header("UI References")]
    public GameObject gameplayUI;
    public GameObject resultUI;
    
    [Header("Player 1 UI (Bottom)")]
    public TextMeshProUGUI player1QuestionText;
    public Button[] player1AnswerButtons;
    public TextMeshProUGUI player1ScoreText;
    public GameObject player1FeedbackPanel;
    public TextMeshProUGUI player1FeedbackText;
    public Image player1TeamIcon; // NEU: Dynamisches Team-Icon
    
    [Header("Player 2 UI (Top - Rotated)")]
    public TextMeshProUGUI player2QuestionText;
    public Button[] player2AnswerButtons;
    public TextMeshProUGUI player2ScoreText;
    public GameObject player2FeedbackPanel;
    public TextMeshProUGUI player2FeedbackText;
    public Image player2TeamIcon; // NEU: Dynamisches Team-Icon
    
    [Header("Shared UI")]
    public QuestionProgressIcons questionProgressIcons;
    public Button continueButton;
    
    [Header("Answer Feedback Colors")]
    [SerializeField] private Color correctAnswerColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color wrongAnswerColor = new Color(0.8f, 0.2f, 0.2f);
    [SerializeField] private Color selectedAnswerColor = Color.gray;
    
    [Header("Result Screen")]
    public TextMeshProUGUI finalScoreText;
    public Button playAgainButton;
    public Button continueToNextRoomButton;
    public Image result1TeamIcon; // NEU: Team-Icon auf Results Screen
    public Image result2TeamIcon; // NEU: Team-Icon auf Results Screen
    
    [Header("Team Icon Provider (NEU)")]
    [SerializeField] private TeamIconProvider teamIconProvider;
    
    [Header("Localized Texts")]
    [SerializeField] private LocalizedText correctAnswerLocalizedText;
    [SerializeField] private LocalizedText wrongAnswerLocalizedText;
    [SerializeField] private LocalizedText pointsLabelLocalizedText;
    [SerializeField] private LocalizedText singlePointLocalizedText;
    [SerializeField] private LocalizedText multiplePointsLocalizedText;
    [SerializeField] private LocalizedText winnerLocalizedText;
    [SerializeField] private LocalizedText tieLocalizedText;
    [SerializeField] private LocalizedText team1NameLocalizedText;
    [SerializeField] private LocalizedText team2NameLocalizedText;
    
    // Game State
    private PlayerData player1 = new PlayerData { playerName = "Team 1" };
    private PlayerData player2 = new PlayerData { playerName = "Team 2" };
    private int currentQuestionIndex = 0;
    private bool bothPlayersAnswered = false;
    private bool showingFeedback = false;
    
    // Fragen für beide Teams basierend auf Schwierigkeitsgrad
    private QuizQuestion[] team1Questions;
    private QuizQuestion[] team2Questions;
    
    // Timing system for first/second answer
    private float player1AnswerTime = float.MaxValue;
    private float player2AnswerTime = float.MaxValue;
    private float questionStartTime;
    
    // Aktuelle Sprache für Performance
    private LanguageSystem.Language currentLanguage = LanguageSystem.Language.German_Standard;
    
    void Start()
    {
        InitializeLocalization();
        InitializeQuiz();
        
        // NEU: Team-Icons setzen
        UpdateTeamIcons();
        
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
        
        UpdateLocalizedPlayerNames();
    }
    
    void UpdateLocalizedPlayerNames()
    {
        if (team1NameLocalizedText != null)
            player1.playerName = team1NameLocalizedText.GetText(currentLanguage);
        else
            player1.playerName = GetDefaultTeamName(1, currentLanguage);
            
        if (team2NameLocalizedText != null)
            player2.playerName = team2NameLocalizedText.GetText(currentLanguage);
        else
            player2.playerName = GetDefaultTeamName(2, currentLanguage);
    }
    
    string GetDefaultTeamName(int teamNumber, LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => $"Team {teamNumber}",
            LanguageSystem.Language.English_Simple => $"Team {teamNumber}",
            LanguageSystem.Language.German_Simple => $"Team {teamNumber}",
            _ => $"Team {teamNumber}"
        };
    }
    
    // NEU: Team-Icons aktualisieren
    void UpdateTeamIcons()
    {
        if (teamIconProvider == null)
        {
            Debug.LogWarning("TeamIconProvider nicht zugewiesen! Verwende Fallback.");
            return;
        }
        
        // Gameplay Icons
        if (player1TeamIcon != null)
        {
            Sprite team1Icon = teamIconProvider.GetTeam1Icon();
            if (team1Icon != null)
                player1TeamIcon.sprite = team1Icon;
        }
        
        if (player2TeamIcon != null)
        {
            Sprite team2Icon = teamIconProvider.GetTeam2Icon();
            if (team2Icon != null)
                player2TeamIcon.sprite = team2Icon;
        }
        
        // Results Screen Icons
        if (result1TeamIcon != null)
        {
            Sprite team1Icon = teamIconProvider.GetTeam1Icon();
            if (team1Icon != null)
                result1TeamIcon.sprite = team1Icon;
        }
        
        if (result2TeamIcon != null)
        {
            Sprite team2Icon = teamIconProvider.GetTeam2Icon();
            if (team2Icon != null)
                result2TeamIcon.sprite = team2Icon;
        }
        
        Debug.Log($"Team Icons updated: Team1={teamIconProvider.GetTeam1Icon()?.name}, Team2={teamIconProvider.GetTeam2Icon()?.name}");
    }
    
    void OnLanguageChanged(LanguageSystem.Language newLanguage)
    {
        currentLanguage = newLanguage;
        UpdateLocalizedPlayerNames();
        UpdateScoreDisplay();
        
        if (showingFeedback)
        {
            RefreshFeedbackTexts();
        }
    }
    
    void RefreshFeedbackTexts()
    {
        if (player1.hasAnswered)
        {
            QuizQuestion team1Question = team1Questions[currentQuestionIndex];
            bool player1Correct = IsAnswerCorrect(player1, team1Question);
            int player1Points = CalculatePoints(player1Correct, player1AnswerTime, player2AnswerTime, player2.hasAnswered && IsAnswerCorrect(player2, team2Questions[currentQuestionIndex]));
            UpdateFeedbackText(player1FeedbackText, player1Points, player1Correct);
        }
        
        if (player2.hasAnswered)
        {
            QuizQuestion team2Question = team2Questions[currentQuestionIndex];
            bool player2Correct = IsAnswerCorrect(player2, team2Question);
            int player2Points = CalculatePoints(player2Correct, player2AnswerTime, player1AnswerTime, player1.hasAnswered && IsAnswerCorrect(player1, team1Questions[currentQuestionIndex]));
            UpdateFeedbackText(player2FeedbackText, player2Points, player2Correct);
        }
    }
    
    void InitializeQuiz()
    {
        if (roomData == null)
        {
            Debug.LogError("Keine Quiz-Daten gefunden!");
            return;
        }
        
        LoadQuestionsForTeams();
        
        int maxQuestions = Mathf.Min(team1Questions.Length, team2Questions.Length);
        if (maxQuestions == 0)
        {
            Debug.LogError("Keine Fragen für die gewählten Schwierigkeitsgrade verfügbar!");
            return;
        }
        
        if (questionProgressIcons != null)
        {
            questionProgressIcons.Initialize(maxQuestions);
        }
        
        SetupUI();
        ShowCurrentQuestion();
    }
    
    void LoadQuestionsForTeams()
    {
        DifficultyLevel team1Difficulty = DifficultyLevel.Adults;
        DifficultyLevel team2Difficulty = DifficultyLevel.Adults;
        
        if (GameDataManager.Instance != null)
        {
            team1Difficulty = GameDataManager.Instance.GetTeamDifficulty(0);
            team2Difficulty = GameDataManager.Instance.GetTeamDifficulty(1);
        }
        
        team1Questions = roomData.GetQuestionsForDifficulty(team1Difficulty);
        team2Questions = roomData.GetQuestionsForDifficulty(team2Difficulty);
        
        Debug.Log($"Team 1 ({team1Difficulty}): {team1Questions.Length} Fragen, Team 2 ({team2Difficulty}): {team2Questions.Length} Fragen");
    }
    
    void SetupUI()
    {
        for (int i = 0; i < player1AnswerButtons.Length; i++)
        {
            int index = i;
            player1AnswerButtons[i].onClick.AddListener(() => OnPlayer1Answer(index));
        }
        
        for (int i = 0; i < player2AnswerButtons.Length; i++)
        {
            int index = i;
            player2AnswerButtons[i].onClick.AddListener(() => OnPlayer2Answer(index));
        }
        
        continueButton.onClick.AddListener(NextQuestion);
        playAgainButton.onClick.AddListener(RestartQuiz);
        continueToNextRoomButton.onClick.AddListener(ContinueToNextRoom);
        
        player1FeedbackPanel.SetActive(false);
        player2FeedbackPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
        resultUI.SetActive(false);
    }
    
    void ShowCurrentQuestion()
    {
        int maxQuestions = Mathf.Min(team1Questions.Length, team2Questions.Length);
        
        if (currentQuestionIndex >= maxQuestions)
        {
            ShowResults();
            return;
        }
        
        QuizQuestion team1Question = team1Questions[currentQuestionIndex];
        QuizQuestion team2Question = team2Questions[currentQuestionIndex];
        
        player1.ResetForNewQuestion();
        player2.ResetForNewQuestion();
        bothPlayersAnswered = false;
        showingFeedback = false;
        
        player1AnswerTime = float.MaxValue;
        player2AnswerTime = float.MaxValue;
        questionStartTime = Time.time;
        
        player1.shuffledAnswers = team1Question.GetShuffledAnswers(currentLanguage);
        player2.shuffledAnswers = team2Question.GetShuffledAnswers(currentLanguage);
        
        player1QuestionText.text = team1Question.GetQuestionText(currentLanguage);
        player2QuestionText.text = team2Question.GetQuestionText(currentLanguage);
        
        UpdateAnswerButtons(player1AnswerButtons, player1.shuffledAnswers, true);
        UpdateAnswerButtons(player2AnswerButtons, player2.shuffledAnswers, true);
        
        UpdateScoreDisplay();
        
        player1FeedbackPanel.SetActive(false);
        player2FeedbackPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }
    
    void UpdateAnswerButtons(Button[] buttons, string[] answers, bool interactable)
    {
        for (int i = 0; i < buttons.Length && i < answers.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];
            buttons[i].interactable = interactable;
            
            ColorBlock colors = buttons[i].colors;
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            buttons[i].colors = colors;
            
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = Color.black;
            }
        }
    }
    
    void OnPlayer1Answer(int answerIndex)
    {
        if (player1.hasAnswered || showingFeedback) return;
        
        player1.hasAnswered = true;
        player1.selectedAnswerIndex = answerIndex;
        player1AnswerTime = Time.time - questionStartTime;
        
        foreach (var button in player1AnswerButtons)
        {
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
            button.colors = colors;
        }
        
        HighlightSelectedButton(player1AnswerButtons[answerIndex], selectedAnswerColor);
        CheckIfBothAnswered();
    }
    
    void OnPlayer2Answer(int answerIndex)
    {
        if (player2.hasAnswered || showingFeedback) return;
        
        player2.hasAnswered = true;
        player2.selectedAnswerIndex = answerIndex;
        player2AnswerTime = Time.time - questionStartTime;
        
        foreach (var button in player2AnswerButtons)
        {
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
            button.colors = colors;
        }
        
        HighlightSelectedButton(player2AnswerButtons[answerIndex], selectedAnswerColor);
        CheckIfBothAnswered();
    }
    
    void HighlightSelectedButton(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }
    
    void HighlightSelectedButtonWithTextColor(Button button, Color backgroundColor, Color textColor)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.selectedColor = backgroundColor;
        colors.disabledColor = backgroundColor;
        button.colors = colors;
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = textColor;
        }
    }
    
    void CheckIfBothAnswered()
    {
        if (player1.hasAnswered && player2.hasAnswered)
        {
            bothPlayersAnswered = true;
            ProcessAnswers();
        }
    }
    
    void ProcessAnswers()
    {
        showingFeedback = true;
        
        QuizQuestion team1Question = team1Questions[currentQuestionIndex];
        QuizQuestion team2Question = team2Questions[currentQuestionIndex];
        
        bool player1Correct = IsAnswerCorrect(player1, team1Question);
        bool player2Correct = IsAnswerCorrect(player2, team2Question);
        
        int player1Points = CalculatePoints(player1Correct, player1AnswerTime, player2AnswerTime, player2Correct);
        int player2Points = CalculatePoints(player2Correct, player2AnswerTime, player1AnswerTime, player1Correct);
        
        player1.AddScore(player1Points);
        player2.AddScore(player2Points);
        
        ShowFeedback(player1, player1Points, player1Correct, player1AnswerButtons, team1Question);
        ShowFeedback(player2, player2Points, player2Correct, player2AnswerButtons, team2Question);
        
        UpdateScoreDisplay();
        
        if (questionProgressIcons != null)
        {
            questionProgressIcons.MarkQuestionCompleted();
        }
        
        continueButton.gameObject.SetActive(true);
        MakeContinueButtonInvisible();
    }
    
    void MakeContinueButtonInvisible()
    {
        Image buttonImage = continueButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = 0f;
            buttonImage.color = color;
        }
        
        TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            Color textColor = buttonText.color;
            textColor.a = 0f;
            buttonText.color = textColor;
        }
    }
    
    int CalculatePoints(bool playerCorrect, float playerTime, float otherPlayerTime, bool otherPlayerCorrect)
    {
        if (!playerCorrect) return 0;
        if (!otherPlayerCorrect) return 2;
        return playerTime < otherPlayerTime ? 2 : 1;
    }
    
    bool IsAnswerCorrect(PlayerData player, QuizQuestion question)
    {
        if (player.selectedAnswerIndex < 0 || player.selectedAnswerIndex >= player.shuffledAnswers.Length)
            return false;
            
        string selectedAnswer = player.shuffledAnswers[player.selectedAnswerIndex];
        return selectedAnswer == question.GetCorrectAnswer(currentLanguage);
    }
    
    void ShowFeedback(PlayerData player, int points, bool correct, Button[] buttons, QuizQuestion question)
    {
        GameObject feedbackPanel = (player == player1) ? player1FeedbackPanel : player2FeedbackPanel;
        TextMeshProUGUI feedbackText = (player == player1) ? player1FeedbackText : player2FeedbackText;
        
        UpdateFeedbackText(feedbackText, points, correct);
        feedbackPanel.SetActive(true);
        
        // Schritt 1: Alle Buttons zurücksetzen
        for (int i = 0; i < buttons.Length; i++)
        {
            HighlightSelectedButtonWithTextColor(buttons[i], Color.white, Color.black);
        }
        
        // Schritt 2: Finde den Index der korrekten Antwort
        string correctAnswer = question.GetCorrectAnswer(currentLanguage);
        int correctAnswerIndex = -1;
        
        for (int i = 0; i < player.shuffledAnswers.Length; i++)
        {
            if (player.shuffledAnswers[i] == correctAnswer)
            {
                correctAnswerIndex = i;
                break;
            }
        }
        
        // Schritt 3: Markiere die gewählte Antwort (falls falsch in Rot)
        if (player.selectedAnswerIndex >= 0 && player.selectedAnswerIndex < buttons.Length)
        {
            if (!correct)
            {
                // Falsche Antwort in Rot
                HighlightSelectedButtonWithTextColor(buttons[player.selectedAnswerIndex], wrongAnswerColor, Color.white);
            }
        }
        
        // Schritt 4: Markiere IMMER die richtige Antwort in Grün
        if (correctAnswerIndex >= 0 && correctAnswerIndex < buttons.Length)
        {
            HighlightSelectedButtonWithTextColor(buttons[correctAnswerIndex], correctAnswerColor, Color.white);
        }
    }
    
    void UpdateFeedbackText(TextMeshProUGUI feedbackText, int points, bool correct)
    {
        string feedbackMessage;
        
        if (correct)
        {
            string correctText = correctAnswerLocalizedText != null ? 
                correctAnswerLocalizedText.GetText(currentLanguage) : GetDefaultCorrectText(currentLanguage);
            
            string pointsText = GetPointsText(points);
            feedbackMessage = $"{correctText} +{points} {pointsText}";
        }
        else
        {
            string wrongText = wrongAnswerLocalizedText != null ? 
                wrongAnswerLocalizedText.GetText(currentLanguage) : GetDefaultWrongText(currentLanguage);
            
            string pointsText = GetPointsText(0);
            feedbackMessage = $"{wrongText} +0 {pointsText}";
        }
        
        feedbackText.text = feedbackMessage;
    }
    
    string GetPointsText(int points)
    {
        if (points == 1)
        {
            return singlePointLocalizedText != null ? 
                singlePointLocalizedText.GetText(currentLanguage) : GetDefaultSinglePointText(currentLanguage);
        }
        else
        {
            return multiplePointsLocalizedText != null ? 
                multiplePointsLocalizedText.GetText(currentLanguage) : GetDefaultMultiplePointsText(currentLanguage);
        }
    }
    
    string GetDefaultCorrectText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Correct!",
            LanguageSystem.Language.English_Simple => "Right!",
            LanguageSystem.Language.German_Simple => "Richtig!",
            _ => "Richtig!"
        };
    }
    
    string GetDefaultWrongText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Wrong!",
            LanguageSystem.Language.English_Simple => "Wrong!",
            LanguageSystem.Language.German_Simple => "Falsch!",
            _ => "Leider falsch!"
        };
    }
    
    string GetDefaultSinglePointText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Point",
            LanguageSystem.Language.English_Simple => "Point",
            LanguageSystem.Language.German_Simple => "Punkt",
            _ => "Punkt"
        };
    }
    
    string GetDefaultMultiplePointsText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Points",
            LanguageSystem.Language.English_Simple => "Points",
            LanguageSystem.Language.German_Simple => "Punkte",
            _ => "Punkte"
        };
    }
    
    void UpdateScoreDisplay()
    {
        string pointsLabel = pointsLabelLocalizedText != null ? 
            pointsLabelLocalizedText.GetText(currentLanguage) : GetDefaultPointsLabel(currentLanguage);
        
        player1ScoreText.text = $"{pointsLabel}: {player1.score}";
        player2ScoreText.text = $"{pointsLabel}: {player2.score}";
    }
    
    string GetDefaultPointsLabel(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "Points",
            LanguageSystem.Language.English_Simple => "Points",
            LanguageSystem.Language.German_Simple => "Punkte",
            _ => "Punkte"
        };
    }
    
    public void NextQuestion()
    {
        currentQuestionIndex++;
        ShowCurrentQuestion();
    }
    
    void ShowResults()
    {
        gameplayUI.SetActive(false);
        resultUI.SetActive(true);
        
        // NEU: Icons auf Results Screen aktualisieren
        UpdateTeamIcons();
        
        string winnerText;
        if (player1.score > player2.score)
        {
            string winnerFormat = winnerLocalizedText != null ? 
                winnerLocalizedText.GetText(currentLanguage) : GetDefaultWinnerText(currentLanguage);
            winnerText = string.Format(winnerFormat, player1.playerName);
        }
        else if (player2.score > player1.score)
        {
            string winnerFormat = winnerLocalizedText != null ? 
                winnerLocalizedText.GetText(currentLanguage) : GetDefaultWinnerText(currentLanguage);
            winnerText = string.Format(winnerFormat, player2.playerName);
        }
        else
        {
            winnerText = tieLocalizedText != null ? 
                tieLocalizedText.GetText(currentLanguage) : GetDefaultTieText(currentLanguage);
        }
        
        string pointsLabel = GetDefaultPointsLabel(currentLanguage);
        finalScoreText.text = $"{winnerText}\n\n{player1.playerName}: {player1.score} {pointsLabel}\n{player2.playerName}: {player2.score} {pointsLabel}";
        
        if (GameDataManager.Instance != null && roomData != null)
        {
            GameDataManager.Instance.SaveRoomResult(
                roomData.GetRoomName(currentLanguage),
                roomData.roomNumber,
                player1.score,
                player2.score,
                currentQuestionIndex
            );
        }
    }
    
    string GetDefaultWinnerText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "{0} wins!",
            LanguageSystem.Language.English_Simple => "{0} wins!",
            LanguageSystem.Language.German_Simple => "{0} gewinnt!",
            _ => "{0} gewinnt!"
        };
    }
    
    string GetDefaultTieText(LanguageSystem.Language language)
    {
        return language switch
        {
            LanguageSystem.Language.English_Standard => "It's a tie!",
            LanguageSystem.Language.English_Simple => "Tie!",
            LanguageSystem.Language.German_Simple => "Unentschieden!",
            _ => "Unentschieden!"
        };
    }
    
    void RestartQuiz()
    {
        player1.score = 0;
        player2.score = 0;
        currentQuestionIndex = 0;
        
        LoadQuestionsForTeams();
        
        if (questionProgressIcons != null)
        {
            int maxQuestions = Mathf.Min(team1Questions.Length, team2Questions.Length);
            questionProgressIcons.Initialize(maxQuestions);
            questionProgressIcons.ResetProgress();
        }
        
        gameplayUI.SetActive(true);
        resultUI.SetActive(false);
        
        // NEU: Icons neu laden (falls geändert)
        UpdateTeamIcons();
        
        ShowCurrentQuestion();
    }
    
    void ContinueToNextRoom()
    {
        SceneManager.LoadScene(nextScene);
    }
}