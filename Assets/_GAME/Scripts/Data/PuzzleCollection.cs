using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Puzzle Collection", menuName = "Quiz Game/Puzzle Collection")]
public class PuzzleCollection : ScriptableObject
{
    [Header("Collection Info")]
    [Tooltip("Localized name of this puzzle collection")]
    public LocalizedText collectionName;
    
    [Tooltip("Unique room number for saving progress (must be unique across all collections!)")]
    public int roomNumber = 0; // NEU: Für GameDataManager
    
    [Header("Game Settings")]
    [Tooltip("Number of rounds per team (both teams get equal rounds)")]
    [Range(1, 10)]
    public int roundsPerTeam = 3;
    
    [Tooltip("Starting points per puzzle (before hints are used)")]
    [Range(1, 10)]
    public int startingPoints = 3;
    
    [Tooltip("Points deducted per hint used")]
    [Range(1, 5)]
    public int hintPenalty = 1;
    
    [Header("Timer Settings")]
    [Tooltip("Base round duration in seconds (multiplied by difficulty)")]
    [Range(30f, 300f)]
    public float roundDuration = 120f;
    
    public DifficultyTimeSettings timeSettings;
    
    [Header("Timer Toggle per Difficulty")]
    [Tooltip("If disabled, Kids play without timer (UI hidden)")]
    public bool useTimerForKids = false;
    
    [Tooltip("If disabled, BigKids play without timer (UI hidden)")]
    public bool useTimerForBigKids = true;
    
    [Tooltip("If disabled, Adults play without timer (UI hidden)")]
    public bool useTimerForAdults = true;
    
    [Header("Puzzle Pools (Separate per Difficulty)")]
    [Tooltip("Puzzles for Kids difficulty")]
    public PuzzlePiece[] kidsPuzzles;
    
    [Tooltip("Puzzles for BigKids difficulty")]
    public PuzzlePiece[] bigKidsPuzzles;
    
    [Tooltip("Puzzles for Adults difficulty")]
    public PuzzlePiece[] adultsPuzzles;
    
    [Header("Team Images")]
    public Sprite team1Image;
    public Sprite team2Image;
    
    // ============================================
    // PUBLIC METHODS
    // ============================================
    
    /// <summary>
    /// Gets the localized collection name.
    /// </summary>
    public string GetCollectionName(LanguageSystem.Language language)
    {
        if (collectionName != null)
        {
            return collectionName.GetText(language);
        }
        
        // Fallback to asset name
        return name;
    }
    
    /// <summary>
    /// Gets the puzzle pool for the specified difficulty.
    /// </summary>
    public PuzzlePiece[] GetPuzzlesForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsPuzzles,
            DifficultyLevel.BigKids => bigKidsPuzzles,
            DifficultyLevel.Adults => adultsPuzzles,
            _ => kidsPuzzles
        };
    }
    
    /// <summary>
    /// Gets random puzzles from the pool for the specified difficulty.
    /// Returns unique puzzles (no duplicates).
    /// </summary>
    public List<PuzzlePiece> GetRandomPuzzles(int count, DifficultyLevel difficulty)
    {
        PuzzlePiece[] pool = GetPuzzlesForDifficulty(difficulty);
        
        if (pool == null || pool.Length == 0)
        {
            Debug.LogError($"No puzzles available for difficulty {difficulty}!");
            return new List<PuzzlePiece>();
        }
        
        // Shuffle pool and take requested count
        List<PuzzlePiece> shuffled = pool.OrderBy(x => Random.value).ToList();
        
        // Clamp count to available puzzles
        int actualCount = Mathf.Min(count, shuffled.Count);
        
        return shuffled.Take(actualCount).ToList();
    }
    
    /// <summary>
    /// Gets random puzzles excluding specific puzzles (for team exclusion).
    /// </summary>
    public List<PuzzlePiece> GetRandomPuzzlesExcluding(int count, DifficultyLevel difficulty, List<PuzzlePiece> excludePuzzles)
    {
        PuzzlePiece[] pool = GetPuzzlesForDifficulty(difficulty);
        
        if (pool == null || pool.Length == 0)
        {
            Debug.LogError($"No puzzles available for difficulty {difficulty}!");
            return new List<PuzzlePiece>();
        }
        
        // Filter out excluded puzzles
        List<PuzzlePiece> availablePuzzles = pool.Where(p => !excludePuzzles.Contains(p)).ToList();
        
        if (availablePuzzles.Count == 0)
        {
            Debug.LogWarning($"Not enough unique puzzles for difficulty {difficulty}! Reusing pool.");
            availablePuzzles = pool.ToList();
        }
        
        // Shuffle and take requested count
        List<PuzzlePiece> shuffled = availablePuzzles.OrderBy(x => Random.value).ToList();
        int actualCount = Mathf.Min(count, shuffled.Count);
        
        return shuffled.Take(actualCount).ToList();
    }
    
    /// <summary>
    /// Gets the adjusted round duration based on difficulty.
    /// Returns 0 if timer is disabled for this difficulty.
    /// </summary>
    public float GetAdjustedRoundDuration(DifficultyLevel difficulty)
    {
        // Check if timer is disabled for this difficulty
        if (!UseTimerForDifficulty(difficulty))
        {
            return 0f; // 0 = no timer
        }
        
        if (timeSettings == null)
        {
            return roundDuration;
        }
        
        // FIXED: Use correct property names from DifficultyTimeSettings
        float multiplier = difficulty switch
        {
            DifficultyLevel.Kids => timeSettings.kidsTimeMultiplier,
            DifficultyLevel.BigKids => timeSettings.bigKidsTimeMultiplier,
            DifficultyLevel.Adults => timeSettings.adultsTimeMultiplier,
            _ => 1f
        };
        
        return roundDuration * multiplier;
    }
    
    /// <summary>
    /// Checks if timer should be used for the specified difficulty.
    /// </summary>
    public bool UseTimerForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => useTimerForKids,
            DifficultyLevel.BigKids => useTimerForBigKids,
            DifficultyLevel.Adults => useTimerForAdults,
            _ => true
        };
    }
    
    /// <summary>
    /// Gets the team image for the specified team number (1 or 2).
    /// </summary>
    public Sprite GetTeamImage(int teamNumber)
    {
        return teamNumber == 1 ? team1Image : team2Image;
    }
    
    // ============================================
    // VALIDATION
    // ============================================
    
    /// <summary>
    /// Checks if puzzle pool has enough puzzles for the configured rounds.
    /// </summary>
    public bool HasEnoughPuzzles(DifficultyLevel difficulty)
    {
        PuzzlePiece[] pool = GetPuzzlesForDifficulty(difficulty);
        int requiredPuzzles = roundsPerTeam * 2; // Both teams need puzzles
        
        return pool != null && pool.Length >= requiredPuzzles;
    }
    
    /// <summary>
    /// Checks if all puzzle pools have enough puzzles.
    /// </summary>
    public bool HasEnoughPuzzlesForAllDifficulties()
    {
        return HasEnoughPuzzles(DifficultyLevel.Kids) &&
               HasEnoughPuzzles(DifficultyLevel.BigKids) &&
               HasEnoughPuzzles(DifficultyLevel.Adults);
    }
    
#if UNITY_EDITOR
    [ContextMenu("Validate Puzzle Collection")]
    private void ValidatePuzzleCollection()
    {
        int requiredPuzzles = roundsPerTeam * 2;
        
        Debug.Log($"[{name}] Room Number: {roomNumber}"); // NEU
        Debug.Log($"[{name}] Required puzzles per difficulty: {requiredPuzzles}");
        
        if (kidsPuzzles == null || kidsPuzzles.Length < requiredPuzzles)
        {
            Debug.LogWarning($"[{name}] Not enough Kids puzzles! Need {requiredPuzzles}, have {kidsPuzzles?.Length ?? 0}", this);
        }
        
        if (bigKidsPuzzles == null || bigKidsPuzzles.Length < requiredPuzzles)
        {
            Debug.LogWarning($"[{name}] Not enough BigKids puzzles! Need {requiredPuzzles}, have {bigKidsPuzzles?.Length ?? 0}", this);
        }
        
        if (adultsPuzzles == null || adultsPuzzles.Length < requiredPuzzles)
        {
            Debug.LogWarning($"[{name}] Not enough Adults puzzles! Need {requiredPuzzles}, have {adultsPuzzles?.Length ?? 0}", this);
        }
        
        if (timeSettings == null)
        {
            Debug.LogWarning($"[{name}] Missing DifficultyTimeSettings!", this);
        }
        
        if (team1Image == null || team2Image == null)
        {
            Debug.LogWarning($"[{name}] Missing team images!", this);
        }
        
        if (HasEnoughPuzzlesForAllDifficulties())
        {
            Debug.Log($"[{name}] ? Puzzle Collection is valid!", this);
        }
    }
#endif
}