using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Room", menuName = "Quiz/Room Data")]
public class QuizRoomData : ScriptableObject
{
    [Header("Raum Information")]
    public string roomName;
    public int roomNumber;
    
    [Header("Quiz Fragen")]
    public QuizQuestion[] questions;
    
    [Header("UI Einstellungen")]
    public Color roomColor = Color.blue;
    public Sprite roomIcon;
}