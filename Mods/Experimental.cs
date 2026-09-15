using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using LagMenu.Notifications;
using LagMenu.Utilities;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace LagMenu.Mods
{
    internal class Experimental
    {
        public static void CopyObjectInfoGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (GunLib.raycastHit.collider == null)
                    return;

                GameObject obj = GunLib.raycastHit.collider.gameObject;

                string objectName = obj.name;
                string objectPath = GetGameObjectPath(obj.transform);

                string data =
                    $"Name: {objectName}\n" +
                    $"Path: {objectPath}";

                GUIUtility.systemCopyBuffer = data;

                Debug.Log(data);
            });
        }

        private static string GetGameObjectPath(Transform current)
        {
            StringBuilder path = new StringBuilder(current.name);

            while (current.parent != null)
            {
                current = current.parent;
                path.Insert(0, current.name + "/");
            }

            return path.ToString();
        }






        public static void ModCheckerGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig targetRig = GunLib.GetTargetRig();
                if (targetRig == null || targetRig.isLocal) return;

                Photon.Realtime.Player targetPhotonPlayer = GunLib.GetPlayerFromVRRig(targetRig);
                if (targetPhotonPlayer == null) return;

                ExitGames.Client.Photon.Hashtable customProperties = targetPhotonPlayer.CustomProperties;
                string displayText = $"{targetRig.playerText1}\n";

                if (customProperties.Count == 0)
                {
                    displayText += "No custom properties";
                }
                else
                {
                    foreach (DictionaryEntry entry in customProperties)
                    {
                        string key = entry.Key.ToString();
                        string value = entry.Value?.ToString() ?? "null";
                        if (value.Length > 30)
                            value = value.Substring(0, 27) + "...";
                        displayText += $"{key}: {value}\n";
                    }
                }

                NotifiLib.SendNotification(displayText);
            }, rightHand: true);
        }



        public static void CopySelfID()
        {
            string id = PhotonNetwork.LocalPlayer.UserId;
            NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + id);
            GUIUtility.systemCopyBuffer = id;
        }


        public static void CustomPropExpander()
        {
            Hashtable properties = new Hashtable()
    {
        { "no way a cheater", "Lagmenu By Mr Lag" },

        { "Seralyth", "Enabled" },
        { "Sigma Menu", "Enabled" },
        { "Hamburbur", "Enabled" },
        { "Rexon Paid", "Enabled" },
        { "Rexon Free", "Enabled" },
        { "Untitled", "Enabled" },
        { "ii stupid menu", "Enabled" },
        { "Hi", "Enabled" },
        { "Monke Mod", "Enabled" },
        { "Ghost Client", "Enabled" },
        { "Project Void", "Enabled" },
        { "Nebula", "Enabled" },
        { "Carbon", "Enabled" },
        { "Echo", "Enabled" },
        { "Velocity", "Enabled" },
        { "Quantum", "Enabled" },
        { "Lunar", "Enabled" },
        { "Solaris", "Enabled" },
        { "Crimson", "Enabled" },
        { "Hydra", "Enabled" },
        { "Tempest", "Enabled" },
        { "Aether", "Enabled" },
        { "StupidTemplate", "Enabled" },
        { "MalachiTemp", "Enabled" },
        { "VoidWare", "Enabled" },
        { "MonkeyWare", "Enabled" },
        { "Lag Injector", "Enabled" },
        { "Stealth", "Enabled" },
        { "Invisible", "Enabled" },
        { "banana", "Enabled" },
        { "gorilla", "Enabled" },
        { "shiba", "Enabled" },
        { "ron", "Enabled" },
        { "apollo", "Enabled" },
        { "dream", "Enabled" },
        { "chaos", "Enabled" },
        { "disconnect", "Enabled" },
        { "Crash V2", "Enabled" },
        { "Crash V3", "Enabled" },
        { "OP MOD", "Enabled" },
        { "Untitled Menu", "Enabled" },
        { "Skibidi Client", "Enabled" },
        { "SigmaWare", "Enabled" },
        { "toilet", "Enabled" },
        { "LOL", "Enabled" },
        { "hehe", "Enabled" },
        { "REAL", "Enabled" },
        { "SUS", "Enabled" },
        { "what", "Enabled" },
        { "aaaaaaaa", "Enabled" },
        { "orbit", "Enabled" },
        { "zenith", "Enabled" },
        { "eclipse", "Enabled" },
        { "vortex", "Enabled" },
        { "vertex", "Enabled" },
        { "null", "Enabled" },
        { "nullptr", "Enabled" },
        { "fatal", "Enabled" },
        { "internal", "Enabled" },
        { "external", "Enabled" },
        { "loader", "Enabled" },
        { "dll", "Enabled" },
        { "injector", "Enabled" },
        { "xenon", "Enabled" },
        { "kraken", "Enabled" },
        { "phantom", "Enabled" },
        { "wraith", "Enabled" },
        { "onyx", "Enabled" },
        { "ruby", "Enabled" },
        { "emerald", "Enabled" },
        { "diamond", "Enabled" },
        { "plasma", "Enabled" },
        { "fusion", "Enabled" },
        { "nexus", "Enabled" },
        { "matrix", "Enabled" },
        { "binary", "Enabled" },
        { "hex", "Enabled" },
        { "byte", "Enabled" },
        { "packet", "Enabled" },
        { "lag", "Enabled" },
        { "freeze", "Enabled" },
        { "disconnect all", "Enabled" },
        { "master", "Enabled" },
        { "admin", "Enabled" },
        { "modder", "Enabled" },
        { "owner", "Enabled" },
        { "host", "Enabled" },
        { "client", "Enabled" },
        { "network", "Enabled" },
        { "photon", "Enabled" },
        { "rpc spam", "Enabled" },
        { "antiban", "Enabled" },
        { "antireport", "Enabled" },
        { "overpowered", "Enabled" },
        { "crazy", "Enabled" },
        { "weird", "Enabled" },
        { "funny", "Enabled" },
        { "goofy", "Enabled" },
        { "silly", "Enabled" },
        { "monke", "Enabled" },
        { "gtag", "Enabled" },
        { "gorilla tag", "Enabled" },
        { "banana menu", "Enabled" },
        { "appleware", "Enabled" },
        { "water", "Enabled" },
        { "fire", "Enabled" },
        { "earth", "Enabled" },
        { "air", "Enabled" },
        { "light", "Enabled" },
        { "darkness", "Enabled" },
        { "storm", "Enabled" },
        { "rain", "Enabled" },
        { "snow", "Enabled" },
        { "ice", "Enabled" },
        { "lava", "Enabled" },
        { "coco", "Enabled" },
        { "cookie", "Enabled" },
        { "milk", "Enabled" },
        { "bread", "Enabled" },
        { "burger", "Enabled" },
        { "cheese", "Enabled" },
        { "fries", "Enabled" },
        { "pizza", "Enabled" },
        { "hotdog", "Enabled" },
        { "nugget", "Enabled" },
        { "fish", "Enabled" },
        { "taco", "Enabled" },
        { "burrito", "Enabled" },
        { "alpha", "Enabled" },
        { "beta", "Enabled" },
        { "release", "Enabled" },
        { "preview", "Enabled" },
        { "nightly", "Enabled" },
        { "dev build", "Enabled" },
        { "test", "Enabled" },
        { "real", "Enabled" },
        { "fake", "Enabled" },
        { "broken", "Enabled" },
        { "working", "Enabled" },
        { "paid", "Enabled" },
        { "free", "Enabled" },
        { "premium", "Enabled" },
        { "ultimate", "Enabled" },
        { "godmode", "Enabled" },
        { "fly", "Enabled" },
        { "speed", "Enabled" },
        { "longarms", "Enabled" },
        { "platforms", "Enabled" },
        { "invis", "Enabled" },
        { "esp", "Enabled" },
        { "wallhack", "Enabled" },
        { "tracers", "Enabled" },
        { "aimbot", "Enabled" },
        { "triggerbot", "Enabled" },
        { "silent aim", "Enabled" },
        { "mod menu", "Enabled" },
        { "menu open", "Enabled" },
        { "who asked", "Enabled" },
        { "bro", "Enabled" },
        { "bro what", "Enabled" },
        { "wtf", "Enabled" },
        { "real sigma", "Enabled" },
        { "gigachad", "Enabled" },
        { "npc", "Enabled" },
        { "ohio", "Enabled" },
        { "brainrot", "Enabled" },
        { "skibidi", "Enabled" },
        { "among us", "Enabled" },
        { "susware", "Enabled" },
        { "cheat", "Enabled" },
        { "modded", "Enabled" },
        { "bypass", "Enabled" },
        { "spoofed", "Enabled" },
        { "randomprop1", "Enabled" },
        { "randomprop2", "Enabled" },
        { "randomprop3", "Enabled" },
        { "randomprop4", "Enabled" },
        { "randomprop5", "Enabled" },
        { "randomprop6", "Enabled" },
        { "randomprop7", "Enabled" },
        { "randomprop8", "Enabled" },
        { "randomprop9", "Enabled" },
        { "randomprop10", "Enabled" }
    };

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }


    }
}

