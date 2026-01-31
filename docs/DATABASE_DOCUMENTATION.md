# 📚 OpenDAoC Database - Documentazione Completa

**Versione Database**: MariaDB 10.6.24  
**Engine**: InnoDB  
**Charset**: utf8mb3 / utf8mb4  
**Autore**: Analisi approfondita del dump SQL e codice sorgente  
**Data**: 30 Gennaio 2026

---

## 📋 Indice

1. [Introduzione](#introduzione)
2. [Struttura Generale](#struttura-generale)
3. [Tabelle Account e Autenticazione](#tabelle-account-e-autenticazione)
4. [Tabelle Personaggi](#tabelle-personaggi)
5. [Tabelle Inventario e Items](#tabelle-inventario-e-items)
6. [Tabelle Mondo e Geografia](#tabelle-mondo-e-geografia)
7. [Tabelle PvP e RvR](#tabelle-pvp-e-rvr)
8. [Tabelle Guild e Social](#tabelle-guild-e-social)
9. [Tabelle Quest e Progressione](#tabelle-quest-e-progressione)
10. [Tabelle Crafting](#tabelle-crafting)
11. [Tabelle Housing](#tabelle-housing)
12. [Tabelle Sistema](#tabelle-sistema)
13. [Relazioni Parent-Child](#relazioni-parent-child)
14. [Operazioni Comuni](#operazioni-comuni)
15. [Best Practices](#best-practices)

---

## 🎯 Introduzione

Il database OpenDAoC è il cuore del server di gioco. Contiene tutte le informazioni necessarie per gestire:
- Account e personaggi dei giocatori
- Mondo di gioco (regioni, zone, NPC, mob)
- Sistema di combattimento e progressione
- Economia (items, crafting, merchant)
- Sistema sociale (guild, housing)
- PvP/RvR (realm points, keeps, relics)

### Caratteristiche Principali

- **Auto-incrementing IDs**: Molte tabelle usano `AUTO_INCREMENT` per generare ID univoci
- **Timestamp Tracking**: Campo `LastTimeRowUpdated` presente in quasi tutte le tabelle
- **Soft Deletes**: Alcune tabelle non cancellano fisicamente i dati ma li marcano come inattivi
- **Relazioni Lazy/Eager**: Il codice C# usa attributi `[Relation]` per definire relazioni tra tabelle

---

## 🏗️ Struttura Generale

### Convenzioni di Naming

- **Tabelle**: Lowercase con underscore (es. `account`, `dolcharacters`)
- **Primary Keys**: Solitamente `{TableName}_ID` (es. `Account_ID`, `Character_ID`)
- **Foreign Keys**: Riferimenti espliciti con suffisso `_ID` o nome della tabella parent
- **Indici**: Prefisso `I_` per indici normali, `U_` per unique

### Tipi di Dati Comuni

```sql
-- Stringhe
varchar(255)        -- Nomi, ID brevi
text                -- Descrizioni, dati lunghi

-- Numeri
int(11)            -- ID, contatori
tinyint(1)         -- Boolean (0/1)
smallint(5)        -- Numeri piccoli (region ID, level)
bigint(20)         -- Denaro, grandi contatori

-- Date
datetime           -- Timestamp con formato 'YYYY-MM-DD HH:MM:SS'
```

---

## 👤 Tabelle Account e Autenticazione

### `account` - Account Utente

**Scopo**: Gestisce gli account dei giocatori e le loro credenziali.

**Struttura**:
```sql
CREATE TABLE `account` (
  `Name` varchar(255) NOT NULL,              -- Username (PRIMARY KEY)
  `Password` text NOT NULL,                  -- Password (MD5 o ##cleartext)
  `CreationDate` datetime NOT NULL,          -- Data creazione account
  `LastLogin` datetime DEFAULT NULL,         -- Ultimo login
  `Realm` int(11) NOT NULL DEFAULT 0,        -- Realm preferito (0=none, 1=Alb, 2=Mid, 3=Hib)
  `PrivLevel` int(10) unsigned NOT NULL,     -- Livello privilegi (1=Player, 2=GM, 3=Admin)
  `Status` int(11) NOT NULL DEFAULT 0,       -- Status account (0=attivo, 1=banned, etc)
  `Mail` text DEFAULT NULL,                  -- Email
  `LastLoginIP` varchar(255) DEFAULT NULL,   -- Ultimo IP di connessione
  `IsMuted` tinyint(1) NOT NULL DEFAULT 0,   -- Account mutato
  `IsWarned` tinyint(1) NOT NULL DEFAULT 0,  -- Account avvisato
  `IsTester` tinyint(1) NOT NULL DEFAULT 0,  -- Accesso PTR
  `DiscordID` text DEFAULT NULL,             -- ID Discord collegato
  `Realm_Timer_Realm` int(11) NOT NULL,      -- Realm timer per cambio realm
  `Realm_Timer_Last_Combat` datetime,        -- Ultimo combattimento PvP
  PRIMARY KEY (`Name`)
);
```

**Relazioni**:
- **Parent di**: `dolcharacters` (1:N) - Un account può avere più personaggi
- **Parent di**: `ban` (1:N) - Un account può avere più ban storici
- **Parent di**: `accountxcustomparam` (1:N) - Parametri custom per account

**Cosa Modifica nel Gioco**:
- ✅ **Login**: Credenziali per accedere al server
- ✅ **Privilegi GM**: `PrivLevel` determina i comandi disponibili (1=player, 2=GM, 3=Admin)
- ✅ **Realm Timer**: Previene il cambio realm troppo frequente dopo combattimento PvP
- ✅ **Mute/Ban**: Controllo moderazione
- ✅ **Realm Preferito**: Determina quale realm vedere al login

**Operazioni Comuni**:
```sql
-- Creare nuovo account
INSERT INTO account (Name, Password, CreationDate, PrivLevel, Realm) 
VALUES ('username', '##password', NOW(), 1, 0);

-- Promuovere a GM
UPDATE account SET PrivLevel = 2 WHERE Name = 'username';

-- Mutare un account
UPDATE account SET IsMuted = 1 WHERE Name = 'spammer';

-- Verificare ultimo login
SELECT Name, LastLogin, LastLoginIP FROM account 
WHERE LastLogin > DATE_SUB(NOW(), INTERVAL 7 DAY);
```

---

### `accountxcrafting` - Crafting per Account

**Scopo**: Memorizza le skill di crafting per account (condivise tra personaggi dello stesso realm).

**Struttura**:
```sql
CREATE TABLE `accountxcrafting` (
  `AccountId` varchar(255) NOT NULL,
  `Realm` int(11) NOT NULL DEFAULT 0,                    -- 1=Alb, 2=Mid, 3=Hib
  `SerializedCraftingSkills` text DEFAULT NULL,          -- "1|500;2|400;..." (SkillID|Level)
  `CraftingPrimarySkill` int(11) NOT NULL DEFAULT 0,     -- Skill primaria
  PRIMARY KEY (`AccountxCrafting_ID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Crafting Skills**: Tutti i personaggi dello stesso realm condividono le skill di crafting
- ✅ **Primary Skill**: Determina quale craft può essere portato oltre 1000

**Formato SerializedCraftingSkills**:
```
"1|500;2|400;3|300"
1 = Weaponcraft (500 skill)
2 = Armorcraft (400 skill)
3 = Tailoring (300 skill)
```

---

### `accountxmoney` - Denaro Account

**Scopo**: Vault condiviso di denaro tra personaggi dello stesso account e realm.

**Struttura**:
```sql
CREATE TABLE `accountxmoney` (
  `AccountId` varchar(255) NOT NULL,
  `Realm` int(11) NOT NULL DEFAULT 0,
  `Copper` int(11) NOT NULL DEFAULT 0,
  `Silver` int(11) NOT NULL DEFAULT 0,
  `Gold` int(11) NOT NULL DEFAULT 0,
  `Platinum` int(11) NOT NULL DEFAULT 0,
  `Mithril` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`AccountxMoney_ID`)
);
```

**Conversione Valuta**:
```
1 Mithril = 1000 Platinum
1 Platinum = 1000 Gold
1 Gold = 100 Silver
1 Silver = 100 Copper
```

---

### `ban` - Ban e Sanzioni

**Scopo**: Traccia i ban applicati agli account.

**Struttura**:
```sql
CREATE TABLE `ban` (
  `Author` text NOT NULL,           -- GM che ha applicato il ban
  `Type` varchar(255) NOT NULL,     -- "Account", "IP", "Both"
  `Ip` varchar(255) NOT NULL,       -- IP bannato
  `Account` varchar(255) NOT NULL,  -- Account bannato
  `DateBan` datetime NOT NULL,      -- Data del ban
  `Reason` text NOT NULL,           -- Motivazione
  PRIMARY KEY (`Ban_ID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Login Block**: Account bannati non possono accedere
- ✅ **IP Block**: IP bannati non possono creare nuovi account

---

## 🎮 Tabelle Personaggi

### `dolcharacters` - Personaggi Giocatori

**Scopo**: La tabella più importante - contiene tutti i dati dei personaggi.

**Struttura Principale** (semplificata - ha oltre 100 colonne):
```sql
CREATE TABLE `dolcharacters` (
  `DOLCharacters_ID` varchar(100) NOT NULL,  -- PRIMARY KEY (GUID)
  `AccountName` varchar(255) NOT NULL,       -- FK -> account.Name
  `Name` varchar(255) NOT NULL,              -- Nome personaggio (UNIQUE)
  `Level` int(11) NOT NULL DEFAULT 1,        -- Livello (1-50)
  `Class` int(11) NOT NULL DEFAULT 0,        -- Classe (eCharacterClass enum)
  `Realm` int(11) NOT NULL DEFAULT 0,        -- 1=Albion, 2=Midgard, 3=Hibernia
  `Race` int(11) NOT NULL DEFAULT 0,         -- Razza (eRace enum)
  `Gender` int(11) NOT NULL DEFAULT 0,       -- 0=Male, 1=Female
  
  -- Posizione
  `Region` smallint(5) unsigned NOT NULL,    -- ID Regione
  `Xpos` int(11) NOT NULL DEFAULT 0,         -- Coordinata X
  `Ypos` int(11) NOT NULL DEFAULT 0,         -- Coordinata Y
  `Zpos` int(11) NOT NULL DEFAULT 0,         -- Coordinata Z
  `Direction` smallint(5) unsigned NOT NULL, -- Heading (0-4095)
  
  -- Bind Point
  `BindRegion` smallint(5) unsigned NOT NULL,
  `BindXpos` int(11) NOT NULL DEFAULT 0,
  `BindYpos` int(11) NOT NULL DEFAULT 0,
  `BindZpos` int(11) NOT NULL DEFAULT 0,
  `BindHeading` smallint(5) unsigned NOT NULL,
  
  -- Stats Base
  `Strength` int(11) NOT NULL DEFAULT 0,
  `Dexterity` int(11) NOT NULL DEFAULT 0,
  `Constitution` int(11) NOT NULL DEFAULT 0,
  `Quickness` int(11) NOT NULL DEFAULT 0,
  `Intelligence` int(11) NOT NULL DEFAULT 0,
  `Piety` int(11) NOT NULL DEFAULT 0,
  `Empathy` int(11) NOT NULL DEFAULT 0,
  `Charisma` int(11) NOT NULL DEFAULT 0,
  
  -- Progressione
  `Experience` bigint(20) NOT NULL DEFAULT 0,
  `RealmPoints` bigint(20) NOT NULL DEFAULT 0,
  `BountyPoints` bigint(20) NOT NULL DEFAULT 0,
  `SkillSpecialtyPoints` int(11) NOT NULL,
  `RealmSpecialtyPoints` int(11) NOT NULL,
  
  -- Denaro
  `Copper` bigint(20) NOT NULL DEFAULT 0,
  `Silver` bigint(20) NOT NULL DEFAULT 0,
  `Gold` bigint(20) NOT NULL DEFAULT 0,
  `Platinum` bigint(20) NOT NULL DEFAULT 0,
  `Mithril` bigint(20) NOT NULL DEFAULT 0,
  
  -- Aspetto
  `CustomisationStep` int(11) NOT NULL DEFAULT 0,
  `EyeSize` int(11) NOT NULL DEFAULT 0,
  `LipSize` int(11) NOT NULL DEFAULT 0,
  `EyeColor` int(11) NOT NULL DEFAULT 0,
  `HairColor` int(11) NOT NULL DEFAULT 0,
  `FaceType` int(11) NOT NULL DEFAULT 0,
  `HairStyle` int(11) NOT NULL DEFAULT 0,
  `MoodType` int(11) NOT NULL DEFAULT 0,
  
  -- PvP
  `DeathCount` int(11) NOT NULL DEFAULT 0,
  `KillsAlbionPlayers` int(11) NOT NULL DEFAULT 0,
  `KillsMidgardPlayers` int(11) NOT NULL DEFAULT 0,
  `KillsHiberniaPlayers` int(11) NOT NULL DEFAULT 0,
  `KillsAlbionDeathBlows` int(11) NOT NULL DEFAULT 0,
  `KillsMidgardDeathBlows` int(11) NOT NULL DEFAULT 0,
  `KillsHiberniaDeathBlows` int(11) NOT NULL DEFAULT 0,
  
  -- Flags
  `IsLevelSecondStage` tinyint(1) NOT NULL DEFAULT 0,
  `IsLevelRespecUsed` tinyint(1) NOT NULL DEFAULT 0,
  `SafetyFlag` tinyint(1) NOT NULL DEFAULT 1,
  `IsCloakHoodUp` tinyint(1) NOT NULL DEFAULT 0,
  `IsAnonymous` tinyint(1) NOT NULL DEFAULT 0,
  
  PRIMARY KEY (`DOLCharacters_ID`),
  UNIQUE KEY `U_DOLCharacters_Name` (`Name`),
  KEY `I_DOLCharacters_AccountName` (`AccountName`)
);
```

**Relazioni**:
- **Child di**: `account` (N:1)
- **Parent di**: `inventoryitem` (1:N) - Inventario del personaggio
- **Parent di**: `characterxdataquest` (1:N) - Quest in corso
- **Parent di**: `characterxmasterlevel` (1:N) - Master Level progress

**Cosa Modifica nel Gioco**:
- ✅ **Tutto il personaggio**: Stats, livello, posizione, aspetto, denaro
- ✅ **Realm Points**: Determinano il Realm Rank (RR)
- ✅ **Bounty Points**: Valuta PvP per comprare items speciali
- ✅ **Skill Points**: Punti per specializzazioni
- ✅ **Safety Flag**: Protezione PvP per personaggi sotto livello 10
- ✅ **Bind Point**: Dove il personaggio respawna dopo la morte

**Operazioni Comuni**:
```sql
-- Trovare personaggi di un account
SELECT Name, Level, Class, Realm FROM dolcharacters 
WHERE AccountName = 'username';

-- Dare Realm Points
UPDATE dolcharacters SET RealmPoints = RealmPoints + 1000 
WHERE Name = 'PlayerName';

-- Teletrasportare un personaggio
UPDATE dolcharacters 
SET Region = 10, Xpos = 35564, Ypos = 27157, Zpos = 8448 
WHERE Name = 'PlayerName';

-- Reset specializzazioni
UPDATE dolcharacters 
SET SkillSpecialtyPoints = (Level - 5) * 20, 
    IsLevelRespecUsed = 1 
WHERE Name = 'PlayerName';
```

---

### `inventoryitem` - Inventario Personaggi

**Scopo**: Memorizza tutti gli items nell'inventario, equipaggiati o in vault.

**Struttura**:
```sql
CREATE TABLE `inventoryitem` (
  `InventoryItem_ID` varchar(255) NOT NULL,
  `OwnerID` varchar(100) DEFAULT NULL,         -- FK -> dolcharacters.DOLCharacters_ID
  `ObjectId` varchar(255) DEFAULT NULL,         -- ID univoco dell'item
  `Id_nb` varchar(255) NOT NULL,                -- Template ID (FK -> itemtemplate.Id_nb)
  `SlotPosition` int(11) NOT NULL DEFAULT 0,    -- Slot (0-1199)
  `Count` int(11) NOT NULL DEFAULT 0,           -- Stack size
  `Condition` int(11) NOT NULL DEFAULT 0,       -- Durabilità (0-100)
  `Durability` int(11) NOT NULL DEFAULT 0,      -- Durabilità massima
  `Quality` int(11) NOT NULL DEFAULT 0,         -- Qualità (0-100)
  `DPS_AF` int(11) NOT NULL DEFAULT 0,          -- DPS o AF
  `SPD_ABS` int(11) NOT NULL DEFAULT 0,         -- Speed o ABS
  `Charges` int(11) NOT NULL DEFAULT 0,         -- Cariche magiche
  `MaxCharges` int(11) NOT NULL DEFAULT 0,      -- Cariche massime
  PRIMARY KEY (`InventoryItem_ID`)
);
```

**Slot Positions**:
```
0-39    = Inventory (backpack)
40-79   = Vault 1
80-119  = Vault 2
120-159 = Vault 3
160-199 = Vault 4
600-699 = Consignment Merchant
700-799 = House Vault
1000+   = Equipped items

Equipped Slots:
10 = Right Hand
11 = Left Hand
12 = Two-Handed
13 = Ranged
21-28 = Armor slots (head, hands, feet, jewels, etc.)
29 = Cloak
32 = Mythirian
```

**Cosa Modifica nel Gioco**:
- ✅ **Inventario**: Tutti gli items del personaggio
- ✅ **Equipment**: Items equipaggiati e loro stats
- ✅ **Durabilità**: Condizione degli items
- ✅ **Vault**: Storage condiviso tra personaggi

---

## 🗺️ Tabelle Mondo e Geografia

### `region` - Regioni del Mondo

**Scopo**: Definisce le regioni di gioco (zone, dungeon, città).

**Struttura**:
```sql
CREATE TABLE `region` (
  `RegionID` smallint(5) unsigned NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` text DEFAULT NULL,
  `IP` varchar(100) NOT NULL DEFAULT 'any',
  `Port` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Expansion` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `IsDisabled` tinyint(1) NOT NULL DEFAULT 0,
  `WaterLevel` int(11) NOT NULL DEFAULT 0,
  `DivingFlag` tinyint(3) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`RegionID`)
);
```

**Region IDs Importanti**:
```
1   = Albion (Camelot Hills, Salisbury, etc.)
10  = Camelot City
100 = Midgard (Mularn, Svealand, etc.)
101 = Jordheim City
200 = Hibernia (Lough Derg, Connacht, etc.)
201 = Tir na Nog City
163 = Darkness Falls (dungeon cross-realm)
244 = New Frontiers (RvR zone)
```

**Cosa Modifica nel Gioco**:
- ✅ **Accesso Regioni**: `IsDisabled` blocca l'accesso
- ✅ **Expansion**: Determina quali expansion sono necessarie
- ✅ **Water Level**: Livello dell'acqua per nuoto/diving

---

### `zone` - Zone nelle Regioni

**Scopo**: Suddivide le regioni in zone più piccole.

**Struttura**:
```sql
CREATE TABLE `zone` (
  `ZoneID` smallint(5) unsigned NOT NULL,
  `RegionID` smallint(5) unsigned NOT NULL,
  `Description` text NOT NULL,
  `OffX` int(11) NOT NULL DEFAULT 0,
  `OffY` int(11) NOT NULL DEFAULT 0,
  `Width` int(11) NOT NULL DEFAULT 0,
  `Height` int(11) NOT NULL DEFAULT 0,
  `WaterLevel` int(11) NOT NULL DEFAULT 0,
  `IsLava` tinyint(1) NOT NULL DEFAULT 0,
  `BonusExperience` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `BonusRealmpoints` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `BonusBountypoints` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `BonusCoin` tinyint(3) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`ZoneID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Zone Bonus**: XP, RP, BP, Coin bonus per zona
- ✅ **Lava Zones**: Danno continuo se `IsLava = 1`
- ✅ **Coordinate**: Definisce i confini della zona

---

### `area` - Aree Speciali

**Scopo**: Definisce aree con comportamenti speciali (bind points, safe zones, etc.).

**Struttura**:
```sql
CREATE TABLE `area` (
  `Area_ID` varchar(255) NOT NULL,
  `Description` text NOT NULL,
  `X` int(11) NOT NULL DEFAULT 0,
  `Y` int(11) NOT NULL DEFAULT 0,
  `Z` int(11) NOT NULL DEFAULT 0,
  `Radius` int(11) NOT NULL DEFAULT 0,
  `Region` smallint(5) unsigned NOT NULL,
  `ClassType` text DEFAULT NULL,              -- Tipo di area (BindArea, SafeArea, etc.)
  `CanBroadcast` tinyint(1) NOT NULL DEFAULT 0,
  `Sound` tinyint(3) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`Area_ID`)
);
```

**Tipi di Area**:
- `DOL.GS.Area+BindArea` - Permette di bindare
- `DOL.GS.Area+SafeArea` - Zona sicura (no PvP)
- `DOL.GS.Area+Circle` - Area circolare
- `DOL.GS.Area+Square` - Area quadrata

---

### `mob` - Mob e NPC

**Scopo**: Definisce tutti i mob e NPC del mondo.

**Struttura**:
```sql
CREATE TABLE `mob` (
  `Mob_ID` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Guild` varchar(255) DEFAULT NULL,
  `Model` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Level` tinyint(3) unsigned NOT NULL DEFAULT 1,
  `Size` smallint(5) unsigned NOT NULL DEFAULT 50,
  `Realm` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `Faction` text DEFAULT NULL,
  `X` int(11) NOT NULL DEFAULT 0,
  `Y` int(11) NOT NULL DEFAULT 0,
  `Z` int(11) NOT NULL DEFAULT 0,
  `Heading` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Region` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Speed` smallint(6) NOT NULL DEFAULT 0,
  `Flags` int(10) unsigned NOT NULL DEFAULT 0,
  `RespawnInterval` int(11) NOT NULL DEFAULT -1,  -- Tempo respawn in millisecondi
  `ClassType` varchar(200) DEFAULT NULL,          -- Classe C# custom
  `NPCTemplate` varchar(100) DEFAULT NULL,        -- FK -> npctemplate
  `EquipmentTemplateID` varchar(100) DEFAULT NULL,
  `PathID` varchar(255) DEFAULT NULL,             -- Percorso di pattuglia
  PRIMARY KEY (`Mob_ID`)
);
```

**Flags Comuni**:
```
0x01 = Peace (non attacca)
0x02 = Ghost (trasparente)
0x04 = Stealth (invisibile)
0x08 = Don't Show Name
0x10 = Statue (immobile)
```

**Cosa Modifica nel Gioco**:
- ✅ **Spawn Mob**: Posizione, livello, modello
- ✅ **Respawn Time**: `RespawnInterval` in ms (-1 = no respawn)
- ✅ **Faction**: Determina aggressività verso player
- ✅ **Loot**: Collegato tramite `mobxloottemplate`

---

## ⚔️ Tabelle PvP e RvR

### `keep` - Fortezze RvR

**Scopo**: Gestisce le fortezze conquistabili in RvR.

**Struttura**:
```sql
CREATE TABLE `keep` (
  `KeepID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `Region` smallint(5) unsigned NOT NULL,
  `X` int(11) NOT NULL DEFAULT 0,
  `Y` int(11) NOT NULL DEFAULT 0,
  `Z` int(11) NOT NULL DEFAULT 0,
  `Heading` smallint(5) unsigned NOT NULL,
  `Realm` tinyint(3) unsigned NOT NULL DEFAULT 0,  -- Realm che controlla il keep
  `Level` tinyint(3) unsigned NOT NULL DEFAULT 50,
  `KeepType` int(11) NOT NULL DEFAULT 0,
  `BaseLevel` tinyint(3) unsigned NOT NULL DEFAULT 50,
  PRIMARY KEY (`KeepID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Controllo Realm**: `Realm` indica chi possiede il keep
- ✅ **Livello Keep**: Determina la difficoltà di conquista
- ✅ **Bonus Realm**: Keep controllati danno bonus al realm

---

### `relic` - Reliquie RvR

**Scopo**: Gestisce le reliquie conquistabili (Strength, Power).

**Struttura**:
```sql
CREATE TABLE `relic` (
  `RelicID` int(11) NOT NULL AUTO_INCREMENT,
  `RelicType` int(11) NOT NULL DEFAULT 0,      -- 0=Strength, 1=Power
  `OriginalRealm` int(11) NOT NULL DEFAULT 0,  -- Realm originale
  `Realm` int(11) NOT NULL DEFAULT 0,          -- Realm attuale
  `Region` smallint(5) unsigned NOT NULL,
  `X` int(11) NOT NULL DEFAULT 0,
  `Y` int(11) NOT NULL DEFAULT 0,
  `Z` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`RelicID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Bonus Realm**: Reliquie danno +10% stats al realm
- ✅ **Posizione**: Dove si trova la reliquia

---

## 👥 Tabelle Guild e Social

### `guild` - Gilde

**Scopo**: Gestisce le gilde dei giocatori.

**Struttura**:
```sql
CREATE TABLE `guild` (
  `GuildID` varchar(255) NOT NULL,
  `GuildName` varchar(255) NOT NULL,
  `Realm` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `Emblem` int(11) NOT NULL DEFAULT 0,
  `GuildLevel` int(11) NOT NULL DEFAULT 0,
  `BountyPoints` bigint(20) NOT NULL DEFAULT 0,
  `RealmPoints` bigint(20) NOT NULL DEFAULT 0,
  `MeritPoints` bigint(20) NOT NULL DEFAULT 0,
  `AllianceID` varchar(255) DEFAULT NULL,
  `Motd` text DEFAULT NULL,
  `oMotd` text DEFAULT NULL,
  `Webpage` text DEFAULT NULL,
  `Email` text DEFAULT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `U_Guild_GuildName` (`GuildName`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Guild Name**: Nome della gilda (unico per server)
- ✅ **Guild Level**: Livello della gilda (determina benefici)
- ✅ **Bounty/Realm Points**: Punti accumulati dalla gilda
- ✅ **Alliance**: Collegamento ad alleanze

---

### `guildrank` - Rank nelle Gilde

**Scopo**: Definisce i rank e permessi nelle gilde.

**Struttura**:
```sql
CREATE TABLE `guildrank` (
  `GuildRank_ID` varchar(255) NOT NULL,
  `GuildID` varchar(255) NOT NULL,
  `RankLevel` tinyint(3) unsigned NOT NULL,
  `Title` varchar(255) NOT NULL,
  `GCHear` tinyint(1) NOT NULL DEFAULT 0,
  `GCSpeak` tinyint(1) NOT NULL DEFAULT 0,
  `OfficerHear` tinyint(1) NOT NULL DEFAULT 0,
  `OfficerSpeak` tinyint(1) NOT NULL DEFAULT 0,
  `Invite` tinyint(1) NOT NULL DEFAULT 0,
  `Promote` tinyint(1) NOT NULL DEFAULT 0,
  `Remove` tinyint(1) NOT NULL DEFAULT 0,
  `View` tinyint(1) NOT NULL DEFAULT 0,
  `Alli` tinyint(1) NOT NULL DEFAULT 0,
  `Emblem` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`GuildRank_ID`)
);
```

**Rank Levels**:
```
0 = Guild Leader
1-9 = Custom ranks
```

---

## 📜 Tabelle Quest e Progressione

### `dataquest` - Quest Dinamiche

**Scopo**: Sistema di quest configurabili tramite database.

**Struttura**:
```sql
CREATE TABLE `dataquest` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `StartType` tinyint(3) unsigned NOT NULL,     -- 0=NPC, 1=Item, 2=Area
  `StartName` varchar(100) NOT NULL,            -- Nome NPC/Item/Area
  `StartRegionID` smallint(5) unsigned NOT NULL,
  `Description` text DEFAULT NULL,
  `StepType` text DEFAULT NULL,                 -- Tipo di step (Kill, Collect, Interact)
  `StepText` text DEFAULT NULL,                 -- Testo per ogni step
  `TargetName` text DEFAULT NULL,               -- Target per ogni step
  `CollectItemTemplate` text DEFAULT NULL,      -- Item da collezionare
  `MaxCount` smallint(6) NOT NULL DEFAULT 0,    -- Quanti item/kill servono
  `MinLevel` tinyint(3) unsigned NOT NULL,
  `MaxLevel` tinyint(3) unsigned NOT NULL,
  `RewardMoney` text DEFAULT NULL,
  `RewardXP` text DEFAULT NULL,
  `RewardRP` text DEFAULT NULL,
  `RewardBP` text DEFAULT NULL,
  `OptionalRewardItemTemplates` text DEFAULT NULL,
  `FinalRewardItemTemplates` text DEFAULT NULL,
  PRIMARY KEY (`ID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Quest System**: Quest completamente configurabili
- ✅ **Rewards**: XP, denaro, items, RP, BP
- ✅ **Level Range**: Chi può prendere la quest

---

### `characterxdataquest` - Progress Quest

**Scopo**: Traccia il progresso dei personaggi nelle quest.

**Struttura**:
```sql
CREATE TABLE `characterxdataquest` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Character_ID` varchar(100) NOT NULL,
  `DataQuestID` int(11) NOT NULL,
  `Step` smallint(6) NOT NULL DEFAULT 0,        -- Step corrente
  `Count` smallint(6) NOT NULL DEFAULT 0,       -- Progresso step
  PRIMARY KEY (`ID`)
);
```

---

### `ability` - Abilità e Realm Abilities

**Scopo**: Definisce tutte le abilità disponibili nel gioco.

**Struttura**:
```sql
CREATE TABLE `ability` (
  `AbilityID` int(11) NOT NULL AUTO_INCREMENT,
  `KeyName` varchar(100) NOT NULL,              -- Identificatore unico
  `Name` varchar(255) NOT NULL,                 -- Nome visualizzato
  `InternalID` int(11) NOT NULL DEFAULT 0,      -- ID interno
  `Description` text NOT NULL,
  `IconID` int(11) NOT NULL,
  `Implementation` varchar(255) DEFAULT NULL,   -- Classe C# che implementa
  PRIMARY KEY (`AbilityID`),
  UNIQUE KEY `U_Ability_KeyName` (`KeyName`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Realm Abilities**: Abilità acquistabili con Realm Points
- ✅ **Class Abilities**: Abilità di classe
- ✅ **Implementation**: Collega al codice C#

---

## 🔨 Tabelle Crafting

### `crafteditem` - Ricette Crafting

**Scopo**: Definisce cosa si può craftare.

**Struttura**:
```sql
CREATE TABLE `crafteditem` (
  `CraftedItemID` varchar(255) NOT NULL,
  `Id_nb` varchar(255) NOT NULL,                -- Item risultante
  `CraftingLevel` int(11) NOT NULL,             -- Skill richiesta
  `CraftingSkillType` int(11) NOT NULL,         -- Tipo di craft (1=Weaponcraft, 2=Armorcraft, etc.)
  `MakeTemplated` tinyint(1) NOT NULL,          -- Se crea item templated
  PRIMARY KEY (`CraftedItemID`)
);
```

**Crafting Skill Types**:
```
1 = Weaponcraft
2 = Armorcraft
3 = Siegecraft
4 = Alchemy
5 = Metalworking
6 = Leatherworking
7 = Clothworking
8 = Gemcutting
9 = Herbcraft
10 = Tailoring
11 = Spellcrafting
12 = Woodworking
```

---

### `craftedxitem` - Ingredienti Crafting

**Scopo**: Definisce gli ingredienti necessari per craftare.

**Struttura**:
```sql
CREATE TABLE `craftedxitem` (
  `CraftedXItem_ID` varchar(255) NOT NULL,
  `CraftedItemId_nb` varchar(255) NOT NULL,     -- FK -> crafteditem
  `IngredientId_nb` text NOT NULL,              -- Item ingrediente
  `Count` int(11) NOT NULL DEFAULT 0,           -- Quantità necessaria
  PRIMARY KEY (`CraftedXItem_ID`)
);
```

**Esempio**:
```sql
-- Per craftare una "steel sword" servono:
-- 10x steel bars
-- 2x leather strips
-- 1x wooden handle
```

---

## 🏠 Tabelle Housing

### `dbhouse` - Case Giocatori

**Scopo**: Gestisce le case dei giocatori.

**Struttura**:
```sql
CREATE TABLE `dbhouse` (
  `HouseNumber` int(11) NOT NULL,
  `X` int(11) NOT NULL,
  `Y` int(11) NOT NULL,
  `Z` int(11) NOT NULL,
  `RegionID` smallint(5) unsigned NOT NULL,
  `Name` text DEFAULT NULL,
  `Model` int(11) NOT NULL DEFAULT 0,           -- Modello casa
  `OwnerID` text DEFAULT NULL,                  -- FK -> dolcharacters
  `LastPaid` datetime DEFAULT NULL,             -- Ultima volta che è stata pagata
  `KeptMoney` bigint(20) NOT NULL DEFAULT 0,    -- Denaro nel vault casa
  `GuildHouse` tinyint(1) NOT NULL DEFAULT 0,   -- Se è una guild house
  `HasConsignment` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`HouseNumber`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Ownership**: Chi possiede la casa
- ✅ **Rent**: Sistema di affitto mensile
- ✅ **Consignment**: Merchant per vendere items
- ✅ **Guild House**: Casa di gilda con vault condiviso

---

### `dbindooritem` - Oggetti Interni Casa

**Scopo**: Memorizza la decorazione interna delle case.

**Struttura**:
```sql
CREATE TABLE `dbindooritem` (
  `DBIndoorItem_ID` varchar(255) NOT NULL,
  `HouseNumber` int(11) NOT NULL,
  `Model` int(11) NOT NULL,
  `Position` int(11) NOT NULL,                  -- Slot posizione
  `X` int(11) NOT NULL,
  `Y` int(11) NOT NULL,
  `Rotation` int(11) NOT NULL,
  `Color` int(11) NOT NULL,
  `Size` int(11) NOT NULL,
  PRIMARY KEY (`DBIndoorItem_ID`)
);
```

---

## 🔧 Tabelle Sistema

### `serverproperty` - Configurazione Server

**Scopo**: Memorizza tutte le configurazioni del server.

**Struttura**:
```sql
CREATE TABLE `serverproperty` (
  `Property_ID` varchar(255) NOT NULL,
  `Category` varchar(255) NOT NULL,
  `Key` varchar(255) NOT NULL,
  `Description` text NOT NULL,
  `DefaultValue` text NOT NULL,
  `Value` text NOT NULL,
  PRIMARY KEY (`Property_ID`)
);
```

**Proprietà Importanti**:
```sql
-- Rates
rp_rate = 1.0              -- Moltiplicatore Realm Points
bp_rate = 1.0              -- Moltiplicatore Bounty Points
xp_rate = 1.0              -- Moltiplicatore Experience
money_drop = 1.0           -- Moltiplicatore denaro drop

-- PvP
pvp_damage_cap = 1.0       -- Cap danno PvP
safe_level = 10            -- Livello sotto cui safety flag funziona

-- Server
server_name = "OpenDAoC"
server_type = "PvP"        -- Normal, PvP, PvE, etc.
max_players = 2500

-- Crafting
craft_speed = 1.0
craft_xp_rate = 1.0
```

**Operazioni Comuni**:
```sql
-- Modificare RP rate
UPDATE serverproperty SET Value = '2.0' WHERE `Key` = 'rp_rate';

-- Modificare nome server
UPDATE serverproperty SET Value = 'My Server' WHERE `Key` = 'server_name';
```

---

### `startuplocation` - Spawn Point Nuovi Personaggi

**Scopo**: Definisce dove spawnano i nuovi personaggi.

**Struttura**:
```sql
CREATE TABLE `startuplocation` (
  `StartupLocation_ID` varchar(255) NOT NULL,
  `Region` smallint(5) unsigned NOT NULL,
  `XPos` int(11) NOT NULL,
  `YPos` int(11) NOT NULL,
  `ZPos` int(11) NOT NULL,
  `Heading` smallint(5) unsigned NOT NULL,
  `Realm` tinyint(3) unsigned NOT NULL,         -- 1=Alb, 2=Mid, 3=Hib
  `Class` int(11) NOT NULL DEFAULT 0,           -- 0=tutte le classi
  `Race` int(11) NOT NULL DEFAULT 0,            -- 0=tutte le razze
  PRIMARY KEY (`StartupLocation_ID`)
);
```

**Cosa Modifica nel Gioco**:
- ✅ **Spawn Iniziale**: Dove i nuovi personaggi iniziano
- ✅ **Per Classe/Razza**: Spawn diversi per classi/razze diverse

---

### `itemtemplate` - Template Items

**Scopo**: Definisce tutti i template degli items del gioco.

**Struttura** (semplificata - ha oltre 80 colonne):
```sql
CREATE TABLE `itemtemplate` (
  `ItemTemplate_ID` varchar(255) NOT NULL,
  `Id_nb` varchar(255) NOT NULL,                -- ID univoco template
  `Name` varchar(255) NOT NULL,
  `Level` int(11) NOT NULL DEFAULT 0,
  `Durability` int(11) NOT NULL DEFAULT 0,
  `MaxDurability` int(11) NOT NULL DEFAULT 0,
  `Condition` int(11) NOT NULL DEFAULT 0,
  `MaxCondition` int(11) NOT NULL DEFAULT 0,
  `Quality` int(11) NOT NULL DEFAULT 0,
  `DPS_AF` int(11) NOT NULL DEFAULT 0,          -- DPS per armi, AF per armature
  `SPD_ABS` int(11) NOT NULL DEFAULT 0,         -- Speed per armi, ABS per armature
  `Hand` int(11) NOT NULL DEFAULT 0,            -- 0=1H, 1=2H, 2=Left
  `Type_Damage` int(11) NOT NULL DEFAULT 0,     -- Tipo danno (Crush, Slash, Thrust)
  `Object_Type` int(11) NOT NULL DEFAULT 0,     -- Tipo oggetto (Sword, Shield, etc.)
  `Item_Type` int(11) NOT NULL DEFAULT 0,       -- Categoria (Weapon, Armor, Magical)
  `Color` int(11) NOT NULL DEFAULT 0,
  `Emblem` int(11) NOT NULL DEFAULT 0,
  `Effect` int(11) NOT NULL DEFAULT 0,
  `Model` int(11) NOT NULL DEFAULT 0,
  `Extension` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `Bonus` int(11) NOT NULL DEFAULT 0,           -- Bonus principale
  `Bonus1` int(11) NOT NULL DEFAULT 0,
  `Bonus2` int(11) NOT NULL DEFAULT 0,
  `Bonus3` int(11) NOT NULL DEFAULT 0,
  `Bonus4` int(11) NOT NULL DEFAULT 0,
  `Bonus5` int(11) NOT NULL DEFAULT 0,
  `Bonus6` int(11) NOT NULL DEFAULT 0,
  `Bonus7` int(11) NOT NULL DEFAULT 0,
  `Bonus8` int(11) NOT NULL DEFAULT 0,
  `Bonus9` int(11) NOT NULL DEFAULT 0,
  `Bonus10` int(11) NOT NULL DEFAULT 0,
  `Bonus1Type` int(11) NOT NULL DEFAULT 0,
  `Bonus2Type` int(11) NOT NULL DEFAULT 0,
  `Bonus3Type` int(11) NOT NULL DEFAULT 0,
  `Bonus4Type` int(11) NOT NULL DEFAULT 0,
  `Bonus5Type` int(11) NOT NULL DEFAULT 0,
  `Bonus6Type` int(11) NOT NULL DEFAULT 0,
  `Bonus7Type` int(11) NOT NULL DEFAULT 0,
  `Bonus8Type` int(11) NOT NULL DEFAULT 0,
  `Bonus9Type` int(11) NOT NULL DEFAULT 0,
  `Bonus10Type` int(11) NOT NULL DEFAULT 0,
  `IsPickable` tinyint(1) NOT NULL DEFAULT 0,
  `IsDropable` tinyint(1) NOT NULL DEFAULT 0,
  `IsTradable` tinyint(1) NOT NULL DEFAULT 0,
  `Price` bigint(20) NOT NULL DEFAULT 0,
  `MaxCount` int(11) NOT NULL DEFAULT 1,        -- Stack size massimo
  `PackSize` int(11) NOT NULL DEFAULT 1,
  `Charges` int(11) NOT NULL DEFAULT 0,
  `MaxCharges` int(11) NOT NULL DEFAULT 0,
  `SpellID` int(11) NOT NULL DEFAULT 0,
  `ProcSpellID` int(11) NOT NULL DEFAULT 0,
  `Realm` int(11) NOT NULL DEFAULT 0,
  `AllowedClasses` varchar(200) DEFAULT NULL,
  `CanUseEvery` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`ItemTemplate_ID`),
  UNIQUE KEY `U_ItemTemplate_Id_nb` (`Id_nb`)
);
```

**Object Types Comuni**:
```
1 = Generic Item
2 = Armor
3 = Weapon
4 = Magical
5 = Crafting Material
6 = Alchemy Tincture
7 = Money
8 = Poison
9 = Instrument
10 = Shield
11 = Arrow
12 = Bolt
13 = Thrown
```

**Bonus Types**:
```
1 = Strength
2 = Dexterity
3 = Constitution
4 = Quickness
5 = Intelligence
6 = Piety
7 = Empathy
8 = Charisma
10 = Hits
11 = Power
12 = Resist Body
13 = Resist Cold
14 = Resist Crush
15 = Resist Energy
16 = Resist Heat
17 = Resist Matter
18 = Resist Slash
19 = Resist Spirit
20 = Resist Thrust
```

---

## 🔗 Relazioni Parent-Child

### Gerarchia Account → Personaggi → Items

```
account (Name)
  ├─→ dolcharacters (AccountName)
  │     ├─→ inventoryitem (OwnerID)
  │     ├─→ characterxdataquest (Character_ID)
  │     ├─→ characterxmasterlevel (Character_ID)
  │     └─→ characterxonetimedrop (CharacterID)
  │
  ├─→ accountxcrafting (AccountId)
  ├─→ accountxmoney (AccountId)
  ├─→ accountxcustomparam (Name)
  └─→ ban (Account)
```

### Gerarchia Guild

```
guild (GuildID)
  ├─→ guildrank (GuildID)
  ├─→ dolcharacters (GuildID)
  └─→ guildalliance (AllianceID)
```

### Gerarchia Mondo

```
region (RegionID)
  ├─→ zone (RegionID)
  │     └─→ mob (Region)
  │
  ├─→ area (Region)
  ├─→ keep (Region)
  └─→ dbhouse (RegionID)
```

### Gerarchia Items

```
itemtemplate (Id_nb)
  ├─→ inventoryitem (Id_nb)
  ├─→ merchantitem (ItemListID)
  ├─→ crafteditem (Id_nb)
  └─→ loottemplate (ItemTemplateID)
```

### Gerarchia Crafting

```
crafteditem (CraftedItemID)
  └─→ craftedxitem (CraftedItemId_nb)
        └─→ itemtemplate (IngredientId_nb)
```

### Gerarchia Quest

```
dataquest (ID)
  └─→ characterxdataquest (DataQuestID)
        └─→ dolcharacters (Character_ID)
```

---

## 🛠️ Operazioni Comuni

### Gestione Account

```sql
-- Creare nuovo account
INSERT INTO account (Name, Password, CreationDate, PrivLevel, Realm) 
VALUES ('newuser', '##password123', NOW(), 1, 0);

-- Promuovere a GM
UPDATE account SET PrivLevel = 2 WHERE Name = 'username';

-- Bannare account
INSERT INTO ban (Author, Type, Ip, Account, DateBan, Reason, Ban_ID) 
VALUES ('AdminName', 'Account', '', 'baduser', NOW(), 'Cheating', UUID());

-- Vedere account online
SELECT a.Name, a.LastLogin, a.LastLoginIP, 
       COUNT(c.DOLCharacters_ID) as NumChars
FROM account a
LEFT JOIN dolcharacters c ON a.Name = c.AccountName
WHERE a.LastLogin > DATE_SUB(NOW(), INTERVAL 1 HOUR)
GROUP BY a.Name;
```

### Gestione Personaggi

```sql
-- Dare livello 50
UPDATE dolcharacters 
SET Level = 50, 
    Experience = 1000000000,
    SkillSpecialtyPoints = 900
WHERE Name = 'PlayerName';

-- Dare Realm Points
UPDATE dolcharacters 
SET RealmPoints = RealmPoints + 100000 
WHERE Name = 'PlayerName';

-- Teletrasportare a Camelot
UPDATE dolcharacters 
SET Region = 10, 
    Xpos = 35564, 
    Ypos = 27157, 
    Zpos = 8448,
    Direction = 2048
WHERE Name = 'PlayerName';

-- Reset specializzazioni (respec)
UPDATE dolcharacters 
SET SkillSpecialtyPoints = (Level - 5) * 20,
    IsLevelRespecUsed = 1
WHERE Name = 'PlayerName';

-- Dare denaro
UPDATE dolcharacters 
SET Platinum = Platinum + 100 
WHERE Name = 'PlayerName';
```

### Gestione Items

```sql
-- Dare item a personaggio
INSERT INTO inventoryitem (
    InventoryItem_ID, OwnerID, Id_nb, SlotPosition, Count, Quality, Condition, Durability
)
SELECT 
    UUID(),
    c.DOLCharacters_ID,
    'epic_sword_template',
    0,  -- Primo slot libero inventory
    1,
    100,
    100,
    100
FROM dolcharacters c
WHERE c.Name = 'PlayerName';

-- Rimuovere tutti gli items da un personaggio
DELETE FROM inventoryitem 
WHERE OwnerID = (
    SELECT DOLCharacters_ID FROM dolcharacters WHERE Name = 'PlayerName'
);

-- Trovare chi ha un certo item
SELECT c.Name, i.SlotPosition, i.Count
FROM inventoryitem i
JOIN dolcharacters c ON i.OwnerID = c.DOLCharacters_ID
WHERE i.Id_nb = 'rare_item_template';
```

### Gestione Guild

```sql
-- Creare nuova guild
INSERT INTO guild (GuildID, GuildName, Realm, GuildLevel, BountyPoints, RealmPoints)
VALUES (UUID(), 'My Guild', 1, 1, 0, 0);

-- Aggiungere personaggio a guild
UPDATE dolcharacters 
SET GuildID = (SELECT GuildID FROM guild WHERE GuildName = 'My Guild'),
    GuildRank = 9  -- Rank più basso
WHERE Name = 'PlayerName';

-- Dare BP alla guild
UPDATE guild 
SET BountyPoints = BountyPoints + 10000 
WHERE GuildName = 'My Guild';
```

### Gestione Mob e NPC

```sql
-- Creare nuovo mob
INSERT INTO mob (
    Mob_ID, Name, Model, Level, Realm, X, Y, Z, Heading, Region, 
    RespawnInterval, Flags
)
VALUES (
    UUID(), 'Test Monster', 100, 50, 0, 
    500000, 500000, 5000, 2048, 1,
    300000,  -- 5 minuti respawn
    0
);

-- Modificare livello di tutti i mob in una regione
UPDATE mob 
SET Level = Level + 5 
WHERE Region = 1 AND Level < 50;

-- Disabilitare respawn di un mob
UPDATE mob 
SET RespawnInterval = -1 
WHERE Name = 'Boss Monster';
```

### Gestione Server Properties

```sql
-- Modificare rate XP
UPDATE serverproperty 
SET Value = '2.0' 
WHERE `Key` = 'xp_rate';

-- Modificare rate RP
UPDATE serverproperty 
SET Value = '0.5' 
WHERE `Key` = 'rp_rate';

-- Vedere tutte le properties modificate
SELECT Category, `Key`, Value, DefaultValue
FROM serverproperty
WHERE Value != DefaultValue;
```

### Query Statistiche

```sql
-- Personaggi per livello
SELECT Level, COUNT(*) as Count
FROM dolcharacters
GROUP BY Level
ORDER BY Level;

-- Top 10 personaggi per Realm Points
SELECT Name, Realm, RealmPoints, Level
FROM dolcharacters
ORDER BY RealmPoints DESC
LIMIT 10;

-- Distribuzione classi per realm
SELECT Realm, Class, COUNT(*) as Count
FROM dolcharacters
GROUP BY Realm, Class
ORDER BY Realm, Count DESC;

-- Guild più ricche (BP)
SELECT GuildName, Realm, BountyPoints, RealmPoints
FROM guild
ORDER BY BountyPoints DESC
LIMIT 10;

-- Account con più personaggi
SELECT AccountName, COUNT(*) as NumChars, 
       SUM(Level) as TotalLevels
FROM dolcharacters
GROUP BY AccountName
ORDER BY NumChars DESC
LIMIT 10;
```

---

## ✅ Best Practices

### Backup

```bash
# Backup completo
mysqldump -u root -p opendaoc | gzip > backup_$(date +%Y%m%d).sql.gz

# Backup solo struttura
mysqldump -u root -p --no-data opendaoc > structure.sql

# Backup solo dati
mysqldump -u root -p --no-create-info opendaoc > data.sql

# Backup tabella specifica
mysqldump -u root -p opendaoc dolcharacters > characters_backup.sql
```

### Restore

```bash
# Restore completo
gunzip < backup.sql.gz | mysql -u root -p opendaoc

# Restore tabella specifica
mysql -u root -p opendaoc < characters_backup.sql
```

### Manutenzione

```sql
-- Ottimizzare tabelle
OPTIMIZE TABLE dolcharacters;
OPTIMIZE TABLE inventoryitem;
OPTIMIZE TABLE mob;

-- Riparare tabelle
REPAIR TABLE dolcharacters;

-- Analizzare tabelle per query optimizer
ANALYZE TABLE dolcharacters;

-- Vedere dimensioni tabelle
SELECT 
    table_name AS 'Table',
    ROUND(((data_length + index_length) / 1024 / 1024), 2) AS 'Size (MB)'
FROM information_schema.TABLES
WHERE table_schema = 'opendaoc'
ORDER BY (data_length + index_length) DESC;
```

### Pulizia Dati

```sql
-- Rimuovere personaggi inattivi (non loggati da 1 anno)
DELETE FROM dolcharacters 
WHERE LastPlayed < DATE_SUB(NOW(), INTERVAL 1 YEAR)
AND Level < 20;

-- Rimuovere items orfani (senza owner)
DELETE FROM inventoryitem 
WHERE OwnerID NOT IN (SELECT DOLCharacters_ID FROM dolcharacters);

-- Rimuovere quest completate vecchie
DELETE FROM characterxdataquest 
WHERE Step = 999  -- Step finale
AND LastTimeRowUpdated < DATE_SUB(NOW(), INTERVAL 6 MONTH);
```

### Sicurezza

```sql
-- Creare utente read-only per statistiche
CREATE USER 'readonly'@'localhost' IDENTIFIED BY 'password';
GRANT SELECT ON opendaoc.* TO 'readonly'@'localhost';

-- Creare utente per applicazioni
CREATE USER 'gameserver'@'%' IDENTIFIED BY 'strongpassword';
GRANT SELECT, INSERT, UPDATE, DELETE ON opendaoc.* TO 'gameserver'@'%';

-- Revocare permessi DROP
REVOKE DROP ON opendaoc.* FROM 'gameserver'@'%';
```

### Performance

```sql
-- Aggiungere indici per query frequenti
CREATE INDEX idx_char_level ON dolcharacters(Level);
CREATE INDEX idx_char_realm ON dolcharacters(Realm);
CREATE INDEX idx_item_template ON inventoryitem(Id_nb);
CREATE INDEX idx_mob_region ON mob(Region);

-- Vedere query lente
SHOW PROCESSLIST;

-- Abilitare slow query log
SET GLOBAL slow_query_log = 'ON';
SET GLOBAL long_query_time = 2;  -- Query > 2 secondi
```

---

## 📊 Tabelle Riepilogative

### Tabelle Principali per Categoria

| Categoria | Tabelle | Descrizione |
|-----------|---------|-------------|
| **Account** | account, accountxcrafting, accountxmoney, ban | Gestione account e autenticazione |
| **Personaggi** | dolcharacters, inventoryitem, characterxdataquest | Dati personaggi e inventario |
| **Mondo** | region, zone, area, mob, npctemplate | Geografia e NPC |
| **Items** | itemtemplate, loottemplate, merchantitem | Template items e loot |
| **Guild** | guild, guildrank, guildalliance | Sistema guild |
| **PvP/RvR** | keep, relic, battleground | Fortezze e PvP |
| **Quest** | dataquest, characterxdataquest | Sistema quest |
| **Crafting** | crafteditem, craftedxitem | Ricette crafting |
| **Housing** | dbhouse, dbindooritem, dboutdooritem | Case giocatori |
| **Sistema** | serverproperty, startuplocation, autoxmlupdate | Configurazione server |

### Campi Comuni in Tutte le Tabelle

| Campo | Tipo | Descrizione |
|-------|------|-------------|
| `LastTimeRowUpdated` | datetime | Timestamp ultima modifica |
| `{Table}_ID` | varchar(255) | Primary key (UUID o AUTO_INCREMENT) |

### Realm IDs

| ID | Realm | Colore |
|----|-------|--------|
| 0 | None/Neutral | Grigio |
| 1 | Albion | Rosso |
| 2 | Midgard | Blu |
| 3 | Hibernia | Verde |

### Privilege Levels

| Level | Ruolo | Permessi |
|-------|-------|----------|
| 1 | Player | Gioco normale |
| 2 | GM | Comandi moderazione |
| 3 | Admin | Accesso completo |

---

## 🎓 Conclusione

Questa documentazione copre la struttura completa del database OpenDAoC. Per modifiche specifiche o dubbi, consulta:

1. **Codice Sorgente**: `OpenDAoC-Core/CoreDatabase/Tables/*.cs`
2. **Server Rules**: `OpenDAoC-Core/GameServer/serverrules/`
3. **Game Objects**: `OpenDAoC-Core/GameServer/gameobjects/`

### Risorse Utili

- **Backup Automatico**: Configurare cron job per backup giornalieri
- **Monitoring**: Usare tools come phpMyAdmin o Adminer
- **Logs**: Controllare `/var/log/mysql/` per errori
- **Performance**: Monitorare con `SHOW STATUS` e `SHOW VARIABLES`

---

**Documento creato il**: 30 Gennaio 2026  
**Versione**: 1.0  
**Autore**: Analisi approfondita database OpenDAoC
