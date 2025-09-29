using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplitScreenQuizManager : MonoBehaviour
{
    [Header("Quiz Data")]
    public QuizRoomData roomData;
    
    [Header("UI References")]
    public GameObject gameplayUI;
    public GameObject resultUI;
    
    [Header("Player 1 UI (Bottom)")]
    public TextMeshProUGUI player1QuestionText;
    public Button[] player1AnswerButtons;
    public TextMeshProUGUI player1ScoreText;
    public GameObject player1FeedbackPanel;
    public TextMeshProUGUI player1FeedbackText;
    
    [Header("Player 2 UI (Top - Rotated)")]
    public TextMeshProUGUI player2QuestionText;
    public Button[] player2AnswerButtons;
    public TextMeshProUGUI player2ScoreText;
    public GameObject player2FeedbackPanel;
    public TextMeshProUGUI player2FeedbackText;
    
    [Header("Shared UI")]
    public QuestionProgressIcons questionProgressIcons; // Ersetzt questionCounterText
    public Button continueButton;
    
    [Header("Result Screen")]
    public TextMeshProUGUI finalScoreText;
    public Button playAgainButton;
    public Button continueToNextRoomButton;
    
    // Game State
    private PlayerData player1 = new PlayerData { playerName = "Team 1" };
    private PlayerData player2 = new PlayerData { playerName = "Team 2" };
    private int currentQuestionIndex = 0;
    private bool bothPlayersAnswered = false;
    private bool showingFeedback = false;
    
    // Timing system for first/second answer
    private float player1AnswerTime = float.MaxValue;
    private float player2AnswerTime = float.MaxValue;
    private float questionStartTime;
    
    void Start()
    {
        InitializeQuiz();
    }
    
    void InitializeQuiz()
    {
        if (roomData == null || roomData.questions.Length == 0)
        {
            Debug.LogError("Keine Quiz-Daten gefunden!");
            return;
        }
        
        // Initialisiere das Icon-System
        if (questionProgressIcons != null)
        {
            questionProgressIcons.Initialize(roomData.questions.Length);
        }
        
        SetupUI();
        ShowCurrentQuestion();
    }
    
    void SetupUI()
    {
        // Setup button events
        for (int i = 0; i < player1AnswerButtons.Length; i++)
        {
            int index = i; // Closure capture
            player1AnswerButtons[i].onClick.AddListener(() => OnPlayer1Answer(index));
        }
        
        for (int i = 0; i < player2AnswerButtons.Length; i++)
        {
            int index = i; // Closure capture
            player2AnswerButtons[i].onClick.AddListener(() => OnPlayer2Answer(index));
        }
        
        continueButton.onClick.AddListener(NextQuestion);
        playAgainButton.onClick.AddListener(RestartQuiz);
        continueToNextRoomButton.onClick.AddListener(ContinueToNextRoom);
        
        // Hide feedback panels initially
        player1FeedbackPanel.SetActive(false);
        player2FeedbackPanel.SetActive(false);
        continueButton.gameObject.SetActive(false);
        resultUI.SetActive(false);
    }
    
    void ShowCurrentQuestion()
    {
        if (currentQuestionIndex >= roomData.questions.Length)
        {
            ShowResults();
            return;
        }
        
        QuizQuestion question = roomData.questions[currentQuestionIndex];
        
        // Reset player states
        player1.ResetForNewQuestion();
        player2.ResetForNewQuestion();
        bothPlayersAnswered = false;
        showingFeedback = false;
        
        // Reset timing
        player1AnswerTime = float.MaxValue;
        player2AnswerTime = float.MaxValue;
        questionStartTime = Time.time;
        
        // Generate different shuffled answers for each player
        player1.shuffledAnswers = question.GetShuffledAnswers();
        player2.shuffledAnswers = question.GetShuffledAnswers();
        
        // Update question texts
        player1QuestionText.text = question.questionText;
        player2QuestionText.text = question.questionText;
        
        // Update answer buttons
        UpdateAnswerButtons(player1AnswerButtons, player1.shuffledAnswers, true);
        UpdateAnswerButtons(player2AnswerButtons, player2.shuffledAnswers, true);
        
        // Update UI state
        UpdateScoreDisplay();
        
        // Hide feedback
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
            
            // Reset button colors to default
            ColorBlock colors = buttons[i].colors;
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            buttons[i].colors = colors;
            
            // Reset text color to default (schwarz)
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
        
        // Disable buttons for this player und setze halbtransparente disabled color
        foreach (var button in player1AnswerButtons)
        {
            button.interactable = false;
            
            // Setze disabled color auf halbtransparent
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f); // Halbtransparentes Weiß
            button.colors = colors;
        }
        
        // Visual feedback for selection
        HighlightSelectedButton(player1AnswerButtons[answerIndex], Color.gray);
        
        CheckIfBothAnswered();
    }
    
    void OnPlayer2Answer(int answerIndex)
    {
        if (player2.hasAnswered || showingFeedback) return;
        
        player2.hasAnswered = true;
        player2.selectedAnswerIndex = answerIndex;
        player2AnswerTime = Time.time - questionStartTime;
        
        // Disable buttons for this player und setze halbtransparente disabled color
        foreach (var button in player2AnswerButtons)
        {
            button.interactable = false;
            
            // Setze disabled color auf halbtransparent
            ColorBlock colors = button.colors;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.5f); // Halbtransparentes Weiß
            button.colors = colors;
        }
        
        // Visual feedback for selection
        HighlightSelectedButton(player2AnswerButtons[answerIndex], Color.gray);
        
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
        // Setze Button-Hintergrundfarbe für alle Zustände
        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.selectedColor = backgroundColor;
        colors.disabledColor = backgroundColor; // Wichtig: Auch disabled color setzen
        button.colors = colors;
        
        // Setze Text-Farbe
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
        QuizQuestion question = roomData.questions[currentQuestionIndex];
        
        // Check correct answers
        bool player1Correct = IsAnswerCorrect(player1, question);
        bool player2Correct = IsAnswerCorrect(player2, question);
        
        // Calculate scores based on correctness and timing
        int player1Points = CalculatePoints(player1Correct, player1AnswerTime, player2AnswerTime, player2Correct);
        int player2Points = CalculatePoints(player2Correct, player2AnswerTime, player1AnswerTime, player1Correct);
        
        // Add points
        player1.AddScore(player1Points);
        player2.AddScore(player2Points);
        
        // Show feedback
        ShowFeedback(player1, player1Points, player1Correct, player1AnswerButtons);
        ShowFeedback(player2, player2Points, player2Correct, player2AnswerButtons);
        
        // Update score display
        UpdateScoreDisplay();
        
        // Update progress icons - markiere die aktuelle Frage als abgeschlossen
        if (questionProgressIcons != null)
        {
            questionProgressIcons.MarkQuestionCompleted();
        }
        
        // Show continue button
        continueButton.gameObject.SetActive(true);
    }
    
    int CalculatePoints(bool playerCorrect, float playerTime, float otherPlayerTime, bool otherPlayerCorrect)
    {
        if (!playerCorrect) return 0;
        
        if (!otherPlayerCorrect) return 2; // Only this player was correct
        
        // Both correct - check timing
        return playerTime < otherPlayerTime ? 2 : 1;
    }
    
    bool IsAnswerCorrect(PlayerData player, QuizQuestion question)
    {
        if (player.selectedAnswerIndex < 0 || player.selectedAnswerIndex >= player.shuffledAnswers.Length)
            return false;
            
        string selectedAnswer = player.shuffledAnswers[player.selectedAnswerIndex];
        return selectedAnswer == question.GetCorrectAnswer();
    }
    
    void ShowFeedback(PlayerData player, int points, bool correct, Button[] buttons)
    {
        GameObject feedbackPanel = (player == player1) ? player1FeedbackPanel : player2FeedbackPanel;
        TextMeshProUGUI feedbackText = (player == player1) ? player1FeedbackText : player2FeedbackText;
        
        string feedbackMessage = correct 
            ? $"Richtig! +{points} Punkt{(points != 1 ? "e" : "")}"
            : "Leider falsch! +0 Punkte";
            
        feedbackText.text = feedbackMessage;
        feedbackPanel.SetActive(true);
        
        // Setze alle Buttons auf weiß mit schwarzem Text (disabled color = weiß)
        for (int i = 0; i < buttons.Length; i++)
        {
            HighlightSelectedButtonWithTextColor(buttons[i], Color.white, Color.black);
        }
        
        // Highlight selected button with final color and white text
        if (player.selectedAnswerIndex >= 0 && player.selectedAnswerIndex < buttons.Length)
        {
            Color finalColor = correct ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.2f, 0.2f);
            HighlightSelectedButtonWithTextColor(buttons[player.selectedAnswerIndex], finalColor, Color.white);
        }
    }
    
    void UpdateScoreDisplay()
    {
        player1ScoreText.text = $"Punkte: {player1.score}";
        player2ScoreText.text = $"Punkte: {player2.score}";
    }
    
    void NextQuestion()
    {
        currentQuestionIndex++;
        ShowCurrentQuestion();
    }
    
    void ShowResults()
    {
        gameplayUI.SetActive(false);
        resultUI.SetActive(true);
        
        string winnerText;
        if (player1.score > player2.score)
            winnerText = $"{player1.playerName} gewinnt!";
        else if (player2.score > player1.score)
            winnerText = $"{player2.playerName} gewinnt!";
        else
            winnerText = "Unentschieden!";
        
        finalScoreText.text = $"{winnerText}\n\n{player1.playerName}: {player1.score} Punkte\n{player2.playerName}: {player2.score} Punkte";
    }
    
    void RestartQuiz()
    {
        // Reset game state
        player1.score = 0;
        player2.score = 0;
        currentQuestionIndex = 0;
        
        // Reset progress icons
        if (questionProgressIcons != null)
        {
            questionProgressIcons.ResetProgress();
        }
        
        // Reset UI
        gameplayUI.SetActive(true);
        resultUI.SetActive(false);
        
        ShowCurrentQuestion();
    }
    
    void ContinueToNextRoom()
    {
        // TODO: Implement navigation to next room
        Debug.Log("Navigation zum nächsten Raum - noch nicht implementiert");
        
        // Placeholder für Scene-Management
        // SceneManager.LoadScene("NextRoomScene");
    }
}