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
    public TeamSettings teamSettings = new TeamSettings(); // NEU: Team-Schwierigkeitsgrade
    
    public GameProgressData()
    {
        sessionId = System.Guid.NewGuid().ToString();
        lastPlayedTime = DateTime.Now;
        teamSettings = new TeamSettings(); // Sicherstellen, dass TeamSettings initialisiert sind
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
    [SerializeField] private bool enableCloudSave = false; // Für später
    [SerializeField] private bool enableLocalBackup = true;
    
    private const string SAVE_KEY = "MuseumQuizProgress";
    private const string BACKUP_KEY = "MuseumQuizBackup";
    
    public static GameDataManager Instance { get; private set; }
    
    public GameProgressData CurrentProgress { get; private set; }
    
    // NEU: Properties für einfachen Zugriff auf Team-Einstellungen
    public TeamSettings TeamSettings => CurrentProgress.teamSettings;
    public DifficultyLevel Team1Difficulty => CurrentProgress.teamSettings.team1Difficulty;
    public DifficultyLevel Team2Difficulty => CurrentProgress.teamSettings.team2Difficulty;
    
    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
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
        CurrentProgress.roomResults.RemoveAll(r => r.roomNumber == roomNumber);
        CurrentProgress.roomResults.Add(result);
        
        CurrentProgress.lastPlayedTime = DateTime.Now;
        
        SaveGameData();
        
        Debug.Log($"Saved result for {roomName}: Team1={player1Score}, Team2={player2Score}");
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
    
    // NEU: Team-Schwierigkeitsgrad-Management
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
        CurrentProgress = new GameProgressData();
        SaveGameData();
        Debug.Log("Game progress reset");
    }
    
    #endregion
    
    #region Save/Load System
    
    void SaveGameData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(CurrentProgress, true);
            
            // Primäre Speicherung
            PlayerPrefs.SetString(SAVE_KEY, jsonData);
            
            // Backup-Speicherung
            if (enableLocalBackup)
            {
                PlayerPrefs.SetString(BACKUP_KEY, jsonData);
            }
            
            PlayerPrefs.Save();
            
            Debug.Log("Game data saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
        }
    }
    
    void LoadGameData()
    {
        try
        {
            string jsonData = "";
            
            // Versuche primäre Daten zu laden
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                jsonData = PlayerPrefs.GetString(SAVE_KEY);
            }
            // Fallback auf Backup
            else if (enableLocalBackup && PlayerPrefs.HasKey(BACKUP_KEY))
            {
                jsonData = PlayerPrefs.GetString(BACKUP_KEY);
                Debug.Log("Loaded from backup data");
            }
            
            if (!string.IsNullOrEmpty(jsonData))
            {
                CurrentProgress = JsonUtility.FromJson<GameProgressData>(jsonData);
                
                // NEU: Sicherstellen, dass TeamSettings existieren (für bestehende Saves)
                if (CurrentProgress.teamSettings == null)
                {
                    CurrentProgress.teamSettings = new TeamSettings();
                    SaveGameData(); // Aktualisiere den Save mit den neuen TeamSettings
                }
                
                Debug.Log($"Game data loaded. Session: {CurrentProgress.sessionId}");
            }
            else
            {
                CurrentProgress = new GameProgressData();
                Debug.Log("No save data found, created new progress");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
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
                 $"Team1: {Team1Difficulty}, Team2: {Team2Difficulty}");
    }
    
    [ContextMenu("Reset All Data")]
    void DebugResetData()
    {
        ResetProgress();
    }
    
    // NEU: Debug-Methoden für Schwierigkeitsgrade
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
    
    #endregion
}