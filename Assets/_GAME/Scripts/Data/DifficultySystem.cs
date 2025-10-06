using UnityEngine;

[System.Serializable]
public enum DifficultyLevel
{
    Kids,
    BigKids,
    Adults
}

[System.Serializable]
public class TeamSettings
{
    public DifficultyLevel team1Difficulty = DifficultyLevel.Adults;
    public DifficultyLevel team2Difficulty = DifficultyLevel.Adults;
    
    public DifficultyLevel GetTeamDifficulty(int teamIndex)
    {
        return teamIndex == 0 ? team1Difficulty : team2Difficulty;
    }
    
    public void SetTeamDifficulty(int teamIndex, DifficultyLevel difficulty)
    {
        if (teamIndex == 0)
            team1Difficulty = difficulty;
        else
            team2Difficulty = difficulty;
    }
}

[System.Serializable]
public class DifficultyTimeSettings
{
    [Header("Zeitlimits nach Schwierigkeitsgrad (falls verwendet)")]
    public float kidsTimeMultiplier = 1.5f;
    public float bigKidsTimeMultiplier = 1.2f;
    public float adultsTimeMultiplier = 1.0f;
    
    public float GetTimeMultiplier(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsTimeMultiplier,
            DifficultyLevel.BigKids => bigKidsTimeMultiplier,
            DifficultyLevel.Adults => adultsTimeMultiplier,
            _ => adultsTimeMultiplier
        };
    }
}