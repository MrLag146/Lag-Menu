using HarmonyLib;
using LagMenu.Mods;
using LagMenu.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), "PressButton")]
    public class AntiReportCrashPatch
    {
        public static bool Enabled = false;

        static bool Prefix(GorillaPlayerScoreboardLine __instance, bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
        {
            try
            {
                if (!Enabled) return true;
                if (!isOn) return true;
                if (buttonType != GorillaPlayerLineButton.ButtonType.Report) return true;
                if (__instance == null || __instance.linePlayer == null) return true;

                VRRig rig = RigManager.GetVRRigFromPlayer(__instance.linePlayer);
                if (rig == null) return true;

                Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                if (target == null) return true;

                VStump.VStumpCrash(target.ActorNumber);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
