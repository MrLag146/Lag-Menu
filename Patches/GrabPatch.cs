using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using HarmonyLib;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(GTPlayer), "TakeMyHand_ProcessMovement")]
    public class GrabPatch
    {
        public static bool GrabPatch_State_08;

        [HarmonyPrefix]
        public static bool Prefix(GTPlayer __instance)
        {
            return !GrabPatch_State_08;
        }


	public static bool GrabPatch_State_06;

        public static bool GrabPatch_State_07 = true;

        public static bool GrabPatch_State_03;

        public static float GrabPatch_Value_01 = 99999f;

        public static float GrabPatch_Value_02 = 99999f;

        public static int GrabPatch_Index_01 = 1;

        public static bool GrabPatch_State_05;

        public static bool GrabPatch_State_09;

        public static bool GrabPatch_State_11 = false;

        public static bool GrabPatch_State_12;

        public static bool GrabPatch_State_13;

        public static bool GrabPatch_State_01;

        public static bool GrabPatch_State_10;

        public static bool GrabPatch_State_14;

        public static bool GrabPatch_State_02;

        public static bool GrabPatch_State_04;
    }
}
