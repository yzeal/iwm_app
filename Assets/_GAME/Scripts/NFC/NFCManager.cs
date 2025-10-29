using UnityEngine;
using TMPro;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class NFCManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private string lastReadTag = "";
    
    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _StartNFCReading();
    
    [DllImport("__Internal")]
    private static extern bool _IsNFCAvailable();
    #endif
    
    #if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject nfcHelper;
    private AndroidJavaObject currentActivity;
    private string lastProcessedAction = "";
    #endif
    
    void Start()
    {
        UpdateStatus("NFC Manager bereit. Tippe zum Scannen.");
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        // NFC Helper initialisieren
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            nfcHelper = new AndroidJavaObject("com.yourcompany.nfc.NFCHelper");
            
            // Automatisch NFC aktivieren bei Start
            StartAndroidNFC();
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Init Fehler: {e.Message}");
            Debug.LogError($"[NFCManager] Init Exception: {e}");
        }
        #endif
    }
    
    void Update()
    {
        // Touch zum manuellen Starten des Scans
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            StartNFCReading();
        }
    }
    
    public void StartNFCReading()
    {
        #if UNITY_EDITOR
        // Editor-Test
        OnNFCTagRead("TEST_ROOM_01");
        #elif UNITY_ANDROID
        StartAndroidNFC();
        #elif UNITY_IOS
        if (_IsNFCAvailable())
        {
            _StartNFCReading();
            UpdateStatus("iOS NFC Scan gestartet...");
        }
        else
        {
            UpdateStatus("NFC nicht verfuegbar auf diesem Geraet");
        }
        #endif
    }
    
    #if UNITY_ANDROID && !UNITY_EDITOR
    private void StartAndroidNFC()
    {
        try
        {
            if (nfcHelper != null && currentActivity != null)
            {
                nfcHelper.Call("startNFCReading", currentActivity);
                UpdateStatus("Android NFC wird aktiviert...");
                
                // Starte Polling fuer NFC Intents
                InvokeRepeating(nameof(CheckForNFCIntent), 0.5f, 0.5f);
            }
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Android NFC Fehler: {e.Message}");
            Debug.LogError($"[NFCManager] Exception: {e}");
        }
    }

    private void CheckForNFCIntent()
    {
        try
        {
            if (currentActivity == null) return;
            
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            if (intent == null) return;
            
            string action = intent.Call<string>("getAction");
            
            if (action != null && (action.Equals("android.nfc.action.TAG_DISCOVERED") || 
                                   action.Equals("android.nfc.action.NDEF_DISCOVERED")))
            {
                // Verhindere doppelte Verarbeitung
                if (action == lastProcessedAction)
                {
                    return;
                }
                
                lastProcessedAction = action;
                
                Debug.Log("[NFCManager] NFC Intent erkannt: " + action);
                
                if (nfcHelper != null)
                {
                    nfcHelper.Call("handleIntent", intent);
                }
                
                // Intent als verarbeitet markieren
                AndroidJavaObject newIntent = new AndroidJavaObject("android.content.Intent");
                currentActivity.Call("setIntent", newIntent);
                
                // Polling kurz pausieren und neu starten
                CancelInvoke(nameof(CheckForNFCIntent));
                Invoke(nameof(RestartPolling), 2.0f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NFCManager] CheckForNFCIntent Fehler: {e.Message}");
        }
    }

    private void RestartPolling()
    {
        lastProcessedAction = "";
        InvokeRepeating(nameof(CheckForNFCIntent), 0.5f, 0.5f);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && nfcHelper != null && currentActivity != null)
        {
            // NFC reaktivieren wenn App zurueckkommt
            nfcHelper.Call("startNFCReading", currentActivity);
            
            // Polling neu starten
            CancelInvoke(nameof(CheckForNFCIntent));
            InvokeRepeating(nameof(CheckForNFCIntent), 0.5f, 0.5f);
        }
        else if (pauseStatus)
        {
            // Polling stoppen wenn App pausiert
            CancelInvoke(nameof(CheckForNFCIntent));
        }
    }
    
    void OnDestroy()
    {
        CancelInvoke(nameof(CheckForNFCIntent));
    }
    #endif
    
    // Wird von nativem Code aufgerufen - fuer Tag-Daten
    public void OnNFCTagRead(string tagData)
    {
        lastReadTag = tagData;
        debugText.text = $"Gelesener Tag:\n{tagData}";
        UpdateStatus("Tag erfolgreich gelesen!");
        
        Debug.Log($"[NFCManager] Tag gelesen: {tagData}");
    }
    
    // Wird von nativem Code aufgerufen - fuer Status-Updates
    public void OnNFCStatus(string status)
    {
        UpdateStatus(status);
        Debug.Log($"[NFCManager] NFC Status: {status}");
    }
    
    // Wird bei Fehlern von nativem Code aufgerufen
    public void OnNFCError(string error)
    {
        UpdateStatus($"Fehler: {error}");
        Debug.LogError($"[NFCManager] NFC Fehler: {error}");
    }
    
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[NFCManager] {message}");
    }
}