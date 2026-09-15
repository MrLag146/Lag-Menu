using System.Collections.Generic;

using HarmonyLib;
using LagMenu.Mods;
using LagMenu.Notifications;
using LagMenu.Utilities;
using Photon.Pun;
using UnityEngine;

namespace LagMenu.Patches
{
  
        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.SendReport))]
    public static class AntiCheat
    {
        private const           float                     PlayerReportLogCooldown = 1f;
        private static readonly Dictionary<string, float> LastLoggedReport = new Dictionary<string, float>();

        private static bool Prefix(string susReason, string susId, string susNick)
        {
            if (!Saftey.AntiCheatNotiEnabled)
                return true;

            if (LastLoggedReport.ContainsKey(susId) && LastLoggedReport[susId] > Time.time)
                return susId                                                   != PhotonNetwork.LocalPlayer.UserId;

            NotifiLib.SendNotification(
                "<color=red>Anti Cheat</color>",
                $"MonkeAgent reported {susNick} for: {susReason}");

            LastLoggedReport[susId] = Time.time + PlayerReportLogCooldown;

            return susId != PhotonNetwork.LocalPlayer.UserId;
        }
    }
}