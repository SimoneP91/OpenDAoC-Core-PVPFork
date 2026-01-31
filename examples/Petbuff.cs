//// Author : Yemla

using System;
using System.Reflection;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using log4net;
using System.Collections;
using System.Collections.Generic;
using DOL.GS;
using DOL.Database;
using DOL.AI.Brain;
using DOL.GS.Keeps;
using DOL.Events;
using DOL.GS.RealmAbilities;
using DOL.Language;


namespace DOL.GS.Commands
{
    [CmdAttribute("&petbuff",

        ePrivLevel.Player,
        "Buffs the players pet.",
        "/petbuff")]
    public class PetBuffCommandHandler : ICommandHandler
    {
        public const string Pet_Buff = "PetBuff";
        public void OnCommand(GameClient client, string[] args)
        {
            GamePlayer player = client.Player as GamePlayer;
            long PetBuffs = player.TempProperties.getProperty(Pet_Buff, 0L);
            long changeTime = player.CurrentRegion.Time - PetBuffs;
            if (client.Player.ControlledBrain == null)
            {
                client.Player.Out.SendMessage("You must control a pet to use this command.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            if (player.InCombat)
            {
                client.Player.Out.SendMessage("You cannot be in combat and use this command.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                return;
            }
            if (changeTime < 60000)
            {
                player.Out.SendMessage("You must wait " + ((60000 - changeTime) / 1000).ToString() + " more second to attempt to use this command!", eChatType.CT_System, eChatLoc.CL_ChatWindow);
                return;
            }
            player.TempProperties.setProperty(Pet_Buff, player.CurrentRegion.Time);
            if (client.Player.IsAlive)
            {
                if (player.ControlledBrain != null)
                {
                    player.CastSpell(PetBaseAFBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetConBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetDexBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetDexQuiBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetDmgaddBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetHasteBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetSpecAFBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetStrBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetStrConBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetAcuityBuff, (SkillBase.GetSpellLine("Item Effects")));
                    player.CastSpell(PetMagicBuff, (SkillBase.GetSpellLine("Item Effects")));
                }
            }
        }
        #region SpellCasting

        private static Spell m_baseaf;
        private static Spell m_basestr;
        private static Spell m_basecon;
        private static Spell m_basedex;
        private static Spell m_strcon;
        private static Spell m_dexqui;
        private static Spell m_acuity;
        private static Spell m_specaf;
        private static Spell m_dmgadd;
        private static Spell m_haste;
        private static Spell m_abs;

        /// <summary>
        /// Pet Base AF buff
        /// </summary>
        public static Spell PetBaseAFBuff
        {
            get
            {
                if (m_baseaf == null)
                {
                    DBSpell spell = new DBSpell(); spell.AllowAdd = false; spell.CastTime = 0;
                    spell.ClientEffect = 1467;
                    spell.Icon = 1467;
                    spell.Duration = 65535;
                    spell.Value = 50;
                    spell.Name = "Armorfactor Buff";
                    spell.Description = "Adds to the recipient's Armor Factor (AF) resulting in better protection againts some forms of attack. It acts in addition to any armor the target is wearing.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100011;
                    spell.Target = "Pet";
                    spell.Type = "ArmorFactorBuff";
                    spell.EffectGroup = 1;
                    m_baseaf = new Spell(spell, 50);
                }
                return m_baseaf;

            }
        }

        /// <summary>
        /// Pet Str buff
        /// </summary>
        public static Spell PetStrBuff
        {
            get
            {
                if (m_basestr == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1457;
                    spell.Icon = 1457;
                    spell.Duration = 65535;
                    spell.Value = 50;
                    spell.Name = "Strength Buff";
                    spell.Description = "Increases target's Strength.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100012;
                    spell.Target = "Pet";
                    spell.Type = "StrengthBuff";
                    spell.EffectGroup = 4;
                    m_basestr = new Spell(spell, 50);
                }
                return m_basestr;
            }
        }

        /// <summary>
        /// Pet Con buff
        /// </summary>
        public static Spell PetConBuff
        {
            get
            {
                if (m_basecon == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1486;
                    spell.Icon = 1486;
                    spell.Duration = 65535;
                    spell.Value = 50;
                    spell.Name = "Constitution Buff";
                    spell.Description = "Increases target's Constitution.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100013;
                    spell.Target = "Pet";
                    spell.Type = "ConstitutionBuff";
                    m_basecon = new Spell(spell, 50);
                }
                return m_basecon;
            }
        }

        /// <summary>
        /// Pet Dex buff
        /// </summary>
        public static Spell PetDexBuff
        {
            get
            {
                if (m_basedex == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1476;
                    spell.Icon = 1476;
                    spell.Duration = 65535;
                    spell.Value = 50;
                    spell.Name = "Dexterity Buff";
                    spell.Description = "Increases target's Dexterity.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100014;
                    spell.Target = "Pet";
                    spell.Type = "DexterityBuff";
                    m_basedex = new Spell(spell, 50);
                }
                return m_basedex;
            }
        }

        /// <summary>
        /// Pet Str/Con buff
        /// </summary>
        public static Spell PetStrConBuff
        {
            get
            {
                if (m_strcon == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1517;
                    spell.Icon = 1517;
                    spell.Duration = 65535;
                    spell.Value = 75;
                    spell.Name = "Strength/Constitution Buff";
                    spell.Description = "Increases Str/Con for a character";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100015;
                    spell.Target = "Pet";
                    spell.Type = "StrengthConstitutionBuff";
                    m_strcon = new Spell(spell, 50);
                }
                return m_strcon;
            }
        }

        /// <summary>
        /// Pet Dex/Qui buff
        /// </summary>
        public static Spell PetDexQuiBuff
        {
            get
            {
                if (m_dexqui == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1526;
                    spell.Icon = 1526;
                    spell.Duration = 65535;
                    spell.Value = 75;
                    spell.Name = "Dexterity/Quickness Buff";
                    spell.Description = "Decreases Dexterity and Quickness for a character.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100016;
                    spell.Target = "Pet";
                    spell.Type = "DexterityQuicknessBuff";
                    m_dexqui = new Spell(spell, 50);
                }
                return m_dexqui;
            }
        }

        /// <summary>
        /// Pet Acuity buff
        /// </summary>
        public static Spell PetAcuityBuff
        {
            get
            {
                if (m_acuity == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1538;
                    spell.Icon = 1538;
                    spell.Duration = 65535;
                    spell.Value = 75;
                    spell.Name = "Acuity Buff Buff";
                    spell.Description = "Increases Acuity (casting attribute) for a character.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100017;
                    spell.Target = "Pet";
                    spell.Type = "AcuityBuff";
                    m_acuity = new Spell(spell, 50);
                }
                return m_acuity;
            }
        }
        /// <summary>
        /// Pet Spec Af buff
        /// </summary>
        public static Spell PetSpecAFBuff
        {
            get
            {
                if (m_specaf == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 1506;
                    spell.Icon = 1506;
                    spell.Duration = 65535;
                    spell.Value = 75;
                    spell.Name = "Spec AF Buff";
                    spell.Description = "Adds to the recipient's Armor Factor (AF), resulting in better protection against some forms of attack. It acts in addition to any armor the target is wearing.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100014;
                    spell.Target = "Pet";
                    spell.Type = "ArmorFactorBuff";
                    m_specaf = new Spell(spell, 50);
                }
                return m_specaf;
            }
        }
        /// <summary>
        /// Pet DamageAdd buff
        /// </summary>
        public static Spell PetDmgaddBuff
        {
            get
            {
                if (m_dmgadd == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 18;
                    spell.Icon = 18;
                    spell.Duration = 65535;
                    spell.Damage = 7.5;
                    spell.DamageType = 5;
                    spell.Name = "Damage Add Buff";
                    spell.Description = "Target's melee attacks do additional damage.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100019;
                    spell.Target = "Pet";
                    spell.Type = "DamageAdd";
                    m_dmgadd = new Spell(spell, 50);
                }
                return m_dmgadd;
            }
        }

        /// <summary>
        /// Pet DamageAdd buff
        /// </summary>
        public static Spell PetHasteBuff
        {
            get
            {
                if (m_haste == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 407;
                    spell.Icon = 407;
                    spell.Duration = 65535;
                    spell.Value = 15;
                    spell.Name = "Haste Buff";
                    spell.Description = "Increases the target's combat speed.";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100020;
                    spell.Target = "Pet";
                    spell.Type = "CombatSpeedBuff";
                    m_haste = new Spell(spell, 50);
                }
                return m_haste;
            }
        }
        public static Spell PetMagicBuff
        {
            get
            {
                if (m_abs == null)
                {
                    DBSpell spell = new DBSpell();
                    spell.AllowAdd = false;
                    spell.CastTime = 0;
                    spell.ClientEffect = 407;
                    spell.Icon = 407;
                    spell.Duration = 65535;
                    spell.Damage = 75;
                    spell.Name = "Abs Buff";
                    spell.Description = "Increases Magic Abs";
                    spell.Range = WorldMgr.VISIBILITY_DISTANCE;
                    spell.SpellID = 100020;
                    spell.Target = "Pet";
                    spell.Type = "PetMagicBuff";
                    m_haste = new Spell(spell, 50);
                }
                return m_haste;
            }
        }
        #endregion SpellCasting
    }
}
namespace DOL.GS.Spells
{
    #region PetMagicBuff
    [SpellHandlerAttribute("PetMagicBuff")]
    public class PetMagicBuff : SpellHandler
    {
        public const string ABLATIVE_HP = "ablative hp";
        public override void OnEffectStart(GameSpellEffect effect)
        {
            GamePlayer player = Caster as GamePlayer;
            base.OnEffectStart(effect);
            effect.Owner.TempProperties.setProperty(ABLATIVE_HP, 100000000);
            GameEventMgr.AddHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            eChatType toLiving = (Spell.Pulse == 0) ? eChatType.CT_Spell : eChatType.CT_SpellPulse;
            eChatType toOther = (Spell.Pulse == 0) ? eChatType.CT_System : eChatType.CT_SpellPulse;
            MessageToLiving(effect.Owner, Spell.Message1, toLiving);
            Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message2, effect.Owner.GetName(0, false)), toOther, effect.Owner);
        }
        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            GamePlayer player = Caster as GamePlayer;
            GameEventMgr.RemoveHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            effect.Owner.TempProperties.removeProperty(ABLATIVE_HP);
            if (!noMessages && Spell.Pulse == 0)
            {
                MessageToLiving(effect.Owner, Spell.Message3, eChatType.CT_SpellExpires);
                Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message4, effect.Owner.GetName(0, false)), eChatType.CT_SpellExpires, effect.Owner);
            }
            return 0;
        }
        public override void FinishSpellCast(GameLiving target)
        {
            m_caster.Mana -= PowerCost(target);
            base.FinishSpellCast(target);
        }
        private void OnAttack(DOLEvent e, object sender, EventArgs arguments)
        {
            GameLiving living = sender as GameLiving;
            if (living == null) return;
            AttackedByEnemyEventArgs attackedByEnemy = arguments as AttackedByEnemyEventArgs;
            AttackData ad = null;
            if (attackedByEnemy != null)
                ad = attackedByEnemy.AttackData;

            if (ad == null || (ad.AttackResult == GameLiving.eAttackResult.HitStyle && ad.AttackResult == GameLiving.eAttackResult.HitUnstyled))
                return;
            if (ad.IsMeleeAttack && ad.AttackType == AttackData.eAttackType.Ranged)
                return;
            int ablativehp = living.TempProperties.getProperty<int>(ABLATIVE_HP);
            double absorbPercent = 0;
            if (Spell.Damage > 0)
                absorbPercent = Spell.Damage;
            if (absorbPercent > 100)
                absorbPercent = 100;
            int damageAbsorbed = (int)(0.01 * absorbPercent * (ad.Damage + ad.CriticalDamage));
            if (damageAbsorbed > ablativehp)
                damageAbsorbed = ablativehp;
            ablativehp -= damageAbsorbed;
            ad.Damage -= damageAbsorbed;
            OnDamageAbsorbed(ad, damageAbsorbed);
            if (ablativehp <= 0)
            {
                GameSpellEffect effect = SpellHandler.FindEffectOnTarget(living, this);
                if (effect != null)
                    effect.Cancel(false);
            }
            else
            {
                living.TempProperties.setProperty(ABLATIVE_HP, ablativehp);
            }
        }

        protected virtual void OnDamageAbsorbed(AttackData ad, int DamageAmount)
        {
        }

        public override PlayerXEffect GetSavedEffect(GameSpellEffect e)
        {
            if ( //VaNaTiC-> this cannot work, cause PulsingSpellEffect is derived from object and only implements IConcEffect
                //e is PulsingSpellEffect ||
                //VaNaTiC<-
                Spell.Pulse != 0 || Spell.Concentration != 0 || e.RemainingTime < 1)
                return null;
            PlayerXEffect eff = new PlayerXEffect();
            eff.Var1 = Spell.ID;
            eff.Duration = e.RemainingTime;
            eff.IsHandler = true;
            eff.Var2 = (int)Spell.Value;
            eff.SpellLine = SpellLine.KeyName;
            return eff;
        }

        public override void OnEffectRestored(GameSpellEffect effect, int[] vars)
        {
            effect.Owner.TempProperties.setProperty(ABLATIVE_HP, (int)vars[1]);
            GameEventMgr.AddHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
        }

        public override int OnRestoredEffectExpires(GameSpellEffect effect, int[] vars, bool noMessages)
        {
            GameEventMgr.RemoveHandler(effect.Owner, GameLivingEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            effect.Owner.TempProperties.removeProperty(ABLATIVE_HP);
            if (!noMessages && Spell.Pulse == 0)
            {
                MessageToLiving(effect.Owner, Spell.Message3, eChatType.CT_SpellExpires);
                Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message4, effect.Owner.GetName(0, false)), eChatType.CT_SpellExpires, effect.Owner);
            }
            return 0;
        }
        public PetMagicBuff(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion
}