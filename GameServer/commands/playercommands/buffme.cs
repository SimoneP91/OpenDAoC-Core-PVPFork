using System;
using DOL.GS.PacketHandler;
using DOL.GS.Effects;
using DOL.GS.Spells;
using DOL.Database;

namespace DOL.GS.Commands
{
    [Cmd(
        "&bb",
        ePrivLevel.Player,
        "Buffs yourself instantly",
        "/bb - Get instant buffs")]
    public class BuffMeCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            GamePlayer player = client.Player;
            
            if (player == null)
                return;

            // Check cooldown
            long lastBuff = player.TempProperties.GetProperty<long>("LastBuffCommand");
            long currentTime = GameLoop.GameLoopTime;
            
            if (currentTime - lastBuff < 30000) // 30 second cooldown
            {
                player.Out.SendMessage($"You must wait {(30000 - (currentTime - lastBuff)) / 1000} more seconds!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }

            // Apply buffs based on class type
            if (player.CharacterClass.ClassType == eClassType.ListCaster)
            {
                ApplyCasterBuffs(player);
            }
            else
            {
                ApplyMeleeBuffs(player);
            }

            // Heal player
            player.Health = player.MaxHealth;
            if (player.CharacterClass.ID != (int)eCharacterClass.MaulerAlb &&
                player.CharacterClass.ID != (int)eCharacterClass.MaulerHib &&
                player.CharacterClass.ID != (int)eCharacterClass.MaulerMid &&
                player.CharacterClass.ID != (int)eCharacterClass.Vampiir)
            {
                player.Mana = player.MaxMana;
            }

            player.TempProperties.SetProperty("LastBuffCommand", currentTime);
            player.Out.SendMessage("You have been buffed by the power of the realm!", eChatType.CT_Important, eChatLoc.CL_SystemWindow);
            player.Out.SendUpdatePlayer();
        }

        private void ApplyCasterBuffs(GamePlayer player)
        {
            var spellLine = new SpellLine("BuffCommand", "Buff Command", "unknown", false);
            
            // Base buffs
            CastBuff(player, CreateBuffSpell("Strength of the Realm", 55, 1457, eSpellType.StrengthBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Dexterity of the Realm", 55, 1476, eSpellType.DexterityBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Fortitude of the Realm", 55, 1486, eSpellType.ConstitutionBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Armor of the Realm", 58, 1467, eSpellType.ArmorFactorBuff), spellLine);
            
            // Spec buffs (doppi)
            CastBuff(player, CreateBuffSpell("Might of the Realm", 85, 1517, eSpellType.StrengthConstitutionBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Deftness of the Realm", 85, 1526, eSpellType.DexterityQuicknessBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Acuity of the Realm", 72, 1538, eSpellType.AcuityBuff), spellLine);
        }

        private void ApplyMeleeBuffs(GamePlayer player)
        {
            var spellLine = new SpellLine("BuffCommand", "Buff Command", "unknown", false);
            
            // Base buffs
            CastBuff(player, CreateBuffSpell("Strength of the Realm", 74, 1457, eSpellType.StrengthBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Dexterity of the Realm", 74, 1476, eSpellType.DexterityBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Fortitude of the Realm", 74, 1486, eSpellType.ConstitutionBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Armor of the Realm", 78, 1467, eSpellType.ArmorFactorBuff), spellLine);
            
            // Spec buffs (doppi)
            CastBuff(player, CreateBuffSpell("Might of the Realm", 114, 1517, eSpellType.StrengthConstitutionBuff), spellLine);
            CastBuff(player, CreateBuffSpell("Deftness of the Realm", 114, 1526, eSpellType.DexterityQuicknessBuff), spellLine);
        }
        
        private Spell CreateBuffSpell(string name, int value, int icon, eSpellType type)
        {
            var dbSpell = new DbSpell
            {
                Name = name,
                Description = $"Increases stats by {value}",
                Value = value,
                Duration = 7200,
                CastTime = 0,
                ClientEffect = icon,
                Icon = (ushort)icon,
                Type = type.ToString(),
                Target = eSpellTarget.SELF.ToString(),
                Range = 0,
                SpellID = 90000 + icon
            };
            return new Spell(dbSpell, 50);
        }
        
        private void CastBuff(GamePlayer player, Spell spell, SpellLine line)
        {
            var spellHandler = ScriptMgr.CreateSpellHandler(player, spell, line);
            if (spellHandler != null)
            {
                spellHandler.StartSpell(player);
            }
        }
    }
}
