# Projekt Notizen - Museum Quiz App

## WICHTIG F�R GITHUB COPILOT
Diese Datei dient als Projektdokumentation und �bergabe-Interface zwischen verschiedenen Chat-Sitzungen. 
Die Notizen sind prim�r f�r GitHub Copilot bestimmt, um schnell in neue Gespr�che einsteigen zu k�nnen.

FORMATIERUNGS-REGEL: Keine dreifachen Anf�hrungszeichen (```) in dieser Datei verwenden - das beendet den Code-Output im Chat!

## PROJEKT �BERSICHT

**Projektname**: Museum Quiz App f�r Unity 6.2
**Zielplattform**: Mobile (Portrait-Modus), eventuell als Web-App
**Zweck**: Digitale F�hrung durch Museum mit 6 R�umen, jeder Raum hat Minispiele

### AKTUELLE ENTWICKLUNGSPHASE
- Entwicklung des ersten Prototyps: Team-Quiz Minispiel
- Splitscreen-Design f�r 2 Spieler an einem Ger�t
- Verwendung des neuen Unity Input Systems und UI Systems

## AKTUELLER STAND DER IMPLEMENTIERUNG

### FERTIGGESTELLTE KOMPONENTEN

**1. Datenstrukturen**
- `QuizQuestion.cs`: Klasse f�r Fragen mit 4 Antworten, Shuffle-Funktionalit�t
- `PlayerData.cs`: Spielerdaten (Name, Score, Antwort-Status)
- `QuizRoomData.cs`: ScriptableObject f�r raumspezifische Quiz-Daten

**2. Game Logic**
- `SplitScreenQuizManager.cs`: Haupt-Controller f�r Quiz-Spielablauf
  - Timing-System f�r faire Punktevergabe (2 Punkte f�r ersten, 1 Punkt f�r zweiten bei richtiger Antwort)
  - Antwort-Shuffling pro Spieler (verhindert Abschauen)
  - Feedback-System mit Farbcodierung
  - Score-Tracking und Endscreen

**3. Input System**
- `TouchInputHandler.cs`: Touch-Eingabe f�r Continue-Funktionalit�t
  - Erkennt Touch-Areas f�r beide Spieler
  - Automatisches Continue nach beiden Antworten

### AKTUELLE ARCHITEKTUR

**Splitscreen Layout (Portrait-Modus):**
- Obere H�lfte: Spieler 2 (um 180� rotiert)
- Untere H�lfte: Spieler 1
- Zentrale Trennlinie
- Jeder Spieler: Frage + 2x2 Antwort-Grid + Score + Feedback

**Punktesystem:**
- Falsche Antwort: 0 Punkte
- Richtige Antwort (allein): 2 Punkte
- Richtige Antwort (beide richtig): 2 Punkte f�r Schnelleren, 1 Punkt f�r Langsameren

## N�CHSTE SCHRITTE

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
   - UI-Responsiveness f�r verschiedene Bildschirmgr��en
   - Touch-Input Funktionalit�t

### MITTELFRISTIGE ZIELE
- Zweites Team-Minispiel entwickeln
- Navigation zwischen R�umen implementieren
- Single-Player Versionen der Minispiele
- Integration in Gesamt-App Architektur

## TECHNISCHE DETAILS

**Unity Version**: 6.2
**Target Framework**: .NET Framework 4.7.1
**C# Version**: 9.0

**Verwendete Unity Features**:
- Neues Input System (geplant)
- UI Toolkit / Canvas System
- ScriptableObjects f�r Datenmanagement
- Portrait-Orientierung f�r Mobile

## DESIGN DECISIONS

**Modularer Ansatz**: Jedes Minispiel als eigene Szene
**Datenmanagement**: ScriptableObjects f�r einfache Inhaltsverwaltung
**Anti-Cheat**: Timing-System und individuelles Answer-Shuffling
**Accessibility**: Gro�e Buttons, klare Farbcodierung

## OFFENE FRAGEN / TODO

- [ ] Exakte UI-Dimensionen f�r verschiedene Aspect Ratios
- [ ] Sound-Design und Audio-Feedback
- [ ] Animationen f�r �berg�nge
- [ ] Lokalisierung (Deutsch als Hauptsprache)
- [ ] Performance-Optimierung f�r �ltere Mobile-Ger�te
- [ ] Scene-Management f�r Navigation zwischen R�umen

## DATEIEN STATUS

**Vollst�ndig implementiert**:
- Assets/_GAME/Scripts/Data/QuizQuestion.cs
- Assets/_GAME/Scripts/Data/QuizRoomData.cs
- Assets/_GAME/Scripts/Game/PlayerData.cs
- Assets/_GAME/Scripts/Game/SplitScreenQuizManager.cs (mit Timing-System)
- Assets/_GAME/Scripts/UI/TouchInputHandler.cs

**Bereit f�r Unity Setup**:
- UI-Hierarchie (detaillierte Anleitung verf�gbar)
- QuizRoomData Assets (Beispielstruktur definiert)
- Canvas-Konfiguration (Einstellungen dokumentiert)

## MUSEUM KONTEXT

**Zielgruppe**: Museumsbesucher (alle Altersgruppen)
**R�ume**: 6 verschiedene Ausstellungsbereiche
**Content**: Wird sp�ter vom Museum geliefert, aktuell Platzhalter
**Einsatzgebiet**: F�hrung durch Ausstellung mit spielerischen Elementen

Stand: Grundger�st f�r erstes Team-Minispiel fertig, UI-Setup in Unity steht als n�chstes an.