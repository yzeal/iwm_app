using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public bool hasAnswered;
    public int selectedAnswerIndex;
    public string[] shuffledAnswers;
    
    public void ResetForNewQuestion()
    {
        hasAnswered = false;
        selectedAnswerIndex = -1;
    }
    
    public void AddScore(int points)
    {
        score += points;
    }
}