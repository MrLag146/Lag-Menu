using HarmonyLib;
using UnityEngine;
using static LagMenu.Mods.RIg;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.PostTick))]
    public static class PCRigPatch
    {
        public const float SkellonRotationSpeed = 40f;

        private static float yRotation;

        private static void Postfix(VRRig __instance)
        {
            if (!__instance.isLocal || !PCRigEnabled) return;

            __instance.head.rigTarget.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            yRotation = Mathf.LerpAngle(
                yRotation,
                GorillaTagger.Instance.mainCamera.transform.eulerAngles.y,
                Time.deltaTime * SkellonRotationSpeed);

            __instance.transform.rotation = Quaternion.Euler(
                __instance.transform.eulerAngles.x,
                yRotation,
                __instance.transform.eulerAngles.z);

            __instance.head.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
            __instance.leftHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
            __instance.rightHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);

            __instance.head.rigTarget.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }
    }
}
