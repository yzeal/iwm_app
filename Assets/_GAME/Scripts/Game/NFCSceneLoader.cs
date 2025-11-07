using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class NFCSceneLoader : MonoBehaviour
{
    [Header("Room Mapping")]
    [SerializeField] private NFCRoomMapping roomMapping;
    
    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeLoad = 1.5f;
    [SerializeField] private bool showLoadingScreen = false;
    
    [Header("Hub Scene")]
    [SerializeField] private string hubSceneName = "NFCTest";
    
    #if UNITY_EDITOR
    [Header("Editor Debug (nur im Editor)")]
    [SerializeField] private bool enableEditorDebug = true;
    [SerializeField] private bool requireModifier = false; // Wenn true, muss Ctrl gedrückt werden
    
    private Keyboard keyboard;
    #endif
    
    private bool isLoading = false;
    
    void Start()
    {
        if (roomMapping == null)
        {
            Debug.LogError("[NFCSceneLoader] NFCRoomMapping fehlt! Bitte im Inspector zuweisen.");
        }
        
        #if UNITY_EDITOR
        // Input System Keyboard initialisieren
        keyboard = Keyboard.current;
        
        if (enableEditorDebug && roomMapping != null)
        {
            Debug.Log("[NFCSceneLoader] ?? EDITOR DEBUG MODE aktiv!");
            Debug.Log($"[NFCSceneLoader] Drücke Tasten 0-9 um Räume zu laden (Modifier: {(requireModifier ? "Ctrl" : "Nicht erforderlich")})");
            PrintAvailableRooms();
        }
        #endif
    }
    
    #if UNITY_EDITOR
    void Update()
    {
        if (!enableEditorDebug || roomMapping == null || keyboard == null) return;
        
        // Prüfe ob Modifier gedrückt werden muss
        if (requireModifier && !keyboard.ctrlKey.isPressed) return;
        
        // Tasten 0-9 (Alpha-Keys)
        if (keyboard.digit0Key.wasPressedThisFrame) LoadRoomByIndex(0);
        else if (keyboard.digit1Key.wasPressedThisFrame) LoadRoomByIndex(1);
        else if (keyboard.digit2Key.wasPressedThisFrame) LoadRoomByIndex(2);
        else if (keyboard.digit3Key.wasPressedThisFrame) LoadRoomByIndex(3);
        else if (keyboard.digit4Key.wasPressedThisFrame) LoadRoomByIndex(4);
        else if (keyboard.digit5Key.wasPressedThisFrame) LoadRoomByIndex(5);
        else if (keyboard.digit6Key.wasPressedThisFrame) LoadRoomByIndex(6);
        else if (keyboard.digit7Key.wasPressedThisFrame) LoadRoomByIndex(7);
        else if (keyboard.digit8Key.wasPressedThisFrame) LoadRoomByIndex(8);
        else if (keyboard.digit9Key.wasPressedThisFrame) LoadRoomByIndex(9);
        
        // Numpad-Tasten (optional)
        else if (keyboard.numpad0Key.wasPressedThisFrame) LoadRoomByIndex(0);
        else if (keyboard.numpad1Key.wasPressedThisFrame) LoadRoomByIndex(1);
        else if (keyboard.numpad2Key.wasPressedThisFrame) LoadRoomByIndex(2);
        else if (keyboard.numpad3Key.wasPressedThisFrame) LoadRoomByIndex(3);
        else if (keyboard.numpad4Key.wasPressedThisFrame) LoadRoomByIndex(4);
        else if (keyboard.numpad5Key.wasPressedThisFrame) LoadRoomByIndex(5);
        else if (keyboard.numpad6Key.wasPressedThisFrame) LoadRoomByIndex(6);
        else if (keyboard.numpad7Key.wasPressedThisFrame) LoadRoomByIndex(7);
        else if (keyboard.numpad8Key.wasPressedThisFrame) LoadRoomByIndex(8);
        else if (keyboard.numpad9Key.wasPressedThisFrame) LoadRoomByIndex(9);
        
        // H-Taste für Hub
        else if (keyboard.hKey.wasPressedThisFrame)
        {
            Debug.Log("[NFCSceneLoader] ?? Debug: Lade Hub-Szene");
            ReturnToHub();
        }
        
        // P-Taste für Print Rooms (Liste nochmal anzeigen)
        else if (keyboard.pKey.wasPressedThisFrame)
        {
            PrintAvailableRooms();
        }
    }
    
    /// <summary>
    /// EDITOR DEBUG: Lädt Raum basierend auf Index im roomMapping
    /// </summary>
    private void LoadRoomByIndex(int index)
    {
        if (roomMapping == null)
        {
            Debug.LogError("[NFCSceneLoader] Room Mapping fehlt!");
            return;
        }
        
        string sceneName = GetSceneNameByIndex(index);
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[NFCSceneLoader] ?? Kein Raum für Index {index} gefunden!");
            return;
        }
        
        Debug.Log($"[NFCSceneLoader] ?? DEBUG: Lade Raum #{index} ? '{sceneName}'");
        StartCoroutine(LoadSceneCoroutine(sceneName, $"DEBUG_INDEX_{index}"));
    }
    
    /// <summary>
    /// Holt Szenen-Namen basierend auf Index (für Debug)
    /// </summary>
    private string GetSceneNameByIndex(int index)
    {
        if (roomMapping == null) return null;
        
        // Nutze Reflection um auf roomMappings zuzugreifen
        var field = typeof(NFCRoomMapping).GetField("roomMappings", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field == null)
        {
            Debug.LogError("[NFCSceneLoader] Konnte roomMappings nicht finden! Ist NFCRoomMapping korrekt?");
            return null;
        }
        
        var mappings = field.GetValue(roomMapping) as System.Array;
        
        if (mappings == null || index < 0 || index >= mappings.Length)
        {
            return null;
        }
        
        var entry = mappings.GetValue(index);
        var sceneNameField = entry.GetType().GetField("sceneName");
        
        return sceneNameField?.GetValue(entry) as string;
    }
    
    /// <summary>
    /// Zeigt alle verfügbaren Räume im Debug-Log
    /// </summary>
    private void PrintAvailableRooms()
    {
        if (roomMapping == null)
        {
            Debug.LogWarning("[NFCSceneLoader] Keine Room Mappings verfügbar");
            return;
        }
        
        Debug.Log("========== VERFÜGBARE RÄUME (EDITOR DEBUG) ==========");
        
        var field = typeof(NFCRoomMapping).GetField("roomMappings", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            var mappings = field.GetValue(roomMapping) as System.Array;
            
            if (mappings != null)
            {
                for (int i = 0; i < mappings.Length; i++)
                {
                    var entry = mappings.GetValue(i);
                    var sceneNameField = entry.GetType().GetField("sceneName");
                    var displayNameField = entry.GetType().GetField("displayName");
                    var nfcTagIdField = entry.GetType().GetField("nfcTagId");
                    
                    string sceneName = sceneNameField?.GetValue(entry) as string;
                    string displayName = displayNameField?.GetValue(entry) as string;
                    string tagId = nfcTagIdField?.GetValue(entry) as string;
                    
                    Debug.Log($"  [{i}] {displayName ?? sceneName} (Tag: {tagId})");
                }
                
                Debug.Log($"\n?? Drücke Taste {(requireModifier ? "Ctrl + " : "")}0-{mappings.Length - 1} zum Laden");
                Debug.Log("?? Drücke 'H' für Hub-Szene");
                Debug.Log("?? Drücke 'P' um diese Liste erneut anzuzeigen");
            }
        }
        
        Debug.Log("=".PadRight(50, '='));
    }
    #endif
    
    /// <summary>
    /// Lädt Szene basierend auf NFC-Tag
    /// </summary>
    public void LoadSceneForTag(string tagId)
    {
        if (isLoading)
        {
            Debug.LogWarning("[NFCSceneLoader] Szene wird bereits geladen");
            return;
        }
        
        if (roomMapping == null)
        {
            Debug.LogError("[NFCSceneLoader] Room Mapping fehlt!");
            return;
        }
        
        string sceneName = roomMapping.GetSceneNameForTag(tagId);
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[NFCSceneLoader] Keine Szene gefunden für Tag: {tagId}");
            return;
        }
        
        StartCoroutine(LoadSceneCoroutine(sceneName, tagId));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName, string tagId)
    {
        isLoading = true;
        
        Debug.Log($"[NFCSceneLoader] Lade Szene: {sceneName} für Tag: {tagId}");
        
        // Optional: GameDataManager informieren
        if (GameDataManager.Instance != null)
        {
            // Speichere aktuellen Raum (falls gewünscht)
            // GameDataManager.Instance.SetCurrentRoom(tagId);
        }
        
        // Warte kurz für UI-Feedback
        yield return new WaitForSeconds(delayBeforeLoad);
        
        // Lade Szene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        if (asyncLoad == null)
        {
            Debug.LogError($"[NFCSceneLoader] Szene '{sceneName}' konnte nicht geladen werden! Ist sie in Build Settings?");
            isLoading = false;
            yield break;
        }
        
        // Optional: Loading Progress
        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            Debug.Log($"[NFCSceneLoader] Loading progress: {progress * 100}%");
            yield return null;
        }
        
        Debug.Log($"[NFCSceneLoader] Szene '{sceneName}' erfolgreich geladen");
        isLoading = false;
    }
    
    /// <summary>
    /// Kehrt zur Hub-Szene zurück
    /// </summary>
    public void ReturnToHub()
    {
        if (isLoading)
        {
            Debug.LogWarning("[NFCSceneLoader] Szene wird bereits geladen");
            return;
        }
        
        Debug.Log($"[NFCSceneLoader] Kehre zurück zu Hub: {hubSceneName}");
        StartCoroutine(LoadSceneCoroutine(hubSceneName, "HUB"));
    }
    
    /// <summary>
    /// Prüft ob Tag ein gültiger Raum ist
    /// </summary>
    public bool IsValidRoomTag(string tagId)
    {
        if (roomMapping == null) return false;
        
        string sceneName = roomMapping.GetSceneNameForTag(tagId);
        return !string.IsNullOrEmpty(sceneName);
    }
}