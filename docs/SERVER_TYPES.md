# Server Types - OpenDAoC

Guida completa alle modalità di gioco (Server Types) disponibili in OpenDAoC.

## 📋 Indice

- [Panoramica](#panoramica)
- [Normal Server](#normal-server)
- [PvP Server](#pvp-server)
- [PvE Server](#pve-server)
- [Roleplay Server](#roleplay-server)
- [Casual Server](#casual-server)
- [Test Server](#test-server)
- [Confronto Modalità](#confronto-modalità)
- [Configurazione](#configurazione)

## 🎮 Panoramica

OpenDAoC supporta 6 diverse modalità di server, ognuna con regole e meccaniche specifiche. La modalità viene impostata tramite la variabile `GAME_TYPE`.

### Enum EGameServerType

```csharp
public enum EGameServerType
{
    GST_Normal = 0,      // Server standard
    GST_Test = 1,        // Server di test
    GST_PvP = 2,         // Player vs Player
    GST_PvE = 3,         // Player vs Environment
    GST_Roleplay = 4,    // Roleplay
    GST_Casual = 5,      // Casual
    GST_Unknown = 6,     // Sconosciuto
}
```

### Come Funziona

Ogni modalità è implementata tramite una classe `ServerRules` che estende `AbstractServerRules`:

```
GameServer/serverrules/
├── AbstractServerRules.cs      # Classe base
├── NormalServerRules.cs        # Regole Normal
├── PvPServerRules.cs           # Regole PvP
├── PvEServerRules.cs           # Regole PvE
└── ...
```

Il server carica automaticamente le regole corrette all'avvio basandosi su `GAME_TYPE`.

## 🛡️ Normal Server

**Modalità classica di Dark Age of Camelot con regole RvR standard.**

### Caratteristiche

- ✅ **RvR (Realm vs Realm)** - Combattimento tra realm nelle Frontiers
- ✅ **Protezione stesso realm** - Non puoi attaccare membri del tuo realm
- ✅ **Duelli** - Sistema duelli tra giocatori consenzienti
- ✅ **Keep Warfare** - Assedi e difesa fortezze
- ✅ **Relics** - Sistema reliquie di realm
- ✅ **Dungeons PvE** - Contenuto PvE completo
- ❌ **No PvP in safe zones** - Zone sicure rispettate

### Regole di Combattimento

```csharp
// Da NormalServerRules.cs
public override bool IsAllowedToAttack(GameLiving attacker, GameLiving defender, bool quiet)
{
    // Non puoi attaccare te stesso
    if (attacker == defender)
        return false;
    
    // Non puoi attaccare membri dello stesso realm
    if (attacker.Realm == defender.Realm)
    {
        // Eccezione: duelli
        if (attacker is GamePlayer player && player.IsDuelPartner(defender))
            return true;
        
        return false;
    }
    
    return true;
}
```

### Zone PvP

- **Frontiers** (RvR zones): PvP attivo
- **Darkness Falls**: PvP attivo
- **Città e zone PvE**: PvP disabilitato (tranne duelli)

### Configurazione

```yaml
environment:
  GAME_TYPE: "Normal"
```

### Ideale Per

- Server classici fedeli a DAoC 1.65
- Community RvR-focused
- Esperienza autentica Dark Age of Camelot

## ⚔️ PvP Server

**Full PvP con sistema di immunità temporanee.**

### Caratteristiche

- ✅ **PvP ovunque** - Combattimento possibile in tutte le zone
- ✅ **Cross-realm PvP** - Attacca qualsiasi giocatore di altro realm
- ✅ **Timer immunità** - Protezione temporanea dopo morte/teleport
- ✅ **Safety flag** - Protezione per low-level (< livello 10)
- ✅ **Keep & Relics** - Sistema completo RvR
- ⚠️ **High risk/reward** - Esperienza più intensa

### Sistema Immunità

#### Timer Dopo Morte

```csharp
// Da PvPServerRules.cs
public override void OnReleased(DOLEvent e, object sender, EventArgs args)
{
    GamePlayer player = (GamePlayer)sender;
    
    if (player.TempProperties.GetProperty<string>(KILLED_BY_PLAYER_PROP) != null)
    {
        // Ucciso da player: 120 secondi immunità (default)
        StartImmunityTimer(player, ServerProperties.Properties.TIMER_KILLED_BY_PLAYER * 1000);
    }
    else
    {
        // Ucciso da mob: 60 secondi immunità (default)
        StartImmunityTimer(player, ServerProperties.Properties.TIMER_KILLED_BY_MOB * 1000);
    }
}
```

#### Timer Dopo Teleport

```csharp
public override void OnPlayerTeleport(GamePlayer player, GameLocation source, DbTeleport destination)
{
    // Teleport nella stessa region: 30 secondi immunità (default)
    if (source.RegionID == destination.RegionID)
    {
        StartImmunityTimer(player, ServerProperties.Properties.TIMER_PVP_TELEPORT * 1000);
    }
}
```

### Configurazione Timer

Modifica nel database `serverproperty`:

```sql
-- Timer immunità dopo morte da player (secondi)
UPDATE serverproperty SET value='120' WHERE keyname='TIMER_KILLED_BY_PLAYER';

-- Timer immunità dopo morte da mob (secondi)
UPDATE serverproperty SET value='60' WHERE keyname='TIMER_KILLED_BY_MOB';

-- Timer immunità dopo teleport (secondi)
UPDATE serverproperty SET value='30' WHERE keyname='TIMER_PVP_TELEPORT';
```

### Safety Flag

Giocatori sotto livello 10 possono usare `/safety` per protezione aggiuntiva:

```
/safety on   # Attiva protezione
/safety off  # Disattiva protezione
```

Quando attivo:
- Non puoi essere attaccato da altri player
- Non puoi attaccare altri player
- Disabilitato automaticamente al livello 10

### Zone Comportamento

| Zona | PvP | Note |
|------|-----|------|
| Città | ✅ | PvP attivo ma sconsigliato |
| Zone PvE | ✅ | PvP sempre attivo |
| Frontiers | ✅ | PvP full |
| Dungeons | ✅ | PvP attivo |
| Istanze | ⚠️ | Dipende dall'istanza |

### Configurazione

```yaml
environment:
  GAME_TYPE: "PvP"
```

### Ideale Per

- Server hardcore PvP
- Community competitive
- Giocatori che amano il rischio costante
- Server "full loot" (con modifiche custom)

## 🐉 PvE Server

**Focus su contenuto PvE con PvP consensuale.**

### Caratteristiche

- ✅ **PvE primario** - Focus su mob, dungeon, raid
- ✅ **PvP consensuale** - Solo duelli e arene
- ✅ **Cooperazione cross-realm** - Possibilità di gruppi misti (con modifiche)
- ✅ **Protezioni extra** - Sicurezza maggiore per player
- ❌ **No RvR forzato** - Keep e relics opzionali
- ❌ **No ganking** - Nessun PvP non consensuale

### Regole di Combattimento

```csharp
// Da PvEServerRules.cs
public override bool IsAllowedToAttack(GameLiving attacker, GameLiving defender, bool quiet)
{
    // Solo attacchi PvE o duelli consenzienti
    if (defender is GamePlayer)
    {
        // Permetti solo duelli
        if (attacker is GamePlayer player && player.IsDuelPartner(defender))
            return true;
        
        // Blocca tutti gli altri attacchi PvP
        return false;
    }
    
    // Permetti attacchi PvE
    return base.IsAllowedToAttack(attacker, defender, quiet);
}
```

### Zone Comportamento

| Zona | PvP | Note |
|------|-----|------|
| Tutte le zone | ❌ | Solo duelli |
| Arene | ✅ | PvP consensuale |
| Frontiers | ⚠️ | Opzionale, configurabile |

### Configurazione

```yaml
environment:
  GAME_TYPE: "PvE"
```

### Modifiche Consigliate

Per server PvE puro, considera:

```sql
-- Disabilita keep warfare
UPDATE serverproperty SET value='False' WHERE keyname='ENABLE_KEEPS';

-- Aumenta XP PvE
UPDATE serverproperty SET value='2.0' WHERE keyname='XP_RATE';

-- Riduci difficoltà mob (opzionale)
UPDATE serverproperty SET value='0.8' WHERE keyname='MOB_DAMAGE_MULTIPLIER';
```

### Ideale Per

- Server cooperativi
- Community PvE-focused
- Giocatori casual
- Server family-friendly

## 🎭 Roleplay Server

**Server con regole roleplay enforced.**

### Caratteristiche

- ✅ **Naming policy stretta** - Nomi lore-friendly obbligatori
- ✅ **Comportamento IC** - In-character richiesto
- ✅ **Eventi RP** - Focus su eventi roleplay
- ✅ **Regole RvR standard** - Come Normal server
- ⚠️ **Moderazione attiva** - Staff monitora comportamenti

### Regole Aggiuntive

1. **Nomi**: Devono essere lore-appropriate
   - ✅ Corretto: "Aethelred", "Bjorn", "Fionn"
   - ❌ Sbagliato: "XxDarkKillerxX", "Legolas", "Batman"

2. **Chat**: Comportamento in-character in chat pubbliche
   - `/say`, `/yell`, `/emote`: Solo IC
   - `/guild`, `/group`: Permesso OOC
   - `/broadcast`: Solo IC

3. **Comportamento**: Azioni coerenti con il personaggio

### Configurazione

```yaml
environment:
  GAME_TYPE: "Roleplay"
  INVALID_NAMES_FILE: "./config/invalidnames_strict.txt"
```

File `invalidnames_strict.txt` più restrittivo:

```
# Nomi vietati per RP server
admin
gm
mod
# Nomi da altre franchise
legolas
gandalf
frodo
batman
superman
# Nomi non-lore
killer
death
dark
shadow
# etc...
```

### Ideale Per

- Community roleplay
- Server immersivi
- Giocatori che amano il lore
- Eventi narrativi

## 🎲 Casual Server

**Server casual-friendly con bonus e regole rilassate.**

### Caratteristiche

- ✅ **XP bonus** - Leveling più veloce
- ✅ **Drop rate aumentato** - Più loot
- ✅ **Regole rilassate** - Meno restrizioni
- ✅ **Quality of life** - Miglioramenti QoL
- ✅ **RvR standard** - Come Normal server
- ✅ **Friendly per nuovi player**

### Modifiche Tipiche

```sql
-- XP bonus 2x
UPDATE serverproperty SET value='2.0' WHERE keyname='XP_RATE';

-- Drop rate aumentato 1.5x
UPDATE serverproperty SET value='1.5' WHERE keyname='LOOT_RATE';

-- Realm points bonus
UPDATE serverproperty SET value='1.5' WHERE keyname='RP_RATE';

-- Riduzione death penalty
UPDATE serverproperty SET value='0.5' WHERE keyname='DEATH_XP_LOSS_PERCENT';

-- Aumento velocità mount
UPDATE serverproperty SET value='1.2' WHERE keyname='MOUNT_SPEED_MULTIPLIER';
```

### Configurazione

```yaml
environment:
  GAME_TYPE: "Casual"
```

### Ideale Per

- Server casual
- Giocatori con poco tempo
- Server "fun"
- Testing veloce di contenuti

## 🧪 Test Server

**Server per sviluppo e testing.**

### Caratteristiche

- ✅ **Comandi GM estesi** - Tutti i comandi disponibili
- ✅ **Regole variabili** - Configurabile a piacere
- ✅ **Debug mode** - Logging esteso
- ✅ **Instant level** - Comandi per level up rapido
- ✅ **Item spawning** - Creazione item facilitata
- ⚠️ **Non per produzione** - Solo sviluppo

### Comandi Aggiuntivi

```
/gmrelevel 50              # Setta livello a 50
/item create [template]    # Crea item
/mob create [template]     # Crea NPC
/jump [location]           # Teleport
/speed [value]             # Cambia velocità
/god                       # Modalità invincibile
/invisible                 # Invisibilità
```

### Configurazione

```yaml
environment:
  GAME_TYPE: "Test"
  ENABLE_COMPILATION: "True"  # Sempre true per test
  AUTO_ACCOUNT_CREATION: "True"
  # Logging esteso
  LOG_LEVEL: "DEBUG"
```

### Ideale Per

- Sviluppo features
- Testing modifiche
- Debug problemi
- Training GM staff

## 📊 Confronto Modalità

### Tabella Comparativa

| Feature | Normal | PvP | PvE | Roleplay | Casual | Test |
|---------|--------|-----|-----|----------|--------|------|
| **RvR Frontiers** | ✅ | ✅ | ⚠️ | ✅ | ✅ | ⚠️ |
| **PvP in PvE zones** | ❌ | ✅ | ❌ | ❌ | ❌ | ⚠️ |
| **Duelli** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Keep Warfare** | ✅ | ✅ | ⚠️ | ✅ | ✅ | ⚠️ |
| **Relics** | ✅ | ✅ | ⚠️ | ✅ | ✅ | ⚠️ |
| **Same Realm Attack** | ❌ | ❌ | ❌ | ❌ | ❌ | ⚠️ |
| **Immunity Timers** | ❌ | ✅ | N/A | ❌ | ❌ | ⚠️ |
| **Safety Flag** | ❌ | ✅ | N/A | ❌ | ❌ | ⚠️ |
| **XP Bonus** | 1x | 1x | 1x | 1x | 2x+ | ⚠️ |
| **Naming Policy** | Normal | Normal | Normal | Strict | Normal | None |
| **RP Enforced** | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| **GM Commands** | Limited | Limited | Limited | Limited | Limited | Full |

**Legenda:**
- ✅ = Abilitato/Supportato
- ❌ = Disabilitato
- ⚠️ = Configurabile/Variabile
- N/A = Non applicabile

### Difficoltà

| Modalità | Difficoltà | Curva Apprendimento | Competitività |
|----------|-----------|---------------------|---------------|
| Normal | Media | Media | Alta |
| PvP | Alta | Alta | Molto Alta |
| PvE | Bassa | Bassa | Bassa |
| Roleplay | Media | Media | Media |
| Casual | Bassa | Bassa | Media |
| Test | N/A | N/A | N/A |

### Player Base Tipica

| Modalità | Tipo Giocatori | Dimensione Community | Retention |
|----------|---------------|---------------------|-----------|
| Normal | Veterani DAoC | Media-Grande | Alta |
| PvP | Hardcore PvP | Piccola-Media | Media |
| PvE | Casual/Coop | Media | Alta |
| Roleplay | RP enthusiasts | Piccola | Molto Alta |
| Casual | Casual players | Grande | Media |
| Test | Developers | Molto Piccola | N/A |

## ⚙️ Configurazione

### Cambiare Modalità

#### Docker Compose

```yaml
environment:
  GAME_TYPE: "PvP"  # Cambia qui: Normal, PvP, PvE, Roleplay, Casual, Test
```

#### serverconfig.xml

```xml
<GameType>PvP</GameType>
```

#### Verifica Modalità Attiva

```bash
docker logs opendaoc-server | grep -i "server rules"
```

Output:
```
Found server rules for GST_PvP server type (standard PvP server rules).
```

### Riavvio Necessario

⚠️ **Importante**: Dopo aver cambiato `GAME_TYPE`, devi riavviare il server:

```bash
docker compose down
docker compose up -d
```

### Modalità Custom

Puoi creare modalità custom estendendo `AbstractServerRules`:

```csharp
[ServerRules(EGameServerType.GST_Custom)]
public class CustomServerRules : AbstractServerRules
{
    public override string RulesDescription()
    {
        return "My custom server rules";
    }
    
    public override bool IsAllowedToAttack(GameLiving attacker, GameLiving defender, bool quiet)
    {
        // Implementa logica custom
        return base.IsAllowedToAttack(attacker, defender, quiet);
    }
}
```

## 🎯 Raccomandazioni

### Per Nuovo Server

1. **Community piccola/nuova**: Inizia con **Casual** o **PvE**
2. **Community veterana**: Usa **Normal**
3. **Focus PvP**: Usa **PvP**
4. **Focus RP**: Usa **Roleplay**
5. **Testing**: Usa **Test**

### Per Migrare Modalità

⚠️ **Attenzione**: Cambiare modalità su server esistente può causare problemi:

- Player abituati a certe regole
- Bilanciamento diverso
- Possibili exploit

**Raccomandazione**: Annuncia con anticipo e fai backup completo.

### Combinazioni Popolari

#### Server Classico
```yaml
GAME_TYPE: "Normal"
XP_RATE: "1.0"
RP_RATE: "1.0"
```

#### Server Casual PvE
```yaml
GAME_TYPE: "PvE"
XP_RATE: "2.0"
LOOT_RATE: "1.5"
```

#### Server Hardcore PvP
```yaml
GAME_TYPE: "PvP"
TIMER_KILLED_BY_PLAYER: "60"  # Ridotto
DEATH_XP_LOSS_PERCENT: "10"   # Aumentato
```

## 📚 Riferimenti

- [CONFIGURATION.md](CONFIGURATION.md) - Configurazione dettagliata
- [DOCKER.md](DOCKER.md) - Deployment Docker
- [DEVELOPMENT.md](DEVELOPMENT.md) - Sviluppo custom rules

### File Sorgente

- `CoreBase/Enums/EGameServerType.cs` - Enum tipi server
- `GameServer/serverrules/AbstractServerRules.cs` - Classe base
- `GameServer/serverrules/NormalServerRules.cs` - Regole Normal
- `GameServer/serverrules/PvPServerRules.cs` - Regole PvP
- `GameServer/serverrules/PvEServerRules.cs` - Regole PvE

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026
