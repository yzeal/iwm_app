using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FossilInputHandler : MonoBehaviour, IPointerDownHandler
{
    [Header("Input Actions")]
    public InputActionAsset inputActions;
    
    [Header("Tilt Settings")]
    [Range(0.1f, 1f)]
    public float tiltThreshold = 0.3f;
    
    [Header("Touch Areas (Simple Fallback)")]
    public RectTransform leftTouchArea;  // Skip - Linke Bildschirmhälfte
    public RectTransform rightTouchArea; // Correct - Rechte Bildschirmhälfte
    
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
        CheckAccelerometerAvailability();
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
            Debug.LogWarning("No Input Actions assigned! Using touch detection only.");
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
            Debug.Log("Accelerometer not available - using touch fallback");
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
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!inputEnabled) return;
        
        // Touch-System: Einfache Links/Rechts Aufteilung
        Vector2 touchPos = eventData.position;
        
        if (RectTransformUtility.RectangleContainsScreenPoint(rightTouchArea, touchPos, eventData.pressEventCamera))
        {
            if (debugMode) Debug.Log("Touch: RIGHT SIDE - Correct");
            TriggerCorrect();
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(leftTouchArea, touchPos, eventData.pressEventCamera))
        {
            if (debugMode) Debug.Log("Touch: LEFT SIDE - Skip");
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
        Invoke(nameof(EnableInput), duration);
    }
    
    void EnableInput()
    {
        inputEnabled = true;
    }
    
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        
        if (enabled)
        {
            hasInitialReading = false;
            CheckAccelerometerAvailability(); // Nochmal prüfen
        }
    }
    
    public void CalibrateDevice()
    {
        hasInitialReading = false;
        Debug.Log("Device calibrated");
    }
    
    // Public method für UI-Testing
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