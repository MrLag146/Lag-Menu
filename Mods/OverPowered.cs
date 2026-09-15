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
using Liv.Lck.Tablet;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

using static GorillaNetworking.CosmeticsController;
using static LagMenu.Mods.Saftey;
using static LagMenu.Utilities.GunLib;
using static LagMenu.Utilities.RigManager;
using static LagMenu.Utilities.VRRigExtensions;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    internal class OverPowered
    {






























        public static void DeafenAll()
        {
            int num = 0;
            if (num < 2)
            {
                do
                {
                    DeafenPlayer((object)(ReceiverGroup)0);
                    num++;
                }
                while (num < 2);
            }
        }
        public static void DeafenPlayer(object player)
        {
            RaiseEventOptions val = new RaiseEventOptions();
            if (player is ReceiverGroup receivers)
            {
                val.Receivers = receivers;
            }
            else
            {
                if (!(player is int[] targetActors))
                {
                    return;
                }
                val.TargetActors = targetActors;
            }
            SendOptions val2 = default(SendOptions);
            val2.Reliability = true;
            val2.Channel = 0;
            SendOptions val3 = val2;
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>
        {
            { 1, 255 },
            { 2, 48000 },
            { 3, 2 },
            { 4, 20000 },
            { 5, 30000 },
            { 10, null },
            {
                11,
                (byte)0
            },
            {
                12,
                (byte)11
            }
        };
            object[] array = new object[3]
            {
            (byte)0,
            (byte)1,
            new object[1] { dictionary }
            };
            ((LoadBalancingClient)((VoiceConnection)PhotonVoiceNetwork.Instance).Client).OpRaiseEvent((byte)202, (object)array, val, val3);
        }


        public static void DeafenGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    DeafenPlayer(new int[] { target.ActorNumber });
                },
                rightHand: true
            );
        }









          public static void SendBarrelFling(int Mode)
        {
            switch (Mode)
            {
                case 0:
                    BarrelFlingMethod1();
                    break;
                case 1:
                    BarrelFlingMethod2();
                    break;
                case 2:
                    BarrelFlingMethod3();
                    break;
                case 3:
                    BarrelFlingMethod4();
                    break;
                case 4:
                    BarrelFlingMethod5();
                    break;
            }
        }





        public static VRRig targetFlinged;
        public static int barrelFlingMode = 1;



        public static void IncreaseBarrelFlingMode()
        {
            barrelFlingMode++;
            if (barrelFlingMode > 4)
            {
                barrelFlingMode = 1;
            }
        }




        public static void SpamDeploy(DeployableObject deployable)
        {
            var value = Traverse.Create(deployable).Field("_onDeploy").GetValue<UnityEvent>();
            for (var i = 0; i > Random.Range(500, 50000); i++)
                if (value != null)
                    value.Invoke();
        }

        public static DeployableObject GetBarrelDeployable()
        {
            if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
            {
                foreach (var obj in GorillaTagger.Instance.offlineVRRig.GetComponentsInChildren<DeployableObject>(true))
                {
                    if (obj.gameObject.name == "LMAPE.") return obj;
                }
            }
            return null;
        }

        public static void BarrelFlingMethod1()
        {
            if (targetFlinged == null || targetFlinged.isLocal) return;

            var deployable = GetBarrelDeployable();


            if (deployable == null) return;

            var child = Traverse.Create(deployable).Field("_child").GetValue<DeployedChild>();
            if (child == null) return;

            var rb = Traverse.Create(child).Field("_rigidbody").GetValue<Rigidbody>();
            if (rb == null) return;

            var data = PhotonUtils.FetchScratchArray(5);
            data[0] = (int)typeof(DeployableObject)
                .GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(deployable)
                .GetType().GetField("_signalID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(
                    typeof(DeployableObject).GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(deployable));
            data[1] = PhotonNetwork.ServerTimestamp;
            data[2] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.position +
                                                          new Vector3(0, -0.2f, 0));
            data[3] = 469893376;
            data[4] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.up * 500f);

            PhotonNetwork.RaiseEvent(177, data,
                TargetedWCO(targetFlinged.Creator.ActorNumber, EventCaching.AddToRoomCacheGlobal), STS());

            child.Deploy(deployable, targetFlinged.bodyTransform.position + new Vector3(0, -0.2f, 0),
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                targetFlinged.bodyTransform.up * 500f);
            deployable.DeployChild();
            SpamDeploy(deployable);

            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.detectCollisions = false;
            rb.velocity = targetFlinged.bodyTransform.up * 500f;

            var barrelObj = GetBarrelDeployable()?.gameObject;

            if (barrelObj != null)
                barrelObj.transform.rotation =
                    Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            for (var i = 0; i < 4; i++)
            {
                rb.AddForce(targetFlinged.bodyTransform.up * 500f, ForceMode.Force);
                rb.AddForce(targetFlinged.bodyTransform.up * 500f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.up * 500f, ForceMode.VelocityChange);
                rb.AddForce(targetFlinged.bodyTransform.up * 500f, ForceMode.Acceleration);
            }
            child.ReturnToParent(2f);
        }
        public static void BarrelFlingMethod2()
        {
            if (targetFlinged == null || targetFlinged.isLocal) return;

            var deployable = GetBarrelDeployable();

            if (deployable == null) return;

            var child = Traverse.Create(deployable).Field("_child").GetValue<DeployedChild>();
            if (child == null) return;

            var rb = Traverse.Create(child).Field("_rigidbody").GetValue<Rigidbody>();
            if (rb == null) return;

            var data = PhotonUtils.FetchScratchArray(5);
            data[0] = (int)typeof(DeployableObject)
                .GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(deployable)
                .GetType().GetField("_signalID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(
                    typeof(DeployableObject).GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(deployable));
            data[1] = PhotonNetwork.ServerTimestamp;
            data[2] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.position +
                                                          new Vector3(0, -0.2f, 0));
            data[3] = 469893376;
            data[4] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.up * 9999.99f);

            PhotonNetwork.RaiseEvent(177, data,
                TargetedWCO(targetFlinged.Creator.ActorNumber, EventCaching.AddToRoomCacheGlobal), STS());

            child.Deploy(deployable, targetFlinged.bodyTransform.position + new Vector3(0, -0.2f, 0),
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                targetFlinged.bodyTransform.up * 9999.99f);
            deployable.DeployChild();
            SpamDeploy(deployable);

            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.detectCollisions = false;
            rb.velocity = targetFlinged.bodyTransform.up * 9999.99f;

            var barrelObj = GetBarrelDeployable()?.gameObject;

            if (barrelObj != null)
                barrelObj.transform.rotation =
                    Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            for (var i = 0; i < 4; i++)
            {
                rb.AddForce(targetFlinged.bodyTransform.up * 9999.99f, ForceMode.Force);
                rb.AddForce(targetFlinged.bodyTransform.up * 9999.99f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.up * 9999.99f, ForceMode.VelocityChange);
                rb.AddForce(targetFlinged.bodyTransform.up * 9999.99f, ForceMode.Acceleration);
            }
            child.ReturnToParent(2f);
        }
        public static void BarrelFlingMethod3()
        {
            if (targetFlinged == null || targetFlinged.isLocal) return;

            var deployable = GetBarrelDeployable();

            if (deployable == null) return;

            var child = Traverse.Create(deployable).Field("_child").GetValue<DeployedChild>();
            if (child == null) return;

            var rb = Traverse.Create(child).Field("_rigidbody").GetValue<Rigidbody>();
            if (rb == null) return;

            var data = PhotonUtils.FetchScratchArray(5);
            data[0] = (int)typeof(DeployableObject)
                .GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(deployable)
                .GetType().GetField("_signalID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(
                    typeof(DeployableObject).GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(deployable));
            data[1] = PhotonNetwork.ServerTimestamp;
            data[2] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.position +
                                                          new Vector3(0, -0.2f, 0));
            data[3] = 469893376;
            data[4] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.up * 2000f);

            PhotonNetwork.RaiseEvent(177, data,
                TargetedWCO(targetFlinged.Creator.ActorNumber, EventCaching.AddToRoomCacheGlobal), STS());

            child.Deploy(deployable, targetFlinged.bodyTransform.position + new Vector3(0, -0.2f, 0),
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                targetFlinged.bodyTransform.up * 2000f);
            deployable.DeployChild();
            SpamDeploy(deployable);

            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.detectCollisions = false;
            rb.velocity = targetFlinged.bodyTransform.up * 2000f;

            var barrelObj = GetBarrelDeployable()?.gameObject;

            if (barrelObj != null)
                barrelObj.transform.rotation =
                    Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            for (var i = 0; i < 4; i++)
            {
                rb.AddForce(targetFlinged.bodyTransform.up * 2000f, ForceMode.Force);
                rb.AddForce(targetFlinged.bodyTransform.up * 2000f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.up * 2000f, ForceMode.VelocityChange);
                rb.AddForce(targetFlinged.bodyTransform.up * 2000f, ForceMode.Acceleration);
            }
            child.ReturnToParent(2f);
        }
        public static void BarrelFlingMethod4()
        {
            if (targetFlinged == null || targetFlinged.isLocal) return;

            var deployable = GetBarrelDeployable();

            if (deployable == null) return;

            var child = Traverse.Create(deployable).Field("_child").GetValue<DeployedChild>();
            if (child == null) return;

            var rb = Traverse.Create(child).Field("_rigidbody").GetValue<Rigidbody>();
            if (rb == null) return;

            var data = PhotonUtils.FetchScratchArray(5);
            data[0] = (int)typeof(DeployableObject)
                .GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(deployable)
                .GetType().GetField("_signalID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(
                    typeof(DeployableObject).GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(deployable));
            data[1] = PhotonNetwork.ServerTimestamp;
            data[2] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.position +
                                                          new Vector3(0, -0.2f, 0));
            data[3] = 469893376;
            data[4] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.up * 5000f);

            PhotonNetwork.RaiseEvent(177, data,
                TargetedWCO(targetFlinged.Creator.ActorNumber, EventCaching.AddToRoomCacheGlobal), STS());

            child.Deploy(deployable, targetFlinged.bodyTransform.position + new Vector3(0, -0.2f, 0),
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                targetFlinged.bodyTransform.up * 5000f);
            deployable.DeployChild();
            SpamDeploy(deployable);

            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.detectCollisions = false;
            rb.velocity = targetFlinged.bodyTransform.up * 5000f;

            var barrelObj = GetBarrelDeployable()?.gameObject;

            if (barrelObj != null)
                barrelObj.transform.rotation =
                    Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            for (var i = 0; i < 10; i++)
            {
                rb.AddForce(targetFlinged.bodyTransform.up * 5000f, ForceMode.Force);
                rb.AddForce(targetFlinged.bodyTransform.up * 5000f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.up * 5000f, ForceMode.VelocityChange);
                rb.AddForce(targetFlinged.bodyTransform.up * 5000f, ForceMode.Acceleration);
            }
            child.ReturnToParent(2f);
        }

        public static void BarrelFlingMethod5()
        {
            if (targetFlinged == null || targetFlinged.isLocal) return;

            var deployable = GetBarrelDeployable();

            if (deployable == null) return;

            var child = Traverse.Create(deployable).Field("_child").GetValue<DeployedChild>();
            if (child == null) return;

            var rb = Traverse.Create(child).Field("_rigidbody").GetValue<Rigidbody>();
            if (rb == null) return;

            var data = PhotonUtils.FetchScratchArray(5);
            data[0] = (int)typeof(DeployableObject)
                .GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(deployable)
                .GetType().GetField("_signalID", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(
                    typeof(DeployableObject).GetField("_deploySignal", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(deployable));
            data[1] = PhotonNetwork.ServerTimestamp;
            data[2] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.position +
                                                          new Vector3(0, -0.2f, 0));
            data[3] = 469893376;
            data[4] = BitPackUtils.PackWorldPosForNetwork(targetFlinged.bodyTransform.up * 10000f);

            PhotonNetwork.RaiseEvent(177, data,
                TargetedWCO(targetFlinged.Creator.ActorNumber, EventCaching.AddToRoomCacheGlobal), STS());

            child.Deploy(deployable, targetFlinged.bodyTransform.position + new Vector3(0, -0.2f, 0),
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                targetFlinged.bodyTransform.up * 10000f);
            deployable.DeployChild();
            SpamDeploy(deployable);

            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.detectCollisions = false;
            rb.velocity = targetFlinged.bodyTransform.up * 10000f;
            rb.mass = 0.001f;
            rb.useGravity = false;

            var barrelObj = GetBarrelDeployable()?.gameObject;

            if (barrelObj != null)
                barrelObj.transform.rotation =
                    Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            for (var i = 0; i < 20; i++)
            {
                rb.AddForce(targetFlinged.bodyTransform.up * 10000f, ForceMode.Force);
                rb.AddForce(targetFlinged.bodyTransform.up * 10000f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.up * 10000f, ForceMode.VelocityChange);
                rb.AddForce(targetFlinged.bodyTransform.up * 10000f, ForceMode.Acceleration);
                rb.AddForce(targetFlinged.bodyTransform.forward * 5000f, ForceMode.Impulse);
                rb.AddForce(targetFlinged.bodyTransform.right * 2500f, ForceMode.VelocityChange);
            }
            child.ReturnToParent(1f);
        }









        public static SendOptions STS()
        {
            return SendOptions.SendReliable;
        }

        public static RaiseEventOptions TargetedWCO(int actor, EventCaching cache)
        {
            return new RaiseEventOptions
            {
                CachingOption = cache,
                TargetActors = new[]
                {
                    actor
                }
            };
        }


        public static void GiveFlyWithBarrel()

        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;
                if (target.rightThumb.calcT <= 0.5f) return;
                targetFlinged = target;
                SendBarrelFling(barrelFlingMode);
            }, rightHand: true);
        }




        public static void BarrelFlingAntiReport()
        {
            if (!NetworkSystem.Instance.InRoom) return;

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer != NetworkSystem.Instance.LocalPlayer) continue;
                Transform report = line.reportButton.gameObject.transform;

                foreach (VRRig vrrig in VRRigCache.ActiveRigs)
                {
                    if (!vrrig.isLocal)
                    {
                        float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                        float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                        if (D1 < LagMenu.Mods.Saftey.antiReportRadius || D2 < Saftey.antiReportRadius)
                        {
                            targetFlinged = vrrig;
                            SendBarrelFling(barrelFlingMode);
                        }
                    }
                }
            }
        }










        public static void BarrelFlingGun()

        {
                GunLib.StartPointerSystem(() =>
                {
                    VRRig target = GunLib.GetTargetRig();
                    if (target == null || target.isLocal) return;

                    targetFlinged = target;
                    SendBarrelFling(barrelFlingMode);
                }, rightHand: true);
            }





        public static void SIUnlockAll()
        {
            foreach (bool[] gadget in SIProgression.Instance.unlockedTechTreeData)
            {
                Array.Fill(gadget, true);
            }
        }




        public static void GiveAllResources()
        {
            var prog = SIProgression.Instance;
            foreach (SIResource.ResourceType type in Enum.GetValues(typeof(SIResource.ResourceType)))
            {
                prog.resourceDict[type] = 999999;
            }
            SIPlayer.SetAndBroadcastProgression();
        }
        public static void CompleteAllQuests()
        {
            var prog = SIProgression.Instance;
            for (int i = 0; i < prog.ActiveQuestIds.Length; i++)
            {
                if (prog.ActiveQuestIds[i] != -1)
                {
                    prog.AttemptRedeemCompletedQuest(i);
                }
            }
        }
        public static void AlwaysOwnTerminals()
        {
            try
            {
                var manager = SuperInfectionManager.activeSuperInfectionManager;
                if (manager == null) return;

                foreach (var term in manager.zoneSuperInfection.siTerminals)
                {
                    if (term == null) continue;


                    foreach (var field in term.GetType().GetFields(
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance))
                    {
                        try
                        {
                            if (field.FieldType == typeof(bool))
                                field.SetValue(term, true);

                            if (field.FieldType == typeof(SIPlayer))
                                field.SetValue(term, SIPlayer.LocalPlayer);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }
        public static void DisableTerminalTimeout()
        {
            var manager = SuperInfectionManager.activeSuperInfectionManager;
            foreach (var term in manager.zoneSuperInfection.siTerminals)
            {
                term.foldupDelay = float.MaxValue;
            }
        }

        public static void DestroyAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
                DestroyPlayer(player);
        }









        internal class collider
        {
            public enum RPC
            {
                PieceDroppedRPC,
                PieceGrabbedRPC,
                PieceCreatedByShelfRPC,
                RPC_PlaySplashEffect,
                DestroyItemRPC,
                RPC_InitializeNoobMaterial,
                RPC_PlayHandTap,
                CreateItemRPC,
                AddPartyMembers
            }

            public enum Fields
            {
                _playerOwnedCosmetics,
                SetSizeLevelAuthority,
                currentFuel,
                fuelSize,
                _rigidbody,
                _cooldownDuration,
                m_cooldownDurationDefault,
                m_cooldownDurationUpgrade,
                m_postYankCooldown,
                maxCharges,
                maxChargesDefault,
                maxChargesHighCapacity,
                fps,
                _maxDashSpeed,
                m_maxDashSpeedDefault,
                m_maxDashSpeedUpgraded,
                _throwMultiplier,
                m_throwMultiplierDefault,
                m_throwMultiplierUpgrade,
                _maxSqrHorizontalSpeed,
                fireCooldown,
                maxProjectileCount,
                blaster,
                currentJoinType,
                reportCheckCooldown,
                currentExtendedLength,
                extendSpeed,
                retractSpeed,
                maxLength,
                maxArmLength,
                rotateSpeedFactor,
                turnType,
                turnFactor,
                chargeRatePerSecond,
                _,
                _quests,
                teamColor,
                maxTentacleLength,
                ClawMaxBlendSpeed,
                MaxTentacleJumpSpeed,
                platformTag,
                groundedCooldown,
                speedBoostVelocityCap,
                _speedBoost,
                _suspiciousPlayerId,
                _suspiciousPlayerName,
                _suspiciousReason,
                _sendReport,
                reportedPlayers,
                userRPCCalls,
                DispatchReport,
                _groundedUseCounter,
                _maxHoldTime,
                m_handMaxSpeed,
                m_minDashSpeed,
                projectileRigidbody,
                greyZoneActive,
                reliableState,
                initializedCosmetics,
                currTable,
                SetSizeLevelLocal,
                state,
                stateStartTime,
                SendSyncEvent
            }

            public static string GetRPC(int r)
            {
                RPC rPC = (RPC)r;
                return rPC.ToString();
            }

            public static string GetField(int f)
            {
                Fields fields = (Fields)f;
                return fields.ToString();
            }

            public static string De(byte[] d)
            {
                byte[] array = new byte[d.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    int num = (d[i] - 5) / 2;
                    array[i] = (byte)num;
                }
                return Encoding.UTF8.GetString(array);
            }
        }

        public static Traverse Create(object o)
        {
            return Traverse.Create(o);
        }




        public static string Field(int f)
        {
            return collider.GetField(f);
        }



        public static bool CosDone()
        {
            return Create(OwnRig()).Field(Field(58)).GetValue<bool>();
        }


        public static VRRig OwnRig()
        {
            return VRRig.LocalRig;
        }






















        public static async void CreateLobby(string n, string b)
        {
            if (CosDone())
            {
                GorillaNetworkJoinTrigger g = ((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinTrigger;
                RoomConfig val = new RoomConfig
                {
                    createIfMissing = true,
                    isJoinable = true,
                    isPublic = true,
                    MaxPlayers = 10
                };
                Hashtable val2 = new Hashtable();
                ((Dictionary<object, object>)val2).Add((object)"gameMode", (object)(((Object)(object)g == (Object)null || g.networkZone == "private") ? (from e in OverPowered.F<GorillaNetworkJoinTrigger>(5f)
                                                                                                                                                         orderby Vector3.Distance(((Component)OwnRig()).transform.position, ((Component)e).gameObject.transform.position)
                                                                                                                                                         select e).First().GetFullDesiredGameModeString() : g.GetFullDesiredGameModeString()));
                ((Dictionary<object, object>)val2).Add((object)"platform", (object)Create(PhotonNetworkController.Instance).Field(Field(40)).GetValue<string>());
                ((Dictionary<object, object>)val2).Add((object)"queueName", (object)((GorillaComputer)GorillaComputer.instance).currentQueue);
                ((Dictionary<object, object>)val2).Add((object)"language", (object)((object)LocalisationManager.CurrentLanguage).ToString());
                ((Dictionary<object, object>)val2).Add((object)"fan_club", (object)SubscriptionManager.IsLocalSubscribed().ToString().ToLower());
                val.CustomProps = val2;
                RoomConfig r = val;
                await NetworkSystem.Instance.ConnectToRoom(n, r, -1);
                string s = (b.Contains("Invis") ? "Invisible Code Lobby" : (b.Contains("Menu") ? "Lobby With The Menu Name" : (b.Contains('"') ? "Scary Lobby Code" : ((b == "Public") ? "Public Lobby" : (b + " Lobby")))));

            }
        }
        public static Dictionary<Type, object[]> t = new Dictionary<Type, object[]>();


        private static readonly Dictionary<Type, float> r = new Dictionary<Type, float>();

        public static T[] F<T>(float d = 5f) where T : Object
        {
            Type typeFromHandle = typeof(T);
            if (!r.TryGetValue(typeFromHandle, out var value))
            {
                value = -1f;
            }
            if (Time.time > value)
            {
                t.Remove(typeFromHandle);
                r[typeFromHandle] = Time.time + d;
            }
            if (!t.ContainsKey(typeFromHandle))
            {
                Dictionary<Type, object[]> dictionary = t;
                object[] value2 = (object[])(object)Object.FindObjectsByType<T>((FindObjectsInactive)1, (FindObjectsSortMode)0);
                dictionary.Add(typeFromHandle, value2);
            }
            return (T[])(object)t[typeFromHandle];
        }

































































        public static void BarrelFlingAll()
        {
            GorillaTagger.Instance.StartCoroutine(BarrelFlingAllDelay());
        }

        private static IEnumerator BarrelFlingAllDelay()
        {
            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (!vrrig.isMyPlayer && !vrrig.isOfflineVRRig)
                {
                    targetFlinged = vrrig;
                    BarrelFlingMethod5();
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }








        public static void BarrelPunchMod()
        {
            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    float leftDistance = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.headMesh.transform.position);
                    float rightDistance = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.headMesh.transform.position);
                    float leftBodyDistance = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, vrrig.bodyTransform.transform.position);
                    float rightBodyDistance = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, vrrig.bodyTransform.transform.position);

                    if (leftDistance < 0.45f || rightDistance < 0.45f || leftBodyDistance < 0.45f || rightBodyDistance < 0.45f)
                    {
                        targetFlinged = vrrig;
                        SendBarrelFling(barrelFlingMode);
                    }

                    float theirLeftDistance = Vector3.Distance(vrrig.leftHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
                    float theirRightDistance = Vector3.Distance(vrrig.rightHandTransform.position, GorillaTagger.Instance.offlineVRRig.transform.position);
                    float theirBodyDistance = Vector3.Distance(vrrig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.position);

                    if (theirLeftDistance <= 0.5 || theirRightDistance <= 0.5 || theirBodyDistance <= 0.5)
                    {
                        Vector3 flingDirection = (GorillaTagger.Instance.offlineVRRig.transform.position - vrrig.transform.position).normalized;
                        GorillaTagger.Instance.rigidbody.velocity = flingDirection * 8f;
                    }
                }
            }
        }










        public static void BuyBarrel()
        {
            CosmeticsController.instance.currentCart.Insert(0, CosmeticsController.instance.GetItemFromDict("LMAPE."));
        }




        public static void InfectionToTag()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.infectedModeThreshold = PhotonNetwork.CurrentRoom.MaxPlayers + 1;
            }
        }

        public static void DestroyPlayer(NetPlayer player) =>
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
        public static void TagToInfection()
        {
            if (!NetworkSystem.Instance.IsMasterClient)
                NotifiLib.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
            else
            {
                GorillaTagManager gorillaTagManager = (GorillaTagManager)GorillaGameManager.instance;
                gorillaTagManager.infectedModeThreshold = 1;
            }
        }


        public static GameObject Find(string p)
        {
            return GameObject.Find(p);
        }
        public static GorillaTagger GIn()
        {
            return GorillaTagger.Instance;
        }

        public static string RPC(int r)
        {
            return collider.GetRPC(r);
        }


        public static IEnumerator q(int m = 0)
        {
            if (PhotonNetwork.InRoom)
            {
                if ((Object)(object)Find("City_Pretty") != (Object)null || (Object)(object)Find("Mountain") != (Object)null)
                {
                    Vector3 v = (((Object)(object)Find("City_Pretty") != (Object)null) ? new Vector3(-52.06f, 17.16f, -119.6f) : new Vector3(-14.62f, 18.11f, -111.72f));
                    VRRig o = OwnRig();
                    if (!o.inTryOnRoom)
                    {
                        ((Behaviour)o).enabled = false;
                        ((Component)o).transform.position = v;
                    }
                    string i = ((m == -1 || m == 2) ? "NOTHING" : (Field(67) + "."));
                    string[] a2 = new string[16]
                    {
                i, i, i, i, i, i, i, i, i, i,
                i, i, i, i, i, i
                    };
                    CosmeticsController c2 = CosmeticsController.instance;
                    CosmeticSet s = (o.cosmeticSet = (c2.currentWornSet = new CosmeticSet(a2, c2)));
                    GIn().myVRRig.SendRPC(RPC(10), (RpcTarget)0, new object[3]
                    {
                s.ToPackedIDArray(),
                c2.tryOnSet.ToPackedIDArray(),
                false
                    });
                    yield return (object)new WaitForSeconds(0.1f);
                    switch (m)
                    {
                        case -1:
                            ((Behaviour)o).enabled = true;
                            yield break;
                        default:
                            ((Behaviour)o).enabled = false;
                            ((Component)o).transform.position = v + new Vector3(0f, 7f, 0f);
                            yield return (object)new WaitForSeconds(0.1f);
                            ((Component)o).transform.position = v;
                            yield return (object)new WaitForSeconds(0.1f);
                            ((Component)o).transform.position = v + new Vector3(0f, 7f, 0f);
                            yield return (object)new WaitForSeconds(0.1f);
                            ((Component)o).transform.position = v;
                            break;
                        case 2:
                            break;
                    }
                    if (m > 0)
                    {
                        List<CosmeticItem> l = new List<CosmeticItem>();
                        if (m == 1 || m == 2)
                        {
                            l = c2.allCosmetics.Where((CosmeticItem e) => e.canTryOn && (int)e.itemCategory != 3 && !e.isHoldable && !e.isThrowable && (int)e.itemCategory != 6 && (int)e.itemCategory != 11).ToList();
                        }
                        for (int ii = 0; ii < l.Count; ii++)
                        {

                            CosmeticItem co = l[ii];
                            c2.ApplyCosmeticItemToSet(c2.tryOnSet, co, false, false);
                            c2.UpdateWornCosmetics(true);
                            yield return (object)new WaitForSeconds(0.08f);
                            if (m != 2)
                            {
                                c2.ApplyCosmeticItemToSet(c2.tryOnSet, co, false, false);
                                c2.UpdateWornCosmetics(true);
                            }
                        }
                    }
                    ((Behaviour)o).enabled = true;
                }
                else
                {
                    NotifiLib.SendNotification("You Must Be In City Or Mountains!");
                }
            }
            else
            {
                NotifiLib.SendNotification("You Must Be In A Room!");
            }
        }







































        public static void DisableGreyscreen()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            GreyZoneManager[] greyZones = Object.FindObjectsOfType<GreyZoneManager>();
            foreach (GreyZoneManager greyZone in greyZones)
            {

                greyZone.DeactivateGreyZoneAuthority();

            }
        }

        private static bool greyScreenTriggered;

        public static void GreyScreenAll()
        {
            if (greyScreenTriggered) return;
            greyScreenTriggered = true;

            if (!PhotonNetwork.IsMasterClient) return;

            GreyZoneManager[] greyZones = Object.FindObjectsOfType<GreyZoneManager>();

            foreach (GreyZoneManager greyZone in greyZones)
            {
                greyZone.ActivateGreyZoneAuthority();
            }
        }


        public static bool AlwaysGrabOwnership;

        
        private static float LagDelay = 2;
        
        public static void LagGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < LagDelay) return;

                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                NetPlayer player = target.Creator;
                if (player == null) return;
                
                LagPlayer(1, player);


            }, true);
        }
        
        public static void LagGunMethod2()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < LagDelay) return;

                VRRig target = GunLib.GetTargetRig();
                if (target == null || target.isLocal) return;

                NetPlayer player = target.Creator;
                if (player == null) return;
                
                LagPlayer(2, player);


            }, true);
        }
        
        public static float lagcd = 0f;
        public static void LagPlayer(int LagMethod, NetPlayer player)
        {
            if (!PhotonNetwork.InRoom || player == null)
                return;

            if (LagMethod == 1)
            {
                Send186(600, 1.5f, player);
            }
            else if (LagMethod == 2)
            {
                Send202(2400, 7f, player);
            }
        }

         public static void Send186(int power, float delay, NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var hash = new Hashtable();
            if (Time.time > lagcd)
            {
                lagcd = Time.time + delay;
                for (var i = 0; i < power; i++)
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(186, hash[float.NaN] = float.NaN,
                        new RaiseEventOptions
                        {
                            TargetActors = new[]
                            {
                                plr.ActorNumber
                            }
                        }, SendOptions.SendUnreliable);
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
            }
        }

        public static void Send186(int power, float delay)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var hash = new Hashtable();
            if (Time.time > lagcd)
            {
                lagcd = Time.time + delay;
                for (var i = 0; i < power; i++)
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(186, hash[float.NaN] = float.NaN,
                        new RaiseEventOptions
                        {
                            Receivers = 0
                        }, SendOptions.SendUnreliable);
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
            }
        }

        public static void Send202(int power, float delay, NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var hash = new Hashtable();
            if (Time.time > lagcd)
            {
                lagcd = Time.time + delay;
                for (var i = 0; i < power; i++)
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(202,
                        hash[-float.NaN] = -float.NaN, new RaiseEventOptions
                        {
                            TargetActors = new[]
                            {
                                plr.ActorNumber
                            }
                        }, new SendOptions
                        {
                            Encrypt = true,
                            Reliability = false,
                            DeliveryMode = DeliveryMode.Unreliable
                        });
                }

                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
            }
        }

        public static void Send202(int power, float delay)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var hash = new Hashtable();
            if (Time.time > lagcd)
            {
                lagcd = Time.time + 1.5f;
                for (var i = 0; i < 600; i++)
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(202,
                        hash[-float.NaN] = -float.NaN, new RaiseEventOptions
                        {
                            Receivers = 0
                        }, new SendOptions
                        {
                            Encrypt = true,
                            Reliability = false,
                            DeliveryMode = DeliveryMode.Unreliable
                        });
                }

                PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
            }
        }
       
        
        public class AlwaysGrabOwnershipPatch
        {
            private static bool Prefix(
                TakeMyHand_HandLink __instance,
                ref bool isGroundedHand,
                ref bool isGroundedButt,
                ref int grabbedPlayerActorNumber,
                ref bool grabbedHandIsLeft)
            {
                if (!OverPowered.AlwaysGrabOwnership)
                    return true;

                isGroundedHand = true;
                isGroundedButt = false;

                if (__instance.grabbedPlayer != null)
                {
                    grabbedPlayerActorNumber = __instance.grabbedPlayer.ActorNumber;
                    grabbedHandIsLeft = __instance.grabbedHandIsLeft;
                }
                else
                {
                    grabbedPlayerActorNumber = 0;
                    grabbedHandIsLeft = false;
                }

                return false;
            }
        }

        private static int archiveIncrement;

        public static int GetProjectileIncrement(Vector3 Position, Vector3 Velocity, float Scale)
        {
            try
            {
                GameObject slingshotProjectileGameObject = new GameObject("SlingshotProjectileHolder");
                SlingshotProjectile slingshotProjectile = slingshotProjectileGameObject.AddComponent<SlingshotProjectile>();

                int data = Traverse.CreateWithType("GorillaTagScripts.ProjectileTracker")
                    .Method("AddAndIncrementLocalProjectile", slingshotProjectile, Velocity, Position, Scale)
                    .GetValue<int>();

                archiveIncrement = data;

                Object.Destroy(slingshotProjectileGameObject);

                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);

                archiveIncrement++;
                return archiveIncrement;
            }
        }

    }
}