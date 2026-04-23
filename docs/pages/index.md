---
icon: lucide/house
---

# Startseite

turnierplan.NET ist eine **Open-Source Webanwendung zur Organisation von Turnieren** in Fußballvereinen ([GitHub](https://github.com/turnierplan-NET/turnierplan.NET)).

<div style="margin-top: 2em;" class="grid cards" markdown>
- :lucide-land-plot: Flexible Spielpläne
- :lucide-shield-user: Benutzerverwaltung
- :lucide-globe: Öffentliche Ansicht für Besucher
- :lucide-file-text: Export von PDF-Dokumenten
- :lucide-settings: Zahlreiche Konfigurationsmöglichkeiten
- :lucide-mail: Verwaltung von Turnieranmeldungen
</div>

## Made in Germany

turnierplan.NET wird hauptsächlich von Fußballfreunden aus dem nordöstlichen Baden-Württemberg, Deutschland verwendet und maintained 🇩🇪.

Aus diesem Grund sind die Benutzeroberfläche sowie die Dokumentation aktuell ausschließlich in deutscher Sprache verfügbar. Zudem sind die verwendeten Begriffe und Funktionen speziell für die hierzulande üblichen Turniere und Veranstaltungen ausgelegt. Verbesserungsvorschläge sind jederzeit gerne gesehen und können im [Repository](https://github.com/turnierplan-NET/turnierplan.NET) hinterlassen werden.

Falls turnierplan.NET dennoch für deinen Verein / deine Organisation infrage kommt: Der Quelltext und die Container-Images sind unter der [AGPL-3.0](https://github.com/turnierplan-NET/turnierplan.NET/blob/main/LICENSE) lizenziert und dementsprechend frei verwendbar. Mögliche Vorgehensweisen zum Erstellen eines eigenen Setups sind in der [Installationsanleitung](installation/index.md) beschrieben.

## Technische Dokumentation

Der Großteil der Anwendung ist in C# geschrieben und basiert auf dem [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) Framework. Die primäre Datenbank, welche von turnierplan.NET verwendet wird, ist [PostgreSQL](https://www.postgresql.org/). Bilddateien werden außerhalb der Datenbank als lokale Dateien oder in einem cloud-basierten Blob-Storage gespeichert. Sämtliche administrative Aufgaben wie das Anlegen von Nutzern oder das Planen und Durchführen von Turnieren werden mit dem turnierplan.NET *Portal* erledigt. Dies ist eine [SPA](https://de.wikipedia.org/wiki/Single-Page-Webanwendung) basierend auf dem [Angular](https://angular.dev/) Framework. Öffentlich sichtbare HTML-Seiten werden allerdings direkt von der ASP.NET-Anwendung gerendert und bereitgestellt.
