using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace FFXIV_Vibe_Plugin.Commons
{
    public class Structures
    {
        public enum ActionEffectType : byte
        {
            Any = 0,
            Miss = 1,
            FullResist = 2,
            Damage = 3,
            Heal = 4,
            BlockedDamage = 5,
            ParriedDamage = 6,
            Invulnerable = 7,
            NoEffectText = 8,
            Unknown_0 = 9,
            MpLoss = 10, // 0x0A
            MpGain = 11, // 0x0B
            TpLoss = 12, // 0x0C
            TpGain = 13, // 0x0D
            GpGain = 14, // 0x0E
            ApplyStatusEffectTarget = 15, // 0x0F
            ApplyStatusEffectSource = 16, // 0x10
            StatusNoEffect = 20, // 0x14
            Taunt = 24, // 0x18
            StartActionCombo = 27, // 0x1B
            ComboSucceed = 28, // 0x1C
            Knockback = 33, // 0x21
            Mount = 40, // 0x28
            VFX = 59, // 0x3B
            Transport = 60, // 0x3C
            MountJapaneseVersion = 240, // 0xF0
        }

        public enum DamageType
        {
            Unknown = 0,
            Slashing = 1,
            Piercing = 2,
            Blunt = 3,
            Magic = 5,
            Darkness = 6,
            Physical = 7,
            LimitBreak = 8,
        }

        public struct EffectEntry
        {
            public Structures.ActionEffectType type;
            public byte param0;
            public byte param1;
            public byte param2;
            public byte mult;
            public byte flags;
            public ushort value;

            public EffectEntry(
              Structures.ActionEffectType type,
              byte param0,
              byte param1,
              byte param2,
              byte mult,
              byte flags,
              ushort value)
            {
                this.type = Structures.ActionEffectType.Any;
                this.param0 = (byte)0;
                this.param1 = (byte)0;
                this.param2 = (byte)0;
                this.mult = (byte)0;
                this.flags = (byte)0;
                this.value = (ushort)0;
                this.type = type;
                this.param0 = param0;
                this.param1 = param1;
                this.param2 = param2;
                this.mult = mult;
                this.flags = flags;
                this.value = value;
            }

            public override string ToString()
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 8);
                interpolatedStringHandler.AppendLiteral("type: ");
                interpolatedStringHandler.AppendFormatted<Structures.ActionEffectType>(this.type);
                interpolatedStringHandler.AppendLiteral(", p0: ");
                interpolatedStringHandler.AppendFormatted<byte>(this.param0);
                interpolatedStringHandler.AppendLiteral(", p1: ");
                interpolatedStringHandler.AppendFormatted<byte>(this.param1);
                interpolatedStringHandler.AppendLiteral(", p2: ");
                interpolatedStringHandler.AppendFormatted<byte>(this.param2);
                interpolatedStringHandler.AppendLiteral(", mult: ");
                interpolatedStringHandler.AppendFormatted<byte>(this.mult);
                interpolatedStringHandler.AppendLiteral(", flags: ");
                interpolatedStringHandler.AppendFormatted<byte>(this.flags);
                interpolatedStringHandler.AppendLiteral(" | ");
                interpolatedStringHandler.AppendFormatted(Convert.ToString(this.flags, 2));
                interpolatedStringHandler.AppendLiteral(", value: ");
                interpolatedStringHandler.AppendFormatted<ushort>(this.value);
                return interpolatedStringHandler.ToStringAndClear();
            }
        }

        public struct Player
        {
            public int Id;
            public string Name;
            public string? Info;

            public Player(int id, string name, string? info = null)
            {
                this.Id = id;
                this.Name = name;
                this.Info = info;
            }

            public override string ToString()
            {
                if (this.Info != null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 3);
                    interpolatedStringHandler.AppendFormatted(this.Name);
                    interpolatedStringHandler.AppendLiteral("(");
                    interpolatedStringHandler.AppendFormatted<int>(this.Id);
                    interpolatedStringHandler.AppendLiteral(") [info:");
                    interpolatedStringHandler.AppendFormatted(this.Info);
                    interpolatedStringHandler.AppendLiteral("]");
                    return interpolatedStringHandler.ToStringAndClear();
                }
                DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(2, 2);
                interpolatedStringHandler1.AppendFormatted(this.Name);
                interpolatedStringHandler1.AppendLiteral("(");
                interpolatedStringHandler1.AppendFormatted<int>(this.Id);
                interpolatedStringHandler1.AppendLiteral(")");
                return interpolatedStringHandler1.ToStringAndClear();
            }
        }

        public struct Spell
        {
            public int Id;
            public string Name;
            public Structures.Player Player;
            public int[]? Amounts;
            public float AmountAverage;
            public List<Structures.Player>? Targets;
            public Structures.DamageType DamageType;
            public Structures.ActionEffectType ActionEffectType;

            public Spell(
              int id,
              string name,
              Structures.Player player,
              int[]? amounts,
              float amountAverage,
              List<Structures.Player>? targets,
              Structures.DamageType damageType,
              Structures.ActionEffectType actionEffectType)
            {
                this.Name = "Undefined_Spell_Name";
                this.DamageType = Structures.DamageType.Unknown;
                this.ActionEffectType = Structures.ActionEffectType.Any;
                this.Id = id;
                this.Name = name;
                this.Player = player;
                this.Amounts = amounts;
                this.AmountAverage = amountAverage;
                this.Targets = targets;
                this.DamageType = damageType;
                this.ActionEffectType = actionEffectType;
            }

            public override string ToString()
            {
                string str = "";
                if (this.Targets != null)
                    str = this.Targets.Count <= 0 ? "*no target*" : string.Join<Structures.Player>(",", (IEnumerable<Structures.Player>)this.Targets);
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 5);
                interpolatedStringHandler.AppendFormatted<Structures.Player>(this.Player);
                interpolatedStringHandler.AppendLiteral(" casts ");
                interpolatedStringHandler.AppendFormatted(this.Name);
                interpolatedStringHandler.AppendLiteral("#");
                interpolatedStringHandler.AppendFormatted<Structures.ActionEffectType>(this.ActionEffectType);
                interpolatedStringHandler.AppendLiteral(" on: ");
                interpolatedStringHandler.AppendFormatted(str);
                interpolatedStringHandler.AppendLiteral(". Avg: ");
                interpolatedStringHandler.AppendFormatted<float>(this.AmountAverage);
                return interpolatedStringHandler.ToStringAndClear();
            }
        }
    }
}
