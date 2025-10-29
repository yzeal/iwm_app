using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NFCSceneLoader : MonoBehaviour
{
    [Header("Room Mapping")]
    [SerializeField] private NFCRoomMapping roomMapping;
    
    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeLoad = 1.5f;
    [SerializeField] private bool showLoadingScreen = false;
    
    [Header("Hub Scene")]
    [SerializeField] private string hubSceneName = "NFCTest";
    
    private bool isLoading = false;
    
    void Start()
    {
        if (roomMapping == null)
        {
            Debug.LogError("[NFCSceneLoader] NFCRoomMapping fehlt! Bitte im Inspector zuweisen.");
        }
    }
    
    /// <summary>
    /// L�dt Szene basierend auf NFC-Tag
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
            Debug.LogWarning($"[NFCSceneLoader] Keine Szene gefunden f�r Tag: {tagId}");
            return;
        }
        
        StartCoroutine(LoadSceneCoroutine(sceneName, tagId));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName, string tagId)
    {
        isLoading = true;
        
        Debug.Log($"[NFCSceneLoader] Lade Szene: {sceneName} f�r Tag: {tagId}");
        
        // Optional: GameDataManager informieren
        if (GameDataManager.Instance != null)
        {
            // Speichere aktuellen Raum (falls gew�nscht)
            // GameDataManager.Instance.SetCurrentRoom(tagId);
        }
        
        // Warte kurz f�r UI-Feedback
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
    /// Kehrt zur Hub-Szene zur�ck
    /// </summary>
    public void ReturnToHub()
    {
        if (isLoading)
        {
            Debug.LogWarning("[NFCSceneLoader] Szene wird bereits geladen");
            return;
        }
        
        Debug.Log($"[NFCSceneLoader] Kehre zur�ck zu Hub: {hubSceneName}");
        StartCoroutine(LoadSceneCoroutine(hubSceneName, "HUB"));
    }
    
    /// <summary>
    /// Pr�ft ob Tag ein g�ltiger Raum ist
    /// </summary>
    public bool IsValidRoomTag(string tagId)
    {
        if (roomMapping == null) return false;
        
        string sceneName = roomMapping.GetSceneNameForTag(tagId);
        return !string.IsNullOrEmpty(sceneName);
    }
}