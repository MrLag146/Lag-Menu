using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag;
using GorillaTagScripts;
using HarmonyLib;
using LagMenu.Menu;
using LagMenu.Mods;
using LagMenu.Notifications;
using LagMenu.Patches;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms;
using UnityEngine.XR;
using static InfectionLavaController;
using static LagMenu.Main;
using static LagMenu.Mods.Saftey;
using static LagMenu.Utilities.GameModeUtilities;
using static LagMenu.Utilities.GunLib;
using static LagMenu.Utilities.RigManager;
using static LagMenu.Utilities.VRRigExtensions;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
namespace LagMenu.Mods
{
    internal class Master
    {



        public static void BetaSetStatus(RoomSystem.StatusEffects state, RaiseEventOptions reo)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                object[] statusSendData = new object[1];
                statusSendData[0] = (int)state;
                object[] sendEventData = new object[3];
                sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
                sendEventData[1] = (byte)2;
                sendEventData[2] = statusSendData;
                PhotonNetwork.RaiseEvent((byte)Constants.Network.ROOM_SYSTEM, sendEventData, reo, SendOptions.SendUnreliable);
            }
        }






        private static float _slowDelay;

        public static void SlowGun(bool rightHand = true)
        {
            StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _slowDelay) return;
                    VRRig target = GetTargetRig();
                    if (target == null || target.isLocal) return;

                    NetPlayer player = RigManager.GetPlayerFromVRRig(target);
                    BetaSetStatus(RoomSystem.StatusEffects.TaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } });
                    RPCProtection();
                    _slowDelay = Time.time + 1f;
                },
                rightHand: rightHand
            );
        }







        public static void SlowSelf()
        {
            NetPlayer player = PhotonNetwork.LocalPlayer;
            BetaSetStatus(RoomSystem.StatusEffects.TaggedTime, new RaiseEventOptions { TargetActors = new[] { player.ActorNumber } });
            RPCProtection();
        }

        private static float slowDelay;

        public static void SlowAll()
        {
            if (Time.time > slowDelay)
            {
                BetaSetStatus(RoomSystem.StatusEffects.TaggedTime, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                slowDelay = Time.time + 1f;
            }
        }


        private static float vibrateDelay;
        public static void VibrateAll()
        {
            if (Time.time > vibrateDelay)
            {
                BetaSetStatus(RoomSystem.StatusEffects.JoinedTaggedTime, new RaiseEventOptions { Receivers = ReceiverGroup.Others });
                RPCProtection();
                vibrateDelay = Time.time + 0.5f;
            }
        }

        public static void VibrateSelf()
        {
            NetPlayer owner = PhotonNetwork.LocalPlayer;
            BetaSetStatus(RoomSystem.StatusEffects.JoinedTaggedTime, new RaiseEventOptions { TargetActors = new[] { owner.ActorNumber } });
            RPCProtection();
        }



        private static float _vibrateDelay;

        public static void VibrateGun(bool rightHand = true)
        {
            StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _vibrateDelay) return;
                    VRRig target = GetTargetRig();
                    if (target == null || target.isLocal) return;

                    NetPlayer owner = RigManager.GetPlayerFromVRRig(target);
                    BetaSetStatus(RoomSystem.StatusEffects.JoinedTaggedTime, new RaiseEventOptions { TargetActors = new[] { owner.ActorNumber } });
                    RPCProtection();
                    _vibrateDelay = Time.time + 0.5f;
                },
                rightHand: rightHand
            );
        }








        public static int paintbrawlKillIndex = 0;
        public static readonly Dictionary<int, float> paintbrawlKillDelays = new Dictionary<int, float>();

        public static void PaintbrawlKillPlayer(NetPlayer target)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {

                if (paintbrawlKillDelays.TryGetValue(target.ActorNumber, out float lastTime))
                    if (Time.time < lastTime)
                        return;

                paintbrawlKillDelays[target.ActorNumber] = Time.time + 3.1f;

                VRRig rig = GetVRRigFromPlayer(target.GetPlayerRef());
                if (rig == null) return;

                GorillaGameManager.instance.photonView.RPC(
    "RPC_ReportSlingshotHit",
    target.GetPlayerRef(),
    rig.transform.position,
    paintbrawlKillIndex
);


                RPCProtection();
                paintbrawlKillIndex++;
            }
            else
            {
                GorillaPaintbrawlManager brawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                if (brawlManager != null)
                    brawlManager.playerLives[target.ActorNumber] = 0;
            }
        }










        public static void PaintbrawlKillGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                PaintbrawlKillPlayer(target.Creator);

            }, rightHand: true);
        }

        public static void PaintbrawlKillAll()
        {
            if (!IsGameMode(GameModeType.Paintbrawl)) return;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal) return;
                PaintbrawlKillPlayer(rig.Creator);
            }
        }












        private static float playerColorDelay;
        public static void SetColorSelf(int color) =>
            SetPlayerColors(new Dictionary<int, int> { { NetworkSystem.Instance.LocalPlayer.ActorNumber, color } });


        public static void SetPlayerColors(Dictionary<int, int> colors)
        {
            var filteredPlayers = NetworkSystem.Instance.AllNetPlayers
                .Where(p => colors.ContainsKey(p.ActorNumber));
            MonkeBallGame.Instance.photonView.RPC(
                "RequestSetGameStateRPC",
                RpcTarget.All,
                (int)MonkeBallGame.GameState.Playing,
                PhotonNetwork.Time + (MonkeBallGame.Instance.gameDuration - 1f),
                filteredPlayers.Select(p => p.ActorNumber).ToArray(),
                filteredPlayers.Select(p => colors[p.ActorNumber]).ToArray(),
                new int[MonkeBallGame.Instance.team.Count],
                MonkeBallGame.Instance.startingBalls
                    .Select(ball => BitPackUtils.PackHandPosRotForNetwork(ball.transform.position, ball.transform.rotation))
                    .ToArray(),
                MonkeBallGame.Instance.startingBalls
                    .Select(ball => BitPackUtils.PackWorldPosForNetwork(ball.gameBall.GetVelocity()))
                    .ToArray()
            );
        }




        public static void SetGuardianTarget(NetPlayer target)
        {
            if (!NetworkSystem.Instance.IsMasterClient)
            {
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
                return;
            }

            GorillaGuardianManager guardianManager = (GorillaGuardianManager)GorillaGameManager.instance;
            if (guardianManager.IsPlayerGuardian(target))
                return;

            foreach (GorillaGuardianZoneManager zoneManager in GorillaGuardianZoneManager.zoneManagers)
            {
                if (zoneManager.IsZoneValid() && zoneManager.CurrentGuardian == null)
                {
                    zoneManager.SetGuardian(target);
                    return;
                }
            }
        }



        public static void GuardianSelf() =>
         SetGuardianTarget(PhotonNetwork.LocalPlayer);


        private static float guardianDelay;
        public static void GuardianGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < guardianDelay) return;

                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                SetGuardianTarget(GunLib.GetPlayerFromVRRig(target));
                guardianDelay = Time.time + 0.1f;

            }, true);
        }






        public static void GuardianAll()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                int i = 0;
                foreach (var gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers.Where(gorillaGuardianZoneManager => gorillaGuardianZoneManager.enabled && gorillaGuardianZoneManager.IsZoneValid()))
                {
                    gorillaGuardianZoneManager.SetGuardian(PhotonNetwork.PlayerList[i]);
                    i++;
                }
            }

        }











        public static void UntagSelf()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                RemoveInfected(PhotonNetwork.LocalPlayer);
                GTPlayer.Instance.disableMovement = false;

            }
            else
                NotifiLib.SendNotification("Your not master client dipshit ");


        }




        public static void UntagAll()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                foreach (Player v in PhotonNetwork.PlayerList)
                    RemoveInfected(v);
            }
        }



        public static void UntagGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < guardianDelay) return;

                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                NetPlayer player = target.Creator;
                if (player == null) return;

                RemoveInfected(player);


            }, true);
        }




        public static float MasterClientTimedAction;

        public static bool MaterialSwap;



        public static void MatAll()
        {
            if (Time.time > MasterClientTimedAction)
            {
                MasterClientTimedAction = Time.time + 0.05f;
                if (MaterialSwap)
                {
                    foreach (var player in VRRigCache.ActiveRigs)
                    {
                        Advantage.TagPlayer(player.Creator);
                    }
                    MaterialSwap = false;
                }
                else
                {
                    foreach (var player in VRRigCache.ActiveRigs)
                    {
                        UntagPlayer(player.Creator);
                    }
                    MaterialSwap = true;
                }
            }
        }




        private static float _matGunDelay;

        public static void MatGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _matGunDelay) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _matGunDelay = Time.time + 0.05f;
                    if (MaterialSwap)
                    {
                        Advantage.TagPlayer(rig.Creator);
                        MaterialSwap = true;
                    }
                    else
                    {
                        UntagPlayer(rig.Creator);
                        MaterialSwap = false;
                    }
                },
                rightHand: true
            );
        }

        static float lavaSpazDelay;
        static bool lavaSpaz;
        public static void SpazLava()
        {
            if (Time.time > lavaSpazDelay + 0.3f)
            {
                lavaSpazDelay = Time.time;
                lavaSpaz = !lavaSpaz;
                ChangeLavaState(lavaSpaz ? RisingLavaState.Full : RisingLavaState.Drained);
            }
        }




        public static void LavaSwimGun()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            GunLib.StartPointerSystem(() =>
            {
                Photon.Realtime.Player target = GunLib.GetTargetPlayer();
                if (target == null)
                    return;

                InfectionLavaController instance =
                    GetObject("Rewind_2024-02_Forest/VIMForestLava (prefab)/ILavaYou_PrefabV/ForestLavaController")
                   .GetComponent<InfectionLavaController>();

                instance.reliableState.stateStartTime += 500;

                RoomSystem.SendLavaSyncToPlayer(
                    (byte)instance.zone,
                    (byte)InfectionLavaController.RisingLavaState.Full,
                    instance.reliableState.stateStartTime,
                    instance.reliableState.activationProgress,
                    instance.lavaActivationVoteCount,
                    instance.lavaActivationVotePlayerIds,
                    target
                );
            });
        }

        public static void ChangeLavaState(InfectionLavaController.RisingLavaState state)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var instance = GetObject("Rewind_2024-02_Forest/VIMForestLava (prefab)/ILavaYou_PrefabV/ForestLavaController").GetComponent<InfectionLavaController>();
                instance.JumpToState(state);
            }
        }

        public static void UntagPlayer(NetPlayer plr)
        {
            GorillaTagManager tagManager = GorillaGameManager.instance as GorillaTagManager;
            if (tagManager.isCurrentlyTag && tagManager.currentIt == plr)
                tagManager.currentIt = null;
            else if (!tagManager.isCurrentlyTag && tagManager.currentInfected.Contains(plr))
                tagManager.currentInfected.Remove(plr);
        }












        public static readonly Dictionary<Type, object[]> typePool = new Dictionary<Type, object[]>();
        private static readonly Dictionary<Type, float> receiveTypeDelay = new Dictionary<Type, float>();

        public static T[] GetAllType<T>(float decayTime = 5f) where T : Object
        {
            Type type = typeof(T);

            float lastReceivedTime = receiveTypeDelay.GetValueOrDefault(type, -1f);

            if (Time.time > lastReceivedTime)
            {
                typePool.Remove(type);
                receiveTypeDelay[type] = Time.time + decayTime;
            }

            if (!typePool.ContainsKey(type))
                typePool.Add(type, Object.FindObjectsOfType<T>(true));

            return (T[])typePool[type];
        }

        public static Coroutine RopeCoroutine;
        public static IEnumerator RopeEnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        public static Vector3 ServerPos;
        public static Vector3 ServerLeftHandPos;
        public static Vector3 ServerRightHandPos;
        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(Random.Range(0f, range),
                        Random.Range(0f, range),
                        Random.Range(0f, range));

        private static Coroutine _critterCoroutine;
        private static float _critterGrabDelay;

        public static void CritterGun(bool rightHand = true)
        {
            StartPointerSystem(
                onTrigger: () =>
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        List<CrittersPawn> critters = CrittersManager.instance.crittersPawns
                            .Where(c => c != null).ToList();

                        CrittersPawn target = critters[Random.Range(0, critters.Count)];
                        target.transform.position = pointer.transform.position;
                        target.transform.rotation = RandomQuaternion();
                    }
                    else
                    {
                        CrittersGrabber localGrabber = GetAllType<CrittersGrabber>()
                            .Where(g => g.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && g.isLeft)
                            .FirstOrDefault();

                        Vector3 bodyPos = GorillaTagger.Instance.bodyCollider.transform.position;

                        List<CrittersPawn> critters = CrittersManager.instance.crittersPawns
                            .Where(c => c != null)
                            .Where(c => { float d = Vector3.Distance(c.transform.position, bodyPos); return d < 25f && d > 3f; })
                            .OrderByDescending(c => Vector3.Distance(c.transform.position, bodyPos))
                            .ToList();

                        if (critters.Count <= 0)
                            critters = CrittersManager.instance.crittersPawns
                                .Where(c => c != null && Vector3.Distance(c.transform.position, bodyPos) < 25f)
                                .OrderByDescending(c => Vector3.Distance(c.transform.position, bodyPos))
                                .ToList();

                        if (critters.Count <= 0)
                            critters = CrittersManager.instance.crittersPawns
                                .Where(c => c != null)
                                .OrderByDescending(c => Vector3.Distance(c.transform.position, bodyPos))
                                .ToList();

                        CrittersPawn critter = critters[Random.Range(0, critters.Count)];

                        if (Vector3.Distance(critter.transform.position, bodyPos) > 25f)
                        {
                            VRRig.LocalRig.enabled = false;
                            VRRig.LocalRig.transform.position = critter.transform.position - Vector3.one * 5f;

                            if (_critterCoroutine != null)
                                CoroutineManager.instance.StopCoroutine(_critterCoroutine);

                            _critterCoroutine = CoroutineManager.instance.StartCoroutine(RopeEnableRig());
                        }

                        if (Vector3.Distance(critter.transform.position, ServerPos) < 25f && Time.time > _critterGrabDelay)
                        {
                            _critterGrabDelay = Time.time + 0.05f;

                            critter.transform.position = pointer.transform.position + Vector3.up;
                            critter.transform.rotation = RandomQuaternion();

                            if (localGrabber != null)
                                CrittersManager.instance.SendRPC("RemoteCrittersActorGrabbedby",
                                    CrittersManager.instance.guard.currentOwner, critter.actorId, localGrabber.actorId,
                                    Quaternion.identity, Vector3.zero, false);

                            CrittersManager.instance.SendRPC("RemoteCritterActorReleased",
                                CrittersManager.instance.guard.currentOwner, critter.actorId, false,
                                critter.transform.rotation, critter.transform.position, Vector3.zero, Vector3.zero);
                        }
                    }
                },
                rightHand: rightHand
            );
        }








    }
}
