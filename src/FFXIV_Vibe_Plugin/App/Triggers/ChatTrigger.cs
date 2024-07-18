using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace FFXIV_Vibe_Plugin.Triggers
{
    [Serializable]
    public class ChatTrigger : IComparable
    {
        public ChatTrigger(int intensity, string text)
        {
            this.Intensity = intensity;
            this.Text = text;
        }

        public int Intensity { get; }

        public string Text { get; }

        public override string ToString()
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
            interpolatedStringHandler.AppendLiteral("Trigger(intensity: ");
            interpolatedStringHandler.AppendFormatted<int>(this.Intensity);
            interpolatedStringHandler.AppendLiteral(", text: '");
            interpolatedStringHandler.AppendFormatted(this.Text);
            interpolatedStringHandler.AppendLiteral("')");
            return interpolatedStringHandler.ToStringAndClear();
        }

        public string ToConfigString()
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted<int>(this.Intensity);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(this.Text);
            return interpolatedStringHandler.ToStringAndClear();
        }

        public int CompareTo(object? obj)
        {
            return this.Intensity.CompareTo(obj is ChatTrigger chatTrigger ? chatTrigger.Intensity : 0);
        }
    }
}
