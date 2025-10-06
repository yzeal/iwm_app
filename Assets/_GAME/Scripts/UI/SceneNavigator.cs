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
    /// �ffnet das Team-Settings-Men�
    /// </summary>
    public void OpenTeamSettings()
    {
        PlayNavigationSound();
        
        // Versuche zuerst mit Scene-Namen, falls das fehlschl�gt mit Index
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
    /// Geht zur�ck zum Hauptmen�
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
    /// L�dt eine Szene per Name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        PlayNavigationSound();
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// L�dt eine Szene per Index
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