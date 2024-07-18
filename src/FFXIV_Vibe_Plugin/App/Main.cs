using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIV_Vibe_Plugin.App;
using FFXIV_Vibe_Plugin.Commons;
using FFXIV_Vibe_Plugin.Device;
using FFXIV_Vibe_Plugin.Experimental;
using FFXIV_Vibe_Plugin.Hooks;
using FFXIV_Vibe_Plugin.Migrations;
using FFXIV_Vibe_Plugin.Triggers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace FFXIV_Vibe_Plugin
{
    public class Main
    {
        public readonly string CommandName = "";
        public PluginUI PluginUi { get; init; }
        public Configuration Configuration { get; init; }

        private bool ThreadMonitorPartyListRunning = true;
        private readonly Plugin Plugin;
        private readonly bool wasInit;
        private readonly string ShortName = "";
        private bool _firstUpdated;
        private readonly PlayerStats PlayerStats;
        private ConfigurationProfile ConfigurationProfile;
        private readonly Logger Logger;
        private readonly ActionEffect hook_ActionEffect;
        private readonly DevicesController DeviceController;
        private readonly TriggersController TriggersController;
        private readonly Patterns Patterns;
        private Premium Premium;
        private readonly NetworkCapture experiment_networkCapture;


        [PluginService]
        private IChatGui? DalamudChat { get; init; }

        [PluginService]
        private IGameNetwork GameNetwork { get; init; }

        [PluginService]
        private IDataManager DataManager { get; init; }

        [PluginService]
        private IClientState ClientState { get; init; }

        [PluginService]
        private ISigScanner Scanner { get; init; }

        [PluginService]
        private IObjectTable GameObjects { get; init; }

        [PluginService]
        private IDalamudPluginInterface PluginInterface { get; init; }

        [PluginService]
        private IPartyList PartyList { get; init; }

        [PluginService]
        private IGameInteropProvider InteropProvider { get; init; }

        [PluginService]
        public IPluginLog? PluginLog { get; init; }

        [PluginService]
        private ICommandManager CommandManager { get; init; }

        public Main(Plugin plugin, string commandName, string shortName, Configuration configuration)
        {
            Main main = this;
            Plugin = plugin;
            CommandName = commandName;
            ShortName = shortName;
            Configuration = configuration;

            if (DalamudChat != null)
            {
                DalamudChat.ChatMessage += new IChatGui.OnMessageDelegate(ChatWasTriggered);
            }

            Logger = new Logger(DalamudChat, ShortName, Logger.LogLevel.VERBOSE);

            if (DalamudChat == null)
                Logger.Error("DalamudChat was not initialized correctly.");

            Migration migration = new Migration(Configuration, Logger);
            ConfigurationProfile = Configuration.GetDefaultProfile();
            Patterns = new Patterns();
            Patterns.SetCustomPatterns(ConfigurationProfile.PatternList);
            DeviceController = new DevicesController(Logger, Configuration, ConfigurationProfile, Patterns);

            if (ConfigurationProfile.AUTO_CONNECT)
                new Thread((ThreadStart)(() =>
                {
                    Thread.Sleep(2000);
                    main.Command_DeviceController_Connect();
                })).Start();

            hook_ActionEffect = new ActionEffect(DataManager, Logger, (SigScanner)Scanner, ClientState, GameObjects, InteropProvider);

            hook_ActionEffect.ReceivedEvent += new EventHandler<HookActionEffects_ReceivedEventArgs>(SpellWasTriggered);

            ClientState.Login += new Action(ClientState_LoginEvent);

            PlayerStats = new PlayerStats(Logger, ClientState);

            PlayerStats.Event_CurrentHpChanged += new EventHandler(PlayerCurrentHPChanged);

            PlayerStats.Event_MaxHpChanged += new EventHandler(PlayerCurrentHPChanged);

            TriggersController = new TriggersController(Logger, PlayerStats, ConfigurationProfile);

            Premium = new Premium(Logger, ConfigurationProfile);

            PluginUi = new PluginUI(this, Logger, PluginInterface, Configuration, ConfigurationProfile, DeviceController, TriggersController, Patterns, Premium);

            experiment_networkCapture = new NetworkCapture(Logger, GameNetwork);

            new Thread((ThreadStart)(() => main.MonitorPartyList(PartyList))).Start();

            SetProfile(Configuration.CurrentProfileName);

            wasInit = true;
        }

        public void Dispose()
        {
            this.Logger.Debug("Disposing plugin...");

            if (!wasInit)
                return;

            if (DeviceController != null)
            {
                try
                {
                    DeviceController.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error("App.Dispose: " + ex.Message);
                }
            }

            if (DalamudChat != null)
            {
                DalamudChat.ChatMessage -= new IChatGui.OnMessageDelegate(ChatWasTriggered);
            }

            hook_ActionEffect.Dispose();
            PluginUi.Dispose();
            experiment_networkCapture.Dispose();
            Premium.Dispose();

            Logger.Debug("Plugin disposed!");

            ThreadMonitorPartyListRunning = false;
        }

        public static string GetHelp(string command)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(126, 5);
            interpolatedStringHandler.AppendLiteral("Usage:\n      ");
            interpolatedStringHandler.AppendFormatted(command);
            interpolatedStringHandler.AppendLiteral(" config      \n      ");
            interpolatedStringHandler.AppendFormatted(command);
            interpolatedStringHandler.AppendLiteral(" connect\n      ");
            interpolatedStringHandler.AppendFormatted(command);
            interpolatedStringHandler.AppendLiteral(" disconnect\n      ");
            interpolatedStringHandler.AppendFormatted(command);
            interpolatedStringHandler.AppendLiteral(" send <0-100> # Send vibe intensity to all toys\n      ");
            interpolatedStringHandler.AppendFormatted(command);
            interpolatedStringHandler.AppendLiteral(" stop\n");
            return interpolatedStringHandler.ToStringAndClear();
        }

        public void OnCommand(string command, string args)
        {
            if (args.Length == 0)
                this.DisplayUI();
            else if (args.StartsWith("help"))
                this.Logger.Chat(Main.GetHelp("/" + this.ShortName));
            else if (args.StartsWith("config"))
                this.DisplayConfigUI();
            else if (args.StartsWith("connect"))
                this.Command_DeviceController_Connect();
            else if (args.StartsWith("disconnect"))
                this.Command_DeviceController_Disconnect();
            else if (args.StartsWith("send"))
                this.Command_SendIntensity(args);
            else if (args.StartsWith("stop"))
                this.DeviceController.SendVibeToAll(0);
            else if (args.StartsWith("profile"))
                this.Command_ProfileSet(args);
            else if (args.StartsWith("exp_network_start"))
                this.experiment_networkCapture.StartNetworkCapture();
            else if (args.StartsWith("exp_network_stop"))
                this.experiment_networkCapture.StopNetworkCapture();
            else
                this.Logger.Chat("Unknown subcommand: " + args);
        }

        private void FirstUpdated()
        {
            this.Logger.Debug("First updated");
            if (this.ConfigurationProfile == null || !this.ConfigurationProfile.AUTO_OPEN)
                return;
            this.DisplayUI();
        }

        private void DisplayUI() => this.Plugin.DrawConfigUI();

        private void DisplayConfigUI() => this.Plugin.DrawConfigUI();

        public void DrawUI()
        {
            if (this.PluginUi == null)
                return;
            if (this.ClientState != null && this.ClientState.IsLoggedIn)
                this.PlayerStats.Update(this.ClientState);
            if (this._firstUpdated)
                return;
            this.FirstUpdated();
            this._firstUpdated = true;
        }

        public void Command_DeviceController_Connect()
        {
            if (this.DeviceController == null)
            {
                this.Logger.Error("No device controller available to connect.");
            }
            else
            {
                if (this.ConfigurationProfile == null)
                    return;
                this.DeviceController.Connect(this.ConfigurationProfile.BUTTPLUG_SERVER_HOST, this.ConfigurationProfile.BUTTPLUG_SERVER_PORT);
            }
        }

        private void Command_DeviceController_Disconnect()
        {
            if (this.DeviceController == null)
            {
                this.Logger.Error("No device controller available to disconnect.");
            }
            else
            {
                try
                {
                    this.DeviceController.Disconnect();
                }
                catch (Exception ex)
                {
                    this.Logger.Error("App.Command_DeviceController_Disconnect: " + ex.Message);
                }
            }
        }

        private void Command_SendIntensity(string args)
        {
            int intensity;

            try
            {
                intensity = int.Parse(args.Split(" ", 2)[1]);

                Logger logger = this.Logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
                interpolatedStringHandler.AppendLiteral("Command Send intensity ");
                interpolatedStringHandler.AppendFormatted<int>(intensity);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();

                logger.Chat(stringAndClear);

            } catch (Exception ex) when (ex is FormatException || ex is IndexOutOfRangeException)
            {
                this.Logger.Error("Malformed arguments for send [intensity].", ex);
                return;
            }

            if (this.DeviceController == null)
                this.Logger.Error("No device controller available to send intensity.");
            else
                this.DeviceController.SendVibeToAll(intensity);
        }

        private void SpellWasTriggered(object? sender, HookActionEffects_ReceivedEventArgs args)
        {
            if (this.TriggersController == null)
            {
                this.Logger.Warn("SpellWasTriggered: TriggersController not init yet, ignoring spell...");
            }
            else
            {
                Structures.Spell spell = args.Spell;
                if (this.ConfigurationProfile != null && this.ConfigurationProfile.VERBOSE_SPELL)
                {
                    Logger logger = this.Logger;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
                    interpolatedStringHandler.AppendLiteral("VERBOSE_SPELL: ");
                    interpolatedStringHandler.AppendFormatted<Structures.Spell>(spell);
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    logger.Debug(stringAndClear);
                }
                foreach (Trigger trigger in this.TriggersController.CheckTrigger_Spell(spell))
                    this.DeviceController.SendTrigger(trigger);
            }
        }
            
        private void ChatWasTriggered(XivChatType chatType, int timestamp, ref SeString _sender, ref SeString _message, ref bool isHandled)
        {
            string ChatFromPlayerName = _sender.ToString();
            if (this.TriggersController == null)
            {
                this.Logger.Warn("ChatWasTriggered: TriggersController not init yet, ignoring chat...");
            }
            else
            {
                if (this.ConfigurationProfile != null && this.ConfigurationProfile.VERBOSE_CHAT)
                {
                    string str = chatType.ToString();
                    Logger logger = this.Logger;
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 3);
                    interpolatedStringHandler.AppendLiteral("VERBOSE_CHAT: ");
                    interpolatedStringHandler.AppendFormatted(ChatFromPlayerName);
                    interpolatedStringHandler.AppendLiteral(" type=");
                    interpolatedStringHandler.AppendFormatted(str);
                    interpolatedStringHandler.AppendLiteral(": ");
                    interpolatedStringHandler.AppendFormatted<SeString>(_message);
                    string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                    logger.Debug(stringAndClear);
                }
                foreach (Trigger trigger in this.TriggersController.CheckTrigger_Chat(chatType, ChatFromPlayerName, _message.TextValue))
                    this.DeviceController.SendTrigger(trigger);
            }
        }

        private void PlayerCurrentHPChanged(object? send, EventArgs e)
        {
            float currentHp = this.PlayerStats.GetCurrentHP();
            float maxHp = this.PlayerStats.GetMaxHP();
            if (this.TriggersController == null)
            {
                this.Logger.Warn("PlayerCurrentHPChanged: TriggersController not init yet, ignoring HP change...");
            }
            else
            {
                float percentageHP = currentHp * 100f / maxHp;
                List<Trigger> triggerList = this.TriggersController.CheckTrigger_HPChanged((int)currentHp, percentageHP);
                Logger logger = this.Logger;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 3);
                interpolatedStringHandler.AppendLiteral("PlayerCurrentHPChanged SelfPlayer ");
                interpolatedStringHandler.AppendFormatted<float>(currentHp);
                interpolatedStringHandler.AppendLiteral("/");
                interpolatedStringHandler.AppendFormatted<float>(maxHp);
                interpolatedStringHandler.AppendLiteral(" ");
                interpolatedStringHandler.AppendFormatted<float>(percentageHP, "0.##");
                interpolatedStringHandler.AppendLiteral("%");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                logger.Verbose(stringAndClear);
                foreach (Trigger trigger in triggerList)
                    this.DeviceController.SendTrigger(trigger);
            }
        }

        private void ClientState_LoginEvent() => this.PlayerStats.Update(this.ClientState);

        private void MonitorPartyList(IPartyList partyList)
        {
            while (this.ThreadMonitorPartyListRunning)
            {
                if (this.TriggersController == null)
                {
                    this.Logger.Warn("HPChangedOtherPlayer: TriggersController not init yet, ignoring HP change other...");
                    break;
                }
                if (partyList.Length >= 0)
                {
                    foreach (Trigger trigger in this.TriggersController.CheckTrigger_HPChangedOther(partyList))
                    {
                        Logger logger = this.Logger;
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 3);
                        interpolatedStringHandler.AppendLiteral("HPChangedOtherPlayer ");
                        interpolatedStringHandler.AppendFormatted(trigger.FromPlayerName);
                        interpolatedStringHandler.AppendLiteral(" min:");
                        interpolatedStringHandler.AppendFormatted<int>(trigger.AmountMinValue);
                        interpolatedStringHandler.AppendLiteral(" max:");
                        interpolatedStringHandler.AppendFormatted<int>(trigger.AmountMaxValue);
                        interpolatedStringHandler.AppendLiteral(" triggered!");
                        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                        logger.Verbose(stringAndClear);
                        this.DeviceController.SendTrigger(trigger);
                    }
                }
                Thread.Sleep(500);
            }
        }

        public bool SetProfile(string profileName)
        {
            if (!this.Configuration.SetCurrentProfile(profileName))
            {
                this.Logger.Warn("You are trying to use profile " + profileName + " which can't be found");
                return false;
            }
            ConfigurationProfile profile = this.Configuration.GetProfile(profileName);
            if (profile != null)
            {
                this.ConfigurationProfile = profile;
                this.PluginUi.SetProfile(this.ConfigurationProfile);
                this.DeviceController.SetProfile(this.ConfigurationProfile);
                this.TriggersController.SetProfile(this.ConfigurationProfile);
            }
            return true;
        }

        private void Command_ProfileSet(string args)
        {
            List<string> list = ((IEnumerable<string>)args.Split(" ")).ToList<string>();
            if (list.Count == 2)
            {
                if (this.Premium.IsPremium())
                    this.SetProfile(list[1]);
                else
                    this.Logger.Warn("Premium feature Only: /fvp profile [name]");
            }
            else
                this.Logger.Error("Wrong command: /fvp profile [name]");
        }
    }
}
