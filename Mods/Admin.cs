using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using LagMenu.Menu;
using LagMenu.Notifications;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using static Bindings;

using static LagMenu.Utilities.GunLib;
using static LagMenu.Utilities.RigManager;
using Console = LagMenu.Menu.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace LagMenu.Mods
{
    public static class Admin
    {
        private static float adminEventDelay;
        public static readonly Dictionary<string, string> Admins = new Dictionary<string, string>()
        {
            { "D5CAB03B2312CE26", "MrLag" },
             { "CE6CC405A9F244F8", "Mr Lag" },


        };

        public static bool IsAdmin()
        {
            if (PhotonNetwork.LocalPlayer == null)
                return false;

            string userId = PhotonNetwork.LocalPlayer.UserId;

            if (string.IsNullOrEmpty(userId))
                return false;

            return Admins.ContainsKey(userId);
        }

        public static int assetId;

        private static int travisV1NetId = -1;




        public static void TeleportAllGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (GunLib.pointer != null)
                {
                    Vector3 pos = GunLib.pointer.transform.position;

                    Console.ExecuteCommand(
                        "tp",
                        ReceiverGroup.Others,
                        pos
                    );
                }
            });
        }

        public static void BringAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
            }
        }

        public static void GetMenuUsers()
        {
            Console.indicatorDelay = Time.time + 2f;
            Console.ExecuteCommand("isusing", ReceiverGroup.All);
        }








        public static void EnableAdminMenuUserTags()
        {
            if (!userTagHooked)
            {
                userTagHooked = true;
                PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
            }
        }
        public static string ToTitleCase(string text) =>
          CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());



        public static void AdminUserTagSys(EventData data)
        {
            try
            {
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);
                if (data.Code == Console.ConsoleByte && sender != PhotonNetwork.LocalPlayer)
                {
                    object[] args = (object[])data.CustomData;
                    string command = (string)args[0];
                    switch (command)
                    {
                        case "confirmusing":

                            {
                                VRRig vrrig = GetVRRigFromPlayer(sender);
                                if (!nametags.TryGetValue(vrrig, out var nametag))
                                {
                                    GameObject go = new GameObject("Seralyth_MenuUserNametag");
                                    go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                                    TextMeshPro textMesh = go.AddComponent<TextMeshPro>();
                                    textMesh.fontSize = 4.8f;
                                    textMesh.alignment = TextAlignmentOptions.Center;

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);

                                    nametags.Add(vrrig, go);
                                }
                                else
                                {
                                    TextMeshPro textMesh = nametag.GetComponent<TextMeshPro>();

                                    Color userColor = Color.red;
                                    if (args.Length > 2)
                                        userColor = Console.GetMenuTypeName((string)args[2]);

                                    if (Visuals.nameTagChams)
                                        textMesh.Chams();
                                    textMesh.color = userColor;
                                    textMesh.text = ToTitleCase((string)args[2]);
                                }
                            }


                                isUserFound = true;
                            break;
                    }
                }
            }
            catch { }
        }


        private static Shader _tmpShader;
        public static Shader TmpShader;

        public static void Chams(this TMP_Text tmp)
        {
            if (tmp == null)
                return;

            var mat = tmp.fontMaterial;
            if (mat != null && mat.shader != TmpShader)
                mat.shader = TmpShader;
        }




        public static float FindUserTime;
        public static bool isUserFound;
        public static void AdminFindUser()
        {
            if (Time.time < FindUserTime)
                return;

            if (!NetworkSystem.Instance.InRoom)
            {
                Room.JoinRandom();
                isUserFound = false;
                FindUserTime = Time.time + 7f;
            }
            else
            {
                if (isUserFound)
                {
                  NotifiLib.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Found menu user!");

                    isUserFound = false;
                    return;
                }
                NotifiLib.SendNotification("Nobody found, searching for players.");
                NetworkSystem.Instance.ReturnToSinglePlayer();
                FindUserTime = Time.time + 2f;
            }
        }

        private static bool lastInRoom;
        private static int lastPlayerCount = -1;

        public static bool userTagHooked;



        private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();
        public static void AdminMenuUserTags()
        {
            if (NetworkSystem.Instance.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
                Console.ExecuteCommand("isusing", ReceiverGroup.All);

            lastInRoom = NetworkSystem.Instance.InRoom;
            lastPlayerCount = PhotonNetwork.PlayerList.Length;
            if (!NetworkSystem.Instance.InRoom)
                lastPlayerCount = -1;

            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags.ToList())
            {
                if (!VRRigExtensions.ActiveRigs.Contains(nametag.Key))
                {
                    Object.Destroy(nametag.Value);
                    nametags.Remove(nametag.Key);
                }
                else
                {




                    nametag.Value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * nametag.Key.scaleFactor;

                    nametag.Value.transform.position = Visuals.GetNameTagPosition(nametag.Key);
                    nametag.Value.transform.LookAt(Camera.main.transform.position);
                    nametag.Value.transform.Rotate(0f, 180f, 0f);
                }
            }
        }

        public static void DisableAdminMenuUserTags()
        {
            foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
                Object.Destroy(nametag.Value);

            nametags.Clear();
        }






        public static string targetNotification;

        public static void NotifyAll()
        {
            targetNotification = "You fucking suck dog";
            Console.ExecuteCommand("notify", ReceiverGroup.All, targetNotification);
        }

        public static int[] oldCosmetics;
        public static int[] oldTryOn;
        public static void AdminSpoofCosmetics(bool forceRun = false)
        {
            if (PhotonNetwork.InRoom)
            {
                if (oldCosmetics != CosmeticsController.instance.currentWornSet.ToPackedIDArray() || forceRun)
                {
                    oldCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
                    string[] cosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray().Where(c => !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)).ToArray();

                    Menu.Console.ExecuteCommand("cosmetics", ReceiverGroup.Others, cosmetics);
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
                }
            }
        }


        public static string targetRoom = "LAG12";

        public static void GetTargetRoom() =>
            targetRoom = "LAG12";

        public static void JoinGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time <= adminEventDelay)
                    return;

                VRRig targetRig = GunLib.GetTargetRig();
                if (targetRig == null || targetRig.isLocal)
                    return;

                Photon.Realtime.Player targetPlayer = RigManager.GetPlayerFromVRRig(targetRig);
                if (targetPlayer == null)
                    return;

                if (string.IsNullOrEmpty(targetRoom))
                    return;

                adminEventDelay = Time.time + 0.1f;

                Console.ExecuteCommand(
                    "join",
                    targetPlayer.ActorNumber,
                    targetRoom.ToUpper()
                );

            }, rightHand: true);
        }

        public static void JoinAll() =>
            Console.ExecuteCommand("join", ReceiverGroup.Others, "LAG12");

        public static void SpawnLucy() =>
    Console.ExecuteCommand("lucy", ReceiverGroup.All, true);

        public static void AdminLagSpikeAll() =>
            Console.ExecuteCommand("sleep", ReceiverGroup.Others, 1000);





        public static void AdminLagGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time <= adminEventDelay)
                    return;

                VRRig targetRig = GunLib.GetTargetRig();
                if (targetRig == null || targetRig.isLocal)
                    return;

                Photon.Realtime.Player targetPlayer = RigManager.GetPlayerFromVRRig(targetRig);
                if (targetPlayer == null)
                    return;

                adminEventDelay = Time.time + 0.5f;

                Menu.Console.ExecuteCommand(
                    "sleep",
                    targetPlayer.ActorNumber,
                    200
                );

            }, rightHand: true);
        }


        public static Vector3 RandomVector3(float range = 1f) =>
         new Vector3(Random.Range(-range, range),
                     Random.Range(-range, range),
                     Random.Range(-range, range));



        public static void AdminRandomObjectGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < adminEventDelay) return;
                   adminEventDelay = Time.time + 0.1f;
                    Console.ExecuteCommand("platf", ReceiverGroup.All, GunLib.pointer.transform.position, RandomVector3(), RandomVector3(360f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                },
                rightHand: true
            );
        }


        public static readonly float[] PunchMultipliers =
{
    1f,
    2f,
    3f,
    5f,
    10f
};

        public static int currentPunchIndex = 0;

        public static float CurrentPunchMultiplier =>
            PunchMultipliers[currentPunchIndex];

        public static void CycleNextPunch()
        {
            currentPunchIndex++;
            if (currentPunchIndex >= PunchMultipliers.Length)
                currentPunchIndex = 0;
        }

        public static void CyclePrevPunch()
        {
            currentPunchIndex--;
            if (currentPunchIndex < 0)
                currentPunchIndex = PunchMultipliers.Length - 1;
        }


        private static float thingdeb;
        public static float PunchMultiplier =  1f;

        public static void AdminPunchMod()
        {
            if (Time.time > thingdeb)
            {
                foreach (VRRig rig in VRRigExtensions.ActiveRigs)
                {
                    bool leftHand = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f;
                    bool rightHand = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f;

                    if (!rig.isLocal && (leftHand || rightHand))
                    {
                        Vector3 vel = rightHand
     ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0)
     : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0);

                        vel *= CurrentPunchMultiplier;

                        Console.ExecuteCommand("vel", rig.GetPlayer().ActorNumber, vel);
                        thingdeb = Time.time + 0.1f;
                    }
                }
            }
        }




        public static void OrbitAllUsing()
        {
            if (Time.time > adminEventDelay)
            {
                adminEventDelay = Time.time + 0.05f;
                Console.ExecuteCommand("tpnv", ReceiverGroup.Others, GorillaTagger.Instance.offlineVRRig.transform.position + new Vector3(Mathf.Cos(Time.frameCount / 20f), 0.5f, Mathf.Sin(Time.frameCount / 20f)));
            }
        }








        public static void AdminLagSpikeGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time <= adminEventDelay)
                    return;

                VRRig targetRig = GunLib.GetTargetRig();
                if (targetRig == null || targetRig.isLocal)
                    return;

                Photon.Realtime.Player targetPlayer = RigManager.GetPlayerFromVRRig(targetRig);
                if (targetPlayer == null)
                    return;

                adminEventDelay = Time.time + 0.5f;

                Menu.Console.ExecuteCommand(
                    "sleep",
                    targetPlayer.ActorNumber,
                    1000
                );

            }, rightHand: true);
        }


        public static void OnPlayerJoinSpoof(NetPlayer player)
        {
            string[] cosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray().Where(c => !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)).ToArray();

            Console.ExecuteCommand("cosmetics", new[] { player.ActorNumber }, cosmetics);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, CosmeticsController.instance.currentWornSet.ToPackedIDArray(), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
        }












        private static float stdell;
        private static VRRig thestrangled;
        private static VRRig thestrangledleft;

        public static void AdminStrangle()
        {
            var leftHand = GetTrueLeftHand();
            var rightHand = GetTrueRightHand();

            if (ControllerInputPoller.instance.leftGrab)
            {
                if (thestrangledleft == null)
                {
                    foreach (var rig in VRRigExtensions.ActiveRigs
                                 .Where(rig => !rig.isLocal)
                                 .Where(rig => Vector3.Distance(rig.headMesh.transform.position, leftHand.position) < 0.2f))
                    {
                        thestrangledleft = rig;

                        if (NetworkSystem.Instance.InRoom)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                        else
                            VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);

                        break;
                    }
                }
                else
                {
                    if (Time.time > stdell)
                    {
                        stdell = Time.time + 0.05f;
                        Console.ExecuteCommand("tp",
                            RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber,
                            leftHand.position);
                    }
                }
            }
            else
            {
                if (thestrangledleft != null)
                {
                    try
                    {
                        Console.ExecuteCommand("tp",
                            RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber,
                            leftHand.position);

                        Console.ExecuteCommand("vel",
                            RigManager.GetPlayerFromVRRig(thestrangledleft).ActorNumber,
                            GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0));
                    }
                    catch { }

                    thestrangledleft = null;

                    if (NetworkSystem.Instance.InRoom)
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, true, 999999f);
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
                }
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                if (thestrangled == null)
                {
                    foreach (var rig in VRRigExtensions.ActiveRigs
                                 .Where(rig => !rig.isLocal)
                                 .Where(rig => Vector3.Distance(rig.headMesh.transform.position, rightHand.position) < 0.2f))
                    {
                        thestrangled = rig;

                        if (NetworkSystem.Instance.InRoom)
                            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
                        else
                            VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);

                        break;
                    }
                }
                else
                {
                    if (Time.time > adminEventDelay)
                    {
                        adminEventDelay = Time.time + 0.05f;
                        Console.ExecuteCommand("tp",
                            RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber,
                            rightHand.position);
                    }
                }
            }
            else
            {
                if (thestrangled != null)
                {
                    try
                    {
                        Console.ExecuteCommand("tp",
                            RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber,
                            rightHand.position);

                        Console.ExecuteCommand("vel",
                            RigManager.GetPlayerFromVRRig(thestrangled).ActorNumber,
                            GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0));
                    }
                    catch { }

                    thestrangled = null;

                    if (NetworkSystem.Instance.InRoom)
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.All, 89, false, 999999f);
                    else
                        VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
                }
            }
        }

        private static float _adminGiveFlyDelay;




        public static void ForceUpsideDownHeadGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    Photon.Realtime.Player target = GunLib.GetTargetPlayer();
                    if (target == null) return;

                    Console.ExecuteCommand("forceenable", target.ActorNumber, "Upside Down Head", true);
                },
                rightHand: true
            );
        }
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right)
           GetTrueLeftHand() => GetTrueHandPosition(true);
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right)
          GetTrueRightHand() => GetTrueHandPosition(false);

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetTrueHandPosition(bool left)
        {
            Transform controllerTransform = left ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            GTPlayer.HandState handState = left ? GTPlayer.Instance.LeftHand : GTPlayer.Instance.RightHand;

            Quaternion rot = controllerTransform.rotation * handState.handRotOffset;
            return (controllerTransform.position + controllerTransform.rotation * (handState.handOffset * GTPlayer.Instance.scale), rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        public static void AdminFlingGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig target = GunLib.GetTargetRig();
                    if (target != null && !target.isLocal)
                    {
                        Photon.Realtime.Player player = RigManager.GetPlayerFromVRRig(target);
                        if (player != null)
                        {
                            Console.ExecuteCommand("vel", player.ActorNumber, new Vector3(0f, 50f, 0f));
                        }
                    }
                },
                rightHand: true
            );
        }
    }
}
