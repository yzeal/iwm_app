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
    public TeamSettings teamSettings = new TeamSettings(); // Team-Schwierigkeitsgrade + Icons
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
    
    private const string SAVE_KEY = "MuseumQuizProgress";
    private const string BACKUP_KEY = "MuseumQuizBackup";
    
    public static GameDataManager Instance { get; private set; }
    
    public GameProgressData CurrentProgress { get; private set; }
    
    // Properties für einfachen Zugriff auf Team-Einstellungen
    public TeamSettings TeamSettings => CurrentProgress.teamSettings;
    public DifficultyLevel Team1Difficulty => CurrentProgress.teamSettings.team1Difficulty;
    public DifficultyLevel Team2Difficulty => CurrentProgress.teamSettings.team2Difficulty;
    
    // Properties für Sprach-Zugriff
    public LanguageSystem.Language CurrentLanguage => CurrentProgress.selectedLanguage;
    
    // NEU: Properties für Icon-Zugriff
    public int Team1IconIndex => CurrentProgress.teamSettings.team1IconIndex;
    public int Team2IconIndex => CurrentProgress.teamSettings.team2IconIndex;
    
    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
            
            // Sprache mit LanguageSystem synchronisieren
            SyncLanguageWithSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #region Language Management
    
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
            // Wenn LanguageSystem eine andere Sprache hat, diese übernehmen
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
    
    // NEU: Team-Icon-Management
    /// <summary>
    /// Setzt den Icon-Index für ein Team (0-2)
    /// </summary>
    public void SetTeamIconIndex(int teamIndex, int iconIndex)
    {
        CurrentProgress.teamSettings.SetTeamIconIndex(teamIndex, iconIndex);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team {teamIndex + 1} icon set to index {iconIndex}");
    }
    
    /// <summary>
    /// Holt den Icon-Index für ein Team (0-2)
    /// </summary>
    public int GetTeamIconIndex(int teamIndex)
    {
        return CurrentProgress.teamSettings.GetTeamIconIndex(teamIndex);
    }
    
    /// <summary>
    /// Setzt Icon-Indizes für beide Teams gleichzeitig
    /// </summary>
    public void SetTeamIconIndices(int team1IconIndex, int team2IconIndex)
    {
        CurrentProgress.teamSettings.team1IconIndex = Mathf.Clamp(team1IconIndex, 0, 2);
        CurrentProgress.teamSettings.team2IconIndex = Mathf.Clamp(team2IconIndex, 0, 2);
        CurrentProgress.lastPlayedTime = DateTime.Now;
        SaveGameData();
        Debug.Log($"Team icons set: Team1={team1IconIndex}, Team2={team2IconIndex}");
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
                
                // Sicherstellen, dass TeamSettings existieren (für bestehende Saves)
                if (CurrentProgress.teamSettings == null)
                {
                    CurrentProgress.teamSettings = new TeamSettings();
                }
                
                // NEU: Icon-Indizes validieren (für bestehende Saves)
                CurrentProgress.teamSettings.team1IconIndex = Mathf.Clamp(CurrentProgress.teamSettings.team1IconIndex, 0, 2);
                CurrentProgress.teamSettings.team2IconIndex = Mathf.Clamp(CurrentProgress.teamSettings.team2IconIndex, 0, 2);
                
                Debug.Log($"Game data loaded. Session: {CurrentProgress.sessionId}, Language: {CurrentProgress.selectedLanguage}, Team1Icon: {Team1IconIndex}, Team2Icon: {Team2IconIndex}");
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
                 $"Team1: {Team1Difficulty} (Icon {Team1IconIndex}), Team2: {Team2Difficulty} (Icon {Team2IconIndex}), " +
                 $"Language: {CurrentProgress.selectedLanguage}");
    }
    
    [ContextMenu("Reset All Data")]
    void DebugResetData()
    {
        ResetProgress();
    }
    
    // Debug-Methoden für Schwierigkeitsgrade
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
    
    // Debug-Methoden für Sprachen
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
    
    // NEU: Debug-Methoden für Icons
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
    
    #endregion
}