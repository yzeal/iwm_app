using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle Piece", menuName = "Quiz Game/Puzzle Piece")]
public class PuzzlePiece : ScriptableObject
{
    [Header("Hint Images (0 = smallest, 2 = largest)")]
    [Tooltip("Hint 0: Smallest image excerpt (starting image)")]
    public Sprite hintImage0;
    
    [Tooltip("Hint 1: Medium image excerpt (first hint)")]
    public Sprite hintImage1;
    
    [Tooltip("Hint 2: Largest image excerpt (second hint)")]
    public Sprite hintImage2;
    
    [Header("Solution")]
    [Tooltip("Complete exhibit photo shown on solution screen")]
    public Sprite solutionImage;
    
    [Tooltip("Localized name of the exhibit")]
    public LocalizedText exhibitName;
    
    [Tooltip("Optional localized description of the exhibit")]
    public LocalizedText exhibitDescription;
    
    // ============================================
    // PUBLIC METHODS
    // ============================================
    
    /// <summary>
    /// Gets the hint image for the specified hint level (0-2).
    /// </summary>
    public Sprite GetHintImage(int hintLevel)
    {
        return hintLevel switch
        {
            0 => hintImage0,
            1 => hintImage1,
            2 => hintImage2,
            _ => hintImage0 // Fallback to smallest image
        };
    }
    
    /// <summary>
    /// Gets the localized exhibit name.
    /// </summary>
    public string GetExhibitName(LanguageSystem.Language language)
    {
        if (exhibitName != null)
        {
            return exhibitName.GetText(language);
        }
        
        // Fallback
        return "Unknown Exhibit";
    }
    
    /// <summary>
    /// Gets the localized exhibit description (can be empty).
    /// </summary>
    public string GetExhibitDescription(LanguageSystem.Language language)
    {
        if (exhibitDescription != null)
        {
            return exhibitDescription.GetText(language);
        }
        
        // Empty description is valid
        return string.Empty;
    }
    
    // ============================================
    // VALIDATION
    // ============================================
    
    /// <summary>
    /// Checks if all hint images are assigned.
    /// </summary>
    public bool HasAllHintImages()
    {
        return hintImage0 != null && hintImage1 != null && hintImage2 != null;
    }
    
    /// <summary>
    /// Checks if solution image is assigned.
    /// </summary>
    public bool HasSolutionImage()
    {
        return solutionImage != null;
    }
    
    /// <summary>
    /// Checks if exhibit name is assigned and localized.
    /// </summary>
    public bool HasExhibitName()
    {
        return exhibitName != null;
    }
    
    /// <summary>
    /// Checks if all required assets are assigned.
    /// </summary>
    public bool IsValid()
    {
        return HasAllHintImages() && HasSolutionImage() && HasExhibitName();
    }
    
#if UNITY_EDITOR
    [ContextMenu("Validate Puzzle Piece")]
    private void ValidatePuzzlePiece()
    {
        if (!HasAllHintImages())
        {
            Debug.LogWarning($"[{name}] Missing hint images! All 3 hint images (0-2) must be assigned.", this);
        }
        
        if (!HasSolutionImage())
        {
            Debug.LogWarning($"[{name}] Missing solution image!", this);
        }
        
        if (!HasExhibitName())
        {
            Debug.LogWarning($"[{name}] Missing exhibit name LocalizedText!", this);
        }
        
        if (IsValid())
        {
            Debug.Log($"[{name}] ? Puzzle Piece is valid!", this);
        }
    }
#endif
}