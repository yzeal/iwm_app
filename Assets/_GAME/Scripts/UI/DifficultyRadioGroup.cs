using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class DifficultyRadioButton
{
    [Header("UI Components")]
    public Button button;
    public Image backgroundImage;
    public Image iconImage; // Für zukünftige Icons
    public TextMeshProUGUI labelText;
    
    [Header("Difficulty")]
    public DifficultyLevel difficulty;
    
    [Header("Visual States")]
    public Color selectedColor = new Color(0.2f, 0.8f, 0.2f); // Grün
    public Color unselectedColor = Color.white;
    public Color selectedTextColor = Color.white;
    public Color unselectedTextColor = Color.black;
    
    public bool IsSelected { get; private set; }
    
    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        UpdateVisuals();
    }
    
    void UpdateVisuals()
    {
        if (backgroundImage)
        {
            backgroundImage.color = IsSelected ? selectedColor : unselectedColor;
        }
        
        if (labelText)
        {
            labelText.color = IsSelected ? selectedTextColor : unselectedTextColor;
        }
        
        // Update Button ColorBlock für bessere Interaktion
        if (button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = IsSelected ? selectedColor : unselectedColor;
            colors.selectedColor = IsSelected ? selectedColor : unselectedColor;
            button.colors = colors;
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
        
        SetSelected(false); // Default state
    }
    
    string GetDifficultyDisplayName()
    {
        return difficulty switch
        {
            DifficultyLevel.Kids => "Kids",
            DifficultyLevel.BigKids => "BigKids",
            DifficultyLevel.Adults => "Adults",
            _ => "Adults"
        };
    }
}

public class DifficultyRadioGroup : MonoBehaviour
{
    [Header("Radio Buttons")]
    public DifficultyRadioButton[] radioButtons;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectionSound;
    
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
            radioButtons[i].Initialize(OnRadioButtonClicked);
        }
        
        // Setze Standard-Auswahl
        SetSelectedDifficulty(selectedDifficulty);
    }
    
    void OnRadioButtonClicked(DifficultyLevel difficulty)
    {
        SetSelectedDifficulty(difficulty);
        
        // Spiele Sound ab
        if (audioSource && selectionSound)
        {
            audioSource.PlayOneShot(selectionSound);
        }
        
        // Benachrichtige Listener
        OnDifficultyChanged?.Invoke(difficulty);
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
    
    [ContextMenu("Setup Default Colors")]
    void SetupDefaultColors()
    {
        for (int i = 0; i < radioButtons.Length; i++)
        {
            radioButtons[i].selectedColor = new Color(0.2f, 0.8f, 0.2f); // Grün
            radioButtons[i].unselectedColor = Color.white;
            radioButtons[i].selectedTextColor = Color.white;
            radioButtons[i].unselectedTextColor = Color.black;
        }
    }
    
    [ContextMenu("Test Selection")]
    void TestSelection()
    {
        if (radioButtons.Length > 0)
        {
            OnRadioButtonClicked(radioButtons[0].difficulty);
        }
    }
    
    #endregion
}