# OpenDAoC - Documentazione Completa

# secret pUcFmpBdQe4r
Benvenuto nella documentazione completa di **OpenDAoC**, un emulatore open-source per Dark Age of Camelot (DAoC).

## 📚 Indice della Documentazione

- **[Configurazione](CONFIGURATION.md)** - Guida completa alle impostazioni del server
- **[Docker & Deployment](DOCKER.md)** - Come deployare il server con Docker
- **[Sviluppo](DEVELOPMENT.md)** - Guida per sviluppatori
- **[Tipi di Server](SERVER_TYPES.md)** - Modalità di gioco disponibili (Normal, PvP, PvE, etc.)
- **[Architettura](ARCHITECTURE.md)** - Struttura del progetto e componenti

## 🎮 Cos'è OpenDAoC?

OpenDAoC è un emulatore completo per Dark Age of Camelot, originariamente un fork di [DOLSharp](https://github.com/Dawn-of-Light/DOLSharp), ora completamente riscritto con architettura ECS (Entity Component System).

### Caratteristiche Principali

- **Architettura ECS moderna** - Performance e scalabilità ottimizzate
- **Supporto multi-realm** - Albion, Midgard, Hibernia
- **Sistema di combattimento completo** - RvR, PvE, duelli
- **Keeps & Relics** - Sistema completo di fortezze e reliquie
- **Crafting & Housing** - Sistema di crafting e case per giocatori
- **Docker-ready** - Deploy facile con Docker e docker-compose
- **Database MySQL/SQLite** - Supporto per database multipli
- **Patch 1.65 focus** - Ricrea l'esperienza classica di DAoC

## 🚀 Quick Start

### Con Docker (Consigliato)

```bash
# Clone del repository
git clone https://github.com/OpenDAoC/OpenDAoC-Core.git
cd OpenDAoC-Core

# Avvia con docker-compose
docker compose up -d

# Verifica i log
docker logs -f opendaoc-server
```

### Build Locale

```bash
# Build del progetto
dotnet build "Dawn of Light.sln" -c Release

# Publish
dotnet publish CoreServer/CoreServer.csproj -c Release -o publish

# Esegui
cd publish
dotnet CoreServer.dll
```

## 📁 Struttura del Progetto

```
OpenDAoC-Core/
├── CoreBase/           # Classi base e utilities
├── CoreDatabase/       # Layer di accesso al database
├── CoreServer/         # Entry point dell'applicazione
├── GameServer/         # Logica principale del gioco
│   ├── ai/            # Intelligenza artificiale NPCs
│   ├── commands/      # Comandi giocatore e GM
│   ├── ECS-Components/# Componenti Entity Component System
│   ├── ECS-Effects/   # Sistema effetti ECS
│   ├── gameobjects/   # Oggetti di gioco (player, NPC, items)
│   ├── keeps/         # Sistema keeps e fortezze
│   ├── packets/       # Gestione pacchetti di rete
│   ├── serverrules/   # Regole server (Normal, PvP, PvE)
│   ├── spells/        # Sistema magie
│   └── world/         # Gestione mondo di gioco
├── Pathing/           # Sistema pathfinding
├── Tests/             # Unit tests
├── config/            # File di configurazione
├── Dockerfile         # Docker build
└── docker-compose.yml # Docker orchestration
```

## 🔧 Configurazione Rapida

### Variabili d'Ambiente Principali

```yaml
GAME_TYPE: "PvP"              # Tipo server: Normal, PvP, PvE, Roleplay, Casual, Test
SERVER_NAME: "OpenDAoC"       # Nome del server
DB_TYPE: "MYSQL"              # Tipo database: MYSQL, SQLITE, MSSQL
DB_CONNECTION_STRING: "..."   # Stringa connessione database
AUTO_ACCOUNT_CREATION: "True" # Creazione automatica account
ENABLE_COMPILATION: "True"    # Compilazione script all'avvio
```

Vedi [CONFIGURATION.md](CONFIGURATION.md) per la lista completa.

## 🎯 Modalità di Gioco

OpenDAoC supporta diverse modalità di server:

| Modalità | Descrizione | PvP | Regole Speciali |
|----------|-------------|-----|-----------------|
| **Normal** | Server standard con regole classiche | Solo RvR | Nessun attacco stesso realm |
| **PvP** | Full PvP con immunità temporanee | Ovunque | Timer immunità dopo morte |
| **PvE** | Focus su contenuto PvE | Solo consensuale | Protezioni extra |
| **Roleplay** | Server roleplay | RvR | Regole RP enforced |
| **Casual** | Server casual friendly | RvR | XP bonus, regole rilassate |
| **Test** | Server di test | Variabile | Per sviluppo |

Vedi [SERVER_TYPES.md](SERVER_TYPES.md) per dettagli completi.

## 🐳 Docker

### Immagini Disponibili

- **GitHub Container Registry** (consigliato): `ghcr.io/opendaoc/opendaoc-core:latest`
- **Docker Hub**: `claitz/opendaoc:latest`

### Comandi Utili

```bash
# Avvia i container
docker compose up -d

# Ferma i container
docker compose down

# Rebuild e riavvio
docker compose down && docker compose up -d --build

# Visualizza log
docker logs -f opendaoc-server

# Accedi al container
docker exec -it opendaoc-server sh

# Backup database
docker exec opendaoc-db mysqldump -u root -p[password] opendaoc > backup.sql
```

Vedi [DOCKER.md](DOCKER.md) per la guida completa.

## 🗄️ Database

### Setup Database

OpenDAoC richiede il database companion:

```bash
git clone https://github.com/OpenDAoC/OpenDAoC-Database.git
```

Il database viene automaticamente importato al primo avvio con Docker.

### Supporto Database

- **MySQL/MariaDB** (consigliato per produzione)
- **SQLite** (consigliato per sviluppo/test)
- **MSSQL** (supportato)

## 👥 Repository Companion

- **[OpenDAoC Database](https://github.com/OpenDAoC/OpenDAoC-Database)** - Database v1.65
- **[Account Manager](https://github.com/OpenDAoC/opendaoc-accountmanager)** - Gestione account web
- **[Client Launcher](https://github.com/OpenDAoC/OpenDAoC-Launcher)** - Launcher per client

## 🛠️ Sviluppo

### Requisiti

- **.NET 10.0 SDK** o superiore
- **IDE**: Visual Studio 2022, Rider, o VS Code
- **Database**: MySQL 8.0+ o SQLite 3
- **Git** per version control

### Build & Test

```bash
# Build
dotnet build "Dawn of Light.sln" -c Debug

# Run tests
dotnet test

# Publish release
dotnet publish "Dawn of Light.sln" -c Release -o publish
```

Vedi [DEVELOPMENT.md](DEVELOPMENT.md) per la guida completa allo sviluppo.

## 📝 Comandi GM Utili

```
/gmrelevel [level]     # Cambia il tuo livello
/jump [location]       # Teleport
/mob create [template] # Crea un NPC
/item create [template]# Crea un item
/broadcast [message]   # Messaggio globale
/kick [player]         # Kick giocatore
```

## 🔐 Sicurezza

- **Non committare** credenziali database nel codice
- Usa **variabili d'ambiente** per dati sensibili
- Cambia le **password di default** in produzione
- Abilita **firewall** per porte del server
- Usa **SSL/TLS** per connessioni database remote

## 📊 Performance

### Ottimizzazioni Consigliate

- **CPU_USE**: Imposta al numero di core disponibili
- **DB_AUTOSAVE_INTERVAL**: 10-15 minuti per produzione
- **ENABLE_COMPILATION**: False in produzione (compila prima)
- **Garbage Collection**: Server mode per .NET

### Monitoring

```bash
# CPU e memoria
docker stats opendaoc-server

# Log in tempo reale
docker logs -f opendaoc-server

# Metriche database
docker exec opendaoc-db mysqladmin -u root -p status
```

## 🐛 Troubleshooting

### Il server non parte

1. Verifica i log: `docker logs opendaoc-server`
2. Controlla la connessione database
3. Verifica le porte siano libere (10300, 10400)

### Database connection failed

1. Verifica `DB_CONNECTION_STRING`
2. Assicurati che il container db sia avviato
3. Controlla password e username

### Server in modalità sbagliata

1. Verifica `GAME_TYPE` in docker-compose.yml
2. Riavvia i container: `docker compose down && docker compose up -d`
3. Controlla i log per conferma: `docker logs opendaoc-server | grep -i rule`

## 📖 Risorse Aggiuntive

- **Sito Ufficiale**: [https://www.opendaoc.com](https://www.opendaoc.com)
- **Documentazione Online**: [https://www.opendaoc.com/docs/](https://www.opendaoc.com/docs/)
- **GitHub**: [https://github.com/OpenDAoC](https://github.com/OpenDAoC)
- **Discord**: Unisciti alla community per supporto

## 📄 Licenza

OpenDAoC è rilasciato sotto licenza **GNU General Public License (GPL) v3**.

Vedi il file [LICENSE](../LICENSE) per i dettagli completi.

## 🤝 Contribuire

Contributi sono benvenuti! Per contribuire:

1. Fork del repository
2. Crea un branch per la tua feature (`git checkout -b feature/AmazingFeature`)
3. Commit delle modifiche (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

Vedi [DEVELOPMENT.md](DEVELOPMENT.md) per le linee guida di sviluppo.

---

**Versione Documentazione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026  
**Compatibilità**: OpenDAoC-Core latest
