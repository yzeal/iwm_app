package com.yourcompany.nfc;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.nfc.tech.Ndef;
import android.nfc.NdefMessage;
import android.nfc.NdefRecord;
import com.unity3d.player.UnityPlayer;

public class NFCHelper {
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
        
        // Sende Statusmeldung an OnNFCStatus statt OnNFCError
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
        
        if (NfcAdapter.ACTION_TAG_DISCOVERED.equals(action) ||
            NfcAdapter.ACTION_NDEF_DISCOVERED.equals(action)) {
            
            Tag tag = intent.getParcelableExtra(NfcAdapter.EXTRA_TAG);
            String tagData = readTag(tag);
            
            UnityPlayer.UnitySendMessage("NFCManager", "OnNFCTagRead", tagData);
        }
    }
    
    private String readTag(Tag tag) {
        // Versuche NDEF zu lesen
        Ndef ndef = Ndef.get(tag);
        if (ndef != null) {
            try {
                ndef.connect();
                NdefMessage message = ndef.getNdefMessage();
                if (message != null) {
                    NdefRecord[] records = message.getRecords();
                    
                    if (records.length > 0) {
                        byte[] payload = records[0].getPayload();
                        
                        // NDEF Text Record hat Language Code Prefix (z.B. "enHello")
                        if (payload.length > 3) {
                            int languageCodeLength = payload[0] & 0x3F;
                            return new String(payload, languageCodeLength + 1, 
                                payload.length - languageCodeLength - 1);
                        }
                        return new String(payload);
                    }
                }
                ndef.close();
            } catch (Exception e) {
                UnityPlayer.UnitySendMessage("NFCManager", "OnNFCError", 
                    "NDEF Fehler: " + e.getMessage());
            }
        }
        
        // Fallback: Tag ID als Hex-String
        byte[] id = tag.getId();
        StringBuilder hexString = new StringBuilder();
        for (byte b : id) {
            hexString.append(String.format("%02X", b));
        }
        return "TAG_ID:" + hexString.toString();
    }
}