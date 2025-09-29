using UnityEngine;

[CreateAssetMenu(fileName = "New Fossil Collection", menuName = "FossilGame/Fossil Collection")]
public class FossilCollection : ScriptableObject
{
    [Header("Collection Info")]
    public string collectionName;
    public int roomNumber;

    [Header("Team Images")]
    public Sprite team1Image;
    public Sprite team2Image;

    [Header("Fossils")]
    public FossilData[] fossils;

    [Header("Game Settings")]
    [Range(3, 10)]
    public int fossilsPerRound = 5;
    [Range(15f, 120f)]
    public float roundDuration = 30f;

    public FossilData[] GetRandomFossils(int count)
    {
        if (fossils.Length <= count)
            return fossils;

        // Fisher-Yates Shuffle für zufällige Auswahl
        FossilData[] shuffled = new FossilData[fossils.Length];
        System.Array.Copy(fossils, shuffled, fossils.Length);

        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        FossilData[] result = new FossilData[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }
}