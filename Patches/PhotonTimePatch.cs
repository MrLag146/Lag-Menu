using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagMenu.Patches
{

    [HarmonyPatch(typeof(PhotonNetwork), "get_ServerTimestamp")]
    public class PhotonTimePatch
    {
        public static bool enabled = false;
        public static int distTime = 0;

        public static int lastTimestamp = -1;
        public static int currentTimestamp = -1;

        public static void Postfix(ref int __result)
        {
            if (enabled)
                __result += distTime;

            lastTimestamp = currentTimestamp;
            currentTimestamp = __result;
        }
    }
}
