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

## GAME DESIGN - FOSSILIEN-STIRNRATEN

### KERNMECHANIK
- **Gameplay**: Heads-Up Style Ratespiel mit Handy an der Stirn
- **Teams**: 2 Teams spielen abwechselnd (Team 1, dann Team 2)
- **Rundendauer**: Konfigurierbar (Standard: 30 Sekunden)
- **Fossilien pro Runde**: Konfigurierbar (Standard: 3-5)

### STEUERUNG
- **Accelerometer**: Handy nach vorne neigen = Richtig erraten
- **Accelerometer**: Handy nach hinten neigen = Überspringen
- **Touch-Fallback**: Links tippen = Überspringen, Rechts tippen = Richtig
- **Platform-Detection**: Automatischer Wechsel zwischen Accelerometer und Touch

### SPIELABLAUF
1. Explanation Screen mit Regeln und Team-Bild
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Ratephase mit Timer-Countdown in letzten 3 Sekunden
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung

### FEATURES
- **Fossil-Recycling**: Übersprungene Fossilien kommen später wieder
- **Timer-Warnung**: Audio-Countdown bei 3-2-1 Sekunden verbleibend
- **Team-Images**: Visuelle Team-Darstellung statt Text
- **Score-Tracking**: Einfache X/Y Anzeige während Spiel

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (VOLLSTÄNDIG IMPLEMENTIERT)

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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (VOLLSTÄNDIG IMPLEMENTIERT)

**FossilGameManager.cs**
- Zentrale Spiellogik für Heads-Up Style Gameplay
- Team-basiertes Spiel mit abwechselnden Runden
- Timer-System mit Audio-Countdown in letzten Sekunden
- Fossil-Recycling System für übersprungene Begriffe
- Integration mit GameDataManager für Score-Persistence

**FossilInputHandler.cs**
- Accelerometer-Eingabe mit neuem Unity Input System
- Touch-Fallback für Web/Testing
- Platform-Detection und automatische Input-Methoden-Wahl
- Tilt-Threshold konfigurierbar im Editor

**FossilData.cs & FossilCollection.cs (ScriptableObject)**
- Datenstruktur für Fossil-Begriffe und Bilder
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System für zufällige Fossil-Auswahl

### SHARED SYSTEMS (PLATTFORMÜBERGREIFEND)

**GameDataManager.cs**
- Plattformunabhängiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern für globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**FullscreenManager.cs & UniversalFullscreenButton.cs**
- Plattformunabhängiges Fullscreen-System
- Plug-and-Play Button-Script für beliebige UI-Buttons
- WebGL JavaScript Integration für Browser-Fullscreen
- Auto-Hide auf nicht-unterstützten Plattformen
- Icon-basierte UI (kein Text)

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

## AKTUELLER PROJEKTSTATUS

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Split-Screen Quiz
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben

#### Fossilien-Stirnraten
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling für übersprungene Begriffe
- Touch-Fallback für Web-Builds
- Team-Image System statt Text-Labels

#### Shared Systems
- Plattformunabhängiges Save-System für Spielerfortschritt
- Universal Fullscreen-System für alle Szenen
- Modular structure für verschiedene Räume
- WebGL-kompatible Input-Systeme

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (POTENTIELL)
- Integration in größeres Führungssystem
- Weitere Minispiel-Prototypen
- Content-Integration mit echten Museumsfragen
- Cloud-Save Erweiterung (Firebase/PlayFab)
- Performance-Optimierung für mobile Geräte
- UI-Polishing und Animation-System

### ?? PROJEKTSTRUKTUR
Assets/_GAME/
??? Scripts/
?   ??? UI/
?   ?   ??? TouchInputHandler.cs
?   ?   ??? QuestionProgressIcons.cs
?   ?   ??? FossilInputHandler.cs
?   ?   ??? FullscreenManager.cs
?   ?   ??? UniversalFullscreenButton.cs
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs
?   ?   ??? FossilGameManager.cs
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs
?   ?   ??? QuizRoomData.cs
?   ?   ??? QuizQuestion.cs
?   ?   ??? FossilData.cs
?   ?   ??? FossilCollection.cs
?   ??? Notes/
?       ??? Notes.md
??? Plugins/
?   ??? FullscreenWebGL.jslib
??? ...

### ?? DEVELOPMENT NOTES
- **Modularity**: Jeder Museumsraum kann eigene Szene haben
- **Content Management**: ScriptableObjects für einfache Content-Erstellung
- **Skalierbarkeit**: System designed für 6 verschiedene Räume
- **Input Systems**: Neues Unity Input System + Touch-Fallbacks
- **Save System**: Plattformunabhängig mit Backup-Funktionalität
- **Fullscreen**: Universal Button-System für alle Szenen
- **Web-Compatibility**: WebGL-optimierte Input-Systeme

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### SPLIT-SCREEN QUIZ DETAILS
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers()
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System für saubere Button-States
- **Continue-System**: Touch-anywhere nach Feedback-Phase

### FOSSILIEN-STIRNRATEN DETAILS  
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### SHARED SYSTEM DETAILS
- **Save-System**: PlayerPrefs + JSON + Backup-Mechanismus
- **Fullscreen**: WebGL JavaScript Integration + Platform-Detection
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile, Web, Desktop compatibility

### AUDIO-SYSTEM
- **Split-Screen Quiz**: Continue-Sounds, Feedback-Audio
- **Fossilien-Stirnraten**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

**LETZTER STATUS**: Zwei vollständig funktionsfähige Minispiel-Prototypen (Split-Screen Quiz + Fossilien-Stirnraten) mit shared Save-System und Universal Fullscreen-Funktionalität. Beide Spiele bereit für Content-Integration und Web-Deployment. Plattformunabhängige Input-Systeme implementiert.