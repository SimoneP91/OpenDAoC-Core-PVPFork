using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;
using DOL.Language;

namespace DOL.GS
{
    public class BuffBot : GameNPC
    {
        #region BuffBot attrib/spells/casting
        public BuffBot() : base()
        {
            Flags |= GameNPC.eFlags.PEACE;
        }

        public override int Concentration
        {
            get { return 10000; }
        }

        public override int Mana
        {
            get { return 10000; }
        }

        private Queue m_buffs = new Queue();
        private const int BUFFS_SPELL_DURATION = 7200;
        private const bool BUFFS_PLAYER_PET = true;

        public override bool AddToWorld()
        {
            Level = 50;
            return base.AddToWorld();
        }

        /// <summary>
        /// Adds a buff to the queue for casting on the player/pet.
        /// </summary>
        public void BuffPlayer(GamePlayer player, Spell spell, SpellLine spellLine)
        {
            if (m_buffs == null) m_buffs = new Queue();
            
            m_buffs.Enqueue(new Container(spell, spellLine, player));

            // Don't forget his pet!
            if(BUFFS_PLAYER_PET && player.ControlledBrain != null)  
            {
                if(player.ControlledBrain.Body != null) 
                {
                    m_buffs.Enqueue(new Container(spell, spellLine, player.ControlledBrain.Body));
                }
            }

            CastBuffs();
        }

        /// <summary>
        /// Casts all pending buffs in the queue.
        /// </summary>
        public void CastBuffs()
        {
            Container con = null;
            while (m_buffs.Count > 0)
            {
                con = (Container)m_buffs.Dequeue();

                ISpellHandler spellHandler = ScriptMgr.CreateSpellHandler(con.Target, con.Spell, con.SpellLine);

                if (spellHandler != null)
                {
                    spellHandler.StartSpell(con.Target);
                }
            }
        }

        #region SpellCasting

        private static SpellLine m_MerchBaseSpellLine;
        private static SpellLine m_MerchSpecSpellLine;
        private static SpellLine m_MerchOtherSpellLine;

        /// <summary>
        /// Spell line used by Merchs
        /// </summary>
        public static SpellLine MerchBaseSpellLine
        {
            get
            {
                if (m_MerchBaseSpellLine == null)
                    m_MerchBaseSpellLine = new SpellLine("MerchBaseSpellLine", "BuffMerch Spells", "unknown", true);

                return m_MerchBaseSpellLine;
            }
        }
        public static SpellLine MerchSpecSpellLine
        {
            get
            {
                if (m_MerchSpecSpellLine == null)
                    m_MerchSpecSpellLine = new SpellLine("MerchSpecSpellLine", "BuffMerch Spells", "unknown", false);

                return m_MerchSpecSpellLine;
            }
        }
        public static SpellLine MerchOtherSpellLine
        {
            get
            {
                if (m_MerchOtherSpellLine == null)
                    m_MerchOtherSpellLine = new SpellLine("MerchOtherSpellLine", "BuffMerch Spells", "unknown", true);
                
                return m_MerchOtherSpellLine;
            }
        }

        private static Spell m_baseaf;
        private static Spell m_basestr;
        private static Spell m_basecon;
        private static Spell m_basedex;
        private static Spell m_strcon;
        private static Spell m_dexqui;
        private static Spell m_acuity;
        private static Spell m_specaf;
        private static Spell m_casterbaseaf;
        private static Spell m_casterbasestr;
        private static Spell m_casterbasecon;
        private static Spell m_casterbasedex;
        private static Spell m_casterstrcon;
        private static Spell m_casterdexqui;
        private static Spell m_casteracuity;
        private static Spell m_casterspecaf;
        private static Spell m_haste;
        private static Spell m_waterbreathing;

        #region Spells

        /// <summary>
        /// Merch Base AF buff (30 AF)
        /// </summary>
        public static Spell MerchBaseAFBuff
        {
            get
            {
                if (m_baseaf == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2000;
                    spell.Icon = 5017;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 30;
                    spell.Name = "Armor of the Realm (A)";
                    spell.Description = "Base AF: 30. Increases armor factor.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88001;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.BaseArmorFactorBuff.ToString();
                    spell.EffectGroup = 801;

                    m_baseaf = new Spell(spell, 50);
                }
                return m_baseaf;
            }
        }

        /// <summary>
        /// Merch Base Str buff (32 Str)
        /// </summary>
        public static Spell MerchStrBuff
        {
            get
            {
                if (m_basestr == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2001;
                    spell.Icon = 5005;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 54;
                    spell.Name = "Strength of the Realm (A)";
                    spell.Description = "Base Strength: 32. Increases strength.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88002;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.StrengthBuff.ToString();
                    spell.EffectGroup = 802;
                    
                    m_basestr = new Spell(spell, 50);
                }
                return m_basestr;
            }
        }

        /// <summary>
        /// Merch Caster Base Str buff (32 Str)
        /// </summary>
        public static Spell casterMerchStrBuff
        {
            get
            {
                if (m_casterbasestr == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2001;
                    spell.Icon = 5005;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 54;
                    spell.Name = "Strength of the Realm (C)";
                    spell.Description = "Base Strength: 32. Increases strength.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89002;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.StrengthBuff.ToString();
                    spell.EffectGroup = 802;

                    m_casterbasestr = new Spell(spell, 50);
                }
                return m_casterbasestr;
            }
        }

        /// <summary>
        /// Merch Base Con buff (32 Con)
        /// </summary>
        public static Spell MerchConBuff
        {
            get
            {
                if (m_basecon == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2002;
                    spell.Icon = 5034;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 32;
                    spell.Name = "Fortitude of the Realm (A)";
                    spell.Description = "Base Constitution: 32. Increases constitution/hit points.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88003;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.ConstitutionBuff.ToString();
                    spell.EffectGroup = 803;

                    m_basecon = new Spell(spell, 50);
                }
                return m_basecon;
            }
        }

        /// <summary>
        /// Merch Caster Base Con buff (32 Con)
        /// </summary>
        public static Spell casterMerchConBuff
        {
            get
            {
                if (m_casterbasecon == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2002;
                    spell.Icon = 5034;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 32;
                    spell.Name = "Fortitude of the Realm (C)";
                    spell.Description = "Base Constitution: 32. Increases constitution/hit points.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89003;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.ConstitutionBuff.ToString();
                    spell.EffectGroup = 803;

                    m_casterbasecon = new Spell(spell, 50);
                }
                return m_casterbasecon;
            }
        }

        /// <summary>
        /// Merch Base Dex buff (32 Dex)
        /// </summary>
        public static Spell MerchDexBuff
        {
            get
            {
                if (m_basedex == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2003;
                    spell.Icon = 5024;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 32;
                    spell.Name = "Dexterity of the Realm (A)";
                    spell.Description = "Base Dexterity: 32. Increases dexterity.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88004;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.DexterityBuff.ToString();
                    spell.EffectGroup = 804;

                    m_basedex = new Spell(spell, 50);
                }
                return m_basedex;
            }
        }

        /// <summary>
        /// Merch Caster Base Dex buff (32 Dex)
        /// </summary>
        public static Spell casterMerchDexBuff
        {
            get
            {
                if (m_casterbasedex == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2003;
                    spell.Icon = 5024;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 32;
                    spell.Name = "Dexterity of the Realm (C)";
                    spell.Description = "Base Dexterity: 32. Increases dexterity.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89004;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.DexterityBuff.ToString();
                    spell.EffectGroup = 804;

                    m_casterbasedex = new Spell(spell, 50);
                }
                return m_casterbasedex;
            }
        }

        /// <summary>
        /// Merch Spec Str/Con buff (47 Str/Con)
        /// </summary>
        public static Spell MerchStrConBuff
        {
            get
            {
                if (m_strcon == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2004;
                    spell.Icon = 5065;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 47;
                    spell.Name = "Might of the Realm (A)";
                    spell.Description = "Spec Str/Con: 47. Increases strength and constitution.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88005;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.StrengthConstitutionBuff.ToString();
                    spell.EffectGroup = 805;

                    m_strcon = new Spell(spell, 50);
                }
                return m_strcon;
            }
        }

        /// <summary>
        /// Merch Caster Spec Str/Con buff (47 Str/Con)
        /// </summary>
        public static Spell casterMerchStrConBuff
        {
            get
            {
                if (m_casterstrcon == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2004;
                    spell.Icon = 5065;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 47;
                    spell.Name = "Might of the Realm (C)";
                    spell.Description = "Spec Str/Con: 47. Increases strength and constitution.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89005;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.StrengthConstitutionBuff.ToString();
                    spell.EffectGroup = 805;

                    m_casterstrcon = new Spell(spell, 50);
                }
                return m_casterstrcon;
            }
        }

        /// <summary>
        /// Merch Spec Dex/Qui buff (47 Dex/Qui)
        /// </summary>
        public static Spell MerchDexQuiBuff
        {
            get
            {
                if (m_dexqui == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2005;
                    spell.Icon = 5074;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 47;
                    spell.Name = "Deftness of the Realm (A)";
                    spell.Description = "Spec Dex/Qui: 47. Increases dexterity and quickness.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88006;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.DexterityQuicknessBuff.ToString();
                    spell.EffectGroup = 806;

                    m_dexqui = new Spell(spell, 50);
                }
                return m_dexqui;
            }
        }

        /// <summary>
        /// Merch Caster Spec Dex/Qui buff (47 Dex/Qui)
        /// </summary>
        public static Spell casterMerchDexQuiBuff
        {
            get
            {
                if (m_casterdexqui == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2005;
                    spell.Icon = 5074;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 47;
                    spell.Name = "Deftness of the Realm (C)";
                    spell.Description = "Spec Dex/Qui: 47. Increases dexterity and quickness.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89006;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.DexterityQuicknessBuff.ToString();
                    spell.EffectGroup = 806;

                    m_casterdexqui = new Spell(spell, 50);
                }
                return m_casterdexqui;
            }
        }

        /// <summary>
        /// Merch Spec Acuity buff (44 Acuity)
        /// </summary>
        public static Spell MerchAcuityBuff
        {
            get
            {
                if (m_acuity == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2006;
                    spell.Icon = 5078;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 44;
                    spell.Name = "Acuity of the Realm (A)";
                    spell.Description = "Spec Acuity: 44. Increases spell attributes.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88007;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.AcuityBuff.ToString();
                    spell.EffectGroup = 807;

                    m_acuity = new Spell(spell, 50);
                }
                return m_acuity;
            }
        }

        /// <summary>
        /// Merch Caster Spec Acuity buff (44 Acuity)
        /// </summary>
        public static Spell casterMerchAcuityBuff
        {
            get
            {
                if (m_casteracuity == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2006;
                    spell.Icon = 5078;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 44;
                    spell.Name = "Acuity of the Realm (C)";
                    spell.Description = "Spec Acuity: 44. Increases spell attributes.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89007;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.AcuityBuff.ToString();
                    spell.EffectGroup = 807;

                    m_casteracuity = new Spell(spell, 50);
                }
                return m_casteracuity;
            }
        }

        /// <summary>
        /// Merch Spec Af buff (43 Spec AF)
        /// </summary>
        public static Spell MerchSpecAFBuff
        {
            get
            {
                if (m_specaf == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2007;
                    spell.Icon = 1504;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 43;
                    spell.Name = "Reinforced Armor of the Realm (A)";
                    spell.Description = "Spec AF: 43. Increases armor factor (Spec).";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88014;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.SpecArmorFactorBuff.ToString();
                    spell.EffectGroup = 808;

                    m_specaf = new Spell(spell, 50);
                }
                return m_specaf;
            }
        }

        /// <summary>
        /// Merch Caster Spec Af buff (43 Spec AF)
        /// </summary>
        public static Spell casterMerchSpecAFBuff
        {
            get
            {
                if (m_casterspecaf == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2007;
                    spell.Icon = 1504;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 43;
                    spell.Name = "Reinforced Armor of the Realm (C)";
                    spell.Description = "Spec AF: 43. Increases armor factor (Spec).";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 89014;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.SpecArmorFactorBuff.ToString();
                    spell.EffectGroup = 808;

                    m_casterspecaf = new Spell(spell, 50);
                }
                return m_casterspecaf;
            }
        }

        /// <summary>
        /// Merch Haste buff (12% Haste)
        /// </summary>
        public static Spell MerchHasteBuff
        {
            get
            {
                if (m_haste == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2008;
                    spell.Icon = 5054;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 12;
                    spell.Name = "Quickness of the Realm";
                    spell.Description = "Haste: 12%. Increases combat speed.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88010;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.CombatSpeedBuff.ToString();
                    spell.EffectGroup = 809;
                    
                    m_haste = new Spell(spell, 50);
                }
                return m_haste;
            }
        }

        /// <summary>
        /// Merch Waterbreathing (100% Waterbreathing)
        /// </summary>
        public static Spell MerchWaterbreathingBuff
        {
            get
            {
                if (m_waterbreathing == null)
                {
                    DbSpell spell = new DbSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.Concentration = 1;
                    spell.ClientEffect = 2009;
                    spell.Icon = 2009;
                    spell.Duration = BUFFS_SPELL_DURATION;
                    spell.Value = 100;
                    spell.Name = "Breath of the Realm";
                    spell.Description = "Waterbreathing: 100%. Allows underwater breathing.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 88015;
                    spell.Target = eSpellTarget.REALM.ToString();
                    spell.Type = eSpellType.WaterBreathing.ToString();
                    spell.EffectGroup = 810;
                    
                    m_waterbreathing = new Spell(spell, 50);
                }
                return m_waterbreathing;
            }
        }

        #endregion Spells

        #endregion SpellCasting

        private void SendReply(GamePlayer target, string msg)
        {
            target.Out.SendMessage(msg, eChatType.CT_System, eChatLoc.CL_PopupWindow);
        }

        public class Container
        {
            private Spell m_spell;
            public Spell Spell
            {
                get { return m_spell; }
            }

            private SpellLine m_spellLine;
            public SpellLine SpellLine
            {
                get { return m_spellLine; }
            }

            private GameLiving m_target;
            public GameLiving Target
            {
                get { return m_target; }
                set { m_target = value; }
            }
            public Container(Spell spell, SpellLine spellLine, GameLiving target)
            {
                m_spell = spell;
                m_spellLine = spellLine;
                m_target = target;
            }
        }

        #endregion
        
        /// <summary>
        /// Overrides the Interact method to apply buffs directly on right-click.
        /// </summary>
        public override bool Interact(GamePlayer player)
        {
            if (GetDistanceTo(player) > WorldMgr.INTERACT_DISTANCE)
            {
                player.Out.SendMessage("You are too far away to receive buffs from " + GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return false;
            }
            
            TurnTo(player, 10000);

            // Determine if player is a caster or melee/hybrid
            bool isCaster = player.CharacterClass.ClassType == eClassType.ListCaster;

            // Apply buffs based on class type
            if (isCaster)
            {
                // Caster Buffs
                BuffPlayer(player, casterMerchStrBuff, MerchBaseSpellLine);
                BuffPlayer(player, casterMerchDexBuff, MerchBaseSpellLine);
                BuffPlayer(player, casterMerchConBuff, MerchBaseSpellLine);
                BuffPlayer(player, casterMerchSpecAFBuff, MerchSpecSpellLine);
                BuffPlayer(player, casterMerchStrConBuff, MerchSpecSpellLine);
                BuffPlayer(player, casterMerchDexQuiBuff, MerchSpecSpellLine);
                BuffPlayer(player, casterMerchAcuityBuff, MerchSpecSpellLine);
            }
            else
            {
                // Melee/Hybrid Buffs
                BuffPlayer(player, MerchBaseAFBuff, MerchBaseSpellLine);
                BuffPlayer(player, MerchStrBuff, MerchBaseSpellLine);
                BuffPlayer(player, MerchDexBuff, MerchBaseSpellLine);
                BuffPlayer(player, MerchConBuff, MerchBaseSpellLine);
                BuffPlayer(player, MerchSpecAFBuff, MerchSpecSpellLine);
                BuffPlayer(player, MerchStrConBuff, MerchSpecSpellLine);
                BuffPlayer(player, MerchDexQuiBuff, MerchSpecSpellLine);
                BuffPlayer(player, MerchAcuityBuff, MerchSpecSpellLine);
            }

            // Buffs for all classes
            BuffPlayer(player, MerchHasteBuff, MerchSpecSpellLine);
            BuffPlayer(player, MerchWaterbreathingBuff, MerchSpecSpellLine);

            // Send confirmation message
            string realmName = player.Realm.ToString();
            player.Out.SendMessage("The green light of " + realmName + " refreshes your enhancements. Fight well!", eChatType.CT_Say, eChatLoc.CL_PopupWindow);

            return true;
        }

        public override bool WhisperReceive(GameLiving source, string str)
        {
            return false;
        }

        public override void OnPlayerBuy(GamePlayer player, int item_slot, int number)
        {
            return;
        }

        public override bool ReceiveItem(GameLiving source, DbInventoryItem item)
        {
            GamePlayer t = source as GamePlayer;
            if (t != null && item != null)
            {
                t.Out.SendMessage(GetName(0, false) + " no longer accepts buff tokens. I can grant you the buffs directly when you speak to me.", eChatType.CT_Say, eChatLoc.CL_PopupWindow);
            }
            return false;
        }
    }
}
