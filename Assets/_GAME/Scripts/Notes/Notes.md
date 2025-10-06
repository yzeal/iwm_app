# PROJECT NOTES F�R GITHUB COPILOT
## WICHTIGE HINWEISE F�R ZUK�NFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext f�r GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu k�nnen ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: In dieser Datei KEINE dreifachen Anf�hrungszeichen oder Backticks verwenden - diese beenden den Code-Output im Chat und verhindern direktes Kopieren in Dokumente.

---

# MUSEUM QUIZ APP - PROJEKT�BERSICHT

## PROJEKTBESCHREIBUNG
- **Ziel**: Mobile Quiz-App f�r Museum mit Split-Screen Multiplayer
- **Plattform**: Unity 6.2, Mobile iOS/Android (Portrait-Modus) - UMSTELLUNG AUF MOBILE ERFOLGT
- **Zielgruppe**: Museumsbesucher, 2 Teams/Spieler pro Handy mit konfigurierbaren Schwierigkeitsgraden
- **Kontext**: Teil einer virtuellen F�hrung durch 6 Museumsr�ume

## TECHNISCHE DETAILS
- **Unity Version**: 6.2
- **Input System**: Neues Unity Input System
- **UI System**: Neues Unity UI System f�r verschiedene Screen-Ratios + Mobile-Optimierungen
- **C# Version**: 9.0
- **Target Framework**: .NET Framework 4.7.1
- **Platform**: iOS/Android (WebGL-Features entfernt)

## GAME DESIGN - SPLIT-SCREEN QUIZ

### KERNMECHANIK
- **Layout**: Portrait-Modus, vertikal gespiegelt
- **Spieler**: 2 Teams an einem Ger�t mit individuellen Schwierigkeitsgraden
- **UI-Aufteilung**: Jeder Spieler hat eine Bildschirmh�lfte, andere H�lfte steht kopf
- **Anti-Schummel**: Unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad + zuf�llig angeordnete Antworten

### SCHWIERIGKEITSGRADE (NEU IMPLEMENTIERT)
- **Kids**: Einfachere Sprache, weniger komplexe Fragen
- **BigKids**: Mittlere Komplexit�t
- **Adults**: Vollst�ndige Komplexit�t
- **Individuelle Auswahl**: Jedes Team kann unabh�ngig seinen Schwierigkeitsgrad w�hlen
- **Persistente Speicherung**: Einstellungen bleiben f�r gesamte F�hrung gespeichert

### PUNKTESYSTEM
- **Richtige Antwort (Erster)**: 2 Punkte
- **Richtige Antwort (Zweiter)**: 1 Punkt  
- **Falsche Antwort**: 0 Punkte
- **Zeitlimit**: Keines (kann bei Bedarf pro Schwierigkeitsgrad angepasst werden)
- **Fortschritt**: Touch nach beiden Antworten ? n�chste Frage

### SPIELABLAUF
1. Teams erhalten Fragen basierend auf ihren Schwierigkeitsgraden
2. 4 Antwortm�glichkeiten (1 richtig, 3 falsch) in unterschiedlicher Reihenfolge
3. Simultane Antwortphase
4. Feedback mit Punkteverteilung
5. Continue per Touch (anywhere auf Screen)
6. Ergebnisscreen nach letzter Frage

## GAME DESIGN - FOSSILIEN-STIRNRATEN

### KERNMECHANIK
- **Gameplay**: Heads-Up Style Ratespiel mit Handy an der Stirn
- **Teams**: 2 Teams spielen abwechselnd (Team 1, dann Team 2) mit individuellen Schwierigkeitsgraden
- **Rundendauer**: Konfigurierbar mit Zeit-Multiplikatoren pro Schwierigkeitsgrad
- **Fossilien pro Runde**: Konfigurierbar (Standard: 3-5)

### SCHWIERIGKEITSGRADE (NEU IMPLEMENTIERT)
- **Kids**: Einfachere Begriffe, l�nger Zeit (1.5x Multiplikator)
- **BigKids**: Mittlere Komplexit�t, etwas l�nger Zeit (1.2x Multiplikator)
- **Adults**: Vollst�ndige Komplexit�t, normale Zeit (1.0x Multiplikator)
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad

### STEUERUNG
- **Accelerometer**: Handy nach vorne neigen = Richtig erraten
- **Accelerometer**: Handy nach hinten neigen = �berspringen
- **Touch-Fallback**: Links tippen = �berspringen, Rechts tippen = Richtig
- **Platform-Detection**: Automatischer Wechsel zwischen Accelerometer und Touch
- **Haptic Feedback**: Mobile Vibration f�r bessere UX

### SPIELABLAUF
1. Explanation Screen mit Regeln, Team-Bild und Schwierigkeitsgrad-Anzeige
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Ratephase mit angepasster Timer-Dauer basierend auf Schwierigkeitsgrad
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung

### FEATURES
- **Fossil-Recycling**: �bersprungene Fossilien kommen sp�ter wieder
- **Timer-Warnung**: Audio-Countdown bei 3-2-1 Sekunden verbleibend
- **Team-Images**: Visuelle Team-Darstellung statt Text
- **Score-Tracking**: Einfache X/Y Anzeige w�hrend Spiel
- **Adaptive Zeiten**: Schwierigkeitsgrad-basierte Rundendauer

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (NEU IMPLEMENTIERT)

**DifficultySystem.cs**
- Enum DifficultyLevel (Kids, BigKids, Adults)
- TeamSettings Klasse f�r persistente Team-Konfiguration
- DifficultyTimeSettings f�r schwierigkeitsgrad-basierte Zeitmultiplikatoren
- Integration in GameDataManager f�r persistente Speicherung

**TeamSettingsManager.cs**
- Zentrale UI-Logik f�r Team-Schwierigkeitsgrad-Auswahl
- Mobile-optimierte UI mit Safe Area Support
- Haptic Feedback f�r iOS/Android
- Icon-basierte Team-Darstellung
- Apply/Back Button-System (Reset entfernt)

**DifficultyRadioGroup.cs**
- Radio-Button-System mit Image-Swapping statt Farbwechsel
- Checkbox-�hnliche Darstellung f�r bessere Mobile-UX
- Individual Sprite-Support pro Button
- Mobile Touch-Target-Optimierung
- Haptic Feedback Integration

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (ERWEITERT F�R SCHWIERIGKEITSGRADE)

**SplitScreenQuizManager.cs**
- ERWEITERT: L�dt unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad
- ERWEITERT: LoadQuestionsForTeams() Methode f�r difficulty-basierte Content-Auswahl
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (ERWEITERT F�R SCHWIERIGKEITSGRADE)

**FossilGameManager.cs**
- ERWEITERT: L�dt Fossilien basierend auf Team-Schwierigkeitsgrad
- ERWEITERT: Angepasste Rundendauer per GetAdjustedRoundDuration()
- ERWEITERT: Schwierigkeitsgrad-Anzeige im Explanation Screen
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
- ERWEITERT: Separate Arrays f�r Kids/BigKids/Adults Fossilien
- ERWEITERT: GetFossilsForDifficulty() und GetRandomFossils(count, difficulty) Methoden
- ERWEITERT: DifficultyTimeSettings Integration
- ERWEITERT: Legacy-Support f�r bestehenden Code
- Datenstruktur f�r Fossil-Begriffe und Bilder
- Team-Images f�r visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System f�r zuf�llige Fossil-Auswahl

### SHARED SYSTEMS (MOBILE-OPTIMIERT)

**GameDataManager.cs**
- ERWEITERT: TeamSettings Speicherung und Verwaltung
- ERWEITERT: SetTeamDifficulty() und GetTeamDifficulty() Methoden
- ERWEITERT: R�ckw�rtskompatibilit�t f�r bestehende Saves
- Plattformunabh�ngiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern f�r globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs, Team-Einstellungen
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- ERWEITERT: Separate Question-Arrays f�r Kids/BigKids/Adults
- ERWEITERT: GetQuestionsForDifficulty() Methode
- ERWEITERT: DifficultyTimeSettings Integration
- ERWEITERT: Validation-Methoden f�r alle Schwierigkeitsgrade
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

**SceneNavigator.cs (NEU)**
- Navigation zwischen Szenen (Settings, MainMenu)
- Audio-Integration f�r Navigations-Sounds
- Fallback-System f�r Scene-Namen/Indizes

### MOBILE-SPEZIFISCHE FEATURES (NEU IMPLEMENTIERT)

**Mobile UI Optimizations**
- Safe Area Support f�r iOS Notch/Android Cutouts
- Touch-Target-Gr��en (mindestens 60 Unity Units)
- Haptic Feedback f�r alle Interaktionen
- Platform-Detection (iOS/Android)
- Canvas Scaler Optimierungen f�r verschiedene Screen-Ratios

**Input Optimizations**
- Accelerometer-basierte Steuerung f�r Mobile
- Touch-Fallback-Systeme
- Vibration/Haptic Feedback Integration

## AKTUELLER PROJEKTSTATUS

### ? VOLLST�NDIG IMPLEMENTIERT UND FUNKTIONSF�HIG

#### Schwierigkeitsgrad-System (NEU)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl f�r Quiz und Fossilien
- Zeit-Multiplikatoren f�r verschiedene Schwierigkeitsgrade
- Mobile-optimierte UI mit Haptic Feedback

#### Split-Screen Quiz (ERWEITERT)
- Difficulty-basierte Fragen pro Team
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben

#### Fossilien-Stirnraten (ERWEITERT)
- Difficulty-basierte Fossil-Sets pro Team
- Adaptive Rundendauer basierend auf Schwierigkeitsgrad
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling f�r �bersprungene Begriffe
- Touch-Fallback f�r Testing
- Team-Image System statt Text-Labels

#### Shared Systems (MOBILE-OPTIMIERT)
- Plattformunabh�ngiges Save-System f�r Spielerfortschritt und Team-Settings
- Modular structure f�r verschiedene R�ume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? N�CHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### �bersetzungssystem (N�CHSTE PRIORIT�T)
- **Sprachen**: Deutsch (Standard), Englisch, Leichte Sprache Deutsch, Leichte Sprache Englisch
- **Erweiterbarkeit**: System soll leicht um weitere Sprachen erweiterbar sein
- **Fallback-System**: Automatische Fallbacks (Englisch ? Deutsch, Leichte Sprache ? Standard)
- **Implementierung**: Unity Localization Package vs. eigenes ScriptableObject-System evaluieren
- **Integration**: In bestehende UI-Systeme und Content-ScriptableObjects

#### Content-Integration
- Echte Museumsfragen und -fossilien integrieren
- Content-Validierung f�r alle Schwierigkeitsgrade
- Qualit�tssicherung f�r verschiedene Sprachniveaus

#### Weitere Entwicklung
- Integration in gr��eres F�hrungssystem
- Weitere Minispiel-Prototypen
- Cloud-Save Erweiterung (Firebase/PlayFab)
- Performance-Optimierung f�r mobile Ger�te
- UI-Polishing und Animation-System

### ?? PROJEKTSTRUKTUR (AKTUALISIERT)
Assets/_GAME/
??? Scripts/
?   ??? UI/
?   ?   ??? TouchInputHandler.cs
?   ?   ??? QuestionProgressIcons.cs
?   ?   ??? FossilInputHandler.cs
?   ?   ??? TeamSettingsManager.cs (NEU)
?   ?   ??? DifficultyRadioGroup.cs (NEU)
?   ?   ??? SceneNavigator.cs (NEU)
?   ?   ??? FullscreenManager.cs (DEPRECATED f�r Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED f�r Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT)
?   ?   ??? FossilGameManager.cs (ERWEITERT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT)
?   ?   ??? QuizQuestion.cs
?   ?   ??? FossilData.cs
?   ?   ??? FossilCollection.cs (ERWEITERT)
?   ?   ??? DifficultySystem.cs (NEU)
?   ??? Notes/
?       ??? Notes.md
??? Plugins/
    ??? FullscreenWebGL.jslib (DEPRECATED f�r Mobile)

### ?? DEVELOPMENT NOTES

#### Mobile-First Approach (NEU)
- **Plattform**: iOS/Android prim�r, WebGL-Features entfernt
- **UI**: Safe Area Support, Touch-Target-Optimierung
- **Input**: Accelerometer + Touch-Fallbacks, Haptic Feedback
- **Performance**: Mobile-optimierte Rendering und Memory-Management

#### Schwierigkeitsgrad-System (NEU)
- **Modularity**: Jeder Schwierigkeitsgrad hat eigene Content-Sets
- **Persistence**: Team-Settings bleiben �ber gesamte F�hrung erhalten
- **Flexibility**: Zeit-Multiplikatoren und Content-Anpassungen pro Level
- **Extensibility**: Einfache Erweiterung um weitere Schwierigkeitsgrade

#### Content Management
- **ScriptableObjects**: F�r einfache Content-Erstellung mit Difficulty-Support
- **Validation**: Automatische Pr�fung ob Content f�r alle Schwierigkeitsgrade vorhanden
- **Skalierbarkeit**: System designed f�r 6 verschiedene R�ume
- **Lokalisierung**: Vorbereitet f�r Multi-Language-Support

#### Technical Architecture
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabh�ngig mit Backup-Funktionalit�t und Team-Settings
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilit�t

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (NEU)
- **Team-Settings**: Persistent �ber GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty()
- **UI-System**: Image-Swapping Radio-Buttons f�r bessere Mobile-UX
- **Zeit-System**: Multiplikatoren f�r angepasste Rundendauer
- **Fallback**: Legacy-Support f�r bestehende Content-ScriptableObjects

### MOBILE-OPTIMIERUNG DETAILS (NEU)
- **Safe Area**: Automatische Anpassung an iOS Notch und Android Cutouts
- **Touch-Targets**: Mindestgr��e 60 Unity Units f�r optimale Bedienbarkeit
- **Haptic Feedback**: Vibration bei allen wichtigen Interaktionen
- **Platform-Detection**: Automatische Anpassung iOS vs Android Features

### SPLIT-SCREEN QUIZ DETAILS (ERWEITERT)
- **Multi-Difficulty**: Teams k�nnen unterschiedliche Fragen basierend auf Schwierigkeitsgrad erhalten
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers()
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System f�r saubere Button-States
- **Continue-System**: Touch-anywhere nach Feedback-Phase

### FOSSILIEN-STIRNRATEN DETAILS (ERWEITERT)
- **Adaptive Zeiten**: GetAdjustedRoundDuration() basierend auf Team-Schwierigkeitsgrad
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### SHARED SYSTEM DETAILS (ERWEITERT)
- **Save-System**: PlayerPrefs + JSON + Backup-Mechanismus + TeamSettings
- **Mobile-Support**: Platform-Detection und Mobile-spezifische Features
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilit�t

### AUDIO-SYSTEM
- **Split-Screen Quiz**: Continue-Sounds, Feedback-Audio
- **Fossilien-Stirnraten**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback
- **Settings-UI**: Button-Click-Sounds, Applied-Feedback-Sounds
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

**LETZTER STATUS**: Vollst�ndiges Schwierigkeitsgrad-System implementiert mit Mobile-Optimierungen. Beide Minispiele (Split-Screen Quiz + Fossilien-Stirnraten) unterst�tzen jetzt individuelle Team-Schwierigkeitsgrade mit persistenter Speicherung. Settings-UI mit Radio-Button-System und Image-Swapping f�r optimale Mobile-UX erstellt. N�chster Schritt: Multi-Language-System f�r Deutsch/Englisch und jeweils leichte Sprache.