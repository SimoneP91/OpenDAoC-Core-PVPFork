-- =====================================================
-- OpenDAoC - SPAWN POINTS CON COORDINATE UFFICIALI
-- Coordinate prese dalla documentazione ufficiale DOL
-- =====================================================

-- Pulisci tutto
DELETE FROM startuplocation;

-- =====================================================
-- SPAWN POINT ALBION - Cotswold Village (coordinate ufficiali)
-- Fonte: Dawn of Light Forum - Jump Points List
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    559016,    -- X ufficiale di Cotswold
    513069,    -- Y ufficiale di Cotswold
    0,         -- Z ufficiale di Cotswold
    2048,      -- Heading
    1,         -- Region ID (Camelot Hills)
    0,         -- MinVersion
    1,         -- RealmID (Albion)
    0,         -- RaceID (tutte le razze albionesi)
    0,         -- ClassID (tutte le classi)
    0          -- ClientRegionID
);

-- =====================================================
-- SPAWN POINT MIDGARD - Mularn (coordinate ufficiali)
-- Fonte: Dawn of Light Forum - Jump Points List
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    804763,    -- X ufficiale di Mularn
    723998,    -- Y ufficiale di Mularn
    4680,      -- Z ufficiale di Mularn
    2048,      -- Heading
    100,       -- Region ID (Vale of Mularn)
    0,         -- MinVersion
    2,         -- RealmID (Midgard)
    0,         -- RaceID (tutte le razze midgardiane)
    0,         -- ClassID (tutte le classi)
    0          -- ClientRegionID
);

-- =====================================================
-- SPAWN POINT HIBERNIA - Mag Mell (coordinate ufficiali)
-- Fonte: Dawn of Light Forum - Jump Points List
-- =====================================================
INSERT INTO startuplocation (
    XPos, YPos, ZPos, Heading, Region, MinVersion, RealmID, RaceID, ClassID, ClientRegionID
) VALUES (
    345684,    -- X ufficiale di Mag Mell
    490996,    -- Y ufficiale di Mag Mell
    5200,      -- Z ufficiale di Mag Mell
    2048,      -- Heading
    200,       -- Region ID (Lough Derg)
    0,         -- MinVersion
    3,         -- RealmID (Hibernia)
    0,         -- RaceID (tutte le razze hiberniane)
    0,         -- ClassID (tutte le classi)
    0          -- ClientRegionID
);

-- =====================================================
-- VERIFICA FINALE
-- =====================================================
SELECT 
    RealmID,
    CASE RealmID 
        WHEN 1 THEN 'Albion - Cotswold Village (ufficiale)'
        WHEN 2 THEN 'Midgard - Mularn (ufficiale)'
        WHEN 3 THEN 'Hibernia - Mag Mell (ufficiale)'
    END as Location,
    XPos, YPos, ZPos, Region
FROM startuplocation 
ORDER BY RealmID;

-- =====================================================
-- NOTE:
-- Coordinate UFFICIALI da Dawn of Light Forum:
-- https://dolserver.net/forum/viewtopic.php?t=726
-- 
-- - Cotswold: 559016, 513069, 0 (Region 1)
-- - Mularn: 804763, 723998, 4680 (Region 100)
-- - Mag Mell: 345684, 490996, 5200 (Region 200)
-- 
-- Queste sono le coordinate VERIFICATE e usate dalla community!
-- =====================================================
