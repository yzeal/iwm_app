using UnityEngine;
using UnityEngine.UI;

public class UniversalFullscreenButton : MonoBehaviour
{
    [Header("Settings")]
    public bool hideIfNotSupported = true;
    public bool updateButtonImageAutomatically = true;
    
    [Header("Button Images (Optional)")]
    public Sprite fullscreenIcon; // Icon für "Enter Fullscreen"
    public Sprite exitFullscreenIcon; // Icon für "Exit Fullscreen"
    
    private Button button;
    private Image buttonImage;
    
    void Start()
    {
        SetupButton();
    }
    
    void SetupButton()
    {
        // Finde Button-Komponente
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("UniversalFullscreenButton: No Button component found on this GameObject!");
            return;
        }
        
        // Finde Image-Komponente
        buttonImage = button.GetComponent<Image>();
        
        // Prüfe Fullscreen-Support
        bool isSupported = FullscreenManager.Instance.IsFullscreenSupported();
        
        if (!isSupported && hideIfNotSupported)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Setup Button Event
        button.onClick.AddListener(OnButtonClick);
        
        // Initial Button Image Update
        if (updateButtonImageAutomatically)
        {
            UpdateButtonImage();
        }
        
        Debug.Log("UniversalFullscreenButton: Setup complete");
    }
    
    void OnButtonClick()
    {
        FullscreenManager.Instance.ToggleFullscreen();
        
        // Update Button Image nach Fullscreen-Wechsel
        if (updateButtonImageAutomatically)
        {
            // Kleine Verzögerung für UI-Update
            Invoke(nameof(UpdateButtonImage), 0.1f);
        }
    }
    
    void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            bool isFullscreen = FullscreenManager.Instance.IsCurrentlyFullscreen();
            
            // Wechsle Icon basierend auf Fullscreen-Status
            if (isFullscreen && exitFullscreenIcon != null)
            {
                buttonImage.sprite = exitFullscreenIcon;
            }
            else if (!isFullscreen && fullscreenIcon != null)
            {
                buttonImage.sprite = fullscreenIcon;
            }
            
            // Optional: Tooltip-Text aktualisieren (falls Image mit Tooltip verwendet wird)
            UpdateTooltip();
        }
    }
    
    void UpdateTooltip()
    {
        // Falls du später Tooltips hinzufügen möchtest
        // Tooltip tooltip = GetComponent<Tooltip>();
        // if (tooltip != null)
        // {
        //     tooltip.text = FullscreenManager.Instance.GetFullscreenButtonText();
        // }
    }
    
    // Optional: Manueller Image-Update von außen
    public void ForceUpdateButtonImage()
    {
        UpdateButtonImage();
    }
}