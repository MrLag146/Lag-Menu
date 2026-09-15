using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using LagMenu;
using LagMenu.Notifications;
using LagMenu.Patches;
using LagMenu.Utilities;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using static GorillaNetworking.CosmeticsController;
using static LagMenu.Mods.Saftey;
using static LagMenu.Patches.GamemodePatch;
using static LagMenu.Utilities.GunLib;
using static LagMenu.Utilities.RigManager;
using static LagMenu.Utilities.VRRigExtensions;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    internal class Detected
    {

        public static string[] GamemodesStrings =
        {
            "Casual",
            "Infection",
            "HuntDown",
            "Paintbrawl",
            "Ambush",
            "Ghost",
            "Guardian",
            "FreezeTag",
            "Custom",
            "PropHunt",
            "SuperInfect",
            "SuperCasual",
        };

        public static void ChangeMode(string mode)
        {
            if (GamemodesStrings.Contains(mode))
            {
                InstantModeChange(mode, 0, false);
            }
        }

        public static void GhostGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                Photon.Realtime.Player player = RigManager.GetPlayerFromVRRig(target);
                if (player == null) return;

                GhostPlayer(player);

            }, rightHand: true);
        }

        public static void CrashGun()
        {
            {
                StartPointerSystem(
                    onTrigger: () =>
                    {
                        VRRig target = GetTargetRig();
                        if (target != null && !target.isLocal)
                        {
                            PhotonNetwork.SetMasterClient(RigManager.GetPlayerFromVRRig(target));
                            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                        }
                    },
                    rightHand: true
                );
            }
        }


        public static void Destroy(object target, Hashtable hashtable = null, RaiseEventOptions raiseEventOptions = null, int viewID = -1)
        {
            switch (target)
            {
                case VRRig rig:
                    if (hashtable == null)
                    {
                        PhotonView view = GetPhotonViewFromVRRig(rig);
                        hashtable = new Hashtable { { 0, viewID == -1 ? view.ViewID : viewID } };
                    }
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new[] { rig.GetPlayer().ActorNumber } };
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(Photon.Pun.PunEvent.Destroy, hashtable, raiseEventOptions, SendOptions.SendReliable);
                    break;
                case Player player:
                    hashtable ??= new Hashtable { { 0, player.ActorNumber } };
                    raiseEventOptions ??= new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } };
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(Photon.Pun.PunEvent.DestroyPlayer, hashtable, raiseEventOptions, SendOptions.SendReliable);
                    break;
                case GameObject _:
                    break;
            }
        }
        public static Dictionary<VRRig, int> viewIdArchive = new Dictionary<VRRig, int>();


        public static void UnghostGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    int viewID = viewIdArchive[rig];
                    Destroy(rig, null, null, viewID);
                },
                rightHand: true
            );
        }

        public static void UnghostAll()
        {
            foreach (VRRig rig in VRRigExtensions.ActiveRigs)
            {
                if (viewIdArchive.TryGetValue(rig, out int viewID))
                    Destroy(rig, null, null, viewID);
            }
        }
        public static void GhostAll()
        {
            foreach (var p in PhotonNetwork.PlayerListOthers)
            {
                GhostPlayer(p);
            }
        }

        public static void ForceDestroyPlayer(Player player, int[] targets, bool all = false)
        {
            if (!all)
            {
                var hash = new ExitGames.Client.Photon.Hashtable();
                var target = GorillaGameManager.instance.FindPlayerVRRig(player);
                var netview = (NetworkView)Traverse.Create(target).Field("netView").GetValue();
                hash[0] = netview.ViewID;
                PhotonNetwork.NetworkingClient.OpRaiseEvent(204, hash, new RaiseEventOptions { TargetActors = targets }, SendOptions.SendReliable);
            }
            else
            {
                var hash = new ExitGames.Client.Photon.Hashtable();
                var target = GorillaGameManager.instance.FindPlayerVRRig(player);
                var netview = (NetworkView)Traverse.Create(target).Field("netView").GetValue();
                hash[0] = netview.ViewID;
                PhotonNetwork.NetworkingClient.OpRaiseEvent(204, hash, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }

        public static void GhostPlayer(Player p)
        {
            List<int> targets = new List<int>();
            foreach (var plr in PhotonNetwork.PlayerList)
            {
                if (p != plr && !targets.Contains(plr.ActorNumber))
                {
                    targets.Add(plr.ActorNumber);
                }
            }
            var r = GorillaGameManager.instance.FindPlayerVRRig(p);
            ForceDestroyPlayer(p, targets.ToArray());
        }

        public static int modecounter;
        public static float modecooldown;
        public static void ModeSpaz()
        {
            if (modecounter == 4)
            {
                modecounter = 0;
            }
            if (Time.time > modecooldown)
            {
                modecooldown = Time.time + 0.02f;
                modecounter++;
            }
            switch (modecounter)
            {
                case 0:
                    InstantModeChange("Infection", 1, true);
                    break;
                case 1:
                    InstantModeChange("HuntDown", 2, true);
                    break;
                case 2:
                    InstantModeChange("Paintbrawl", 1, true);
                    break;
                case 3:
                    InstantModeChange("FreezeTag", 7, true);
                    break;
            }
        }

        public static void LagAll()
        {
            foreach (var rig in VRRigExtensions.ActiveRigs.Where(rig => !rig.IsLocal()))
                Destroy(rig.GetPhotonPlayer());
        }

        public static void LagGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal)
                    return;

                Photon.Realtime.Player player = RigManager.GetPlayerFromVRRig(target);
                if (player != null)
                {
                    Destroy(player);
                }
            });
        }
    }
    }


