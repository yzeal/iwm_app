using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class GameProgressData
{
    public string playerName = "";
    public List<RoomResult> roomResults = new List<RoomResult>();
    public int currentRoomIndex = 0;
    public DateTime lastPlayedTime;
    public string sessionId;
    public TeamSettings teamSettings = new TeamSettings();
    public LanguageSystem.Language selectedLanguage = LanguageSystem.Language.German_Standard;
    
    public GameProgressData()
    {
        sessionId = System.Guid.NewGuid().ToString();
        lastPlayedTime = DateTime.Now;
        teamSettings = new TeamSettings();
        selectedLanguage = LanguageSystem.Language.German_Standard;
    }
}

[System.Serializable]
public class RoomResult
{
    public string roomName;
    public int roomNumber;
    public int player1Score;
    public int player2Score;
    public int totalQuestions;
    public DateTime completedTime;
    public bool isCompleted;
}

public class GameDataManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool enableCloudSave = false;
    [SerializeField] private bool enableLocalBackup = true;
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableVerboseLogging = false; // NEU: Detaillierte Logs
    
    private const string SAVE_KEY = "MuseumQuizProgress";
    private const string BACKUP_KEY = "MuseumQuizBackup";
    
    public static GameDataManager Instance { get; private set; }
    
    public GameProgressData CurrentProgress { get; private set; }
    
    public TeamSettings TeamSettings => CurrentProgress.teamSettings;
    public DifficultyLevel Team1Difficulty => CurrentProgress.teamSettings.team1Difficulty;
    public DifficultyLevel Team2Difficulty => CurrentProgress.teamSettings.team2Difficulty;
    public LanguageSystem.Language CurrentLanguage => CurrentProgress.selectedLanguage;
    public int Team1IconIndex => CurrentProgress.teamSettings.team1IconIndex;
    public int Team2IconIndex => CurrentProgress.teamSettings.team2IconIndex;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
            SyncLanguageWithSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #region Language Management
    
    public void SetLanguage(LanguageSystem.Language language)
    {
        if (CurrentProgress.selectedLanguage != language)
        {
            CurrentProgress.selectedLanguage = language;
            CurrentProgress.lastPlayedTime = DateTime.Now;
            SaveGameData();
            
            if (LanguageSystem.Instance != null)
            {
                LanguageSystem.Instance.SetLanguage(language);
            }
            
            Debug.Log($"Language changed to: {language}");
        }
    }
    
    private void SyncLanguageWithSystem()
    {
        if (LanguageSystem.Instance != null)
        {
            LanguageSystem.Language systemLanguage = LanguageSystem.Instance.GetCurrentLanguage();
            if (systemLanguage != CurrentProgress.selectedLanguage)
            {
                CurrentProgress.selectedLanguage = systemLanguage;
                SaveGameData();
            }
            else
            {
                LanguageSystem.Instance.SetLanguage(CurrentProgress.selectedLanguage);
            }
        }
    }
    
    #endregion
    
    #region Public Methods
    
    public void SaveRoomResult(string roomName, int roomNumber, int player1Score, int player2Score, int totalQuestions)
    {
        RoomResult result = new RoomResult
        {
            roomName = roomName,
            roomNumber = roomNumber,
            player1Score = player1Score,
            player2Score = player2Score,
            totalQuestions = totalQuestions,
            completedTime = DateTime.Now,
            isCompleted = true
        };
        
        // Entferne vorhandenes Ergebnis für diesen Raum
        int removedCount = CurrentProgress.roomResults.RemoveAll(r => r.roomNumber == roomNumber);
        
        // DEBUG: Log wenn altes Ergebnis überschrieben wird
        if (removedCount > 0 && enableVerboseLogging)
        {
            Debug.Log($"[SAVE] Überschreibe altes Ergebnis für Raum {roomNumber}");
        }
        
        CurrentProgress.roomResults.Add(result);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        
        // DEBUG: Detailliertes Logging
        if (enableVerboseLogging)
        {
            Debug.Log($"[SAVE] ? Raum '{roomName}' (#{roomNumber}) gespeichert: Team1={player1Score}, Team2={player2Score}, Fragen={totalQuestions}");
            Debug.Log($"[SAVE] Gesamt gespeicherte Räume: {CurrentProgress.roomResults.Count}");
            
            // Zeige alle gespeicherten Ergebnisse
            for (int i = 0; i < CurrentProgress.roomResults.Count; i++)
            {
                var r = CurrentProgress.roomResults[i];
                Debug.Log($"[SAVE]   Raum {i + 1}: '{r.roomName}' (#{r.roomNumber}) - Team1={r.player1Score}, Team2={r.player2Score}, Completed={r.isCompleted}");
            }
        }
    }
    
    public void SetCurrentRoom(int roomIndex)
    {
        CurrentProgress.currentRoomIndex = roomIndex;
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
    }
    
    public void SetPlayerName(string name)
    {
        CurrentProgress.playerName = name;
        SaveGameData();
    }
    
    public void SetTeamDifficulty(int teamIndex, DifficultyLevel difficulty)
    {
        CurrentProgress.teamSettings.SetTeamDifficulty(teamIndex, difficulty);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team {teamIndex + 1} difficulty set to {difficulty}");
    }
    
    public DifficultyLevel GetTeamDifficulty(int teamIndex)
    {
        return CurrentProgress.teamSettings.GetTeamDifficulty(teamIndex);
    }
    
    public void SetTeamDifficulties(DifficultyLevel team1, DifficultyLevel team2)
    {
        CurrentProgress.teamSettings.team1Difficulty = team1;
        CurrentProgress.teamSettings.team2Difficulty = team2;
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team difficulties set: Team1={team1}, Team2={team2}");
    }
    
    public void SetTeamIconIndex(int teamIndex, int iconIndex)
    {
        CurrentProgress.teamSettings.SetTeamIconIndex(teamIndex, iconIndex);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team {teamIndex + 1} icon set to index {iconIndex}");
    }
    
    public int GetTeamIconIndex(int teamIndex)
    {
        return CurrentProgress.teamSettings.GetTeamIconIndex(teamIndex);
    }
    
    public void SetTeamIconIndices(int team1IconIndex, int team2IconIndex)
    {
        CurrentProgress.teamSettings.team1IconIndex = Mathf.Clamp(team1IconIndex, 0, 2);
        CurrentProgress.teamSettings.team2IconIndex = Mathf.Clamp(team2IconIndex, 0, 2);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team icons set: Team1={team1IconIndex}, Team2={team2IconIndex}");
    }
    
    /// <summary>
    /// Berechnet den Gesamtscore für ein Team über alle abgeschlossenen Räume
    /// </summary>
    public int GetTotalScoreForTeam(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex > 1)
        {
            Debug.LogWarning($"[SCORE] GetTotalScoreForTeam: Ungültiger teamIndex {teamIndex}. Verwende 0 oder 1.");
            return 0;
        }

        int totalScore = 0;
        int roomsProcessed = 0;

        // DEBUG: Header
        if (enableVerboseLogging)
        {
            Debug.Log($"[SCORE] ========== GetTotalScoreForTeam({teamIndex}) START ==========");
            Debug.Log($"[SCORE] Gespeicherte RoomResults: {CurrentProgress.roomResults.Count}");
        }

        foreach (var result in CurrentProgress.roomResults)
        {
            int scoreToAdd = teamIndex == 0 ? result.player1Score : result.player2Score;
            
            // DEBUG: Jedes Raum-Ergebnis loggen
            if (enableVerboseLogging)
            {
                Debug.Log($"[SCORE]   Raum '{result.roomName}' (#{result.roomNumber}): " +
                         $"Team1={result.player1Score}, Team2={result.player2Score}, " +
                         $"Completed={result.isCompleted}, " +
                         $"Adding {scoreToAdd} to Team{teamIndex + 1}");
            }
            
            if (result.isCompleted)
            {
                totalScore += scoreToAdd;
                roomsProcessed++;
            }
            else if (enableVerboseLogging)
            {
                Debug.LogWarning($"[SCORE]   ? Raum {result.roomNumber} ist NICHT completed! Score wird NICHT addiert.");
            }
        }

        // DEBUG: Finale Zusammenfassung
        if (enableVerboseLogging)
        {
            Debug.Log($"[SCORE] ========== GetTotalScoreForTeam({teamIndex}) ENDE ==========");
            Debug.Log($"[SCORE] Team {teamIndex + 1} Gesamtscore: {totalScore} Punkte über {roomsProcessed}/{CurrentProgress.roomResults.Count} Räume");
        }
        else
        {
            Debug.Log($"[SCORE] Team {teamIndex + 1}: {totalScore} Punkte über {roomsProcessed} Räume");
        }

        return totalScore;
    }

    /// <summary>
    /// Gibt den Score beider Teams als Tuple zurück
    /// </summary>
    public (int team1Score, int team2Score) GetTotalScores()
    {
        int team1Total = 0;
        int team2Total = 0;
        int roomsProcessed = 0;

        if (enableVerboseLogging)
        {
            Debug.Log($"[SCORE] ========== GetTotalScores() START ==========");
        }

        foreach (var result in CurrentProgress.roomResults)
        {
            if (enableVerboseLogging)
            {
                Debug.Log($"[SCORE]   Raum '{result.roomName}': Team1={result.player1Score}, Team2={result.player2Score}, Completed={result.isCompleted}");
            }
            
            if (result.isCompleted)
            {
                team1Total += result.player1Score;
                team2Total += result.player2Score;
                roomsProcessed++;
            }
        }

        if (enableVerboseLogging)
        {
            Debug.Log($"[SCORE] ========== GetTotalScores() ENDE ==========");
            Debug.Log($"[SCORE] Team 1: {team1Total} | Team 2: {team2Total} | Räume: {roomsProcessed}");
        }

        return (team1Total, team2Total);
    }
    
    public RoomResult GetRoomResult(int roomNumber)
    {
        return CurrentProgress.roomResults.Find(r => r.roomNumber == roomNumber);
    }
    
    public bool IsRoomCompleted(int roomNumber)
    {
        var result = GetRoomResult(roomNumber);
        return result != null && result.isCompleted;
    }
    
    public void ResetProgress()
    {
        LanguageSystem.Language currentLang = CurrentProgress.selectedLanguage;
        CurrentProgress = new GameProgressData();
        CurrentProgress.selectedLanguage = currentLang;
        SaveGameData();
        Debug.Log("[RESET] Game progress reset (language preserved)");
    }
    
    #endregion
    
    #region Save/Load System
    
    void SaveGameData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(CurrentProgress, true);
            
            PlayerPrefs.SetString(SAVE_KEY, jsonData);
            
            if (enableLocalBackup)
            {
                PlayerPrefs.SetString(BACKUP_KEY, jsonData);
            }
            
            PlayerPrefs.Save();
            
            if (enableVerboseLogging)
            {
                Debug.Log($"[SAVE] ? Game data saved successfully ({CurrentProgress.roomResults.Count} rooms)");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SAVE] ? Failed to save game data: {e.Message}");
        }
    }
    
    void LoadGameData()
    {
        try
        {
            string jsonData = "";
            
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                jsonData = PlayerPrefs.GetString(SAVE_KEY);
            }
            else if (enableLocalBackup && PlayerPrefs.HasKey(BACKUP_KEY))
            {
                jsonData = PlayerPrefs.GetString(BACKUP_KEY);
                Debug.Log("[LOAD] Loaded from backup data");
            }
            
            if (!string.IsNullOrEmpty(jsonData))
            {
                CurrentProgress = JsonUtility.FromJson<GameProgressData>(jsonData);
                
                if (CurrentProgress.teamSettings == null)
                {
                    CurrentProgress.teamSettings = new TeamSettings();
                }
                
                CurrentProgress.teamSettings.team1IconIndex = Mathf.Clamp(CurrentProgress.teamSettings.team1IconIndex, 0, 2);
                CurrentProgress.teamSettings.team2IconIndex = Mathf.Clamp(CurrentProgress.teamSettings.team2IconIndex, 0, 2);
                
                Debug.Log($"[LOAD] ? Game data loaded. Session: {CurrentProgress.sessionId}, " +
                         $"Language: {CurrentProgress.selectedLanguage}, " +
                         $"Rooms: {CurrentProgress.roomResults.Count}, " +
                         $"Team1Icon: {Team1IconIndex}, Team2Icon: {Team2IconIndex}");
                
                SaveGameData();
            }
            else
            {
                CurrentProgress = new GameProgressData();
                Debug.Log("[LOAD] No save data found, created new progress");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[LOAD] ? Failed to load game data: {e.Message}");
            CurrentProgress = new GameProgressData();
        }
    }
    
    #endregion
    
    #region Platform Detection
    
    public bool IsWebGL()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer;
    }
    
    public bool IsMobile()
    {
        return Application.platform == RuntimePlatform.Android || 
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Print Current Progress")]
    void PrintProgress()
    {
        Debug.Log($"Progress: Room {CurrentProgress.currentRoomIndex}, " +
                 $"Completed Rooms: {CurrentProgress.roomResults.Count}, " +
                 $"Session: {CurrentProgress.sessionId}, " +
                 $"Team1: {Team1Difficulty} (Icon {Team1IconIndex}), Team2: {Team2Difficulty} (Icon {Team2IconIndex}), " +
                 $"Language: {CurrentProgress.selectedLanguage}");
    }
    
    [ContextMenu("Print ALL Room Results")]
    void PrintAllRoomResults()
    {
        Debug.Log($"========== ALLE RAUM-ERGEBNISSE ({CurrentProgress.roomResults.Count}) ==========");
        for (int i = 0; i < CurrentProgress.roomResults.Count; i++)
        {
            var r = CurrentProgress.roomResults[i];
            Debug.Log($"Raum {i + 1}: '{r.roomName}' (#{r.roomNumber})\n" +
                     $"  Team 1: {r.player1Score} | Team 2: {r.player2Score}\n" +
                     $"  Fragen: {r.totalQuestions} | Completed: {r.isCompleted}\n" +
                     $"  Zeit: {r.completedTime}");
        }
        Debug.Log("=".PadRight(50, '='));
    }
    
    [ContextMenu("Reset All Data")]
    void DebugResetData()
    {
        ResetProgress();
    }
    
    [ContextMenu("Set Both Teams to Kids")]
    void DebugSetKids()
    {
        SetTeamDifficulties(DifficultyLevel.Kids, DifficultyLevel.Kids);
    }
    
    [ContextMenu("Set Mixed Difficulties")]
    void DebugSetMixed()
    {
        SetTeamDifficulties(DifficultyLevel.Kids, DifficultyLevel.Adults);
    }
    
    [ContextMenu("Set Language to English")]
    void DebugSetEnglish()
    {
        SetLanguage(LanguageSystem.Language.English_Standard);
    }
    
    [ContextMenu("Set Language to German Simple")]
    void DebugSetGermanSimple()
    {
        SetLanguage(LanguageSystem.Language.German_Simple);
    }
    
    [ContextMenu("Set Team 1 Icon to 2")]
    void DebugSetTeam1Icon2()
    {
        SetTeamIconIndex(0, 2);
    }
    
    [ContextMenu("Set Team 2 Icon to 1")]
    void DebugSetTeam2Icon1()
    {
        SetTeamIconIndex(1, 1);
    }

    [ContextMenu("Print Total Scores")]
    void DebugPrintTotalScores()
    {
        var (team1, team2) = GetTotalScores();
        Debug.Log($"=== TOTAL SCORES ===\nTeam 1: {team1}\nTeam 2: {team2}");
    }

    [ContextMenu("Add Test Room Result")]
    void DebugAddTestRoomResult()
    {
        int randomRoom = UnityEngine.Random.Range(1, 7);
        int team1Score = UnityEngine.Random.Range(5, 20);
        int team2Score = UnityEngine.Random.Range(5, 20);
        SaveRoomResult($"Test Room {randomRoom}", randomRoom, team1Score, team2Score, 10);
        Debug.Log($"[DEBUG] Added test result: Room {randomRoom}, Team1={team1Score}, Team2={team2Score}");
    }
    
    #endregion
}