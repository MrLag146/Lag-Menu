using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using HarmonyLib;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(GTPlayer), "TakeMyHand_ProcessMovement", MethodType.Normal)]
    public class HandLinkPatch
    {
        public static bool enabled;

        public static bool Prefix(GTPlayer __instance) =>
            !enabled;
    }
}
