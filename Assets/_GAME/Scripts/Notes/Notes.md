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

## GAME DESIGN - TABU-MINISPIEL (NEU - IMPLEMENTIERT)

### KERNMECHANIK
- **Gameplay**: Tabu-Style Erkl�rsystem - Begriffe erkl�ren ohne Tabu-W�rter zu verwenden
- **Teams**: 2 Teams spielen abwechselnd (Team 1, dann Team 2) mit individuellen Schwierigkeitsgraden
- **Rundendauer**: Konfigurierbar mit Zeit-Multiplikatoren pro Schwierigkeitsgrad (Standard: 60 Sekunden)
- **Begriffe pro Runde**: Konfigurierbar (Standard: 5)
- **Layout**: Portrait-Vollbild (KEIN Split-Screen), Teams spielen nacheinander

### SCHWIERIGKEITSGRADE (IMPLEMENTIERT)
- **Kids**: Einfachere Begriffe, weniger/einfachere Tabu-W�rter, l�nger Zeit (1.5x Multiplikator)
- **BigKids**: Mittlere Komplexit�t, moderate Tabu-W�rter, etwas l�nger Zeit (1.2x Multiplikator)
- **Adults**: Vollst�ndige Komplexit�t, viele Tabu-W�rter, normale Zeit (1.0x Multiplikator)
- **Content-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad

### STEUERUNG
- **Button-basiert**: "Richtig" (rechts) und "�berspringen" (links) Buttons
- **Haptic Feedback**: Mobile Vibration bei Button-Klicks
- **Touch-Targets**: Mobile-optimierte Button-Gr��en

### SPIELABLAUF
1. Explanation Screen mit Regeln, Team-Bild und Schwierigkeitsgrad-Anzeige
2. 3-2-1-! Countdown mit unterschiedlichen Sound-Effekten
3. Erkl�rungs-Phase:
   - Hauptbegriff oben (gro�)
   - Tabu-W�rter darunter (dynamische Anzahl)
   - Timer-Countdown oben
   - Score-Anzeige (X/Y Format)
   - Richtig/�berspringen Buttons unten
4. Team-Wechsel nach 1. Runde
5. Ergebnisscreen mit Gewinner-Ermittlung und Gewinner-Icon

### FEATURES
- **Begriff-Recycling**: �bersprungene Begriffe kommen ans Ende der Queue zur�ck
- **Timer-Warnung**: Audio-Countdown bei letzten 3 Sekunden
- **Team-Images**: Visuelle Team-Darstellung auf allen Screens
- **Gewinner-Icon**: Gewinner-Team-Image wird auf Results Screen angezeigt
- **Dynamische Tabu-W�rter**: Variable Anzahl pro Begriff (im ScriptableObject definiert)
- **Adaptive Zeiten**: Schwierigkeitsgrad-basierte Rundendauer
- **Team-Exklusion**: Team 2 bekommt (wenn m�glich) andere Begriffe als Team 1

### PUNKTESYSTEM
- **Richtig erraten**: +1 Punkt
- **�berspringen**: 0 Punkte (Begriff kommt zur�ck in Queue)
- **Zeit abgelaufen**: Runde endet
- **Alle Begriffe durch**: Runde endet

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN - MEHRSPRACHIGKEITSSYSTEM (IMPLEMENTIERT)

**LanguageSystem.cs**
- Enum Language (German_Standard, English_Standard, German_Simple, English_Simple)
- WICHTIG: Enum ist INNERHALB der LanguageSystem Klasse (Zugriff via LanguageSystem.Language)
- Singleton-Pattern f�r globalen Zugriff
- Fallback-System: Englisch ? Deutsch, Leichte Sprache ? Standard-Variante ? Deutsch
- Persistente Speicherung in PlayerPrefs
- Events f�r Sprachwechsel (OnLanguageChanged)
- Mobile-kompatible Implementierung
- FindFirstObjectByType statt FindObjectOfType (Unity 6.2 kompatibel)

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
- Debug-Methoden f�r Testing

**LanguageSettingsManager.cs**
- VEREINFACHT: Nur noch LanguageRadioGroup + Back Button
- ENTFERNT: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- Sofortige Sprachwechsel beim Radio-Button-Click
- Lokalisierte UI-Texte mit Live-Updates
- Mobile-optimierte Safe Area Unterst�tzung
- Integration mit GameDataManager f�r persistente Speicherung

### HAUPTKLASSEN - SCHWIERIGKEITSGRAD-SYSTEM (IMPLEMENTIERT)

**DifficultySystem.cs**
- Enum DifficultyLevel (Kids, BigKids, Adults) - DIREKT im globalen Namespace (NICHT in Klasse!)
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

### HAUPTKLASSEN - FOSSILIEN-STIRNRATEN (VOLLST�NDIG LOKALISIERT)

**FossilGameManager.cs**
- ERWEITERT: Vollst�ndige Lokalisierung ALLER UI-Texte (keine hardcodierten Strings mehr)
- ERWEITERT: Neue LocalizedText-Variablen f�r Anweisungen und Input-Modi
  - holdPhoneInstructionLocalizedText
  - teamExplainsInstructionLocalizedText
  - tiltModeLocalizedText
  - touchModeLocalizedText
- ERWEITERT: GetLocalizedInputModeInfo() f�r dynamische Input-Mode-Texte
- ERWEITERT: IsUsingAccelerometer() Integration f�r korrekte Input-Mode-Anzeige
- ERWEITERT: Fallback-Funktionen f�r alle neuen lokalisierten Texte
- ENTFERNT: Hardcodierte GetLocalizedHoldPhoneText() und GetLocalizedTeamExplainsText()
- Verwendet LocalizedText Assets f�r alle UI-Texte
- GetLocalizedText() Helper-Methode mit Fallback-System
- OnLanguageChanged Event-Handler f�r Live-Updates
- UpdateGameplayUI() und UpdateResultsUI() f�r Language-Updates
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
- IsUsingAccelerometer() Methode f�r externe Abfragen

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

### HAUPTKLASSEN - TABU-MINISPIEL (NEU - VOLLST�NDIG IMPLEMENTIERT + LOKALISIERT)

**TabuGameManager.cs**
- NEU: Vollst�ndige Implementierung des Tabu-Minispiels
- Analog zu FossilGameManager strukturiert f�r Konsistenz
- Vollst�ndig lokalisiert (12 LocalizedText Assets ben�tigt)
- Vier Screens: Explanation, Countdown, Gameplay, Results
- Button-basierte Steuerung (Richtig/�berspringen)
- Timer-System mit Audio-Countdown bei letzten 3 Sekunden
- Schwierigkeitsgrad-basierte Rundendauer
- Begriff-Recycling f�r �bersprungene Begriffe (ans Ende der Queue)
- Team-Exklusions-System (Team 2 bekommt andere Begriffe wenn m�glich)
- Dynamische Tabu-W�rter-Anzeige (UI-Pool-System)
- Gewinner-Icon auf Results Screen (wie bei FossilGameManager)
- OnLanguageChanged Event-Handler f�r Live-Updates
- GetLocalizedText() Helper-Methode mit Fallback-System
- Haptic Feedback bei Button-Klicks
- Integration mit GameDataManager f�r Team-Schwierigkeitsgrade

**TabuTerm.cs (ScriptableObject)**
- NEU: Datenstruktur f�r einzelne Tabu-Begriffe
- Vollst�ndig lokalisiert (4 Sprachen)
- Hauptbegriff in allen 4 Sprachen
- Tabu-W�rter-Arrays in allen 4 Sprachen (dynamische L�nge)
- Optional: Sprite f�r Begriff-Bild
- GetMainTerm(Language) Methode mit Fallback-Hierarchie
- GetTabuWords(Language) Methode mit Fallback-Hierarchie
- HasAllLocalizations() f�r Content-Validation
- ValidateLocalizations() Context-Menu f�r Editor
- CreateAssetMenu Integration

**TabuCollection.cs (ScriptableObject)**
- NEU: Container f�r Tabu-Begriffe mit Schwierigkeitsgrad-Sets
- Analog zu FossilCollection strukturiert
- Lokalisierter Collection-Name �ber LocalizedText
- Separate Term-Arrays f�r Kids/BigKids/Adults
- Team-Images f�r visuelle Darstellung
- Konfigurierbare Spieleinstellungen (termsPerRound, roundDuration)
- DifficultyTimeSettings Integration
- GetTermsForDifficulty(DifficultyLevel) Methode
- GetRandomTerms(count, difficulty) Methode
- GetRandomTermsExcluding(count, difficulty, excludeList) f�r Team 2
- GetAdjustedRoundDuration(difficulty) f�r angepasste Zeiten
- HasTermsForAllDifficulties() Validation
- HasAllLocalizations() f�r Content-Validation
- Context-Menu Validierungs-Tools
- CreateAssetMenu Integration

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
- Accelerometer-basierte Steuerung f�r Mobile (Fossil-Stirnraten)
- Button-basierte Steuerung f�r Mobile (Tabu)
- Touch-Fallback-Systeme
- Vibration/Haptic Feedback Integration

## AKTUELLER PROJEKTSTATUS

### ? VOLLST�NDIG IMPLEMENTIERT UND FUNKTIONSF�HIG

#### Mehrsprachigkeitssystem (IMPLEMENTIERT + POLISHED)
- **Vier Sprachen**: Deutsch Standard, Englisch Standard, Deutsch Einfach, Englisch Einfach
- **Custom ScriptableObject System**: Optimiert f�r Unity-Workflow
- **Fallback-Hierarchie**: Automatische Fallbacks bei fehlenden �bersetzungen
- **Game-Manager Integration**: Alle drei Hauptspiele vollst�ndig lokalisiert (KEINE hardcodierten Strings mehr)
- **Live Language-Switching**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimierte Settings-UI**: Vereinfachtes Radio-Button-System mit sofortigem Sprachwechsel
- **Performance-optimiert**: Sprach-Caching und Event-basierte Updates
- **Erweiterbar**: Einfache Integration weiterer Sprachen m�glich
- **Legacy-Support**: Bestehende Inhalte funktionieren weiterhin
- **Content-Validation**: Editor-Tools f�r �bersetzungs-Vollst�ndigkeit

#### Schwierigkeitsgrad-System (IMPLEMENTIERT)
- Drei Schwierigkeitsgrade (Kids, BigKids, Adults)
- Individuelle Team-Konfiguration mit persistenter Speicherung
- Settings-UI mit Radio-Button-System und Image-Swapping
- Difficulty-basierte Content-Auswahl f�r Quiz, Fossilien und Tabu
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

#### Fossilien-Stirnraten (VOLLST�NDIG LOKALISIERT)
- **100% Lokalisierung**: ALLE UI-Texte �ber LocalizedText Assets
- **Keine hardcodierten Strings mehr**: Explanation-Screen, Input-Modi, Anweisungen
- **Lokalisierte Fossilien**: Fossil-Namen in gew�hlter Sprache
- **Adaptive Schwierigkeitsgrad-Anzeige**: Angepasst an gew�hlte Sprache
- **Live Language-Updates**: UI aktualisiert sich automatisch
- **Dynamische Input-Mode-Texte**: Zeigt korrekte Steuerung basierend auf Platform
- Difficulty-basierte Fossil-Sets pro Team
- Adaptive Rundendauer basierend auf Schwierigkeitsgrad
- Heads-Up Style Gameplay mit Accelerometer-Steuerung
- Team-basiertes Spielsystem mit Rundenwechsel
- Timer-System mit Audio-Countdown
- Fossil-Recycling f�r �bersprungene Begriffe
- Touch-Fallback f�r Testing
- Team-Image System statt Text-Labels

#### Tabu-Minispiel (NEU - VOLLST�NDIG IMPLEMENTIERT + LOKALISIERT)
- **100% Lokalisierung**: ALLE UI-Texte �ber LocalizedText Assets (12 Assets ben�tigt)
- **Vollst�ndige Code-Implementierung**: TabuGameManager, TabuTerm, TabuCollection
- **Button-basierte Steuerung**: Richtig/�berspringen Buttons mit Haptic Feedback
- **Difficulty-basierte Begriff-Sets**: Separate Term-Arrays pro Schwierigkeitsgrad
- **Adaptive Rundendauer**: Schwierigkeitsgrad-basierte Zeitmultiplikatoren
- **Team-Exklusion**: Team 2 bekommt andere Begriffe (wenn m�glich)
- **Begriff-Recycling**: �bersprungene Begriffe kommen zur�ck in Queue
- **Timer-System**: Audio-Countdown bei letzten 3 Sekunden
- **Dynamische Tabu-W�rter**: Variable Anzahl pro Begriff mit UI-Pool-System
- **Gewinner-Icon**: Gewinner-Team-Image auf Results Screen
- **Live Language-Updates**: UI aktualisiert sich automatisch bei Sprachwechsel
- **Mobile-optimiert**: Touch-Targets, Haptic Feedback, Portrait-Vollbild
- **ScriptableObject-basiert**: Einfache Content-Erstellung mit Validation-Tools

#### Shared Systems (MOBILE-OPTIMIERT + LOKALISIERT)
- **Sprach-persistente Speicherung**: Gew�hlte Sprache bleibt gespeichert
- **Cross-System-Synchronisation**: GameDataManager ? LanguageSystem
- Plattformunabh�ngiges Save-System f�r Spielerfortschritt, Team-Settings und Sprache
- Modular structure f�r verschiedene R�ume
- Mobile-kompatible Input-Systeme
- Safe Area Support und Touch-Target-Optimierung
- Haptic Feedback Integration

### ?? N�CHSTE ENTWICKLUNGSSCHRITTE (GEPLANT)

#### Content-Erstellung (H�CHSTE PRIORIT�T)
- **LocalizedText Assets f�r Tabu-Minispiel** (12 neue Assets ben�tigt):
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
  - TabuTerm ScriptableObjects f�r alle Schwierigkeitsgrade
  - Mindestens 10-15 Begriffe pro Schwierigkeitsgrad
  - Vollst�ndige Lokalisierung (4 Sprachen)
  - TabuCollection ScriptableObject anlegen

- **UI-Setup f�r Tabu-Minispiel**:
  - Scene erstellen (analog zu Fossil-Stirnraten)
  - UI-Hierarchie aufbauen (4 Screens)
  - TabuWordText Prefab erstellen (TextMeshProUGUI)
  - Alle Referenzen im TabuGameManager zuweisen

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
?   ?   ??? LocalizedTextComponent.cs
?   ?   ??? LanguageRadioGroup.cs
?   ?   ??? LanguageSettingsManager.cs
?   ?   ??? FullscreenManager.cs (DEPRECATED f�r Mobile)
?   ?   ??? UniversalFullscreenButton.cs (DEPRECATED f�r Mobile)
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilGameManager.cs (VOLLST�NDIG LOKALISIERT)
?   ?   ??? TabuGameManager.cs (NEU - VOLLST�NDIG LOKALISIERT)
?   ?   ??? PlayerData.cs
?   ?   ??? LoadScene.cs
?   ??? Data/
?   ?   ??? GameDataManager.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizRoomData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? QuizQuestion.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilData.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? FossilCollection.cs (ERWEITERT + LOKALISIERT)
?   ?   ??? TabuTerm.cs (NEU - VOLLST�NDIG LOKALISIERT)
?   ?   ??? TabuCollection.cs (NEU - VOLLST�NDIG LOKALISIERT)
?   ?   ??? DifficultySystem.cs
?   ?   ??? LanguageSystem.cs
?   ?   ??? LocalizedText.cs
?   ??? Notes/
?       ??? Notes.md
??? Plugins/
    ??? FullscreenWebGL.jslib (DEPRECATED f�r Mobile)

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
- **Skalierbarkeit**: System designed f�r 6 verschiedene R�ume + mehrere Minispiele
- **Lokalisierung**: Vollst�ndig implementiert mit Fallback-System
- **Legacy-Support**: Bestehende Inhalte ohne Lokalisierung funktionieren weiterhin
- **Keine Hardcoding**: Alle Game-Manager vollst�ndig auf LocalizedText Assets umgestellt

#### Technical Architecture (ERWEITERT)
- **Localization System**: Event-basiert mit automatischer UI-Aktualisierung
- **Input Systems**: Neues Unity Input System + Mobile-Optimierungen
- **Save System**: Plattformunabh�ngig mit Backup-Funktionalit�t, Team-Settings und Sprache
- **Error-Handling**: Try-Catch mit Debug-Logging
- **Platform-Support**: Mobile-First mit Cross-Platform-Kompatibilit�t
- **Enum-Namespace**: DifficultyLevel global, LanguageSystem.Language in Klasse (WICHTIG f�r Kompatibilit�t!)

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### MEHRSPRACHIGKEITSSYSTEM DETAILS (IMPLEMENTIERT + POLISHED)

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

#### LanguageSettingsManager Vereinfachung (NEU)
- **Entfernt**: Apply Button, Current Language Display, Language Icons, Audio-Dopplung
- **Beibehalten**: LanguageRadioGroup, Back Button, Safe Area Support
- **Verbessert**: Sofortige Sprachwechsel ohne Apply-Step
- **Reduziert**: Von ~250 auf ~140 Zeilen Code

### SCHWIERIGKEITSGRAD-SYSTEM DETAILS (IMPLEMENTIERT)
- **Team-Settings**: Persistent �ber GameDataManager gespeichert
- **Content-Loading**: Difficulty-basierte GetQuestionsForDifficulty() / GetFossilsForDifficulty() / GetTermsForDifficulty()
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

### FOSSILIEN-STIRNRATEN DETAILS (VOLLST�NDIG LOKALISIERT)
- **100% LocalizedText Assets**: ALLE UI-Texte �ber ScriptableObjects
- **Neue LocalizedText-Variablen** (ben�tigt Assets-Erstellung):
  - holdPhoneInstructionLocalizedText: "Halte das Handy an deine Stirn" etc.
  - teamExplainsInstructionLocalizedText: "Dein Team erkl�rt dir das Fossil" etc.
  - tiltModeLocalizedText: "Neige-Modus - Neige das Handy..." etc.
  - touchModeLocalizedText: "Touch-Modus - Tippe links..." etc.
- **Dynamische Input-Mode-Texte**: GetLocalizedInputModeInfo() pr�ft IsUsingAccelerometer()
- **Entfernte Hardcoding**: GetLocalizedHoldPhoneText() und GetLocalizedTeamExplainsText() gel�scht
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

### TABU-MINISPIEL DETAILS (NEU - VOLLST�NDIG IMPLEMENTIERT)

#### Architektur
- **Analog zu FossilGameManager**: Konsistente Struktur f�r einfache Wartung
- **Vier Screens**: Explanation, Countdown, Gameplay, Results (wie Fossil-Stirnraten)
- **Portrait-Vollbild**: Kein Split-Screen, Teams spielen nacheinander
- **Button-basiert**: Keine Accelerometer-Steuerung, nur Touch-Buttons

#### Lokalisierung
- **12 LocalizedText Assets ben�tigt**: Alle UI-Texte vollst�ndig lokalisierbar
- **Keine hardcodierten Strings**: Alle Texte �ber LocalizedText Assets
- **Live Language-Updates**: OnLanguageChanged Event-Handler implementiert
- **GetLocalizedText() Helper**: Zentrale Methode mit Fallback-System

#### Gameplay-Features
- **Begriff-Recycling**: �bersprungene Begriffe kommen ans Ende der Queue zur�ck
- **Team-Exklusion**: GetRandomTermsExcluding() f�r unterschiedliche Begriffe pro Team
- **Dynamische Tabu-W�rter**: UI-Pool-System f�r variable Anzahl Tabu-W�rter
- **Timer-System**: Countdown mit Audio-Warnung bei letzten 3 Sekunden
- **Adaptive Zeiten**: DifficultyTimeSettings f�r schwierigkeitsgrad-basierte Rundendauer
- **Gewinner-Icon**: winnerTeamImage zeigt Gewinner-Team-Sprite auf Results Screen

#### ScriptableObjects
- **TabuTerm**: Einzelner Begriff mit lokalisierten Hauptbegriffen und Tabu-W�rtern
- **TabuCollection**: Container mit Schwierigkeitsgrad-Sets und Spieleinstellungen
- **Validation-Tools**: Context-Menu-Funktionen f�r Content-Pr�fung
- **Fallback-System**: Automatische Fallbacks bei fehlenden Lokalisierungen

#### Mobile-Optimierung
- **Haptic Feedback**: Bei allen Button-Klicks (Correct, Skip, Start, Back)
- **Touch-Targets**: Mobile-optimierte Button-Gr��en
- **Portrait-Layout**: Optimiert f�r mobile Ger�te
- **Safe Area Support**: Automatische Anpassung an Notch/Cutouts (wenn TabuGameManager Scene erstellt)

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
- **Tabu-Minispiel**: Countdown-Sounds, Timer-Warnings, Correct/Skip-Feedback (Audio-Clips im Inspector zuweisbar)
- **Settings-UI**: Button-Click-Sounds entfernt (Dopplung mit LanguageRadioGroup)
- **Universal**: AudioSource-basiert mit optional AudioClip assignments

---

## CHANGELOG - SESSION VOM 15. OKTOBER 2025

### TABU-MINISPIEL VOLLST�NDIG IMPLEMENTIERT (NEU)

#### Code-Implementierung
- **TabuTerm.cs erstellt**:
  - ScriptableObject f�r einzelne Tabu-Begriffe
  - Vollst�ndig lokalisiert (4 Sprachen)
  - Hauptbegriff + Tabu-W�rter-Arrays pro Sprache
  - Fallback-Hierarchie implementiert
  - Validation-Tools (HasAllLocalizations, Context-Menu)
  - CreateAssetMenu Integration

- **TabuCollection.cs erstellt**:
  - ScriptableObject Container f�r Tabu-Begriffe
  - Separate Term-Arrays f�r Kids/BigKids/Adults
  - Team-Images f�r visuelle Darstellung
  - DifficultyTimeSettings Integration
  - GetRandomTerms() und GetRandomTermsExcluding() Methoden
  - Validation-Tools und Editor-Helpers
  - CreateAssetMenu Integration

- **TabuGameManager.cs erstellt**:
  - Vollst�ndige Spiellogik implementiert
  - Vier Screens: Explanation, Countdown, Gameplay, Results
  - Button-basierte Steuerung (Correct/Skip)
  - Timer-System mit Audio-Countdown
  - Begriff-Recycling (Queue-System)
  - Team-Exklusion f�r unterschiedliche Begriffe
  - Dynamische Tabu-W�rter-Anzeige (UI-Pool)
  - Gewinner-Icon auf Results Screen
  - Vollst�ndig lokalisiert (12 LocalizedText Assets ben�tigt)
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
  - Enum innerhalb der Klasse verschoben f�r besseres Namespacing

- **TabuCollection.cs Fehlerkorrektur**:
  - Alle DifficultySystem.DifficultyLevel zu DifficultyLevel ge�ndert
  - DifficultySystem.DifficultyTimeSettings zu DifficultyTimeSettings ge�ndert

- **TabuGameManager.cs Gewinner-Icon hinzugef�gt**:
  - winnerTeamImage Variable hinzugef�gt
  - UpdateResultsUI() erweitert f�r Gewinner-Icon-Anzeige
  - Analog zu FossilGameManager implementiert

#### Dokumentation
- **LocalizedText Assets Liste erstellt**:
  - 12 ben�tigte Assets mit allen 4 Sprachen dokumentiert
  - Texte f�r Explanation, Gameplay und Results Screen
  - Deutsche und englische �bersetzungen (Standard + Einfach)

- **Game Design Dokumentation**:
  - Vollst�ndige Beschreibung des Tabu-Minispiels
  - Kernmechanik, Schwierigkeitsgrade, Steuerung
  - Spielablauf, Features, Punktesystem

### N�CHSTE SCHRITTE (PRIORISIERT)

#### Content-Erstellung (H�CHSTE PRIORIT�T)
1. **12 LocalizedText Assets f�r Tabu erstellen** (siehe Tabelle oben)
2. **TabuTerm ScriptableObjects erstellen**:
   - Mindestens 10-15 Begriffe pro Schwierigkeitsgrad
   - Vollst�ndige Lokalisierung (4 Sprachen)
   - Variable Anzahl Tabu-W�rter pro Begriff
3. **TabuCollection ScriptableObject erstellen**:
   - Term-Arrays zuweisen
   - Team-Images zuweisen
   - Spieleinstellungen konfigurieren

#### UI-Setup (N�CHSTE PRIORIT�T)
1. **TabuGame Scene erstellen**:
   - Analog zu Fossil-Stirnraten strukturieren
   - 4 Screen-Hierarchie aufbauen
2. **TabuWordText Prefab erstellen**:
   - TextMeshProUGUI Komponente
   - Style f�r Tabu-W�rter-Anzeige
3. **TabuGameManager zuweisen**:
   - Alle UI-Referenzen im Inspector
   - LocalizedText Assets verlinken
   - Audio-Clips zuweisen

---

## WICHTIGE HINWEISE F�R ZUK�NFTIGE ENTWICKLUNGSSCHRITTE
- **KEINE neuen gro�en Features** ohne vorherige Absprache!
- **Kleinere �nderungen** und **Content-Erstellung** k�nnen gerne eigenst�ndig erfolgen.
- Bei Unsicherheiten oder Fragen immer zuerst im Team absprechen.
- **Enum-Namespace beachten**: DifficultyLevel global, LanguageSystem.Language in Klasse!
