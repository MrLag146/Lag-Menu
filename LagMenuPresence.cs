using Lagmenu.DiscordRPC;
using Photon.Pun;
using UnityEngine;

namespace LagMenu.Menu
{
    public static class LagMenuPresence
    {
        private static DiscordRpcClient _client;

        private const string ApplicationId = "1530117068966596628";
        private const float RefreshInterval = 8f;

        private static float _nextRefresh;

        public static void Initialize()
        {
            _client = new DiscordRpcClient(ApplicationId);
            _client.Initialize();

            UpdatePresence("Using LagMenu");
        }

        public static void Tick()
        {
            if (_client == null || !_client.IsInitialized)
                return;

            if (Time.unscaledTime < _nextRefresh)
                return;

            _nextRefresh = Time.unscaledTime + RefreshInterval;
            RefreshPresence();
        }

        private static void RefreshPresence()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
            {
                UpdatePresence("Using LagMenu");
                return;
            }

            string roomCode = PhotonNetwork.CurrentRoom.Name;
            string gameMode = LagMenu.Utilities.Useful.GetGameMode();
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

            string details = $"Room: {roomCode}";
            string state = maxPlayers > 0
                ? $"{gameMode} \u00b7 {playerCount}/{maxPlayers} players"
                : $"{gameMode} \u00b7 {playerCount} players";

            UpdatePresence(details, state);
        }

        public static void UpdatePresence(string details, string state = null)
        {
            if (_client == null || !_client.IsInitialized) return;

            var presence = new RichPresence()
                .WithDetails(details)
                .WithState(state)
                .WithAssets(new Lagmenu.DiscordRPC.Assets
                {
                    LargeImageKey = "lagmenu_logo",
                    LargeImageText = "LagMenu"
                })
                .WithButtons(new Button
                {
                    Label = "Discord",
                    Url = "https://discord.gg/SpbHbYHPPD"
                });

            _client.SetPresence(presence);
        }

        public static void Shutdown()
        {
            _client?.Dispose();
        }
    }
}