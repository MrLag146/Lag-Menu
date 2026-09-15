using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ExitGames.Client.Photon;
using GorillaNetworking;
using HarmonyLib;
using LagMenu.Notifications;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static LagMenu.Mods.VStump;
using static Mono.Security.X509.X509Stores;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace LagMenu.Mods
{
    internal class Saftey

    {
        public static float antiReportRadius = 2.65f;
        public static VRRig reportRig;
        public static void AntiReport(System.Action<VRRig, Vector3> onReport)
        {
            if (!NetworkSystem.Instance.InRoom) return;

            if (reportRig != null)
            {
                onReport?.Invoke(reportRig, reportRig.transform.position);
                reportRig = null;
                return;
            }

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer != NetworkSystem.Instance.LocalPlayer) continue;
                Transform report = line.reportButton.gameObject.transform;

                foreach (var vrrig in from vrrig in VRRigCache.ActiveRigs where !vrrig.isLocal let D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position) let D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position) where D1 < 0.65f || D2 < 0.65f select vrrig)
                    onReport?.Invoke(vrrig, report.transform.position);
            }
        }

        public static float antiReportDelay;
        public static void AntiReportDisconnect()
        {
            AntiReport((vrrig, position) =>
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
            });
        }
        public static void AntiReportNotify()
        {
            AntiReport((vrrig, position) =>
            {
                NotifiLib.SendNotification("You Have Been Reported");
            });
        }
        public static void AntiReportReconnect()
        {
            AntiReport((vrrig, position) =>
            {
                string name = PhotonNetwork.CurrentRoom.Name;
                NetworkSystem.Instance.ReturnToSinglePlayer();
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(name, GorillaNetworking.JoinType.Solo);
            });
        }

        public static      bool AntiCheatNotiEnabled;
        public static void AntiCheatNotification()
        {
            AntiCheatNotiEnabled = true;
        }

        public static void AntiReportKill()
        {
            if (!NetworkSystem.Instance.InRoom) return;

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line == null || line.linePlayer == null) continue;

                Transform report = line.reportButton?.gameObject?.transform;
                if (report == null) continue;

                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig == null || rig.isLocal) continue;

                    float d1 = Vector3.Distance(rig.rightHandTransform.position, report.position);
                    float d2 = Vector3.Distance(rig.leftHandTransform.position, report.position);

                    if (d1 < 2.65f || d2 < 2.65f)
                    {
                        Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                        if (target != null)
                            MysteryTagNetwork.Kill(rig);
                    }
                }
            }
        }

        public static void ResetNetworkLimits()
        {
            if (!PhotonNetwork.InRoom)
            {
                return;
            }
            try
            {
                ((MonkeAgent)MonkeAgent.instance).rpcErrorMax = int.MaxValue;
                ((MonkeAgent)MonkeAgent.instance).rpcCallLimit = int.MaxValue;
                ((MonkeAgent)MonkeAgent.instance).logErrorMax = int.MaxValue;
                ReflectionCompat.GetField<System.Collections.IDictionary>(MonkeAgent.instance, "userRPCCalls")?.Clear();
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                PhotonNetwork.SendAllOutgoingCommands();
            }
            catch (Exception)
            {
            }
        }


        private static readonly List<VRRig> nameSpoofRigs = new List<VRRig>();

        private static readonly List<VRRig> colorSpoofRigs = new List<VRRig>();

        public static void ColorSpoof()
        {
            List<VRRig> list = new List<VRRig>();
            foreach (VRRig colorSpoofRig in colorSpoofRigs)
            {
                if (!VRRigCache.ActiveRigs.Contains(colorSpoofRig))
                {
                    list.Add(colorSpoofRig);
                }
            }
            foreach (VRRig item in list)
            {
                colorSpoofRigs.Remove(item);
            }
            list.Clear();
            foreach (VRRig item2 in from rig in VRRigCache.ActiveRigs
                                    where !rig.isLocal
                                    where !colorSpoofRigs.Contains(rig)
                                    select rig)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RigManager.GetNetPlayerFromVRRig(item2), new object[3]
                {
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
                });
                colorSpoofRigs.Add(item2);
            }
        }


        public static void NameSpoof()
        {
            List<VRRig> list = new List<VRRig>();
            foreach (VRRig nameSpoofRig in nameSpoofRigs)
            {
                if (!VRRigCache.ActiveRigs.Contains(nameSpoofRig))
                {
                    list.Add(nameSpoofRig);
                }
            }
            foreach (VRRig item in list)
            {
                nameSpoofRigs.Remove(item);
            }
            list.Clear();
            string nickName = PhotonNetwork.NickName;
            foreach (VRRig current3 in VRRigCache.ActiveRigs)
            {
                if (current3.isLocal || nameSpoofRigs.Contains(current3))
                {
                    continue;
                }
                string text = (UnityEngine.Random.Range(0, 3) == 0) ? namePrefix[UnityEngine.Random.Range(0, namePrefix.Length)] : "";
                string text2 = (UnityEngine.Random.Range(0, 3) == 0) ? nameSuffix[UnityEngine.Random.Range(0, nameSuffix.Length)] : "";
                string text3 = text + names[UnityEngine.Random.Range(0, names.Length)] + text2;
                RigManager.ChangeName((text3.Length <= 12) ? text3 : text3.Substring(0, 12), noColor: true);
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RigManager.GetNetPlayerFromVRRig(current3), new object[3]
                {
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f)
                });
                nameSpoofRigs.Add(current3);
            }
            if (PhotonNetwork.NickName != nickName)
            {
                PhotonNetwork.NickName = nickName;
            }
        }










        public static readonly string[] namePrefix = new string[11]
{
        "EPIC", "REAL", "NOT", "SILLY", "LITTLE", "BIG", "MAYBE", "SUB2", "OG", "FR",
        "NOT"
};

        public static readonly string[] nameSuffix = new string[12]
        {
        "GT", "VR", "LOL", "FAN", "XD", "LOL", "MONKE", "YT", "NOT", "FR",
        "LMAO", "GTAG"
        };

        public static readonly string[] names = new string[67]
        {
        "0", "PBBV", "J3VU", "BEES", "NEMO", "LEMMING", "BILLY", "TIMMY", "MINIGAMES", "JMANCURLY",
        "VMT", "ELLIOT", "DAISY09", "MONK", "MONKE", "MONKI", "MONKEY", "MONKIY", "GORILL", "GOORILA",
        "GORILLA", "TTT", "TTTPIG", "PPPTIG", "K9", "BANANA", "PEANUTBUTTER", "GHOSTMONKE", "STATUE", "NOVA",
        "LUNAR", "MOON", "SUN", "RANDOM", "UNKNOWN", "GLITCH", "BUG", "ERROR", "CODE", "HACKER",
        "MODDER", "INVIS", "INVISIBLE", "TAGGER", "UNTAGGED", "BLUE", "RED", "GREEN", "PURPLE", "YELLOW",
        "BLACK", "WHITE", "BROWN", "CYAN", "GRAY", "GREY", "BANNED", "LEMON", "PLUSHIE", "CHEETO",
        "TIKTOK", "YOUTUBE", "TWITCH", "DISCORD", "MODDER", "HACKER", "SCOTT",
        };


        public static void RPCProtection()
        {
            if (!PhotonNetwork.InRoom)
                return;

            try
            {
                MonkeAgent.instance.rpcErrorMax = int.MaxValue;
                MonkeAgent.instance.rpcCallLimit = int.MaxValue;
                MonkeAgent.instance.logErrorMax = int.MaxValue;

                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;

                PhotonNetwork.SendAllOutgoingCommands();
            }
            catch { }
        }






        private static object GetDefaultValue(string fieldName)
        {
            if (fieldName.Contains("Max") || fieldName.Contains("Limit") || fieldName.Contains("Count"))
                return int.MaxValue;
            if (fieldName.Contains("userRPCCalls"))
                return null;
            if (fieldName.Contains("_sendReport"))
                return false;
            return null;
        }

        public static object RunViewUpdate()
        {
            return typeof(PhotonNetwork).GetMethod("RunViewUpdate", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }

        static float rpcDel;



        public static void AntiRPCKick()
        {
            try
            {
                AntiRPCKicker();
                Type gorillaNotType = typeof(MonkeAgent);
                MonkeAgent gorillaInstance = MonkeAgent.instance;
                if (gorillaInstance == null)
                    return;
                MonkeAgent.instance.rpcErrorMax = int.MaxValue;
                MonkeAgent.instance.rpcCallLimit = int.MaxValue;
                MonkeAgent.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                var peer = PhotonNetwork.NetworkingClient?.LoadBalancingPeer;
                if (peer != null)
                {
                    peer.SentCountAllowance = int.MaxValue;
                    peer.QuickResendAttempts = 3;
                    peer.CrcEnabled = false;
                    peer.UseByteArraySlicePoolForEvents = false;
                    peer.TrafficStatsEnabled = false;
                    peer.TrafficStatsReset();
                    peer.SendOutgoingCommands();
                    try
                    {
                        var type = peer.GetType();
                        var queueField = type.GetField("outgoingStreamQueue", BindingFlags.NonPublic | BindingFlags.Instance);
                        var queue = queueField?.GetValue(peer) as System.Collections.IList;
                        queue?.Clear();
                        var commandsField = type.GetField("commandList", BindingFlags.NonPublic | BindingFlags.Instance);
                        var commands = commandsField?.GetValue(peer) as System.Collections.IList;
                        commands?.Clear();
                        var resentField = type.GetField("resentCommandsCount", BindingFlags.NonPublic | BindingFlags.Instance);
                        resentField?.SetValue(peer, 0);
                    }
                    catch { }
                }
                PhotonNetwork.NetworkStatisticsEnabled = false;
                ValueTuple<Type, object, string, bool>[] targets = new ValueTuple<Type, object, string, bool>[]
                {
                    (gorillaNotType, gorillaInstance, "rpcErrorMax", false),
                    (gorillaNotType, gorillaInstance, "rpcCallLimit", false),
                    (gorillaNotType, gorillaInstance, "logErrorMax", false),
                    (gorillaNotType, gorillaInstance, "userRPCCalls", false),
                    (gorillaNotType, gorillaInstance, "_sendReport", false),
                    (typeof(PhotonNetwork), null, "QuickResends", true),
                    (typeof(PhotonNetwork), null, "MaxResendsBeforeDisconnect", true)
                };
                foreach (var entry in targets)
                    TrySetMember(entry.Item1, entry.Item2, entry.Item3, GetDefaultValue(entry.Item3), entry.Item4);
                try
                {
                    var userRPCCallsField = gorillaNotType.GetField("userRPCCalls", BindingFlags.NonPublic | BindingFlags.Instance);
                    var userRPCCalls = userRPCCallsField?.GetValue(gorillaInstance) as System.Collections.IDictionary;
                    userRPCCalls?.Clear();
                }
                catch { }
                PhotonNetwork.NetworkingClient.OpRaiseEvent(200, new Hashtable()
                {
                    { 0, GorillaTagger.Instance.myVRRig.ViewID }
                }, new RaiseEventOptions
                {
                    CachingOption = (EventCaching)6,
                    TargetActors = new int[] { PhotonNetwork.LocalPlayer.ActorNumber }
                }, SendOptions.SendReliable);
                if (Time.time > rpcDel)
                {
                    try
                    {
                        rpcDel = Time.time + 0.47f;
                        PhotonNetwork.RemoveBufferedRPCs(int.MaxValue, null, null);
                        PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
                        PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
                        PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig.GetView);
                        PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
                        Traverse yeah = Traverse.Create(typeof(PhotonNetwork));
                        yeah.Property("ResentReliableCommands").SetValue(0);
                        PhotonNetwork.NetworkingClient.Service();
                        PhotonNetwork.NetworkingClient.OpChangeGroups(null, new byte[] { 1, 2, 3, 4 });
                        PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
                        try
                        {
                            var system = AppDomain.CurrentDomain.GetAssemblies()
                                .First(a => a.GetName().Name == "Assembly-CSharp")
                                .GetType("RoomSystem");

                            system.GetMethod("OnPlayerLeftRoom", BindingFlags.NonPublic | BindingFlags.Instance)
                                .Invoke(null, new object[] { NetworkSystem.Instance.LocalPlayer });
                        }
                        catch { }
                        try
                        {
                            NetSystemState state = new NetSystemState();
                            PeerStateValue val = new PeerStateValue();
                            state.Equals(NetSystemState.Connecting);
                            val.Equals(PeerStateValue.Connected);
                            RunViewUpdate();
                        }
                        catch { }
                        PhotonNetwork.SendAllOutgoingCommands();
                        try
                        {
                            var photonViewList = typeof(PhotonNetwork).GetField("photonViewList",
                                BindingFlags.NonPublic | BindingFlags.Static);
                            var viewDict = photonViewList?.GetValue(null) as System.Collections.IDictionary;
                            if (viewDict != null)
                            {
                                var keysToRemove = new System.Collections.ArrayList();
                                foreach (System.Collections.DictionaryEntry entry in viewDict)
                                {
                                    var view = entry.Value as PhotonView;
                                    if (view != null && view.IsMine && view.isRuntimeInstantiated)
                                        keysToRemove.Add(entry.Key);
                                }
                                foreach (var key in keysToRemove)
                                    viewDict.Remove(key);
                            }
                        }
                        catch { }
                    }
                    catch { }
                }
                MethodInfo refresh = gorillaNotType.GetMethod("RefreshRPCs", BindingFlags.NonPublic | BindingFlags.Instance);
                refresh?.Invoke(gorillaInstance, null);
            }
            catch { }
        }
        private static byte[] cachedSerializedRpc;

        private static void AntiRPCKicker()
        {
            for (int i = 0; i < 1300; i++)
                ResendCachedRpc();
            try
            {
                var peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
                var field = peer.GetType().GetField("outgoingStreamQueue", BindingFlags.Instance | BindingFlags.NonPublic);

                if (field != null)
                {
                    IList list = field.GetValue(peer) as IList;
                    if (list != null && list.Count > 0)
                        cachedSerializedRpc = list[list.Count - 1] as byte[];
                }
            }
            catch
            {
                cachedSerializedRpc = null;
            }
        }





        public static void RPCShield()
        {
            if (PhotonNetwork.InRoom)
            {
                ((MonkeAgent)MonkeAgent.instance).rpcErrorMax = int.MaxValue;
                ((MonkeAgent)MonkeAgent.instance).rpcCallLimit = int.MaxValue;
                ((MonkeAgent)MonkeAgent.instance).logErrorMax = int.MaxValue;
                ((MonkeAgent)MonkeAgent.instance).userRPCCalls.Clear();
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }















        private static void ResendCachedRpc()
        {
            if (cachedSerializedRpc == null)
                return;
            try
            {
                var peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
                var type = peer.GetType();
                var method = type.GetMethod("SendReliable", BindingFlags.Instance | BindingFlags.NonPublic)
                            ?? type.GetMethod("SendUnreliable", BindingFlags.Instance | BindingFlags.NonPublic);
                method?.Invoke(peer, new object[] { cachedSerializedRpc });
            }
            catch
            {
                SetTick(9999f);
            }
        }

        public static void SetTick(float tickMultiplier)
        {
            var photonMono = GameObject.Find("PhotonMono")?.GetComponent<PhotonHandler>();
            if (photonMono != null)
            {
                Traverse.Create(photonMono).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * tickMultiplier));
                PhotonHandler.SendAsap = true;
            }
        }

        private static bool TrySetMember(Type type, object instance, string fieldName, object value, bool isStatic)
        {
            try
            {
                var field = type.GetField(fieldName,
                    (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                    BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(instance, value);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void EnableAntiAFK()
        {
            PhotonNetworkController.Instance.disableAFKKick = true;
        }

        public static void DisableAntiAFK()
        {
            PhotonNetworkController.Instance.disableAFKKick = false;
        }

        private static bool steamWarnActive;
        private static readonly HashSet<string> warnedUserIds = new HashSet<string>();

        public static void WarnOnSteamUserJoin()
        {
            steamWarnActive = true;
            if (PhotonNetwork.CurrentRoom == null) return;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsLocal || warnedUserIds.Contains(p.UserId)) continue;
                warnedUserIds.Add(p.UserId);

                if (!string.IsNullOrEmpty(p.UserId) && p.UserId.Length == 17 && p.UserId.All(char.IsDigit))
                {
                    string nm = string.IsNullOrEmpty(p.NickName) ? "A player" : p.NickName;
                    NotifiLib.SendNotification("LagMenu", $"{nm} (Steam) joined your lobby.");
                }
            }
        }

        public static void DisableWarnOnSteamUserJoin()
        {
            steamWarnActive = false;
            warnedUserIds.Clear();
        }
    }
}
