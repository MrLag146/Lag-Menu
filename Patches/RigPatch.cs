using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace LagMenu.Patches
{
    internal class RigPatch
    {
        [HarmonyPatch(typeof(VRRig), nameof(VRRig.OnDisable))]
        public class OnDisable
        {
            public static bool Prefix(VRRig __instance) =>
                !__instance.isLocal;
        }

        [HarmonyPatch(typeof(VRRig), nameof(VRRig.Awake))]
        public class Awake
        {
            public static bool Prefix(VRRig __instance) =>
                __instance.gameObject.name != "Local Gorilla Player(Clone)";
        }

        [HarmonyPatch(typeof(VRRig), nameof(VRRig.PostTick))]
        public class PostTick
        {
            public static bool Prefix(VRRig __instance) =>
                !__instance.isLocal || __instance.enabled;
        }
    }
}
