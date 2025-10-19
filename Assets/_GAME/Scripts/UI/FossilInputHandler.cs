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
    
    // Globaler Input-Cooldown für ALLE Input-Methoden
    private float lastInputTime = 0f;
    private const float INPUT_COOLDOWN = 0.5f;
    
    // NEU: Tracking für "neutrale" Position
    private bool isInNeutralZone = true;
    private const float NEUTRAL_ZONE_THRESHOLD = 0.15f; // Kleinerer Threshold für "zurück zur Mitte"
    
    void Start()
    {
        SetupInputActions();
        SetupButtons();
        CheckAccelerometerAvailability();
        EnableAccelerometerSensor();
        
        Debug.Log($"[FossilInputHandler] Started - inputEnabled: {inputEnabled}");
    }
    
    void SetupButtons()
    {
        // Touch-Buttons sind IMMER aktiv (parallel zu Accelerometer)
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(() => {
                Debug.Log("[FossilInputHandler] LEFT BUTTON clicked");
                TriggerSkip();
            });
        }
        
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(() => {
                Debug.Log("[FossilInputHandler] RIGHT BUTTON clicked");
                TriggerCorrect();
            });
        }
        
        Debug.Log("[FossilInputHandler] Touch buttons setup - always active");
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
                Debug.Log("[FossilInputHandler] Accelerometer action enabled");
            }
        }
        else
        {
            Debug.LogWarning("[FossilInputHandler] No Input Actions assigned! Using button fallback only.");
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
                Debug.Log("[FossilInputHandler] Accelerometer device found and enabled");
                
                if (Accelerometer.current.samplingFrequency > 0)
                {
                    Debug.Log($"[FossilInputHandler] Accelerometer sampling frequency: {Accelerometer.current.samplingFrequency} Hz");
                }
            }
            else
            {
                Debug.LogWarning("[FossilInputHandler] Accelerometer.current is null - device may not support accelerometer");
            }
        }
        else
        {
            Debug.LogWarning("[FossilInputHandler] Device does not support accelerometer according to SystemInfo");
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
                Debug.Log($"[FossilInputHandler] Input Action test reading: {testReading}");
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
                Debug.Log("[FossilInputHandler] Using direct Accelerometer access as fallback");
            }
        }
        
        // Methode 3 (Legacy Fallback): Input.acceleration
        if (!accelerometerAvailable)
        {
            Invoke(nameof(CheckLegacyAccelerometer), 0.5f);
        }
        
        Debug.Log($"[FossilInputHandler] Platform: {Application.platform}");
        Debug.Log($"[FossilInputHandler] SystemInfo.supportsAccelerometer: {SystemInfo.supportsAccelerometer}");
        Debug.Log($"[FossilInputHandler] Accelerometer available: {accelerometerAvailable}");
        Debug.Log($"[FossilInputHandler] Using direct accelerometer: {useDirectAccelerometer}");
        Debug.Log($"[FossilInputHandler] Force touch mode: {forceTouchMode}");
        Debug.Log($"[FossilInputHandler] Allow dual input: {allowDualInput}");
    }
    
    void CheckLegacyAccelerometer()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        if (!accelerometerAvailable && Input.acceleration != Vector3.zero)
        {
            Debug.Log("[FossilInputHandler] Legacy Input.acceleration available - using as fallback");
            accelerometerAvailable = true;
            useDirectAccelerometer = true;
        }
#pragma warning restore CS0618
    }
    
    void Update()
    {
        // KRITISCH: Prüfe Accelerometer nur wenn Input auch ENABLED ist
        if (!inputEnabled)
        {
            // WICHTIG: Kein Accelerometer-Check wenn disabled!
            return;
        }
        
        // Prüfe Accelerometer nur wenn verfügbar UND nicht im Force-Touch-Modus
        if (accelerometerAvailable && !forceTouchMode)
        {
            CheckTiltInput();
        }
    }
    
    void CheckTiltInput()
    {
        // Globaler Cooldown-Check
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
                Debug.LogWarning("[FossilInputHandler] Zero acceleration reading");
            }
            return;
        }
        
        if (!hasInitialReading)
        {
            initialAcceleration = acceleration;
            hasInitialReading = true;
            isInNeutralZone = true; // ? NEU: Starte in neutraler Zone
            
            if (debugMode)
            {
                Debug.Log($"[FossilInputHandler] Initial acceleration set: {initialAcceleration}");
            }
            return;
        }
        
        Vector3 deltaAccel = acceleration - initialAcceleration;
        
        if (debugMode)
        {
            Debug.Log($"[FossilInputHandler] Acceleration: {acceleration} | Delta Z: {deltaAccel.z:F2} | Threshold: {tiltThreshold} | Neutral: {isInNeutralZone}");
        }
        
        // ? NEU: Prüfe ob wir in der neutralen Zone sind
        if (Mathf.Abs(deltaAccel.z) < NEUTRAL_ZONE_THRESHOLD)
        {
            isInNeutralZone = true;
        }
        
        // ? NEU: Input nur triggern wenn wir AUS der neutralen Zone kommen
        if (!isInNeutralZone)
        {
            // Handy ist bereits gekippt - warte bis es zurück kommt
            return;
        }
        
        // Nach vorne neigen (Handy von Stirn weg) = Richtig
        if (deltaAccel.z > tiltThreshold)
        {
            Debug.Log("[FossilInputHandler] TILT FORWARD detected!");
            isInNeutralZone = false; // ? NEU: Verlasse neutrale Zone
            TriggerCorrect();
        }
        // Nach hinten neigen (Handy zur Stirn hin) = Überspringen  
        else if (deltaAccel.z < -tiltThreshold)
        {
            Debug.Log("[FossilInputHandler] TILT BACKWARD detected!");
            isInNeutralZone = false; // ? NEU: Verlasse neutrale Zone
            TriggerSkip();
        }
    }
    
    public void TriggerCorrect()
    {
        Debug.Log($"[FossilInputHandler] TriggerCorrect() called - inputEnabled: {inputEnabled}, cooldown: {Time.time - lastInputTime}");
        
        if (!inputEnabled || Time.time - lastInputTime < INPUT_COOLDOWN) 
        {
            Debug.Log("[FossilInputHandler] Input BLOCKED - cooldown active or input disabled");
            return;
        }
        
        Debug.Log("[FossilInputHandler] CORRECT! - Processing input");
        lastInputTime = Time.time;
        
        // ? HAPTIC VOR DisableInputTemporarily
        Debug.Log("[FossilInputHandler] Triggering haptic feedback");
        TriggerHapticFeedback();
        
        // ? DisableInputTemporarily VOR Event!
        DisableInputTemporarily(0.5f);
        
        // ? Event als LETZTES auslösen
        OnCorrectInput?.Invoke();
    }
    
    public void TriggerSkip()
    {
        Debug.Log($"[FossilInputHandler] TriggerSkip() called - inputEnabled: {inputEnabled}, cooldown: {Time.time - lastInputTime}");
        
        if (!inputEnabled || Time.time - lastInputTime < INPUT_COOLDOWN)
        {
            Debug.Log("[FossilInputHandler] Input BLOCKED - cooldown active or input disabled");
            return;
        }
        
        Debug.Log("[FossilInputHandler] SKIP! - Processing input");
        lastInputTime = Time.time;
        
        // ? HAPTIC VOR DisableInputTemporarily
        Debug.Log("[FossilInputHandler] Triggering haptic feedback");
        TriggerHapticFeedback();
        
        // ? DisableInputTemporarily VOR Event!
        DisableInputTemporarily(0.5f);
        
        // ? Event als LETZTES auslösen
        OnSkipInput?.Invoke();
    }
    
    void TriggerHapticFeedback()
    {
#if UNITY_ANDROID || UNITY_IOS
        Debug.Log("[FossilInputHandler] >>> VIBRATING NOW <<<");
        Handheld.Vibrate();
#endif
    }
    
    void DisableInputTemporarily(float duration)
    {
        Debug.Log($"[FossilInputHandler] DisableInputTemporarily({duration}s)");
        inputEnabled = false;
        SetButtonsInteractable(false);
        
        Invoke(nameof(EnableInput), duration);
    }
    
    void EnableInput()
    {
        Debug.Log("[FossilInputHandler] EnableInput() - Input re-enabled");
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
        Debug.Log($"[FossilInputHandler] SetInputEnabled({enabled})");
        
        // ? NEU: Cancel alle geplanten EnableInput() Callbacks!
        CancelInvoke(nameof(EnableInput));
        
        inputEnabled = enabled;
        SetButtonsInteractable(enabled);
        
        if (enabled)
        {
            hasInitialReading = false;
            lastInputTime = 0f; // Reset Cooldown
            CheckAccelerometerAvailability();
        }
        else
        {
            // Bei Deaktivierung auch Initial Reading zurücksetzen
            hasInitialReading = false;
        }
    }
    
    public void CalibrateDevice()
    {
        Debug.Log("[FossilInputHandler] CalibrateDevice()");
        
        hasInitialReading = false;
        lastInputTime = 0f;
        isInNeutralZone = true; // ? NEU: Reset neutral zone
        
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
            
            Debug.Log($"[FossilInputHandler] Device calibrated - Initial: {initialAcceleration}");
        }
    }
    
    public bool IsUsingAccelerometer()
    {
        return accelerometerAvailable && !forceTouchMode;
    }
    
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
        Debug.Log("[FossilInputHandler] OnDestroy()");
        
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }
}