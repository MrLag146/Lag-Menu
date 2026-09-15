using System;
using System.Linq;
using System.Reflection;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;

namespace LagMenu.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EssentialPatchAttribute : Attribute { }

    public static class PatchManager
    {
        private static Harmony harmonyInstance;

        public static bool HasPatched { get; private set; }
        public static bool IsLockedDown { get; private set; }
        public static int FailedPatchCount { get; private set; }

        public static bool ApplyAllPatches()
        {
            if (HasPatched)
                return !IsLockedDown;

            harmonyInstance ??= new Harmony(PluginInfo.GUID);

            bool essentialPatchFailed = false;

            Type[] types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (Type type in types.Where(t => t.IsClass && t.GetCustomAttributes<HarmonyPatch>().Any()))
            {
                try
                {
                    harmonyInstance.CreateClassProcessor(type).Patch();
                }
                catch (Exception ex)
                {
                    FailedPatchCount++;
                    Debug.LogError($"[LagMenu] Failed to apply patch {type.FullName}: {ex}");

                    bool isEssential = type.GetCustomAttributes<EssentialPatchAttribute>().Any() ||
                                        (type.DeclaringType?.GetCustomAttributes<EssentialPatchAttribute>().Any() ?? false);

                    if (isEssential)
                    {
                        essentialPatchFailed = true;
                        TriggerLockdown(type.Name);
                    }
                }
            }

            HasPatched = true;
            return !essentialPatchFailed;
        }

        private static void TriggerLockdown(string patchName)
        {
            if (IsLockedDown)
                return;

            IsLockedDown = true;

            string message =
                "LagMenu was unable to apply a required patch and has locked itself down for safety.\n" +
                "Failed patch: " + patchName;

            Debug.LogError(message);

            if (GorillaComputer.instance != null)
                GorillaComputer.instance.GeneralFailureMessage(message.ToUpper());
        }

        public static void RemoveAllPatches()
        {
            if (harmonyInstance == null)
                return;

            harmonyInstance.UnpatchSelf();
            harmonyInstance = null;
            HasPatched = false;
        }
    }
}