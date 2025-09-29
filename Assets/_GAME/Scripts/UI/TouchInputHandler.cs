using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputHandler : MonoBehaviour, IPointerDownHandler
{
    [Header("Touch Areas")]
    public RectTransform player1TouchArea;
    public RectTransform player2TouchArea;
    
    private SplitScreenQuizManager quizManager;
    
    void Start()
    {
        quizManager = FindFirstObjectByType<SplitScreenQuizManager>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // Prüfe ob beide Spieler geantwortet haben und Continue-Button aktiv ist
        if (quizManager.continueButton.gameObject.activeInHierarchy)
        {
            // Beliebiger Touch aktiviert "Continue"
            quizManager.continueButton.onClick.Invoke();
            return;
        }
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint);
        
        // Bestimme welcher Spieler getippt hat basierend auf der Touch-Position
        if (RectTransformUtility.RectangleContainsScreenPoint(player1TouchArea, eventData.position, eventData.pressEventCamera))
        {
            Debug.Log("Touch in Player 1 Area");
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(player2TouchArea, eventData.position, eventData.pressEventCamera))
        {
            Debug.Log("Touch in Player 2 Area");
        }
    }
}