using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using FFXIV_Vibe_Plugin.App;
using FFXIV_Vibe_Plugin.Device;
using ImGuiNET;
using System.Numerics;

#nullable enable
namespace FFXIV_Vibe_Plugin.UI
{
    internal class UIConnect
    {
        public static void Draw(
          Configuration configuration,
          ConfigurationProfile configurationProfile,
          Main plugin,
          DevicesController devicesController,
          Premium premium)
        {
            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Server address & port");
            ImGui.BeginChild("###Server", new Vector2(-1f, 40f), true);
            string buttplugServerHost = configurationProfile.BUTTPLUG_SERVER_HOST;
            ImGui.SetNextItemWidth(200f);
            if (ImGui.InputText("##serverHost", ref buttplugServerHost, 99U))
            {
                configurationProfile.BUTTPLUG_SERVER_HOST = buttplugServerHost.Trim().ToLower();
                configuration.Save();
            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Go in the option tab if you need WSS (default: 127.0.0.1)");
            ImGui.SameLine();
            int buttplugServerPort = configurationProfile.BUTTPLUG_SERVER_PORT;
            ImGui.SetNextItemWidth(100f);
            if (ImGui.InputInt("##serverPort", ref buttplugServerPort, 10))
            {
                configurationProfile.BUTTPLUG_SERVER_PORT = buttplugServerPort;
                configuration.Save();
            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Use '-1' as port number to not define it (default: 12345)");
            ImGui.EndChild();
            ImGui.Spacing();
            ImGui.BeginChild("###Main_Connection", new Vector2(-1f, 40f), true);
            if (!devicesController.IsConnected())
            {
                if (ImGui.Button("Connect", new Vector2(100f, 24f)))
                    plugin.Command_DeviceController_Connect();
            }
            else if (ImGui.Button("Disconnect", new Vector2(100f, 24f)))
                devicesController.Disconnect();
            ImGui.SameLine();
            bool autoConnect = configurationProfile.AUTO_CONNECT;
            if (ImGui.Checkbox("Automatically connects. ", ref autoConnect))
            {
                configurationProfile.AUTO_CONNECT = autoConnect;
                configuration.Save();
            }
            ImGui.EndChild();
            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Premium settings");
            ImGui.BeginChild("###Premium", new Vector2(-1f, 60f), true);
            ImGui.Text("Premium token");
            ImGui.SameLine();
            string premiumToken = configurationProfile.PREMIUM_TOKEN;
            ImGui.SetNextItemWidth(200f);
            if (ImGui.InputText("##premium_token", ref premiumToken, 200U) && premiumToken != configurationProfile.PREMIUM_TOKEN_SECRET)
            {
                configurationProfile.PREMIUM_TOKEN = premiumToken == "" ? "" : "********";
                configurationProfile.PREMIUM_TOKEN_SECRET = premiumToken;
                configuration.Save();
                premium.updateStatus();
            }
            ImGui.SameLine();
            if (!premium.invalidToken)
                ImGui.TextColored(ImGuiColors.HealerGreen, "VALID TOKEN");
            else if (premiumToken == "")
                ImGui.TextColored(ImGuiColors.DalamudGrey2, "Please enter your Premium token");
            else
                ImGui.TextColored(ImGuiColors.DPSRed, "Invalid token. " + premium.serverMsg);
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Copy/Paste your personal token. Don't share your configuration file since it will be present in there. Abuse and multiple connections with the same token will result on a permanent ban.");
            ImGui.TextColored(ImGuiColors.DalamudGrey, "Subscribe on patreon to get your token (link at the top). Token can not be used on multiple machines at the same time.");
            ImGui.EndChild();
        }
    }
}
