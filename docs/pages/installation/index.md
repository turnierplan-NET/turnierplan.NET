---
icon: lucide/server
---

# Installation

Dieser Artikel beschreibt verschiedene Möglichkeiten, **turnierplan.NET** mithilfe des offiziellen Container-Images bereitzustellen. Innerhalb der GitHub-Organisation wird für jedes [Release](https://github.com/turnierplan-NET/turnierplan.NET/releases) das offizielle Container-Image veröffentlicht: [ghcr.io/turnierplan-net/turnierplan](https://github.com/turnierplan-NET/turnierplan.NET/pkgs/container/turnierplan)

Um turnierplan.NET lokal zu testen, kann der folgende Befehl verwendet werden:

```shell
docker run -p 80:8080 -e Database__InMemory="true" ghcr.io/turnierplan-net/turnierplan:latest
```

Die Weboberfläche kann über [localhost:80](http://localhost:80) erreicht werden. Der Benutzername und das initiale Passwort für den Administratorbenutzer werden in den Container-Logs angezeigt. Nach einem Neustart des Containers gehen allerdings alle Daten verloren, da nur eine in-memory Datenbank verwendet wird.

## Deployment

Für ein produktives Deployment gibt es nachfolgende Möglichkeiten basierend auf dem Container-Image. Weitere Methoden werden in der Zukunft ergänzt.

- [Docker Compose](./docker-compose.md) für die installation auf einem dedizierten Server oder einer VM
- [Azure (Terraform)](./azure-terraform.md) für ein Deployment innerhalb einer Microsoft Azure Subscription
