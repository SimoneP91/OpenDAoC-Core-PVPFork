# Comandi GM - OpenDAoC

Guida completa a tutti i comandi GM (Game Master) disponibili in OpenDAoC.

## 📋 Indice

- [Livelli di Privilegio](#livelli-di-privilegio)
- [Comandi Player Management](#comandi-player-management)
- [Comandi Level & XP](#comandi-level--xp)
- [Comandi Teleport & Movement](#comandi-teleport--movement)
- [Comandi NPC & Mob](#comandi-npc--mob)
- [Comandi Item](#comandi-item)
- [Comandi Server](#comandi-server)
- [Comandi Keep & RvR](#comandi-keep--rvr)
- [Comandi Debug](#comandi-debug)
- [Comandi Utility](#comandi-utility)

## 🔐 Livelli di Privilegio

| Livello | Nome | Descrizione |
|---------|------|-------------|
| 1 | Player | Giocatore normale |
| 2 | GM | Game Master |
| 3 | Admin | Amministratore |

## 👤 Comandi Player Management

### `/player` - Gestione Completa Player

**Privilege Level**: GM (2)

**Sintassi**:
```
/player name <newName>
/player lastname <change|reset> <newLastName>
/player level <newLevel>
/player levelup
/player reset
/player realm <newRealm>
/player inventory [wear|bag|vault|house|cons]
/player <rps|bps|xp|xpa|clxp|mlxp> <amount>
/player stat <typeofStat> <value>
/player money <copp|silv|gold|plat|mith> <amount>
/player respec <all|line|realm|dol|champion> <amount=1>
/player model <reset|[change]> <modelid>
/player friend <list|playerName>
/player <rez|kill> <albs|mids|hibs|self|all>
/player jump <group|guild|cg|bg> <name>
/player kick <all>
/player save <all>
/player purge
/player update
/player info
/player location
/player showgroup
/player showeffects
/player startchampion
/player clearchampion
/player respecchampion
/player saddlebags <0-15>
/player startml
/player setml <level>
/player setmlstep <level> <step> [false]
/player allchars <PlayerName>
/player class <list|classID|className>
/player areas
```

**Esempi**:
```
/player level 50                    # Porta il target a livello 50
/player levelup                     # Livello +1 al target
/player reset                       # Reset completo del player
/player money gold 1000             # Aggiungi 1000 gold
/player rps 100000                  # Aggiungi 100k realm points
/player respec all                  # Respec completo
/player kill self                   # Uccidi te stesso
/player rez albs                    # Resurrect tutti gli Albion
/player class list                  # Lista classi disponibili
/player class 11                    # Cambia classe (11 = Druid)
```

### `/account` - Gestione Account

**Privilege Level**: Admin (3)

**Sintassi**:
```
/account <accountname> <password> <plvl> <realm>
/account delete <accountname>
/account changepassword <accountname> <newpassword>
/account movechar <charname> <accountname>
```

### `/plvl` - Cambia Privilege Level

**Privilege Level**: Admin (3)

**Sintassi**:
```
/plvl <target> <newPrivLevel>
```

## 📊 Comandi Level & XP

### `/player level` - Cambia Livello

**Sintassi**:
```
/player level <1-255>
```

**Esempi**:
```
/player level 50        # Livello 50
/player level 1         # Livello 1 (reset)
```

### `/player levelup` - Level Up Singolo

**Sintassi**:
```
/player levelup
```

Aumenta di 1 livello il target (o mezzo livello se > 40).

### `/player xp` - Aggiungi XP

**Sintassi**:
```
/player xp <amount>
/player xpa <amount>     # XP alternativo
/player clxp <amount>    # Champion XP
/player mlxp <amount>    # Master Level XP
```

**Esempi**:
```
/player xp 1000000      # 1M XP
/player clxp 50000      # 50k Champion XP
```

### `/player rps` - Realm Points

**Sintassi**:
```
/player rps <amount>
/player bps <amount>    # Bounty Points
```

**Esempi**:
```
/player rps 1000000     # 1M realm points
/player bps 50000       # 50k bounty points
```

## 🚀 Comandi Teleport & Movement

### `/jump` - Teleport

**Privilege Level**: GM (2)

**Sintassi**:
```
/jump <x> <y> <z>
/jump <playerName>
/jump <regionID> <x> <y> <z>
/jump to <playerName>
/jump rel <xOffset> <yOffset> <zOffset>
```

**Esempi**:
```
/jump 500000 500000 5000           # Coordinate assolute
/jump PlayerName                    # Teleport a un player
/jump to PlayerName                 # Teleport a un player
/jump rel 100 0 0                   # 100 unità avanti
/jump 1 500000 500000 5000         # Region 1 + coordinate
```

### `/debugjump` - Teleport Debug

**Privilege Level**: GM (2)

**Sintassi**:
```
/debugjump <x> <y> <z> [heading]
```

### `/offlinejump` - Teleport Player Offline

**Privilege Level**: GM (2)

**Sintassi**:
```
/offlinejump <charName> <regionID> <x> <y> <z>
```

Teleporta un personaggio anche se offline.

### `/player jump` - Teleport Gruppi

**Sintassi**:
```
/player jump group <playerName>     # Porta tutto il gruppo
/player jump guild <guildName>      # Porta tutta la gilda
/player jump cg <playerName>        # Porta il chat group
/player jump bg <playerName>        # Porta il battle group
```

## 👾 Comandi NPC & Mob

### `/mob` - Gestione Mob

**Privilege Level**: GM (2)

**Sintassi**:
```
/mob create <mobID>
/mob model <modelID>
/mob name <name>
/mob guild <guildName>
/mob level <level>
/mob realm <0|1|2|3>
/mob speed <speed>
/mob brain <brainType>
/mob aggro <level>
/mob range <range>
/mob distance <distance>
/mob damagetype <type>
/mob movehere
/mob remove
/mob copy
/mob save
/mob info
/mob equiptemplate <templateID>
/mob equipinfo
/mob addloot <itemTemplateID> <chance>
/mob viewloot
/mob removeloot <slot>
/mob refreshloot
/mob kill
/mob regen
/mob ghost
/mob transparent
/mob fly
/mob noname
/mob notarget
/mob peace
/mob stealth
/mob torch
```

**Esempi**:
```
/mob create 1                       # Crea mob template 1
/mob level 50                       # Livello 50
/mob name "Boss Dragon"             # Nome
/mob brain StandardMobBrain         # AI brain
/mob addloot sword_template 50     # 50% drop chance
/mob save                           # Salva in DB
```

### `/npc` - Gestione NPC

**Privilege Level**: GM (2)

**Sintassi**:
```
/npc create <classtype>
/npc model <modelID>
/npc size <size>
/npc name <name>
/npc guild <guildName>
/npc level <level>
/npc realm <0|1|2|3>
/npc flags <flags>
/npc equiptemplate <templateID>
/npc movehere
/npc remove
/npc save
/npc info
```

### `/spammob` - Spam Mob Creation

**Privilege Level**: GM (2)

**Sintassi**:
```
/spammob create <amount>
/spammob create <amount> <radius>
/spammob remove <radius>
```

**Esempi**:
```
/spammob create 100                 # 100 mob alla tua posizione
/spammob create 50 1000            # 50 mob in raggio 1000
/spammob remove 2000               # Rimuovi mob in raggio 2000
```

## 🎒 Comandi Item

### `/item` - Gestione Item

**Privilege Level**: GM (2)

**Sintassi**:
```
/item create <itemTemplateID>
/item create <itemTemplateID> <count>
/item info
/item model <modelID>
/item name <name>
/item color <color>
/item effect <effect>
/item type <type>
/item maxcharge <charges>
/item charge <charges>
/item realm <realm>
/item level <level>
/item quality <quality>
/item durability <durability>
/item condition <condition>
/item bonus <bonusLevel>
/item save
/item delete
/item findid <itemName>
/item count <templateID>
/item mass <templateID> <count>
```

**Esempi**:
```
/item create sword_template         # Crea item
/item create sword_template 10      # Crea 10 item
/item level 51                      # Item livello 51
/item quality 100                   # Qualità 100%
/item save                          # Salva modifiche
/item mass sword_template 100       # Crea 100 spade
```

### `/clearinventory` - Pulisci Inventario

**Privilege Level**: GM (2)

**Sintassi**:
```
/clearinventory
```

Rimuove tutti gli item dall'inventario del target.

## 🏰 Comandi Keep & RvR

### `/keep` - Gestione Keep

**Privilege Level**: GM (2)

**Sintassi**:
```
/keep create <keepID>
/keep realm <0|1|2|3>
/keep level <level>
/keep name <name>
/keep guild <guildName>
/keep remove
/keep save
/keep info
```

### `/keepguard` - Gestione Keep Guard

**Privilege Level**: GM (2)

**Sintassi**:
```
/keepguard create <classtype>
/keepguard position <slot>
/keepguard remove
/keepguard save
```

### `/keepcomponent` - Gestione Keep Component

**Privilege Level**: GM (2)

**Sintassi**:
```
/keepcomponent create <skin>
/keepcomponent skin <skinID>
/keepcomponent delete
/keepcomponent save
```

### `/gmrelic` - Gestione Reliquie

**Privilege Level**: GM (2)

**Sintassi**:
```
/gmrelic create <relicID>
/gmrelic remove
/gmrelic respawn
```

### `/minorelic` - Mini Reliquie

**Privilege Level**: GM (2)

**Sintassi**:
```
/minorelic create
/minorelic remove
```

## 🖥️ Comandi Server

### `/broadcast` - Messaggio Globale

**Privilege Level**: GM (2)

**Sintassi**:
```
/broadcast <message>
/b <message>
```

Invia messaggio a tutti i player online.

### `/announce` - Annuncio

**Privilege Level**: GM (2)

**Sintassi**:
```
/announce <message>
```

### `/team` - Messaggio Team

**Privilege Level**: GM (2)

**Sintassi**:
```
/team <message>
/te <message>
```

Messaggio a tutti i GM/Admin online.

### `/adviceteam` - Messaggio Advice Team

**Privilege Level**: GM (2)

**Sintassi**:
```
/adviceteam <message>
/at <message>
```

### `/shutdown` - Spegni Server

**Privilege Level**: Admin (3)

**Sintassi**:
```
/shutdown
/shutdown <seconds>
```

**Esempi**:
```
/shutdown           # Shutdown immediato
/shutdown 300       # Shutdown in 5 minuti
```

### `/serverproperties` - Server Properties

**Privilege Level**: Admin (3)

**Sintassi**:
```
/serverproperties
/serverproperties <propertyName>
/serverproperties <propertyName> <value>
```

**Esempi**:
```
/serverproperties                           # Lista tutte
/serverproperties XP_RATE                   # Mostra XP_RATE
/serverproperties XP_RATE 2.0              # Imposta XP_RATE a 2x
```

## 🔍 Comandi Debug

### `/debug` - Debug Mode

**Privilege Level**: GM (2)

**Sintassi**:
```
/debug on
/debug off
```

### `/gminfo` - Info GM

**Privilege Level**: GM (2)

**Sintassi**:
```
/gminfo
```

Mostra informazioni dettagliate sul target.

### `/clientlist` - Lista Client

**Privilege Level**: GM (2)

**Sintassi**:
```
/clientlist
```

Mostra tutti i client connessi.

### `/snoop` - Spia Chat

**Privilege Level**: Admin (3)

**Sintassi**:
```
/snoop <playerName>
```

Monitora tutti i messaggi di un player.

### `/speedhack` - Controlla Speed Hack

**Privilege Level**: Admin (3)

**Sintassi**:
```
/speedhack <playerName>
```

## 🛠️ Comandi Utility

### `/god` - God Mode

**Privilege Level**: GM (2)

**Sintassi**:
```
/god
```

Invincibilità on/off.

### `/invisible` - Invisibilità

**Privilege Level**: GM (2)

**Sintassi**:
```
/invisible
/invis
```

### `/gmstealth` - GM Stealth

**Privilege Level**: GM (2)

**Sintassi**:
```
/gmstealth
```

Stealth permanente GM.

### `/speed` - Cambia Velocità

**Privilege Level**: GM (2)

**Sintassi**:
```
/speed <speed>
```

**Esempi**:
```
/speed 200          # Velocità 200%
/speed 1000         # Velocità 1000%
```

### `/heal` - Heal

**Privilege Level**: GM (2)

**Sintassi**:
```
/heal
/heal <playerName>
/heal <amount>
```

### `/harm` - Damage

**Privilege Level**: GM (2)

**Sintassi**:
```
/harm <amount>
```

### `/rez` - Resurrect

**Privilege Level**: GM (2)

**Sintassi**:
```
/rez
/rez <playerName>
```

### `/kill` - Kill

**Privilege Level**: GM (2)

**Sintassi**:
```
/kill
/kill <playerName>
```

### `/kick` - Kick Player

**Privilege Level**: GM (2)

**Sintassi**:
```
/kick <playerName>
/kick <playerName> <reason>
```

### `/ban` - Ban Player

**Privilege Level**: Admin (3)

**Sintassi**:
```
/ban account <accountName> <duration> <reason>
/ban ip <ipAddress> <duration> <reason>
/ban unban account <accountName>
/ban unban ip <ipAddress>
```

**Esempi**:
```
/ban account BadPlayer 7d Cheating        # Ban 7 giorni
/ban ip 192.168.1.1 30d Hacking          # Ban IP 30 giorni
/ban unban account BadPlayer              # Unban
```

### `/mute` - Mute Player

**Privilege Level**: GM (2)

**Sintassi**:
```
/mute <playerName> <duration>
/unmute <playerName>
```

**Esempi**:
```
/mute BadPlayer 1h          # Mute 1 ora
/unmute BadPlayer           # Unmute
```

### `/freeze` - Freeze Player

**Privilege Level**: Admin (3)

**Sintassi**:
```
/freeze <playerName>
/unfreeze <playerName>
```

### `/morph` - Cambia Modello

**Privilege Level**: GM (2)

**Sintassi**:
```
/morph <modelID>
/morph reset
```

**Esempi**:
```
/morph 100          # Modello 100
/morph reset        # Reset modello
```

### `/mountgm` - Mount GM

**Privilege Level**: GM (2)

**Sintassi**:
```
/mountgm
```

Mount speciale GM (veloce).

### `/cast` - Cast Spell

**Privilege Level**: GM (2)

**Sintassi**:
```
/cast <spellID>
/cast <spellID> <target>
```

### `/area` - Gestione Aree

**Privilege Level**: GM (2)

**Sintassi**:
```
/area create <radius> <description>
/area remove
/area info
```

### `/door` - Gestione Porte

**Privilege Level**: GM (2)

**Sintassi**:
```
/door create <doorID>
/door model <modelID>
/door name <name>
/door remove
/door save
```

### `/object` - Gestione Oggetti Statici

**Privilege Level**: GM (2)

**Sintassi**:
```
/object create <modelID>
/object model <modelID>
/object name <name>
/object emblem <emblem>
/object remove
/object save
/object copy
/object movehere
```

### `/merchant` - Gestione Merchant

**Privilege Level**: GM (2)

**Sintassi**:
```
/merchant create
/merchant articles list <page>
/merchant articles add <slot> <itemTemplateID> <page>
/merchant articles remove <slot> <page>
/merchant articles delete <page>
/merchant save
```

### `/path` - Gestione Path

**Privilege Level**: GM (2)

**Sintassi**:
```
/path create <pathID>
/path add
/path save
```

### `/addbind` - Aggiungi Bind Point

**Privilege Level**: GM (2)

**Sintassi**:
```
/addbind
```

Aggiunge un bind point alla posizione corrente.

### `/faction` - Gestione Faction

**Privilege Level**: GM (2)

**Sintassi**:
```
/faction assign <factionID>
/faction create <factionID>
/faction delete <factionID>
```

### `/crafting` - Crafting

**Privilege Level**: GM (2)

**Sintassi**:
```
/crafting <skill> <points>
```

**Esempi**:
```
/crafting weaponcraft 1000      # 1000 weaponcraft
/crafting armorcraft 1000       # 1000 armorcraft
```

### `/house` - Gestione Case

**Privilege Level**: GM (2)

**Sintassi**:
```
/house edit
/house remove
/house removeall
```

### `/removehouse` - Rimuovi Casa

**Privilege Level**: GM (2)

**Sintassi**:
```
/removehouse <houseNumber>
```

### `/instance` - Gestione Istanze

**Privilege Level**: GM (2)

**Sintassi**:
```
/instance create <instanceID>
/instance destroy <instanceID>
/instance movehere <instanceID>
```

### `/zone` - Gestione Zone

**Privilege Level**: GM (2)

**Sintassi**:
```
/zone info
/zone divingflag <0|1|2>
/zone waterlevel <level>
/zone bonus <zoneID|current> <xpBonus> <rpBonus> <bpBonus> <coinBonus> <save>
```

### `/zonebonus` - Zone Bonus

**Privilege Level**: GM (2)

**Sintassi**:
```
/zonebonus
/zb
```

Cambia la zona bonus casualmente.

### `/weather` - Meteo

**Privilege Level**: GM (2)

**Sintassi**:
```
/weather info
/weather start <line> <duration> <speed> <diffusion> <intensity>
/weather stop
```

### `/blink` - Blink

**Privilege Level**: GM (2)

**Sintassi**:
```
/blink
```

Teleport rapido nella direzione in cui guardi.

### `/walk` - Fai Camminare NPC

**Privilege Level**: GM (2)

**Sintassi**:
```
/walk <xoff> <yoff> <zoff> <speed>
```

### `/stop` - Ferma NPC

**Privilege Level**: GM (2)

**Sintassi**:
```
/stop
```

### `/resetability` - Reset Abilità

**Privilege Level**: GM (2)

**Sintassi**:
```
/resetability
```

### `/speclock` - Lock Spec Modifier

**Privilege Level**: GM (2)

**Sintassi**:
```
/speclock <value>
/speclock reset
```

**Esempi**:
```
/speclock 1.5       # 150% spec modifier
/speclock reset     # Reset normale
```

### `/translate` - Traduzioni

**Privilege Level**: GM (2)

**Sintassi**:
```
/translate add <Language> <TranslationId> <Text>
/translate debug
```

### `/titlegm` - Gestione Titoli

**Privilege Level**: GM (2)

**Sintassi**:
```
/titlegm add <classType>
/titlegm remove <classType>
```

### `/siegeweapon` - Armi d'Assedio

**Privilege Level**: GM (2)

**Sintassi**:
```
/siegeweapon create <type>
```

**Tipi**: miniram, lightram, mediumram, heavyram, catapult, ballista, cauldron, trebuchet

### `/keepdoorteleport` - Teleport Porta Keep

**Privilege Level**: Admin (3)

**Sintassi**:
```
/keepdoorteleport
```

### `/addhookpoint` - Aggiungi Hook Point

**Privilege Level**: GM (2)

**Sintassi**:
```
/addhookpoint <hookpointID>
```

### `/benchmark` - Benchmark

**Privilege Level**: Admin (3)

**Sintassi**:
```
/benchmark
```

## 📝 Note Importanti

### Comandi Disabilitati di Default

Alcuni comandi potrebbero essere disabilitati o richiedere configurazioni specifiche. Controlla i file di configurazione del server.

### Logging

Tutti i comandi GM vengono loggati nel file `gmactions.log` per audit.

### Sicurezza

- Non condividere mai il tuo account GM
- Usa comandi con cautela, specialmente `/player kill all` o `/shutdown`
- Fai backup regolari prima di modifiche massive

### Alias Comuni

Molti comandi hanno alias brevi:
- `/b` = `/broadcast`
- `/te` = `/team`
- `/at` = `/adviceteam`
- `/zb` = `/zonebonus`
- `/zp` = `/zonepoint`

## 🔗 Riferimenti

- [CONFIGURATION.md](CONFIGURATION.md) - Configurazione server
- [SERVER_TYPES.md](SERVER_TYPES.md) - Modalità server
- [DEVELOPMENT.md](DEVELOPMENT.md) - Sviluppo custom commands

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026  
**Compatibilità**: OpenDAoC-Core latest

**Nota**: Per usare questi comandi devi avere un account con `PrivLevel` >= 2 (GM). Usa `/plvl` (come Admin) per cambiare il privilege level di un account.
