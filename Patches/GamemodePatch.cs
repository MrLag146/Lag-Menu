using GorillaGameModes;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using System;
using System.Reflection;
using UnityEngine;

namespace LagMenu.Patches
{
    internal class GamemodePatch
    {
        [HarmonyPatch(typeof(GorillaGameManager), "ValidGameMode")]
        public class GameModePatch
        {
            public static void Postfix(GorillaGameManager __instance, ref bool __result)
            {
                if (enabled)
                {
                    __result = true;
                }
            }
            public static bool enabled;
        }

        private static readonly FieldInfo activeGameModeField =
            typeof(GameMode).GetField("activeGameMode", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly FieldInfo activeNetworkHandlerField =
            typeof(GameMode).GetField("activeNetworkHandler", BindingFlags.Static | BindingFlags.NonPublic);

        private static GorillaGameManager GetActiveGameMode()
        {
            return (GorillaGameManager)activeGameModeField.GetValue(null);
        }

        private static void SetActiveGameMode(GorillaGameManager value)
        {
            activeGameModeField.SetValue(null, value);
        }

        private static Component GetActiveNetworkHandler()
        {
            return activeNetworkHandlerField.GetValue(null) as Component;
        }

        private static void ClearActiveNetworkHandler()
        {
            activeNetworkHandlerField.SetValue(null, null);
        }

        public static void InstantModeChange(string mode, int e, bool tagall)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                return;

            GameModePatch.enabled = true;

            var currentHandler = GetActiveNetworkHandler();
            if (currentHandler != null)
            {
                NetworkSystem.Instance.NetDestroy(currentHandler.gameObject);
            }

            SetActiveGameMode(null);
            ClearActiveNetworkHandler();

            GameMode.LoadGameMode(mode);

            var gm2 = GameObject.Find("GT Systems/GameModeSystem")?.GetComponent<GameMode>();

            if (tagall)
            {
                if (mode == "Paintbrawl" && GorillaGameManager.instance is GorillaPaintbrawlManager)
                {
                    var gam = gm2.GetComponentInChildren<GorillaPaintbrawlManager>();
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        gam.playerStatusArray[i] = GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated;
                    }
                }
                if (mode == "Infection" && GorillaGameManager.instance is GorillaTagManager)
                {
                    var gtm = gm2.GetComponentInChildren<GorillaTagManager>();
                    gtm.infectedModeThreshold = 1;
                    foreach (var p in PhotonNetwork.PlayerList)
                    {
                        if (!gtm.currentInfected.Contains(p))
                            gtm.currentInfected.Add(p);
                    }
                }
                if (mode == "HuntDown" && GorillaGameManager.instance is GorillaHuntManager)
                {
                    var ghm = gm2.GetComponentInChildren<GorillaHuntManager>();
                    foreach (var p in PhotonNetwork.PlayerList)
                    {
                        if (!ghm.currentHunted.Contains(p))
                            ghm.currentHunted.Add(p);
                    }
                }
                if (mode == "FreezeTag" && GorillaGameManager.instance is GorillaFreezeTagManager)
                {
                    var ftm = gm2.GetComponentInChildren<GorillaFreezeTagManager>();
                    ftm.infectedModeThreshold = 1;
                    foreach (var p in PhotonNetwork.PlayerList)
                    {
                        if (!ftm.currentInfected.Contains(p))
                            ftm.currentInfected.Add(p);
                    }
                }
                if (mode == "Guardian" && GorillaGameManager.instance is GorillaGuardianManager)
                {
                    int num = 0;
                    foreach (var zm in GorillaGuardianZoneManager.zoneManagers)
                    {
                        if (zm.enabled)
                        {
                            zm.SetGuardian(PhotonNetwork.PlayerList[num]);
                            num++;
                        }
                    }
                }
            }
        }
    }
}
