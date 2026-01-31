using System;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;
using DOL.Database;

namespace DOL.GS.Commands
{
    [Cmd(
        "&petbuff",
        ePrivLevel.Player,
        "Buffs your pet instantly",
        "/petbuff - Buff your pet")]
    public class PetBuffCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            GamePlayer player = client.Player;
            
            if (player == null)
                return;

            // Check if player has a pet
            if (player.ControlledBrain == null || player.ControlledBrain.Body == null)
            {
                player.Out.SendMessage("You must control a pet to use this command!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Check if in combat
            if (player.InCombat)
            {
                player.Out.SendMessage("You cannot buff your pet while in combat!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Check cooldown (60 seconds)
            long lastBuff = player.TempProperties.GetProperty<long>("LastPetBuffCommand");
            long currentTime = GameLoop.GameLoopTime;
            
            if (currentTime - lastBuff < 60000) // 60 second cooldown
            {
                player.Out.SendMessage($"You must wait {(60000 - (currentTime - lastBuff)) / 1000} more seconds!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Apply buffs to pet
            GameLiving pet = player.ControlledBrain.Body;
            ApplyPetBuffs(player, pet);

            // Heal pet
            pet.Health = pet.MaxHealth;

            player.TempProperties.SetProperty("LastPetBuffCommand", currentTime);
            player.Out.SendMessage("Your pet has been buffed by the power of the realm!", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
        }

        private void ApplyPetBuffs(GamePlayer player, GameLiving pet)
        {
            var spellLine = new SpellLine("PetBuffCommand", "Pet Buff Command", "unknown", false);
            
            // Base buffs
            CastPetBuff(player, pet, CreatePetBuffSpell("Strength of the Realm", 50, 1457, eSpellType.StrengthBuff), spellLine);
            CastPetBuff(player, pet, CreatePetBuffSpell("Dexterity of the Realm", 50, 1476, eSpellType.DexterityBuff), spellLine);
            CastPetBuff(player, pet, CreatePetBuffSpell("Fortitude of the Realm", 50, 1486, eSpellType.ConstitutionBuff), spellLine);
            CastPetBuff(player, pet, CreatePetBuffSpell("Armor of the Realm", 50, 1467, eSpellType.ArmorFactorBuff), spellLine);
            
            // Spec buffs (doppi)
            CastPetBuff(player, pet, CreatePetBuffSpell("Might of the Realm", 75, 1517, eSpellType.StrengthConstitutionBuff), spellLine);
            CastPetBuff(player, pet, CreatePetBuffSpell("Deftness of the Realm", 75, 1526, eSpellType.DexterityQuicknessBuff), spellLine);
            CastPetBuff(player, pet, CreatePetBuffSpell("Acuity of the Realm", 75, 1538, eSpellType.AcuityBuff), spellLine);
            
            // Combat buffs
            CastPetBuff(player, pet, CreatePetBuffSpell("Haste of the Realm", 15, 407, eSpellType.CombatSpeedBuff), spellLine);
            CastPetBuff(player, pet, CreateDamageAddSpell("Damage of the Realm", 7.5, 18), spellLine);
        }
        
        private Spell CreatePetBuffSpell(string name, int value, int icon, eSpellType type)
        {
            var dbSpell = new DbSpell
            {
                Name = name,
                Description = $"Increases pet stats by {value}",
                Value = value,
                Duration = 7200, // 2 hours
                CastTime = 0,
                ClientEffect = icon,
                Icon = (ushort)icon,
                Type = type.ToString(),
                Target = eSpellTarget.PET.ToString(),
                Range = WorldMgr.VISIBILITY_DISTANCE,
                SpellID = 100000 + icon
            };
            return new Spell(dbSpell, 50);
        }
        
        private Spell CreateDamageAddSpell(string name, double damage, int icon)
        {
            var dbSpell = new DbSpell
            {
                Name = name,
                Description = $"Pet's melee attacks do +{damage} damage",
                Damage = damage,
                DamageType = (int)eDamageType.Body,
                Duration = 7200,
                CastTime = 0,
                ClientEffect = icon,
                Icon = (ushort)icon,
                Type = eSpellType.DamageAdd.ToString(),
                Target = eSpellTarget.PET.ToString(),
                Range = WorldMgr.VISIBILITY_DISTANCE,
                SpellID = 100000 + icon
            };
            return new Spell(dbSpell, 50);
        }
        
        private void CastPetBuff(GamePlayer player, GameLiving pet, Spell spell, SpellLine line)
        {
            var spellHandler = ScriptMgr.CreateSpellHandler(player, spell, line);
            if (spellHandler != null)
            {
                spellHandler.StartSpell(pet);
            }
        }
    }
}
