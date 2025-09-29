using UnityEngine;

public class FullscreenManager : MonoBehaviour
{
    [Header("Debug")]
    public bool debugMode = false;
    
    private static FullscreenManager _instance;
    public static FullscreenManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Suche existierenden Manager
                _instance = FindFirstObjectByType<FullscreenManager>();
                
                // Falls keiner existiert, erstelle einen
                if (_instance == null)
                {
                    GameObject fsManager = new GameObject("FullscreenManager");
                    _instance = fsManager.AddComponent<FullscreenManager>();
                    DontDestroyOnLoad(fsManager);
                    
                    if (_instance.debugMode)
                        Debug.Log("FullscreenManager: Created singleton instance");
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        // Singleton Pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (debugMode)
        {
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Fullscreen supported: {IsFullscreenSupported()}");
            Debug.Log($"Currently fullscreen: {IsCurrentlyFullscreen()}");
        }
    }
    
    public void ToggleFullscreen()
    {
        if (IsFullscreenSupported())
        {
            if (IsCurrentlyFullscreen())
            {
                ExitFullscreen();
            }
            else
            {
                EnterFullscreen();
            }
        }
        else
        {
            Debug.LogWarning("Fullscreen not supported on this platform");
        }
    }
    
    public void EnterFullscreen()
    {
        if (IsFullscreenSupported())
        {
            if (debugMode) Debug.Log("Entering Fullscreen...");
            
            Screen.fullScreen = true;
            
            // Für WebGL: JavaScript Fullscreen API
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                RequestFullscreenWebGL();
                #endif
            }
        }
    }
    
    public void ExitFullscreen()
    {
        if (IsFullscreenSupported())
        {
            if (debugMode) Debug.Log("Exiting Fullscreen...");
            
            Screen.fullScreen = false;
            
            // Für WebGL
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                ExitFullscreenWebGL();
                #endif
            }
        }
    }
    
    public bool IsFullscreenSupported()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer ||
               Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer ||
               Application.platform == RuntimePlatform.WindowsPlayer ||
               Application.platform == RuntimePlatform.OSXPlayer ||
               Application.platform == RuntimePlatform.LinuxPlayer;
    }
    
    public bool IsCurrentlyFullscreen()
    {
        return Screen.fullScreen;
    }
    
    public string GetFullscreenButtonText()
    {
        return IsCurrentlyFullscreen() ? "Vollbild verlassen" : "Vollbild";
    }
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void RequestFullscreenWebGL();
    
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void ExitFullscreenWebGL();
    #endif
}