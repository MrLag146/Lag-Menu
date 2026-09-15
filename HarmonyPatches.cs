
using BepInEx;
using ExitGames.Client.Photon;
using LagMenu.Mods;
using LagMenu.Notifications;
using LagMenu.Patches;
using LagMenu.Utilities;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace LagMenu
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static GameObject menuObject = null;

        private void Awake()
        {
            AntiIAuth.AntiIAuthProtection.Initialize(this);
            Utilities.PatchManager.ApplyAllPatches();
            Soundboard.Init();

            Debug.Log("LagMenu Loaded");

            ResourceManager.Init();
            Menu.Console.LoadConsole();
            Menu.Console.PreloadAllAssets();
            UsefulManager.Enable();
            Menu.LagMenuPresence.Initialize();

            Hashtable properties = new Hashtable()
            {
                { "fuck off you modchecker cunt", "LagMenu" }
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }

        private void Update()
        {
            Tracker.Update();

            UsefulManager.Update();
            Menu.LagMenuPresence.Tick();
        }

        private void OnApplicationQuit()
        {
            Menu.LagMenuPresence.Shutdown();
        }

        public static readonly Color MainColour =
            new Color(
                0.1694782f,
                0.1504984f,
                0.3584906f
            );
    }
}
