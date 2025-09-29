# PROJECT NOTES FÜR GITHUB COPILOT
## WICHTIGE HINWEISE FÜR ZUKÜNFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext für GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu können ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: In dieser Datei KEINE dreifachen Anführungszeichen oder Backticks verwenden - diese beenden den Code-Output im Chat und verhindern direktes Kopieren in Dokumente.

---

# MUSEUM QUIZ APP - PROJEKTÜBERSICHT

## PROJEKTBESCHREIBUNG
- **Ziel**: Mobile Quiz-App für Museum mit Split-Screen Multiplayer
- **Plattform**: Unity 6.2, Mobile (Portrait-Modus), eventuell Web-App
- **Zielgruppe**: Museumsbesucher, 2 Teams/Spieler pro Handy
- **Kontext**: Teil einer virtuellen Führung durch 6 Museumsräume

## TECHNISCHE DETAILS
- **Unity Version**: 6.2
- **Input System**: Neues Unity Input System
- **UI System**: Neues Unity UI System für verschiedene Screen-Ratios
- **C# Version**: 9.0
- **Target Framework**: .NET Framework 4.7.1

## GAME DESIGN - SPLIT-SCREEN QUIZ

### KERNMECHANIK
- **Layout**: Portrait-Modus, vertikal gespiegelt
- **Spieler**: 2 Teams an einem Gerät
- **UI-Aufteilung**: Jeder Spieler hat eine Bildschirmhälfte, andere Hälfte steht kopf
- **Anti-Schummel**: Gleiche Fragen, aber zufällig angeordnete Antworten pro Spieler

### PUNKTESYSTEM
- **Richtige Antwort (Erster)**: 2 Punkte
- **Richtige Antwort (Zweiter)**: 1 Punkt  
- **Falsche Antwort**: 0 Punkte
- **Zeitlimit**: Keines
- **Fortschritt**: Touch nach beiden Antworten ? nächste Frage

### SPIELABLAUF
1. Beide Spieler erhalten gleiche Frage
2. 4 Antwortmöglichkeiten (1 richtig, 3 falsch) in unterschiedlicher Reihenfolge
3. Simultane Antwortphase
4. Feedback mit Punkteverteilung
5. Continue per Touch (anywhere auf Screen)
6. Ergebnisscreen nach letzter Frage

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN (ALLE IMPLEMENTIERT UND FUNKTIONSFÄHIG)

**SplitScreenQuizManager.cs**
- Zentrale Spiellogik und UI-Management
- Punkteberechnung basierend auf Antwortzeit
- Feedback-System für beide Spieler mit anpassbaren Farben
- Übergang zwischen Fragen und Ergebnisscreen
- Integration mit GameDataManager für persistente Speicherung

**TouchInputHandler.cs**  
- Touch-Eingaben für beide Spielerbereiche
- Continue-Funktionalität per Touch anywhere
- Bereichserkennung für Player 1/2 Touch Areas

**QuestionProgressIcons.cs**
- Icon-basiertes Progress-System (ersetzt Text-Counter)
- Horizontale Anzeige mit konfigurierbaren Farben
- Auto-Create Funktionalität für Icons
- Editor-anpassbare Farben für default/completed States

**GameDataManager.cs (NEU)**
- Plattformunabhängiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern für globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- Raumspezifische Fragen und Metadaten
- Modularer Aufbau für verschiedene Museumsräume
- Einfache Content-Erweiterung

**QuizQuestion.cs**
- Fragendatenstruktur mit 4 Antworten
- Automatisches Answer-Shuffling
- Korrekte Antwort Index-Tracking

**PlayerData.cs**
- Spielerstate (Name, Score, hasAnswered, etc.)
- Antwort-Tracking und Score-Management

### UI-STRUKTUR (BEREITS IMPLEMENTIERT)
- **Split-Screen Layout**: Funktionsfähige gespiegelte Darstellung
- **Touch Areas**: Klar getrennte Eingabebereiche
- **Answer Buttons**: 4 Buttons pro Spieler mit konfigurierbarem Color-Feedback
- **Question Progress Icons**: Icon-basierte Progress-Anzeige statt Text
- **Feedback System**: Anpassbare Farben für richtig/falsch/ausgewählt
- **Score Display**: Live-Update während Spiel
- **Continue Mechanism**: Touch-anywhere System nach beiden Antworten

## AKTUELLER PROJEKTSTATUS

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben
- Plattformunabhängiges Save-System für Spielerfortschritt
- Ergebnisscreen mit automatischer Datenspeicherung
- Modular structure für verschiedene Räume

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (POTENTIELL)
- Integration in größeres Führungssystem
- Weitere Minispiel-Prototypen
- Content-Integration mit echten Museumsfragen
- Cloud-Save Erweiterung (Firebase/PlayFab)
- Performance-Optimierung für mobile Geräte
- Web-App Export Testing

### ?? PROJEKTSTRUKTUR
Assets/_GAME/
??? Scripts/
?   ??? UI/
?   ?   ??? TouchInputHandler.cs
?   ?   ??? QuestionProgressIcons.cs
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs
?   ?   ??? PlayerData.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs
?   ?   ??? QuizRoomData.cs
?   ?   ??? QuizQuestion.cs
?   ??? Notes/
?       ??? Notes.md
??? ...

### ?? DEVELOPMENT NOTES
- **Modularity**: Jeder Museumsraum kann eigene Szene haben
- **Content Management**: QuizRoomData ScriptableObjects für einfache Fragenerstellung
- **Skalierbarkeit**: System designed für 6 verschiedene Räume
- **Touch-System**: Funktioniert mit Unity's neuem Input System
- **Save System**: Plattformunabhängig mit Backup-Funktionalität
- **Color Management**: Alle UI-Farben editor-anpassbar

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### ANTI-SCHUMMEL-MECHANISMUS
- Jeder Spieler erhält shuffled answers über QuizQuestion.GetShuffledAnswers()
- Tracking der korrekten Antwort per GetCorrectAnswerIndex()

### TIMING-SYSTEM
- Antwortzeit wird per Time.time gemessen
- Punkteverteilung basiert auf player1AnswerTime vs player2AnswerTime
- Berücksichtigt auch Korrektheit beider Antworten

### UI-MANAGEMENT
- Dynamische Button-Beschriftung
- Color-Highlighting mit disabled-Color-System für saubere Darstellung
- Automatische UI-Updates nach Antworten
- Icon-basierte Progress-Anzeige mit auto-create Funktionalität

### SAVE-SYSTEM DETAILS
- **Primärer Storage**: PlayerPrefs mit JSON-Serialisierung
- **Backup-System**: Automatisches Backup bei jedem Save
- **Plattform-Support**: Mobile (nativer Storage), Web (LocalStorage), Desktop (Registry)
- **Datenstruktur**: GameProgressData mit RoomResult-Liste
- **Session-Tracking**: Unique Session-IDs für Analytics
- **Error-Handling**: Try-Catch mit Fallback-Mechanismen

### TOUCH-SYSTEM DETAILS
- **Continue-Mechanismus**: Touch anywhere auf Screen nach Feedback-Phase
- **Button-Interaktion**: Disabled-Color System verhindert multiple Clicks
- **Visual Feedback**: Grau ? Grün/Rot ? Continue State
- **Platform-Agnostic**: Funktioniert mit Mouse und Touch

---

**LETZTER STATUS**: Split-Screen Quiz vollständig funktional mit persistentem Save-System. Prototyp bereit für Content-Integration und Erweiterung um weitere Minispiele. Plattformunabhängige Speicherung implementiert.