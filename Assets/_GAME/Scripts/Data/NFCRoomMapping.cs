using UnityEngine;

[CreateAssetMenu(fileName = "NFCRoomMapping", menuName = "Museum/NFC Room Mapping")]
public class NFCRoomMapping : ScriptableObject
{
    [System.Serializable]
    public class RoomEntry
    {
        [Tooltip("NFC Tag ID (z.B. 'ROOM_01' oder 'TAG_ID:04A3B2C1')")]
        public string nfcTagId;
        
        [Tooltip("Name der zu ladenden Szene")]
        public string sceneName;
        
        [Tooltip("Optional: Anzeigename für UI")]
        public string displayName;
        
        [Tooltip("Optional: Icon für Raum")]
        public Sprite roomIcon;
    }
    
    [Header("Room Mappings")]
    [SerializeField] private RoomEntry[] roomMappings;
    
    /// <summary>
    /// Findet Szenen-Namen basierend auf NFC-Tag
    /// </summary>
    public string GetSceneNameForTag(string tagId)
    {
        if (string.IsNullOrEmpty(tagId))
        {
            Debug.LogWarning("[NFCRoomMapping] Tag ID ist null oder leer");
            return null;
        }
        
        // Exakte Übereinstimmung
        foreach (var entry in roomMappings)
        {
            if (entry.nfcTagId.Equals(tagId, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"[NFCRoomMapping] Match gefunden: {tagId} ? {entry.sceneName}");
                return entry.sceneName;
            }
        }
        
        // Partial Match für TAG_ID: Format
        if (tagId.StartsWith("TAG_ID:"))
        {
            foreach (var entry in roomMappings)
            {
                if (entry.nfcTagId.StartsWith("TAG_ID:") && 
                    tagId.Contains(entry.nfcTagId.Substring(7, 4))) // Erste 4 Hex-Zeichen
                {
                    Debug.Log($"[NFCRoomMapping] Partial match gefunden: {tagId} ? {entry.sceneName}");
                    return entry.sceneName;
                }
            }
        }
        
        Debug.LogWarning($"[NFCRoomMapping] Keine Szene gefunden für Tag: {tagId}");
        return null;
    }
    
    /// <summary>
    /// Gibt Display-Namen für Tag zurück
    /// </summary>
    public string GetDisplayNameForTag(string tagId)
    {
        foreach (var entry in roomMappings)
        {
            if (entry.nfcTagId.Equals(tagId, System.StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrEmpty(entry.displayName) ? entry.sceneName : entry.displayName;
            }
        }
        return tagId;
    }
    
    /// <summary>
    /// Validiert ob alle Szenen existieren
    /// </summary>
    public bool ValidateMappings()
    {
        bool allValid = true;
        
        foreach (var entry in roomMappings)
        {
            if (string.IsNullOrEmpty(entry.nfcTagId))
            {
                Debug.LogError("[NFCRoomMapping] Entry hat keine Tag ID!");
                allValid = false;
            }
            
            if (string.IsNullOrEmpty(entry.sceneName))
            {
                Debug.LogError($"[NFCRoomMapping] Entry '{entry.nfcTagId}' hat keinen Szenen-Namen!");
                allValid = false;
            }
        }
        
        return allValid;
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Validate All Mappings")]
    private void ValidateInEditor()
    {
        if (ValidateMappings())
        {
            Debug.Log("[NFCRoomMapping] Alle Mappings sind valide!");
        }
    }
    #endif
}