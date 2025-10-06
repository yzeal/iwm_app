# PROJECT NOTES FÜR GITHUB COPILOT
## WICHTIGE HINWEISE FÜR ZUKÜNFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext für GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu können ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: In dieser Datei KEINE dreifachen Anführungszeichen oder Backticks verwenden - diese beenden den Code-Output im Chat und verhindern direktes Kopieren in Dokumente.

---

# MUSEUM QUIZ APP - PROJEKTÜBERSICHT

## PROJEKTBESCHREIBUNG
- **Ziel**: Mobile Quiz-App für Museum mit Split-Screen Multiplayer
- **Plattform**: Unity 6.2, Mobile iOS/Android (Portrait-Modus) - UMSTELLUNG AUF MOBILE ERFOLGT
- **Zielgruppe**: Museumsbesucher, 2 Teams/Spieler pro Handy mit konfigurierbaren Schwierigkeitsgraden
- **Kontext**: Teil einer virtuellen Führung durch 6 Museumsräume

## TECHNISCHE DETAILS
- **Unity Version**: 6.2
- **Input System**: Neues Unity Input System
- **UI System**: Neues Unity UI System für verschiedene Screen-Ratios + Mobile-Optimierungen
- **C# Version**: 9.0
- **Target Framework**: .NET Framework 4.7.1
- **Platform**: iOS/Android (WebGL-Features entfernt)

## GAME DESIGN - SPLIT-SCREEN QUIZ

### KERNMECHANIK
- **Layout**: Portrait-Modus, vertikal gespiegelt
- **Spieler**: 2 Teams an einem Gerät mit individuellen Schwierigkeitsgraden
- **UI-Aufteilung**: Jeder Spieler hat eine Bildschirmhälfte, andere Hälfte steht kopf
- **Anti-Schummel**: Unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad + zufällig angeordnete Antworten

### SCHWIERIGKEITSGRADE (NEU IMPLEMENTIERT)
- **Kids**: Einfachere Sprache, weniger komplexe Fragen
- **BigKids**: Mittlere Komplexität
- **Adults**: Vollständige Komplexität
- **Individuelle Auswahl**: Jedes Team kann unabhängig seinen Schwierigkeitsgrad wählen
- **Persistente Speicherung**: Einstellungen bleiben für gesamte Führung gespeichert

### PUNKTESYSTEM
- **Richtige Antwort (Erster)**: 2 Punkte
- **Richtige Antwort (Zweiter)**: 1 Punkt  
- **Falsche Antwort**: 0 Punkte
- **Zeitlimit**: Keines (kann bei Bedarf pro Schwierigkeitsgrad angepasst werden)
- **Fortschritt**: Touch nach beiden Antworten ? nächste Frage

### SPIELABLAUF
1. Teams erhalten Fragen basierend auf ihren Schwierigkeitsgraden
2. 4 Antwortmöglichkeiten (1 richtig, 3 falsch) in unterschiedlicher Reihenfolge
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
- **Kids**: Einfachere Begriffe, länger Zeit (1.5x Multiplikator)
- **BigKids**: Mittlere Komplexität, etwas länger Zeit (1.2x Multiplikator)
- **Adults**: Vollständige Komplexität, normale Zeit (1.0x Multiplikator)
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad

### STEUERUNG
- **Accelerometer**: Handy nach vorne neigen = Richtig erraten
- **Accelerometer**: Handy nach hinten neigen = Überspringen
- **Touch-Fallback**: Links tippen = Überspringen, Rechts tippen = Richtig
- **Platform-Detection**: Automatischer Wechsel zwischen Accelerometer und Touch
- **Haptic Feedback**: Mobile Vibration für bessere UX

### SPIELABLAUF
1. Explanation Screen mit Regeln, Team-Bild und Schwierigkeitsgrad-Anzeige
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Ratephase mit angepasster Timer-Dauer basierend auf Schwierigkeitsgrad
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung

### FEATURES
- **Fossil-Recycling**: Übersprungene Fossilien kommen später wieder
- **Timer-Warnung**: Audio-Countdown bei 3-2-1 Sekunden verbleibend
- **Team-Images**: Visuelle Team-Darstellung statt Text
- **Score-Tracking**: Einfache X/Y Anzeige während Spiel
- **Adaptive Zeiten**: Schwierigkeitsgrad-basierte Rundendauer

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (NEU IMPLEMENTIERT)

**DifficultySystem.cs**
- Enum DifficultyLevel (Kids, BigKids, Adults)
- TeamSettings Klasse für persistente Team-Konfiguration
- DifficultyTimeSettings für schwierigkeitsgrad-basierte Zeitmultiplikatoren
- Integration in GameDataManager für persistente Speicherung

**TeamSettingsManager.cs**
- Zentrale UI-Logik für Team-Schwierigkeitsgrad-Auswahl
- Mobile-optimierte UI mit Safe Area Support
- Haptic Feedback für iOS/Android
- Icon-basierte Team-Darstellung
- Apply/Back Button-System (Reset entfernt)

**DifficultyRadioGroup.cs**
- Radio-Button-System mit Image-Swapping statt Farbwechsel
- Checkbox-ähnliche Darstellung für bessere Mobile-UX
- Individual Sprite-Support pro Button
- Mobile Touch-Target-Optimierung
- Haptic Feedback Integration

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (ERWEITERT FÜR SCHWIERIGKEITSGRADE)

**SplitScreenQuizManager.cs**
- ERWEITERT: Lädt unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad
- ERWEITERT: LoadQuestionsForTeams() Methode für difficulty-basierte Content-Auswahl
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (ERWEITERT FÜR SCHWIERIGKEITSGRADE)

**FossilGameManager.cs**
- ERWEITERT: Lädt Fossilien basierend auf Team-Schwierigkeitsgrad
- ERWEITERT: Angepasste Rundendauer per GetAdjustedRoundDuration()
- ERWEITERT: Schwierigkeitsgrad-Anzeige im Explanation Screen
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
- ERWEITERT: Separate Arrays für Kids/BigKids/Adults Fossilien
- ERWEITERT: GetFossilsForDifficulty() und GetRandomFossils(count, difficulty) Methoden
- ERWEITERT: DifficultyTimeSettings Integration
- ERWEITERT: Legacy-Support für bestehenden Code
- Datenstruktur für Fossil-Begriffe und Bilder
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System für zufällige Fossil-Auswahl

### SHARED SYSTEMS (MOBILE-OPTIMIERT)

**GameDataManager.cs**
- ERWEITERT: TeamSettings Speicherung und Verwaltung
- ERWEITERT: SetTeamDifficulty() und GetTeamDifficulty() Methoden
- ERWEITERT: Rückwärtskompatibilität für bestehende Saves
- Plattformunabhängiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern für globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs, Team-Einstellungen
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- ERWEITERT: Separate Question-Arrays für Kids/BigKids/Adults
- ERWEITERT: GetQuestionsForDifficulty() Methode
- ERWEITERT: DifficultyTimeSettings Integration
- ERWEITERT: Validation-Methoden für alle Schwierigkeitsgrade
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

**SceneNavigator.cs (NEU)**
- Navigation zwischen Szenen (Settings, MainMenu)
- Audio-Integration für Navigations-Sounds
- Fallback-System für Scene-Namen/Indizes

### MOBILE-SPEZIFISCHE FEATURES (NEU IMPLEMENTIERT)

**Mobile UI Optimizations**
- Safe Area Support für iOS Notch/Android Cutouts
- Touch-Target-Größen (mindestens 60 Unity Units)
- Haptic Feedback für alle Interaktionen
- Platform-Detection (iOS/Android)
- Canvas Scaler Optimierungen für verschiedene Screen-Ratios

**Input Optimizations**
- Accelerometer-basierte Steuerung für Mobile
- Touch-Fallback-Systeme
- Vibration/Haptic Feedback Integration

## AKTUELLER PROJEKTSTATUS

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Schwierigkeitsgrad-System (NEU)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl für Quiz und Fossilien
- Zeit-Multiplikatoren für verschiedene Schwierigkeitsgrade
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
- Fossil-Recycling für übersprungene Begriffe
- Touch-Fallback für Testing
- Team-Image System statt Text-Labels

#### Shared Systems (MOBILE-OPTIMIERT)
- Plattformunabhängiges Save-System für Spielerfortschritt und Team-Settings
- Modular structure für verschiedene Räume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Übersetzungssystem (NÄCHSTE PRIORITÄT)
- **Sprachen**: Deutsch (Standard), Englisch, Leichte Sprache Deutsch, Leichte Sprache Englisch
- **Erweiterbarkeit**: System soll leicht um weitere Sprachen erweiterbar sein
- **Fallback-System**: Automatische Fallbacks (Englisch ? Deutsch, Leichte Sprache ? Standard)
- **Implementierung**: Unity Localization Package vs. eigenes ScriptableObject-System evaluieren
- **Integration**: In bestehende UI-Systeme und Content-ScriptableObjects

#### Content-Integration
- Echte Museumsfragen und -fossilien integrieren
- Content-Validierung für alle Schwierigkeitsgrade
- Qualitätssicherung für verschiedene Sprachniveaus

#### Weitere Entwicklung
- Integration in größeres Führungssystem
- Weitere Minispiel-Prototypen
- Cloud-Save Erweiterung (Firebase/PlayFab)
- Performance-Optimierung für mobile Geräte
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
?   ?   ??? FullscreenManager.cs (DEPRECATED für Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED für Mobile)
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
    ??? FullscreenWebGL.jslib (DEPRECATED für Mobile)

### ?? DEVELOPMENT NOTES

#### Mobile-First Approach (NEU)
- **Plattform**: iOS/Android primär, WebGL-Features entfernt
- **UI**: Safe Area Support, Touch-Target-Optimierung
- **Input**: Accelerometer + Touch-Fallbacks, Haptic Feedback
- **Performance**: Mobile-optimierte Rendering und Memory-Management

#### Schwierigkeitsgrad-System (NEU)
- **Modularity**: Jeder Schwierigkeitsgrad hat eigene Content-Sets
- **Persistence**: Team-Settings bleiben über gesamte Führung erhalten
- **Flexibility**: Zeit-Multiplikatoren und Content-Anpassungen pro Level
- **Extensibility**: Einfache Erweiterung um weitere Schwierigkeitsgrade

#### Content Management
- **ScriptableObjects**: Für einfache Content-Erstellung mit Difficulty-Support
- **Validation**: Automatische Prüfung ob Content für alle Schwierigkeitsgrade vorhanden
- **Skalierbarkeit**: System designed für 6 verschiedene Räume
- **Lokalisierung**: Vorbereitet für Multi-Language-Support

#### Technical Architecture
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabhängig mit Backup-Funktionalität und Team-Settings
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilität

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (NEU)
- **Team-Settings**: Persistent über GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty()
- **UI-System**: Image-Swapping Radio-Buttons für bessere Mobile-UX
- **Zeit-System**: Multiplikatoren für angepasste Rundendauer
- **Fallback**: Legacy-Support für bestehende Content-ScriptableObjects

### MOBILE-OPTIMIERUNG DETAILS (NEU)
- **Safe Area**: Automatische Anpassung an iOS Notch und Android Cutouts
- **Touch-Targets**: Mindestgröße 60 Unity Units für optimale Bedienbarkeit
- **Haptic Feedback**: Vibration bei allen wichtigen Interaktionen
- **Platform-Detection**: Automatische Anpassung iOS vs Android Features

### SPLIT-SCREEN QUIZ DETAILS (ERWEITERT)
- **Multi-Difficulty**: Teams können unterschiedliche Fragen basierend auf Schwierigkeitsgrad erhalten
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers()
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System für saubere Button-States
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
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilität

### AUDIO-SYSTEM
- **Split-Screen Quiz**: Continue-Sounds, Feedback-Audio
- **Fossilien-Stirnraten**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback
- **Settings-UI**: Button-Click-Sounds, Applied-Feedback-Sounds
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

**LETZTER STATUS**: Vollständiges Schwierigkeitsgrad-System implementiert mit Mobile-Optimierungen. Beide Minispiele (Split-Screen Quiz + Fossilien-Stirnraten) unterstützen jetzt individuelle Team-Schwierigkeitsgrade mit persistenter Speicherung. Settings-UI mit Radio-Button-System und Image-Swapping für optimale Mobile-UX erstellt. Nächster Schritt: Multi-Language-System für Deutsch/Englisch und jeweils leichte Sprache.