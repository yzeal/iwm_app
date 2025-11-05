using UnityEngine;

/// <summary>
/// Zentrales System für Team-Icon-Management
/// Stellt die ausgewählten Team-Icons basierend auf GameDataManager bereit
/// </summary>
[CreateAssetMenu(fileName = "TeamIconProvider", menuName = "Museum Quiz/Team Icon Provider")]
public class TeamIconProvider : ScriptableObject
{
    [System.Serializable]
    public class TeamIconSet
    {
        [Header("Team Icon Options (3 pro Team)")]
        public Sprite icon0;
        public Sprite icon1;
        public Sprite icon2;
        
        /// <summary>
        /// Gibt das Icon für einen bestimmten Index zurück (0-2)
        /// </summary>
        public Sprite GetIcon(int index)
        {
            index = Mathf.Clamp(index, 0, 2);
            return index switch
            {
                0 => icon0,
                1 => icon1,
                2 => icon2,
                _ => icon0
            };
        }
        
        /// <summary>
        /// Validiert ob alle Icons zugewiesen sind
        /// </summary>
        public bool HasAllIcons()
        {
            return icon0 != null && icon1 != null && icon2 != null;
        }
    }
    
    [Header("Team 1 Icons")]
    public TeamIconSet team1Icons;
    
    [Header("Team 2 Icons")]
    public TeamIconSet team2Icons;
    
    /// <summary>
    /// Gibt das aktuell ausgewählte Icon für Team 1 zurück
    /// Liest Index aus GameDataManager
    /// </summary>
    public Sprite GetTeam1Icon()
    {
        int iconIndex = 0;
        if (GameDataManager.Instance != null)
        {
            iconIndex = GameDataManager.Instance.GetTeamIconIndex(0);
        }
        return team1Icons.GetIcon(iconIndex);
    }
    
    /// <summary>
    /// Gibt das aktuell ausgewählte Icon für Team 2 zurück
    /// Liest Index aus GameDataManager
    /// </summary>
    public Sprite GetTeam2Icon()
    {
        int iconIndex = 0;
        if (GameDataManager.Instance != null)
        {
            iconIndex = GameDataManager.Instance.GetTeamIconIndex(1);
        }
        return team2Icons.GetIcon(iconIndex);
    }
    
    /// <summary>
    /// Gibt Icon für ein Team zurück (0 = Team 1, 1 = Team 2)
    /// </summary>
    public Sprite GetTeamIcon(int teamIndex)
    {
        return teamIndex == 0 ? GetTeam1Icon() : GetTeam2Icon();
    }
    
    /// <summary>
    /// Validiert ob alle Icons korrekt zugewiesen sind
    /// </summary>
    public bool ValidateIcons()
    {
        bool valid = true;
        
        if (!team1Icons.HasAllIcons())
        {
            Debug.LogWarning("Team 1 Icons nicht vollständig zugewiesen!");
            valid = false;
        }
        
        if (!team2Icons.HasAllIcons())
        {
            Debug.LogWarning("Team 2 Icons nicht vollständig zugewiesen!");
            valid = false;
        }
        
        return valid;
    }
    
    #region Context Menu Methods
    
    [ContextMenu("Validate All Icons")]
    void ContextValidateIcons()
    {
        bool valid = ValidateIcons();
        Debug.Log(valid ? "? Alle Icons korrekt zugewiesen" : "? Fehlende Icons gefunden");
    }
    
    [ContextMenu("Print Current Icon Indices")]
    void ContextPrintCurrentIndices()
    {
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"Current Icon Selection: Team1={GameDataManager.Instance.GetTeamIconIndex(0)}, Team2={GameDataManager.Instance.GetTeamIconIndex(1)}");
        }
        else
        {
            Debug.LogWarning("GameDataManager Instance nicht verfügbar");
        }
    }
    
    [ContextMenu("Test - Get All Icons")]
    void ContextTestGetAllIcons()
    {
        Debug.Log($"Team 1 Icons: {team1Icons.icon0?.name}, {team1Icons.icon1?.name}, {team1Icons.icon2?.name}");
        Debug.Log($"Team 2 Icons: {team2Icons.icon0?.name}, {team2Icons.icon1?.name}, {team2Icons.icon2?.name}");
        Debug.Log($"Current Team 1 Icon: {GetTeam1Icon()?.name}");
        Debug.Log($"Current Team 2 Icon: {GetTeam2Icon()?.name}");
    }
    
    #endregion
}