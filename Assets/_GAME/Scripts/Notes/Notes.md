# PROJECT NOTES F�R GITHUB COPILOT
## WICHTIGE HINWEISE F�R ZUK�NFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext f�r GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu k�nnen ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: In dieser Datei KEINE dreifachen Anf�hrungszeichen oder Backticks verwenden - diese beenden den Code-Output im Chat und verhindern direktes Kopieren in Dokumente.

---

# MUSEUM QUIZ APP - PROJEKT�BERSICHT

## PROJEKTBESCHREIBUNG
- **Ziel**: Mobile Quiz-App f�r Museum mit Split-Screen Multiplayer
- **Plattform**: Unity 6.2, Mobile (Portrait-Modus), eventuell Web-App
- **Zielgruppe**: Museumsbesucher, 2 Teams/Spieler pro Handy
- **Kontext**: Teil einer virtuellen F�hrung durch 6 Museumsr�ume

## TECHNISCHE DETAILS
- **Unity Version**: 6.2
- **Input System**: Neues Unity Input System
- **UI System**: Neues Unity UI System f�r verschiedene Screen-Ratios
- **C# Version**: 9.0
- **Target Framework**: .NET Framework 4.7.1

## GAME DESIGN - SPLIT-SCREEN QUIZ

### KERNMECHANIK
- **Layout**: Portrait-Modus, vertikal gespiegelt
- **Spieler**: 2 Teams an einem Ger�t
- **UI-Aufteilung**: Jeder Spieler hat eine Bildschirmh�lfte, andere H�lfte steht kopf
- **Anti-Schummel**: Gleiche Fragen, aber zuf�llig angeordnete Antworten pro Spieler

### PUNKTESYSTEM
- **Richtige Antwort (Erster)**: 2 Punkte
- **Richtige Antwort (Zweiter)**: 1 Punkt  
- **Falsche Antwort**: 0 Punkte
- **Zeitlimit**: Keines
- **Fortschritt**: Touch nach beiden Antworten ? n�chste Frage

### SPIELABLAUF
1. Beide Spieler erhalten gleiche Frage
2. 4 Antwortm�glichkeiten (1 richtig, 3 falsch) in unterschiedlicher Reihenfolge
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
- **Accelerometer**: Handy nach hinten neigen = �berspringen
- **Touch-Fallback**: Links tippen = �berspringen, Rechts tippen = Richtig
- **Platform-Detection**: Automatischer Wechsel zwischen Accelerometer und Touch

### SPIELABLAUF
1. Explanation Screen mit Regeln und Team-Bild
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Ratephase mit Timer-Countdown in letzten 3 Sekunden
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung

### FEATURES
- **Fossil-Recycling**: �bersprungene Fossilien kommen sp�ter wieder
- **Timer-Warnung**: Audio-Countdown bei 3-2-1 Sekunden verbleibend
- **Team-Images**: Visuelle Team-Darstellung statt Text
- **Score-Tracking**: Einfache X/Y Anzeige w�hrend Spiel

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (VOLLST�NDIG IMPLEMENTIERT)

**SplitScreenQuizManager.cs**
- Zentrale Spiellogik und UI-Management
- Punkteberechnung basierend auf Antwortzeit
- Feedback-System f�r beide Spieler mit anpassbaren Farben
- �bergang zwischen Fragen und Ergebnisscreen
- Integration mit GameDataManager f�r persistente Speicherung

**TouchInputHandler.cs**  
- Touch-Eingaben f�r beide Spielerbereiche
- Continue-Funktionalit�t per Touch anywhere
- Bereichserkennung f�r Player 1/2 Touch Areas

**QuestionProgressIcons.cs**
- Icon-basiertes Progress-System (ersetzt Text-Counter)
- Horizontale Anzeige mit konfigurierbaren Farben
- Auto-Create Funktionalit�t f�r Icons
- Editor-anpassbare Farben f�r default/completed States

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (VOLLST�NDIG IMPLEMENTIERT)

**FossilGameManager.cs**
- Zentrale Spiellogik f�r Heads-Up Style Gameplay
- Team-basiertes Spiel mit abwechselnden Runden
- Timer-System mit Audio-Countdown in letzten Sekunden
- Fossil-Recycling System f�r �bersprungene Begriffe
- Integration mit GameDataManager f�r Score-Persistence

**FossilInputHandler.cs**
- Accelerometer-Eingabe mit neuem Unity Input System
- Touch-Fallback f�r Web/Testing
- Platform-Detection und automatische Input-Methoden-Wahl
- Tilt-Threshold konfigurierbar im Editor

**FossilData.cs & FossilCollection.cs (ScriptableObject)**
- Datenstruktur f�r Fossil-Begriffe und Bilder
- Team-Images f�r visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System f�r zuf�llige Fossil-Auswahl

### SHARED SYSTEMS (PLATTFORM�BERGREIFEND)

**GameDataManager.cs**
- Plattformunabh�ngiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern f�r globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**FullscreenManager.cs & UniversalFullscreenButton.cs**
- Plattformunabh�ngiges Fullscreen-System
- Plug-and-Play Button-Script f�r beliebige UI-Buttons
- WebGL JavaScript Integration f�r Browser-Fullscreen
- Auto-Hide auf nicht-unterst�tzten Plattformen
- Icon-basierte UI (kein Text)

**QuizRoomData.cs (ScriptableObject)**
- Raumspezifische Fragen und Metadaten
- Modularer Aufbau f�r verschiedene Museumsr�ume
- Einfache Content-Erweiterung

**QuizQuestion.cs**
- Fragendatenstruktur mit 4 Antworten
- Automatisches Answer-Shuffling
- Korrekte Antwort Index-Tracking

**PlayerData.cs**
- Spielerstate (Name, Score, hasAnswered, etc.)
- Antwort-Tracking und Score-Management

## AKTUELLER PROJEKTSTATUS

### ? VOLLST�NDIG IMPLEMENTIERT UND FUNKTIONSF�HIG

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
- Fossil-Recycling f�r �bersprungene Begriffe
- Touch-Fallback f�r Web-Builds
- Team-Image System statt Text-Labels

#### Shared Systems
- Plattformunabh�ngiges Save-System f�r Spielerfortschritt
- Universal Fullscreen-System f�r alle Szenen
- Modular structure f�r verschiedene R�ume
- WebGL-kompatible Input-Systeme

### ?? N�CHSTE ENTWICKLUNGSSCHRITTE (POTENTIELL)
- Integration in gr��eres F�hrungssystem
- Weitere Minispiel-Prototypen
- Content-Integration mit echten Museumsfragen
- Cloud-Save Erweiterung (Firebase/PlayFab)
- Performance-Optimierung f�r mobile Ger�te
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
- **Content Management**: ScriptableObjects f�r einfache Content-Erstellung
- **Skalierbarkeit**: System designed f�r 6 verschiedene R�ume
- **Input Systems**: Neues Unity Input System + Touch-Fallbacks
- **Save System**: Plattformunabh�ngig mit Backup-Funktionalit�t
- **Fullscreen**: Universal Button-System f�r alle Szenen
- **Web-Compatibility**: WebGL-optimierte Input-Systeme

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### SPLIT-SCREEN QUIZ DETAILS
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers()
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System f�r saubere Button-States
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

**LETZTER STATUS**: Zwei vollst�ndig funktionsf�hige Minispiel-Prototypen (Split-Screen Quiz + Fossilien-Stirnraten) mit shared Save-System und Universal Fullscreen-Funktionalit�t. Beide Spiele bereit f�r Content-Integration und Web-Deployment. Plattformunabh�ngige Input-Systeme implementiert.