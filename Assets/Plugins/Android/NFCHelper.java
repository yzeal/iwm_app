package com.yourcompany.nfc;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.nfc.tech.Ndef;
import android.nfc.NdefMessage;
import android.nfc.NdefRecord;
import android.os.Parcelable;
import android.util.Log;
import com.unity3d.player.UnityPlayer;

public class NFCHelper {
    private static final String TAG = "NFCHelper";
    private NfcAdapter nfcAdapter;
    private PendingIntent pendingIntent;
    private Activity currentActivity;
    
    public void startNFCReading(Activity activity) {
        currentActivity = activity;
        nfcAdapter = NfcAdapter.getDefaultAdapter(activity);
        
        if (nfcAdapter == null) {
            UnityPlayer.UnitySendMessage("NFCManager", "OnNFCError", 
                "NFC nicht verfuegbar");
            return;
        }
        
        if (!nfcAdapter.isEnabled()) {
            UnityPlayer.UnitySendMessage("NFCManager", "OnNFCError", 
                "NFC ist deaktiviert. Bitte in Einstellungen aktivieren.");
            return;
        }
        
        Intent intent = new Intent(activity, activity.getClass());
        intent.addFlags(Intent.FLAG_ACTIVITY_SINGLE_TOP);
        pendingIntent = PendingIntent.getActivity(activity, 0, intent, 
            PendingIntent.FLAG_MUTABLE);
        
        nfcAdapter.enableForegroundDispatch(activity, pendingIntent, null, null);
        
        UnityPlayer.UnitySendMessage("NFCManager", "OnNFCStatus", 
            "NFC Scan bereit - Tag ans Geraet halten");
    }
    
    public void stopNFCReading() {
        if (nfcAdapter != null && currentActivity != null) {
            nfcAdapter.disableForegroundDispatch(currentActivity);
        }
    }
    
    public void handleIntent(Intent intent) {
        String action = intent.getAction();
        Log.d(TAG, "handleIntent called with action: " + action);
        
        if (NfcAdapter.ACTION_TAG_DISCOVERED.equals(action) ||
            NfcAdapter.ACTION_NDEF_DISCOVERED.equals(action)) {
            
            Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
            
            if (tag == null) {
                Log.e(TAG, "Tag is null in intent!");
                UnityPlayer.UnitySendMessage("NFCManager", "OnNFCError", "Tag ist null");
                return;
            }
            
            // WICHTIG: Versuche zuerst NDEF Message direkt aus Intent zu lesen
            String tagData = readTagFromIntent(intent, tag);
            
            Log.d(TAG, "Sending tag data to Unity: " + tagData);
            UnityPlayer.UnitySendMessage("NFCManager", "OnNFCTagRead", tagData);
        }
    }
    
    /**
     * Liest Tag-Daten - zuerst aus Intent, dann direkt vom Tag
     */
    private String readTagFromIntent(Intent intent, Tag tag) {
        Log.d(TAG, "=== Start Tag Reading ===");
        
        // METHODE 1: Lese NDEF Message direkt aus Intent (bevorzugt!)
        Parcelable[] rawMessages = intent.getParcelableArrayExtra(NfcAdapter.EXTRA_NDEF_MESSAGES);
        
        if (rawMessages != null && rawMessages.length > 0) {
            Log.d(TAG, "NDEF Messages found in Intent: " + rawMessages.length);
            
            NdefMessage message = (NdefMessage) rawMessages[0];
            NdefRecord[] records = message.getRecords();
            
            Log.d(TAG, "NDEF Records in Intent: " + records.length);
            
            if (records.length > 0) {
                String text = parseNdefTextRecord(records[0]);
                if (text != null && !text.isEmpty()) {
                    Log.d(TAG, "NDEF Text from Intent: " + text);
                    return text;
                }
            }
        } else {
            Log.d(TAG, "No NDEF Messages in Intent");
        }
        
        // METHODE 2: Versuche direkt vom Tag zu lesen (Fallback)
        String tagData = readTagDirect(tag);
        if (tagData != null) {
            return tagData;
        }
        
        // METHODE 3: Finale Fallback - Tag ID
        return getTagIdAsHex(tag);
    }
    
    /**
     * Versucht direkt vom Tag zu lesen (kann "out of date" Fehler werfen)
     */
    private String readTagDirect(Tag tag) {
        Ndef ndef = Ndef.get(tag);
        
        if (ndef == null) {
            Log.w(TAG, "Tag ist NICHT NDEF-formatiert");
            return null;
        }
        
        Log.d(TAG, "Tag ist NDEF-formatiert - versuche direkte Verbindung");
        
        try {
            ndef.connect();
            
            if (!ndef.isConnected()) {
                Log.w(TAG, "Tag connection failed");
                return null;
            }
            
            NdefMessage message = ndef.getNdefMessage();
            
            if (message == null) {
                Log.w(TAG, "NDEF Message ist NULL");
                ndef.close();
                return null;
            }
            
            NdefRecord[] records = message.getRecords();
            Log.d(TAG, "NDEF Records from direct read: " + records.length);
            
            if (records.length > 0) {
                String text = parseNdefTextRecord(records[0]);
                ndef.close();
                
                if (text != null && !text.isEmpty()) {
                    Log.d(TAG, "NDEF Text from direct read: " + text);
                    return text;
                }
            }
            
            ndef.close();
            
        } catch (Exception e) {
            Log.e(TAG, "NDEF Direct Read Error: " + e.getMessage());
            // Nicht als Fehler an Unity senden - ist erwartbar
        }
        
        return null;
    }
    
    /**
     * Parsed NDEF Text Record
     */
    private String parseNdefTextRecord(NdefRecord record) {
        byte[] payload = record.getPayload();
        
        if (payload == null || payload.length == 0) {
            Log.w(TAG, "Payload ist leer");
            return null;
        }
        
        Log.d(TAG, "Payload length: " + payload.length);
        
        // NDEF Text Record Format:
        // Byte 0: Status byte (bit 7 = encoding, bit 6 = reserved, bit 5-0 = language length)
        // Byte 1-n: Language code (z.B. "en")
        // Byte n+1-end: Text
        
        try {
            int statusByte = payload[0] & 0xFF;
            int languageCodeLength = statusByte & 0x3F;
            
            Log.d(TAG, "Status byte: " + statusByte);
            Log.d(TAG, "Language code length: " + languageCodeLength);
            
            if (payload.length > languageCodeLength + 1) {
                String text = new String(payload, languageCodeLength + 1, 
                    payload.length - languageCodeLength - 1, "UTF-8");
                
                Log.d(TAG, "Parsed NDEF Text: " + text);
                return text;
            } else {
                // Fallback: Komplettes Payload als String
                String text = new String(payload, "UTF-8");
                Log.d(TAG, "Raw payload as text: " + text);
                return text;
            }
        } catch (Exception e) {
            Log.e(TAG, "Error parsing NDEF record: " + e.getMessage());
            return null;
        }
    }
    
    /**
     * Tag-ID als Hex-String
     */
    private String getTagIdAsHex(Tag tag) {
        byte[] id = tag.getId();
        StringBuilder hexString = new StringBuilder();
        for (byte b : id) {
            hexString.append(String.format("%02X", b));
        }
        String result = "TAG_ID:" + hexString.toString();
        Log.d(TAG, "Using Tag-ID: " + result);
        return result;
    }
}