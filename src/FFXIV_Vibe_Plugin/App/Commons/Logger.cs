using Dalamud.Logging;
using Dalamud.Plugin.Services;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace FFXIV_Vibe_Plugin.Commons
{
    public class Logger
    {
        private readonly IChatGui? DalamudChatGui;
        private readonly string name = "";
        private readonly Logger.LogLevel log_level = Logger.LogLevel.DEBUG;
        private readonly string prefix = ">";

        public Logger(IChatGui? DalamudChatGui, string name, Logger.LogLevel log_level)
        {
            this.DalamudChatGui = DalamudChatGui;
            this.name = name;
            this.log_level = log_level;
        }

        public void Chat(string msg)
        {
            if (this.DalamudChatGui != null)
                this.DalamudChatGui.Print(this.FormatMessage(Logger.LogLevel.LOG, msg), (string)null, new ushort?());
            /*else
                PluginLog.LogError("No gui chat", Array.Empty<object>());*/
        }

        public void ChatError(string msg)
        {
            this.DalamudChatGui?.PrintError(this.FormatMessage(Logger.LogLevel.ERROR, msg), (string)null, new ushort?());
            this.Error(msg);
        }

        public void ChatError(string msg, Exception e)
        {
            string msg1 = this.FormatMessage(Logger.LogLevel.ERROR, msg, e);
            this.DalamudChatGui?.PrintError(msg1, (string)null, new ushort?());
            this.Error(msg1);
        }

        public void Verbose(string msg)
        {
            if (this.log_level > Logger.LogLevel.VERBOSE)
                return;
            // PluginLog.LogVerbose(this.FormatMessage(Logger.LogLevel.VERBOSE, msg), Array.Empty<object>());
        }

        public void Debug(string msg)
        {
            if (this.log_level > Logger.LogLevel.DEBUG)
                return;
            // PluginLog.LogDebug(this.FormatMessage(Logger.LogLevel.DEBUG, msg), Array.Empty<object>());
        }

        public void Log(string msg)
        {
            if (this.log_level > Logger.LogLevel.LOG)
                return;
            // PluginLog.Log(this.FormatMessage(Logger.LogLevel.LOG, msg), Array.Empty<object>());
        }

        public void Info(string msg)
        {
            if (this.log_level > Logger.LogLevel.INFO)
                return;
            // PluginLog.Information(this.FormatMessage(Logger.LogLevel.INFO, msg), Array.Empty<object>());
        }

        public void Warn(string msg)
        {
            if (this.log_level > Logger.LogLevel.WARN)
                return;
            // PluginLog.Warning(this.FormatMessage(Logger.LogLevel.WARN, msg), Array.Empty<object>());
        }

        public void Error(string msg)
        {
            if (this.log_level > Logger.LogLevel.ERROR)
                return;
            // PluginLog.Error(this.FormatMessage(Logger.LogLevel.ERROR, msg), Array.Empty<object>());
        }

        public void Error(string msg, Exception e)
        {
            if (this.log_level > Logger.LogLevel.ERROR)
                return;
            // PluginLog.Error(this.FormatMessage(Logger.LogLevel.ERROR, msg, e), Array.Empty<object>());
        }

        public void Fatal(string msg)
        {
            if (this.log_level > Logger.LogLevel.FATAL)
                return;
            // PluginLog.Fatal(this.FormatMessage(Logger.LogLevel.FATAL, msg), Array.Empty<object>());
        }

        public void Fatal(string msg, Exception e)
        {
            if (this.log_level > Logger.LogLevel.FATAL)
                return;
            // PluginLog.Fatal(this.FormatMessage(Logger.LogLevel.FATAL, msg, e), Array.Empty<object>());
        }

        private string FormatMessage(Logger.LogLevel type, string msg)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 4);
            interpolatedStringHandler.AppendFormatted(this.name != "" ? this.name + " " : "");
            interpolatedStringHandler.AppendFormatted<Logger.LogLevel>(type);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(this.prefix);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(msg);
            return interpolatedStringHandler.ToStringAndClear();
        }

        private string FormatMessage(Logger.LogLevel type, string msg, Exception e)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 5);
            interpolatedStringHandler.AppendFormatted(this.name != "" ? this.name + " " : "");
            interpolatedStringHandler.AppendFormatted<Logger.LogLevel>(type);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(this.prefix);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(e.Message);
            interpolatedStringHandler.AppendLiteral("\\n");
            interpolatedStringHandler.AppendFormatted(msg);
            return interpolatedStringHandler.ToStringAndClear();
        }

        public enum LogLevel
        {
            VERBOSE,
            DEBUG,
            LOG,
            INFO,
            WARN,
            ERROR,
            FATAL,
        }
    }
}
