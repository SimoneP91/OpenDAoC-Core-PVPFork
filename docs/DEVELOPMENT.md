# Development Guide - OpenDAoC

Guida completa allo sviluppo per OpenDAoC.

## 📋 Indice

- [Setup Ambiente](#setup-ambiente)
- [Struttura Progetto](#struttura-progetto)
- [Build & Compile](#build--compile)
- [Debugging](#debugging)
- [Testing](#testing)
- [Contribuire](#contribuire)
- [Best Practices](#best-practices)
- [API & Estensioni](#api--estensioni)

## 🔧 Setup Ambiente

### Requisiti

#### Software Obbligatorio

- **.NET 10.0 SDK** o superiore ([Download](https://dotnet.microsoft.com/download))
- **Git** per version control
- **IDE** (uno dei seguenti):
  - Visual Studio 2022 (Community/Professional/Enterprise)
  - JetBrains Rider
  - Visual Studio Code con estensione C#

#### Software Consigliato

- **Docker Desktop** per testing containerizzato
- **MySQL Workbench** o **DBeaver** per gestione database
- **Postman** o **Insomnia** per testing API
- **Git GUI** (GitKraken, SourceTree, GitHub Desktop)

### Verifica Installazione

```bash
# Verifica .NET SDK
dotnet --version
# Output: 10.0.x o superiore

# Verifica Git
git --version
# Output: git version 2.x.x

# Lista SDK installati
dotnet --list-sdks
```

### Clone Repository

```bash
# Clone repository principale
git clone https://github.com/OpenDAoC/OpenDAoC-Core.git
cd OpenDAoC-Core

# Clone database (opzionale ma consigliato)
cd ..
git clone https://github.com/OpenDAoC/OpenDAoC-Database.git
```

### Setup Database Locale

#### Opzione 1: SQLite (Più Semplice)

```bash
# Il database SQLite viene creato automaticamente al primo avvio
# Nessuna configurazione necessaria
```

#### Opzione 2: MySQL/MariaDB

```bash
# Installa MySQL/MariaDB
# Windows: Download da mysql.com
# Linux: sudo apt install mariadb-server
# macOS: brew install mariadb

# Crea database
mysql -u root -p
CREATE DATABASE opendaoc CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci;
CREATE USER 'opendaoc'@'localhost' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON opendaoc.* TO 'opendaoc'@'localhost';
FLUSH PRIVILEGES;
EXIT;

# Importa database
cd OpenDAoC-Database/opendaoc-db-core
cat *.sql > combined.sql
mysql -u opendaoc -p opendaoc < combined.sql
```

### Configurazione IDE

#### Visual Studio 2022

1. Apri `Dawn of Light.sln`
2. Imposta `CoreServer` come startup project
3. Build > Build Solution (Ctrl+Shift+B)
4. Debug > Start Debugging (F5)

**Configurazione consigliata:**
- Tools > Options > Text Editor > C# > Code Style > Formatting
- Abilita "Format document on save"
- Usa EditorConfig del progetto

#### Visual Studio Code

1. Installa estensioni:
   - C# (Microsoft)
   - C# Dev Kit
   - .NET Core Test Explorer

2. Apri cartella progetto
3. Premi F5 per debug

**File `.vscode/launch.json`:**

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/CoreServer/bin/Debug/net10.0/CoreServer.dll",
            "args": [],
            "cwd": "${workspaceFolder}/CoreServer/bin/Debug/net10.0",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```

#### JetBrains Rider

1. Apri `Dawn of Light.sln`
2. Run > Edit Configurations
3. Imposta CoreServer come startup
4. Run > Debug (Shift+F9)

## 📁 Struttura Progetto

### Overview

```
OpenDAoC-Core/
├── CoreBase/              # Classi base e utilities
│   ├── Enums/            # Enumerazioni globali
│   └── ...
├── CoreDatabase/          # Layer database
│   ├── Connection/       # Gestione connessioni
│   └── Tables/           # Definizioni tabelle
├── CoreServer/            # Entry point applicazione
│   ├── MainClass.cs      # Main entry point
│   └── config/           # File configurazione
├── GameServer/            # Logica principale gioco
│   ├── ai/               # AI NPCs
│   ├── commands/         # Comandi player/GM
│   ├── ECS-Components/   # Entity Component System
│   ├── ECS-Effects/      # Sistema effetti
│   ├── gameobjects/      # Oggetti di gioco
│   ├── packets/          # Gestione pacchetti rete
│   ├── serverrules/      # Regole server
│   ├── spells/           # Sistema magie
│   └── world/            # Gestione mondo
├── Pathing/              # Sistema pathfinding
├── Tests/                # Unit tests
└── Dawn of Light.sln     # Solution file
```

### Progetti Principali

#### CoreBase

Classi base condivise tra tutti i progetti:

```csharp
// Esempio: Enumerazioni
namespace DOL;

public enum eRealm
{
    None = 0,
    Albion = 1,
    Midgard = 2,
    Hibernia = 3
}
```

#### CoreDatabase

Layer di accesso al database con supporto multi-DB:

```csharp
// Esempio: Definizione tabella
[DataTable(TableName = "Account")]
public class DbAccount : DataObject
{
    private string m_name;
    private string m_password;
    
    [DataElement(AllowDbNull = false, Index = true, Unique = true)]
    public string Name
    {
        get => m_name;
        set => m_name = value;
    }
    
    [DataElement(AllowDbNull = false)]
    public string Password
    {
        get => m_password;
        set => m_password = value;
    }
}
```

#### GameServer

Cuore del server di gioco:

```csharp
// Esempio: GameObject base
public abstract class GameObject
{
    public string Name { get; set; }
    public ushort CurrentRegionID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public ushort Heading { get; set; }
    
    public abstract void AddToWorld();
    public abstract void RemoveFromWorld();
}
```

## 🔨 Build & Compile

### Build da Linea di Comando

```bash
# Build Debug
dotnet build "Dawn of Light.sln" -c Debug

# Build Release
dotnet build "Dawn of Light.sln" -c Release

# Build con verbose output
dotnet build "Dawn of Light.sln" -c Release -v detailed

# Clean
dotnet clean "Dawn of Light.sln"

# Rebuild
dotnet clean "Dawn of Light.sln"
dotnet build "Dawn of Light.sln" -c Release
```

### Publish

```bash
# Publish Release
dotnet publish "Dawn of Light.sln" -c Release -o publish

# Publish per Linux
dotnet publish "Dawn of Light.sln" -c Release -r linux-x64 -o publish/linux

# Publish per Windows
dotnet publish "Dawn of Light.sln" -c Release -r win-x64 -o publish/windows

# Self-contained (include runtime)
dotnet publish "Dawn of Light.sln" -c Release -r linux-x64 --self-contained -o publish/linux-standalone
```

### Build Script

Crea `build.sh` (Linux/macOS):

```bash
#!/bin/bash
set -e

echo "Building OpenDAoC..."
dotnet clean "Dawn of Light.sln"
dotnet build "Dawn of Light.sln" -c Release

echo "Publishing..."
dotnet publish "Dawn of Light.sln" -c Release -o publish

echo "Build completato!"
```

Crea `build.bat` (Windows):

```batch
@echo off
echo Building OpenDAoC...
dotnet clean "Dawn of Light.sln"
dotnet build "Dawn of Light.sln" -c Release

echo Publishing...
dotnet publish "Dawn of Light.sln" -c Release -o publish

echo Build completato!
```

### Compilazione Script

OpenDAoC compila automaticamente gli script nella cartella `GameServer/scripts/` all'avvio.

**Disabilita per produzione:**

```yaml
environment:
  ENABLE_COMPILATION: "False"
```

**Compila manualmente:**

```bash
# Gli script vengono compilati in GameServerScripts.dll
# Output in: lib/GameServerScripts.dll
```

## 🐛 Debugging

### Debug con Visual Studio

1. Imposta breakpoint (F9)
2. Avvia debug (F5)
3. Usa Debug toolbar:
   - Continue (F5)
   - Step Over (F10)
   - Step Into (F11)
   - Step Out (Shift+F11)

### Debug con VS Code

```json
// .vscode/launch.json
{
    "configurations": [
        {
            "name": "Debug OpenDAoC",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/CoreServer/bin/Debug/net10.0/CoreServer.dll",
            "args": [],
            "cwd": "${workspaceFolder}/CoreServer/bin/Debug/net10.0",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
    ]
}
```

### Logging

Configura logging in `config/logconfig.xml`:

```xml
<log4net>
    <root>
        <level value="DEBUG" />  <!-- DEBUG per sviluppo -->
        <appender-ref ref="FileAppender" />
        <appender-ref ref="ConsoleAppender" />
    </root>
</log4net>
```

**Usa logging nel codice:**

```csharp
using log4net;

public class MyClass
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public void MyMethod()
    {
        log.Debug("Debug message");
        log.Info("Info message");
        log.Warn("Warning message");
        log.Error("Error message", exception);
    }
}
```

### Debug Docker

```bash
# Avvia in modalità interattiva
docker compose run --rm gameserver sh

# Attach a container in esecuzione
docker exec -it opendaoc-server sh

# Log in tempo reale
docker logs -f opendaoc-server

# Debug con VS Code Remote Containers
# Installa estensione "Remote - Containers"
# F1 > "Remote-Containers: Attach to Running Container"
```

### Profiling

#### dotnet-trace

```bash
# Installa tool
dotnet tool install --global dotnet-trace

# Avvia profiling
dotnet-trace collect --process-id [PID]

# Analizza con PerfView o Visual Studio
```

#### dotnet-counters

```bash
# Installa tool
dotnet tool install --global dotnet-counters

# Monitor performance
dotnet-counters monitor --process-id [PID]
```

## 🧪 Testing

### Unit Tests

```bash
# Esegui tutti i test
dotnet test

# Esegui test specifici
dotnet test --filter "FullyQualifiedName~GameServer.Tests"

# Con coverage
dotnet test /p:CollectCoverage=true
```

### Scrivere Test

```csharp
using Xunit;

namespace DOL.Tests
{
    public class GamePlayerTests
    {
        [Fact]
        public void TestPlayerCreation()
        {
            var player = new GamePlayer();
            Assert.NotNull(player);
        }
        
        [Theory]
        [InlineData(1, "Albion")]
        [InlineData(2, "Midgard")]
        [InlineData(3, "Hibernia")]
        public void TestRealmNames(int realmId, string expectedName)
        {
            var realm = (eRealm)realmId;
            Assert.Equal(expectedName, realm.ToString());
        }
    }
}
```

### Integration Tests

```csharp
public class DatabaseTests : IDisposable
{
    private readonly GameServer _server;
    
    public DatabaseTests()
    {
        // Setup test server
        _server = new GameServer();
        _server.InitializeDatabase();
    }
    
    [Fact]
    public void TestAccountCreation()
    {
        var account = new DbAccount
        {
            Name = "testuser",
            Password = "password"
        };
        
        GameServer.Database.AddObject(account);
        
        var retrieved = GameServer.Database.SelectObject<DbAccount>(
            DB.Column("Name").IsEqualTo("testuser")
        );
        
        Assert.NotNull(retrieved);
        Assert.Equal("testuser", retrieved.Name);
    }
    
    public void Dispose()
    {
        _server?.Stop();
    }
}
```

## 🤝 Contribuire

### Workflow Git

```bash
# 1. Fork repository su GitHub

# 2. Clone del tuo fork
git clone https://github.com/TUO_USERNAME/OpenDAoC-Core.git
cd OpenDAoC-Core

# 3. Aggiungi upstream
git remote add upstream https://github.com/OpenDAoC/OpenDAoC-Core.git

# 4. Crea branch per feature
git checkout -b feature/my-awesome-feature

# 5. Fai modifiche e commit
git add .
git commit -m "Add: My awesome feature"

# 6. Push al tuo fork
git push origin feature/my-awesome-feature

# 7. Apri Pull Request su GitHub
```

### Commit Messages

Usa conventional commits:

```
feat: Add new spell system
fix: Fix player teleport bug
docs: Update configuration guide
refactor: Refactor database layer
test: Add unit tests for GamePlayer
chore: Update dependencies
```

### Code Style

Segui le convenzioni C#:

```csharp
// ✅ Corretto
public class GamePlayer : GameLiving
{
    private string m_name;
    private int m_level;
    
    public string Name
    {
        get => m_name;
        set => m_name = value;
    }
    
    public void LevelUp()
    {
        m_level++;
        OnLevelUp();
    }
    
    private void OnLevelUp()
    {
        // Implementation
    }
}

// ❌ Sbagliato
public class gameplayer : gameliving
{
    public string name;
    public int level;
    
    public void levelup()
    {
        level++;
    }
}
```

**Convenzioni:**
- PascalCase per classi, metodi, proprietà
- camelCase per variabili locali
- m_ prefix per campi privati
- Usa `var` quando il tipo è ovvio
- Commenti XML per API pubbliche

### Pull Request Checklist

- [ ] Codice compila senza errori
- [ ] Test passano
- [ ] Documentazione aggiornata
- [ ] Commit messages descrittivi
- [ ] Nessun file non necessario (bin/, obj/, .vs/)
- [ ] Code style rispettato
- [ ] Breaking changes documentati

## 💡 Best Practices

### Performance

```csharp
// ✅ Usa StringBuilder per concatenazioni multiple
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i);
string result = sb.ToString();

// ❌ Evita concatenazioni in loop
string result = "";
for (int i = 0; i < 1000; i++)
    result += i;  // Crea 1000 stringhe!

// ✅ Usa object pooling per oggetti frequenti
var pool = new ObjectPool<GameObject>();
var obj = pool.Get();
// Use object
pool.Return(obj);

// ✅ Cache risultati costosi
private Dictionary<int, GameNPC> _npcCache = new();

public GameNPC GetNPC(int id)
{
    if (!_npcCache.TryGetValue(id, out var npc))
    {
        npc = LoadNPCFromDatabase(id);
        _npcCache[id] = npc;
    }
    return npc;
}
```

### Thread Safety

```csharp
// ✅ Usa lock per accesso concorrente
private readonly object _lock = new();
private List<GamePlayer> _players = new();

public void AddPlayer(GamePlayer player)
{
    lock (_lock)
    {
        _players.Add(player);
    }
}

// ✅ Oppure usa collezioni thread-safe
private ConcurrentDictionary<int, GamePlayer> _players = new();

public void AddPlayer(int id, GamePlayer player)
{
    _players.TryAdd(id, player);
}
```

### Error Handling

```csharp
// ✅ Gestisci eccezioni specifiche
try
{
    var player = LoadPlayer(id);
}
catch (DatabaseException ex)
{
    log.Error($"Database error loading player {id}", ex);
    return null;
}
catch (Exception ex)
{
    log.Error($"Unexpected error loading player {id}", ex);
    throw;
}

// ❌ Evita catch generici senza re-throw
try
{
    DoSomething();
}
catch (Exception)
{
    // Silent fail - BAD!
}
```

### Dependency Injection

```csharp
// ✅ Usa DI per testabilità
public class PlayerManager
{
    private readonly IDatabase _database;
    private readonly ILogger _logger;
    
    public PlayerManager(IDatabase database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public GamePlayer LoadPlayer(int id)
    {
        return _database.SelectObject<GamePlayer>(id);
    }
}

// Test diventa facile
var mockDb = new Mock<IDatabase>();
var mockLogger = new Mock<ILogger>();
var manager = new PlayerManager(mockDb.Object, mockLogger.Object);
```

## 🔌 API & Estensioni

### Creare Custom Command

```csharp
using DOL.GS.Commands;

namespace DOL.GS.Scripts.Commands
{
    [CmdAttribute(
        "&mycommand",
        ePrivLevel.Player,
        "My custom command",
        "/mycommand [args]")]
    public class MyCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }
            
            var player = client.Player;
            var argument = args[1];
            
            player.Out.SendMessage($"You used mycommand with: {argument}", 
                eChatType.CT_System, eChatLoc.CL_SystemWindow);
        }
    }
}
```

### Creare Custom Spell

```csharp
using DOL.GS.Spells;

namespace DOL.GS.Scripts.Spells
{
    [SpellHandler("CustomDamage")]
    public class CustomDamageSpellHandler : SpellHandler
    {
        public CustomDamageSpellHandler(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
        }
        
        public override void FinishSpellCast(GameLiving target)
        {
            base.FinishSpellCast(target);
            
            if (target == null || !target.IsAlive)
                return;
            
            int damage = CalculateDamage(target);
            DamageTarget(target, damage);
        }
        
        private int CalculateDamage(GameLiving target)
        {
            // Custom damage calculation
            return Spell.Damage * Caster.Level / 50;
        }
    }
}
```

### Creare Custom Server Rules

```csharp
using DOL.GS.ServerRules;

namespace DOL.GS.Scripts.ServerRules
{
    [ServerRules(EGameServerType.GST_Custom)]
    public class CustomServerRules : AbstractServerRules
    {
        public override string RulesDescription()
        {
            return "Custom server rules";
        }
        
        public override bool IsAllowedToAttack(GameLiving attacker, GameLiving defender, bool quiet)
        {
            // Custom attack rules
            if (attacker.Level > defender.Level + 10)
            {
                if (!quiet)
                    MessageToLiving(attacker, "Target is too low level!");
                return false;
            }
            
            return base.IsAllowedToAttack(attacker, defender, quiet);
        }
    }
}
```

### Event System

```csharp
using DOL.Events;

public class MyEventHandler
{
    [ScriptLoadedEvent]
    public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
    {
        // Register event handlers
        GameEventMgr.AddHandler(GamePlayerEvent.GameEntered, OnPlayerEntered);
        GameEventMgr.AddHandler(GamePlayerEvent.Dying, OnPlayerDying);
    }
    
    [ScriptUnloadedEvent]
    public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args)
    {
        // Unregister event handlers
        GameEventMgr.RemoveHandler(GamePlayerEvent.GameEntered, OnPlayerEntered);
        GameEventMgr.RemoveHandler(GamePlayerEvent.Dying, OnPlayerDying);
    }
    
    private static void OnPlayerEntered(DOLEvent e, object sender, EventArgs args)
    {
        var player = sender as GamePlayer;
        if (player == null) return;
        
        player.Out.SendMessage("Welcome to the server!", 
            eChatType.CT_System, eChatLoc.CL_SystemWindow);
    }
    
    private static void OnPlayerDying(DOLEvent e, object sender, EventArgs args)
    {
        var player = sender as GamePlayer;
        if (player == null) return;
        
        // Custom death logic
    }
}
```

## 📚 Riferimenti

- [ARCHITECTURE.md](ARCHITECTURE.md) - Architettura dettagliata
- [CONFIGURATION.md](CONFIGURATION.md) - Configurazione
- [SERVER_TYPES.md](SERVER_TYPES.md) - Modalità server

### Risorse Esterne

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [C# Coding Conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [xUnit Documentation](https://xunit.net/)

---

**Versione**: 1.0  
**Ultimo Aggiornamento**: Gennaio 2026
