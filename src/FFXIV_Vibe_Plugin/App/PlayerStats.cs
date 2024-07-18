using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIV_Vibe_Plugin.Commons;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace FFXIV_Vibe_Plugin
{
    public class PlayerStats
    {
        private readonly Logger Logger;
        private float _CurrentHp;
        private float _prevCurrentHp = -1f;
        private float _MaxHp;
        private float _prevMaxHp = -1f;
        public string PlayerName = "*unknown*";

        public event EventHandler? Event_CurrentHpChanged;

        public event EventHandler? Event_MaxHpChanged;

        public PlayerStats(Logger logger, IClientState clientState)
        {
            this.Logger = logger;
            this.UpdatePlayerState(clientState);
        }

        public void Update(IClientState clientState)
        {
            if (clientState == null || clientState.LocalPlayer == null) // A VERIFIER
                return;
            this.UpdatePlayerState(clientState);
            this.UpdatePlayerName(clientState);
            this.UpdateCurrentHp(clientState);
        }

        public void UpdatePlayerState(IClientState clientState)
        {
            if (clientState == null || clientState.LocalPlayer == null || (double)this._CurrentHp != -1.0 && (double)this._MaxHp != -1.0) // A VERIFIER
                return;
            Logger logger1 = this.Logger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
            interpolatedStringHandler.AppendLiteral("UpdatePlayerState ");
            interpolatedStringHandler.AppendFormatted<float>(this._CurrentHp);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted<float>(this._MaxHp);
            string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
            logger1.Debug(stringAndClear1);
            this._CurrentHp = this._prevCurrentHp = (float)((ICharacter)clientState.LocalPlayer).CurrentHp;
            this._MaxHp = this._prevMaxHp = (float)((ICharacter)clientState.LocalPlayer).MaxHp;
            Logger logger2 = this.Logger;
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
            interpolatedStringHandler.AppendLiteral("UpdatePlayerState ");
            interpolatedStringHandler.AppendFormatted<float>(this._CurrentHp);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted<float>(this._MaxHp);
            string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
            logger2.Debug(stringAndClear2);
        }

        public string UpdatePlayerName(IClientState clientState)
        {
            if (clientState != null && clientState.LocalPlayer != null) // A VERIFIER
                this.PlayerName = ((IGameObject)clientState.LocalPlayer).Name.TextValue;
            return this.PlayerName;
        }

        public string GetPlayerName() => this.PlayerName;

        private void UpdateCurrentHp(IClientState clientState)
        {
            if (clientState != null && clientState.LocalPlayer != null) // A VERIFIER
            {
                this._CurrentHp = (float)((ICharacter)clientState.LocalPlayer).CurrentHp;
                this._MaxHp = (float)((ICharacter)clientState.LocalPlayer).MaxHp;
            }
            if ((double)this._CurrentHp != (double)this._prevCurrentHp)
            {
                EventHandler currentHpChanged = this.Event_CurrentHpChanged;
                if (currentHpChanged != null)
                    currentHpChanged((object)this, EventArgs.Empty);
            }
            if ((double)this._MaxHp != (double)this._prevMaxHp)
            {
                EventHandler eventMaxHpChanged = this.Event_MaxHpChanged;
                if (eventMaxHpChanged != null)
                    eventMaxHpChanged((object)this, EventArgs.Empty);
            }
            this._prevCurrentHp = this._CurrentHp;
            this._prevMaxHp = this._MaxHp;
        }

        public float GetCurrentHP() => this._CurrentHp;

        public float GetMaxHP() => this._MaxHp;
    }
}
