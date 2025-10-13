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

**LETZTER STATUS**: Vollst�ndiges Schwierigkeitsgrad-System implementiert mit Mobile-Optimierungen. Beide Minispiele (Split-Screen Quiz + Fossilien-Stirnraten) unterst�tzen jetzt individuelle Team-Schwierigkeitsgrade mit persistenter Speicherung. Settings-UI mit Radio-Button-System und Image-Swapping f�r optimale Mobile-UX erstellt. N�chster Schritt: Multi-Language-System f�r Deutsch/Englisch und jeweils leichte Sprache.# PROJECT NOTES F�R GITHUB COPILOT
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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

### HAUPTKLASSEN - MEHRSPRACHIGKEITSSYSTEM (NEU IMPLEMENTIERT)

**LanguageSystem.cs**
- Enum Language (German_Standard, English_Standard, German_Simple, English_Simple)
- Singleton-Pattern f�r globalen Zugriff
- Fallback-System: Englisch ? Deutsch, Leichte Sprache ? Standard-Variante ? Deutsch
- Persistente Speicherung in PlayerPrefs
- Events f�r Sprachwechsel (OnLanguageChanged)
- Mobile-kompatible Implementierung

**LocalizedText.cs (ScriptableObject)**
- Container f�r lokalisierte Texte mit 4 Sprach-Varianten
- Automatisches Fallback-System bei fehlenden �bersetzungen
- CreateAssetMenu Integration f�r einfache Content-Erstellung
- Validation-Methoden f�r Editor-Workflow
- .NET Framework 4.7.1 kompatible Enum-Iteration

**LocalizedTextComponent.cs**
- UI-Komponente f�r automatische Text-Lokalisierung
- Auto-Detection von Text und TextMeshPro Komponenten
- Event-basierte Updates bei Sprachwechsel
- Runtime-Lokalisierung ohne Performance-Impact

**LanguageRadioGroup.cs**
- Radio-Button-System f�r Sprachauswahl (analog zu DifficultyRadioGroup)
- Image-Swapping f�r bessere Mobile-UX
- Mobile Touch-Target-Optimierung
- Haptic Feedback Integration
- Event-System f�r Auswahl-Changes

**LanguageSettingsManager.cs**
- Vollst�ndige UI f�r Spracheinstellungen
- Lokalisierte UI-Texte mit Live-Updates
- Mobile-optimierte Safe Area Unterst�tzung
- Integration mit GameDataManager f�r persistente Speicherung
- Apply/Back Button-System mit Audio-Feedback

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (IMPLEMENTIERT)

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

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (ERWEITERT F�R LOKALISIERUNG)

**SplitScreenQuizManager.cs**
- ERWEITERT: Vollst�ndige Lokalisierung aller UI-Texte
- ERWEITERT: Verwendet LocalizedText Assets f�r alle Feedback-Texte
- ERWEITERT: Language-Parameter f�r GetQuestionText() und GetShuffledAnswers()
- ERWEITERT: OnLanguageChanged Event-Handler f�r Live-Updates
- ERWEITERT: Fallback-Texte f�r alle Sprachen als Backup
- L�dt unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad
- LoadQuestionsForTeams() Methode f�r difficulty-basierte Content-Auswahl
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (ERWEITERT F�R LOKALISIERUNG)

**FossilGameManager.cs**
- ERWEITERT: Vollst�ndige Lokalisierung aller Explanation-Texte
- ERWEITERT: Verwendet LocalizedText Assets f�r alle UI-Texte
- ERWEITERT: GetLocalizedText() Helper-Methode mit Fallback-System
- ERWEITERT: OnLanguageChanged Event-Handler f�r Live-Updates
- ERWEITERT: Fallback-Texte f�r alle Sprachen und Schwierigkeitsgrade
- ERWEITERT: UpdateGameplayUI() und UpdateResultsUI() f�r Language-Updates
- L�dt Fossilien basierend auf Team-Schwierigkeitsgrad
- Angepasste Rundendauer per GetAdjustedRoundDuration()
- Schwierigkeitsgrad-Anzeige im Explanation Screen
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
- ERWEITERT: Lokalisierung f�r Fossil-Namen und Beschreibungen
- ERWEITERT: GetFossilName(Language) und GetDescription(Language) Methoden
- ERWEITERT: HasLocalization() und HasAllLocalizations() Validation
- ERWEITERT: GetCollectionName(Language) f�r lokalisierte Collection-Namen
- ERWEITERT: Legacy-Support f�r bestehende nicht-lokalisierte Inhalte
- Separate Arrays f�r Kids/BigKids/Adults Fossilien
- GetFossilsForDifficulty() und GetRandomFossils(count, difficulty) Methoden
- DifficultyTimeSettings Integration
- Datenstruktur f�r Fossil-Begriffe und Bilder
- Team-Images f�r visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System f�r zuf�llige Fossil-Auswahl

### SHARED SYSTEMS (MOBILE-OPTIMIERT + LOKALISIERT)

**GameDataManager.cs**
- ERWEITERT: Language-Speicherung und Synchronisation mit LanguageSystem
- ERWEITERT: SetLanguage() und CurrentLanguage Property
- ERWEITERT: SyncLanguageWithSystem() f�r Startup-Synchronisation
- ERWEITERT: Language wird bei ResetProgress() beibehalten
- TeamSettings Speicherung und Verwaltung
- SetTeamDifficulty() und GetTeamDifficulty() Methoden
- R�ckw�rtskompatibilit�t f�r bestehende Saves
- Plattformunabh�ngiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern f�r globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs, Team-Einstellungen, Sprache
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- ERWEITERT: Lokalisierung f�r Raum-Namen mit GetRoomName(Language)
- ERWEITERT: HasAllLocalizations() f�r Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu f�r Editor
- ERWEITERT: Legacy-Support f�r bestehende nicht-lokalisierte R�ume
- Separate Question-Arrays f�r Kids/BigKids/Adults
- GetQuestionsForDifficulty() Methode
- DifficultyTimeSettings Integration
- Validation-Methoden f�r alle Schwierigkeitsgrade
- Raumspezifische Fragen und Metadaten
- Modularer Aufbau f�r verschiedene Museumsr�ume
- Einfache Content-Erweiterung

**QuizQuestion.cs**
- ERWEITERT: Vollst�ndige Lokalisierung f�r Fragen und Antworten
- ERWEITERT: GetQuestionText(Language) und GetAnswer(index, Language) Methoden
- ERWEITERT: GetShuffledAnswers(Language) und GetCorrectAnswerIndex(Language)
- ERWEITERT: HasAllLocalizations() f�r Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu f�r Editor
- ERWEITERT: Legacy-Support f�r bestehende nicht-lokalisierte Fragen
- Fragendatenstruktur mit 4 Antworten pro Sprache
- Automatisches Answer-Shuffling pro Sprache
- Korrekte Antwort Index-Tracking

**PlayerData.cs**
- Spielerstate (Name, Score, hasAnswered, etc.)
- Antwort-Tracking und Score-Management

**SceneNavigator.cs**
- Navigation zwischen Szenen (Settings, MainMenu)
- Audio-Integration f�r Navigations-Sounds
- Fallback-System f�r Scene-Namen/Indizes

### MOBILE-SPEZIFISCHE FEATURES (IMPLEMENTIERT)

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

#### Mehrsprachigkeitssystem (NEU IMPLEMENTIERT)
- **Vier Sprachen**: Deutsch Standard, Englisch Standard, Deutsch Einfach, Englisch Einfach
- **Custom ScriptableObject System**: Optimiert f�r Unity-Workflow
- **Fallback-Hierarchie**: Automatische Fallbacks bei fehlenden �bersetzungen
- **Game-Manager Integration**: Beide Hauptspiele vollst�ndig lokalisiert
- **Live Language-Switching**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimierte Settings-UI**: Radio-Button-System mit Haptic Feedback
- **Performance-optimiert**: Sprach-Caching und Event-basierte Updates
- **Erweiterbar**: Einfache Integration weiterer Sprachen m�glich
- **Legacy-Support**: Bestehende Inhalte funktionieren weiterhin
- **Content-Validation**: Editor-Tools f�r �bersetzungs-Vollst�ndigkeit

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl f�r Quiz und Fossilien
- Zeit-Multiplikatoren f�r verschiedene Schwierigkeitsgrade
- Mobile-optimierte UI mit Haptic Feedback

#### Split-Screen Quiz (ERWEITERT + LOKALISIERT)
- **Vollst�ndige Lokalisierung**: Alle UI-Texte, Feedback-Nachrichten, Ergebnisse
- **Live Language-Updates**: Automatische Aktualisierung bei Sprachwechsel
- **Lokalisierte Fragen**: Teams erhalten Fragen in gew�hlter Sprache
- Difficulty-basierte Fragen pro Team
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben

#### Fossilien-Stirnraten (ERWEITERT + LOKALISIERT)
- **Vollst�ndige Lokalisierung**: Explanation-Screen, alle UI-Texte, Ergebnisse
- **Lokalisierte Fossilien**: Fossil-Namen in gew�hlter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gew�hlte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- Difficulty-basierte Fossil-Sets pro Team
- Adaptive Rundendauer basierend auf Schwierigkeitsgrad
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling f�r �bersprungene Begriffe
- Touch-Fallback f�r Testing
- Team-Image System statt Text-Labels

#### Shared Systems (MOBILE-OPTIMIERT + LOKALISIERT)
- **Sprach-persistente Speicherung**: Gew�hlte Sprache bleibt gespeichert
- **Cross-System-Synchronisation**: GameDataManager ? LanguageSystem
- Plattformunabh�ngiges Save-System f�r Spielerfortschritt, Team-Settings und Sprache
- Modular structure f�r verschiedene R�ume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? N�CHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Content-Erstellung (N�CHSTE PRIORIT�T)
- **LocalizedText Assets erstellen**: F�r alle Standard-UI-Texte
- **Beispiel-Content**: Quiz-Fragen und Fossilien in allen 4 Sprachen
- **Content-Validation**: Pr�fung der �bersetzungsqualit�t
- **Editor-Workflow**: Optimierung f�r Content-Creators

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
?   ?   ??? TeamSettingsManager.cs
?   ?   ??? DifficultyRadioGroup.cs
?   ?   ??? SceneNavigator.cs
?   ?   ??? LocalizedTextComponent.cs (NEU)
?   ?   ??? LanguageRadioGroup.cs (NEU)
?   ?   ??? LanguageSettingsManager.cs (NEU)
?   ?   ??? FullscreenManager.cs (DEPRECATED f�r Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED f�r Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilGameManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizQuestion.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilCollection.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? DifficultySystem.cs
?   ?   ??? LanguageSystem.cs (NEU)
?   ?   ??? LocalizedText.cs (NEU)
?   ??? Notes/
?       ??? Notes.md
??? Plugins/
    ??? FullscreenWebGL.jslib (DEPRECATED f�r Mobile)

### ??? DEVELOPMENT NOTES

#### Mehrsprachigkeitssystem (NEU)
- **Architektur**: Custom ScriptableObject-System statt Unity Localization Package
- **Performance**: Sprach-Caching und Event-basierte Updates minimieren Overhead
- **Fallback-Hierarchie**: 
  1. Leichte Sprache ? Standard-Variante
  2. Englisch ? Deutsch
  3. Alle ? Deutsch Standard
- **Mobile-First**: Touch-optimierte Settings-UI mit Haptic Feedback
- **Extensibility**: Neue Sprachen durch Language-Enum-Erweiterung
- **.NET Framework 4.7.1 Kompatibilit�t**: System.Enum.GetValues(typeof()) statt generischer Variante

#### Mobile-First Approach (IMPLEMENTIERT)
- **Plattform**: iOS/Android prim�r, WebGL-Features entfernt
- **UI**: Safe Area Support, Touch-Target-Optimierung
- **Input**: Accelerometer + Touch-Fallbacks, Haptic Feedback
- **Performance**: Mobile-optimierte Rendering und Memory-Management

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- **Modularity**: Jeder Schwierigkeitsgrad hat eigene Content-Sets
- **Persistence**: Team-Settings bleiben �ber gesamte F�hrung erhalten
- **Flexibility**: Zeit-Multiplikatoren und Content-Anpassungen pro Level
- **Extensibility**: Einfache Erweiterung um weitere Schwierigkeitsgrade

#### Content Management (ERWEITERT)
- **ScriptableObjects**: F�r einfache Content-Erstellung mit Difficulty + Language-Support
- **Validation**: Automatische Pr�fung ob Content f�r alle Schwierigkeitsgrade und Sprachen vorhanden
- **Skalierbarkeit**: System designed f�r 6 verschiedene R�ume
- **Lokalisierung**: Vollst�ndig implementiert mit Fallback-System
- **Legacy-Support**: Bestehende Inhalte ohne Lokalisierung funktionieren weiterhin

#### Technical Architecture (ERWEITERT)
- **Localization System**: Event-basiert mit automatischer UI-Aktualisierung
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabh�ngig mit Backup-Funktionalit�t, Team-Settings und Sprache
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilit�t

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### MEHRSPRACHIGKEITSSYSTEM DETAILS (NEU IMPLEMENTIERT)

#### Fallback-Hierarchie
1. **Erste Stufe**: Leichte Sprache ? Standard-Variante (German_Simple ? German_Standard)
2. **Zweite Stufe**: Ziel-Sprache ? Fallback-Sprache (English ? German)
3. **Dritte Stufe**: Alle ? German_Standard (final fallback)

#### Performance-Optimierungen
- **Language Caching**: Aktuelle Sprache wird gecacht f�r bessere Performance
- **Event-basierte Updates**: OnLanguageChanged Event minimiert unn�tige Updates
- **Lazy Loading**: Texte werden nur bei Bedarf geladen

#### Content-Validation
- **HasAllLocalizations()**: Pr�ft Vollst�ndigkeit aller �bersetzungen
- **Context-Menu Validation**: Editor-Tools f�r Content-Creators
- **Debug-Logging**: Optional aktivierbare Fallback-Logs

#### Legacy-Support
- **R�ckw�rtskompatibilit�t**: Bestehende Inhalte funktionieren ohne �nderungen
- **Obsolete-Markierungen**: Sanfte Migration zu neuen lokalisierten Methoden
- **Fallback-Texte**: Hardcodierte Fallbacks f�r alle Sprachen als Backup

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (IMPLEMENTIERT)
- **Team-Settings**: Persistent �ber GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty()
- **UI-System**: Image-Swapping Radio-Buttons f�r bessere Mobile-UX
- **Zeit-System**: Multiplikatoren f�r angepasste Rundendauer
- **Fallback**: Legacy-Support f�r bestehende Content-ScriptableObjects

### MOBILE-OPTIMIERUNG DETAILS (IMPLEMENTIERT)
- **Safe Area**: Automatische Anpassung an iOS Notch und Android Cutouts
- **Touch-Targets**: Mindestgr��e 60 Unity Units f�r optimale Bedienbarkeit
- **Haptic Feedback**: Vibration bei allen wichtigen Interaktionen
- **Platform-Detection**: Automatische Anpassung iOS vs Android Features

### SPLIT-SCREEN QUIZ DETAILS (ERWEITERT + LOKALISIERT)
- **Multi-Language**: Teams erhalten Fragen in gew�hlter Sprache
- **Live Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Lokalisierte Feedback**: Alle Punkte-Nachrichten und Gewinner-Texte
- **Multi-Difficulty**: Teams k�nnen unterschiedliche Fragen basierend auf Schwierigkeitsgrad erhalten
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers(Language)
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System f�r saubere Button-States
- **Continue-System**: Touch-anywhere nach Feedback-Phase

### FOSSILIEN-STIRNRATEN DETAILS (ERWEITERT + LOKALISIERT)
- **Multi-Language Explanation**: Vollst�ndig lokalisierter Explanation-Screen
- **Lokalisierte Fossilien**: Fossil-Namen in gew�hlter Sprache
- **Adaptive Difficulty-Display**: Schwierigkeitsgrade in passender Sprache angezeigt
- **Live Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Adaptive Zeiten**: GetAdjustedRoundDuration() basierend auf Team-Schwierigkeitsgrad
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### SHARED SYSTEM DETAILS (ERWEITERT + LOKALISIERT)
- **Language-Persistence**: Gew�hlte Sprache wird dauerhaft gespeichert
- **Cross-System-Sync**: GameDataManager synchronisiert automatisch mit LanguageSystem
- **Save-System**: PlayerPrefs + JSON + Backup-Mechanismus + TeamSettings + Language
- **Mobile-Support**: Platform-Detection und Mobile-spezifische Features
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilit�t

### AUDIO-SYSTEM
- **Split-Screen Quiz**: Continue-Sounds, Feedback-Audio
- **Fossilien-Stirnraten**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback
- **Settings-UI**: Button-Click-Sounds, Applied-Feedback-Sounds, Language-Change-Sounds
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

**AKTUELLER STATUS**: Vollst�ndiges Mehrsprachigkeitssystem implementiert und funktionsf�hig. Beide Hauptspiele (Split-Screen Quiz + Fossilien-Stirnraten) sind vollst�ndig lokalisiert mit Live Language-Switching. Custom ScriptableObject-System mit Fallback-Hierarchie optimiert f�r Unity-Workflow. GameDataManager erweitert um Sprach-Persistierung. Alle UI-Komponenten mobile-optimiert mit Haptic Feedback. System bereit f�r Content-Erstellung mit LocalizedText Assets. .NET Framework 4.7.1 Kompatibilit�t sichergestellt.

**N�CHSTER SCHRITT**: LocalizedText Assets f�r Standard-UI-Texte erstellen und Beispiel-Content in allen 4 Sprachen entwickeln (Deutsch Standard, Englisch Standard, Deutsch Einfach, Englisch Einfach).
