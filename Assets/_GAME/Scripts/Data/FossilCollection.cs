using UnityEngine;

[CreateAssetMenu(fileName = "New Fossil Collection", menuName = "FossilGame/Fossil Collection")]
public class FossilCollection : ScriptableObject
{
    [Header("Collection Info")]
    public string collectionName;
    public int roomNumber;

    [Header("Team Images")]
    public Sprite team1Image;
    public Sprite team2Image;

    [Header("Fossils nach Schwierigkeitsgrad")]
    [SerializeField] private FossilData[] kidsFossils;
    [SerializeField] private FossilData[] bigKidsFossils;
    [SerializeField] private FossilData[] adultsFossils;

    [Header("Game Settings")]
    [Range(3, 10)]
    public int fossilsPerRound = 5;
    [Range(15f, 120f)]
    public float roundDuration = 30f;
    
    [Header("Zeit-Einstellungen")]
    public DifficultyTimeSettings timeSettings = new DifficultyTimeSettings();

    /// <summary>
    /// Gibt die Fossilien f�r den angegebenen Schwierigkeitsgrad zur�ck
    /// </summary>
    public FossilData[] GetFossilsForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => kidsFossils ?? new FossilData[0],
            DifficultyLevel.BigKids => bigKidsFossils ?? new FossilData[0],
            DifficultyLevel.Adults => adultsFossils ?? new FossilData[0],
            _ => adultsFossils ?? new FossilData[0]
        };
    }

    /// <summary>
    /// Gibt zuf�llige Fossilien f�r den angegebenen Schwierigkeitsgrad zur�ck
    /// </summary>
    public FossilData[] GetRandomFossils(int count, DifficultyLevel difficulty)
    {
        var availableFossils = GetFossilsForDifficulty(difficulty);
        
        if (availableFossils.Length <= count)
            return availableFossils;

        // Fisher-Yates Shuffle f�r zuf�llige Auswahl
        FossilData[] shuffled = new FossilData[availableFossils.Length];
        System.Array.Copy(availableFossils, shuffled, availableFossils.Length);

        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        FossilData[] result = new FossilData[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }
    
    /// <summary>
    /// Berechnet die angepasste Rundendauer basierend auf Schwierigkeitsgrad
    /// </summary>
    public float GetAdjustedRoundDuration(DifficultyLevel difficulty)
    {
        return roundDuration * timeSettings.GetTimeMultiplier(difficulty);
    }
    
    /// <summary>
    /// Gibt die Anzahl verf�gbarer Fossilien f�r einen Schwierigkeitsgrad zur�ck
    /// </summary>
    public int GetFossilCountForDifficulty(DifficultyLevel difficulty)
    {
        return GetFossilsForDifficulty(difficulty).Length;
    }
    
    /// <summary>
    /// Validierung: Pr�ft ob f�r alle Schwierigkeitsgrade Fossilien vorhanden sind
    /// </summary>
    public bool HasFossilsForAllDifficulties()
    {
        return kidsFossils != null && kidsFossils.Length > 0 &&
               bigKidsFossils != null && bigKidsFossils.Length > 0 &&
               adultsFossils != null && adultsFossils.Length > 0;
    }

    #region Legacy Support - f�r bestehenden Code
    
    /// <summary>
    /// Legacy-Methode f�r R�ckw�rtskompatibilit�t - verwendet Adults-Fossilien
    /// </summary>
    [System.Obsolete("Use GetRandomFossils(count, difficulty) instead")]
    public FossilData[] GetRandomFossils(int count)
    {
        return GetRandomFossils(count, DifficultyLevel.Adults);
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    [ContextMenu("Validate All Difficulties")]
    private void ValidateFossils()
    {
        Debug.Log($"Collection {collectionName} - Kids: {GetFossilCountForDifficulty(DifficultyLevel.Kids)}, " +
                 $"BigKids: {GetFossilCountForDifficulty(DifficultyLevel.BigKids)}, " +
                 $"Adults: {GetFossilCountForDifficulty(DifficultyLevel.Adults)}");
    }
    
    #endregion
}