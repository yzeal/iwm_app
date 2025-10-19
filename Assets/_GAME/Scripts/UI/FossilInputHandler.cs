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
    
    [Header("Touch Buttons")]
    public Button leftButton;   // Skip - Linke Bildschirmhälfte
    public Button rightButton;  // Correct - Rechte Bildschirmhälfte
    
    [Header("Input Mode")]
    [Tooltip("Wenn aktiviert, sind BEIDE Input-Methoden gleichzeitig verfügbar")]
    public bool allowDualInput = true;
    public bool forceTouchMode = false; // Deaktiviert Accelerometer komplett
    
    [Header("Debug")]
    public bool debugMode = false;
    
    public System.Action OnCorrectInput;
    public System.Action OnSkipInput;
    
    private InputAction accelerometerAction;
    private bool inputEnabled = true;
    private Vector3 initialAcceleration;
    private bool hasInitialReading = false;
    private bool accelerometerAvailable = false;
    
    // Für direkte Accelerometer-Abfrage als Fallback
    private bool useDirectAccelerometer = false;
    
    // NEU: Globaler Input-Cooldown für ALLE Input-Methoden
    private float lastInputTime = 0f;
    private const float INPUT_COOLDOWN = 0.5f;
    
    void Start()
    {
        SetupInputActions();
        SetupButtons();
        CheckAccelerometerAvailability();
        EnableAccelerometerSensor();
    }
    
    void SetupButtons()
    {
        // Touch-Buttons sind IMMER aktiv (parallel zu Accelerometer)
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
        
        if (debugMode)
        {
            Debug.Log("Touch buttons setup - always active");
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
    
    void EnableAccelerometerSensor()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Aktiviere Accelerometer-Sensor explizit
        if (SystemInfo.supportsAccelerometer)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            
            if (Accelerometer.current != null)
            {
                Debug.Log("Accelerometer device found and enabled");
                
                if (Accelerometer.current.samplingFrequency > 0)
                {
                    Debug.Log($"Accelerometer sampling frequency: {Accelerometer.current.samplingFrequency} Hz");
                }
            }
            else
            {
                Debug.LogWarning("Accelerometer.current is null - device may not support accelerometer");
            }
        }
        else
        {
            Debug.LogWarning("Device does not support accelerometer according to SystemInfo");
        }
#endif
    }
    
    void CheckAccelerometerAvailability()
    {
        // Methode 1: Über Input Action
        if (accelerometerAction != null)
        {
            Vector3 testReading = accelerometerAction.ReadValue<Vector3>();
            accelerometerAvailable = testReading != Vector3.zero;
            
            if (debugMode)
            {
                Debug.Log($"Input Action test reading: {testReading}");
            }
        }
        
        // Methode 2 (Fallback): Direkt über Unity Input System
        if (!accelerometerAvailable && Accelerometer.current != null)
        {
            Vector3 directReading = Accelerometer.current.acceleration.ReadValue();
            if (directReading != Vector3.zero)
            {
                useDirectAccelerometer = true;
                accelerometerAvailable = true;
                Debug.Log("Using direct Accelerometer access as fallback");
            }
        }
        
        // Methode 3 (Legacy Fallback): Input.acceleration
        if (!accelerometerAvailable)
        {
            Invoke(nameof(CheckLegacyAccelerometer), 0.5f);
        }
        
        if (debugMode)
        {
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"SystemInfo.supportsAccelerometer: {SystemInfo.supportsAccelerometer}");
            Debug.Log($"Accelerometer available: {accelerometerAvailable}");
            Debug.Log($"Using direct accelerometer: {useDirectAccelerometer}");
            Debug.Log($"Force touch mode: {forceTouchMode}");
            Debug.Log($"Allow dual input: {allowDualInput}");
        }
    }
    
    void CheckLegacyAccelerometer()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        if (!accelerometerAvailable && Input.acceleration != Vector3.zero)
        {
            Debug.Log("Legacy Input.acceleration available - using as fallback");
            accelerometerAvailable = true;
            useDirectAccelerometer = true;
        }
#pragma warning restore CS0618
    }
    
    void Update()
    {
        if (!inputEnabled) return;
        
        // NEU: Prüfe Accelerometer nur wenn verfügbar UND nicht im Force-Touch-Modus
        // Touch-Buttons funktionieren IMMER parallel (wenn allowDualInput = true)
        if (accelerometerAvailable && !forceTouchMode)
        {
            CheckTiltInput();
        }
    }
    
    void CheckTiltInput()
    {
        // NEU: Globaler Cooldown-Check (gilt für ALLE Input-Arten)
        if (Time.time - lastInputTime < INPUT_COOLDOWN)
        {
            return;
        }
        
        Vector3 acceleration = Vector3.zero;
        
        if (useDirectAccelerometer)
        {
            if (Accelerometer.current != null)
            {
                acceleration = Accelerometer.current.acceleration.ReadValue();
            }
#pragma warning disable CS0618 // Type or member is obsolete
            else
            {
                acceleration = Input.acceleration;
            }
#pragma warning restore CS0618
        }
        else if (accelerometerAction != null)
        {
            acceleration = accelerometerAction.ReadValue<Vector3>();
        }
        
        if (acceleration == Vector3.zero)
        {
            if (debugMode)
            {
                Debug.LogWarning("Zero acceleration reading");
            }
            return;
        }
        
        if (!hasInitialReading)
        {
            initialAcceleration = acceleration;
            hasInitialReading = true;
            
            if (debugMode)
            {
                Debug.Log($"Initial acceleration set: {initialAcceleration}");
            }
            return;
        }
        
        Vector3 deltaAccel = acceleration - initialAcceleration;
        
        if (debugMode)
        {
            Debug.Log($"Acceleration: {acceleration} | Delta Z: {deltaAccel.z:F2} | Threshold: {tiltThreshold}");
        }
        
        // Nach vorne neigen (Handy von Stirn weg) = Richtig
        if (deltaAccel.z > tiltThreshold)
        {
            if (debugMode) Debug.Log("TILT FORWARD - Correct!");
            TriggerCorrect();
        }
        // Nach hinten neigen (Handy zur Stirn hin) = Überspringen  
        else if (deltaAccel.z < -tiltThreshold)
        {
            if (debugMode) Debug.Log("TILT BACKWARD - Skip!");
            TriggerSkip();
        }
    }
    
    public void TriggerCorrect()
    {
        // NEU: Cooldown-Check VOR Ausführung (gilt für Touch UND Tilt)
        if (!inputEnabled || Time.time - lastInputTime < INPUT_COOLDOWN) 
        {
            if (debugMode) Debug.Log("Input blocked - cooldown active");
            return;
        }
        
        Debug.Log("CORRECT!");
        lastInputTime = Time.time; // NEU: Setze Cooldown-Timer
        
        OnCorrectInput?.Invoke();
        TriggerHapticFeedback();
        
        DisableInputTemporarily(0.5f);
    }
    
    public void TriggerSkip()
    {
        // NEU: Cooldown-Check VOR Ausführung (gilt für Touch UND Tilt)
        if (!inputEnabled || Time.time - lastInputTime < INPUT_COOLDOWN)
        {
            if (debugMode) Debug.Log("Input blocked - cooldown active");
            return;
        }
        
        Debug.Log("SKIP!");
        lastInputTime = Time.time; // NEU: Setze Cooldown-Timer
        
        OnSkipInput?.Invoke();
        TriggerHapticFeedback();
        
        DisableInputTemporarily(0.5f);
    }
    
    void TriggerHapticFeedback()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
    
    void DisableInputTemporarily(float duration)
    {
        inputEnabled = false;
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
            lastInputTime = 0f; // Reset Cooldown
            CheckAccelerometerAvailability();
        }
    }
    
    public void CalibrateDevice()
    {
        hasInitialReading = false;
        lastInputTime = 0f; // Reset Cooldown
        
        if (accelerometerAvailable)
        {
            if (useDirectAccelerometer && Accelerometer.current != null)
            {
                initialAcceleration = Accelerometer.current.acceleration.ReadValue();
            }
            else if (accelerometerAction != null)
            {
                initialAcceleration = accelerometerAction.ReadValue<Vector3>();
            }
            
            if (debugMode)
            {
                Debug.Log($"Device calibrated - Initial: {initialAcceleration}");
            }
        }
    }
    
    public bool IsUsingAccelerometer()
    {
        return accelerometerAvailable && !forceTouchMode;
    }
    
    // NEU: Erweiterte Info-Methode
    public string GetInputModeInfo()
    {
        if (IsUsingAccelerometer() && allowDualInput)
        {
            return "Accelerometer + Touch aktiv - Neige das Handy ODER tippe";
        }
        else if (IsUsingAccelerometer())
        {
            return "Accelerometer aktiv - Neige das Handy";
        }
        else
        {
            return "Touch-Modus - Tippe links (überspringen) oder rechts (richtig)";
        }
    }
    
    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }
}