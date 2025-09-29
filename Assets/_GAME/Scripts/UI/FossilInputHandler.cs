using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FossilInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset inputActions;
    
    [Header("Tilt Settings")]
    [Range(0.1f, 1f)]
    public float tiltThreshold = 0.3f;
    
    [Header("Touch Buttons (Fallback)")]
    public Button leftButton;   // Skip - Linke Bildschirmhälfte
    public Button rightButton;  // Correct - Rechte Bildschirmhälfte
    
    [Header("Debug")]
    public bool debugMode = false;
    public bool forceTouchMode = false; // Für Testing
    
    public System.Action OnCorrectInput;
    public System.Action OnSkipInput;
    
    private InputAction accelerometerAction;
    private bool inputEnabled = true;
    private Vector3 initialAcceleration;
    private bool hasInitialReading = false;
    private bool accelerometerAvailable = false;
    
    void Start()
    {
        SetupInputActions();
        SetupButtons();
        CheckAccelerometerAvailability();
    }
    
    void SetupButtons()
    {
        // Setup Button Events
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("LEFT BUTTON - Skip");
                TriggerSkip();
            });
        }
        
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(() => {
                if (debugMode) Debug.Log("RIGHT BUTTON - Correct");
                TriggerCorrect();
            });
        }
    }
    
    void SetupInputActions()
    {
        if (inputActions != null)
        {
            inputActions.Enable();
            
            accelerometerAction = inputActions.FindAction("Accelerometer");
            if (accelerometerAction != null)
            {
                accelerometerAction.Enable();
                Debug.Log("Accelerometer action enabled");
            }
        }
        else
        {
            Debug.LogWarning("No Input Actions assigned! Using button fallback only.");
        }
    }
    
    void CheckAccelerometerAvailability()
    {
        // Teste ob Accelerometer verfügbar ist
        if (accelerometerAction != null)
        {
            Vector3 testReading = accelerometerAction.ReadValue<Vector3>();
            accelerometerAvailable = testReading != Vector3.zero;
        }
        
        if (!accelerometerAvailable)
        {
            Debug.Log("Accelerometer not available - using button fallback");
        }
        
        if (debugMode)
        {
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Accelerometer available: {accelerometerAvailable}");
            Debug.Log($"Force touch mode: {forceTouchMode}");
        }
    }
    
    void Update()
    {
        if (!inputEnabled) return;
        
        // Verwende Accelerometer nur wenn verfügbar und nicht im Force Touch Mode
        if (accelerometerAvailable && !forceTouchMode)
        {
            CheckTiltInput();
        }
    }
    
    void CheckTiltInput()
    {
        if (accelerometerAction == null) return;
        
        Vector3 acceleration = accelerometerAction.ReadValue<Vector3>();
        
        // Wenn keine Daten, deaktiviere Accelerometer
        if (acceleration == Vector3.zero)
        {
            accelerometerAvailable = false;
            return;
        }
        
        // Initialisierung beim ersten Lesen
        if (!hasInitialReading)
        {
            initialAcceleration = acceleration;
            hasInitialReading = true;
            return;
        }
        
        // Berechne Veränderung gegenüber Initialposition
        Vector3 deltaAccel = acceleration - initialAcceleration;
        
        if (debugMode)
        {
            Debug.Log($"Acceleration: {acceleration} | Delta: {deltaAccel.z:F2}");
        }
        
        // Nach vorne neigen (Handy von Stirn weg) = Richtig
        if (deltaAccel.z > tiltThreshold)
        {
            TriggerCorrect();
        }
        // Nach hinten neigen (Handy zur Stirn hin) = Überspringen  
        else if (deltaAccel.z < -tiltThreshold)
        {
            TriggerSkip();
        }
    }
    
    public void TriggerCorrect()
    {
        if (!inputEnabled) return;
        
        Debug.Log("CORRECT!");
        OnCorrectInput?.Invoke();
        
        DisableInputTemporarily(0.5f);
    }
    
    public void TriggerSkip()
    {
        if (!inputEnabled) return;
        
        Debug.Log("SKIP!");
        OnSkipInput?.Invoke();
        
        DisableInputTemporarily(0.5f);
    }
    
    void DisableInputTemporarily(float duration)
    {
        inputEnabled = false;
        
        // Deaktiviere auch Buttons temporär
        SetButtonsInteractable(false);
        
        Invoke(nameof(EnableInput), duration);
    }
    
    void EnableInput()
    {
        inputEnabled = true;
        SetButtonsInteractable(true);
    }
    
    void SetButtonsInteractable(bool interactable)
    {
        if (leftButton != null)
            leftButton.interactable = interactable;
        if (rightButton != null)
            rightButton.interactable = interactable;
    }
    
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        SetButtonsInteractable(enabled);
        
        if (enabled)
        {
            hasInitialReading = false;
            CheckAccelerometerAvailability();
        }
    }
    
    public void CalibrateDevice()
    {
        hasInitialReading = false;
        Debug.Log("Device calibrated");
    }
    
    public bool IsUsingAccelerometer()
    {
        return accelerometerAvailable && !forceTouchMode;
    }
    
    public string GetInputModeInfo()
    {
        if (IsUsingAccelerometer())
            return "Accelerometer aktiv - Neige das Handy";
        else
            return "Touch-Modus - Tippe links (überspringen) oder rechts (richtig)";
    }
    
    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }
}