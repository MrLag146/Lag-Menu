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
using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;
using static LagMenu.Main;
using static LagMenu.Mods.Saftey;
using static LagMenu.Utilities.GameModeUtilities;
using static LagMenu.Utilities.RigManager;
using static LagMenu.Utilities.GunLib;
using static LagMenu.Utilities.VRRigExtensions;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;


namespace LagMenu.Mods
{
    internal class Advantage
    {
        public static void AimbotOff()
        {
            aimbot = false;
        }






        private static float _flyGunCooldown;

        public static void GuardianFlyGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _flyGunCooldown) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _flyGunCooldown = Time.time + 0.2f;
                    rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                    rig.netView.SendRPC("DroppedByPlayer", rig.Creator, Vector3.up * 99f);
                },
                rightHand: true
            );
        }






        public static void TagPlayer(NetPlayer player)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                AddInfected(player);
                return;
            }



            if (instantTag)
            {
                InstantTagPlayer(player);

                return;
            }

            VRRig targetRig = GetVRRigFromPlayer(player);
            if (!targetRig.IsTagged())
            {
                VRRig.LocalRig.enabled = false;


                {
                    Vector3 position = targetRig.transform.position + RandomVector3();

                    VRRig.LocalRig.transform.position = position;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

                    VRRig.LocalRig.leftIndex.calcT = 0f;
                    VRRig.LocalRig.leftMiddle.calcT = 0f;
                    VRRig.LocalRig.leftThumb.calcT = 0f;

                    VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                    VRRig.LocalRig.rightIndex.calcT = 0f;
                    VRRig.LocalRig.rightMiddle.calcT = 0f;
                    VRRig.LocalRig.rightThumb.calcT = 0f;

                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                }

                if (ValidateTag(targetRig))
                    ReportTag(targetRig);
            }
            else;

        }




        public static void InstantTagPlayer(NetPlayer Target)
        {
            if (!VRRig.LocalRig.IsTagged() || Target.VRRig().IsTagged())
                return;

            Vector3 archiveRigPosition = VRRig.LocalRig.transform.position;
            VRRig.LocalRig.transform.position = GetVRRigFromPlayer(Target).transform.position;

            Seralizeing.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions { TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber } });
            GameMode.ReportTag(Target);

            VRRig.LocalRig.transform.position = archiveRigPosition;

            RPCProtection();
        }

        private static readonly Dictionary<int, float> PaintDelays =
            new Dictionary<int, float>();

        private static void PaintbrawlKillPlayer(VRRig target, bool rapid = false)
        {
            if (target == null || target.Creator == null) return;
            int actor = target.Creator.ActorNumber;

            if (PhotonNetwork.IsMasterClient)
            {
                GorillaPaintbrawlManager manager = PaintManager();
                if (manager != null) manager.playerLives[actor] = 0;
                return;
            }

            float next;
            if (PaintDelays.TryGetValue(actor, out next) && Time.time < next) return;
            if (GameMode.ActiveNetworkHandler == null) return;

            PaintDelays[actor] = Time.time + (rapid ? 0.2f : 0.15f);
            GameMode.ActiveNetworkHandler.SendRPC(
                "RPC_ReportSlingshotHit", false,
                RigManager.NetPlayerToPlayer(target.Creator), target.transform.position, _paintIndex++);
            Saftey.RPCProtection();
        }
        private static int _paintIndex;
        private static GorillaPaintbrawlManager PaintManager()
        {
            return GameMode.ActiveGameMode as GorillaPaintbrawlManager ??
                   GorillaGameManager.instance as GorillaPaintbrawlManager;
        }

        public static Quaternion RandomQuaternion(float range = 360f) =>
           Quaternion.Euler(Random.Range(0f, range),
                       Random.Range(0f, range),
                       Random.Range(0f, range));

        public static bool instantTag = true;
        public static void TagGun(bool rightHand = true)
        {
            if (instantTag)
            {
                InstantTagGun();
                return;
            }

            StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig target = GetTargetRig();
                    if (target == null || target.isLocal) return;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        AddInfected(GunLib.GetPlayerFromVRRig(target));
                    }
                    else
                    {
                        if (!VRRig.LocalRig.IsTagged()) return;
                        gunLocked = true;
                        lockTarget = target;
                    }
                },
                onGrip: () =>
                {
                    if (!gunLocked || lockTarget == null) return;

                    if (!lockTarget.IsTagged())
                    {
                        VRRig.LocalRig.enabled = false;


                        {
                            VRRig.LocalRig.transform.position = lockTarget.transform.position + RandomVector3();
                            VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();
                            VRRig.LocalRig.leftHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();
                            VRRig.LocalRig.rightHand.rigTarget.transform.position = lockTarget.transform.position + RandomVector3();
                            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();
                            VRRig.LocalRig.leftIndex.calcT = 0f;
                            VRRig.LocalRig.leftMiddle.calcT = 0f;
                            VRRig.LocalRig.leftThumb.calcT = 0f;
                            VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                            VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                            VRRig.LocalRig.leftThumb.LerpFinger(1f, false);
                            VRRig.LocalRig.rightIndex.calcT = 0f;
                            VRRig.LocalRig.rightMiddle.calcT = 0f;
                            VRRig.LocalRig.rightThumb.calcT = 0f;
                            VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                            VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                            VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                        }

                        if (ValidateTag(lockTarget))
                            ReportTag(lockTarget);
                    }
                    else
                    {
                        gunLocked = false;
                        VRRig.LocalRig.enabled = true;
                    }
                },
                rightHand: rightHand
            );


            if (!isGripping && gunLocked)
                VRRig.LocalRig.enabled = true;
        }

        public static bool ValidateTag(VRRig Rig) =>
         Vector3.Distance(ServerSyncPos, Rig.transform.position) < 6f;

        private static float reportTagDelay;
        public static void ReportTag(VRRig rig)
        {
            if (Time.time > reportTagDelay)
            {
                reportTagDelay = Time.time + 0.1f;
                GameMode.ReportTag(GunLib.GetPlayerFromVRRig(rig));
            }
        }
        public static Vector3 ServerSyncPos;
        private static float _tagGunDelay;

        public static void InstantTagGun(bool rightHand = true)
        {
            StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _tagGunDelay) return;
                    VRRig target = GetTargetRig();
                    if (target == null || target.isLocal) return;

                    try
                    {
                        _tagGunDelay = Time.time + 0.2f;
                        InstantTagPlayer(NetPlayerToPlayer(RigManager.GetPlayerFromVRRig(target)));
                    }
                    catch { }
                },
                rightHand: rightHand
            );
        }


        public static void InstantTagAll()
        {
            if (!ActualTagged(VRRig.LocalRig)) return;

            VRRig[] targets = VRRigCache.ActiveRigs
                .Where(rig => rig != null && !rig.isLocal && !ActualTagged(rig))
                .ToArray();

            if (targets.Length == 0) return;

            foreach (VRRig rig in targets)
                InstantTagPlayer(rig.Creator);
        }





        public static void TagSelf()
        {
            if (!PhotonNetwork.InRoom) return;

            if (NetworkSystem.Instance.IsMasterClient)
            {
                GameMode.ReportTag(NetworkSystem.Instance.LocalPlayer);
                return;
            }

            if (ActualTagged(VRRig.LocalRig))
            {
                VRRig.LocalRig.enabled = true;
                return;
            }

            VRRig rig = VRRigCache.ActiveRigs
                .Where(r => r != null && !r.isLocal && ActualTagged(r))
                .OrderBy(r => Vector3.Distance(
                    r.transform.position,
                    GorillaTagger.Instance.headCollider.transform.position)
                    + r.LatestVelocity().magnitude)
                .FirstOrDefault();

            if (rig == null) return;

            PhotonView pv = GorillaTagger.Instance.myVRRig.GetComponent<PhotonView>();
            if (pv == null) return;

            Vector3 startPos = VRRig.LocalRig.transform.position;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = rig.rightHandTransform.position;

            Seralizeing.SendSerialize(pv, new RaiseEventOptions
            {
                TargetActors = new[]
                {
            PhotonNetwork.MasterClient.ActorNumber,
            rig.Creator.GetPlayerRef().ActorNumber
        }
            });

            GameMode.ReportTag(rig.Creator);
            RPCProtection();

            VRRig.LocalRig.transform.position = startPos;
            VRRig.LocalRig.enabled = true;
        }


        public static void TagAll()
        {
            if (NetworkSystem.Instance.IsMasterClient)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig == null) continue;
                    GameMode.ReportTag(rig.Creator);
                }

                Debug.Log("[LagMenu] Everyone is tagged!");
                return;
            }

            if (!ControllerInputPoller.instance.rightControllerTriggerButton) return;

            bool localIsInfected = GorillaTagger.Instance.offlineVRRig.mainSkin.material.name.Contains("fected");
            if (!localIsInfected) return;

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal) continue;
                if (rig.mainSkin.material.name.Contains("fected")) continue;

                GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
                GorillaTagger.Instance.offlineVRRig.rightHandTransform.position = rig.transform.position;
                GorillaTagger.Instance.offlineVRRig.leftHandTransform.position = rig.transform.position;

                GameMode.ReportTag(rig.Creator);
            }
        }




        public static bool PlayerIsTagged(VRRig rig)
        {
            string text = rig.mainSkin.material.name.ToLower();
            return text.Contains("fected") || text.Contains("it") || text.Contains("stealth") || text.Contains("ice") || !rig.nameTagAnchor.activeSelf;
        }
        public static void NoTagOnJoin()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
            PlayerPrefs.SetString("didTutorial", "");
            PlayerPrefs.Save();
        }

        public static void TagOnJoin()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("didTutorial", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
            PlayerPrefs.SetString("didTutorial", "done");
            PlayerPrefs.Save();
        }



        public static void KillGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null || target.IsLocal) return;

                    if (_wasShooting) return;
                    _wasShooting = true;

                    PaintbrawlKillPlayer(rig);
                },
                onGrip: () => { _wasShooting = false; },
                rightHand: true
            );

            if (GunLib.isGripping) ;
        }

        public static void KillAll()
        {

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null) continue;
                PaintbrawlKillPlayer(rig);
            }
        }


        private static bool _wasShooting;
        public static void EnableTagFreeze()
        {
            GTPlayer.Instance.disableMovement = true;
        }


        public static void NoTagFreeze()
        {
            GTPlayer.Instance.disableMovement = false;
        }







        public static bool IsMasterClient
        {
            get
            {
                return PhotonNetwork.IsMasterClient;
            }
        }
        public static void BetaSetVelocityPlayer(NetPlayer victim, Vector3 velocity)
        {
            if (velocity.sqrMagnitude > 20f)
                velocity = Vector3.Normalize(velocity) * 20f;

            GorillaGuardianManager gman = (GorillaGuardianManager)GorillaGameManager.instance;
            if (gman.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                GetNetworkViewFromVRRig(GetVRRigFromPlayer(victim)).SendRPC("GrabbedByPlayer", victim, true, false, false);
                GetNetworkViewFromVRRig(GetVRRigFromPlayer(victim)).SendRPC("DroppedByPlayer", victim, velocity);
            }
            else
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
        }


        public static void TagSoundSpam()
        {
            if (IsMasterClient)
            {
                var sendData = new object[]
                {
                   0,
                   0.25f,
                   false,
                   PhotonNetwork.LocalPlayer.ActorNumber
                };
                SendEvent(3, sendData, true);
            }
        }
        public static List<HitTargetNetworkState> TargetNetworkStates = new List<HitTargetNetworkState>();
        public static void TargetHitSpam()
        {
            if (TargetNetworkStates.Count == 0)
            {
                foreach (HitTargetNetworkState hitTargetNetworkState in Resources.FindObjectsOfTypeAll<HitTargetNetworkState>())
                {
                    TargetNetworkStates.Add(hitTargetNetworkState);
                }
            }
            foreach (HitTargetNetworkState hitTargetNetworkState in TargetNetworkStates)
            {
                Traverse.Create(hitTargetNetworkState).Field("hitCooldownTime").SetValue(0);
                hitTargetNetworkState.TargetHit(Vector3.zero, Vector3.zero);
            }
        }


        public static void SendEvent(in byte code, in object evData, bool reliable)
        {
            var sendEventData = new object[] { };
            sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
            sendEventData[1] = code;
            sendEventData[2] = evData;

            PhotonNetwork.RaiseEvent(3, sendEventData, new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
            }, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
        }








        public static Vector3 RandomVector3(float range = 1f) =>
       new Vector3(Random.Range(-range, range),
                   Random.Range(-range, range),
                   Random.Range(-range, range));

        public static float alwaysGuardianDelay;



        static float antiGuardianCooldown;
        public static void GuardianProtector()
        {
            if (antiGuardianCooldown < Time.time)
            {
                antiGuardianCooldown = Time.time + 0.1f;
                var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
                if (!g.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    return;
                foreach (TappableGuardianIdol tgi in GetGuardianIdols())
                {
                    if (!tgi.isChangingPositions)
                    {
                        foreach (VRRig p in VRRigCache.ActiveRigs)
                        {
                            if (Vector3.Distance(p.transform.position, tgi.transform.position) < 3.5f)
                            {
                                p.netView.SendRPC("GrabbedByPlayer", p.Creator, new object[] { true, false, false });
                                p.netView.SendRPC("DroppedByPlayer", p.Creator, new object[] { new Vector3(Random.Range(-99, 99), Random.Range(-99, 99), Random.Range(-99, 99)) });
                            }
                        }
                    }
                }
            }
        }



        public static bool aimbot;
        public static bool manipulation;
        public static Vector3 aimOverride;
        public static Vector3 aimPosOverride;
        public static SlingshotProjectile Projectile;
        public static Vector3 LaunchPos;
        public static Vector3 LaunchVel;

        static VRRig oldPlayer;


        static Dictionary<VRRig, Vector3> lastPos = new Dictionary<VRRig, Vector3>();
        static Dictionary<VRRig, Vector3> lastVel = new Dictionary<VRRig, Vector3>();
        static Dictionary<VRRig, float> lastTime = new Dictionary<VRRig, float>();






        public static Vector3 Solve(Vector3 start, Vector3 targetPos, Vector3 targetVel, out Vector3 intercept, float minT = 0.12f, float maxT = 0.9f, int iterations = 8)
        {
            var g = Physics.gravity;
            var dist = Vector3.Distance(start, targetPos);
            var t = Mathf.Clamp(dist / 90f, minT, maxT);
            var v = Vector3.zero;
            intercept = targetPos;
            for (int i = 0; i < iterations; i++)
            {
                intercept = targetPos + targetVel * t;
                var to = intercept - start;
                v = (to - 0.5f * g * t * t) / Mathf.Max(0.0001f, t);
                var vm = Mathf.Max(0.01f, v.magnitude);
                var tNew = Mathf.Clamp(to.magnitude / vm, minT, maxT);
                t = Mathf.Lerp(t, tNew, 0.6f);
            }
            return v;
        }

        public static VRRig GetClosestPlayerToViewCenter()
        {
            VRRig myRig = GorillaTagger.Instance.offlineVRRig;
            if (myRig == null) myRig = GorillaTagger.Instance.offlineVRRig;
            Transform camT = myRig != null ? myRig.head?.rigTarget : null;
            if (camT == null)
            {
                var cam = Camera.main;
                if (cam == null) return null;
                camT = cam.transform;
            }

            if (VRRigCache.ActiveRigs == null) return null;

            float bestDot = -1f;
            VRRig current = null;

            Vector3 camPos = camT.position;
            Vector3 camForward = camT.forward;

            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig == null) continue;
                if (rig == myRig) continue;
                var target = rig.head?.rigTarget;
                if (target == null) continue;

                Vector3 toTarget = (target.position - camPos).normalized;
                float dot = Vector3.Dot(camForward, toTarget);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    current = rig;
                }
            }

            return current;
        }

        public static void Aimbot()
        {
            aimbot = true;
            var player = GetClosestPlayerToViewCenter();
            if (player == null || player.mainSkin == null) return;

            if (oldPlayer != null && oldPlayer != player && oldPlayer.mainSkin != null)
                oldPlayer.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");

            player.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
            player.mainSkin.material.color = UnityEngine.Color.red;
            oldPlayer = player;

            var tgtPos = player.syncPos;

            var now = Time.time;
            lastTime.TryGetValue(player, out float lt);
            lastPos.TryGetValue(player, out Vector3 lp);
            lastVel.TryGetValue(player, out Vector3 lv);

            if (lt <= 0f) { lt = now - Mathf.Max(Time.deltaTime, 0.001f); lp = tgtPos; lv = Vector3.zero; }

            var dt = Mathf.Max(0.005f, now - lt);
            var velRaw = (tgtPos - lp) / dt;
            var vel = Vector3.Lerp(lv, velRaw, 0.35f);

            lastTime[player] = now;
            lastPos[player] = tgtPos;
            lastVel[player] = vel;

            var start = GorillaTagger.Instance.offlineVRRig.projectileWeapon.transform.position;

            Vector3 intercept;

            aimOverride = Solve(start, tgtPos, vel, out intercept);

        }
        public static void GuardianFlingGun(bool rightHand = true)
        {
            StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig target = GetTargetRig();
                    if (target != null && !target.isLocal)
                    {
                        BetaSetVelocityPlayer(GunLib.GetPlayerFromVRRig(target), new Vector3(0f, 19.9f, 0f));
                        RPCProtection();
                    }
                },
                rightHand: rightHand
            );
        }

        public static float cooldown;
        public static TappableGuardianIdol[] idols;
        public static TappableGuardianIdol[] GetGuardianIdols()
        {
            if (Time.time > cooldown)
            {
                idols = null;
                cooldown = Time.time + 5f;
            }
            if (idols == null)
            {
                idols = UnityEngine.Object.FindObjectsOfType<TappableGuardianIdol>();
            }
            return idols;
        }

        public static void AlwaysGuardian()
        {
            var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
            if (g != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (!g.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
                        {
                            if (gorillaGuardianZoneManager.enabled)
                            {
                                if (gorillaGuardianZoneManager.CurrentGuardian != NetworkSystem.Instance.LocalPlayer)
                                {
                                    gorillaGuardianZoneManager.SetGuardian(NetworkSystem.Instance.LocalPlayer);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (TappableGuardianIdol tgi in GetGuardianIdols())
                    {
                        if (!tgi.isChangingPositions)
                        {
                            if (!g.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                            {
                                GorillaTagger.Instance.offlineVRRig.enabled = false;
                                GorillaTagger.Instance.offlineVRRig.transform.position = tgi.transform.position;

                                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = tgi.transform.position;
                                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = tgi.transform.position;

                                tgi.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, tgi.tappableId, Random.Range(0.2f, 0.4f));
                            }
                        }
                        else
                        {
                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                        }
                    }
                }
            }
        }




        private static float _guardianFlingCooldown;

        public static void GuardianFlingGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _guardianFlingCooldown) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _guardianFlingCooldown = Time.time + 0.1f;
                    rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                    rig.netView.SendRPC("DroppedByPlayer", rig.Creator, new Vector3(
                        Random.Range(-20, 20),
                        Random.Range(-20, 20),
                        Random.Range(-20, 20)
                    ));
                },
                rightHand: true
            );
        }




        private static float _grabCooldown;

        public static void BringGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _grabCooldown) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _grabCooldown = Time.time + 0.2f;
                    var vel = (GorillaTagger.Instance.offlineVRRig.transform.position - rig.transform.position).normalized * 20f;
                    rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                    rig.netView.SendRPC("DroppedByPlayer", rig.Creator, new object[] { vel });
                },
                rightHand: true
            );
        }




        private static float _flingCooldown;

        public static void GuardianFlingAll()
        {
            var g = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
            if (g == null || !g.IsPlayerGuardian(PhotonNetwork.LocalPlayer)) return;
            if (Time.time < _flingCooldown) return;

            _flingCooldown = Time.time + 0.1f;
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig.isOfflineVRRig) continue;
                rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                rig.netView.SendRPC("DroppedByPlayer", rig.Creator, new object[] { new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20)) });
            }
        }

        public static void OrbitAll()
        {
            if (Time.time < _flingCooldown) return;

            _flingCooldown = Time.time + 0.2f;
            foreach (var target in VRRigCache.ActiveRigs)
            {
                var pos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30f), 1f, Mathf.Sin((float)Time.frameCount / 30f));
                var direction = pos - target.transform.position;
                direction.Normalize();
                target.netView.SendRPC("GrabbedByPlayer", target.Creator, new object[] { true, false, false });
                target.netView.SendRPC("DroppedByPlayer", target.Creator, direction * 20f);
            }
        }



        private static float _freezeGunCooldown;

        public static void FreezeGunFling()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _freezeGunCooldown) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _freezeGunCooldown = Time.time + 0.2f;
                    rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                    rig.netView.SendRPC("DroppedByPlayer", rig.Creator, new object[] { Vector3.down });
                },
                rightHand: true
            );
        }


        private static float _orbitCooldown;

        public static void OrbitGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _orbitCooldown) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    _orbitCooldown = Time.time + 0.2f;
                    var pos = GorillaTagger.Instance.headCollider.transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30f), 1f, Mathf.Sin((float)Time.frameCount / 30f));
                    var direction = (pos - rig.transform.position).normalized;
                    rig.netView.SendRPC("GrabbedByPlayer", rig.Creator, new object[] { true, false, false });
                    rig.netView.SendRPC("DroppedByPlayer", rig.Creator, direction * 20f);
                },
                rightHand: true
            );
        }






        public static void DebugSlingshotAimbot()
        {
            if (VRRig.LocalRig.GetSlingshot() == null)
                return;

            if (VRRig.LocalRig.GetSlingshot().InLeftHand() ? ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f : ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f)
                return;

            List<NetPlayer> infected = InfectedList();
            List<VRRig> rigs = VRRigCache.ActiveRigs
                .Where(rig => !rig.isLocal)
                .Where(rig => !infected.Contains(GunLib.GetPlayerFromVRRig(rig)))
                .ToList();

            Transform head = GorillaTagger.Instance.headCollider.transform;
            VRRig targetRig = rigs
                .Where(rig => rig != null)
                .Select(rig => new
                {
                    Rig = rig,
                    ToRig = (rig.transform.position - head.position).normalized,
                    Distance = Vector3.Distance(head.position, rig.transform.position)
                })
                .OrderBy(x => Vector3.Angle(head.forward, x.ToRig))
                .ThenBy(x => x.Distance)
                .Select(x => x.Rig)
                .FirstOrDefault();

            if (targetRig == null)
                return;


        }







    }
}



