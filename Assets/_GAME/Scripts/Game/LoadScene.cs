using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private string name;

    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadByNameDelayed(string sceneName)
    {
        name = sceneName;

        Invoke(nameof(DelayedLoad), 1f);
    }

    private void DelayedLoad()
    {
        SceneManager.LoadScene(name);
    }
}
