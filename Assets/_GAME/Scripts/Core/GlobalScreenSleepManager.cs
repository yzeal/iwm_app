using UnityEngine;

/// <summary>
/// Globaler Manager der Screen Sleep für die GESAMTE App verhindert
/// Wird automatisch beim App-Start initialisiert und bleibt über Scene-Wechsel bestehen
/// </summary>
public class GlobalScreenSleepManager : MonoBehaviour
{
    private static GlobalScreenSleepManager instance;
    
    [Header("Sleep Prevention Settings")]
    [SerializeField] private bool preventSleepGlobally = true;
    [Tooltip("Verhindert Sperrbildschirm während der gesamten App-Laufzeit")]
    
    private int previousSleepTimeout;
    
    private void Awake()
    {
        // Singleton-Pattern - nur eine Instanz erlaubt
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject); // Überlebt Scene-Wechsel
        
        // Screen Sleep deaktivieren
        if (preventSleepGlobally)
        {
            previousSleepTimeout = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("[GlobalScreenSleepManager] Screen Sleep GLOBAL deaktiviert - bleibt für gesamte App aktiv");
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Bei App-Resume sicherstellen dass Sleep noch deaktiviert ist
        if (!pauseStatus && preventSleepGlobally)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("[GlobalScreenSleepManager] Screen Sleep nach Resume wiederhergestellt");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Optional: Original-Setting wiederherstellen beim Beenden
        // (Wird aber normalerweise vom OS automatisch gemacht)
        if (preventSleepGlobally)
        {
            Screen.sleepTimeout = previousSleepTimeout;
            Debug.Log("[GlobalScreenSleepManager] Screen Sleep beim Beenden wiederhergestellt");
        }
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            // Screen Sleep wiederherstellen wenn Manager zerstört wird
            Screen.sleepTimeout = previousSleepTimeout;
            Debug.Log("[GlobalScreenSleepManager] Screen Sleep beim Destroy wiederhergestellt");
        }
    }
    
    /// <summary>
    /// Dynamisches An/Ausschalten von Screen Sleep Prevention
    /// </summary>
    public static void SetSleepPrevention(bool prevent)
    {
        if (instance == null)
        {
            Debug.LogWarning("[GlobalScreenSleepManager] Keine Instanz vorhanden - kann Sleep Prevention nicht ändern");
            return;
        }
        
        instance.preventSleepGlobally = prevent;
        Screen.sleepTimeout = prevent ? SleepTimeout.NeverSleep : instance.previousSleepTimeout;
        Debug.Log($"[GlobalScreenSleepManager] Sleep Prevention {(prevent ? "aktiviert" : "deaktiviert")}");
    }
    
    /// <summary>
    /// Prüft ob Screen Sleep Prevention aktiv ist
    /// </summary>
    public static bool IsSleepPreventionActive()
    {
        return instance != null && instance.preventSleepGlobally;
    }
}