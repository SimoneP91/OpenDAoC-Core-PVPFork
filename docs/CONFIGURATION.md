# Configurazione OpenDAoC

Guida completa alla configurazione del server OpenDAoC.

## 📋 Indice

- [File di Configurazione](#file-di-configurazione)
- [Variabili d'Ambiente](#variabili-dambiente)
- [Configurazione Database](#configurazione-database)
- [Configurazione di Rete](#configurazione-di-rete)
- [Configurazione Server Type](#configurazione-server-type)
- [Configurazione Avanzata](#configurazione-avanzata)

## 📄 File di Configurazione

### serverconfig.xml

Il file principale di configurazione si trova in `config/serverconfig.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
    <Server>
        <Port>10300</Port>
        <IP>0.0.0.0</IP>
        <RegionIP>0.0.0.0</RegionIP>
        <RegionPort>10400</RegionPort>
        <UdpIP>0.0.0.0</UdpIP>
        <UdpPort>10400</UdpPort>
        <EnableUPnP>False</EnableUPnP>
        <DetectRegionIP>True</DetectRegionIP>
        <ServerName>OpenDAoC</ServerName>
        <ServerNameShort>OPENDAOC</ServerNameShort>
        <GameType>PvP</GameType>
        <ServerRules>PvPServerRules</ServerRules>
        <EnableCompilation>True</EnableCompilation>
        <AutoAccountCreation>True</AutoAccountCreation>
        <DBType>MYSQL</DBType>
        <DBConnectionString>server=localhost;port=3306;database=opendaoc;userid=root;password=yourpassword</DBConnectionString>
        <DBAutosave>True</DBAutosave>
        <DBAutosaveInterval>10</DBAutosaveInterval>
        <CpuUse>8</CpuUse>
    </Server>
</root>
```

### logconfig.xml

Configurazione del logging in `config/logconfig.xml`.

## 🔧 Variabili d'Ambiente

Quando usi Docker, le variabili d'ambiente sovrascrivono il file `serverconfig.xml`.

### Variabili Server Base

| Variabile | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `SERVER_NAME` | string | "OpenDAoC" | Nome completo del server |
| `SERVER_NAME_SHORT` | string | "OPENDAOC" | Nome breve (max 10 caratteri) |
| `SERVER_IP` | string | "0.0.0.0" | IP di bind del server |
| `SERVER_PORT` | int | 10300 | Porta TCP principale |
| `REGION_IP` | string | "0.0.0.0" | IP per connessioni region |
| `REGION_PORT` | int | 10400 | Porta UDP region |
| `UDP_IP` | string | "0.0.0.0" | IP UDP |
| `UDP_PORT` | int | 10400 | Porta UDP |
| `DETECT_REGION_IP` | bool | True | Auto-detect IP pubblico |
| `ENABLE_UPNP` | bool | False | Abilita UPnP per port forwarding |

### Variabili Game Type

| Variabile | Valori Possibili | Default | Descrizione |
|-----------|------------------|---------|-------------|
| `GAME_TYPE` | Normal, PvP, PvE, Roleplay, Casual, Test | Normal | Tipo di server e regole |

**Valori GAME_TYPE:**

- **Normal**: Server standard con regole classiche RvR
- **PvP**: Full PvP con timer immunità
- **PvE**: Focus su contenuto PvE, PvP limitato
- **Roleplay**: Server roleplay con regole specifiche
- **Casual**: Server casual-friendly con bonus XP
- **Test**: Server di test per sviluppo

### Variabili Database

| Variabile | Valori | Default | Descrizione |
|-----------|--------|---------|-------------|
| `DB_TYPE` | MYSQL, SQLITE, MSSQL, ODBC, OLEDB | MYSQL | Tipo di database |
| `DB_CONNECTION_STRING` | string | - | Stringa di connessione |
| `DB_AUTOSAVE` | bool | True | Salvataggio automatico |
| `DB_AUTOSAVE_INTERVAL` | int | 10 | Intervallo autosave (minuti) |

**Esempi Connection String:**

```bash
# MySQL/MariaDB
DB_CONNECTION_STRING="server=localhost;port=3306;database=opendaoc;userid=root;password=mypassword;treattinyasboolean=true"

# SQLite
DB_CONNECTION_STRING="Data Source=./dol.sqlite3.db;Version=3;Pooling=False;Cache Size=1073741824"

# MSSQL
DB_CONNECTION_STRING="Server=localhost;Database=opendaoc;User Id=sa;Password=mypassword;"
```

### Variabili Compilazione & Script

| Variabile | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `ENABLE_COMPILATION` | bool | True | Compila script all'avvio |
| `SCRIPT_COMPILATION_TARGET` | string | "./lib/GameServerScripts.dll" | Output compilazione |
| `SCRIPT_ASSEMBLIES` | string | "" | Assembly aggiuntive (separati da virgola) |

### Variabili Account & Sicurezza

| Variabile | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `AUTO_ACCOUNT_CREATION` | bool | True | Creazione automatica account |
| `CHEAT_LOGGER_NAME` | string | "cheats" | Nome logger per cheat |
| `GM_ACTION_LOGGER_NAME` | string | "gmactions" | Nome logger azioni GM |
| `INVALID_NAMES_FILE` | string | "./config/invalidnames.txt" | File nomi vietati |

### Variabili Performance

| Variabile | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `CPU_USE` | int | 8 | Numero di CPU core da usare |

### Variabili Docker

| Variabile | Tipo | Default | Descrizione |
|-----------|------|---------|-------------|
| `UID` | int | 1000 | User ID per permessi file |
| `GID` | int | 1000 | Group ID per permessi file |
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | bool | False | Globalizzazione .NET |

## 🗄️ Configurazione Database

### MySQL/MariaDB (Consigliato per Produzione)

```yaml
environment:
  DB_TYPE: "MYSQL"
  DB_CONNECTION_STRING: "server=db;port=3306;database=opendaoc;userid=root;password=yourpassword;treattinyasboolean=true"
```

**Parametri Connection String MySQL:**

- `server`: Hostname del database
- `port`: Porta (default 3306)
- `database`: Nome database
- `userid`: Username
- `password`: Password
- `treattinyasboolean=true`: Tratta TINYINT(1) come boolean
- `charset=utf8mb3`: Set charset (opzionale)
- `pooling=true`: Connection pooling (opzionale)

### SQLite (Consigliato per Dev/Test)

```yaml
environment:
  DB_TYPE: "SQLITE"
  DB_CONNECTION_STRING: "Data Source=./dol.sqlite3.db;Version=3;Pooling=False;Cache Size=1073741824;Journal Mode=Off;Synchronous=Off;Foreign Keys=True;Default Timeout=60"
```

**Parametri Connection String SQLite:**

- `Data Source`: Path al file database
- `Version=3`: Versione SQLite
- `Pooling=False`: Disabilita pooling
- `Cache Size`: Dimensione cache in bytes
- `Journal Mode=Off`: Performance (⚠️ rischio corruzione)
- `Synchronous=Off`: Performance (⚠️ rischio corruzione)
- `Foreign Keys=True`: Abilita foreign keys

⚠️ **Attenzione**: `Journal Mode=Off` e `Synchronous=Off` migliorano le performance ma aumentano il rischio di corruzione database in caso di crash. Usa solo per dev/test.

### MSSQL

```yaml
environment:
  DB_TYPE: "MSSQL"
  DB_CONNECTION_STRING: "Server=localhost;Database=opendaoc;User Id=sa;Password=yourpassword;TrustServerCertificate=True"
```

## 🌐 Configurazione di Rete

### Porte Necessarie

| Porta | Protocollo | Uso | Esporre |
|-------|-----------|-----|---------|
| 10300 | TCP | Server principale | ✅ Sì |
| 10400 | UDP | Region server | ✅ Sì |
| 3306 | TCP | MySQL (se esterno) | ❌ No |

### Configurazione Firewall

```bash
# Linux (ufw)
sudo ufw allow 10300/tcp
sudo ufw allow 10400/udp

# Linux (iptables)
sudo iptables -A INPUT -p tcp --dport 10300 -j ACCEPT
sudo iptables -A INPUT -p udp --dport 10400 -j ACCEPT

# Windows Firewall
netsh advfirewall firewall add rule name="OpenDAoC TCP" dir=in action=allow protocol=TCP localport=10300
netsh advfirewall firewall add rule name="OpenDAoC UDP" dir=in action=allow protocol=UDP localport=10400
```

### IP Pubblico e NAT

Se il server è dietro NAT:

```yaml
environment:
  DETECT_REGION_IP: "True"  # Auto-detect IP pubblico
  REGION_IP: "your.public.ip.here"  # Oppure specifica manualmente
```

### UPnP

Per configurazione automatica port forwarding:

```yaml
environment:
  ENABLE_UPNP: "True"
```

⚠️ **Nota**: UPnP può essere un rischio di sicurezza. Usa solo in ambienti controllati.

## 🎮 Configurazione Server Type

### Normal Server

```yaml
environment:
  GAME_TYPE: "Normal"
```

**Caratteristiche:**
- RvR classico (Realm vs Realm)
- Nessun attacco tra membri dello stesso realm
- Duelli consentiti
- Keep warfare
- Relics

### PvP Server

```yaml
environment:
  GAME_TYPE: "PvP"
```

**Caratteristiche:**
- Full PvP ovunque
- Timer immunità dopo morte:
  - Ucciso da player: 120 secondi (configurabile)
  - Ucciso da mob: 60 secondi (configurabile)
- Timer immunità dopo teleport: 30 secondi
- Safety flag per player sotto livello 10

**Configurazioni Timer PvP:**

Modifica in `ServerProperties` o database:

```sql
-- Timer immunità dopo morte da player (secondi)
UPDATE serverproperty SET value='120' WHERE keyname='TIMER_KILLED_BY_PLAYER';

-- Timer immunità dopo morte da mob (secondi)
UPDATE serverproperty SET value='60' WHERE keyname='TIMER_KILLED_BY_MOB';

-- Timer immunità dopo teleport (secondi)
UPDATE serverproperty SET value='30' WHERE keyname='TIMER_PVP_TELEPORT';
```

### PvE Server

```yaml
environment:
  GAME_TYPE: "PvE"
```

**Caratteristiche:**
- Focus su contenuto PvE
- PvP solo consensuale (duelli, arene)
- Protezioni extra per player
- Ideale per server cooperativi

### Roleplay Server

```yaml
environment:
  GAME_TYPE: "Roleplay"
```

**Caratteristiche:**
- Regole roleplay enforced
- Naming policy più stretta
- Comportamento in-character richiesto

### Casual Server

```yaml
environment:
  GAME_TYPE: "Casual"
```

**Caratteristiche:**
- XP bonus
- Regole più rilassate
- Ideale per casual players

### Test Server

```yaml
environment:
  GAME_TYPE: "Test"
```

**Caratteristiche:**
- Per sviluppo e testing
- Regole variabili
- Comandi GM estesi

## ⚙️ Configurazione Avanzata

### Logging

Configura il logging in `config/logconfig.xml`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="FileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="logs/server.log" />
    <appendToFile value="true" />
    <rollingStyle value="Size" />
    <maxSizeRollBackups value="10" />
    <maximumFileSize value="10MB" />
    <staticLogFileName value="true" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date{HH:mm:ss} | %-5level | %logger | %message%newline" />
    </layout>
  </appender>
  
  <root>
    <level value="INFO" />
    <appender-ref ref="FileAppender" />
  </root>
</log4net>
```

**Livelli di Log:**
- `DEBUG`: Informazioni dettagliate per debugging
- `INFO`: Informazioni generali
- `WARN`: Warning
- `ERROR`: Errori
- `FATAL`: Errori fatali

### Nomi Vietati

Configura nomi vietati in `config/invalidnames.txt`:

```
admin
gm
moderator
fuck
shit
# Aggiungi un nome per riga
```

### Performance Tuning

#### CPU Usage

```yaml
environment:
  CPU_USE: "8"  # Numero di core CPU disponibili
```

#### Database Autosave

```yaml
environment:
  DB_AUTOSAVE: "True"
  DB_AUTOSAVE_INTERVAL: "10"  # Minuti
```

**Raccomandazioni:**
- **Dev/Test**: 5 minuti
- **Produzione**: 10-15 minuti
- **High Traffic**: 15-20 minuti

#### .NET Garbage Collection

Aggiungi al docker-compose.yml:

```yaml
environment:
  DOTNET_gcServer: "1"              # Server GC mode
  DOTNET_GCHeapCount: "8"           # GC heaps (= CPU cores)
  DOTNET_GCConserveMemory: "0"      # 0-9, 0 = max performance
```

### Script Compilation

#### Disabilita in Produzione

Per migliori performance, compila gli script prima del deploy:

```bash
# Build locale
dotnet build "Dawn of Light.sln" -c Release

# Poi in docker-compose.yml
environment:
  ENABLE_COMPILATION: "False"
```

#### Assembly Aggiuntive

```yaml
environment:
  SCRIPT_ASSEMBLIES: "MyCustomAssembly.dll,AnotherAssembly.dll"
```

### Metriche e Monitoring

Abilita metriche OpenTelemetry:

```yaml
environment:
  METRICS_ENABLED: "True"
  METRICS_EXPORT_INTERVAL: "60s"  # Formato: 10s, 2m, 1h
  OTLP_ENDPOINT: "http://localhost:4317/"
```

Richiede un collector OpenTelemetry (Prometheus, Grafana, etc.).

## 🔒 Sicurezza

### Checklist Sicurezza

- [ ] Cambia password database di default
- [ ] Non esporre porta database (3306) pubblicamente
- [ ] Usa password forti per account GM
- [ ] Disabilita `AUTO_ACCOUNT_CREATION` in produzione
- [ ] Configura firewall correttamente
- [ ] Usa SSL/TLS per connessioni database remote
- [ ] Backup regolari del database
- [ ] Monitora log per attività sospette
- [ ] Limita privilegi GM solo a utenti fidati
- [ ] Usa `UID`/`GID` non-root in Docker

### Password Database Sicure

```bash
# Genera password sicura
openssl rand -base64 32

# Usa in docker-compose.yml
environment:
  DB_CONNECTION_STRING: "server=db;port=3306;database=opendaoc;userid=root;password=YOUR_SECURE_PASSWORD_HERE"
```

### Secrets Management

Per produzione, usa Docker secrets invece di variabili d'ambiente:

```yaml
secrets:
  db_password:
    file: ./secrets/db_password.txt

services:
  gameserver:
    secrets:
      - db_password
    environment:
      DB_PASSWORD_FILE: /run/secrets/db_password
```

## 📝 Esempi Configurazione

### Configurazione Dev/Test Locale

```yaml
environment:
  GAME_TYPE: "Test"
  SERVER_NAME: "Dev Server"
  DB_TYPE: "SQLITE"
  DB_CONNECTION_STRING: "Data Source=./dev.db;Version=3"
  AUTO_ACCOUNT_CREATION: "True"
  ENABLE_COMPILATION: "True"
  CPU_USE: "4"
  DB_AUTOSAVE_INTERVAL: "5"
```

### Configurazione Produzione PvP

```yaml
environment:
  GAME_TYPE: "PvP"
  SERVER_NAME: "OpenDAoC PvP"
  SERVER_NAME_SHORT: "ODAOC-PVP"
  DB_TYPE: "MYSQL"
  DB_CONNECTION_STRING: "server=db;port=3306;database=opendaoc;userid=opendaoc_user;password=SECURE_PASSWORD"
  AUTO_ACCOUNT_CREATION: "False"
  ENABLE_COMPILATION: "False"
  CPU_USE: "16"
  DB_AUTOSAVE_INTERVAL: "15"
  DETECT_REGION_IP: "True"
```

### Configurazione Produzione Normal

```yaml
environment:
  GAME_TYPE: "Normal"
  SERVER_NAME: "OpenDAoC Classic"
  SERVER_NAME_SHORT: "ODAOC"
  DB_TYPE: "MYSQL"
  DB_CONNECTION_STRING: "server=db;port=3306;database=opendaoc;userid=opendaoc_user;password=SECURE_PASSWORD"
  AUTO_ACCOUNT_CREATION: "False"
  ENABLE_COMPILATION: "False"
  CPU_USE: "16"
  DB_AUTOSAVE_INTERVAL: "15"
  REGION_IP: "your.public.ip.here"
```

## 🔍 Verifica Configurazione

### Controlla GAME_TYPE Attivo

```bash
docker logs opendaoc-server | grep -i "server rules"
```

Output atteso:
```
Found server rules for GST_PvP server type (standard PvP server rules).
```

### Controlla Connessione Database

```bash
docker logs opendaoc-server | grep -i "database"
```

### Controlla Porte in Ascolto

```bash
# Linux
netstat -tulpn | grep -E '10300|10400'

# Docker
docker exec opendaoc-server netstat -tulpn
```

## 📚 Riferimenti

- [SERVER_TYPES.md](SERVER_TYPES.md) - Dettagli modalità di gioco
- [DOCKER.md](DOCKER.md) - Guida Docker completa
- [DEVELOPMENT.md](DEVELOPMENT.md) - Guida sviluppo

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026
