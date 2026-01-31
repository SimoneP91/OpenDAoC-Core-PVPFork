-- =====================================================
-- OpenDAoC - AGGIUNGI DESTINAZIONI CROSS-REALM
-- I teleporter cercano :Jordheim e :Tir na Nog ma non esistono nel DB
-- =====================================================

-- Coordinate dalle jump points ufficiali:
-- Jordheim: 32006, 35905, 8010, Region 101
-- Tir na Nog: 21355, 34518, 6190, Region 201
-- Camelot: 32192, 31192, 8000, Region 10

-- =====================================================
-- AGGIUNGI DESTINAZIONE :Jordheim (per Albion e Hibernia)
-- =====================================================
INSERT INTO teleport (
    Type, TeleportID, Realm, RegionID, X, Y, Z, Heading, Teleport_ID
) VALUES (
    '',           -- Type
    ':Jordheim',  -- TeleportID (con : come prefisso)
    2,            -- Realm (Midgard)
    101,          -- RegionID (Jordheim City)
    32006,        -- X (coordinate ufficiali)
    35905,        -- Y
    8010,         -- Z
    2048,         -- Heading
    UUID()        -- Teleport_ID (genera UUID automatico)
);

-- =====================================================
-- AGGIUNGI DESTINAZIONE :Tir na Nog (per Albion e Midgard)
-- =====================================================
INSERT INTO teleport (
    Type, TeleportID, Realm, RegionID, X, Y, Z, Heading, Teleport_ID
) VALUES (
    '',              -- Type
    ':Tir na Nog',   -- TeleportID (con : come prefisso)
    3,               -- Realm (Hibernia)
    201,             -- RegionID (Tir na Nog City)
    21355,           -- X (coordinate ufficiali)
    34518,           -- Y
    6190,            -- Z
    2048,            -- Heading
    UUID()           -- Teleport_ID (genera UUID automatico)
);

-- =====================================================
-- AGGIUNGI DESTINAZIONE :Camelot (per Midgard e Hibernia)
-- =====================================================
INSERT INTO teleport (
    Type, TeleportID, Realm, RegionID, X, Y, Z, Heading, Teleport_ID
) VALUES (
    '',          -- Type
    ':Camelot',  -- TeleportID (con : come prefisso)
    1,           -- Realm (Albion)
    10,          -- RegionID (Camelot City)
    32192,       -- X (coordinate ufficiali)
    31192,       -- Y
    8000,        -- Z
    2048,        -- Heading
    UUID()       -- Teleport_ID (genera UUID automatico)
);

-- =====================================================
-- VERIFICA
-- =====================================================
SELECT TeleportID, Realm, RegionID, X, Y, Z 
FROM teleport 
WHERE TeleportID IN (':Jordheim', ':Tir na Nog', ':Camelot')
ORDER BY Realm;

-- =====================================================
-- NOTE:
-- Il codice nei teleporter cerca queste destinazioni con:
-- - WorldMgr.GetTeleportLocation(eRealm.Midgard, ":Jordheim")
-- - WorldMgr.GetTeleportLocation(eRealm.Hibernia, ":Tir na Nog")
-- - WorldMgr.GetTeleportLocation(eRealm.Albion, ":Camelot")
-- 
-- Il prefisso ":" è importante perché distingue le destinazioni
-- speciali dalle destinazioni normali.
-- 
-- Dopo aver eseguito questo script, riavvia il server!
-- =====================================================
