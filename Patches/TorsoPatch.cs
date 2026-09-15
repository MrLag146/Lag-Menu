using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), "PostTick", MethodType.Normal)]
    public class TorsoPatch
    {
        public static event Action VRRigLateUpdate;

        public static void Postfix(VRRig __instance)
        {
            if (__instance.isLocal)
            {
                VRRigLateUpdate?.Invoke();
            }
        }
    }
}
