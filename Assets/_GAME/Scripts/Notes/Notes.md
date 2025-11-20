# PROJECT NOTES FÜR GITHUB COPILOT
## WICHTIGE HINWEISE FÜR ZUKÜNFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext für GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu können ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: 
- **IN DIESER DATEI**: KEINE dreifachen Anführungszeichen oder Backticks verwenden - diese würden den Markdown-Parser stören
- **FÜR CHAT-OUTPUT**: Code-Snippets MIT Backticks umschließen, damit sie im Chat als Code-Block angezeigt werden
- **BEIM EINFÜGEN**: Die äußeren Backticks werden automatisch entfernt - nur der innere Inhalt wird in die Datei geschrieben

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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

## GAME DESIGN - TABU-MINISPIEL (VOLLSTÄNDIG IMPLEMENTIERT + CONTENT ERSTELLT)

### KERNMECHANIK
- **Gameplay**: Tabu-Style Erklärsystem - Begriffe erklären ohne Tabu-Wörter zu verwenden
- **Teams**: 2 Teams spielen abwechselnd (Team 1, dann Team 2) mit individuellen Schwierigkeitsgraden
- **Rundendauer**: Konfigurierbar mit Zeit-Multiplikatoren pro Schwierigkeitsgrad (Standard: 60 Sekunden)
- **Begriffe pro Runde**: Konfigurierbar (Standard: 5)
- **Layout**: Portrait-Vollbild (KEIN Split-Screen), Teams spielen nacheinander

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
- **Kids**: Einfachere Begriffe, weniger/einfachere Tabu-Wörter, länger Zeit (1.5x Multiplikator)
- **BigKids**: Mittlere Komplexität, moderate Tabu-Wörter, etwas länger Zeit (1.2x Multiplikator)
- **Adults**: Vollständige Komplexität, viele Tabu-Wörter, normale Zeit (1.0x Multiplikator)
- **Content-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad

### STEUERUNG
- **Button-basiert**: "Richtig" (rechts) und "Überspringen" (links) Buttons
- **Haptic Feedback**: Mobile Vibration bei Button-Klicks
- **Touch-Targets**: Mobile-optimierte Button-Größen

### SPIELABLAUF
1. Explanation Screen mit Regeln, Team-Bild und Schwierigkeitsgrad-Anzeige
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Erklärungs-Phase:
   - Hauptbegriff oben (groß)
   - Tabu-Wörter darunter (dynamische Anzahl)
   - Timer-Countdown oben
   - Score-Anzeige (X/Y Format)
   - Richtig/Überspringen Buttons unten
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung und Gewinner-Icon

### FEATURES
- **Begriff-Recycling**: Übersprungene Begriffe kommen ans Ende der Queue zurück
- **Timer-Warnung**: Audio-Countdown bei letzten 3 Sekunden
- **Team-Images**: Visuelle Team-Darstellung auf allen Screens
- **Gewinner-Icon**: Gewinner-Team-Image wird auf Results Screen angezeigt
- **Dynamische Tabu-Wörter**: Variable Anzahl pro Begriff (im ScriptableObject definiert)
- **Adaptive Zeiten**: Schwierigkeitsgrad-basierte Rundendauer
- **Team-Exklusion**: Team 2 bekommt (wenn möglich) andere Begriffe als Team 1

### PUNKTESYSTEM
- **Richtig erraten**: +1 Punkt
- **Überspringen**: 0 Punkte (Begriff kommt zurück in Queue)
- **Zeit abgelaufen**: Runde endet
- **Alle Begriffe durch**: Runde endet

### TABU-MINISPIEL - TAP-TO-START TIMER-SYSTEM (NEU - 07.11.2025)

**TabuGameManager.cs - Timer-Steuerungs-Erweiterung**
- **Tap-to-Start Mechanik**: Timer startet erst nach Touch auf den Begriff
- **Begriff-Toggle**: Begriff kann während Timer-Lauf ein/ausgeblendet werden
- **Correct-Button-Delay**: Correct-Button wird erst nach Timer-Start sichtbar
- **Start-Indikator**: Optionales GameObject ("Los!"-Icon) für visuelles Feedback

#### Neue Features:
1. **mainTermButton** - Button für Begriff-Interaktion
   - Erstes Tap: Begriff ausblenden + Timer starten + "Los!"-Icon verstecken
   - Weiteres Tap: Begriff togglen OHNE Timer zu pausieren
   - Haptic Feedback bei jedem Tap

2. **startIndicatorIcon** - GameObject für "Tap to Start" Visualisierung
   - Sichtbar wenn Timer pausiert ist (neuer Begriff)
   - Ausgeblendet wenn Timer läuft
   - Automatische Show/Hide-Logik über UpdateStartIndicatorVisibility()

3. **Neue Game-State-Variablen**:
   - timerPaused (bool) - Steuert Timer-Pausierung
   - termVisible (bool) - Trackt Begriff-Sichtbarkeit

4. **Neue Methoden**:
   - OnTermTapped() - Handler für Begriff-Button-Clicks
   - UpdateStartIndicatorVisibility() - Steuert "Los!"-Icon
   - UpdateButtonVisibility() - Steuert Correct/Skip-Button-Sichtbarkeit

#### Spielablauf-Änderungen:
- **Alt**: Timer läuft sofort nach Countdown ? Begriff sofort sichtbar
- **Neu**: Timer pausiert ? Spieler 1 tippt auf Begriff ? Begriff ausgeblendet ? Timer startet

#### UI-Visibility-States:
- **Timer pausiert** (neuer Begriff):
  - Begriff: Sichtbar
  - "Los!"-Icon: Sichtbar
  - Correct-Button: Versteckt
  - Skip-Button: Sichtbar

- **Timer läuft**:
  - Begriff: Ausgeblendet (togglebar)
  - "Los!"-Icon: Versteckt
  - Correct-Button: Sichtbar
  - Skip-Button: Sichtbar

#### Integration Points:
- StartRound(): Timer startet mit timerPaused = true
- ShowCurrentTerm(): Timer pausieren + Begriff einblenden + Icon anzeigen
- Update(): Timer läuft nur wenn !timerPaused
- EndRound(): Timer pausieren

#### UX-Verbesserungen:
- **Anti-Peek**: Begriff wird vor Timer-Start ausgeblendet
- **Flexible Review**: Begriff kann jederzeit wieder angeschaut werden
- **Clear Indication**: "Los!"-Icon zeigt deutlich wann zu tippen ist
- **Delayed Correct-Button**: Verhindert versehentliche Klicks

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - MEHRSPRACHIGKEITSSYSTEM (IMPLEMENTIERT)

**LanguageSystem.cs**
- Enum Language (German_Standard, English_Standard, German_Simple, English_Simple)
- WICHTIG: Enum ist INNERHALB der LanguageSystem Klasse (Zugriff via LanguageSystem.Language)
- Singleton-Pattern für globalen Zugriff
- Fallback-System: Englisch ? Deutsch, Leichte Sprache ? Standard-Variante ? Deutsch
- Persistente Speicherung in PlayerPrefs
- Events für Sprachwechsel (OnLanguageChanged)
- Mobile-kompatible Implementierung
- FindFirstObjectByType statt FindObjectOfType (Unity 6.2 kompatibel)

**LocalizedText.cs (ScriptableObject)**
- Container für lokalisierte Texte mit 4 Sprach-Varianten
- Automatisches Fallback-System bei fehlenden Übersetzungen
- CreateAssetMenu Integration für einfache Content-Erstellung
- Validation-Methoden für Editor-Workflow
- .NET Framework 4.7.1 kompatible Enum-Iteration

**LocalizedTextComponent.cs**
- UI-Komponente für automatische Text-Lokalisierung
- Auto-Detection von Text und TextMeshPro Komponenten
- Event-basierte Updates bei Sprachwechsel
- Runtime-Lokalisierung ohne Performance-Impact

**LanguageRadioGroup.cs**
- Radio-Button-System für Sprachauswahl (analog zu DifficultyRadioGroup)
- Image-Swapping für bessere Mobile-UX
- Mobile Touch-Target-Optimierung
- Haptic Feedback Integration
- Event-System für Auswahl-Changes
- Debug-Methoden für Testing

**LanguageSettingsManager.cs**
- VEREINFACHT: Nur noch LanguageRadioGroup + Back Button
- ENTFERNT: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- Sofortige Sprachwechsel beim Radio-Button-Click
- Lokalisierte UI-Texte mit Live-Updates
- Mobile-optimierte Safe Area Unterstützung
- Integration mit GameDataManager für persistente Speicherung

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (IMPLEMENTIERT)

**DifficultySystem.cs**
- Enum DifficultyLevel (Kids, BigKids, Adults) - DIREKT im globalen Namespace (NICHT in Klasse!)
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

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (ERWEITERT FÜR LOKALISIERUNG)

**SplitScreenQuizManager.cs**
- ERWEITERT: Vollständige Lokalisierung aller UI-Texte
- ERWEITERT: Verwendet LocalizedText Assets für alle Feedback-Texte
- ERWEITERT: Language-Parameter für GetQuestionText() und GetShuffledAnswers()
- ERWEITERT: OnLanguageChanged Event-Handler für Live-Updates
- ERWEITERT: Fallback-Texte für alle Sprachen als Backup
- Lädt unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad
- LoadQuestionsForTeams() Methode für difficulty-basierte Content-Auswahl
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (VOLLSTÄNDIG LOKALISIERT)

**FossilGameManager.cs**
- ERWEITERT: Vollständige Lokalisierung ALLER UI-Texte (keine hardcodierten Strings mehr)
- ERWEITERT: Neue LocalizedText-Variablen für Anweisungen und Input-Modi
  - holdPhoneInstructionLocalizedText
  - teamExplainsInstructionLocalizedText
  - tiltModeLocalizedText
  - touchModeLocalizedText
- ERWEITERT: GetLocalizedInputModeInfo() für dynamische Input-Mode-Texte
- ERWEITERT: IsUsingAccelerometer() Integration für korrekte Input-Mode-Anzeige
- ERWEITERT: Fallback-Funktionen für alle neuen lokalisierten Texte
- ENTFERNT: Hardcodierte GetLocalizedHoldPhoneText() und GetLocalizedTeamExplainsText()
- Verwendet LocalizedText Assets für alle UI-Texte
- GetLocalizedText() Helper-Methode mit Fallback-System
- OnLanguageChanged Event-Handler für Live-Updates
- UpdateGameplayUI() und UpdateResultsUI() für Language-Updates
- Lädt Fossilien basierend auf Team-Schwierigkeitsgrad
- Angepasste Rundendauer per GetAdjustedRoundDuration()
- Schwierigkeitsgrad-Anzeige im Explanation Screen
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
- IsUsingAccelerometer() Methode für externe Abfragen

**FossilData.cs & FossilCollection.cs (ScriptableObject)**
- ERWEITERT: Lokalisierung für Fossil-Namen und Beschreibungen
- ERWEITERT: GetFossilName(Language) und GetDescription(Language) Methoden
- ERWEITERT: HasLocalization() und HasAllLocalizations() Validation
- ERWEITERT: GetCollectionName(Language) für lokalisierte Collection-Namen
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Inhalte
- Separate Arrays für Kids/BigKids/Adults Fossilien
- GetFossilsForDifficulty() und GetRandomFossils(count, difficulty) Methoden
- DifficultyTimeSettings Integration
- Datenstruktur für Fossil-Begriffe und Bilder
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System für zufällige Fossil-Auswahl

### HAUPTKLASSEN - TABU-MINISPIEL (VOLLSTÄNDIG IMPLEMENTIERT + LOKALISIERT + CONTENT ERSTELLT)

**TabuGameManager.cs**
- Vollständige Implementierung des Tabu-Minispiels
- Analog zu FossilGameManager strukturiert für Konsistenz
- Vollständig lokalisiert (12 LocalizedText Assets erstellt)
- Vier Screens: Explanation, Countdown, Gameplay, Results
- Button-basierte Steuerung (Richtig/Überspringen)
- Timer-System mit Audio-Countdown bei letzten 3 Sekunden
- Schwierigkeitsgrad-basierte Rundendauer
- Begriff-Recycling für übersprungene Begriffe (ans Ende der Queue)
- Team-Exklusions-System (Team 2 bekommt andere Begriffe wenn möglich)
- Dynamische Tabu-Wörter-Anzeige (UI-Pool-System)
- Gewinner-Icon auf Results Screen (wie bei FossilGameManager)
- OnLanguageChanged Event-Handler für Live-Updates
- GetLocalizedText() Helper-Methode mit Fallback-System
- Haptic Feedback bei Button-Klicks
- Integration mit GameDataManager für Team-Schwierigkeitsgrade

**TabuTerm.cs (ScriptableObject)**
- Datenstruktur für einzelne Tabu-Begriffe
- Vollständig lokalisiert (4 Sprachen)
- Hauptbegriff in allen 4 Sprachen
- Tabu-Wörter-Arrays in allen 4 Sprachen (dynamische Länge)
- Optional: Sprite für Begriff-Bild
- GetMainTerm(Language) Methode mit Fallback-Hierarchie
- GetTabuWords(Language) Methode mit Fallback-Hierarchie
- HasAllLocalizations() für Content-Validation
- ValidateLocalizations() Context-Menu für Editor
- CreateAssetMenu Integration

**TabuCollection.cs (ScriptableObject)**
- Container für Tabu-Begriffe mit Schwierigkeitsgrad-Sets
- Analog zu FossilCollection strukturiert
- Lokalisierter Collection-Name über LocalizedText
- Separate Term-Arrays für Kids/BigKids/Adults
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen (termsPerRound, roundDuration)
- DifficultyTimeSettings Integration
- GetTermsForDifficulty(DifficultyLevel) Methode
- GetRandomTerms(count, difficulty) Methode
- GetRandomTermsExcluding(count, difficulty, excludeList) für Team 2
- GetAdjustedRoundDuration(difficulty) für angepasste Zeiten
- HasTermsForAllDifficulties() Validation
- HasAllLocalizations() für Content-Validation
- Context-Menu Validierungs-Tools
- CreateAssetMenu Integration

### SHARED SYSTEMS (MOBILE-OPTIMIERT + LOKALISIERT)

**GameDataManager.cs**
- ERWEITERT: Language-Speicherung und Synchronisation mit LanguageSystem
- ERWEITERT: SetLanguage() und CurrentLanguage Property
- ERWEITERT: SyncLanguageWithSystem() für Startup-Synchronisation
- ERWEITERT: Language wird bei ResetProgress() beibehalten
- TeamSettings Speicherung und Verwaltung
- SetTeamDifficulty() und GetTeamDifficulty() Methoden
- Rückwärtskompatibilität für bestehende Saves
- Plattformunabhängiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern für globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs, Team-Einstellungen, Sprache
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- ERWEITERT: Lokalisierung für Raum-Namen mit GetRoomName(Language)
- ERWEITERT: HasAllLocalizations() für Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu für Editor
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Räume
- Separate Question-Arrays für Kids/BigKids/Adults
- GetQuestionsForDifficulty() Methode
- DifficultyTimeSettings Integration
- Validation-Methoden für alle Schwierigkeitsgrade
- Raumspezifische Fragen und Metadaten
- Modularer Aufbau für verschiedene Museumsräume
- Einfache Content-Erweiterung

**QuizQuestion.cs**
- ERWEITERT: Vollständige Lokalisierung für Fragen und Antworten
- ERWEITERT: GetQuestionText(Language) und GetAnswer(index, Language) Methoden
- ERWEITERT: GetShuffledAnswers(Language) und GetCorrectAnswerIndex(Language)
- ERWEITERT: HasAllLocalizations() für Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu für Editor
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Fragen
- Fragendatenstruktur mit 4 Antworten pro Sprache
- Automatisches Answer-Shuffling pro Sprache
- Korrekte Antwort Index-Tracking

**PlayerData.cs**
- Spielerstate (Name, Score, hasAnswered, etc.)
- Antwort-Tracking und Score-Management

**SceneNavigator.cs**
- Navigation zwischen Szenen (Settings, MainMenu)
- Audio-Integration für Navigations-Sounds
- Fallback-System für Scene-Namen/Indizes

### MOBILE-SPEZIFISCHE FEATURES (IMPLEMENTIERT)

**Mobile UI Optimizations**
- Safe Area Support für iOS Notch/Android Cutouts
- Touch-Target-Größen (mindestens 60 Unity Units)
- Haptic Feedback für alle Interaktionen
- Platform-Detection (iOS/Android)
- Canvas Scaler Optimierungen für verschiedene Screen-Ratios

**Input Optimizations**
- Accelerometer-basierte Steuerung für Mobile (Fossil-Stirnraten)
- Button-basierte Steuerung für Mobile (Tabu)
- Touch-Fallback-Systeme
- Vibration/Haptic Feedback Integration

## AKTUELLER PROJEKTSTATUS

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Mehrsprachigkeitssystem (IMPLEMENTIERT + POLISHED)
- **Vier Sprachen**: Deutsch Standard, Englisch Standard, Deutsch Einfach, Englisch Einfach
- **Custom ScriptableObject System**: Optimiert für Unity-Workflow
- **Fallback-Hierarchie**: Automatische Fallbacks bei fehlenden Übersetzungen
- **Game-Manager Integration**: Alle drei Hauptspiele vollständig lokalisiert (KEINE hardcodierten Strings mehr)
- **Live Language-Switching**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimierte Settings-UI**: Vereinfachtes Radio-Button-System mit sofortigem Sprachwechsel
- **Performance-optimiert**: Sprach-Caching und Event-basierte Updates
- **Erweiterbar**: Einfache Integration weiterer Sprachen möglich
- **Legacy-Support**: Bestehende Inhalte funktionieren weiterhin
- **Content-Validation**: Editor-Tools für Übersetzungs-Vollständigkeit

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl für Quiz, Fossilien und Tabu
- Zeit-Multiplikatoren für verschiedene Schwierigkeitsgrade
- Mobile-optimierte UI mit Haptic Feedback

#### Split-Screen Quiz (ERWEITERT + LOKALISIERT)
- **Vollständige Lokalisierung**: Alle UI-Texte, Feedback-Nachrichten, Ergebnisse
- **Live Language-Updates**: Automatische Aktualisierung bei Sprachwechsel
- **Lokalisierte Fragen**: Teams erhalten Fragen in gewählter Sprache
- Difficulty-basierte Fragen pro Team
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben

#### Fossilien-Stirnraten (VOLLSTÄNDIG LOKALISIERT)
- **100% Lokalisierung**: ALLE UI-Texte über LocalizedText Assets
- **Keine hardcodierten Strings mehr**: Explanation-Screen, Input-Modi, Anweisungen
- **Lokalisierte Fossilien**: Fossil-Namen in gewählter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gewählte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- **Dynamische Input-Mode-Texte**: Zeigt korrekte Steuerung basierend auf Platform
- Difficulty-basierte Fossil-Sets pro Team
- Adaptive Rundendauer basierend auf Schwierigkeitsgrad
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling für übersprungene Begriffe
- Touch-Fallback für Testing
- Team-Image System statt Text-Labels

#### Tabu-Minispiel (VOLLSTÄNDIG FERTIGGESTELLT)
- **100% Code-Implementierung**: TabuGameManager, TabuTerm, TabuCollection vollständig implementiert
- **100% Lokalisierung**: ALLE 12 LocalizedText Assets erstellt und zugewiesen
- **100% Content**: TabuTerm ScriptableObjects für alle Schwierigkeitsgrade erstellt
- **100% UI-Setup**: TabuGame Scene erstellt, UI-Hierarchie aufgebaut, Referenzen zugewiesen
- **Button-basierte Steuerung**: Richtig/Überspringen Buttons mit Haptic Feedback
- **Difficulty-basierte Begriff-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad
- **Adaptive Rundendauer**: Schwierigkeitsgrad-basierte Zeitmultiplikatoren
- **Team-Exklusion**: Team 2 bekommt andere Begriffe (wenn möglich)
- **Begriff-Recycling**: Übersprungene Begriffe kommen zurück in Queue
- **Timer-System**: Audio-Countdown bei letzten 3 Sekunden
- **Dynamische Tabu-Wörter**: Variable Anzahl pro Begriff mit UI-Pool-System
- **Gewinner-Icon**: Gewinner-Team-Image auf Results Screen
- **Live Language-Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimiert**: Touch-Targets, Haptic Feedback, Portrait-Vollbild
- **SPIELBAR**: Vollständig getestet und spielbereit

#### Shared Systems (MOBILE-OPTIMIERT + LOKALISIERT)
- **Sprach-persistente Speicherung**: Gewählte Sprache bleibt gespeichert
- **Cross-System-Synchronisation**: GameDataManager ? LanguageSystem
- Plattformunabhängiges Save-System für Spielerfortschritt, Team-Settings und Sprache
- Modular structure für verschiedene Räume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Neues Minispiel (HÖCHSTE PRIORITÄT)
- Warte auf Design-Beschreibung für nächstes Minispiel
- Analog zu bestehenden Systemen (Difficulty + Language Support)
- Mobile-optimierte Implementierung

#### Weitere Entwicklung
- Integration in größeres Führungssystem
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
?   ?   ??? TeamSettingsManager.cs
?   ?   ??? DifficultyRadioGroup.cs
?   ?   ??? SceneNavigator.cs
?   ?   ??? LocalizedTextComponent.cs
?   ?   ??? LanguageRadioGroup.cs
?   ?   ??? LanguageSettingsManager.cs
?   ?   ??? FullscreenManager.cs (DEPRECATED für Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED für Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilGameManager.cs (VOLLSTÄNDIG LOKALISIERT)
?   ?   ??? TabuGameManager.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizQuestion.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilCollection.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? TabuTerm.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? TabuCollection.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? DifficultySystem.cs
?   ?   ??? LanguageSystem.cs
?   ?   ??? LocalizedText.cs
?   ?   ??? Notes/
?       ??? Notes.md
?   ??? NFC/
?   ?   ??? NFCManager.cs (NEU - 29.10.2025)
?   ??? Game/
?   ?   ??? NFCSceneLoader.cs (NEU - 29.10.2025)
?   ??? Data/
?   ?   ??? NFCRoomMapping.cs (NEU - 29.10.2025)
??? Plugins/
    ??? Android/
    ?   ??? NFCHelper.java (NEU - 29.10.2025)
    ?   ??? AndroidManifest.xml (AKTUALISIERT - NFC Support)
    ??? iOS/
        ??? NFCPlugin.mm (NEU - 29.10.2025)
        ??? UnityApp.entitlements (NEU - NFC Entitlements)

### ?? DEVELOPMENT NOTES

#### Mehrsprachigkeitssystem (IMPLEMENTIERT + POLISHED)
- **Architektur**: Custom ScriptableObject-System statt Unity Localization Package
- **Performance**: Sprach-Caching und Event-basierte Updates minimieren Overhead
- **Fallback-Hierarchie**: 
  1. Leichte Sprache ? Standard-Variante
  2. Englisch ? Deutsch
  3. Alle ? Deutsch Standard
- **Mobile-First**: Touch-optimierte Settings-UI mit Haptic Feedback
- **UI-Vereinfachung**: LanguageSettingsManager stark vereinfacht (Apply Button entfernt)
- **Sofortige Sprachwechsel**: Radio-Button-Click triggert direkt GameDataManager.SetLanguage()
- **Extensibility**: Neue Sprachen durch Language-Enum-Erweiterung
- **.NET Framework 4.7.1 Kompatibilität**: System.Enum.GetValues(typeof()) statt generischer Variante

#### Mobile-First Approach (IMPLEMENTIERT)
- **Plattform**: iOS/Android primär, WebGL-Features entfernt
- **UI**: Safe Area Support, Touch-Target-Optimierung
- **Input**: Accelerometer + Touch-Fallbacks, Haptic Feedback
- **Performance**: Mobile-optimierte Rendering und Memory-Management

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- **Modularity**: Jeder Schwierigkeitsgrad hat eigene Content-Sets
- **Persistence**: Team-Settings bleiben über gesamte Führung erhalten
- **Flexibility**: Zeit-Multiplikatoren und Content-Anpassungen pro Level
- **Extensibility**: Einfache Erweiterung um weitere Schwierigkeitsgrade

#### Content Management (ERWEITERT)
- **ScriptableObjects**: Für einfache Content-Erstellung mit Difficulty + Language-Support
- **Validation**: Automatische Prüfung ob Content für alle Schwierigkeitsgrade und Sprachen vorhanden
- **Skalierbarkeit**: System designed für 6 verschiedene Räume + mehrere Minispiele
- **Lokalisierung**: Vollständig implementiert mit Fallback-System
- **Legacy-Support**: Bestehende Inhalte ohne Lokalisierung funktionieren weiterhin
- **Keine Hardcoding**: Alle Game-Manager vollständig auf LocalizedText Assets umgestellt

#### Technical Architecture (ERWEITERT)
- **Localization System**: Event-basiert mit automatischer UI-Aktualisierung
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabhängig mit Backup-Funktionalität, Team-Settings und Sprache
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilität
- **Enum-Namespace**: DifficultyLevel global, LanguageSystem.Language in Klasse (WICHTIG für Kompatibilität!)

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### MEHRSPRACHIGKEITSSYSTEM DETAILS (IMPLEMENTIERT + POLISHED)

#### Fallback-Hierarchie
1. **Erste Stufe**: Leichte Sprache ? Standard-Variante (German_Simple ? German_Standard)
2. **Zweite Stufe**: Ziel-Sprache ? Fallback-Sprache (English ? German)
3. **Dritte Stufe**: Alle ? German_Standard (final fallback)

#### Performance-Optimierungen
- **Language Caching**: Aktuelle Sprache wird gecacht für bessere Performance
- **Event-basierte Updates**: OnLanguageChanged Event minimiert unnötige Updates
- **Lazy Loading**: Texte werden nur bei Bedarf geladen

#### Content-Validation
- **HasAllLocalizations()**: Prüft Vollständigkeit aller Übersetzungen
- **Context-Menu Validation**: Editor-Tools für Content-Creators
- **Debug-Logging**: Optional aktivierbare Fallback-Logs

#### Legacy-Support
- **Rückwärtskompatibilität**: Bestehende Inhalte funktionieren ohne Änderungen
- **Obsolete-Markierungen**: Sanfte Migration zu neuen lokalisierten Methoden
- **Fallback-Texte**: Hardcodierte Fallbacks für alle Sprachen als Backup

#### LanguageSettingsManager Vereinfachung (NEU)
- **Entfernt**: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- **Beibehalten**: LanguageRadioGroup, Back Button, Safe Area Support
- **Verbessert**: Sofortige Sprachwechsel ohne Apply-Step
- **Reduziert**: Von ~250 auf ~140 Zeilen Code

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (IMPLEMENTIERT)
- **Team-Settings**: Persistent über GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty() / GetTermsForDifficulty()
- **UI-System**: Image-Swapping Radio-Buttons für bessere Mobile-UX
- **Zeit-System**: Multiplikatoren für angepasste Rundendauer
- **Fallback**: Legacy-Support für bestehende Content-ScriptableObjects

### MOBILE-OPTIMIERUNG DETAILS (IMPLEMENTIERT)
- **Safe Area**: Automatische Anpassung an iOS Notch und Android Cutouts
- **Touch-Targets**: Mindestgröße 60 Unity Units für optimale Bedienbarkeit
- **Haptic Feedback**: Vibration bei allen wichtigen Interaktionen
- **Platform-Detection**: Automatische Anpassung iOS vs Android Features

### SPLIT-SCREEN QUIZ DETAILS (ERWEITERT + LOKALISIERT)
- **Multi-Language**: Teams erhalten Fragen in gewählter Sprache
- **Live Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Lokalisierte Feedback**: Alle Punkte-Nachrichten und Gewinner-Texte
- **Multi-Difficulty**: Teams können unterschiedliche Fragen basierend auf Schwierigkeitsgrad erhalten
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers(Language)
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System für saubere Button-States
- **Continue-System**: Touch-anywhere nach Feedback-Phase

### FOSSILIEN-STIRNRATEN DETAILS (VOLLSTÄNDIG LOKALISIERT)
- **100% LocalizedText Assets**: ALLE UI-Texte über ScriptableObjects
- **Keine hardcodierten Strings mehr**: Explanation-Screen, Input-Modi, Anweisungen
- **Lokalisierte Fossilien**: Fossil-Namen in gewählter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gewählte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- **Dynamische Input-Mode-Texte**: GetLocalizedInputModeInfo() prüft IsUsingAccelerometer()
- **CRITICAL FIX (19.10.2025)**: Event-Reihenfolge in TriggerCorrect/TriggerSkip geändert
  - Haptic Feedback ? DisableInputTemporarily() ? Event-Invoke
  - Verhindert ungewollte Vibrationen auf Explanation Screen
  - CancelInvoke() in SetInputEnabled() funktioniert jetzt korrekt
- **Adaptive Zeiten**: GetAdjustedRoundDuration() basierend auf Team-Schwierigkeitsgrad
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### INVOKE-CALLBACK & EVENT-REIHENFOLGE BEST PRACTICES (19.10.2025)

#### Problem-Pattern (VERMEIDEN)

// FALSCH: Event ZUERST
public void TriggerAction()
{
    OnAction?.Invoke();              // Kann SetInputEnabled(false) aufrufen
    TriggerHapticFeedback();
    DisableInputTemporarily(0.5f);   // Invoke wird NACH SetInputEnabled registriert
}

#### Lösung-Pattern (VERWENDEN)

// RICHTIG: Event als LETZTES
public void TriggerAction()
{
    TriggerHapticFeedback();         // Sofortige Aktionen zuerst
    DisableInputTemporarily(0.5f);   // Invoke registrieren
    OnAction?.Invoke();              // Event triggert SetInputEnabled ? CancelInvoke löscht Invoke
}

#### Regeln
1. **Invoke-Callbacks vor Events**: Alle Invoke() Calls VOR Event-Invocations
2. **CancelInvoke() nach Invoke()**: CancelInvoke() kann nur NACH Invoke() greifen
3. **Sofortige Aktionen zuerst**: Haptic Feedback, Audio, UI-Updates vor Callbacks
4. **Events als Letztes**: Events können State ändern ? sollten letzte Aktion sein
5. **Race-Conditions beachten**: Asynchrone Callbacks können State-Management brechen
### NFC-SYSTEM (VOLLSTÄNDIG IMPLEMENTIERT - 29.10.2025)

**NFCManager.cs**
- Zentrale NFC-Verwaltung für iOS und Android
- Platform-Detection mit Compile-Directives
- Automatische NFC-Aktivierung bei App-Start
- Polling-System für Android NFC-Intents (0.5s Intervall)
- Integration mit NFCSceneLoader für automatisches Szenen-Laden
- OnApplicationPause-Handling für NFC-Reaktivierung
- UI-Feedback über TextMeshPro (DebugText, StatusText)
- Editor-Test-Support mit Mock-Tags

**NFCHelper.java (Android)**
- Native Android NFC-Integration
- Foreground Dispatch System für Tag-Erkennung
- KRITISCH: Liest NDEF-Daten aus Intent (NICHT direkt vom Tag!)
  - METHODE 1: intent.getParcelableArrayExtra(NfcAdapter.EXTRA_NDEF_MESSAGES) - Bevorzugt!
  - METHODE 2: Direktes Tag-Lesen als Fallback (kann "out of date" Fehler werfen)
  - METHODE 3: Tag-ID als Hex-String (finaler Fallback)
- Löst "Tag is out of date" Problem durch Intent-basiertes Lesen
- Unterstützt NDEF Text Records mit Language-Code-Parsing
- Automatisches Fallback zu Tag-ID wenn NDEF nicht verfügbar
- Detailliertes Android-Logging für Debugging

**NFCPlugin.mm (iOS)**
- Core NFC Framework Integration für iOS 13+
- NFCTagReaderSession mit TAG Entitlement (NICHT NDEF!)
- WICHTIG: Nutzt com.apple.developer.nfc.readersession.formats mit "TAG"
- NDEF Entitlement ist DEPRECATED seit iOS 13
- Objective-C Bridge für Unity-Kommunikation
- UnitySendMessage für Tag-Daten und Fehler

**NFCRoomMapping.cs (ScriptableObject)**
- Mapping zwischen NFC-Tag-IDs und Szenen-Namen
- Unterstützt sowohl NDEF-Text ("ROOM_01") als auch Tag-IDs ("TAG_ID:04A3B2C1")
- Exakte und Partial-Match-Unterstützung
- GetSceneNameForTag() mit Fallback-Logik
- GetDisplayNameForTag() für UI-Anzeige
- Validation-Tools für Content-Prüfung
- CreateAssetMenu Integration für einfache Erstellung

**NFCSceneLoader.cs**
- Scene-Loading-System basierend auf NFC-Tags
- Async Scene Loading mit Progress-Tracking
- Konfigurierbare Delay vor Szenen-Wechsel
- Hub-Scene-Pattern für Rückkehr-Navigation
- Integration mit GameDataManager für Room-Tracking
- IsValidRoomTag() für Tag-Validierung
- ReturnToHub() Methode für Navigation zurück

**AndroidManifest.xml Konfiguration**
- NFC Permission und Feature deklariert
- Intent-Filter für TAG_DISCOVERED und NDEF_DISCOVERED
- launchMode="singleTop" für korrekte Intent-Verarbeitung
- Verhindert Activity-Duplikation bei Tag-Scans

**iOS Entitlements & Info.plist**
- com.apple.developer.nfc.readersession.formats mit "TAG" Array
- NFCReaderUsageDescription für App Store Compliance
- WICHTIG: Nicht "NDEF" verwenden - ist deprecated!

**Testing & Validation**
- Editor-Support mit Mock-Tags ("TEST_ROOM_01")
- Android-Testing: Xiaomi Device mit NFC Tools beschriebenen Tags
- Erfolgreiche Tag-Erkennung und Szenen-Wechsel
- Hub-Pattern funktioniert wie designed

**Nächste Schritte**
- Raum-Szenen erstellen und mit Tags verknüpfen
- Content für NFCRoomMapping ScriptableObject erstellen
- ReturnToHub-Buttons in Raum-Szenen implementieren
- iOS-Testing sobald Device verfügbar

### NFC-SCENE-LOADER - EDITOR DEBUG MODE (NEU - 07.11.2025)

**NFCSceneLoader.cs - Editor-Tastatur-Steuerung**
- **Neues Unity Input System Integration**: Verwendet Keyboard.current statt altem Input Manager
- **Editor-Only Debug-Modus**: Nur in Unity Editor aktiv (via #if UNITY_EDITOR)
- **Tastatur-Steuerung für Raum-Wechsel**: Keine NFC-Tags im Editor nötig

#### Neue Features:
1. **Keyboard-basierte Raum-Auswahl**
   - Tasten 0-9 (Alpha + Numpad): Lädt Raum mit Index 0-9 aus NFCRoomMapping
   - H-Taste: Kehrt zur Hub-Szene zurück
   - P-Taste: Zeigt verfügbare Räume-Liste erneut an
   - Optional: Ctrl als Modifier (konfigurierbar)

2. **Automatische Raum-Liste beim Start**
   - Zeigt alle verfügbaren Räume im Console-Log
   - Format: [Index] DisplayName (Tag: TagID)
   - Hilfstexte für Tastenbelegung

3. **Reflection-basierter Zugriff**
   - GetSceneNameByIndex(int index) - Holt Szenen-Namen via Reflection
   - PrintAvailableRooms() - Listet alle Mappings aus NFCRoomMapping
   - Zugriff auf private roomMappings Array ohne Code-Änderungen

4. **Neue Inspector-Einstellungen** (nur im Editor):
   - enableEditorDebug (bool) - Debug-Modus aktivieren/deaktivieren
   - debugKeyModifier (KeyCode) - Optional: Modifier-Taste (z.B. Ctrl)
   - requireModifier (bool) - Muss Modifier gedrückt werden?

#### Verwendung im Editor:

// Console-Output beim Start:
[NFCSceneLoader] ?? EDITOR DEBUG MODE aktiv!
[NFCSceneLoader] Drücke Tasten 0-9 um Räume zu laden
========== VERFÜGBARE RÄUME (EDITOR DEBUG) ==========
  [0] Fossil-Raum (Tag: ROOM_01)
  [1] Puzzle-Raum (Tag: ROOM_02)
  [2] Quiz-Raum (Tag: ROOM_03)
?? Drücke Taste 0-2 zum Laden
?? Drücke 'H' für Hub-Szene
?? Drücke 'P' um diese Liste erneut anzuzeigen


#### Technische Details:
- **Input System Integration**: using UnityEngine.InputSystem (nur Editor)
- **Keyboard.current**: Neues Input System API statt Input.GetKeyDown()
- **Tastatur-Abfragen**:
  - keyboard.digit0Key.wasPressedThisFrame (Zifferntasten)
  - keyboard.numpad0Key.wasPressedThisFrame (Numpad)
  - keyboard.hKey.wasPressedThisFrame (H-Taste)
  - keyboard.ctrlKey.isPressed (Modifier-Check)
- **Build-Kompatibilität**: Debug-Code wird in Builds automatisch entfernt
- **Kein Input-Mode-Wechsel**: Funktioniert parallel zu Android/iOS Input

#### Vorteile:
- ? **Kein NFC-Scanner benötigt** für PC-Testing
- ? **Schnelles Testen** aller Räume ohne Hardware
- ? **Kein Unity-Neustart** bei Input-Änderungen
- ? **Build-Safe**: Editor-Code wird automatisch entfernt
- ? **Konsistent**: Nutzt gleiches Input System wie FossilInputHandler

#### Integration Points:
- Start(): Keyboard-Initialisierung, Auto-Print verfügbarer Räume
- Update(): Tastatur-Abfrage nur wenn enableEditorDebug == true
- LoadRoomByIndex(int index): Wrapper für Index-basiertes Scene-Loading
- GetSceneNameByIndex(int index): Reflection-Access auf NFCRoomMapping
- PrintAvailableRooms(): Console-Output aller verfügbaren Räume

#### Debug-Workflow:
1. NFCSceneLoader im Hub platzieren
2. NFCRoomMapping Asset zuweisen
3. Play-Mode starten
4. Console öffnen ? Raum-Liste anschauen
5. Taste drücken ? Raum wird geladen
6. H drücken ? Zurück zum Hub

### TEAM-NAMEN-SYSTEM DETAILS (NEU - 19.11.2025)

#### TeamIconProvider-Erweiterung
- **icon0Name, icon1Name, icon2Name**: LocalizedText pro Icon (optional)
- **GetIconName(index)**: Gibt LocalizedText zurück (kann null sein)
- **GetTeam1IconNameText(Language)**: Helper für direkten String-Zugriff
- **HasAllNames()**: Validation für Vollständigkeit
- **Fallback**: "Team 1" / "Team 2" wenn kein Name zugewiesen

#### UpdateTeamNames() Pattern (Alle Game-Manager)

private void UpdateTeamNames()
{
    if (teamIconProvider == null) return;
    
    // Aktuelles Team (für Explanation/Countdown/Gameplay)
    string currentTeamName = teamIconProvider.GetTeamIconNameText(currentTeam - 1, currentLanguage);
    
    if (explanationTeamNameText != null)
    {
        explanationTeamNameText.text = currentTeamName;
        explanationTeamNameText.gameObject.SetActive(true);
    }
    
    // Results Screen - beide Teams
    if (team1NameText != null)
    {
        team1NameText.text = teamIconProvider.GetTeam1IconNameText(currentLanguage);
        team1NameText.gameObject.SetActive(true);
    }
    // ... team2NameText analog
}


#### Aufruf-Zeitpunkte
- **ShowExplanationScreen()**: Nach UpdateTeamIconForImage()
- **ShowCountdownScreen()**: Nach UpdateTeamIconForImage()
- **ShowGameplayScreen()**: Nach UpdateTeamIconForImage()
- **ShowResultsScreen()**: In UpdateResultsUI() am Anfang
- **HandleLanguageChanged()**: Bei allen States außer Solution

#### UI-Referenzen (Optional!)
- **explanationTeamNameText**: Name auf Explanation Screen (aktuelles Team)
- **countdownTeamNameText**: Name auf Countdown Screen (aktuelles Team)
- **gameplayTeamNameText**: Name auf Gameplay Screen (aktuelles Team)
- **team1NameText**: Name auf Results Screen (Team 1)
- **team2NameText**: Name auf Results Screen (Team 2)
- **winnerNameText**: Gewinner-Name auf Results Screen (optional)

#### TeamSettingsManager-Fix
- **OnTeam1/2IconChanged()**: Speichert Index SOFORT in GameDataManager
- **Reihenfolge kritisch**: SetTeamIconIndex() VOR UpdateTeamNames()
- **Problem gelöst**: Namen wurden mit altem Icon-Index aktualisiert
- **Grund**: UpdateTeamNames() liest Index aus GameDataManager

### TEAM-START-REIHENFOLGE DETAILS (NEU - 19.11.2025)

#### Implementierungspattern

// Header-Feld in allen 3 Managern (Puzzle, Fossil, Tabu)
[Header("Game Flow Settings")]
[SerializeField] private bool team2StartsFirst = false;
[Tooltip("Wenn aktiviert, beginnt Team 2 statt Team 1 das Spiel")]

// InitializeGame() - Puzzle/Fossil/Tabu
currentTeam = team2StartsFirst ? 2 : 1;

// RestartGame() - Fossil/Tabu
currentTeam = team2StartsFirst ? 2 : 1;


#### EndRound() Fix für Fossil & Tabu

**Problem**: 1 Runde pro Team, aber bei team2StartsFirst endete Spiel zu früh

**Alte Logik (FALSCH)**:

if (currentTeam == 1)  // Funktioniert nur wenn Team 1 startet!
{
    currentTeam = 2;
}
else
{
    ShowResults();
}


**Neue Logik (KORREKT)**:

int startingTeam = team2StartsFirst ? 2 : 1;
int secondTeam = team2StartsFirst ? 1 : 2;

if (currentTeam == startingTeam)
{
    currentTeam = secondTeam;  // Nach Startteam ? Zum 2. Team
    ShowExplanation();
}
else
{
    ShowResults();  // 2. Team fertig ? Spiel endet
}


#### Warum Puzzle KEINEN Fix braucht
- **Puzzle hat currentRound-Tracking**: Globaler Round-Counter über beide Teams
- **GetCurrentTeamRound()**: Mapped globalen Counter auf team-spezifische Runde
- **Mehrere Runden pro Team**: roundsPerTeam kann > 1 sein
- **Team-Wechsel-Logik**: Basiert auf totalRounds, nicht auf currentTeam-Wert
- **Funktioniert automatisch**: Mit oder ohne team2StartsFirst

#### Optional - Automatisches Toggle (NICHT implementiert)
Falls später gewünscht - Pattern für automatisches Wechseln:


// Bei EndRound() nach letztem Team:
PlayerPrefs.SetInt("LastStartingTeam", currentTeam);

// Bei InitializeGame():
int lastStartingTeam = PlayerPrefs.GetInt("LastStartingTeam", 1);
currentTeam = lastStartingTeam == 1 ? 2 : 1;  // Toggle


**Grund für manuelle Kontrolle**:
- Designer hat volle Kontrolle
- Keine versteckten State-Änderungen
- Einfacher zu testen und debuggen
- Pro Collection individuell konfigurierbar

# PROJEKT-UPDATE: TEAM-ICON-AUSWAHL-SYSTEM (05.11.2025)

## NEUE FEATURES IMPLEMENTIERT

### TEAM-ICON-AUSWAHL-SYSTEM (VOLLSTÄNDIG IMPLEMENTIERT)
- **Zweck**: Jedes Team kann vor dem Spiel aus 3 verschiedenen Icons wählen
- **Persistenz**: Auswahl wird über GameDataManager gespeichert
- **UI-System**: Radio-Button-Pattern mit Sprite-Swapping für Selected/Unselected States
- **Integration**: Alle 4 Minispiele verwenden dynamisch ausgewählte Icons

#### Neue Dateien erstellt:
1. **TeamIconProvider.cs (ScriptableObject)** - Zentrales Icon-Management
   - TeamIconSet für je 3 Icons pro Team (icon0, icon1, icon2)
   - GetTeam1Icon() / GetTeam2Icon() / GetTeamIcon(teamIndex)
   - Liest Icon-Index aus GameDataManager
   - Validation-Tools für Content-Prüfung
   
2. **TeamIconRadioGroup.cs (UI Component)** - Icon-Auswahl-UI
   - Radio-Button-System analog zu DifficultyRadioGroup
   - IconButton Struktur (Button, IconImage, normalSprite, selectedSprite)
   - Sprite-Swapping für visuelle Hervorhebung der Auswahl
   - Event-System: OnIconChanged(int iconIndex)
   - Haptic Feedback Integration
   - Mobile-optimierte Touch-Targets

#### Erweiterte Dateien:

**DifficultySystem.cs**
- TeamSettings erweitert mit team1IconIndex und team2IconIndex (0-2)
- GetTeamIconIndex(teamIndex) und SetTeamIconIndex(teamIndex, iconIndex)
- Clamp-System für sichere Index-Verwaltung (0-2)

**GameDataManager.cs**
- Icon-Management-Methoden hinzugefügt:
  - SetTeamIconIndex(teamIndex, iconIndex)
  - GetTeamIconIndex(teamIndex)
  - SetTeamIconIndices(team1IconIndex, team2IconIndex)
- Properties: Team1IconIndex, Team2IconIndex
- Icon-Indizes werden validiert und geclampt bei LoadGameData()
- Debug-Methoden für Icon-Testing
- Rückwärtskompatibilität: Alte Saves funktionieren weiterhin (Default: Icon 0)

**TeamSettingsManager.cs**
- UI erweitert mit team1IconGroup und team2IconGroup
- Icon-Change-Handler: OnTeam1IconChanged(), OnTeam2IconChanged()
- ApplySettings() speichert Icon-Indizes zusätzlich zu Difficulty
- InitializeSettings() lädt gespeicherte Icon-Indizes
- SetupUI() bindet Icon-Group-Events

**SplitScreenQuizManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf QuizRoomData.team1/2Image
- Icons werden aktualisiert:
  - Bei Start() nach InitializeQuiz()
  - Bei ShowResults() für Results Screen
  - Bei RestartQuiz() für Icon-Refresh
- player1TeamIcon und player2TeamIcon UI-Referenzen hinzugefügt
- result1TeamIcon und result2TeamIcon für Results Screen
- Legacy-Fallback: Funktioniert ohne TeamIconProvider (nutzt alte team1/2Image)
- DEPRECATED-Warnung für alte QuizRoomData team images

**FossilGameManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf FossilCollection.team1/2Image
  - UpdateCurrentTeamImage(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei InitializeGame() nach SetupUI()
  - Bei ShowResults() für Results Screen
  - Bei RestartGame() für Icon-Refresh
- Explanation, Countdown, Gameplay, Results Screens verwenden dynamische Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- DEPRECATED-Warnung für alte FossilCollection team images

**TabuGameManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf TabuCollection.Team1/2Image
  - UpdateCurrentTeamImage(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei InitializeGame() nach SetupUI()
  - Bei ShowResults() für Results Screen
  - Bei RestartGame() für Icon-Refresh
- Explanation, Gameplay, Results Screens verwenden dynamische Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- DEPRECATED-Warnung für alte TabuCollection team images

**PuzzleGameManager.cs** (TeamIconProvider Integration) - NEU ENTDECKT UND ANGEPASST
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamNumber) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf PuzzleCollection.team1/2Image
  - UpdateCurrentTeamImageLegacy(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei Start() nach InitializeGame()
  - Bei ShowExplanationScreen() für Explanation Icon
  - Bei ShowCountdownScreen() für Countdown Icon
  - Bei ShowGameplayScreen() für Gameplay Icon
  - Bei ShowResultsScreen() für Results Screen Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- Team-Difficulty-Index-Fix: GetTeamDifficulty(0/1) statt (1/2) für Konsistenz
- DEPRECATED-Warnung für alte PuzzleCollection team images

### TECHNISCHE DETAILS - TEAM-ICON-SYSTEM

#### Icon-Index-System
- **3 Icons pro Team**: Index 0, 1, 2 (insgesamt 6 Icons)
- **Default**: Icon 0 für beide Teams bei neuen Saves
- **Persistenz**: Gespeichert in GameProgressData.teamSettings
- **Zugriff**: GameDataManager.GetTeamIconIndex(0/1) - 0-basiert!

#### TeamIconProvider-Architektur
- **ScriptableObject-basiert**: Zentrale Asset-Verwaltung
- **TeamIconSet-Klasse**: Kapselt 3 Icons + GetIcon(index) Methode
- **GetTeamIcon(teamIndex)**: Kombiniert Team-Auswahl + Icon-Index aus GameDataManager
- **Validation**: ValidateIcons() prüft Vollständigkeit, HasAllIcons() für einzelne Sets
- **Fallback-System**: Legacy team1/2Image aus Collections als Backup

#### UI-Pattern: TeamIconRadioGroup
- **Analog zu DifficultyRadioGroup**: Konsistentes UI-System
- **IconButton-Struktur**: 
  - Button (Unity UI Button Component)
  - IconImage (Image Component für Sprite-Anzeige)
  - normalSprite (nicht ausgewählt - z.B. Graustufen-Icon)
  - selectedSprite (ausgewählt - z.B. Farbiges Icon mit Rahmen)
- **Sprite-Swapping statt Farbwechsel**: Bessere Kontrolle über visuelle Hervorhebung
- **Mobile-optimiert**: Touch-Target-Größen, Haptic Feedback
- **Event-System**: OnIconChanged(int iconIndex) für externe Handler

#### Game-Manager-Integration-Pattern
ALLE 4 Game-Manager verwenden identisches Pattern:

1. **Header-Feld**: [SerializeField] private TeamIconProvider teamIconProvider;
2. **UpdateTeamIcons()**: Zentrale Methode, aufgerufen bei:
   - InitializeGame() / Start()
   - ShowResults() (für Results Screen Refresh)
   - RestartGame() (für Icon-Refresh nach Änderungen)
3. **UpdateTeamIconForImage(Image, teamIndex)**: Helper für einzelne Icon-Updates
4. **SetupTeamIconsLegacy()**: Fallback wenn teamIconProvider == null
5. **Legacy-Warnung**: Debug.LogWarning bei Verwendung alter Collection-Images

#### Konsistenz-Fixes
- **Team-Difficulty-Index**: Jetzt einheitlich 0-basiert (0 = Team 1, 1 = Team 2)
  - SplitScreenQuizManager: team1Difficulty = GetTeamDifficulty(0)
  - FossilGameManager: team1Difficulty = GetTeamDifficulty(0)
  - TabuGameManager: team1Difficulty = GetTeamDifficulty(0)
  - PuzzleGameManager: team1Difficulty = GetTeamDifficulty(0) - FIXED!
- **teamIndex vs teamNumber**: 
  - teamIndex ist 0-basiert (für Arrays/Provider)
  - teamNumber ist 1-basiert (für UI-Anzeige)
  - Konvertierung: teamIconProvider.GetTeamIcon(teamNumber - 1)

### RÜCKWÄRTSKOMPATIBILITÄT

**Alte Saves funktionieren weiterhin:**
- Fehlende icon-Indizes werden mit 0 initialisiert (Default-Icon)
- GameDataManager migriert Saves automatisch
- Legacy team1Image/team2Image in Collections werden als Fallback genutzt
- Keine Breaking-Changes für bestehende Inhalte

**Migration-Path:**
1. Bestehende Saves laden automatisch mit Icon-Index 0 für beide Teams
2. TeamIconProvider-Asset zuweisen (optional, Legacy-Fallback funktioniert)
3. Icon-Sprites in TeamIconProvider zuweisen
4. Teams können Icons in TeamSettings auswählen
5. Alte Collection-Images können entfernt werden (aber nicht notwendig)

### CONTENT-REQUIREMENTS

**Für vollständige Nutzung benötigt:**
1. **TeamIconProvider.asset** erstellen (Create ? Museum Quiz ? Team Icon Provider)
2. **6 Icon-Sprites** zuweisen:
   - Team 1: icon0, icon1, icon2
   - Team 2: icon0, icon1, icon2
3. **12 Selected-Sprites** erstellen (für Hervorhebung):
   - Gleiche Icons mit visueller Hervorhebung (Rahmen, Farbe, Glow, etc.)
4. **TeamIconRadioGroup UI-Setup** in TeamSettings-Scene:
   - 2x TeamIconRadioGroup Components (Team 1 + Team 2)
   - Je 3 IconButton-Strukturen konfigurieren
5. **Game-Manager UI-Referenzen** in allen 4 Game-Szenen:
   - TeamIconProvider Asset zuweisen
   - Bestehende Team-Icon-Image-Referenzen bleiben (für Legacy-Fallback)

## GAME DESIGN - BILDERRÄTSEL/PUZZLE-GAME (VOLLSTÄNDIG IMPLEMENTIERT)

### KERNMECHANIK
- **Gameplay**: Find-the-Exhibit Style - Spieler finden Exponate im Raum anhand von Bildausschnitten
- **Teams**: 2 Teams spielen abwechselnd mit individuellen Schwierigkeitsgraden
- **Rundensystem**: Konfigurierbare Anzahl Runden pro Team (roundsPerTeam)
- **Hinweis-System**: 2 verfügbare Hinweise (größere Bildausschnitte) mit Punktabzug
- **Punktesystem**: Startpunkte - (Hinweise × hintPenalty)
- **Timer**: Optional, schwierigkeitsgrad-abhängig

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
- **Kids**: Einfachere Puzzles, optionaler Timer (1.5x länger)
- **BigKids**: Mittlere Komplexität, Timer möglich (1.2x länger)
- **Adults**: Volle Komplexität, Timer standardmäßig aktiv (1.0x)
- **Content-Sets**: Separate PuzzlePiece-Arrays pro Schwierigkeitsgrad

### SPIELABLAUF
1. **Explanation Screen**: Regeln, Team-Icon, Schwierigkeitsgrad-Info, Spieleinstellungen
2. **Countdown**: 3-2-1 mit unterschiedlichen Sounds
3. **Gameplay Phase**:
   - Bildausschnitt (hint0) wird angezeigt
   - Mögliche Punkte werden angezeigt
   - Buttons: "Gefunden!", "Hinweis anzeigen", "Aufgeben"
   - Hinweis-System: hint0 ? hint1 ? hint2 (3 Bilder total)
   - Jeder Hinweis kostet Punkte (hintPenalty)
   - Optional: Timer mit Audio-Warnung bei letzten 3 Sekunden
4. **Solution Screen**: Vollständiges Bild, Exponat-Name, Beschreibung, verdiente Punkte
5. **Team-Wechsel**: Nach Team 1 spielt Team 2
6. **Results Screen**: Gewinner-Ermittlung, Scores, Team-Icons

### FEATURES
- **Hinweis-System**: 2 Hinweise verfügbar (größere Bildausschnitte)
  - hint0: Kleinster Ausschnitt (Start)
  - hint1: Mittlerer Ausschnitt (1. Hinweis)
  - hint2: Großer Ausschnitt (2. Hinweis)
  - Jeder Hinweis kostet konfigurierbare Punkte (hintPenalty)
- **Punktesystem**: startingPoints - (hintsUsed × hintPenalty)
- **Aufgeben-Option**: Popup mit Bestätigung, 0 Punkte für Runde
- **Timer-System**: 
  - Optional pro Schwierigkeitsgrad aktivierbar
  - Time-Up Popup bei Ablauf
  - Audio-Warnung bei letzten 3 Sekunden
- **Team-Exklusion**: Team 2 bekommt (wenn möglich) andere Puzzles als Team 1
- **Lokalisierung**: Vollständig lokalisiert (Exponat-Namen, Beschreibungen, UI)
- **Mobile-optimiert**: Haptic Feedback, Portrait-Vollbild, Touch-optimiert
- **Team-Icon-System**: Dynamische Icons wie bei anderen Minispielen

### PUNKTESYSTEM
- **Startpunkte**: Konfigurierbar in PuzzleCollection (Standard: 3)
- **Hinweis-Kosten**: Konfigurierbar in PuzzleCollection (Standard: 1)
- **Gefunden ohne Hinweis**: Volle Punktzahl (z.B. 3 Punkte)
- **Gefunden mit 1 Hinweis**: Startpunkte - 1 (z.B. 2 Punkte)
- **Gefunden mit 2 Hinweisen**: Startpunkte - 2 (z.B. 1 Punkt)
- **Aufgegeben oder Zeit abgelaufen**: 0 Punkte

### HAUPTKLASSEN - PUZZLE-GAME (VOLLSTÄNDIG IMPLEMENTIERT)

**PuzzleGameManager.cs**
- Vollständige Implementierung des Puzzle-Minispiels
- Fünf Screens: Explanation, Countdown, Gameplay, Solution, Results
- Vollständig lokalisiert (21 LocalizedText Assets)
- Hinweis-System mit Punktabzug
- Timer-System optional mit Audio-Warnung
- Give-Up und Time-Up Popups mit Bestätigung
- Team-Exklusions-System (Team 2 bekommt andere Puzzles)
- Schwierigkeitsgrad-basierte Runden-Timer
- OnLanguageChanged Event-Handler für Live-Updates
- GetLocalizedText() Helper-Methode mit Fallback-System
- Haptic Feedback bei Button-Klicks
- Integration mit GameDataManager für Team-Schwierigkeitsgrade
- **NEU**: TeamIconProvider Integration für dynamische Team-Icons
- Team-Difficulty-Index-Fix: Jetzt 0-basiert (0 = Team 1, 1 = Team 2)

**PuzzlePiece.cs (ScriptableObject)**
- Datenstruktur für einzelne Puzzle-Exponate
- Vollständig lokalisiert (4 Sprachen)
- Drei Hint-Images (hint0, hint1, hint2) für progressives Enthüllen
- Solution-Image (vollständiges Exponat-Foto)
- Exponat-Name in allen 4 Sprachen
- Optional: Exponat-Beschreibung in allen 4 Sprachen
- GetExhibitName(Language) Methode mit Fallback-Hierarchie
- GetExhibitDescription(Language) Methode mit Fallback-Hierarchie
- GetHintImage(hintLevel) Methode (0-2)
- HasAllLocalizations() für Content-Validation
- ValidateLocalizations() Context-Menu für Editor
- CreateAssetMenu Integration

**PuzzleCollection.cs (ScriptableObject)**
- Container für Puzzle-Stücke mit Schwierigkeitsgrad-Sets
- Analog zu FossilCollection und TabuCollection strukturiert
- Lokalisierter Collection-Name über LocalizedText
- Separate PuzzlePiece-Arrays für Kids/BigKids/Adults
- Team-Images für visuelle Darstellung (DEPRECATED - TeamIconProvider nutzen)
- Konfigurierbare Spieleinstellungen:
  - roundsPerTeam (Anzahl Runden pro Team)
  - startingPoints (Punkte bei Start)
  - hintPenalty (Punktabzug pro Hinweis)
  - enableTimerKids, enableTimerBigKids, enableTimerAdults
  - roundDuration (Basis-Rundendauer)
- DifficultyTimeSettings Integration
- GetPuzzlesForDifficulty(DifficultyLevel) Methode
- GetRandomPuzzles(count, difficulty) Methode
- GetRandomPuzzlesExcluding(count, difficulty, excludeList) für Team 2
- GetAdjustedRoundDuration(difficulty) für angepasste Zeiten
- UseTimerForDifficulty(difficulty) für Timer-Aktivierung
- GetTeamImage(teamNumber) - DEPRECATED, nutzt Legacy team1/2Image
- HasPuzzlesForAllDifficulties() Validation
- HasAllLocalizations() für Content-Validation
- Context-Menu Validierungs-Tools
- CreateAssetMenu Integration

## AKTUALISIERTE PROJEKTSTRUKTUR

Assets/_GAME/
??? Scripts/
?   ??? UI/
?   ?   ??? TeamIconRadioGroup.cs (NEU)
?   ?   ??? TeamSettingsManager.cs (ERWEITERT)
?   ?   ??? DifficultyRadioGroup.cs
?   ?   ??? SceneNavigator.cs
?   ?   ??? LocalizedTextComponent.cs
?   ?   ??? LanguageRadioGroup.cs
?   ?   ??? LanguageSettingsManager.cs
?   ?   ??? FullscreenManager.cs (DEPRECATED für Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED für Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT - Icon-System)
?   ?   ??? FossilGameManager.cs (ERWEITERT - Icon-System)
?   ?   ??? TabuGameManager.cs (ERWEITERT - Icon-System)
?   ?   ??? PuzzleGameManager.cs (ERWEITERT - Icon-System + Dokumentiert)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ?   ??? NFCSceneLoader.cs (ERWEITERT - Editor Debug Mode)
?   ??? Data/
?   ?   ??? TeamIconProvider.cs (ERWEITERT - Team-Namen)
?   ?   ??? GameDataManager.cs (ERWEITERT - Icon-Management)
?   ?   ??? DifficultySystem.cs (ERWEITERT - Icon-Indizes)
?   ?   ??? PuzzlePiece.cs (DOKUMENTIERT)
?   ?   ??? PuzzleCollection.cs (DOKUMENTIERT)
?   ?   ??? Notes/
?       ??? Notes.md (AKTUALISIERT)

## AKTUELLER PROJEKTSTATUS (UPDATE 05.11.2025)

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Team-Icon-Auswahl-System (NEU - 05.11.2025)
- **3 Icons pro Team**: Insgesamt 6 wählbare Team-Icons
- **Persistent**: Auswahl bleibt über GameDataManager gespeichert
- **UI-System**: Radio-Button-Pattern mit Sprite-Swapping
- **All-Game Integration**: Alle 4 Minispiele verwenden dynamische Icons
- **Legacy-Support**: Funktioniert auch ohne TeamIconProvider (Fallback)
- **Mobile-optimiert**: Touch-Targets, Haptic Feedback
- **Content-Ready**: ScriptableObject-System für einfache Icon-Verwaltung

#### Puzzle-Game (VOLLSTÄNDIG DOKUMENTIERT - 05.11.2025)
- **4. Minispiel**: Bilderrätsel - Find-the-Exhibit Gameplay
- **Hinweis-System**: 2 Hinweise mit progressiven Bildausschnitten
- **Timer-System**: Optional, schwierigkeitsgrad-abhängig
- **Punktesystem**: Dynamisch basierend auf Hinweis-Verwendung
- **Team-Exklusion**: Teams bekommen unterschiedliche Puzzles
- **Vollständig lokalisiert**: Exponat-Namen, Beschreibungen, UI
- **TeamIconProvider Integration**: Dynamische Team-Icons implementiert

#### Alle 4 Minispiele mit Team-Icon-System (05.11.2025)
1. ? **Split-Screen Quiz** - TeamIconProvider integriert
2. ? **Fossilien-Stirnraten** - TeamIconProvider integriert
3. ? **Tabu** - TeamIconProvider integriert
4. ? **Bilderrätsel/Puzzle** - TeamIconProvider integriert + vollständig dokumentiert

### ?? NÄCHSTE SCHRITTE

#### Team-Icon-System Setup
1. **TeamIconProvider.asset erstellen**
   - Unity Editor: Create ? Museum Quiz ? Team Icon Provider
   - 6 Icon-Sprites zuweisen (3 pro Team)
   
2. **Icon-Sprites erstellen**
   - 6 Normal-State Sprites (z.B. Graustufen)
   - 6 Selected-State Sprites (z.B. Farbe + Rahmen)
   
3. **TeamSettings UI-Setup**
   - 2x TeamIconRadioGroup Components hinzufügen
   - Je 3 IconButton-Strukturen konfigurieren (Button, Image, normalSprite, selectedSprite)
   
4. **Game-Szenen aktualisieren**
   - TeamIconProvider Asset in allen 4 Game-Managern zuweisen
   - UI-Image-Referenzen prüfen (für Legacy-Fallback)
   
5. **Testing durchführen**
   - Icon-Auswahl in TeamSettings testen
   - Persistenz validieren (Icon bleibt nach App-Neustart)
   - Alle 4 Minispiele mit verschiedenen Icons testen
   - Team-Wechsel und Results-Screens validieren

#### Content-Erstellung für Puzzle-Game
- PuzzlePiece ScriptableObjects erstellen
- Für jedes Puzzle: 3 Hint-Images + 1 Solution-Image
- Exponat-Namen und Beschreibungen lokalisieren
- PuzzleCollection erstellen und konfigurieren
- Puzzles für alle 3 Schwierigkeitsgrade zuweisen

## WICHTIGE IMPLEMENTIERUNGSDETAILS (UPDATE 05.11.2025)

### TEAM-ICON-SYSTEM DETAILS

#### Icon-Index-Verwaltung
- **Speicherort**: GameProgressData.teamSettings.team1IconIndex / team2IconIndex
- **Gültiger Bereich**: 0-2 (3 Icons pro Team)
- **Clamping**: Automatisch bei Load und Set (Mathf.Clamp(iconIndex, 0, 2))
- **Default**: 0 (erstes Icon) für neue Saves und Migration

#### TeamIconProvider-Integration-Pattern
ALLE Game-Manager verwenden identisches Pattern:


// 1. Header-Feld
[SerializeField] private TeamIconProvider teamIconProvider;

// 2. Zentrale Update-Methode
void UpdateTeamIcons()
{
    if (teamIconProvider == null)
    {
        SetupTeamIconsLegacy(); // Fallback
        return;
    }
    
    UpdateTeamIconForImage(explanationIcon, currentTeam);
    UpdateTeamIconForImage(gameplayIcon, currentTeam);
    // ... weitere Icons ...
    
    team1ResultImage.sprite = teamIconProvider.GetTeam1Icon();
    team2ResultImage.sprite = teamIconProvider.GetTeam2Icon();
}

// 3. Helper für einzelne Icons
void UpdateTeamIconForImage(Image targetImage, int teamNumber)
{
    if (targetImage == null || teamIconProvider == null) return;
    
    Sprite icon = teamIconProvider.GetTeamIcon(teamNumber - 1); // 1-based ? 0-based
    if (icon != null)
    {
        targetImage.sprite = icon;
        targetImage.gameObject.SetActive(true);
    }
}

// 4. Legacy-Fallback
void SetupTeamIconsLegacy()
{
    team1ResultImage.sprite = collection.team1Image;
    team2ResultImage.sprite = collection.team2Image;
    // ... alte Logik ...
}


#### Aufruf-Zeitpunkte
- **Start() / InitializeGame()**: Initiales Icon-Setup
- **ShowResults()**: Results-Screen Icon-Refresh
- **RestartGame()**: Icon-Refresh nach möglichen Änderungen
- **ShowExplanation() / ShowCountdown() / ShowGameplay()**: Team-spezifische Icons (Puzzle)

#### Team-Index-Konsistenz (WICHTIG!)
- **GameDataManager**: 0-basiert (0 = Team 1, 1 = Team 2)
- **TeamIconProvider**: 0-basiert (0 = Team 1, 1 = Team 2)
- **UI-Anzeige**: 1-basiert (currentTeam = 1 oder 2)
- **Konvertierung**: teamIconProvider.GetTeamIcon(currentTeam - 1)

#### NFC-System mit Editor-Debug-Support (ERWEITERT - 07.11.2025)
- **Editor-Tastatur-Steuerung**: 0-9 für Räume, H für Hub, P für Liste
- **Neues Unity Input System**: Keyboard.current API statt altem Input Manager
- **Reflection-basiert**: Zugriff auf NFCRoomMapping ohne Code-Änderungen
- **Automatische Raum-Liste**: Console-Output beim Editor-Start
- **Build-Safe**: Debug-Code nur im Editor aktiv (#if UNITY_EDITOR)
- **Kein Input-Mode-Wechsel**: Kompatibel mit Android/iOS Input
- **Optional Modifier-Key**: Ctrl + Zahl konfigurierbar

### PUZZLE-GAME DETAILS

#### Hinweis-System-Mechanik
- **3 Bilder pro Puzzle**: hint0 (klein) ? hint1 (mittel) ? hint2 (groß)
- **Start-State**: hint0 wird angezeigt, hintsUsed = 0
- **Hinweis-Click**: currentHintLevel++, hintsUsed++, points -= hintPenalty
- **Max Hinweise**: 2 (hardcoded, immer 2 Hinweise verfügbar)
- **UI-Update**: Button wird deaktiviert nach 2 Hinweisen
- **Punkteberechnung**: currentPossiblePoints = startingPoints - (hintsUsed × hintPenalty)

#### Timer-System-Logik
- **Aktivierung**: UseTimerForDifficulty(difficulty) aus PuzzleCollection
- **Dauer**: GetAdjustedRoundDuration(difficulty) mit DifficultyTimeSettings
- **Audio-Warnung**: Bei 3 Sekunden verbleibend (timerWarningSound)
- **Time-Up**: Popup mit "Weiter"-Button, 0 Punkte für Runde
- **UI-Ausblendung**: timerContainer kann komplett ausgeblendet werden

#### Runden-System
- **currentRound**: Globaler Round-Counter (0-basiert)
- **GetCurrentTeamRound()**: Mapped globalen Counter auf team-spezifische Runde
  - Team 1 spielt bei currentRound 0, 2, 4, ... ? teamRound = currentRound / 2
  - Team 2 spielt bei currentRound 1, 3, 5, ... ? teamRound = (currentRound - 1) / 2
- **totalRounds**: roundsPerTeam × 2 (beide Teams)
- **Team-Wechsel**: currentTeam = currentTeam == 1 ? 2 : 1

#### Screen-Flow mit Coroutines
ALLE UI-Updates verwenden WaitForEndOfFrame-Pattern:


private void ShowExplanationScreen()
{
    explanationScreen.SetActive(true);
    StartCoroutine(UpdateExplanationUIDelayed());
}

private IEnumerator UpdateExplanationUIDelayed()
{
    yield return new WaitForEndOfFrame();
    UpdateExplanationUI(); // Text-Updates nach OnEnable
}


**Grund**: Sicherstellen dass OnEnable() aller UI-Components fertig ist bevor Texte aktualisiert werden

#### Popup-System
- **Give-Up Popup**: Bestätigung erforderlich, 2 Buttons (Ja/Abbrechen)
- **Time-Up Popup**: Info-Popup, 1 Button (Weiter)
- **Popup-Text-Updates**: Auch mit WaitForEndOfFrame-Coroutines
- **Button-Texte**: Lokalisiert über separate LocalizedText Assets (NEU: 3 zusätzliche)

#### Team-Start-Reihenfolge (NEU - 19.11.2025)
- **team2StartsFirst Checkbox**: Optional Team 2 statt Team 1 starten lassen
- **InitializeGame()**: currentTeam = team2StartsFirst ? 2 : 1;
- **RestartGame()**: Gleiche Logik für Consistency
- **Mehrere Runden**: Funktioniert mit roundsPerTeam-System (Reihenfolge bleibt erhalten)

## CONTENT-ANFORDERUNGEN (UPDATE 05.11.2025)

### Team-Icon-System
- **1x TeamIconProvider.asset**: Zentrale Icon-Verwaltung
- **12 Sprites total**:
  - 6 Normal-State Sprites (3 pro Team, nicht ausgewählt)
  - 6 Selected-State Sprites (3 pro Team, ausgewählt/hervorgehoben)
- **Design-Empfehlungen**:
  - Normal: Graustufen oder gedämpfte Farben
  - Selected: Volle Farben + visueller Indikator (Rahmen, Glow, Checkmark)
  - Konsistente Größe (empfohlen: 512×512px)
  - Transparenter Hintergrund (PNG)

### Puzzle-Game Content
- **PuzzlePiece ScriptableObjects**: Pro Exponat 1 Asset
- **4 Images pro Puzzle**:
  - hint0: Kleinster Ausschnitt (z.B. 10% des Bildes)
  - hint1: Mittlerer Ausschnitt (z.B. 25% des Bildes)
  - hint2: Großer Ausschnitt (z.B. 50% des Bildes)
  - solutionImage: Vollständiges Foto des Exponats
- **Texte pro Puzzle** (4 Sprachen):
  - exhibitName: Kurzer Name (z.B. "Ammonit")
  - exhibitDescription: Optional, kurze Info (1-3 Sätze)
- **PuzzleCollection ScriptableObjects**: Pro Raum 1 Asset
- **Mindest-Anzahl Puzzles**: roundsPerTeam × 2 pro Schwierigkeitsgrad

### LocalizedText Assets (Puzzle-Game)
**21 Assets benötigt** (alle bereits im Code referenziert):
1. explanationTitleLocalizedText
2. explanationRulesLocalizedText (mit {0}, {1}, {2} Platzhaltern)
3. startButtonLocalizedText
4. roundCounterLocalizedText (mit {0}/{1} Format)
5. timerLabelLocalizedText
6. possiblePointsLocalizedText (mit {0} Platzhalter)
7. foundButtonLocalizedText
8. hintButtonLocalizedText
9. hintButtonLabelLocalizedText (mit {0}, {1} Platzhaltern)
10. allHintsUsedLocalizedText
11. giveUpButtonLocalizedText
12. giveUpPopupLocalizedText
13. timeUpPopupLocalizedText
14. earnedPointsLocalizedText (mit {0} Platzhalter)
15. nextTeamButtonLocalizedText
16. resultsWinnerLocalizedText (mit {0} Platzhalter)
17. resultsTieLocalizedText
18. backButtonLocalizedText
19. giveUpConfirmButtonLocalizedText (NEU)
20. giveUpCancelButtonLocalizedText (NEU)
21. timeUpContinueButtonLocalizedText (NEU)

---

**LETZTE AKTUALISIERUNG**: 05. November 2025
**HAUPT-FEATURES DIESES UPDATES**:
- Team-Icon-Auswahl-System vollständig implementiert (alle 4 Minispiele)
- Puzzle-Game vollständig dokumentiert
- Team-Difficulty-Index-Konsistenz hergestellt (0-basiert)
- Legacy-Fallback-System für nahtlose Migration

# PROJECT NOTES FÜR GITHUB COPILOT
## WICHTIGE HINWEISE FÜR ZUKÜNFTIGE CHAT-SESSIONS

**ZWECK DIESER DATEI**: Diese Notizen dienen als Kontext für GitHub Copilot in neuen Chat-Sessions, um nahtlos am Projekt weiterarbeiten zu können ohne Zugriff auf vorherige Chat-Historie.

**FORMATIERUNG**: 
- **IN DIESER DATEI**: KEINE dreifachen Anführungszeichen oder Backticks verwenden - diese würden den Markdown-Parser stören
- **FÜR CHAT-OUTPUT**: Code-Snippets MIT Backticks umschließen, damit sie im Chat als Code-Block angezeigt werden
- **BEIM EINFÜGEN**: Die äußeren Backticks werden automatisch entfernt - nur der innere Inhalt wird in die Datei geschrieben

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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
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

## GAME DESIGN - TABU-MINISPIEL (VOLLSTÄNDIG IMPLEMENTIERT + CONTENT ERSTELLT)

### KERNMECHANIK
- **Gameplay**: Tabu-Style Erklärsystem - Begriffe erklären ohne Tabu-Wörter zu verwenden
- **Teams**: 2 Teams spielen abwechselnd (Team 1, dann Team 2) mit individuellen Schwierigkeitsgraden
- **Rundendauer**: Konfigurierbar mit Zeit-Multiplikatoren pro Schwierigkeitsgrad (Standard: 60 Sekunden)
- **Begriffe pro Runde**: Konfigurierbar (Standard: 5)
- **Layout**: Portrait-Vollbild (KEIN Split-Screen), Teams spielen nacheinander

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
- **Kids**: Einfachere Begriffe, weniger/einfachere Tabu-Wörter, länger Zeit (1.5x Multiplikator)
- **BigKids**: Mittlere Komplexität, moderate Tabu-Wörter, etwas länger Zeit (1.2x Multiplikator)
- **Adults**: Vollständige Komplexität, viele Tabu-Wörter, normale Zeit (1.0x Multiplikator)
- **Content-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad

### STEUERUNG
- **Button-basiert**: "Richtig" (rechts) und "Überspringen" (links) Buttons
- **Haptic Feedback**: Mobile Vibration bei Button-Klicks
- **Touch-Targets**: Mobile-optimierte Button-Größen

### SPIELABLAUF
1. Explanation Screen mit Regeln, Team-Bild und Schwierigkeitsgrad-Anzeige
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Erklärungs-Phase:
   - Hauptbegriff oben (groß)
   - Tabu-Wörter darunter (dynamische Anzahl)
   - Timer-Countdown oben
   - Score-Anzeige (X/Y Format)
   - Richtig/Überspringen Buttons unten
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung und Gewinner-Icon

### FEATURES
- **Begriff-Recycling**: Übersprungene Begriffe kommen ans Ende der Queue zurück
- **Timer-Warnung**: Audio-Countdown bei letzten 3 Sekunden
- **Team-Images**: Visuelle Team-Darstellung auf allen Screens
- **Gewinner-Icon**: Gewinner-Team-Image wird auf Results Screen angezeigt
- **Dynamische Tabu-Wörter**: Variable Anzahl pro Begriff (im ScriptableObject definiert)
- **Adaptive Zeiten**: Schwierigkeitsgrad-basierte Rundendauer
- **Team-Exklusion**: Team 2 bekommt (wenn möglich) andere Begriffe als Team 1

### PUNKTESYSTEM
- **Richtig erraten**: +1 Punkt
- **Überspringen**: 0 Punkte (Begriff kommt zurück in Queue)
- **Zeit abgelaufen**: Runde endet
- **Alle Begriffe durch**: Runde endet

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - MEHRSPRACHIGKEITSSYSTEM (IMPLEMENTIERT)

**LanguageSystem.cs**
- Enum Language (German_Standard, English_Standard, German_Simple, English_Simple)
- WICHTIG: Enum ist INNERHALB der LanguageSystem Klasse (Zugriff via LanguageSystem.Language)
- Singleton-Pattern für globalen Zugriff
- Fallback-System: Englisch ? Deutsch, Leichte Sprache ? Standard-Variante ? Deutsch
- Persistente Speicherung in PlayerPrefs
- Events für Sprachwechsel (OnLanguageChanged)
- Mobile-kompatible Implementierung
- FindFirstObjectByType statt FindObjectOfType (Unity 6.2 kompatibel)

**LocalizedText.cs (ScriptableObject)**
- Container für lokalisierte Texte mit 4 Sprach-Varianten
- Automatisches Fallback-System bei fehlenden Übersetzungen
- CreateAssetMenu Integration für einfache Content-Erstellung
- Validation-Methoden für Editor-Workflow
- .NET Framework 4.7.1 kompatible Enum-Iteration

**LocalizedTextComponent.cs**
- UI-Komponente für automatische Text-Lokalisierung
- Auto-Detection von Text und TextMeshPro Komponenten
- Event-basierte Updates bei Sprachwechsel
- Runtime-Lokalisierung ohne Performance-Impact

**LanguageRadioGroup.cs**
- Radio-Button-System für Sprachauswahl (analog zu DifficultyRadioGroup)
- Image-Swapping für bessere Mobile-UX
- Mobile Touch-Target-Optimierung
- Haptic Feedback Integration
- Event-System für Auswahl-Changes
- Debug-Methoden für Testing

**LanguageSettingsManager.cs**
- VEREINFACHT: Nur noch LanguageRadioGroup + Back Button
- ENTFERNT: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- Sofortige Sprachwechsel beim Radio-Button-Click
- Lokalisierte UI-Texte mit Live-Updates
- Mobile-optimierte Safe Area Unterstützung
- Integration mit GameDataManager für persistente Speicherung

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (IMPLEMENTIERT)

**DifficultySystem.cs**
- Enum DifficultyLevel (Kids, BigKids, Adults) - DIREKT im globalen Namespace (NICHT in Klasse!)
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

### HAUPTKLASSEN - SPLIT-SCREEN QUIZ (ERWEITERT FÜR LOKALISIERUNG)

**SplitScreenQuizManager.cs**
- ERWEITERT: Vollständige Lokalisierung aller UI-Texte
- ERWEITERT: Verwendet LocalizedText Assets für alle Feedback-Texte
- ERWEITERT: Language-Parameter für GetQuestionText() und GetShuffledAnswers()
- ERWEITERT: OnLanguageChanged Event-Handler für Live-Updates
- ERWEITERT: Fallback-Texte für alle Sprachen als Backup
- Lädt unterschiedliche Fragen pro Team basierend auf Schwierigkeitsgrad
- LoadQuestionsForTeams() Methode für difficulty-basierte Content-Auswahl
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (VOLLSTÄNDIG LOKALISIERT)

**FossilGameManager.cs**
- ERWEITERT: Vollständige Lokalisierung ALLER UI-Texte (keine hardcodierten Strings mehr)
- ERWEITERT: Neue LocalizedText-Variablen für Anweisungen und Input-Modi
  - holdPhoneInstructionLocalizedText
  - teamExplainsInstructionLocalizedText
  - tiltModeLocalizedText
  - touchModeLocalizedText
- ERWEITERT: GetLocalizedInputModeInfo() für dynamische Input-Mode-Texte
- ERWEITERT: IsUsingAccelerometer() Integration für korrekte Input-Mode-Anzeige
- ERWEITERT: Fallback-Funktionen für alle neuen lokalisierten Texte
- ENTFERNT: Hardcodierte GetLocalizedHoldPhoneText() und GetLocalizedTeamExplainsText()
- Verwendet LocalizedText Assets für alle UI-Texte
- GetLocalizedText() Helper-Methode mit Fallback-System
- OnLanguageChanged Event-Handler für Live-Updates
- UpdateGameplayUI() und UpdateResultsUI() für Language-Updates
- Lädt Fossilien basierend auf Team-Schwierigkeitsgrad
- Angepasste Rundendauer per GetAdjustedRoundDuration()
- Schwierigkeitsgrad-Anzeige im Explanation Screen
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
- IsUsingAccelerometer() Methode für externe Abfragen

**FossilData.cs & FossilCollection.cs (ScriptableObject)**
- ERWEITERT: Lokalisierung für Fossil-Namen und Beschreibungen
- ERWEITERT: GetFossilName(Language) und GetDescription(Language) Methoden
- ERWEITERT: HasLocalization() und HasAllLocalizations() Validation
- ERWEITERT: GetCollectionName(Language) für lokalisierte Collection-Namen
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Inhalte
- Separate Arrays für Kids/BigKids/Adults Fossilien
- GetFossilsForDifficulty() und GetRandomFossils(count, difficulty) Methoden
- DifficultyTimeSettings Integration
- Datenstruktur für Fossil-Begriffe und Bilder
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen pro Collection
- Shuffling-System für zufällige Fossil-Auswahl

### HAUPTKLASSEN - TABU-MINISPIEL (VOLLSTÄNDIG IMPLEMENTIERT + LOKALISIERT + CONTENT ERSTELLT)

**TabuGameManager.cs**
- Vollständige Implementierung des Tabu-Minispiels
- Analog zu FossilGameManager strukturiert für Konsistenz
- Vollständig lokalisiert (12 LocalizedText Assets erstellt)
- Vier Screens: Explanation, Countdown, Gameplay, Results
- Button-basierte Steuerung (Richtig/Überspringen)
- Timer-System mit Audio-Countdown bei letzten 3 Sekunden
- Schwierigkeitsgrad-basierte Rundendauer
- Begriff-Recycling für übersprungene Begriffe (ans Ende der Queue)
- Team-Exklusions-System (Team 2 bekommt andere Begriffe wenn möglich)
- Dynamische Tabu-Wörter-Anzeige (UI-Pool-System)
- Gewinner-Icon auf Results Screen (wie bei FossilGameManager)
- OnLanguageChanged Event-Handler für Live-Updates
- GetLocalizedText() Helper-Methode mit Fallback-System
- Haptic Feedback bei Button-Klicks
- Integration mit GameDataManager für Team-Schwierigkeitsgrade

**TabuTerm.cs (ScriptableObject)**
- Datenstruktur für einzelne Tabu-Begriffe
- Vollständig lokalisiert (4 Sprachen)
- Hauptbegriff in allen 4 Sprachen
- Tabu-Wörter-Arrays in allen 4 Sprachen (dynamische Länge)
- Optional: Sprite für Begriff-Bild
- GetMainTerm(Language) Methode mit Fallback-Hierarchie
- GetTabuWords(Language) Methode mit Fallback-Hierarchie
- HasAllLocalizations() für Content-Validation
- ValidateLocalizations() Context-Menu für Editor
- CreateAssetMenu Integration

**TabuCollection.cs (ScriptableObject)**
- Container für Tabu-Begriffe mit Schwierigkeitsgrad-Sets
- Analog zu FossilCollection strukturiert
- Lokalisierter Collection-Name über LocalizedText
- Separate Term-Arrays für Kids/BigKids/Adults
- Team-Images für visuelle Darstellung
- Konfigurierbare Spieleinstellungen (termsPerRound, roundDuration)
- DifficultyTimeSettings Integration
- GetTermsForDifficulty(DifficultyLevel) Methode
- GetRandomTerms(count, difficulty) Methode
- GetRandomTermsExcluding(count, difficulty, excludeList) für Team 2
- GetAdjustedRoundDuration(difficulty) für angepasste Zeiten
- HasTermsForAllDifficulties() Validation
- HasAllLocalizations() für Content-Validation
- Context-Menu Validierungs-Tools
- CreateAssetMenu Integration

### SHARED SYSTEMS (MOBILE-OPTIMIERT + LOKALISIERT)

**GameDataManager.cs**
- ERWEITERT: Language-Speicherung und Synchronisation mit LanguageSystem
- ERWEITERT: SetLanguage() und CurrentLanguage Property
- ERWEITERT: SyncLanguageWithSystem() für Startup-Synchronisation
- ERWEITERT: Language wird bei ResetProgress() beibehalten
- TeamSettings Speicherung und Verwaltung
- SetTeamDifficulty() und GetTeamDifficulty() Methoden
- Rückwärtskompatibilität für bestehende Saves
- Plattformunabhängiges Speichersystem (PlayerPrefs-basiert)
- Singleton-Pattern für globalen Zugriff
- Speichert Raumergebnisse, Spielerfortschritt, Session-IDs, Team-Einstellungen, Sprache
- Backup-System und Error-Handling
- Funktioniert auf Mobile, Web und Desktop

**QuizRoomData.cs (ScriptableObject)**
- ERWEITERT: Lokalisierung für Raum-Namen mit GetRoomName(Language)
- ERWEITERT: HasAllLocalizations() für Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu für Editor
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Räume
- Separate Question-Arrays für Kids/BigKids/Adults
- GetQuestionsForDifficulty() Methode
- DifficultyTimeSettings Integration
- Validation-Methoden für alle Schwierigkeitsgrade
- Raumspezifische Fragen und Metadaten
- Modularer Aufbau für verschiedene Museumsräume
- Einfache Content-Erweiterung

**QuizQuestion.cs**
- ERWEITERT: Vollständige Lokalisierung für Fragen und Antworten
- ERWEITERT: GetQuestionText(Language) und GetAnswer(index, Language) Methoden
- ERWEITERT: GetShuffledAnswers(Language) und GetCorrectAnswerIndex(Language)
- ERWEITERT: HasAllLocalizations() für Content-Validation
- ERWEITERT: ValidateLocalizations() Context-Menu für Editor
- ERWEITERT: Legacy-Support für bestehende nicht-lokalisierte Fragen
- Fragendatenstruktur mit 4 Antworten pro Sprache
- Automatisches Answer-Shuffling pro Sprache
- Korrekte Antwort Index-Tracking

**PlayerData.cs**
- Spielerstate (Name, Score, hasAnswered, etc.)
- Antwort-Tracking und Score-Management

**SceneNavigator.cs**
- Navigation zwischen Szenen (Settings, MainMenu)
- Audio-Integration für Navigations-Sounds
- Fallback-System für Scene-Namen/Indizes

### MOBILE-SPEZIFISCHE FEATURES (IMPLEMENTIERT)

**Mobile UI Optimizations**
- Safe Area Support für iOS Notch/Android Cutouts
- Touch-Target-Größen (mindestens 60 Unity Units)
- Haptic Feedback für alle Interaktionen
- Platform-Detection (iOS/Android)
- Canvas Scaler Optimierungen für verschiedene Screen-Ratios

**Input Optimizations**
- Accelerometer-basierte Steuerung für Mobile (Fossil-Stirnraten)
- Button-basierte Steuerung für Mobile (Tabu)
- Touch-Fallback-Systeme
- Vibration/Haptic Feedback Integration

## AKTUELLER PROJEKTSTATUS

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Mehrsprachigkeitssystem (IMPLEMENTIERT + POLISHED)
- **Vier Sprachen**: Deutsch Standard, Englisch Standard, Deutsch Einfach, Englisch Einfach
- **Custom ScriptableObject System**: Optimiert für Unity-Workflow
- **Fallback-Hierarchie**: Automatische Fallbacks bei fehlenden Übersetzungen
- **Game-Manager Integration**: Alle drei Hauptspiele vollständig lokalisiert (KEINE hardcodierten Strings mehr)
- **Live Language-Switching**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimierte Settings-UI**: Vereinfachtes Radio-Button-System mit sofortigem Sprachwechsel
- **Performance-optimiert**: Sprach-Caching und Event-basierte Updates
- **Erweiterbar**: Einfache Integration weiterer Sprachen möglich
- **Legacy-Support**: Bestehende Inhalte funktionieren weiterhin
- **Content-Validation**: Editor-Tools für Übersetzungs-Vollständigkeit

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl für Quiz, Fossilien und Tabu
- Zeit-Multiplikatoren für verschiedene Schwierigkeitsgrade
- Mobile-optimierte UI mit Haptic Feedback

#### Split-Screen Quiz (ERWEITERT + LOKALISIERT)
- **Vollständige Lokalisierung**: Alle UI-Texte, Feedback-Nachrichten, Ergebnisse
- **Live Language-Updates**: Automatische Aktualisierung bei Sprachwechsel
- **Lokalisierte Fragen**: Teams erhalten Fragen in gewählter Sprache
- Difficulty-basierte Fragen pro Team
- Split-Screen Quiz-Mechanik
- Touch-Input System mit fullscreen continue
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management mit anpassbaren Farben
- ScriptableObject-basierte Content-Struktur
- Icon-basiertes Progress-System
- Feedback-System mit Editor-konfigurierbaren Farben

#### Fossilien-Stirnraten (VOLLSTÄNDIG LOKALISIERT)
- **100% Lokalisierung**: ALLE UI-Texte über LocalizedText Assets
- **Keine hardcodierten Strings mehr**: Explanation-Screen, Input-Modi, Anweisungen
- **Lokalisierte Fossilien**: Fossil-Namen in gewählter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gewählte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- **Dynamische Input-Mode-Texte**: Zeigt korrekte Steuerung basierend auf Platform
- Difficulty-basierte Fossil-Sets pro Team
- Adaptive Rundendauer basierend auf Schwierigkeitsgrad
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling für übersprungene Begriffe
- Touch-Fallback für Testing
- Team-Image System statt Text-Labels

#### Tabu-Minispiel (VOLLSTÄNDIG FERTIGGESTELLT)
- **100% Code-Implementierung**: TabuGameManager, TabuTerm, TabuCollection vollständig implementiert
- **100% Lokalisierung**: ALLE 12 LocalizedText Assets erstellt und zugewiesen
- **100% Content**: TabuTerm ScriptableObjects für alle Schwierigkeitsgrade erstellt
- **100% UI-Setup**: TabuGame Scene erstellt, UI-Hierarchie aufgebaut, Referenzen zugewiesen
- **Button-basierte Steuerung**: Richtig/Überspringen Buttons mit Haptic Feedback
- **Difficulty-basierte Begriff-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad
- **Adaptive Rundendauer**: Schwierigkeitsgrad-basierte Zeitmultiplikatoren
- **Team-Exklusion**: Team 2 bekommt andere Begriffe (wenn möglich)
- **Begriff-Recycling**: Übersprungene Begriffe kommen zurück in Queue
- **Timer-System**: Audio-Countdown bei letzten 3 Sekunden
- **Dynamische Tabu-Wörter**: Variable Anzahl pro Begriff mit UI-Pool-System
- **Gewinner-Icon**: Gewinner-Team-Image auf Results Screen
- **Live Language-Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimiert**: Touch-Targets, Haptic Feedback, Portrait-Vollbild
- **SPIELBAR**: Vollständig getestet und spielbereit

#### Shared Systems (MOBILE-OPTIMIERT + LOKALISIERT)
- **Sprach-persistente Speicherung**: Gewählte Sprache bleibt gespeichert
- **Cross-System-Synchronisation**: GameDataManager ? LanguageSystem
- Plattformunabhängiges Save-System für Spielerfortschritt, Team-Settings und Sprache
- Modular structure für verschiedene Räume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Neues Minispiel (HÖCHSTE PRIORITÄT)
- Warte auf Design-Beschreibung für nächstes Minispiel
- Analog zu bestehenden Systemen (Difficulty + Language Support)
- Mobile-optimierte Implementierung

#### Weitere Entwicklung
- Integration in größeres Führungssystem
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
?   ?   ??? TeamSettingsManager.cs
?   ?   ??? DifficultyRadioGroup.cs
?   ?   ??? SceneNavigator.cs
?   ?   ??? LocalizedTextComponent.cs
?   ?   ??? LanguageRadioGroup.cs
?   ?   ??? LanguageSettingsManager.cs
?   ?   ??? FullscreenManager.cs (DEPRECATED für Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED für Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilGameManager.cs (VOLLSTÄNDIG LOKALISIERT)
?   ?   ??? TabuGameManager.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizQuestion.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilCollection.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? TabuTerm.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? TabuCollection.cs (VOLLSTÄNDIG FERTIGGESTELLT)
?   ?   ??? DifficultySystem.cs
?   ?   ??? LanguageSystem.cs
?   ?   ??? LocalizedText.cs
?   ?   ??? Notes/
?       ??? Notes.md
?   ??? NFC/
?   ?   ??? NFCManager.cs (NEU - 29.10.2025)
?   ??? Game/
?   ?   ??? NFCSceneLoader.cs (NEU - 29.10.2025)
?   ??? Data/
?   ?   ??? NFCRoomMapping.cs (NEU - 29.10.2025)
??? Plugins/
    ??? Android/
    ?   ??? NFCHelper.java (NEU - 29.10.2025)
    ?   ??? AndroidManifest.xml (AKTUALISIERT - NFC Support)
    ??? iOS/
        ??? NFCPlugin.mm (NEU - 29.10.2025)
        ??? UnityApp.entitlements (NEU - NFC Entitlements)

### ?? DEVELOPMENT NOTES

#### Mehrsprachigkeitssystem (IMPLEMENTIERT + POLISHED)
- **Architektur**: Custom ScriptableObject-System statt Unity Localization Package
- **Performance**: Sprach-Caching und Event-basierte Updates minimieren Overhead
- **Fallback-Hierarchie**: 
  1. Leichte Sprache ? Standard-Variante
  2. Englisch ? Deutsch
  3. Alle ? Deutsch Standard
- **Mobile-First**: Touch-optimierte Settings-UI mit Haptic Feedback
- **UI-Vereinfachung**: LanguageSettingsManager stark vereinfacht (Apply Button entfernt)
- **Sofortige Sprachwechsel**: Radio-Button-Click triggert direkt GameDataManager.SetLanguage()
- **Extensibility**: Neue Sprachen durch Language-Enum-Erweiterung
- **.NET Framework 4.7.1 Kompatibilität**: System.Enum.GetValues(typeof()) statt generischer Variante

#### Mobile-First Approach (IMPLEMENTIERT)
- **Plattform**: iOS/Android primär, WebGL-Features entfernt
- **UI**: Safe Area Support, Touch-Target-Optimierung
- **Input**: Accelerometer + Touch-Fallbacks, Haptic Feedback
- **Performance**: Mobile-optimierte Rendering und Memory-Management

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- **Modularity**: Jeder Schwierigkeitsgrad hat eigene Content-Sets
- **Persistence**: Team-Settings bleiben über gesamte Führung erhalten
- **Flexibility**: Zeit-Multiplikatoren und Content-Anpassungen pro Level
- **Extensibility**: Einfache Erweiterung um weitere Schwierigkeitsgrade

#### Content Management (ERWEITERT)
- **ScriptableObjects**: Für einfache Content-Erstellung mit Difficulty + Language-Support
- **Validation**: Automatische Prüfung ob Content für alle Schwierigkeitsgrade und Sprachen vorhanden
- **Skalierbarkeit**: System designed für 6 verschiedene Räume + mehrere Minispiele
- **Lokalisierung**: Vollständig implementiert mit Fallback-System
- **Legacy-Support**: Bestehende Inhalte ohne Lokalisierung funktionieren weiterhin
- **Keine Hardcoding**: Alle Game-Manager vollständig auf LocalizedText Assets umgestellt

#### Technical Architecture (ERWEITERT)
- **Localization System**: Event-basiert mit automatischer UI-Aktualisierung
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabhängig mit Backup-Funktionalität, Team-Settings und Sprache
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilität
- **Enum-Namespace**: DifficultyLevel global, LanguageSystem.Language in Klasse (WICHTIG für Kompatibilität!)

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### MEHRSPRACHIGKEITSSYSTEM DETAILS (IMPLEMENTIERT + POLISHED)

#### Fallback-Hierarchie
1. **Erste Stufe**: Leichte Sprache ? Standard-Variante (German_Simple ? German_Standard)
2. **Zweite Stufe**: Ziel-Sprache ? Fallback-Sprache (English ? German)
3. **Dritte Stufe**: Alle ? German_Standard (final fallback)

#### Performance-Optimierungen
- **Language Caching**: Aktuelle Sprache wird gecacht für bessere Performance
- **Event-basierte Updates**: OnLanguageChanged Event minimiert unnötige Updates
- **Lazy Loading**: Texte werden nur bei Bedarf geladen

#### Content-Validation
- **HasAllLocalizations()**: Prüft Vollständigkeit aller Übersetzungen
- **Context-Menu Validation**: Editor-Tools für Content-Creators
- **Debug-Logging**: Optional aktivierbare Fallback-Logs

#### Legacy-Support
- **Rückwärtskompatibilität**: Bestehende Inhalte funktionieren ohne Änderungen
- **Obsolete-Markierungen**: Sanfte Migration zu neuen lokalisierten Methoden
- **Fallback-Texte**: Hardcodierte Fallbacks für alle Sprachen als Backup

#### LanguageSettingsManager Vereinfachung (NEU)
- **Entfernt**: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- **Beibehalten**: LanguageRadioGroup, Back Button, Safe Area Support
- **Verbessert**: Sofortige Sprachwechsel ohne Apply-Step
- **Reduziert**: Von ~250 auf ~140 Zeilen Code

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (IMPLEMENTIERT)
- **Team-Settings**: Persistent über GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty() / GetTermsForDifficulty()
- **UI-System**: Image-Swapping Radio-Buttons für bessere Mobile-UX
- **Zeit-System**: Multiplikatoren für angepasste Rundendauer
- **Fallback**: Legacy-Support für bestehende Content-ScriptableObjects

### MOBILE-OPTIMIERUNG DETAILS (IMPLEMENTIERT)
- **Safe Area**: Automatische Anpassung an iOS Notch und Android Cutouts
- **Touch-Targets**: Mindestgröße 60 Unity Units für optimale Bedienbarkeit
- **Haptic Feedback**: Vibration bei allen wichtigen Interaktionen
- **Platform-Detection**: Automatische Anpassung iOS vs Android Features

### SPLIT-SCREEN QUIZ DETAILS (ERWEITERT + LOKALISIERT)
- **Multi-Language**: Teams erhalten Fragen in gewählter Sprache
- **Live Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Lokalisierte Feedback**: Alle Punkte-Nachrichten und Gewinner-Texte
- **Multi-Difficulty**: Teams können unterschiedliche Fragen basierend auf Schwierigkeitsgrad erhalten
- **Anti-Schummel**: Shuffled answers per QuizQuestion.GetShuffledAnswers(Language)
- **Timing-System**: Time.time basierte Punkteverteilung
- **UI-Management**: Disabled-Color System für saubere Button-States
- **Continue-System**: Touch-anywhere nach Feedback-Phase

### FOSSILIEN-STIRNRATEN DETAILS (VOLLSTÄNDIG LOKALISIERT)
- **100% LocalizedText Assets**: ALLE UI-Texte über ScriptableObjects
- **Keine hardcodierten Strings mehr**: Explanation-Screen, Input-Modi, Anweisungen
- **Lokalisierte Fossilien**: Fossil-Namen in gewählter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gewählte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- **Dynamische Input-Mode-Texte**: GetLocalizedInputModeInfo() prüft IsUsingAccelerometer()
- **CRITICAL FIX (19.10.2025)**: Event-Reihenfolge in TriggerCorrect/TriggerSkip geändert
  - Haptic Feedback ? DisableInputTemporarily() ? Event-Invoke
  - Verhindert ungewollte Vibrationen auf Explanation Screen
  - CancelInvoke() in SetInputEnabled() funktioniert jetzt korrekt
- **Adaptive Zeiten**: GetAdjustedRoundDuration() basierend auf Team-Schwierigkeitsgrad
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### INVOKE-CALLBACK & EVENT-REIHENFOLGE BEST PRACTICES (19.10.2025)

#### Problem-Pattern (VERMEIDEN)

// FALSCH: Event ZUERST
public void TriggerAction()
{
    OnAction?.Invoke();              // Kann SetInputEnabled(false) aufrufen
    TriggerHapticFeedback();
    DisableInputTemporarily(0.5f);   // Invoke wird NACH SetInputEnabled registriert
}

#### Lösung-Pattern (VERWENDEN)

// RICHTIG: Event als LETZTES
public void TriggerAction()
{
    TriggerHapticFeedback();         // Sofortige Aktionen zuerst
    DisableInputTemporarily(0.5f);   // Invoke registrieren
    OnAction?.Invoke();              // Event triggert SetInputEnabled ? CancelInvoke löscht Invoke
}

#### Regeln
1. **Invoke-Callbacks vor Events**: Alle Invoke() Calls VOR Event-Invocations
2. **CancelInvoke() nach Invoke()**: CancelInvoke() kann nur NACH Invoke() greifen
3. **Sofortige Aktionen zuerst**: Haptic Feedback, Audio, UI-Updates vor Callbacks
4. **Events als Letztes**: Events können State ändern ? sollten letzte Aktion sein
5. **Race-Conditions beachten**: Asynchrone Callbacks können State-Management brechen

#### Team-Start-Reihenfolge Fix (NEU - 19.11.2025)
- **team2StartsFirst Checkbox**: Optional Team 2 statt Team 1 starten lassen
- **EndRound() Fix**: Neue Logik mit startingTeam/secondTeam Pattern
- **Problem gelöst**: Bei team2StartsFirst endete Spiel nach 1. Runde
- **Lösung**: Prüft ob startingTeam oder secondTeam gespielt hat
- **1 Runde pro Team**: Beide Teams spielen immer, unabhängig von Start-Reihenfolge


### NFC-SYSTEM (VOLLSTÄNDIG IMPLEMENTIERT - 29.10.2025)

**NFCManager.cs**
- Zentrale NFC-Verwaltung für iOS und Android
- Platform-Detection mit Compile-Directives
- Automatische NFC-Aktivierung bei App-Start
- Polling-System für Android NFC-Intents (0.5s Intervall)
- Integration mit NFCSceneLoader für automatisches Szenen-Laden
- OnApplicationPause-Handling für NFC-Reaktivierung
- UI-Feedback über TextMeshPro (DebugText, StatusText)
- Editor-Test-Support mit Mock-Tags

**NFCHelper.java (Android)**
- Native Android NFC-Integration
- Foreground Dispatch System für Tag-Erkennung
- KRITISCH: Liest NDEF-Daten aus Intent (NICHT direkt vom Tag!)
  - METHODE 1: intent.getParcelableArrayExtra(NfcAdapter.EXTRA_NDEF_MESSAGES) - Bevorzugt!
  - METHODE 2: Direktes Tag-Lesen als Fallback (kann "out of date" Fehler werfen)
  - METHODE 3: Tag-ID als Hex-String (finaler Fallback)
- Löst "Tag is out of date" Problem durch Intent-basiertes Lesen
- Unterstützt NDEF Text Records mit Language-Code-Parsing
- Automatisches Fallback zu Tag-ID wenn NDEF nicht verfügbar
- Detailliertes Android-Logging für Debugging

**NFCPlugin.mm (iOS)**
- Core NFC Framework Integration für iOS 13+
- NFCTagReaderSession mit TAG Entitlement (NICHT NDEF!)
- WICHTIG: Nutzt com.apple.developer.nfc.readersession.formats mit "TAG"
- NDEF Entitlement ist DEPRECATED seit iOS 13
- Objective-C Bridge für Unity-Kommunikation
- UnitySendMessage für Tag-Daten und Fehler

**NFCRoomMapping.cs (ScriptableObject)**
- Mapping zwischen NFC-Tag-IDs und Szenen-Namen
- Unterstützt sowohl NDEF-Text ("ROOM_01") als auch Tag-IDs ("TAG_ID:04A3B2C1")
- Exakte und Partial-Match-Unterstützung
- GetSceneNameForTag() mit Fallback-Logik
- GetDisplayNameForTag() für UI-Anzeige
- Validation-Tools für Content-Prüfung
- CreateAssetMenu Integration für einfache Erstellung

**NFCSceneLoader.cs**
- Scene-Loading-System basierend auf NFC-Tags
- Async Scene Loading mit Progress-Tracking
- Konfigurierbare Delay vor Szenen-Wechsel
- Hub-Scene-Pattern für Rückkehr-Navigation
- Integration mit GameDataManager für Room-Tracking
- IsValidRoomTag() für Tag-Validierung
- ReturnToHub() Methode für Navigation zurück

**AndroidManifest.xml Konfiguration**
- NFC Permission und Feature deklariert
- Intent-Filter für TAG_DISCOVERED und NDEF_DISCOVERED
- launchMode="singleTop" für korrekte Intent-Verarbeitung
- Verhindert Activity-Duplikation bei Tag-Scans

**iOS Entitlements & Info.plist**
- com.apple.developer.nfc.readersession.formats mit "TAG" Array
- NFCReaderUsageDescription für App Store Compliance
- WICHTIG: Nicht "NDEF" verwenden - ist deprecated!

**Testing & Validation**
- Editor-Support mit Mock-Tags ("TEST_ROOM_01")
- Android-Testing: Xiaomi Device mit NFC Tools beschriebenen Tags
- Erfolgreiche Tag-Erkennung und Szenen-Wechsel
- Hub-Pattern funktioniert wie designed

**Nächste Schritte**
- Raum-Szenen erstellen und mit Tags verknüpfen
- Content für NFCRoomMapping ScriptableObject erstellen
- ReturnToHub-Buttons in Raum-Szenen implementieren
- iOS-Testing sobald Device verfügbar

# PROJEKT-UPDATE: TEAM-ICON-AUSWAHL-SYSTEM (05.11.2025)

## NEUE FEATURES IMPLEMENTIERT

### TEAM-ICON-AUSWAHL-SYSTEM (VOLLSTÄNDIG IMPLEMENTIERT)
- **Zweck**: Jedes Team kann vor dem Spiel aus 3 verschiedenen Icons wählen
- **Persistenz**: Auswahl wird über GameDataManager gespeichert
- **UI-System**: Radio-Button-Pattern mit Sprite-Swapping für Selected/Unselected States
- **Integration**: Alle 4 Minispiele verwenden dynamisch ausgewählte Icons

#### Neue Dateien erstellt:
1. **TeamIconProvider.cs (ScriptableObject)** - Zentrales Icon-Management
   - TeamIconSet für je 3 Icons pro Team (icon0, icon1, icon2)
   - GetTeam1Icon() / GetTeam2Icon() / GetTeamIcon(teamIndex)
   - Liest Icon-Index aus GameDataManager
   - Validation-Tools für Content-Prüfung
   
2. **TeamIconRadioGroup.cs (UI Component)** - Icon-Auswahl-UI
   - Radio-Button-System analog zu DifficultyRadioGroup
   - IconButton Struktur (Button, IconImage, normalSprite, selectedSprite)
   - Sprite-Swapping für visuelle Hervorhebung der Auswahl
   - Event-System: OnIconChanged(int iconIndex)
   - Haptic Feedback Integration
   - Mobile-optimierte Touch-Targets

#### Erweiterte Dateien:

**DifficultySystem.cs**
- TeamSettings erweitert mit team1IconIndex und team2IconIndex (0-2)
- GetTeamIconIndex(teamIndex) und SetTeamIconIndex(teamIndex, iconIndex)
- Clamp-System für sichere Index-Verwaltung (0-2)

**GameDataManager.cs**
- Icon-Management-Methoden hinzugefügt:
  - SetTeamIconIndex(teamIndex, iconIndex)
  - GetTeamIconIndex(teamIndex)
  - SetTeamIconIndices(team1IconIndex, team2IconIndex)
- Properties: Team1IconIndex, Team2IconIndex
- Icon-Indizes werden validiert und geclampt bei LoadGameData()
- Debug-Methoden für Icon-Testing
- Rückwärtskompatibilität: Alte Saves funktionieren weiterhin (Default: Icon 0)

**TeamSettingsManager.cs**
- UI erweitert mit team1IconGroup und team2IconGroup
- Icon-Change-Handler: OnTeam1IconChanged(), OnTeam2IconChanged()
- ApplySettings() speichert Icon-Indizes zusätzlich zu Difficulty
- InitializeSettings() lädt gespeicherte Icon-Indizes
- SetupUI() bindet Icon-Group-Events

**SplitScreenQuizManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf QuizRoomData.team1/2Image
- Icons werden aktualisiert:
  - Bei Start() nach InitializeQuiz()
  - Bei ShowResults() für Results Screen
  - Bei RestartQuiz() für Icon-Refresh
- player1TeamIcon und player2TeamIcon UI-Referenzen hinzugefügt
- result1TeamIcon und result2TeamIcon für Results Screen
- Legacy-Fallback: Funktioniert ohne TeamIconProvider (nutzt alte team1/2Image)
- DEPRECATED-Warnung für alte QuizRoomData team images

**FossilGameManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf FossilCollection.team1/2Image
  - UpdateCurrentTeamImage(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei InitializeGame() nach SetupUI()
  - Bei ShowResults() für Results Screen
  - Bei RestartGame() für Icon-Refresh
- Explanation, Countdown, Gameplay, Results Screens verwenden dynamische Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- DEPRECATED-Warnung für alte FossilCollection team images

**TabuGameManager.cs** (TeamIconProvider Integration)
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamIndex) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf TabuCollection.Team1/2Image
  - UpdateCurrentTeamImage(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei InitializeGame() nach SetupUI()
  - Bei ShowResults() für Results Screen
  - Bei RestartGame() für Icon-Refresh
- Explanation, Gameplay, Results Screens verwenden dynamische Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- DEPRECATED-Warnung für alte TabuCollection team images

**PuzzleGameManager.cs** (TeamIconProvider Integration) - NEU ENTDECKT UND ANGEPASST
- Header-Feld: [SerializeField] private TeamIconProvider teamIconProvider
- Neue Methoden:
  - UpdateTeamIcons() - Zentrale Icon-Update-Logik
  - UpdateTeamIconForImage(Image, teamNumber) - Helper für einzelne Icons
  - SetupTeamIconsLegacy() - Fallback auf PuzzleCollection.team1/2Image
  - UpdateCurrentTeamImageLegacy(Image) - DEPRECATED Legacy-Methode
- Icons werden aktualisiert:
  - Bei Start() nach InitializeGame()
  - Bei ShowExplanationScreen() für Explanation Icon
  - Bei ShowCountdownScreen() für Countdown Icon
  - Bei ShowGameplayScreen() für Gameplay Icon
  - Bei ShowResultsScreen() für Results Screen Icons
- Winner-Icon auf Results Screen nutzt TeamIconProvider
- Team-Difficulty-Index-Fix: GetTeamDifficulty(0/1) statt (1/2) für Konsistenz
- DEPRECATED-Warnung für alte PuzzleCollection team images

### TECHNISCHE DETAILS - TEAM-ICON-SYSTEM

#### Icon-Index-System
- **3 Icons pro Team**: Index 0, 1, 2 (insgesamt 6 Icons)
- **Default**: Icon 0 für beide Teams bei neuen Saves
- **Persistenz**: Gespeichert in GameProgressData.teamSettings
- **Zugriff**: GameDataManager.GetTeamIconIndex(0/1) - 0-basiert!

#### TeamIconProvider-Architektur
- **ScriptableObject-basiert**: Zentrale Asset-Verwaltung
- **TeamIconSet-Klasse**: Kapselt 3 Icons + GetIcon(index) Methode
- **GetTeamIcon(teamIndex)**: Kombiniert Team-Auswahl + Icon-Index aus GameDataManager
- **Validation**: ValidateIcons() prüft Vollständigkeit, HasAllIcons() für einzelne Sets
- **Fallback-System**: Legacy team1/2Image aus Collections als Backup

#### UI-Pattern: TeamIconRadioGroup
- **Analog zu DifficultyRadioGroup**: Konsistentes UI-System
- **IconButton-Struktur**: 
  - Button (Unity UI Button Component)
  - IconImage (Image Component für Sprite-Anzeige)
  - normalSprite (nicht ausgewählt - z.B. Graustufen-Icon)
  - selectedSprite (ausgewählt - z.B. Farbiges Icon mit Rahmen)
- **Sprite-Swapping statt Farbwechsel**: Bessere Kontrolle über visuelle Hervorhebung
- **Mobile-optimiert**: Touch-Target-Größen, Haptic Feedback
- **Event-System**: OnIconChanged(int iconIndex) für externe Handler

#### Game-Manager-Integration-Pattern
ALLE 4 Game-Manager verwenden identisches Pattern:

1. **Header-Feld**: [SerializeField] private TeamIconProvider teamIconProvider;
2. **UpdateTeamIcons()**: Zentrale Methode, aufgerufen bei:
   - InitializeGame() / Start()
   - ShowResults() (für Results Screen Refresh)
   - RestartGame() (für Icon-Refresh nach Änderungen)
3. **UpdateTeamIconForImage(Image, teamIndex)**: Helper für einzelne Icon-Updates
4. **SetupTeamIconsLegacy()**: Fallback wenn teamIconProvider == null
5. **Legacy-Warnung**: Debug.LogWarning bei Verwendung alter Collection-Images

#### Konsistenz-Fixes
- **Team-Difficulty-Index**: Jetzt einheitlich 0-basiert (0 = Team 1, 1 = Team 2)
  - SplitScreenQuizManager: team1Difficulty = GetTeamDifficulty(0)
  - FossilGameManager: team1Difficulty = GetTeamDifficulty(0)
  - TabuGameManager: team1Difficulty = GetTeamDifficulty(0)
  - PuzzleGameManager: team1Difficulty = GetTeamDifficulty(0) - FIXED!
- **teamIndex vs teamNumber**: 
  - teamIndex ist 0-basiert (für Arrays/Provider)
  - teamNumber ist 1-basiert (für UI-Anzeige)
  - Konvertierung: teamIconProvider.GetTeamIcon(teamNumber - 1)

### RÜCKWÄRTSKOMPATIBILITÄT

**Alte Saves funktionieren weiterhin:**
- Fehlende icon-Indizes werden mit 0 initialisiert (Default-Icon)
- GameDataManager migriert Saves automatisch
- Legacy team1Image/team2Image in Collections werden als Fallback genutzt
- Keine Breaking-Changes für bestehende Inhalte

**Migration-Path:**
1. Bestehende Saves laden automatisch mit Icon-Index 0 für beide Teams
2. TeamIconProvider-Asset zuweisen (optional, Legacy-Fallback funktioniert)
3. Icon-Sprites in TeamIconProvider zuweisen
4. Teams können Icons in TeamSettings auswählen
5. Alte Collection-Images können entfernt werden (aber nicht notwendig)

### CONTENT-REQUIREMENTS

**Für vollständige Nutzung benötigt:**
1. **TeamIconProvider.asset** erstellen (Create ? Museum Quiz ? Team Icon Provider)
2. **6 Icon-Sprites** zuweisen:
   - Team 1: icon0, icon1, icon2
   - Team 2: icon0, icon1, icon2
3. **12 Selected-Sprites** erstellen (für Hervorhebung):
   - Gleiche Icons mit visueller Hervorhebung (Rahmen, Farbe, Glow, etc.)
4. **TeamIconRadioGroup UI-Setup** in TeamSettings-Scene:
   - 2x TeamIconRadioGroup Components (Team 1 + Team 2)
   - Je 3 IconButton-Strukturen konfigurieren
5. **Game-Manager UI-Referenzen** in allen 4 Game-Szenen:
   - TeamIconProvider Asset zuweisen
   - Bestehende Team-Icon-Image-Referenzen bleiben (für Legacy-Fallback)

## GAME DESIGN - BILDERRÄTSEL/PUZZLE-GAME (VOLLSTÄNDIG IMPLEMENTIERT)

### KERNMECHANIK
- **Gameplay**: Find-the-Exhibit Style - Spieler finden Exponate im Raum anhand von Bildausschnitten
- **Teams**: 2 Teams spielen abwechselnd mit individuellen Schwierigkeitsgraden
- **Rundensystem**: Konfigurierbare Anzahl Runden pro Team (roundsPerTeam)
- **Hinweis-System**: 2 verfügbare Hinweise (größere Bildausschnitte) mit Punktabzug
- **Punktesystem**: Startpunkte - (Hinweise × hintPenalty)
- **Timer**: Optional, schwierigkeitsgrad-abhängig

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
- **Kids**: Einfachere Puzzles, optionaler Timer (1.5x länger)
- **BigKids**: Mittlere Komplexität, Timer möglich (1.2x länger)
- **Adults**: Volle Komplexität, Timer standardmäßig aktiv (1.0x)
- **Content-Sets**: Separate PuzzlePiece-Arrays pro Schwierigkeitsgrad

### SPIELABLAUF
1. **Explanation Screen**: Regeln, Team-Icon, Schwierigkeitsgrad-Info, Spieleinstellungen
2. **Countdown**: 3-2-1 mit unterschiedlichen Sounds
3. **Gameplay Phase**:
   - Bildausschnitt (hint0) wird angezeigt
   - Mögliche Punkte werden angezeigt
   - Buttons: "Gefunden!", "Hinweis anzeigen", "Aufgeben"
   - Hinweis-System: hint0 ? hint1 ? hint2 (3 Bilder total)
   - Jeder Hinweis kostet Punkte (hintPenalty)
   - Optional: Timer mit Audio-Warnung bei letzten 3 Sekunden
4. **Solution Screen**: Vollständiges Bild, Exponat-Name, Beschreibung, verdiente Punkte
5. **Team-Wechsel**: Nach Team 1 spielt Team 2
6. **Results Screen**: Gewinner-Ermittlung, Scores, Team-Icons

### FEATURES
- **Hinweis-System**: 2 Hinweise verfügbar (größere Bildausschnitte)
  - hint0: Kleinster Ausschnitt (Start)
  - hint1: Mittlerer Ausschnitt (1. Hinweis)
  - hint2: Großer Ausschnitt (2. Hinweis)
  - Jeder Hinweis kostet konfigurierbare Punkte (hintPenalty)
- **Punktesystem**: startingPoints - (hintsUsed × hintPenalty)
- **Aufgeben-Option**: Popup mit Bestätigung, 0 Punkte für Runde
- **Timer-System**: 
  - Optional pro Schwierigkeitsgrad aktivierbar
  - Time-Up Popup bei Ablauf
  - Audio-Warnung bei letzten 3 Sekunden
- **Team-Exklusion**: Team 2 bekommt (wenn möglich) andere Puzzles als Team 1
- **Lokalisierung**: Vollständig lokalisiert (Exponat-Namen, Beschreibungen, UI)
- **Mobile-optimiert**: Haptic Feedback, Portrait-Vollbild, Touch-optimiert
- **Team-Icon-System**: Dynamische Icons wie bei anderen Minispielen

### PUNKTESYSTEM
- **Startpunkte**: Konfigurierbar in PuzzleCollection (Standard: 3)
- **Hinweis-Kosten**: Konfigurierbar in PuzzleCollection (Standard: 1)
- **Gefunden ohne Hinweis**: Volle Punktzahl (z.B. 3 Punkte)
- **Gefunden mit 1 Hinweis**: Startpunkte - 1 (z.B. 2 Punkte)
- **Gefunden mit 2 Hinweisen**: Startpunkte - 2 (z.B. 1 Punkt)
- **Aufgegeben oder Zeit abgelaufen**: 0 Punkte

### HAUPTKLASSEN - PUZZLE-GAME (VOLLSTÄNDIG IMPLEMENTIERT)

**PuzzleGameManager.cs**
- Vollständige Implementierung des Puzzle-Minispiels
- Fünf Screens: Explanation, Countdown, Gameplay, Solution, Results
- Vollständig lokalisiert (21 LocalizedText Assets)
- Hinweis-System mit Punktabzug
- Timer-System optional mit Audio-Warnung
- Give-Up und Time-Up Popups mit Bestätigung
- Team-Exklusions-System (Team 2 bekommt andere Puzzles)
- Schwierigkeitsgrad-basierte Runden-Timer
- OnLanguageChanged Event-Handler für Live-Updates
- GetLocalizedText() Helper-Methode mit Fallback-System
- Haptic Feedback bei Button-Klicks
- Integration mit GameDataManager für Team-Schwierigkeitsgrade
- **NEU**: TeamIconProvider Integration für dynamische Team-Icons
- Team-Difficulty-Index-Fix: Jetzt 0-basiert (0 = Team 1, 1 = Team 2)

**PuzzlePiece.cs (ScriptableObject)**
- Datenstruktur für einzelne Puzzle-Exponate
- Vollständig lokalisiert (4 Sprachen)
- Drei Hint-Images (hint0, hint1, hint2) für progressives Enthüllen
- Solution-Image (vollständiges Exponat-Foto)
- Exponat-Name in allen 4 Sprachen
- Optional: Exponat-Beschreibung in allen 4 Sprachen
- GetExhibitName(Language) Methode mit Fallback-Hierarchie
- GetExhibitDescription(Language) Methode mit Fallback-Hierarchie
- GetHintImage(hintLevel) Methode (0-2)
- HasAllLocalizations() für Content-Validation
- ValidateLocalizations() Context-Menu für Editor
- CreateAssetMenu Integration

**PuzzleCollection.cs (ScriptableObject)**
- Container für Puzzle-Stücke mit Schwierigkeitsgrad-Sets
- Analog zu FossilCollection und TabuCollection strukturiert
- Lokalisierter Collection-Name über LocalizedText
- Separate PuzzlePiece-Arrays für Kids/BigKids/Adults
- Team-Images für visuelle Darstellung (DEPRECATED - TeamIconProvider nutzen)
- Konfigurierbare Spieleinstellungen:
  - roundsPerTeam (Anzahl Runden pro Team)
  - startingPoints (Punkte bei Start)
  - hintPenalty (Punktabzug pro Hinweis)
  - enableTimerKids, enableTimerBigKids, enableTimerAdults
  - roundDuration (Basis-Rundendauer)
- DifficultyTimeSettings Integration
- GetPuzzlesForDifficulty(DifficultyLevel) Methode
- GetRandomPuzzles(count, difficulty) Methode
- GetRandomPuzzlesExcluding(count, difficulty, excludeList) für Team 2
- GetAdjustedRoundDuration(difficulty) für angepasste Zeiten
- UseTimerForDifficulty(difficulty) für Timer-Aktivierung
- GetTeamImage(teamNumber) - DEPRECATED, nutzt Legacy team1/2Image
- HasPuzzlesForAllDifficulties() Validation
- HasAllLocalizations() für Content-Validation
- Context-Menu Validierungs-Tools
- CreateAssetMenu Integration

### TEAM-START-REIHENFOLGE (NEU - 19.11.2025)

**Problem gelöst**: Bei Spielen mit Team-Abwechslung (Puzzle, Fossil, Tabu) hat immer Team 1 angefangen

**Implementierung - team2StartsFirst Checkbox**:
- Neue Inspector-Checkbox in allen 3 Game-Managern (Puzzle, Fossil, Tabu)
- [SerializeField] private bool team2StartsFirst = false;
- Tooltip: "Wenn aktiviert, beginnt Team 2 statt Team 1 das Spiel"
- Platzierung: Direkt unter Collection-Feld im "Game Flow Settings" Header

**Technische Details**:
- InitializeGame(): currentTeam = team2StartsFirst ? 2 : 1;
- RestartGame(): Gleiche Logik für Consistency
- EndRound() Fix bei Tabu/Fossil:
  - Problem: Bei team2StartsFirst endete Spiel nach 1. Runde (Team 1 kam nie dran)
  - Lösung: Neue Logik mit startingTeam / secondTeam Pattern
  - Beide Teams spielen jetzt immer, unabhängig von Start-Reihenfolge

**EndRound() Pattern (Tabu & Fossil)**:

int startingTeam = team2StartsFirst ? 2 : 1;
int secondTeam = team2StartsFirst ? 1 : 2;

if (currentTeam == startingTeam)
{
    currentTeam = secondTeam;  // Nach Startteam ? Zum 2. Team
    ShowExplanation();
}
else
{
    ShowResults();  // 2. Team fertig ? Spiel Ende
}


**Anwendungsfälle**:
- Fairness: Teams wechseln sich ab wer anfängt
- Pro Collection konfigurierbar: Jede PuzzleCollection / FossilCollection / TabuCollection kann eigene Start-Reihenfolge haben
- Manual Toggle: Designer kann Checkbox per Hand vor jedem Spiel ändern
- Restart-Safe: Funktioniert auch beim "Nochmal spielen"

**HINWEIS - Automatisches Toggle NICHT implementiert**:
- Optional wäre gewesen: Automatisches Wechseln zwischen Runs via PlayerPrefs
- Bewusst NICHT implementiert: Manuelle Kontrolle bevorzugt
- Falls später gewünscht: PlayerPrefs.SetInt("LastStartingTeam", currentTeam) speichern und beim Start togglen

## AKTUALISIERTE PROJEKTSTRUKTUR

Assets/_GAME/
??? Scripts/
?   ??? Game/
?   ?   ??? TourResultsManager.cs (NEU - 06.11.2025)
?   ?   ??? SplitScreenQuizManager.cs
?   ?   ??? FossilGameManager.cs
?   ?   ??? TabuGameManager.cs
?   ?   ??? PuzzleGameManager.cs
?   ??? Data/
?       ??? GameDataManager.cs (ERWEITERT - Score-Abfrage)

### NÄCHSTE SCHRITTE FÜR TOUR-RESULTS

1. **UI-Design finalisieren**:
   - Layout für Winner/Loser UI gestalten
   - Layout für Tie UI gestalten
   - Icons und Sprites erstellen

2. **LocalizedText Assets erstellen**:
   - 4 Assets mit allen Sprach-Varianten
   - Testen mit verschiedenen Sprachen

3. **Scene-Integration**:
   - TourResults Scene erstellen
   - Navigation von letztem Raum zum Results-Screen
   - Zurück-Button zu Hauptmenü

4. **Testing durchführen**:
   - Alle Szenarien testen (Team 1 gewinnt, Team 2 gewinnt, Tie)
   - Sprachwechsel validieren
   - Icon-Anzeige für verschiedene Team-Auswahlen testen
   - Simulationsmodus validieren

## AKTUELLER PROJEKTSTATUS (UPDATE 06.11.2025)

### ? VOLLSTÄNDIG IMPLEMENTIERT UND FUNKTIONSFÄHIG

#### Tour-Results-Screen (NEU - 06.11.2025)
- **Finaler Abschluss-Screen**: Zeigt Gesamtergebnis nach allen Räumen
- **Dual-UI-System**: Separate Layouts für Gewinner/Verlierer und Tie
- **Score-Berechnung**: Automatisch über alle RoomResults aus GameDataManager
- **Team-Icon-Integration**: Konsistent mit allen 4 Minispielen
- **Debug-System**: Umfangreiches Testing-System mit Simulation
- **Vollständig lokalisiert**: 4 LocalizedText Assets für verschiedene Szenarien
- **Mobile-optimiert**: Portrait-Vollbild, Touch-optimiert

#### Team-Namen-System (NEU - 19.11.2025)
- **Lokalisierte Team-Namen**: Jedes Icon hat festen Namen (z.B. "Team Urpferd" / "Team Ancient Horse")
- **TeamIconProvider erweitert**: icon0Name, icon1Name, icon2Name pro Team (LocalizedText)
- **Optional anzeigbar**: UI zeigt Namen nur wenn TextMeshProUGUI-Feld vorhanden
- **Alle Screens**: Explanation, Countdown, Gameplay, Results (wo Platz vorhanden)
- **TeamSettingsManager**: Namen werden live aktualisiert bei Icon-Wechsel
- **Fix**: GameDataManager speichert Icon-Index SOFORT beim Wechsel (nicht erst bei Apply)
- **Integration**: Alle 4 Game-Manager nutzen UpdateTeamNames() Methode
- **Content**: 6 LocalizedText Assets benötigt (3 pro Team)

#### Team-Start-Reihenfolge-System (NEU - 19.11.2025)
- **team2StartsFirst Checkbox**: In Puzzle, Fossil, Tabu Game-Managern
- **Manuelle Kontrolle**: Designer kann pro Collection festlegen wer startet
- **EndRound() Fix**: Tabu & Fossil-Manager mit neuer Logik für korrekten Team-Wechsel
- **Fairness**: Teams können sich abwechseln zwischen verschiedenen Räumen
- **Restart-Safe**: Funktioniert auch beim "Nochmal spielen"

---

**LETZTE AKTUALISIERUNG**: 06. November 2025
**HAUPT-FEATURES DIESES UPDATES**:
- Tour-Results-Screen vollständig implementiert
- GameDataManager um Score-Abfrage-Methoden erweitert
- Dual-UI-System für unterschiedliche Ergebnisse
- Umfangreiches Debug-System für Testing
- Team-Icon-Integration konsistent mit anderen Managern


## MOBILE-SPEZIFISCHE FEATURES (ERWEITERT - 20.11.2025)

### SCREEN SLEEP PREVENTION SYSTEM (NEU - 20.11.2025)

**GlobalScreenSleepManager.cs**
- **Zweck**: Verhindert Sperrbildschirm während der gesamten App-Laufzeit
- **Implementierung**: Globales Singleton-Pattern mit DontDestroyOnLoad
- **Platform-Support**: Funktioniert auf iOS und Android
- **Automatische Aktivierung**: Wird beim App-Start initialisiert

#### Hauptfeatures:
1. **Persistent über Scene-Wechsel**
   - DontDestroyOnLoad() - Manager überlebt alle Scene-Transitions
   - Singleton-Pattern - Nur eine Instanz global
   - Keine Integration in einzelne Szenen nötig

2. **Screen Sleep Management**
   - Screen.sleepTimeout = SleepTimeout.NeverSleep bei Awake()
   - previousSleepTimeout wird gespeichert für Wiederherstellung
   - OnApplicationPause() reaktiviert Sleep Prevention nach App-Resume
   - OnApplicationQuit() / OnDestroy() stellen Original-Setting wieder her

3. **Konfigurierbare Steuerung**
   - Inspector-Checkbox: preventSleepGlobally (Standard: true)
   - Kann zur Laufzeit geändert werden
   - Debug-Logging für alle State-Changes

4. **Öffentliche API**
   - SetSleepPrevention(bool prevent) - Dynamisches An/Ausschalten
   - IsSleepPreventionActive() - Status-Abfrage
   - Statische Methoden für globalen Zugriff

#### Integration:
1. GameObject in erster Scene erstellen (z.B. MainMenu oder Startup)
2. GlobalScreenSleepManager Component hinzufügen
3. Inspector: preventSleepGlobally aktivieren
4. Fertig - funktioniert automatisch in allen Szenen

#### Technische Details:

// Singleton-Pattern mit DontDestroyOnLoad
private void Awake()
{
    if (instance != null && instance != this)
    {
        Destroy(gameObject);
        return;
    }
    
    instance = this;
    DontDestroyOnLoad(gameObject);
    
    if (preventSleepGlobally)
    {
        previousSleepTimeout = Screen.sleepTimeout;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}

// App-Resume-Handling
private void OnApplicationPause(bool pauseStatus)
{
    if (!pauseStatus && preventSleepGlobally)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}


#### Vorteile:
- ? **Einmalige Integration**: Nur 1x GameObject in Startup-Scene
- ? **Zero-Maintenance**: Keine Änderungen in bestehenden Game-Managern
- ? **Persistent**: Funktioniert automatisch über alle Scenes
- ? **Mobile-optimiert**: Verhindert Sperrbildschirm auf iOS & Android
- ? **Dynamisch steuerbar**: Kann zur Laufzeit an/ausgeschaltet werden
- ? **Debug-freundlich**: Console-Logging zeigt alle State-Changes

#### Use-Cases:
- **Standard**: Screen Sleep global deaktiviert für gesamte App
- **Optional**: Temporäres Aktivieren in Menüs möglich (SetSleepPrevention(false))
- **Museum-Kontext**: Spieler können ohne Unterbrechung spielen

#### Unterschied zu lokalem Manager:
- **Lokal** (ScreenSleepManager): Pro Scene hinzufügen, nur für diese Scene aktiv
- **Global** (GlobalScreenSleepManager): 1x hinzufügen, für ALLE Scenes aktiv
- **Projekt**: Global-Variante implementiert für konsistentes Verhalten

---

**LETZTE AKTUALISIERUNG**: 20. November 2025
**HAUPT-FEATURE DIESES UPDATES**:
- Global Screen Sleep Prevention System implementiert
- Verhindert Sperrbildschirm während gesamter App-Laufzeit
- Einmalige Integration, funktioniert automatisch in allen Szenen
- Mobile-optimiert für iOS und Android
