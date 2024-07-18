using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

#nullable enable
namespace FFXIV_Vibe_Plugin.Triggers
{
    public class Trigger : IComparable<Trigger>, IEquatable<Trigger>
    {
        private static readonly int _initAmountMinValue = -1;
        private static readonly int _initAmountMaxValue = 10000000;
        public bool Enabled = true;
        public int SortOder = -1;
        public readonly string Id = "";
        public string Name = "";
        public string Description = "";
        public int Kind;
        public int ActionEffectType;
        public int Direction;
        public string ChatText = "hello world";
        public string SpellText = "";
        public int AmountMinValue = Trigger._initAmountMinValue;
        public int AmountMaxValue = Trigger._initAmountMaxValue;
        public bool AmountInPercentage;
        public string FromPlayerName = "";
        public float StartAfter;
        public float StopAfter;
        public int Priority;
        public readonly List<int> AllowedChatTypes = new List<int>();
        public List<TriggerDevice> Devices = new List<TriggerDevice>();

        public Trigger(string name)
        {
            this.Name = name;
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            this.Id = BitConverter.ToString(SHA256.Create().ComputeHash(bytes)).Replace("-", string.Empty);
        }

        public override string ToString()
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
            interpolatedStringHandler.AppendLiteral("Trigger(name=");
            interpolatedStringHandler.AppendFormatted(this.Name);
            interpolatedStringHandler.AppendLiteral(", id=");
            interpolatedStringHandler.AppendFormatted(this.GetShortID());
            interpolatedStringHandler.AppendLiteral(")");
            return interpolatedStringHandler.ToStringAndClear();
        }

        public int CompareTo(Trigger? other) => other == null ? 1 : other.Name.CompareTo(this.Name);

        public bool Equals(Trigger? other) => other != null && this.Name.Equals(other.Name);

        public string GetShortID() => this.Id.Substring(0, 5);

        public void Reset()
        {
            this.AmountMaxValue = Trigger._initAmountMaxValue;
            this.AmountMinValue = Trigger._initAmountMinValue;
        }
    }
}
