# Architecture - OpenDAoC

Documentazione completa dell'architettura di OpenDAoC.

## 📋 Indice

- [Panoramica Architettura](#panoramica-architettura)
- [Entity Component System (ECS)](#entity-component-system-ecs)
- [Layer Applicazione](#layer-applicazione)
- [Sistema Database](#sistema-database)
- [Sistema Networking](#sistema-networking)
- [Sistema AI](#sistema-ai)
- [Sistema Spell](#sistema-spell)
- [Sistema Combat](#sistema-combat)
- [Flusso Esecuzione](#flusso-esecuzione)

## 🏗️ Panoramica Architettura

OpenDAoC utilizza un'architettura moderna basata su **Entity Component System (ECS)** per massimizzare performance e scalabilità.

### Architettura High-Level

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Layer                          │
│                    (DAoC Game Client)                        │
└───────────────────────────┬─────────────────────────────────┘
                            │ TCP/UDP Packets
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     Network Layer                            │
│              PacketHandlers & PacketLib                      │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Game Logic Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ ECS System   │  │ Server Rules │  │  AI System   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ Spell System │  │Combat System │  │ World Mgmt   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     Database Layer                           │
│              MySQL / SQLite / MSSQL                          │
└─────────────────────────────────────────────────────────────┘
```

### Principi Architetturali

1. **Separation of Concerns** - Ogni componente ha responsabilità ben definite
2. **Data-Driven Design** - Configurazione via database e XML
3. **Event-Driven** - Sistema eventi per comunicazione tra componenti
4. **Scalability** - Architettura ECS per gestire migliaia di entità
5. **Modularity** - Componenti intercambiabili e estendibili

## 🎯 Entity Component System (ECS)

### Cos'è ECS?

ECS separa **dati** (Components) da **logica** (Systems) e **identità** (Entities).

```
Entity = ID + Collection of Components
Component = Pure Data
System = Logic that operates on Components
```

### Struttura ECS in OpenDAoC

```
GameServer/
├── ECS-Components/        # Componenti (dati)
│   ├── HealthComponent.cs
│   ├── PositionComponent.cs
│   ├── StatsComponent.cs
│   └── ...
├── ECS-Effects/          # Effetti (componenti temporanei)
│   ├── DamageOverTimeEffect.cs
│   ├── StunEffect.cs
│   └── ...
└── ECS-Services/         # Sistemi (logica)
    ├── CombatService.cs
    ├── MovementService.cs
    └── ...
```

### Esempio Pratico

#### Component (Dati)

```csharp
// ECS-Components/HealthComponent.cs
public class HealthComponent
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public bool IsAlive => Health > 0;
    
    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }
    
    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
}
```

#### System (Logica)

```csharp
// ECS-Services/HealthRegenerationService.cs
public static class HealthRegenerationService
{
    public static void Tick(GameLiving living)
    {
        if (living.HealthComponent == null || !living.HealthComponent.IsAlive)
            return;
        
        if (living.HealthComponent.Health < living.HealthComponent.MaxHealth)
        {
            int regenAmount = CalculateRegen(living);
            living.HealthComponent.Heal(regenAmount);
        }
    }
    
    private static int CalculateRegen(GameLiving living)
    {
        // Logica calcolo rigenerazione
        return living.Level * 2;
    }
}
```

#### Entity (Identità)

```csharp
// gameobjects/GameLiving.cs
public abstract class GameLiving : GameObject
{
    // Entity ha riferimenti ai suoi Components
    public HealthComponent HealthComponent { get; set; }
    public StatsComponent StatsComponent { get; set; }
    public PositionComponent PositionComponent { get; set; }
    
    public virtual void Tick()
    {
        // Systems operano sui Components
        HealthRegenerationService.Tick(this);
        MovementService.Tick(this);
        CombatService.Tick(this);
    }
}
```

### Vantaggi ECS

- **Performance**: Cache-friendly, dati contigui in memoria
- **Scalabilità**: Facilmente parallelizzabile
- **Flessibilità**: Aggiungi/rimuovi componenti a runtime
- **Manutenibilità**: Logica separata dai dati

## 📚 Layer Applicazione

### 1. CoreBase

**Responsabilità**: Classi base e utilities condivise.

```
CoreBase/
├── Enums/
│   ├── eRealm.cs              # Realm: Albion, Midgard, Hibernia
│   ├── eCharacterClass.cs     # Classi personaggio
│   ├── EGameServerType.cs     # Tipi server
│   └── ...
└── Utilities/
    ├── Util.cs                # Utility functions
    └── ...
```

**Esempio**:

```csharp
namespace DOL;

public enum eRealm : byte
{
    None = 0,
    Albion = 1,
    Midgard = 2,
    Hibernia = 3
}

public static class Util
{
    public static int Random(int min, int max)
    {
        return ThreadSafeRandom.Next(min, max);
    }
}
```

### 2. CoreDatabase

**Responsabilità**: Astrazione database multi-provider.

```
CoreDatabase/
├── Connection/
│   ├── DataConnection.cs      # Connessione base
│   ├── MySQLConnection.cs     # MySQL implementation
│   ├── SQLiteConnection.cs    # SQLite implementation
│   └── ...
└── Tables/
    ├── DbAccount.cs           # Tabella Account
    ├── DbCharacter.cs         # Tabella Character
    ├── DbInventoryItem.cs     # Tabella Items
    └── ...
```

**Pattern**: Repository + Unit of Work

```csharp
// Definizione tabella
[DataTable(TableName = "Account")]
public class DbAccount : DataObject
{
    [PrimaryKey]
    public string AccountId { get; set; }
    
    [DataElement(AllowDbNull = false, Index = true, Unique = true)]
    public string Name { get; set; }
    
    [DataElement(AllowDbNull = false)]
    public string Password { get; set; }
}

// Utilizzo
var account = GameServer.Database.SelectObject<DbAccount>(
    DB.Column("Name").IsEqualTo("username")
);

GameServer.Database.AddObject(account);
GameServer.Database.SaveObject(account);
GameServer.Database.DeleteObject(account);
```

### 3. CoreServer

**Responsabilità**: Entry point e bootstrap applicazione.

```
CoreServer/
├── MainClass.cs               # Entry point
├── GameServerService.cs       # Windows Service wrapper
└── config/
    └── serverconfig.xml       # Configurazione
```

**Flusso Startup**:

```csharp
// MainClass.cs
public static void Main(string[] args)
{
    // 1. Carica configurazione
    var config = new GameServerConfiguration();
    config.LoadFromXMLFile("config/serverconfig.xml");
    
    // 2. Inizializza logging
    LogManager.Configure(config.LogConfigFile);
    
    // 3. Crea e avvia GameServer
    var server = new GameServer(config);
    if (!server.Start())
    {
        log.Error("Failed to start server!");
        return;
    }
    
    // 4. Main loop
    while (server.IsRunning)
    {
        Thread.Sleep(100);
    }
    
    // 5. Cleanup
    server.Stop();
}
```

### 4. GameServer

**Responsabilità**: Logica principale del gioco.

```
GameServer/
├── GameServer.cs              # Server principale
├── GameClient.cs              # Gestione client connessi
├── ai/                        # Intelligenza artificiale
├── commands/                  # Comandi player/GM
├── gameobjects/               # Oggetti di gioco
├── packets/                   # Gestione pacchetti
├── serverrules/               # Regole server
├── spells/                    # Sistema magie
└── world/                     # Gestione mondo
```

## 🗄️ Sistema Database

### Architettura Database

```
┌─────────────────────────────────────────┐
│         Application Layer                │
│    (GameServer, Commands, etc.)          │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│      Database Abstraction Layer          │
│         (CoreDatabase)                   │
│  ┌─────────────────────────────────┐    │
│  │  DataObject (Base Class)        │    │
│  │  - DbAccount                    │    │
│  │  - DbCharacter                  │    │
│  │  - DbInventoryItem              │    │
│  └─────────────────────────────────┘    │
└──────────────────┬──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│      Connection Providers                │
│  ┌──────────┐ ┌──────────┐ ┌─────────┐ │
│  │  MySQL   │ │  SQLite  │ │  MSSQL  │ │
│  └──────────┘ └──────────┘ └─────────┘ │
└─────────────────────────────────────────┘
```

### Schema Database Principale

**Tabelle Core**:

- `account` - Account utenti
- `character` - Personaggi
- `inventoryitem` - Inventario
- `mob` - NPCs e creature
- `worldobject` - Oggetti nel mondo
- `guild` - Gilde
- `keep` - Fortezze
- `relic` - Reliquie

**Relazioni**:

```
account (1) ──── (N) character
character (1) ──── (N) inventoryitem
character (N) ──── (1) guild
realm (1) ──── (N) relic
```

### Caching Strategy

```csharp
public class DatabaseCache
{
    private static Dictionary<string, DbAccount> _accountCache = new();
    private static Dictionary<int, DbCharacter> _characterCache = new();
    
    public static DbAccount GetAccount(string name)
    {
        if (!_accountCache.TryGetValue(name, out var account))
        {
            account = GameServer.Database.SelectObject<DbAccount>(
                DB.Column("Name").IsEqualTo(name)
            );
            if (account != null)
                _accountCache[name] = account;
        }
        return account;
    }
    
    public static void InvalidateAccount(string name)
    {
        _accountCache.Remove(name);
    }
}
```

## 🌐 Sistema Networking

### Architettura Network

```
Client ←→ TCP Socket ←→ PacketProcessor ←→ PacketHandler ←→ Game Logic
          UDP Socket
```

### Packet Flow

```
1. Client invia packet
   ↓
2. PacketProcessor riceve e decodifica
   ↓
3. Identifica PacketHandler appropriato
   ↓
4. PacketHandler processa
   ↓
5. Modifica game state
   ↓
6. Invia response packet al client
```

### Struttura Packets

```
GameServer/packets/
├── Client/                    # Packets da client a server
│   ├── 168/                  # Client version 1.68
│   │   ├── PlayerPositionUpdateHandler.cs
│   │   ├── UseItemHandler.cs
│   │   └── ...
│   └── ...
└── Server/                    # Packets da server a client
    ├── PacketLib168.cs       # Packet library v1.68
    ├── PacketLib1124.cs      # Packet library v1.124
    └── ...
```

### Esempio PacketHandler

```csharp
// Client/168/UseItemHandler.cs
[PacketHandler(PacketHandlerType.TCP, eClientPackets.UseSlot, ClientStatus.Playing)]
public class UseItemHandler : IPacketHandler
{
    public void HandlePacket(GameClient client, GSPacketIn packet)
    {
        // 1. Leggi dati dal packet
        int slot = packet.ReadByte();
        int type = packet.ReadByte();
        
        // 2. Valida
        if (client.Player == null)
            return;
        
        // 3. Esegui azione
        InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slot);
        if (item != null)
        {
            client.Player.UseItem(item);
        }
    }
}
```

### Esempio PacketLib

```csharp
// Server/PacketLib168.cs
public class PacketLib168 : PacketLib
{
    public override void SendMessage(string msg, eChatType type, eChatLoc loc)
    {
        using (var pak = new GSTCPPacketOut(GetPacketCode(eServerPackets.Message)))
        {
            pak.WriteByte((byte)type);
            pak.WriteByte((byte)loc);
            pak.WriteString(msg);
            SendTCP(pak);
        }
    }
    
    public override void SendPlayerPosition()
    {
        using (var pak = new GSTCPPacketOut(GetPacketCode(eServerPackets.PlayerPosition)))
        {
            pak.WriteShort((ushort)m_gameClient.Player.X);
            pak.WriteShort((ushort)m_gameClient.Player.Y);
            pak.WriteShort((ushort)m_gameClient.Player.Z);
            pak.WriteShort(m_gameClient.Player.Heading);
            SendTCP(pak);
        }
    }
}
```

## 🤖 Sistema AI

### Architettura AI

```
GameNPC
  └── Brain (ABrain)
        ├── StandardMobBrain
        ├── GuardBrain
        ├── ControlledNpcBrain (Pets)
        └── CustomBrain (Scripts)
```

### Brain System

```csharp
// ai/brain/ABrain.cs
public abstract class ABrain
{
    protected GameNPC m_body;
    
    public virtual void Think()
    {
        // Main AI loop (chiamato ogni tick)
    }
    
    public virtual void Attack(GameObject target)
    {
        // Logica attacco
    }
    
    public virtual void OnAttacked(AttackData ad)
    {
        // Reazione a attacco
    }
}

// ai/brain/StandardMobBrain.cs
public class StandardMobBrain : ABrain
{
    public override void Think()
    {
        // 1. Check aggro
        if (!HasAggro)
        {
            CheckForAggro();
        }
        
        // 2. Combat
        if (HasAggro)
        {
            AttackMostWanted();
        }
        
        // 3. Return to spawn
        if (!InCombat && !IsNearSpawn)
        {
            WalkToSpawn();
        }
    }
}
```

### FSM (Finite State Machine)

```
┌─────────┐
│  IDLE   │
└────┬────┘
     │ Player in range
     ▼
┌─────────┐
│ AGGRO   │
└────┬────┘
     │ Start combat
     ▼
┌─────────┐
│ COMBAT  │
└────┬────┘
     │ Target dead/escaped
     ▼
┌─────────┐
│ RETURN  │
└────┬────┘
     │ At spawn point
     ▼
┌─────────┐
│  IDLE   │
└─────────┘
```

## ✨ Sistema Spell

### Architettura Spell

```
Spell (Data)
  └── SpellHandler (Logic)
        ├── DamageSpellHandler
        ├── HealSpellHandler
        ├── BuffSpellHandler
        └── CustomSpellHandler
```

### Spell Flow

```
1. Player casts spell
   ↓
2. Check requirements (mana, range, etc.)
   ↓
3. Start casting (cast time)
   ↓
4. FinishSpellCast()
   ↓
5. Apply effect to target
   ↓
6. Send packets to client
```

### Esempio SpellHandler

```csharp
// spells/DamageSpellHandler.cs
[SpellHandler("DirectDamage")]
public class DirectDamageSpellHandler : SpellHandler
{
    public override void FinishSpellCast(GameLiving target)
    {
        // 1. Validate
        if (target == null || !target.IsAlive)
            return;
        
        // 2. Calculate damage
        int damage = CalculateDamage(target);
        
        // 3. Apply resistances
        damage = (int)(damage * target.GetResist(Spell.DamageType));
        
        // 4. Deal damage
        DamageTarget(target, damage);
        
        // 5. Send messages
        MessageToCaster($"You hit {target.Name} for {damage} damage!", eChatType.CT_YouHit);
        MessageToLiving(target, $"{Caster.Name} hits you for {damage} damage!", eChatType.CT_Damaged);
    }
    
    protected virtual int CalculateDamage(GameLiving target)
    {
        double damage = Spell.Damage;
        damage *= Caster.Effectiveness;
        damage *= 1.0 + Caster.GetModified(eProperty.SpellDamage) * 0.01;
        return (int)damage;
    }
}
```

## ⚔️ Sistema Combat

### Combat Flow

```
Attacker.StartAttack(Defender)
  ↓
Check CanAttack()
  ↓
Calculate Hit Chance
  ↓
Roll Hit/Miss
  ↓
If Hit:
  ├─ Calculate Damage
  ├─ Apply Armor
  ├─ Apply Styles/Criticals
  └─ Deal Damage
  ↓
Send Combat Packets
  ↓
Check for Death
```

### Combat Calculation

```csharp
public class CombatService
{
    public static AttackData CalculateAttack(GameLiving attacker, GameLiving defender)
    {
        var ad = new AttackData();
        
        // 1. Hit chance
        int hitChance = CalculateHitChance(attacker, defender);
        if (Util.Random(1, 100) > hitChance)
        {
            ad.AttackResult = eAttackResult.Missed;
            return ad;
        }
        
        // 2. Base damage
        int damage = CalculateBaseDamage(attacker);
        
        // 3. Armor reduction
        damage = ApplyArmor(damage, defender);
        
        // 4. Styles
        if (attacker.ActiveStyle != null)
        {
            damage = ApplyStyle(damage, attacker.ActiveStyle);
        }
        
        // 5. Critical
        if (Util.Random(1, 100) <= attacker.CriticalChance)
        {
            damage *= 2;
            ad.CriticalDamage = damage / 2;
        }
        
        ad.Damage = damage;
        ad.AttackResult = eAttackResult.Hit;
        return ad;
    }
}
```

## 🔄 Flusso Esecuzione

### Server Main Loop

```csharp
public class GameServer
{
    private Timer m_timer;
    
    public bool Start()
    {
        // 1. Initialize
        InitializeDatabase();
        LoadWorldData();
        StartNetworkListener();
        
        // 2. Start main timer (50ms tick = 20 FPS)
        m_timer = new Timer(WorldUpdate, null, 0, 50);
        
        return true;
    }
    
    private void WorldUpdate(object state)
    {
        try
        {
            // 1. Update all regions
            foreach (var region in WorldMgr.Regions.Values)
            {
                region.Update();
            }
            
            // 2. Process scheduled tasks
            TaskScheduler.ProcessTasks();
            
            // 3. Update clients
            foreach (var client in Clients)
            {
                client.Update();
            }
            
            // 4. Autosave
            if (ShouldAutosave())
            {
                SaveDatabase();
            }
        }
        catch (Exception ex)
        {
            log.Error("Error in WorldUpdate", ex);
        }
    }
}
```

### Region Update

```csharp
public class Region
{
    public void Update()
    {
        // 1. Update all objects in region
        foreach (var obj in Objects)
        {
            if (obj is GameLiving living)
            {
                living.Tick();
            }
        }
        
        // 2. Process movement
        ProcessMovement();
        
        // 3. Process combat
        ProcessCombat();
        
        // 4. Cleanup dead objects
        CleanupObjects();
    }
}
```

### Client Update

```csharp
public class GameClient
{
    public void Update()
    {
        // 1. Process incoming packets
        ProcessPackets();
        
        // 2. Update player
        if (Player != null)
        {
            Player.Update();
        }
        
        // 3. Send queued packets
        FlushPacketQueue();
        
        // 4. Check timeout
        if (IsTimedOut())
        {
            Disconnect();
        }
    }
}
```

## 📊 Performance Considerations

### Ottimizzazioni Implementate

1. **Object Pooling**: Riutilizzo oggetti frequenti (packets, effects)
2. **Spatial Partitioning**: Grid-based per collision detection
3. **Lazy Loading**: Caricamento on-demand di dati
4. **Caching**: Cache multi-livello per database
5. **Async I/O**: Operazioni I/O asincrone
6. **ECS**: Data-oriented design per cache efficiency

### Metriche Performance

```csharp
public class PerformanceStatistics
{
    public long WorldUpdateTime { get; set; }
    public long DatabaseQueryTime { get; set; }
    public long PacketProcessingTime { get; set; }
    public int ActivePlayers { get; set; }
    public int ActiveNPCs { get; set; }
    public long MemoryUsage { get; set; }
}
```

## 📚 Riferimenti

- [DEVELOPMENT.md](DEVELOPMENT.md) - Guida sviluppo
- [CONFIGURATION.md](CONFIGURATION.md) - Configurazione
- [SERVER_TYPES.md](SERVER_TYPES.md) - Modalità server

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026
