using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Radio-Button-Gruppe für Team-Icon-Auswahl
/// Zeigt 3 Icons an, erlaubt Auswahl von einem (Radio-Button-Pattern)
/// Verwendet Sprite-Swapping für Selected/Unselected States
/// </summary>
public class TeamIconRadioGroup : MonoBehaviour
{
    [System.Serializable]
    public class IconButton
    {
        public Button button;
        public Image iconImage;
        public Sprite normalSprite;          // Icon wenn NICHT ausgewählt
        public Sprite selectedSprite;        // Icon wenn ausgewählt (Hervorhebung)
    }
    
    [Header("Icon Buttons (3 Icons pro Team)")]
    [SerializeField] private IconButton[] iconButtons = new IconButton[3];
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectionSound;
    
    [Header("Mobile Settings")]
    [SerializeField] private bool enableHapticFeedback = true;
    
    // Events
    public event Action<int> OnIconChanged;
    
    private int currentSelectedIndex = 0;
    
    void Start()
    {
        SetupButtons();
    }
    
    void SetupButtons()
    {
        for (int i = 0; i < iconButtons.Length; i++)
        {
            int index = i; // Closure für Lambda
            
            if (iconButtons[i].button != null)
            {
                iconButtons[i].button.onClick.AddListener(() => SelectIcon(index));
            }
            else
            {
                Debug.LogWarning($"Icon Button {i} ist nicht zugewiesen!");
            }
        }
    }
    
    /// <summary>
    /// Setzt den ausgewählten Icon-Index (0-2)
    /// </summary>
    public void SetSelectedIcon(int iconIndex)
    {
        iconIndex = Mathf.Clamp(iconIndex, 0, 2);
        
        if (currentSelectedIndex != iconIndex)
        {
            currentSelectedIndex = iconIndex;
            UpdateVisuals();
        }
    }
    
    /// <summary>
    /// Gibt den aktuell ausgewählten Icon-Index zurück (0-2)
    /// </summary>
    public int GetSelectedIconIndex()
    {
        return currentSelectedIndex;
    }
    
    /// <summary>
    /// Icon auswählen via Button-Click
    /// </summary>
    void SelectIcon(int iconIndex)
    {
        if (currentSelectedIndex == iconIndex)
            return; // Bereits ausgewählt
        
        currentSelectedIndex = iconIndex;
        UpdateVisuals();
        
        // Feedback
        PlaySelectionSound();
        TriggerHapticFeedback();
        
        // Event feuern
        OnIconChanged?.Invoke(currentSelectedIndex);
        
        Debug.Log($"Icon {iconIndex} selected");
    }
    
    /// <summary>
    /// Aktualisiert die visuellen States aller Buttons
    /// </summary>
    void UpdateVisuals()
    {
        for (int i = 0; i < iconButtons.Length; i++)
        {
            if (iconButtons[i].iconImage == null)
                continue;
            
            bool isSelected = (i == currentSelectedIndex);
            
            // Sprite-Swapping: Selected vs Normal
            if (isSelected && iconButtons[i].selectedSprite != null)
            {
                iconButtons[i].iconImage.sprite = iconButtons[i].selectedSprite;
            }
            else if (!isSelected && iconButtons[i].normalSprite != null)
            {
                iconButtons[i].iconImage.sprite = iconButtons[i].normalSprite;
            }
        }
    }
    
    void PlaySelectionSound()
    {
        if (audioSource != null && selectionSound != null)
        {
            audioSource.PlayOneShot(selectionSound);
        }
    }
    
    void TriggerHapticFeedback()
    {
        if (!enableHapticFeedback) return;
        
        #if UNITY_IOS || UNITY_ANDROID
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
        #endif
    }
    
    #region Debug Methods
    
    [ContextMenu("Test - Select Icon 0")]
    void DebugSelectIcon0()
    {
        SelectIcon(0);
    }
    
    [ContextMenu("Test - Select Icon 1")]
    void DebugSelectIcon1()
    {
        SelectIcon(1);
    }
    
    [ContextMenu("Test - Select Icon 2")]
    void DebugSelectIcon2()
    {
        SelectIcon(2);
    }
    
    [ContextMenu("Print Current Selection")]
    void DebugPrintSelection()
    {
        Debug.Log($"Currently selected icon: {currentSelectedIndex}");
    }
    
    #endregion
}