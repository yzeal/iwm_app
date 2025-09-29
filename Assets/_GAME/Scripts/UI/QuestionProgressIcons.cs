using UnityEngine;
using UnityEngine.UI;

public class QuestionProgressIcons : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private Image[] progressIcons;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.2f, 0.8f, 0.2f); // Grün als Standard
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoCreateIcons = true;
    [SerializeField] private GameObject iconPrefab; // Optional: Eigenes Icon-Prefab
    
    private int totalQuestions;
    private int currentCompletedQuestions = 0;
    
    public void Initialize(int questionCount)
    {
        totalQuestions = questionCount;
        currentCompletedQuestions = 0;
        
        SetupIcons();
        ResetProgress();
    }
    
    void SetupIcons()
    {
        // Falls Auto-Create aktiviert ist und keine Icons vorhanden sind
        if (autoCreateIcons && (progressIcons == null || progressIcons.Length != totalQuestions))
        {
            CreateIcons();
        }
        
        // Stelle sicher, dass wir die richtige Anzahl Icons haben
        if (progressIcons == null || progressIcons.Length != totalQuestions)
        {
            Debug.LogWarning($"QuestionProgressIcons: Anzahl der Icons ({progressIcons?.Length ?? 0}) stimmt nicht mit Fragenanzahl ({totalQuestions}) überein!");
            return;
        }
    }
    
    void CreateIcons()
    {
        // Lösche existierende Icons
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        progressIcons = new Image[totalQuestions];
        
        for (int i = 0; i < totalQuestions; i++)
        {
            GameObject iconObj;
            
            if (iconPrefab != null)
            {
                iconObj = Instantiate(iconPrefab, transform);
            }
            else
            {
                // Erstelle Standard-Icon
                iconObj = new GameObject($"ProgressIcon_{i}");
                iconObj.transform.SetParent(transform, false);
                
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = null; // Wird als einfaches weißes Quadrat angezeigt
                
                // Standard-Größe setzen
                RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(20, 20);
            }
            
            progressIcons[i] = iconObj.GetComponent<Image>();
            
            if (progressIcons[i] == null)
            {
                Debug.LogError($"Icon {i} hat keine Image-Komponente!");
            }
        }
    }
    
    public void ResetProgress()
    {
        currentCompletedQuestions = 0;
        
        if (progressIcons == null) return;
        
        for (int i = 0; i < progressIcons.Length; i++)
        {
            if (progressIcons[i] != null)
            {
                progressIcons[i].color = defaultColor;
            }
        }
    }
    
    public void MarkQuestionCompleted()
    {
        if (currentCompletedQuestions >= totalQuestions || progressIcons == null) return;
        
        if (progressIcons[currentCompletedQuestions] != null)
        {
            progressIcons[currentCompletedQuestions].color = completedColor;
        }
        
        currentCompletedQuestions++;
    }
    
    public void SetProgress(int completedCount)
    {
        if (progressIcons == null) return;
        
        currentCompletedQuestions = Mathf.Clamp(completedCount, 0, totalQuestions);
        
        for (int i = 0; i < progressIcons.Length; i++)
        {
            if (progressIcons[i] != null)
            {
                progressIcons[i].color = i < currentCompletedQuestions ? completedColor : defaultColor;
            }
        }
    }
    
    // Editor-Hilfsmethode
    [ContextMenu("Setup Icons for Current Question Count")]
    void EditorSetupIcons()
    {
        if (!Application.isPlaying && totalQuestions > 0)
        {
            CreateIcons();
        }
    }
}