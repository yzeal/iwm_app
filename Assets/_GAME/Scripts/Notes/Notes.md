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

## GAME DESIGN - TABU-MINISPIEL (NEU - IMPLEMENTIERT)

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

### HAUPTKLASSEN - TABU-MINISPIEL (NEU - VOLLSTÄNDIG IMPLEMENTIERT + LOKALISIERT)

**TabuGameManager.cs**
- NEU: Vollständige Implementierung des Tabu-Minispiels
- Analog zu FossilGameManager strukturiert für Konsistenz
- Vollständig lokalisiert (12 LocalizedText Assets benötigt)
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
- NEU: Datenstruktur für einzelne Tabu-Begriffe
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
- NEU: Container für Tabu-Begriffe mit Schwierigkeitsgrad-Sets
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

#### Tabu-Minispiel (NEU - VOLLSTÄNDIG IMPLEMENTIERT + LOKALISIERT)
- **100% Lokalisierung**: ALLE UI-Texte über LocalizedText Assets (12 Assets benötigt)
- **Vollständige Code-Implementierung**: TabuGameManager, TabuTerm, TabuCollection
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
- **ScriptableObject-basiert**: Einfache Content-Erstellung mit Validation-Tools

#### Shared Systems (MOBILE-OPTIMIERT + LOKALISIERT)
- **Sprach-persistente Speicherung**: Gewählte Sprache bleibt gespeichert
- **Cross-System-Synchronisation**: GameDataManager ? LanguageSystem
- Plattformunabhängiges Save-System für Spielerfortschritt, Team-Settings und Sprache
- Modular structure für verschiedene Räume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? NÄCHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Content-Erstellung (HÖCHSTE PRIORITÄT)
- **LocalizedText Assets für Tabu-Minispiel** (12 neue Assets benötigt):
  1. Tabu_ExplanationTitle
  2. Tabu_ExplanationRules
  3. Tabu_StartButton
  4. Tabu_TimerLabel
  5. Tabu_TabuWordsHeader
  6. Tabu_CorrectButton
  7. Tabu_SkipButton
  8. Tabu_ResultsTeam1Label
  9. Tabu_ResultsTeam2Label
  10. Tabu_ResultsWinner
  11. Tabu_ResultsTie
  12. Tabu_BackButton

- **Tabu-Begriff Content erstellen**:
  - TabuTerm ScriptableObjects für alle Schwierigkeitsgrade
  - Mindestens 10-15 Begriffe pro Schwierigkeitsgrad
  - Vollständige Lokalisierung (4 Sprachen)
  - TabuCollection ScriptableObject anlegen

- **UI-Setup für Tabu-Minispiel**:
  - Scene erstellen (analog zu Fossil-Stirnraten)
  - UI-Hierarchie aufbauen (4 Screens)
  - TabuWordText Prefab erstellen (TextMeshProUGUI)
  - Alle Referenzen im TabuGameManager zuweisen

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
?   ?   ??? TabuGameManager.cs (NEU - VOLLSTÄNDIG LOKALISIERT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizQuestion.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilCollection.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? TabuTerm.cs (NEU - VOLLSTÄNDIG LOKALISIERT)
?   ?   ??? TabuCollection.cs (NEU - VOLLSTÄNDIG LOKALISIERT)
?   ?   ??? DifficultySystem.cs
?   ?   ??? LanguageSystem.cs
?   ?   ??? LocalizedText.cs
?   ??? Notes/
?       ??? Notes.md
??? Plugins/
    ??? FullscreenWebGL.jslib (DEPRECATED für Mobile)

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
- **Neue LocalizedText-Variablen** (benötigt Assets-Erstellung):
  - holdPhoneInstructionLocalizedText: "Halte das Handy an deine Stirn" etc.
  - teamExplainsInstructionLocalizedText: "Dein Team erklärt dir das Fossil" etc.
  - tiltModeLocalizedText: "Neige-Modus - Neige das Handy..." etc.
  - touchModeLocalizedText: "Touch-Modus - Tippe links..." etc.
- **Dynamische Input-Mode-Texte**: GetLocalizedInputModeInfo() prüft IsUsingAccelerometer()
- **Entfernte Hardcoding**: GetLocalizedHoldPhoneText() und GetLocalizedTeamExplainsText() gelöscht
- **Multi-Language Explanation**: Vollständig lokalisierter Explanation-Screen
- **Lokalisierte Fossilien**: Fossil-Namen in gewählter Sprache
- **Adaptive Difficulty-Display**: Schwierigkeitsgrade in passender Sprache angezeigt
- **Live Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Adaptive Zeiten**: GetAdjustedRoundDuration() basierend auf Team-Schwierigkeitsgrad
- **Content-Sets**: Separate Fossil-Arrays pro Schwierigkeitsgrad
- **Input-System**: Accelerometer mit Touch-Fallback
- **Fossil-Management**: Correct = Remove, Skip = Move to End
- **Timer-Audio**: Countdown-Sounds bei 3-2-1 Sekunden
- **Team-System**: Image-basierte Darstellung mit ScriptableObject-Config

### TABU-MINISPIEL DETAILS (NEU - VOLLSTÄNDIG IMPLEMENTIERT)

#### Architektur
- **Analog zu FossilGameManager**: Konsistente Struktur für einfache Wartung
- **Vier Screens**: Explanation, Countdown, Gameplay, Results (wie Fossil-Stirnraten)
- **Portrait-Vollbild**: Kein Split-Screen, Teams spielen nacheinander
- **Button-basiert**: Keine Accelerometer-Steuerung, nur Touch-Buttons

#### Lokalisierung
- **12 LocalizedText Assets benötigt**: Alle UI-Texte vollständig lokalisierbar
- **Keine hardcodierten Strings**: Alle Texte über LocalizedText Assets
- **Live Language-Updates**: OnLanguageChanged Event-Handler implementiert
- **GetLocalizedText() Helper**: Zentrale Methode mit Fallback-System

#### Gameplay-Features
- **Begriff-Recycling**: Übersprungene Begriffe kommen ans Ende der Queue zurück
- **Team-Exklusion**: GetRandomTermsExcluding() für unterschiedliche Begriffe pro Team
- **Dynamische Tabu-Wörter**: UI-Pool-System für variable Anzahl Tabu-Wörter
- **Timer-System**: Countdown mit Audio-Warnung bei letzten 3 Sekunden
- **Adaptive Zeiten**: DifficultyTimeSettings für schwierigkeitsgrad-basierte Rundendauer
- **Gewinner-Icon**: winnerTeamImage zeigt Gewinner-Team-Sprite auf Results Screen

#### ScriptableObjects
- **TabuTerm**: Einzelner Begriff mit lokalisierten Hauptbegriffen und Tabu-Wörtern
- **TabuCollection**: Container mit Schwierigkeitsgrad-Sets und Spieleinstellungen
- **Validation-Tools**: Context-Menu-Funktionen für Content-Prüfung
- **Fallback-System**: Automatische Fallbacks bei fehlenden Lokalisierungen

#### Mobile-Optimierung
- **Haptic Feedback**: Bei allen Button-Klicks (Correct, Skip, Start, Back)
- **Touch-Targets**: Mobile-optimierte Button-Größen
- **Portrait-Layout**: Optimiert für mobile Geräte
- **Safe Area Support**: Automatische Anpassung an Notch/Cutouts (wenn TabuGameManager Scene erstellt)

### SHARED SYSTEM DETAILS (ERWEITERT + LOKALISIERT)
- **Language-Persistence**: Gewählte Sprache wird dauerhaft gespeichert
- **Cross-System-Sync**: GameDataManager synchronisiert automatisch mit LanguageSystem
- **Save-System**: PlayerPrefs + JSON + Backup-Mechanismus + TeamSettings + Language
- **Mobile-Support**: Platform-Detection und Mobile-spezifische Features
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilität

### AUDIO-SYSTEM
- **Split-Screen Quiz**: Continue-Sounds, Feedback-Audio
- **Fossilien-Stirnraten**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback
- **Tabu-Minispiel**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback (Audio-Clips im Inspector zuweisbar)
- **Settings-UI**: Button-Click-Sounds entfernt (Dopplung mit LanguageRadioGroup)
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

## CHANGELOG - SESSION VOM 15. OKTOBER 2025

### TABU-MINISPIEL VOLLSTÄNDIG IMPLEMENTIERT (NEU)

#### Code-Implementierung
- **TabuTerm.cs erstellt**:
  - ScriptableObject für einzelne Tabu-Begriffe
  - Vollständig lokalisiert (4 Sprachen)
  - Hauptbegriff + Tabu-Wörter-Arrays pro Sprache
  - Fallback-Hierarchie implementiert
  - Validation-Tools (HasAllLocalizations, Context-Menu)
  - CreateAssetMenu Integration

- **TabuCollection.cs erstellt**:
  - ScriptableObject Container für Tabu-Begriffe
  - Separate Term-Arrays für Kids/BigKids/Adults
  - Team-Images für visuelle Darstellung
  - DifficultyTimeSettings Integration
  - GetRandomTerms() und GetRandomTermsExcluding() Methoden
  - Validation-Tools und Editor-Helpers
  - CreateAssetMenu Integration

- **TabuGameManager.cs erstellt**:
  - Vollständige Spiellogik implementiert
  - Vier Screens: Explanation, Countdown, Gameplay, Results
  - Button-basierte Steuerung (Correct/Skip)
  - Timer-System mit Audio-Countdown
  - Begriff-Recycling (Queue-System)
  - Team-Exklusion für unterschiedliche Begriffe
  - Dynamische Tabu-Wörter-Anzeige (UI-Pool)
  - Gewinner-Icon auf Results Screen
  - Vollständig lokalisiert (12 LocalizedText Assets benötigt)
  - OnLanguageChanged Event-Handler
  - Haptic Feedback Integration
  - Mobile-optimiert

#### Bugfixes & Optimierungen
- **Enum-Namespace-Fehler behoben**:
  - LanguageSystem.Language statt Language (Enum ist IN Klasse)
  - DifficultyLevel direkt (Enum ist NICHT in Klasse)
  - Alle Dateien entsprechend korrigiert

- **LanguageSystem.cs aktualisiert**:
  - FindFirstObjectByType statt FindObjectOfType (Unity 6.2)
  - Enum innerhalb der Klasse verschoben für besseres Namespacing

- **TabuCollection.cs Fehlerkorrektur**:
  - Alle DifficultySystem.DifficultyLevel zu DifficultyLevel geändert
  - DifficultySystem.DifficultyTimeSettings zu DifficultyTimeSettings geändert

- **TabuGameManager.cs Gewinner-Icon hinzugefügt**:
  - winnerTeamImage Variable hinzugefügt
  - UpdateResultsUI() erweitert für Gewinner-Icon-Anzeige
  - Analog zu FossilGameManager implementiert

#### Dokumentation
- **LocalizedText Assets Liste erstellt**:
  - 12 benötigte Assets mit allen 4 Sprachen dokumentiert
  - Texte für Explanation, Gameplay und Results Screen
  - Deutsche und englische Übersetzungen (Standard + Einfach)

- **Game Design Dokumentation**:
  - Vollständige Beschreibung des Tabu-Minispiels
  - Kernmechanik, Schwierigkeitsgrade, Steuerung
  - Spielablauf, Features, Punktesystem

### NÄCHSTE SCHRITTE (PRIORISIERT)

#### Content-Erstellung (HÖCHSTE PRIORITÄT)
1. **12 LocalizedText Assets für Tabu erstellen** (siehe Tabelle oben)
2. **TabuTerm ScriptableObjects erstellen**:
   - Mindestens 10-15 Begriffe pro Schwierigkeitsgrad
   - Vollständige Lokalisierung (4 Sprachen)
   - Variable Anzahl Tabu-Wörter pro Begriff
3. **TabuCollection ScriptableObject erstellen**:
   - Term-Arrays zuweisen
   - Team-Images zuweisen
   - Spieleinstellungen konfigurieren

#### UI-Setup (NÄCHSTE PRIORITÄT)
1. **TabuGame Scene erstellen**:
   - Analog zu Fossil-Stirnraten strukturieren
   - 4 Screen-Hierarchie aufbauen
2. **TabuWordText Prefab erstellen**:
   - TextMeshProUGUI Komponente
   - Style für Tabu-Wörter-Anzeige
3. **TabuGameManager zuweisen**:
   - Alle UI-Referenzen im Inspector
   - LocalizedText Assets verlinken
   - Audio-Clips zuweisen

---

## WICHTIGE HINWEISE FÜR ZUKÜNFTIGE ENTWICKLUNGSSCHRITTE
- **KEINE neuen großen Features** ohne vorherige Absprache!
- **Kleinere Änderungen** und **Content-Erstellung** können gerne eigenständig erfolgen.
- Bei Unsicherheiten oder Fragen immer zuerst im Team absprechen.
- **Enum-Namespace beachten**: DifficultyLevel global, LanguageSystem.Language in Klasse!
