using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class DifficultyRadioButton
{
    [Header("UI Components")]
    public Button button;
    public Image checkboxImage; // Hauptbild für Checkbox-Darstellung
    public Image difficultyIcon; // Optional: Icon für Schwierigkeitsgrad
    public TextMeshProUGUI labelText;
    
    [Header("Difficulty")]
    public DifficultyLevel difficulty;
    
    [Header("Checkbox Sprites")]
    public Sprite uncheckedSprite; // Bild für unselected State
    public Sprite checkedSprite;   // Bild für selected State
    
    [Header("Optional: Difficulty Icons")]
    public Sprite kidsIcon;
    public Sprite bigKidsIcon;
    public Sprite adultsIcon;
    
    [Header("Text Colors (optional)")]
    public Color selectedTextColor = Color.black;
    public Color unselectedTextColor = Color.gray;
    public bool useTextColorChange = false; // Optional: Textfarbe ändern
    
    public bool IsSelected { get; private set; }
    
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        UpdateVisuals();
    }
    
    void UpdateVisuals()
    {
        // Haupt-Feature: Bild-Swapping für Checkbox
        if (checkboxImage)
        {
            checkboxImage.sprite = IsSelected ? checkedSprite : uncheckedSprite;
        }
        
        // Optional: Textfarbe ändern
        if (labelText && useTextColorChange)
        {
            labelText.color = IsSelected ? selectedTextColor : unselectedTextColor;
        }
        
        // Optional: Button-Zustand für Touch-Feedback
        if (button)
        {
            // Leichte Transparenz für besseres visuelles Feedback
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
                
            canvasGroup.alpha = IsSelected ? 1.0f : 0.8f;
        }
    }
    
    public void Initialize(Action<DifficultyLevel> onClickCallback)
    {
        if (button)
        {
            button.onClick.AddListener(() => onClickCallback(difficulty));
        }
        
        // Setze Label-Text basierend auf Difficulty
        if (labelText)
        {
            labelText.text = GetDifficultyDisplayName();
        }
        
        // Setze Schwierigkeitsgrad-Icon falls vorhanden
        SetupDifficultyIcon();
        
        // Validiere Sprites
        ValidateSprites();
        
        SetSelected(false); // Default state
    }
    
    void SetupDifficultyIcon()
    {
        if (difficultyIcon)
        {
            Sprite iconToUse = difficulty switch
            {
                DifficultyLevel.Kids => kidsIcon,
                DifficultyLevel.BigKids => bigKidsIcon,
                DifficultyLevel.Adults => adultsIcon,
                _ => null
            };
            
            if (iconToUse != null)
            {
                difficultyIcon.sprite = iconToUse;
                difficultyIcon.gameObject.SetActive(true);
            }
            else
            {
                difficultyIcon.gameObject.SetActive(false);
            }
        }
    }
    
    void ValidateSprites()
    {
        if (uncheckedSprite == null || checkedSprite == null)
        {
            Debug.LogWarning($"DifficultyRadioButton ({difficulty}): Missing checkbox sprites! " +
                           "Please assign uncheckedSprite and checkedSprite in the inspector.");
        }
    }
    
    string GetDifficultyDisplayName()
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => "Kinder",
            DifficultyLevel.BigKids => "BigKids",
            DifficultyLevel.Adults => "Erwachsene",
            _ => "Adults"
        };
    }
    
    #region Editor Helpers
    
    /// <summary>
    /// Erstellt Standard-Checkbox-Sprites falls keine vorhanden sind
    /// </summary>
    public void CreateDefaultSprites()
    {
        // Diese Methode könnte später erweitert werden um Standard-Sprites programmatisch zu erstellen
        Debug.Log($"Standard-Sprites für {difficulty} erstellen - in Unity manuell zuweisen");
    }
    
    #endregion
}

public class DifficultyRadioGroup : MonoBehaviour
{
    [Header("Radio Buttons")]
    public DifficultyRadioButton[] radioButtons;
    
    [Header("Default Checkbox Sprites (Fallback)")]
    public Sprite defaultUncheckedSprite;
    public Sprite defaultCheckedSprite;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectionSound;
    
    [Header("Mobile Optimizations")]
    [SerializeField] private float buttonPadding = 10f; // Abstand zwischen Buttons
    [SerializeField] private bool enableHapticFeedback = true;
    
    // Events
    public Action<DifficultyLevel> OnDifficultyChanged;
    
    private DifficultyLevel selectedDifficulty = DifficultyLevel.Adults;
    
    void Start()
    {
        InitializeRadioButtons();
    }
    
    void InitializeRadioButtons()
    {
        for (int i = 0; i < radioButtons.Length; i++)
        {
            // Setze Default-Sprites falls keine zugewiesen sind
            ApplyDefaultSprites(radioButtons[i]);
            
            // Mobile-Optimierungen
            OptimizeForMobile(radioButtons[i]);
            
            // Initialisiere Button
            radioButtons[i].Initialize(OnRadioButtonClicked);
        }
        
        // Setze Standard-Auswahl
        SetSelectedDifficulty(selectedDifficulty);
    }
    
    void ApplyDefaultSprites(DifficultyRadioButton radioButton)
    {
        if (radioButton.uncheckedSprite == null && defaultUncheckedSprite != null)
        {
            radioButton.uncheckedSprite = defaultUncheckedSprite;
        }
        
        if (radioButton.checkedSprite == null && defaultCheckedSprite != null)
        {
            radioButton.checkedSprite = defaultCheckedSprite;
        }
    }
    
    void OptimizeForMobile(DifficultyRadioButton radioButton)
    {
        if (radioButton.button != null)
        {
            RectTransform buttonRect = radioButton.button.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                // Mindest-Touch-Target-Größe für Mobile (empfohlen: 44pt)
                Vector2 currentSize = buttonRect.sizeDelta;
                float minSize = 60f; // Unity Units
                
                if (currentSize.x < minSize || currentSize.y < minSize)
                {
                    buttonRect.sizeDelta = new Vector2(
                        Mathf.Max(currentSize.x, minSize), 
                        Mathf.Max(currentSize.y, minSize)
                    );
                }
            }
        }
    }
    
    void OnRadioButtonClicked(DifficultyLevel difficulty)
    {
        SetSelectedDifficulty(difficulty);
        
        // Spiele Sound ab
        if (audioSource && selectionSound)
        {
            audioSource.PlayOneShot(selectionSound);
        }
        
        // Mobile Haptic Feedback
        if (enableHapticFeedback)
        {
            TriggerHapticFeedback();
        }
        
        // Benachrichtige Listener
        OnDifficultyChanged?.Invoke(difficulty);
    }
    
    void TriggerHapticFeedback()
    {
        #if UNITY_IOS || UNITY_ANDROID
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
        #endif
    }
    
    public void SetSelectedDifficulty(DifficultyLevel difficulty)
    {
        selectedDifficulty = difficulty;
        
        // Update alle Radio Buttons
        for (int i = 0; i < radioButtons.Length; i++)
        {
            bool isSelected = radioButtons[i].difficulty == difficulty;
            radioButtons[i].SetSelected(isSelected);
        }
    }
    
    public DifficultyLevel GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
    
    #region Editor Helpers
    
    [ContextMenu("Setup All Default Sprites")]
    void SetupDefaultSprites()
    {
        for (int i = 0; i < radioButtons.Length; i++)
        {
            ApplyDefaultSprites(radioButtons[i]);
            radioButtons[i].CreateDefaultSprites();
        }
        Debug.Log("Default sprites setup completed. Assign checkbox sprites in inspector.");
    }
    
    [ContextMenu("Test Selection - Kids")]
    void TestSelectionKids()
    {
        OnRadioButtonClicked(DifficultyLevel.Kids);
    }
    
    [ContextMenu("Test Selection - BigKids")]
    void TestSelectionBigKids()
    {
        OnRadioButtonClicked(DifficultyLevel.BigKids);
    }
    
    [ContextMenu("Test Selection - Adults")]
    void TestSelectionAdults()
    {
        OnRadioButtonClicked(DifficultyLevel.Adults);
    }
    
    [ContextMenu("Validate All Buttons")]
    void ValidateAllButtons()
    {
        int validButtons = 0;
        for (int i = 0; i < radioButtons.Length; i++)
        {
            if (radioButtons[i].button != null && 
                radioButtons[i].checkboxImage != null &&
                radioButtons[i].uncheckedSprite != null && 
                radioButtons[i].checkedSprite != null)
            {
                validButtons++;
            }
        }
        Debug.Log($"Valid buttons: {validButtons}/{radioButtons.Length}");
    }
    
    #endregion
}