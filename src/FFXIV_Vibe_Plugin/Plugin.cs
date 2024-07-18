using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System;
using FFXIV_Vibe_Plugin.App;

#nullable enable
namespace FFXIV_Vibe_Plugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

        public string Name => "FFXIV Vibe Plugin";

        public WindowSystem WindowSystem = new("SamplePlugin");
        private Main app;

        public static readonly string ShortName = "FVP";
        public readonly string CommandName = "/fvp";

        public Configuration Configuration { get; init; }

        public Plugin()
        {
            this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(PluginInterface);

            app = PluginInterface.Create<Main>(this, CommandName, ShortName, Configuration)!;

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A vibe plugin for fun..."
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            WindowSystem.AddWindow(app.PluginUi);
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            CommandManager.RemoveHandler(CommandName);
            app.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            app.OnCommand(command, args);
        }

        private void DrawUI()
        {
            WindowSystem.Draw();

            if (app == null)
                return;

            app.DrawUI();
        }

        public void DrawConfigUI()
        {
            this.WindowSystem.Windows[0].IsOpen = true;
        }
    }
}
