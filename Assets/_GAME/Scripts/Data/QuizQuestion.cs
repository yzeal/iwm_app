using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 4)]
    public string questionText;
    
    [Header("Antworten (erste Antwort ist die richtige)")]
    public string[] answers = new string[4];
    
    public string GetCorrectAnswer() => answers[0];
    
    public string[] GetShuffledAnswers()
    {
        string[] shuffled = new string[answers.Length];
        System.Array.Copy(answers, shuffled, answers.Length);
        
        // Fisher-Yates Shuffle
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }
        
        return shuffled;
    }
    
    public int GetCorrectAnswerIndex(string[] shuffledAnswers)
    {
        string correctAnswer = GetCorrectAnswer();
        for (int i = 0; i < shuffledAnswers.Length; i++)
        {
            if (shuffledAnswers[i] == correctAnswer)
                return i;
        }
        return -1;
    }
}