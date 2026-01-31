-- =====================================================
-- OpenDAoC - Configurazione Spawn Points (3 totali)
-- 1 spawn per realm nelle zone richieste
-- =====================================================

-- Pulizia degli spawn point esistenti
TRUNCATE TABLE startuplocation;

-- =====================================================
-- SPAWN POINT ALBION - Cotswold Village
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    558255,    -- Coordinate X di Cotswold
    519789,    -- Coordinate Y di Cotswold  
    1876,      -- Coordinate Z (altezza)
    2048,      -- Heading (direzione in cui guarda il player)
    1,         -- Region ID (1 = Albion mainland)
    0,         -- MinVersion (0 = nessun requisito)
    1,         -- RealmID (1 = Albion)
    0,         -- RaceID (0 = tutte le razze albionesi)
    0,         -- ClassID (0 = tutte le classi)
    0          -- ClientRegionID (0 = region standard)
);

-- =====================================================
-- SPAWN POINT MIDGARD - Mularn
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    749155,    -- Coordinate X di Mularn
    819432,    -- Coordinate Y di Mularn
    4433,      -- Coordinate Z (altezza)
    2048,      -- Heading (direzione in cui guarda il player)
    100,       -- Region ID (100 = Midgard mainland)
    0,         -- MinVersion (0 = nessun requisito)
    2,         -- RealmID (2 = Midgard)
    0,         -- RaceID (0 = tutte le razze midgardiane)
    0,         -- ClassID (0 = tutte le classi)
    0          -- ClientRegionID (0 = region standard)
);

-- =====================================================
-- SPAWN POINT HIBERNIA - Mag Mell
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    350190,    -- Coordinate X di Mag Mell
    508323,    -- Coordinate Y di Mag Mell
    4966,      -- Coordinate Z (altezza)
    2048,      -- Heading (direzione in cui guarda il player)
    200,       -- Region ID (200 = Hibernia mainland)
    0,         -- MinVersion (0 = nessun requisito)
    3,         -- RealmID (3 = Hibernia)
    0,         -- RaceID (0 = tutte le razze hiberniane)
    0,         -- ClassID (0 = tutte le classi)
    0          -- ClientRegionID (0 = region standard)
);

-- =====================================================
-- VERIFICA CONFIGURAZIONE
-- =====================================================
SELECT 
    RealmID,
    CASE RealmID 
        WHEN 1 THEN 'Albion (Cotswold)'
        WHEN 2 THEN 'Midgard (Mularn)'
        WHEN 3 THEN 'Hibernia (Mag Mell)'
    END as Realm_Name,
    Region as Region_ID,
    XPos,
    YPos,
    ZPos,
    Heading
FROM startuplocation 
ORDER BY RealmID;

-- =====================================================
-- NOTE:
-- 1. Tutti i nuovi personaggi di un realm spawnano nella stessa posizione
-- 2. RaceID = 0 permette tutte le razze del realm
-- 3. ClassID = 0 permette tutte le classi del realm
-- 4. Dopo aver eseguito questo script, riavvia il server o usa /refresh
-- =====================================================
