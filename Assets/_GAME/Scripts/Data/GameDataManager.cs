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
    
    public GameProgressData()
    {
        sessionId = System.Guid.NewGuid().ToString();
        lastPlayedTime = DateTime.Now;
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
                 $"Session: {CurrentProgress.sessionId}");
    }
    
    [ContextMenu("Reset All Data")]
    void DebugResetData()
    {
        ResetProgress();
    }
    
    #endregion
}