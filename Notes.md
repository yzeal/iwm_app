# Projekt Notizen - Museum Quiz App

## WICHTIG FÜR GITHUB COPILOT
Diese Datei dient als Projektdokumentation und Übergabe-Interface zwischen verschiedenen Chat-Sitzungen. 
Die Notizen sind primär für GitHub Copilot bestimmt, um schnell in neue Gespräche einsteigen zu können.

FORMATIERUNGS-REGEL: Keine dreifachen Anführungszeichen (```) in dieser Datei verwenden - das beendet den Code-Output im Chat!

## PROJEKT ÜBERSICHT

**Projektname**: Museum Quiz App für Unity 6.2
**Zielplattform**: Mobile (Portrait-Modus), eventuell als Web-App
**Zweck**: Digitale Führung durch Museum mit 6 Räumen, jeder Raum hat Minispiele

### AKTUELLE ENTWICKLUNGSPHASE
- Entwicklung des ersten Prototyps: Team-Quiz Minispiel
- Splitscreen-Design für 2 Spieler an einem Gerät
- Verwendung des neuen Unity Input Systems und UI Systems

## AKTUELLER STAND DER IMPLEMENTIERUNG

### FERTIGGESTELLTE KOMPONENTEN

**1. Datenstrukturen**
- `QuizQuestion.cs`: Klasse für Fragen mit 4 Antworten, Shuffle-Funktionalität
- `PlayerData.cs`: Spielerdaten (Name, Score, Antwort-Status)
- `QuizRoomData.cs`: ScriptableObject für raumspezifische Quiz-Daten

**2. Game Logic**
- `SplitScreenQuizManager.cs`: Haupt-Controller für Quiz-Spielablauf
  - Timing-System für faire Punktevergabe (2 Punkte für ersten, 1 Punkt für zweiten bei richtiger Antwort)
  - Antwort-Shuffling pro Spieler (verhindert Abschauen)
  - Feedback-System mit Farbcodierung
  - Score-Tracking und Endscreen

**3. Input System**
- `TouchInputHandler.cs`: Touch-Eingabe für Continue-Funktionalität
  - Erkennt Touch-Areas für beide Spieler
  - Automatisches Continue nach beiden Antworten

### AKTUELLE ARCHITEKTUR

**Splitscreen Layout (Portrait-Modus):**
- Obere Hälfte: Spieler 2 (um 180° rotiert)
- Untere Hälfte: Spieler 1
- Zentrale Trennlinie
- Jeder Spieler: Frage + 2x2 Antwort-Grid + Score + Feedback

**Punktesystem:**
- Falsche Antwort: 0 Punkte
- Richtige Antwort (allein): 2 Punkte
- Richtige Antwort (beide richtig): 2 Punkte für Schnelleren, 1 Punkt für Langsameren

## NÄCHSTE SCHRITTE

### SOFORT UMSETZBAR
1. **UI-Setup in Unity**:
   - Canvas-Hierarchie erstellen (detaillierte Anleitung in Chat-Historie)
   - Script-Zuordnungen vornehmen
   - QuizRoomData Asset mit Testfragen erstellen

2. **Script-Zuordnungen**:
   - SplitScreenQuizManager ? auf QuizManager GameObject
   - TouchInputHandler ? auf Canvas GameObject
   - Alle UI-References im Inspector zuweisen

3. **Testing & Refinement**:
   - Timing-System testen
   - UI-Responsiveness für verschiedene Bildschirmgrößen
   - Touch-Input Funktionalität

### MITTELFRISTIGE ZIELE
- Zweites Team-Minispiel entwickeln
- Navigation zwischen Räumen implementieren
- Single-Player Versionen der Minispiele
- Integration in Gesamt-App Architektur

## TECHNISCHE DETAILS

**Unity Version**: 6.2
**Target Framework**: .NET Framework 4.7.1
**C# Version**: 9.0

**Verwendete Unity Features**:
- Neues Input System (geplant)
- UI Toolkit / Canvas System
- ScriptableObjects für Datenmanagement
- Portrait-Orientierung für Mobile

## DESIGN DECISIONS

**Modularer Ansatz**: Jedes Minispiel als eigene Szene
**Datenmanagement**: ScriptableObjects für einfache Inhaltsverwaltung
**Anti-Cheat**: Timing-System und individuelles Answer-Shuffling
**Accessibility**: Große Buttons, klare Farbcodierung

## OFFENE FRAGEN / TODO

- [ ] Exakte UI-Dimensionen für verschiedene Aspect Ratios
- [ ] Sound-Design und Audio-Feedback
- [ ] Animationen für Übergänge
- [ ] Lokalisierung (Deutsch als Hauptsprache)
- [ ] Performance-Optimierung für ältere Mobile-Geräte
- [ ] Scene-Management für Navigation zwischen Räumen

## DATEIEN STATUS

**Vollständig implementiert**:
- Assets/_GAME/Scripts/Data/QuizQuestion.cs
- Assets/_GAME/Scripts/Data/QuizRoomData.cs
- Assets/_GAME/Scripts/Game/PlayerData.cs
- Assets/_GAME/Scripts/Game/SplitScreenQuizManager.cs (mit Timing-System)
- Assets/_GAME/Scripts/UI/TouchInputHandler.cs

**Bereit für Unity Setup**:
- UI-Hierarchie (detaillierte Anleitung verfügbar)
- QuizRoomData Assets (Beispielstruktur definiert)
- Canvas-Konfiguration (Einstellungen dokumentiert)

## MUSEUM KONTEXT

**Zielgruppe**: Museumsbesucher (alle Altersgruppen)
**Räume**: 6 verschiedene Ausstellungsbereiche
**Content**: Wird später vom Museum geliefert, aktuell Platzhalter
**Einsatzgebiet**: Führung durch Ausstellung mit spielerischen Elementen

Stand: Grundgerüst für erstes Team-Minispiel fertig, UI-Setup in Unity steht als nächstes an.