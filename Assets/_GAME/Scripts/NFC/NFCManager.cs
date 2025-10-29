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
    
    [Header("Scene Loading")]
    [SerializeField] private NFCSceneLoader sceneLoader;
    [SerializeField] private bool autoLoadSceneOnScan = true;
    
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
        UpdateStatus("Halte dein Geraet an den NFC-Tag");
        
        // Validiere Scene Loader
        if (sceneLoader == null)
        {
            sceneLoader = GetComponent<NFCSceneLoader>();
            if (sceneLoader == null)
            {
                Debug.LogError("[NFCManager] NFCSceneLoader fehlt!");
            }
        }
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        InitializeAndroid();
        #elif UNITY_IOS && !UNITY_EDITOR
        InitializeIOS();
        #endif
    }
    
    #if UNITY_ANDROID && !UNITY_EDITOR
    private void InitializeAndroid()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            nfcHelper = new AndroidJavaObject("com.yourcompany.nfc.NFCHelper");
            
            StartAndroidNFC();
        }
        catch (System.Exception e)
        {
            UpdateStatus($"Init Fehler: {e.Message}");
            Debug.LogError($"[NFCManager] Init Exception: {e}");
        }
    }
    
    private void StartAndroidNFC()
    {
        try
        {
            if (nfcHelper != null && currentActivity != null)
            {
                nfcHelper.Call("startNFCReading", currentActivity);
                UpdateStatus("NFC bereit - Tag ans Geraet halten");
                
                InvokeRepeating(nameof(CheckForNFCIntent), 0.5f, 0.5f);
            }
        }
        catch (System.Exception e)
        {
            UpdateStatus($"NFC Fehler: {e.Message}");
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
                if (action == lastProcessedAction) return;
                
                lastProcessedAction = action;
                Debug.Log("[NFCManager] NFC Intent erkannt: " + action);
                
                if (nfcHelper != null)
                {
                    nfcHelper.Call("handleIntent", intent);
                }
                
                AndroidJavaObject newIntent = new AndroidJavaObject("android.content.Intent");
                currentActivity.Call("setIntent", newIntent);
                
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
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!pauseStatus && nfcHelper != null && currentActivity != null)
        {
            nfcHelper.Call("startNFCReading", currentActivity);
            CancelInvoke(nameof(CheckForNFCIntent));
            InvokeRepeating(nameof(CheckForNFCIntent), 0.5f, 0.5f);
        }
        else if (pauseStatus)
        {
            CancelInvoke(nameof(CheckForNFCIntent));
        }
        #endif
    }
    
    void OnDestroy()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        CancelInvoke(nameof(CheckForNFCIntent));
        #endif
    }
    #endif
    
    #if UNITY_IOS && !UNITY_EDITOR
    private void InitializeIOS()
    {
        if (_IsNFCAvailable())
        {
            _StartNFCReading();
            UpdateStatus("iOS NFC bereit");
        }
        else
        {
            UpdateStatus("NFC nicht verfuegbar");
        }
    }
    #endif
    
    // Wird von nativem Code aufgerufen
    public void OnNFCTagRead(string tagData)
    {
        lastReadTag = tagData;
        
        if (debugText != null)
        {
            debugText.text = $"Raum erkannt:\n{tagData}";
        }
        
        UpdateStatus("Tag erfolgreich gelesen!");
        Debug.Log($"[NFCManager] Tag gelesen: {tagData}");
        
        // Automatisch Szene laden
        if (autoLoadSceneOnScan && sceneLoader != null)
        {
            if (sceneLoader.IsValidRoomTag(tagData))
            {
                UpdateStatus("Lade Raum...");
                sceneLoader.LoadSceneForTag(tagData);
            }
            else
            {
                UpdateStatus($"Unbekannter Raum: {tagData}");
                Debug.LogWarning($"[NFCManager] Kein Mapping gefunden für: {tagData}");
            }
        }
    }
    
    public void OnNFCStatus(string status)
    {
        UpdateStatus(status);
    }
    
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