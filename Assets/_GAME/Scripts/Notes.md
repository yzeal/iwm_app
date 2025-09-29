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
5. Continue per Touch
6. Ergebnisscreen nach letzter Frage

## AKTUELLE CODE-STRUKTUR

### HAUPTKLASSEN (ALLE IMPLEMENTIERT UND FUNKTIONSF�HIG)

**SplitScreenQuizManager.cs**
- Zentrale Spiellogik und UI-Management
- Punkteberechnung basierend auf Antwortzeit
- Feedback-System f�r beide Spieler
- �bergang zwischen Fragen und Ergebnisscreen

**TouchInputHandler.cs**  
- Touch-Eingaben f�r beide Spielerbereiche
- Continue-Button Aktivierung
- Bereichserkennung f�r Player 1/2 Touch Areas

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

### UI-STRUKTUR (BEREITS IMPLEMENTIERT)
- **Split-Screen Layout**: Funktionsf�hige gespiegelte Darstellung
- **Touch Areas**: Klar getrennte Eingabebereiche
- **Answer Buttons**: 4 Buttons pro Spieler mit Highlighting
- **Feedback System**: Punkte- und Korrektheit-Anzeige
- **Score Display**: Live-Update w�hrend Spiel
- **Continue Mechanism**: Touch-anywhere nach beiden Antworten

## AKTUELLER PROJEKTSTATUS

### ? VOLLST�NDIG IMPLEMENTIERT UND FUNKTIONSF�HIG
- Split-Screen Quiz-Mechanik
- Touch-Input System
- Punkteberechnung (Zeit-basiert)
- UI-Layout und -Management
- ScriptableObject-basierte Content-Struktur
- Feedback-System
- Ergebnisscreen
- Modular structure f�r verschiedene R�ume

### ?? N�CHSTE ENTWICKLUNGSSCHRITTE (POTENTIELL)
- Integration in gr��eres F�hrungssystem
- Weitere Minispiel-Prototypen
- Content-Integration mit echten Museumsfragen
- Performance-Optimierung f�r mobile Ger�te
- Web-App Export Testing

### ?? PROJEKTSTRUKTUR
Assets/_GAME/
??? Scripts/
?   ??? UI/
?   ?   ??? TouchInputHandler.cs
?   ??? Game/
?   ?   ??? SplitScreenQuizManager.cs
?   ?   ??? PlayerData.cs
?   ??? Data/
?       ??? QuizRoomData.cs
?       ??? QuizQuestion.cs
??? ...

### ?? DEVELOPMENT NOTES
- **Modularity**: Jeder Museumsraum kann eigene Szene haben
- **Content Management**: QuizRoomData ScriptableObjects f�r einfache Fragenerstellung
- **Skalierbarkeit**: System designed f�r 6 verschiedene R�ume
- **Touch-System**: Funktioniert mit Unity's neuem Input System

## WICHTIGE IMPLEMENTIERUNGSDETAILS

### ANTI-SCHUMMEL-MECHANISMUS
- Jeder Spieler erh�lt shuffled answers �ber QuizQuestion.GetShuffledAnswers()
- Tracking der korrekten Antwort per GetCorrectAnswerIndex()

### TIMING-SYSTEM
- Antwortzeit wird per Time.time gemessen
- Punkteverteilung basiert auf player1AnswerTime vs player2AnswerTime
- Ber�cksichtigt auch Korrektheit beider Antworten

### UI-MANAGEMENT
- Dynamische Button-Beschriftung
- Color-Highlighting f�r ausgew�hlte Antworten
- Automatische UI-Updates nach Antworten

---

**LETZTER STATUS**: Alle grundlegenden Features implementiert und funktionsf�hig. Projekt bereit f�r Content-Integration oder Erweiterung um weitere Minispiele.