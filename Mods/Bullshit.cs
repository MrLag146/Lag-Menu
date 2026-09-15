using GorillaLocomotion;
using HarmonyLib;
using LagMenu.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LagMenu.Mods.OverPowered;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    [HarmonyPatch(typeof(VRRig), "PostTick")]
    public static class Bullshit4
    {
        public static bool upsiderig;

        public static bool fullupsiderig;

        public static bool backrig;

        public static bool fullbackrig;

        public static bool forwardrig;

        public static bool crawl;

        public static bool spin;

        public static bool spaz;

        public static bool track;

        public static bool fullspin;

        public static bool upspin;

        public static bool forrig;

        public static bool crawlrig;

        public static bool spazrig;

        public static string Field(int f)
        {
            return collider.GetField(f);
        }

        public static GTPlayer Loco()
        {
            return GTPlayer.Instance;
        }
        public static float orbit;

        public static float angle;

        public static Traverse Create(object o)
        {
            return Traverse.Create(o);
        }


        public static void Postfix(VRRig __instance)
        {
            if (!__instance.isLocal || (!upsiderig && !spazrig && !crawlrig && !forrig && !fullupsiderig && !backrig && !fullbackrig && !forwardrig && !crawl && !spin && !spaz && !track && !fullspin && !upspin))
            {
                return;
            }
            VRRig vRRig = RigManager.GetOwnVRRig();

            Quaternion rotation = Quaternion.identity;
            Vector3 eulerAngles = OverPowered.GIn().headCollider.transform.rotation.eulerAngles;
            if (upsiderig || fullupsiderig)
            {
                rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + 180f);
            }
            if (backrig || fullbackrig)
            {
                rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 180f, eulerAngles.z);
            }
            if (forwardrig || forrig)
            {
                rotation = Quaternion.Euler(eulerAngles.x + -90f, eulerAngles.y, eulerAngles.z);
            }
            if (crawl || crawlrig)
            {
                rotation = Quaternion.Euler(eulerAngles.x + 90f, eulerAngles.y, eulerAngles.z);
            }
            if (spaz || spazrig)
            {
                rotation = Random.rotationUniform;
            }
            if (track)
            {
                rotation = Camera.main.transform.rotation;
            }
            if (spin || fullspin || upspin)
            {
                if (Time.time > orbit)
                {
                    orbit = Time.time;
                   angle += 280f * Time.deltaTime;
                }
                rotation = Quaternion.Euler(0f, angle, upspin ? (eulerAngles.z + 180f) : 0f);
            }
            vRRig.transform.rotation = rotation;
            float value = Create(vRRig).Field(Field(66)).GetValue<float>();
            Transform playerOffsetTransform = vRRig.playerOffsetTransform;
            if (!fullspin && !forrig && !crawlrig && !spazrig)
            {
                vRRig.head.MapMine(value, playerOffsetTransform);
            }
            vRRig.leftHand.MapMine(value, playerOffsetTransform);
            vRRig.rightHand.MapMine(value, playerOffsetTransform);
        }
    }

}
