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
    public TeamSettings teamSettings = new TeamSettings(); // Team-Schwierigkeitsgrade
    public LanguageSystem.Language selectedLanguage = LanguageSystem.Language.German_Standard; // NEU: Sprache speichern
    
    public GameProgressData()
    {
        sessionId = System.Guid.NewGuid().ToString();
        lastPlayedTime = DateTime.Now;
        teamSettings = new TeamSettings();
        selectedLanguage = LanguageSystem.Language.German_Standard; // Standard-Sprache
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
    [SerializeField] private bool enableCloudSave = false; // F�r sp�ter
    [SerializeField] private bool enableLocalBackup = true;
    
    private const string SAVE_KEY = "MuseumQuizProgress";
    private const string BACKUP_KEY = "MuseumQuizBackup";
    
    public static GameDataManager Instance { get; private set; }
    
    public GameProgressData CurrentProgress { get; private set; }
    
    // Properties f�r einfachen Zugriff auf Team-Einstellungen
    public TeamSettings TeamSettings => CurrentProgress.teamSettings;
    public DifficultyLevel Team1Difficulty => CurrentProgress.teamSettings.team1Difficulty;
    public DifficultyLevel Team2Difficulty => CurrentProgress.teamSettings.team2Difficulty;
    
    // NEU: Properties f�r Sprach-Zugriff
    public LanguageSystem.Language CurrentLanguage => CurrentProgress.selectedLanguage;
    
    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
            
            // NEU: Sprache mit LanguageSystem synchronisieren
            SyncLanguageWithSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #region Language Management (NEU)
    
    /// <summary>
    /// Sprache setzen und mit LanguageSystem synchronisieren
    /// </summary>
    public void SetLanguage(LanguageSystem.Language language)
    {
        if (CurrentProgress.selectedLanguage != language)
        {
            CurrentProgress.selectedLanguage = language;
            CurrentProgress.lastPlayedTime = DateTime.Now;
            SaveGameData();
            
            // Mit LanguageSystem synchronisieren
            if (LanguageSystem.Instance != null)
            {
                LanguageSystem.Instance.SetLanguage(language);
            }
            
            Debug.Log($"Language changed to: {language}");
        }
    }
    
    /// <summary>
    /// Sprache mit LanguageSystem synchronisieren beim Start
    /// </summary>
    private void SyncLanguageWithSystem()
    {
        if (LanguageSystem.Instance != null)
        {
            // Wenn LanguageSystem eine andere Sprache hat, diese �bernehmen
            LanguageSystem.Language systemLanguage = LanguageSystem.Instance.GetCurrentLanguage();
            if (systemLanguage != CurrentProgress.selectedLanguage)
            {
                CurrentProgress.selectedLanguage = systemLanguage;
                SaveGameData();
            }
            else
            {
                // GameDataManager-Sprache an LanguageSystem weitergeben
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
        
        // Entferne vorhandenes Ergebnis f�r diesen Raum
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
    
    // Team-Schwierigkeitsgrad-Management
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
        LanguageSystem.Language currentLang = CurrentProgress.selectedLanguage; // Sprache beibehalten
        CurrentProgress = new GameProgressData();
        CurrentProgress.selectedLanguage = currentLang; // Sprache wiederherstellen
        SaveGameData();
        Debug.Log("Game progress reset (language preserved)");
    }
    
    #endregion
    
    #region Save/Load System
    
    void SaveGameData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(CurrentProgress, true);
            
            // Prim�re Speicherung
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
            
            // Versuche prim�re Daten zu laden
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
                
                // Sicherstellen, dass TeamSettings existieren (f�r bestehende Saves)
                if (CurrentProgress.teamSettings == null)
                {
                    CurrentProgress.teamSettings = new TeamSettings();
                }
                
                // NEU: Sicherstellen, dass selectedLanguage gesetzt ist (f�r bestehende Saves)
                // Bei alten Saves ist selectedLanguage = 0 (German_Standard), was perfekt ist
                
                Debug.Log($"Game data loaded. Session: {CurrentProgress.sessionId}, Language: {CurrentProgress.selectedLanguage}");
                SaveGameData(); // Aktualisiere Save falls Migrationen stattgefunden haben
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
                 $"Team1: {Team1Difficulty}, Team2: {Team2Difficulty}, " +
                 $"Language: {CurrentProgress.selectedLanguage}");
    }
    
    [ContextMenu("Reset All Data")]
    void DebugResetData()
    {
        ResetProgress();
    }
    
    // Debug-Methoden f�r Schwierigkeitsgrade
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
    
    // NEU: Debug-Methoden f�r Sprachen
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
    
    #endregion
}