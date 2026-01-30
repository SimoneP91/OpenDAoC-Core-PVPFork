# Docker & Deployment - OpenDAoC

Guida completa al deployment di OpenDAoC con Docker.

## 📋 Indice

- [Prerequisiti](#prerequisiti)
- [Quick Start](#quick-start)
- [Configurazione Docker](#configurazione-docker)
- [Build Locale vs Immagine Precompilata](#build-locale-vs-immagine-precompilata)
- [Docker Compose](#docker-compose)
- [Comandi Utili](#comandi-utili)
- [Gestione Database](#gestione-database)
- [Troubleshooting](#troubleshooting)
- [Produzione](#produzione)

## 🔧 Prerequisiti

### Software Richiesto

- **Docker Engine** 20.10+ ([Installa Docker](https://docs.docker.com/get-docker/))
- **Docker Compose** 2.0+ (incluso in Docker Desktop)
- **Git** per clonare i repository

### Verifica Installazione

```bash
docker --version
# Docker version 24.0.0 o superiore

docker compose version
# Docker Compose version v2.20.0 o superiore
```

### Requisiti Sistema

**Minimi:**
- CPU: 2 core
- RAM: 4 GB
- Disco: 10 GB liberi

**Consigliati:**
- CPU: 4+ core
- RAM: 8+ GB
- Disco: 20+ GB liberi

**Produzione:**
- CPU: 8+ core
- RAM: 16+ GB
- Disco: 50+ GB SSD

## 🚀 Quick Start

### 1. Clone Repository

```bash
git clone https://github.com/OpenDAoC/OpenDAoC-Core.git
cd OpenDAoC-Core
```

### 2. Configura docker-compose.yml

Modifica le variabili d'ambiente nel file `docker-compose.yml`:

```yaml
environment:
  GAME_TYPE: "PvP"  # Cambia in Normal, PvE, etc.
  SERVER_NAME: "Il Mio Server"
  # ... altre configurazioni
```

### 3. Avvia i Container

```bash
docker compose up -d
```

### 4. Verifica

```bash
# Controlla i log
docker logs -f opendaoc-server

# Verifica il tipo di server
docker logs opendaoc-server | grep -i "server rules"
```

Output atteso:
```
Found server rules for GST_PvP server type (standard PvP server rules).
```

## 🐳 Configurazione Docker

### Dockerfile

Il `Dockerfile` di OpenDAoC usa un build multi-stage:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY . .
RUN dotnet build DOLLinux.sln -c Release

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /build/Release /app
ENTRYPOINT ["/bin/sh", "/app/entrypoint.sh"]
```

**Vantaggi:**
- Immagine finale leggera (solo runtime)
- Build riproducibile
- Layer caching per build veloci

### entrypoint.sh

Lo script `entrypoint.sh` gestisce:

1. **Creazione utente non-root** con UID/GID specificati
2. **Generazione dinamica** di `serverconfig.xml` da variabili d'ambiente
3. **Avvio del server** come utente non-privilegiato

```bash
#!/bin/sh
# Crea utente con UID/GID custom
addgroup -g "${APP_GID}" appgroup
adduser -D -H -u "${APP_UID}" -G appgroup appuser

# Genera serverconfig.xml da variabili d'ambiente
cat << EOF > /app/config/serverconfig.xml
<?xml version="1.0" encoding="utf-8"?>
<root>
    <Server>
        <GameType>${GAME_TYPE}</GameType>
        <!-- ... altre configurazioni ... -->
    </Server>
</root>
EOF

# Avvia server come appuser
exec su-exec appuser sh -c "cd /app && dotnet CoreServer.dll"
```

## 🏗️ Build Locale vs Immagine Precompilata

### Immagine Precompilata (Default)

Usa l'immagine ufficiale da GitHub Container Registry:

```yaml
services:
  gameserver:
    image: ghcr.io/opendaoc/opendaoc-core:latest
```

**Vantaggi:**
- ✅ Setup veloce
- ✅ Nessun build necessario
- ✅ Immagine testata e stabile

**Svantaggi:**
- ❌ Non include modifiche locali al codice
- ❌ Dipende da release ufficiali

### Build Locale

Builda l'immagine dal codice locale:

```yaml
services:
  gameserver:
    build:
      context: .
      dockerfile: Dockerfile
```

**Vantaggi:**
- ✅ Include modifiche locali al codice
- ✅ Controllo completo sulla build
- ✅ Ideale per sviluppo

**Svantaggi:**
- ❌ Build iniziale lenta (~5-10 minuti)
- ❌ Richiede più spazio disco

### Quando Usare Quale?

| Scenario | Usa |
|----------|-----|
| Server di gioco standard | Immagine precompilata |
| Sviluppo e testing | Build locale |
| Modifiche al codice sorgente | Build locale |
| Produzione senza modifiche | Immagine precompilata |
| Produzione con modifiche | Build locale |

## 📦 Docker Compose

### File docker-compose.yml Completo

```yaml
networks:
  opendaoc-network:
    driver: bridge

volumes:
  opendaoc-db-data:
  base-db:

services:
  db:
    image: mariadb:10.6
    container_name: opendaoc-db
    stdin_open: true
    tty: true
    command: --default-authentication-plugin=mysql_native_password --lower_case_table_names=1 --character-set-server=utf8mb3 --collation-server=utf8mb3_general_ci
    restart: always
    environment:
      MYSQL_DATABASE: opendaoc
      MYSQL_ROOT_PASSWORD: your-secure-password-here
    volumes:
      - opendaoc-db-data:/var/lib/mysql
      - base-db:/docker-entrypoint-initdb.d
    networks:
      - opendaoc-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5

  gameserver:
    build:
      context: .
      dockerfile: Dockerfile
    # Oppure usa immagine precompilata:
    # image: ghcr.io/opendaoc/opendaoc-core:latest
    container_name: opendaoc-server
    stdin_open: true
    tty: true
    ports:
      - "10300:10300"   # TCP - Server principale
      - "10400:10400"   # UDP - Region server
    depends_on:
      db:
        condition: service_healthy
    environment:
      UID: "1000"
      GID: "1000"
      AUTO_ACCOUNT_CREATION: "True"
      CHEAT_LOGGER_NAME: "cheats"
      CPU_USE: "8"
      DB_AUTOSAVE: "True"
      DB_AUTOSAVE_INTERVAL: "10"
      DB_CONNECTION_STRING: "server=db;port=3306;database=opendaoc;userid=root;password=your-secure-password-here;treattinyasboolean=true"
      DB_TYPE: "MYSQL"
      DETECT_REGION_IP: "True"
      ENABLE_COMPILATION: "True"
      ENABLE_UPNP: "False"
      GAME_TYPE: "PvP"
      GM_ACTION_LOGGER_NAME: "gmactions"
      INVALID_NAMES_FILE: "./config/invalidnames.txt"
      LOG_CONFIG_FILE: "./config/logconfig.xml"
      REGION_IP: "0.0.0.0"
      REGION_PORT: "10400"
      SCRIPT_ASSEMBLIES: ""
      SCRIPT_COMPILATION_TARGET: "./lib/GameServerScripts.dll"
      SERVER_IP: "0.0.0.0"
      SERVER_NAME: "OpenDAoC"
      SERVER_NAME_SHORT: "OPENDAOC"
      SERVER_PORT: "10300"
      UDP_IP: "0.0.0.0"
      UDP_PORT: "10400"
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT: "False"
    volumes:
      - base-db:/tmp/opendaoc-db
    networks:
      - opendaoc-network
    restart: unless-stopped
```

### Componenti Principali

#### Networks

```yaml
networks:
  opendaoc-network:
    driver: bridge
```

Network isolato per comunicazione tra container.

#### Volumes

```yaml
volumes:
  opendaoc-db-data:  # Dati database persistenti
  base-db:           # Database iniziale
```

**Named volumes** per persistenza dati.

#### Services

**db**: Container MariaDB per il database
**gameserver**: Container OpenDAoC server

## 🎮 Comandi Utili

### Gestione Container

```bash
# Avvia tutti i servizi
docker compose up -d

# Ferma tutti i servizi
docker compose down

# Riavvia un servizio specifico
docker compose restart gameserver

# Ferma senza rimuovere container
docker compose stop

# Avvia container fermati
docker compose start

# Rimuovi tutto (container, network, volumes)
docker compose down -v
```

### Build e Deploy

```bash
# Build senza cache
docker compose build --no-cache

# Build e avvia
docker compose up -d --build

# Build solo gameserver
docker compose build gameserver

# Pull immagini più recenti
docker compose pull

# Rebuild completo
docker compose down
docker compose build --no-cache
docker compose up -d
```

### Logs e Debugging

```bash
# Visualizza log in tempo reale
docker logs -f opendaoc-server

# Ultimi 100 righe
docker logs --tail 100 opendaoc-server

# Log con timestamp
docker logs -t opendaoc-server

# Log di tutti i servizi
docker compose logs -f

# Filtra log
docker logs opendaoc-server | grep -i "error"
docker logs opendaoc-server | grep -i "server rules"
```

### Accesso ai Container

```bash
# Shell nel container gameserver
docker exec -it opendaoc-server sh

# Shell nel container database
docker exec -it opendaoc-db bash

# Esegui comando nel container
docker exec opendaoc-server ls -la /app

# MySQL client
docker exec -it opendaoc-db mysql -u root -p
```

### Monitoring

```bash
# Statistiche in tempo reale
docker stats

# Solo gameserver
docker stats opendaoc-server

# Info container
docker inspect opendaoc-server

# Processi in esecuzione
docker top opendaoc-server

# Spazio disco usato
docker system df
```

## 🗄️ Gestione Database

### Backup Database

```bash
# Backup completo
docker exec opendaoc-db mysqldump -u root -p[password] opendaoc > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup compresso
docker exec opendaoc-db mysqldump -u root -p[password] opendaoc | gzip > backup_$(date +%Y%m%d_%H%M%S).sql.gz

# Backup con Makefile (se disponibile)
make backup
```

### Restore Database

```bash
# Restore da file SQL
cat backup.sql | docker exec -i opendaoc-db mysql -u root -p[password] opendaoc

# Restore da file compresso
gunzip < backup.sql.gz | docker exec -i opendaoc-db mysql -u root -p[password] opendaoc

# Restore con Makefile
make restore
```

### Accesso MySQL

```bash
# MySQL client interattivo
docker exec -it opendaoc-db mysql -u root -p

# Esegui query diretta
docker exec opendaoc-db mysql -u root -p[password] opendaoc -e "SELECT COUNT(*) FROM account;"

# Export tabella specifica
docker exec opendaoc-db mysqldump -u root -p[password] opendaoc account > account_backup.sql
```

### Reset Database

```bash
# Ferma i servizi
docker compose down

# Rimuovi volume database
docker volume rm opendaoc_opendaoc-db-data

# Riavvia (database verrà ricreato)
docker compose up -d
```

⚠️ **Attenzione**: Questo cancella TUTTI i dati del database!

### Importa Database Iniziale

```bash
# Clone database repository
git clone https://github.com/OpenDAoC/OpenDAoC-Database.git

# Copia SQL nel volume
docker cp OpenDAoC-Database/opendaoc-db-core/combined.sql opendaoc-db:/tmp/

# Importa
docker exec opendaoc-db mysql -u root -p[password] opendaoc < /tmp/combined.sql
```

## 🔧 Troubleshooting

### Container non si avvia

```bash
# Controlla status
docker compose ps

# Controlla log per errori
docker logs opendaoc-server
docker logs opendaoc-db

# Verifica configurazione
docker compose config
```

### Errore "No services to build"

Significa che stai usando `image:` invece di `build:` nel docker-compose.yml.

**Soluzione**: Cambia da:
```yaml
image: ghcr.io/opendaoc/opendaoc-core:latest
```

A:
```yaml
build:
  context: .
  dockerfile: Dockerfile
```

### Database connection failed

```bash
# Verifica che db sia avviato
docker compose ps db

# Controlla log database
docker logs opendaoc-db

# Testa connessione
docker exec opendaoc-db mysqladmin -u root -p[password] ping

# Verifica network
docker network inspect opendaoc_opendaoc-network
```

### Porte già in uso

```bash
# Verifica porte in uso (Linux)
netstat -tulpn | grep -E '10300|10400'

# Verifica porte in uso (Windows)
netstat -ano | findstr "10300"

# Cambia porte in docker-compose.yml
ports:
  - "10301:10300"  # Usa porta 10301 invece di 10300
  - "10401:10400"
```

### Server in modalità sbagliata

```bash
# Verifica GAME_TYPE
docker logs opendaoc-server | grep -i "server rules"

# Se sbagliato, modifica docker-compose.yml
environment:
  GAME_TYPE: "PvP"  # Cambia qui

# Riavvia
docker compose down
docker compose up -d

# Verifica di nuovo
docker logs opendaoc-server | grep -i "server rules"
```

### Permessi file

```bash
# Verifica UID/GID
docker exec opendaoc-server id

# Cambia UID/GID in docker-compose.yml
environment:
  UID: "1000"  # Tuo UID
  GID: "1000"  # Tuo GID

# Trova il tuo UID/GID (Linux)
id -u  # UID
id -g  # GID
```

### Out of Memory

```bash
# Limita memoria container
services:
  gameserver:
    mem_limit: 4g
    mem_reservation: 2g
```

### Cleanup Spazio Disco

```bash
# Rimuovi container fermati
docker container prune

# Rimuovi immagini non usate
docker image prune -a

# Rimuovi volumi non usati
docker volume prune

# Cleanup completo
docker system prune -a --volumes
```

## 🚀 Produzione

### Best Practices

#### 1. Usa Immagini Taggate

```yaml
services:
  gameserver:
    image: ghcr.io/opendaoc/opendaoc-core:v1.2.3  # Non :latest
```

#### 2. Health Checks

```yaml
services:
  gameserver:
    healthcheck:
      test: ["CMD", "pgrep", "-f", "CoreServer.dll"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s
```

#### 3. Resource Limits

```yaml
services:
  gameserver:
    deploy:
      resources:
        limits:
          cpus: '8'
          memory: 8G
        reservations:
          cpus: '4'
          memory: 4G
```

#### 4. Restart Policy

```yaml
services:
  gameserver:
    restart: unless-stopped
  
  db:
    restart: always
```

#### 5. Logging

```yaml
services:
  gameserver:
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

#### 6. Secrets

```yaml
secrets:
  db_password:
    file: ./secrets/db_password.txt
  db_root_password:
    file: ./secrets/db_root_password.txt

services:
  db:
    secrets:
      - db_root_password
    environment:
      MYSQL_ROOT_PASSWORD_FILE: /run/secrets/db_root_password
```

### Monitoring Produzione

#### Prometheus + Grafana

```yaml
services:
  prometheus:
    image: prom/prometheus
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
    ports:
      - "9090:9090"
  
  grafana:
    image: grafana/grafana
    ports:
      - "3000:3000"
    environment:
      GF_SECURITY_ADMIN_PASSWORD: admin
```

#### cAdvisor

```yaml
services:
  cadvisor:
    image: gcr.io/cadvisor/cadvisor
    volumes:
      - /:/rootfs:ro
      - /var/run:/var/run:ro
      - /sys:/sys:ro
      - /var/lib/docker/:/var/lib/docker:ro
    ports:
      - "8080:8080"
```

### Backup Automatico

Script `backup.sh`:

```bash
#!/bin/bash
BACKUP_DIR="/backups"
DATE=$(date +%Y%m%d_%H%M%S)

# Backup database
docker exec opendaoc-db mysqldump -u root -p${DB_PASSWORD} opendaoc | gzip > ${BACKUP_DIR}/db_${DATE}.sql.gz

# Cleanup vecchi backup (mantieni ultimi 7 giorni)
find ${BACKUP_DIR} -name "db_*.sql.gz" -mtime +7 -delete

echo "Backup completato: db_${DATE}.sql.gz"
```

Cron job:

```bash
# Backup giornaliero alle 3 AM
0 3 * * * /path/to/backup.sh >> /var/log/opendaoc-backup.log 2>&1
```

### SSL/TLS per Database

```yaml
services:
  db:
    environment:
      MYSQL_ROOT_PASSWORD: ${DB_ROOT_PASSWORD}
    volumes:
      - ./ssl/ca.pem:/etc/mysql/ssl/ca.pem:ro
      - ./ssl/server-cert.pem:/etc/mysql/ssl/server-cert.pem:ro
      - ./ssl/server-key.pem:/etc/mysql/ssl/server-key.pem:ro
    command: >
      --ssl-ca=/etc/mysql/ssl/ca.pem
      --ssl-cert=/etc/mysql/ssl/server-cert.pem
      --ssl-key=/etc/mysql/ssl/server-key.pem
```

### Reverse Proxy (Nginx)

Per esporre metriche o web UI:

```yaml
services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./ssl:/etc/nginx/ssl:ro
    depends_on:
      - gameserver
```

## 📚 Riferimenti

- [CONFIGURATION.md](CONFIGURATION.md) - Configurazione completa
- [SERVER_TYPES.md](SERVER_TYPES.md) - Modalità di gioco
- [DEVELOPMENT.md](DEVELOPMENT.md) - Guida sviluppo

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026
