using UnityEngine;

/// <summary>
/// Zentrales System für Team-Icon-Management
/// Stellt die ausgewählten Team-Icons und Namen basierend auf GameDataManager bereit
/// </summary>
[CreateAssetMenu(fileName = "TeamIconProvider", menuName = "Museum Quiz/Team Icon Provider")]
public class TeamIconProvider : ScriptableObject
{
    [System.Serializable]
    public class TeamIconSet
    {
        [Header("Icon 0")]
        public Sprite icon0;
        [Tooltip("Lokalisierter Name für Icon 0 (z.B. 'Team Urpferd' / 'Team Ancient Horse')")]
        public LocalizedText icon0Name;
        
        [Header("Icon 1")]
        public Sprite icon1;
        [Tooltip("Lokalisierter Name für Icon 1")]
        public LocalizedText icon1Name;
        
        [Header("Icon 2")]
        public Sprite icon2;
        [Tooltip("Lokalisierter Name für Icon 2")]
        public LocalizedText icon2Name;
        
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
        /// NEU (19.11.2025): Gibt den lokalisierten Namen für einen bestimmten Index zurück (0-2)
        /// </summary>
        public LocalizedText GetIconName(int index)
        {
            index = Mathf.Clamp(index, 0, 2);
            return index switch
            {
                0 => icon0Name,
                1 => icon1Name,
                2 => icon2Name,
                _ => icon0Name
            };
        }
        
        /// <summary>
        /// Validiert ob alle Icons zugewiesen sind
        /// </summary>
        public bool HasAllIcons()
        {
            return icon0 != null && icon1 != null && icon2 != null;
        }
        
        /// <summary>
        /// NEU (19.11.2025): Validiert ob alle Namen zugewiesen sind
        /// </summary>
        public bool HasAllNames()
        {
            return icon0Name != null && icon1Name != null && icon2Name != null;
        }
    }
    
    [Header("Team 1 Icons & Names")]
    public TeamIconSet team1Icons;
    
    [Header("Team 2 Icons & Names")]
    public TeamIconSet team2Icons;
    
    // ============================================
    // ICON GETTERS (EXISTING)
    // ============================================
    
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
    
    // ============================================
    // NAME GETTERS (NEU - 19.11.2025)
    // ============================================
    
    /// <summary>
    /// NEU (19.11.2025): Gibt den aktuell ausgewählten Namen für Team 1 zurück
    /// Liest Index aus GameDataManager
    /// </summary>
    public LocalizedText GetTeam1IconName()
    {
        int iconIndex = 0;
        if (GameDataManager.Instance != null)
        {
            iconIndex = GameDataManager.Instance.GetTeamIconIndex(0);
        }
        return team1Icons.GetIconName(iconIndex);
    }
    
    /// <summary>
    /// NEU (19.11.2025): Gibt den aktuell ausgewählten Namen für Team 2 zurück
    /// Liest Index aus GameDataManager
    /// </summary>
    public LocalizedText GetTeam2IconName()
    {
        int iconIndex = 0;
        if (GameDataManager.Instance != null)
        {
            iconIndex = GameDataManager.Instance.GetTeamIconIndex(1);
        }
        return team2Icons.GetIconName(iconIndex);
    }
    
    /// <summary>
    /// NEU (19.11.2025): Gibt Namen für ein Team zurück (0 = Team 1, 1 = Team 2)
    /// </summary>
    public LocalizedText GetTeamIconName(int teamIndex)
    {
        return teamIndex == 0 ? GetTeam1IconName() : GetTeam2IconName();
    }
    
    /// <summary>
    /// NEU (19.11.2025): Gibt den lokalisierten Text-String für Team 1 zurück
    /// Helper-Methode für direkten Zugriff auf den Text
    /// </summary>
    public string GetTeam1IconNameText(LanguageSystem.Language language)
    {
        LocalizedText name = GetTeam1IconName();
        return name != null ? name.GetText(language) : "Team 1";
    }
    
    /// <summary>
    /// NEU (19.11.2025): Gibt den lokalisierten Text-String für Team 2 zurück
    /// Helper-Methode für direkten Zugriff auf den Text
    /// </summary>
    public string GetTeam2IconNameText(LanguageSystem.Language language)
    {
        LocalizedText name = GetTeam2IconName();
        return name != null ? name.GetText(language) : "Team 2";
    }
    
    /// <summary>
    /// NEU (19.11.2025): Gibt den lokalisierten Text-String für ein Team zurück (0 = Team 1, 1 = Team 2)
    /// </summary>
    public string GetTeamIconNameText(int teamIndex, LanguageSystem.Language language)
    {
        return teamIndex == 0 ? GetTeam1IconNameText(language) : GetTeam2IconNameText(language);
    }
    
    // ============================================
    // VALIDATION
    // ============================================
    
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
    
    /// <summary>
    /// NEU (19.11.2025): Validiert ob alle Namen korrekt zugewiesen sind
    /// </summary>
    public bool ValidateNames()
    {
        bool valid = true;
        
        if (!team1Icons.HasAllNames())
        {
            Debug.LogWarning("Team 1 Namen nicht vollständig zugewiesen!");
            valid = false;
        }
        
        if (!team2Icons.HasAllNames())
        {
            Debug.LogWarning("Team 2 Namen nicht vollständig zugewiesen!");
            valid = false;
        }
        
        return valid;
    }
    
    /// <summary>
    /// NEU (19.11.2025): Validiert ob alle Icons UND Namen korrekt zugewiesen sind
    /// </summary>
    public bool ValidateAll()
    {
        bool iconsValid = ValidateIcons();
        bool namesValid = ValidateNames();
        return iconsValid && namesValid;
    }
    
    #region Context Menu Methods
    
    [ContextMenu("Validate All Icons & Names")]
    void ContextValidateAll()
    {
        bool valid = ValidateAll();
        Debug.Log(valid ? "? Alle Icons und Namen korrekt zugewiesen" : "? Fehlende Icons oder Namen gefunden");
    }
    
    [ContextMenu("Validate Icons Only")]
    void ContextValidateIcons()
    {
        bool valid = ValidateIcons();
        Debug.Log(valid ? "? Alle Icons korrekt zugewiesen" : "? Fehlende Icons gefunden");
    }
    
    [ContextMenu("Validate Names Only")]
    void ContextValidateNames()
    {
        bool valid = ValidateNames();
        Debug.Log(valid ? "? Alle Namen korrekt zugewiesen" : "? Fehlende Namen gefunden");
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
    
    [ContextMenu("Test - Get All Icons & Names")]
    void ContextTestGetAllIconsAndNames()
    {
        Debug.Log("=== TEAM 1 ===");
        Debug.Log($"Icon 0: {team1Icons.icon0?.name}, Name: {team1Icons.icon0Name?.TextKey}");
        Debug.Log($"Icon 1: {team1Icons.icon1?.name}, Name: {team1Icons.icon1Name?.TextKey}");
        Debug.Log($"Icon 2: {team1Icons.icon2?.name}, Name: {team1Icons.icon2Name?.TextKey}");
        
        Debug.Log("=== TEAM 2 ===");
        Debug.Log($"Icon 0: {team2Icons.icon0?.name}, Name: {team2Icons.icon0Name?.TextKey}");
        Debug.Log($"Icon 1: {team2Icons.icon1?.name}, Name: {team2Icons.icon1Name?.TextKey}");
        Debug.Log($"Icon 2: {team2Icons.icon2?.name}, Name: {team2Icons.icon2Name?.TextKey}");
        
        Debug.Log("=== CURRENT SELECTION ===");
        if (GameDataManager.Instance != null)
        {
            Debug.Log($"Team 1: {GetTeam1Icon()?.name} ({GetTeam1IconName()?.TextKey})");
            Debug.Log($"Team 2: {GetTeam2Icon()?.name} ({GetTeam2IconName()?.TextKey})");
        }
    }
    
    #endregion
}