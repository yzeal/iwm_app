using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [Header("Scene Names")]
    public string settingsSceneName = "TeamSettings";
    public string mainMenuSceneName = "MainMenu";
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip navigationSound;
    
    /// <summary>
    /// Öffnet das Team-Settings-Menü
    /// </summary>
    public void OpenTeamSettings()
    {
        PlayNavigationSound();
        
        // Versuche zuerst mit Scene-Namen, falls das fehlschlägt mit Index
        try
        {
            SceneManager.LoadScene(settingsSceneName);
        }
        catch
        {
            Debug.LogWarning($"Scene '{settingsSceneName}' nicht gefunden, lade Settings-Scene per Index...");
            // Fallback auf Scene-Index (muss angepasst werden je nach Setup)
            SceneManager.LoadScene("TeamSettings");
        }
    }
    
    /// <summary>
    /// Geht zurück zum Hauptmenü
    /// </summary>
    public void GoToMainMenu()
    {
        PlayNavigationSound();
        
        try
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        catch
        {
            // Fallback auf Szene 0
            SceneManager.LoadScene(0);
        }
    }
    
    /// <summary>
    /// Lädt eine Szene per Name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        PlayNavigationSound();
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// Lädt eine Szene per Index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        PlayNavigationSound();
        SceneManager.LoadScene(sceneIndex);
    }
    
    void PlayNavigationSound()
    {
        if (audioSource && navigationSound)
        {
            audioSource.PlayOneShot(navigationSound);
        }
    }
}