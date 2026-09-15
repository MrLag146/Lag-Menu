using HarmonyLib;
using Photon.Pun;
using System;
using System.Reflection;
using UnityEngine;

namespace LagMenu.Patches
{

    public class SerializePatch
    {
        public static event Action OnSerialize;

        public static Func<bool> OverrideSerialization;

        public static void Apply(Harmony harmony)
        {
            MethodInfo target = typeof(PhotonNetwork).GetMethod(
                "RunViewUpdate",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
            );

            if (target == null)
            {
                Debug.LogError("SerializePatch: Could not find PhotonNetwork.RunViewUpdate via reflection.");
                return;
            }

            HarmonyMethod prefix = new HarmonyMethod(typeof(SerializePatch).GetMethod(
                nameof(Prefix),
                BindingFlags.Public | BindingFlags.Static
            ));

            harmony.Patch(target, prefix: prefix);
        }

        public static bool Prefix()
        {
            if (!PhotonNetwork.InRoom)
                return true;

            try
            {
                OnSerialize?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Log($"Error in SerializePatch.OnSerialize: {e}");
            }

            if (OverrideSerialization == null)
                return true;

            try
            {
                return OverrideSerialization();
            }
            catch (Exception e)
            {
                Debug.Log($"Error in SerializePatch.OverrideSerialization: {e}");
                return false;
            }
        }
    }
}
