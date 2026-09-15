using ExitGames.Client.Photon;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using LagMenu;
using LagMenu.Notifications;
using LagMenu.Patches;
using Liv.Lck.Tablet;
using Photon;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using POpusCodec.Enums;using UnityEngine.Events;
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
    internal class VStump
    {
        public const string MrlaglolUrl = "https://mrlag.lol/";


        public static void VStumpCrash(int actorNumber)
        {
            for (int i = 0; i < 11; i++)
                PhotonNetwork.RaiseEvent(180,
                    new object[] { MrlaglolUrl, true },
                    new RaiseEventOptions { TargetActors = new[] { actorNumber } },
                    SendOptions.SendUnreliable);
        }

        public static void VStumpCrashGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                    if (target == null) return;

                    if (_wasShooting) return;
                    _wasShooting = true;

                    VStumpCrash(target.ActorNumber);
                },
                onGrip: () => { _wasShooting = false; },
                rightHand: true
            );

            if (!GunLib.isGripping)
                _wasShooting = false;
        }



        public static void VStumpCrashAll()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                VStumpCrash(player.ActorNumber);
            }
        }
        private static IEnumerator CrashAll()
        {
            foreach (VRRig rig in RigManager.GetAllRigs())
            {
                if (rig == null || rig.isLocal || rig.Creator == null) continue;
                VStumpCrash(rig.Creator.ActorNumber);
                yield return new WaitForSeconds(2f);
            }
        }
        public static void SpawnLucyOnPlayer(int actorNumber)
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "SummonLucy", (double)actorNumber },
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }


        public static void SpawnLucyAll()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                SpawnLucyOnPlayer(player.ActorNumber);
            }
        }

        public static void LightningStrikePlayer(int actorNumber)
        {
            PhotonNetwork.RaiseEvent(180, new object[] { "SummonThunder", (double)actorNumber },
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        private static float _lightningDelay;

        public static void LightningStrikeGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < _lightningDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal) return;

                Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                if (target == null) return;

                _lightningDelay = Time.time + 0.5f;
                LightningStrikePlayer(target.ActorNumber);
            }, rightHand: true);
        }

        public static void SpawnLucyGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal) return;

                Photon.Realtime.Player target = RigManager.GetPlayerFromVRRig(rig);
                if (target == null) return;

                SpawnLucyOnPlayer(target.ActorNumber);
            }, rightHand: true);
        }


        private static float _murderKillDelay;
        private static float _sheriffKillDelay;

        public static void MurderKillGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _murderKillDelay) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    _murderKillDelay = Time.time + 0.2f;
                    MysteryTagNetwork.Kill(rig, true);
                },
                rightHand: true
            );
        }

        public static void SheriffKillGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _sheriffKillDelay) return;
                    VRRig rig = GunLib.GetTargetRig();
                    if (rig == null || rig.isLocal) return;

                    _sheriffKillDelay = Time.time + 0.12f;
                    MysteryTagNetwork.SheriffKill(rig, true);
                },
                rightHand: true
            );
        }

        public static void KillSheriff()
        {
            if (MysteryTagState.SheriffActorNumber is { } actorNumber)
            {
                NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
                if (player != null)
                    MysteryTagNetwork.Kill(RigManager.GetVRRigFromPlayer(player));
            }
        }

        public static void KillMurderer()
        {
            if (MysteryTagState.MurdererActorNumber is { } actorNumber)
            {
                NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
                if (player != null)
                    MysteryTagNetwork.Kill(RigManager.GetVRRigFromPlayer(player));
            }
        }

        public static void MysteryKillAll()
        {
            foreach (VRRig rig in RigManager.GetAllRigs())
                MysteryTagNetwork.Kill(rig);
        }

        public static void KillAllInnocents()
        {
            foreach (VRRig rig in RigManager.GetAllRigs())
            {
                if (rig?.Creator == null ||
                    MysteryTagState.GetRole(rig.Creator.ActorNumber) != MysteryTagRole.Innocent)
                    continue;

                MysteryTagNetwork.Kill(rig);
            }
        }


        public static class MysteryTagNetwork
        {
            private const float KillVelocity = 50f;

            public static void Send(string eventName, params object[] data)
            {
                if (!PhotonNetwork.InRoom)
                    return;

                object[] content = new object[data.Length + 1];
                content[0] = eventName;
                Array.Copy(data, 0, content, 1, data.Length);

                PhotonNetwork.RaiseEvent(
                        MysteryTagEvents.EventCode,
                        content,
                        new RaiseEventOptions { Receivers = ReceiverGroup.All, },
                        SendOptions.SendReliable);
            }

            public static void Kill(VRRig rig, bool includeAlreadyDead = false)
            {
                if (rig?.Creator == null || !includeAlreadyDead && !MysteryTagState.IsAlive(rig.Creator.ActorNumber))
                    return;

                Vector3 position = rig.transform.position;
                Vector3 velocity = GetKillVelocity(position);
                double skeleton = SpawnKillSkeletons.IsEnabled ? 0d : 1d;

                Send(
                        MysteryTagEvents.PlayerDeath,
                        (double)rig.Creator.ActorNumber,
                        (double)position.x,
                        (double)position.y,
                        (double)position.z,
                        skeleton,
                        (double)velocity.x,
                        (double)velocity.y,
                        (double)velocity.z);
            }

            public static void SheriffKill(VRRig rig, bool includeAlreadyDead = false)
            {
                if (rig?.Creator == null || !includeAlreadyDead && !MysteryTagState.IsAlive(rig.Creator.ActorNumber))
                    return;

                Vector3 position = rig.transform.position;
                Vector3 velocity = GetKillVelocity(position);
                double skeleton = SpawnKillSkeletons.IsEnabled ? 0d : 1d;

                Send(
                        MysteryTagEvents.SheriffHit,
                        (double)rig.Creator.ActorNumber,
                        position,
                        skeleton,
                        velocity);
            }

            private static Vector3 GetKillVelocity(Vector3 position)
            {
                Vector3 origin = GorillaTagger.Instance?.bodyCollider != null
                                         ? GorillaTagger.Instance.bodyCollider.transform.position
                                         : position - Vector3.up;

                Vector3 direction = position - origin;
                if (direction.sqrMagnitude < 0.01f)
                    direction = Vector3.up;

                return direction.normalized * KillVelocity + Vector3.up * (KillVelocity * 0.4f);
            }

            public static bool RequireMasterClient()
            {
                if (PhotonNetwork.IsMasterClient)
                    return true;

                NotifiLib.SendNotification(
                        "<color=red>Error</color>",
                        "You must be master client to use this Gorilla Mystery mod."
                       );

                return false;
            }
        }

        public static class MysteryTagEvents
        {
            public const byte EventCode = 180;

            public const string ChangeWeaponState = "changeWeaponState";
            public const string ChooseMurder = "chooseMurder";
            public const string ChooseSheriff = "chooseSheriff";
            public const string PlayerDeath = "PlayerDeath";
            public const string SheriffHit = "SheriffHit";
            public const string SheriffShot = "SheriffShot";
            public const string StartGame = "StartGame";
            public const string EndGame = "endGame";
            public const string PlayerVote = "PlayerVote";
            public const string StartVoting = "StartVoting";
            public const string WhatGameStatus = "WhatGameStatus";
            public const string GameStatus = "GameStatus";
            public const string DropGun = "DropGun";
            public const string HandChanged = "HandChanged";
        }


        public enum MysteryTagRole
        {
            Innocent,
            Sheriff,
            Murderer,
        }

        public static class MysteryTagState
        {
            private static readonly HashSet<int> DeadPlayers = new HashSet<int>();

            private static bool initialized;
            private static string currentRoomName;
            private static bool statusRequested;

            public static int? MurdererActorNumber { get; private set; }
            public static int? SheriffActorNumber { get; private set; }
            public static bool GameActive { get; private set; }
            public static bool VotingActive { get; private set; }

            public static void EnsureInitialized()
            {
                if (initialized)
                    return;

                initialized                                  =  true;
                PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
                PrepareForCurrentRoom();
            }

            public static MysteryTagRole GetRole(int actorNumber)
            {
                PrepareForCurrentRoom();

                if (MurdererActorNumber == actorNumber)
                    return MysteryTagRole.Murderer;

                if (SheriffActorNumber == actorNumber)
                    return MysteryTagRole.Sheriff;

                return MysteryTagRole.Innocent;
            }

            public static bool IsAlive(int actorNumber)
            {
                PrepareForCurrentRoom();

                return !DeadPlayers.Contains(actorNumber);
            }

            public static void PollGameStatus()
            {
                EnsureInitialized();
                PrepareForCurrentRoom();

                if (statusRequested || !PhotonNetwork.InRoom || PhotonNetwork.LocalPlayer == null)
                    return;

                statusRequested = true;
                MysteryTagNetwork.Send(
                        MysteryTagEvents.WhatGameStatus,
                        (double)PhotonNetwork.LocalPlayer.ActorNumber);
            }

            private static void PrepareForCurrentRoom()
            {
                string roomName = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom?.Name : null;

                if (roomName == currentRoomName)
                    return;

                currentRoomName = roomName;
                statusRequested = false;
                ResetRoundState();
                VotingActive = false;
            }

            private static void OnEventReceived(EventData eventData)
            {
                if (eventData.Code != MysteryTagEvents.EventCode || !(eventData.CustomData is object[] content) ||
     content.Length == 0 || !(content[0] is string eventName))
                    return;

                PrepareForCurrentRoom();

                switch (eventName)
                {
                    case MysteryTagEvents.StartGame when IsMasterSender(eventData.Sender):
                        ResetRoundState();
                        GameActive   = true;
                        VotingActive = false;

                        break;

                    case MysteryTagEvents.EndGame when IsMasterSender(eventData.Sender):
                        ResetRoundState();
                        VotingActive = false;

                        break;

                    case MysteryTagEvents.ChooseMurder when IsMasterSender(eventData.Sender) &&
                                                            TryGetInt(content, 1, out int murderer):
                        MurdererActorNumber = murderer;
                        DeadPlayers.Remove(murderer);

                        break;

                    case MysteryTagEvents.ChooseSheriff when TryGetInt(content, 1, out int sheriff) &&
                                                             IsAuthorizedSheriffChoice(eventData.Sender, sheriff):
                        SheriffActorNumber = sheriff;
                        DeadPlayers.Remove(sheriff);

                        break;

                    case MysteryTagEvents.ChangeWeaponState when TryGetInt(content, 1, out int weaponType):
                        InferRoleFromWeaponEvent(eventData.Sender, weaponType);

                        break;

                    case MysteryTagEvents.PlayerDeath when TryGetInt(content, 1, out int knifeVictim):
                    case MysteryTagEvents.SheriffHit when TryGetInt(content, 1, out knifeVictim):
                        DeadPlayers.Add(knifeVictim);
                        if (SheriffActorNumber == knifeVictim)
                            SheriffActorNumber = null;

                        break;

                    case MysteryTagEvents.DropGun:
                        SheriffActorNumber = null;

                        break;

                    case MysteryTagEvents.SheriffShot:
                        if (SheriffActorNumber == null)
                            SheriffActorNumber = eventData.Sender;

                        break;

                    case MysteryTagEvents.PlayerVote:
                    case MysteryTagEvents.StartVoting:
                        if (!GameActive)
                            VotingActive = true;

                        break;

                    case MysteryTagEvents.GameStatus when IsMasterSender(eventData.Sender):
                        ApplyGameStatus(content);

                        break;
                }
            }

            private static void ApplyGameStatus(object[] content)
            {
                if (!TryGetInt(content, 1, out int gameStatus) || !TryGetInt(content, 2, out int votingStatus))
                    return;

                if (content.Length            > 4     && TryGetInt(content, 4, out int target) &&
                    PhotonNetwork.LocalPlayer != null && target != PhotonNetwork.LocalPlayer.ActorNumber)
                    return;

                GameActive   = gameStatus == 1;
                VotingActive = !GameActive && votingStatus == 1;
            }

            private static bool IsMasterSender(int sender) =>
                    PhotonNetwork.MasterClient == null || PhotonNetwork.MasterClient.ActorNumber == sender;

            private static bool IsAuthorizedSheriffChoice(int sender, int sheriff) =>
                    IsMasterSender(sender) || SheriffActorNumber == null && sender == sheriff;

            private static void InferRoleFromWeaponEvent(int sender, int weaponType)
            {
                if (!GameActive)
                    return;

                if (weaponType == 1 && MurdererActorNumber == null)
                    MurdererActorNumber = sender;
                else if (weaponType == 2 && SheriffActorNumber == null)
                    SheriffActorNumber = sender;
            }

            private static bool TryGetInt(object[] content, int index, out int value)
            {
                value = 0;

                if (index < 0 || index >= content.Length || content[index] == null)
                    return false;

                try
                {
                    value = Convert.ToInt32(content[index]);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            private static void ResetRoundState()
            {
                MurdererActorNumber = null;
                SheriffActorNumber  = null;
                GameActive          = false;
                DeadPlayers.Clear();
            }
        }


        public static class SpawnKillSkeletons
        {
            public static bool IsEnabled { get; set; } = false;
        }

        private static readonly Dictionary<VRRig, GameObject> _murdererBoxes = new Dictionary<VRRig, GameObject>();
        private static readonly Dictionary<VRRig, GameObject> _sheriffBoxes = new Dictionary<VRRig, GameObject>();
        private static readonly Dictionary<VRRig, GameObject> _innocentBoxes = new Dictionary<VRRig, GameObject>();
        private static readonly Vector3 BoxScale = new Vector3(0.31f, 0.41f, 0.31f);

        public static void UpdateMurdererEsp() => UpdateEsp(_murdererBoxes, MysteryTagRole.Murderer, Color.red, "LagMenuMysteryMurdererESP");
        public static void UpdateSheriffEsp() => UpdateEsp(_sheriffBoxes, MysteryTagRole.Sheriff, Color.blue, "LagMenuMysterySheriffESP");
        public static void UpdateInnocentEsp() => UpdateEsp(_innocentBoxes, MysteryTagRole.Innocent, Color.green, "LagMenuMysteryInnocentESP");

        public static void DisableMurdererEsp() => DestroyBoxes(_murdererBoxes);
        public static void DisableSheriffEsp() => DestroyBoxes(_sheriffBoxes);
        public static void DisableInnocentEsp() => DestroyBoxes(_innocentBoxes);

        private static void UpdateEsp(Dictionary<VRRig, GameObject> boxes, MysteryTagRole role, Color color, string boxName)
        {
            MysteryTagState.EnsureInitialized();
            MysteryTagState.PollGameStatus();

            foreach (VRRig rig in RigManager.GetAllRigs())
            {
                if (rig == null || rig.isLocal || rig.Creator == null) continue;

                if (!boxes.TryGetValue(rig, out GameObject box) || box == null)
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.name = boxName;
                    box.transform.SetParent(rig.transform);
                    box.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                    box.transform.localRotation = Quaternion.identity;
                    box.transform.localScale    = Vector3.Scale(BoxScale, rig.transform.localScale);
                    Object.Destroy(box.GetComponent<Collider>());

                    Renderer r = box.GetComponent<Renderer>();
                    r.material.shader = Shader.Find("GUI/Text Shader");
                    boxes[rig] = box;
                }

                int actorNumber = rig.Creator.ActorNumber;
                bool shouldShow = MysteryTagState.GameActive &&
                                  MysteryTagState.IsAlive(actorNumber) &&
                                  VStump.MysteryTagState.GetRole(actorNumber) == role;

                Renderer rend = box.GetComponent<Renderer>();
                rend.enabled       = shouldShow;
                rend.material.color = new Color(color.r, color.g, color.b, 0.4f);
                box.transform.rotation   = rig.transform.rotation;
                box.transform.localScale = Vector3.Scale(BoxScale, rig.transform.localScale);
            }


            var toRemove = new List<VRRig>();
            foreach (var kv in boxes)
                if (kv.Key == null || kv.Value == null) toRemove.Add(kv.Key);
            foreach (var k in toRemove)
            {
                if (boxes[k] != null) Object.Destroy(boxes[k]);
                boxes.Remove(k);
            }
        }

        private static void DestroyBoxes(Dictionary<VRRig, GameObject> boxes)
        {
            foreach (var kv in boxes)
                if (kv.Value != null) Object.Destroy(kv.Value);
            boxes.Clear();
        }

        public static class MecchaEvents
        {
            public const byte Code = 180;
            public const string Claim = "pgClaim";
            public const string Unclaim = "pgUnclaim";
            public const string SelectMap = "pgSelectMap";
            public const string StartRound = "pgStartRound";
            public const string Mini = "pgMini";
            public const string Shotgun = "pgShotgun";
            public const string HeldTool = "pgHeldTool";
            public const string Shot = "pgShot";
            public const string Kill = "pgKill";
            public const string Respawn = "pgRespawn";
            public const string Whistle = "pgWhistle";
            public const string RoundState = "pgRoundState";
            public const string Settings = "pgSettings";
            public const string Leave = "pgLeave";
            public const string Spectate = "pgSpec";
            public const string Dot = "pgDot";
            public const string ClearDots = "pgClearDots";
            public const string BucketColor = "pgBucketColor";
            public const string BucketPart = "pgBucketPart";
            public const string ColorCode = "pgColorCode";
            public const string Role = "pgRole";
            public const string Seekers = "pgSeekers";
        }

        public static class MecchaNetwork
        {
            public static int LocalId => PhotonNetwork.LocalPlayer?.ActorNumber ?? 0;

            public static void Send(string eventName, params object[] data)
            {
                if (!PhotonNetwork.InRoom) return;
                object[] content = new object[data.Length + 1];
                content[0] = eventName;
                Array.Copy(data, 0, content, 1, data.Length);
                PhotonNetwork.RaiseEvent(MecchaEvents.Code, content,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }

            public static void Kill(int victimId) => Send(MecchaEvents.Kill, (double)victimId, (double)LocalId);
            public static void Respawn(int victimId) => Send(MecchaEvents.Respawn, (double)victimId);
            public static void Whistle(int playerId) => Send(MecchaEvents.Whistle, (double)playerId);

            public static void SetRole(int playerId, bool seeker) =>
                Send(MecchaEvents.Role, (double)playerId, seeker ? 1d : 0d);

            public static void PaintMini(int playerId, Color color) =>
                Send(MecchaEvents.BucketColor, (double)color.r, (double)color.g, (double)color.b, (double)playerId);

            public static void PaintPart(int playerId, int part, Color color) =>
                Send(MecchaEvents.BucketPart, (double)part, (double)color.r, (double)color.g, (double)color.b, (double)playerId);

            public static void ColorCode(int playerId, Color color) =>
                Send(MecchaEvents.ColorCode, (double)playerId,
                    (double)(color.r * 255f), (double)(color.g * 255f), (double)(color.b * 255f));

            public static void Shot(Vector3 from, Vector3 to, bool explosion, Color color) =>
                Send(MecchaEvents.Shot,
                    (double)from.x, (double)from.y, (double)from.z,
                    (double)to.x, (double)to.y, (double)to.z,
                    explosion ? 1d : 0d,
                    (double)color.r, (double)color.g, (double)color.b);

            public static void Dot(int playerId, Vector3 offset, float size, Color color) =>
                Send(MecchaEvents.Dot, (double)playerId,
                    (double)offset.x, (double)offset.y, (double)offset.z,
                    (double)size, (double)color.r, (double)color.g, (double)color.b);

            public static void HeldTool(int playerId, int toolType, Transform transform)
            {
                if (transform == null) return;
                Vector3 p = transform.position;
                Quaternion q = transform.rotation;
                Send(MecchaEvents.HeldTool, (double)playerId, (double)toolType,
                    (double)p.x, (double)p.y, (double)p.z,
                    (double)q.x, (double)q.y, (double)q.z, (double)q.w);
            }

            public static void SetRoundState(int state, float duration, int map, int reason = 0) =>
                Send(MecchaEvents.RoundState, (double)state, (double)duration, (double)map, (double)reason);

            public static void Settings(int seekers, int hideTime, int matchTime, int checkTime, int whistleTime, bool testing) =>
                Send(MecchaEvents.Settings,
                    (double)seekers, (double)hideTime, (double)matchTime,
                    (double)checkTime, (double)whistleTime, 1d, testing ? 1d : 0d);

            public static Color Rainbow(float speed = 0.2f) =>
                Color.HSVToRGB(Mathf.Repeat(Time.time * speed, 1f), 1f, 1f);
        }

        public enum MecchaRole { Hider, Seeker }

        public static class MecchaState
        {
            private static readonly Dictionary<int, MecchaRole> Roles = new Dictionary<int, MecchaRole>();
            private static readonly HashSet<int> Dead = new HashSet<int>();
            private static bool initialized;
            private static string roomName;

            public static int RoundState { get; private set; } = 1;
            public static int RoundMap { get; private set; }

            public static void EnsureInitialized()
            {
                if (initialized) return;
                initialized = true;
                PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
                ResetForRoom();
            }

            public static MecchaRole GetRole(int actorNumber)
            {
                PrepareRoom();
                return Roles.TryGetValue(actorNumber, out MecchaRole role) ? role : MecchaRole.Hider;
            }

            public static bool IsAlive(int actorNumber) { PrepareRoom(); return !Dead.Contains(actorNumber); }

            private static void PrepareRoom()
            {
                string current = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom?.Name : null;
                if (current == roomName) return;
                ResetForRoom();
            }

            private static void ResetForRoom()
            {
                roomName   = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom?.Name : null;
                Roles.Clear(); Dead.Clear();
                RoundState = 1; RoundMap = 0;
            }

            private static void OnEvent(EventData eventData)
            {
                if (eventData.Code != MecchaEvents.Code ||
                    !(eventData.CustomData is object[] content) ||
                    content.Length == 0 || !(content[0] is string name))
                    return;

                PrepareRoom();

                int victim;
                switch (name)
                {
                    case MecchaEvents.Role when Int(content, 1, out int player):
                        Roles[player] = Int(content, 2, out int flag) && flag == 1 ? MecchaRole.Seeker : MecchaRole.Hider;
                        break;

                    case MecchaEvents.Seekers:
                        Roles.Clear();
                        for (int i = 1; i < content.Length; i++)
                            if (Int(content, i, out int seeker) && seeker > 0)
                                Roles[seeker] = MecchaRole.Seeker;
                        break;

                    case MecchaEvents.Kill when Int(content, 1, out victim):
                    case MecchaEvents.Leave when Int(content, 1, out victim):
                        Dead.Add(victim);
                        break;

                    case MecchaEvents.Respawn when Int(content, 1, out int respawned):
                        Dead.Remove(respawned);
                        break;

                    case MecchaEvents.StartRound when Int(content, 1, out int startedMap):
                        RoundState = 2; RoundMap = startedMap; Dead.Clear();
                        break;

                    case MecchaEvents.RoundState when Int(content, 1, out int state):
                        RoundState = state;
                        if (Int(content, 3, out int map)) RoundMap = map;
                        if (state == 1 || state == 2) Dead.Clear();
                        break;
                }
            }

            private static bool Int(object[] content, int index, out int value)
            {
                value = 0;
                if (index < 0 || index >= content.Length || content[index] == null) return false;
                try { value = Convert.ToInt32(content[index]); return true; }
                catch { return false; }
            }
        }



        private static float _mecchaKillDelay;
        private static float _mecchaRespawnDelay;
        private static float _mecchaWhistleDelay;
        private static float _mecchaSplatterDelay;
        private static float _mecchaAlwaysAliveDelay;

        public static void MecchaKillGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaKillDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaKillDelay = Time.time + 0.3f;
                MecchaNetwork.Kill(rig.Creator.ActorNumber);
            }, rightHand: true);
        }

        public static void MecchaKillShotGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaKillDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaKillDelay = Time.time + 0.3f;
                Vector3 from = GorillaTagger.Instance?.bodyCollider != null
                    ? GorillaTagger.Instance.bodyCollider.transform.position
                    : rig.transform.position + Vector3.up;
                MecchaNetwork.Shot(from, rig.transform.position, true, MecchaNetwork.Rainbow());
                MecchaNetwork.Kill(rig.Creator.ActorNumber);
            }, rightHand: true);
        }

        public static void MecchaRespawnGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaRespawnDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaRespawnDelay = Time.time + 0.3f;
                MecchaNetwork.Respawn(rig.Creator.ActorNumber);
            }, rightHand: true);
        }

        public static void MecchaWhistleGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaWhistleDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaWhistleDelay = Time.time + 0.3f;
                MecchaNetwork.Whistle(rig.Creator.ActorNumber);
            }, rightHand: true);
        }

        public static void MecchaSplatterGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaSplatterDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaSplatterDelay = Time.time + 0.2f;
                Transform hand = GTPlayer.Instance.RightHand.controllerTransform;
                Vector3 from = hand != null ? hand.position : rig.transform.position + Vector3.up;
                MecchaNetwork.Shot(from, rig.transform.position, false, MecchaNetwork.Rainbow(0.35f));
            }, rightHand: true);
        }

        public static void MecchaExplosionGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (Time.time < _mecchaSplatterDelay) return;
                VRRig rig = GunLib.GetTargetRig();
                if (rig == null || rig.isLocal || rig.Creator == null) return;
                _mecchaSplatterDelay = Time.time + 0.3f;
                Transform hand = GTPlayer.Instance.RightHand.controllerTransform;
                Vector3 from = hand != null ? hand.position : rig.transform.position + Vector3.up;
                MecchaNetwork.Shot(from, rig.transform.position, true, MecchaNetwork.Rainbow(0.35f));
            }, rightHand: true);
        }

        public static void MecchaKillAll()
        {
            foreach (VRRig rig in RigManager.GetAllRigs())
                if (rig?.Creator != null) MecchaNetwork.Kill(rig.Creator.ActorNumber);
        }

        public static void MecchaKillHiders()
        {
            MecchaState.EnsureInitialized();
            foreach (VRRig rig in RigManager.GetAllRigs())
                if (rig?.Creator != null && MecchaState.GetRole(rig.Creator.ActorNumber) == MecchaRole.Hider)
                    MecchaNetwork.Kill(rig.Creator.ActorNumber);
        }

        public static void MecchaKillSeekers()
        {
            MecchaState.EnsureInitialized();
            foreach (VRRig rig in RigManager.GetAllRigs())
                if (rig?.Creator != null && MecchaState.GetRole(rig.Creator.ActorNumber) == MecchaRole.Seeker)
                    MecchaNetwork.Kill(rig.Creator.ActorNumber);
        }

        public static void MecchaRespawnAll()
        {
            MecchaNetwork.Respawn(MecchaNetwork.LocalId);
            foreach (VRRig rig in RigManager.GetAllRigs())
                if (rig?.Creator != null) MecchaNetwork.Respawn(rig.Creator.ActorNumber);
        }

        public static void MecchaAlwaysAlive()
        {
            if (Time.time < _mecchaAlwaysAliveDelay) return;
            _mecchaAlwaysAliveDelay = Time.time + 0.5f;
            MecchaNetwork.Respawn(MecchaNetwork.LocalId);
        }

        public static void MecchaWhistleAll()
        {
            MecchaNetwork.Whistle(MecchaNetwork.LocalId);
            foreach (VRRig rig in RigManager.GetAllRigs())
                if (rig?.Creator != null) MecchaNetwork.Whistle(rig.Creator.ActorNumber);
        }

        private static float _mecchaWhistleSpamDelay;
        public static void MecchaWhistleSpam()
        {
            if (Time.time < _mecchaWhistleSpamDelay) return;
            _mecchaWhistleSpamDelay = Time.time + 0.3f;
            MecchaNetwork.Whistle(MecchaNetwork.LocalId);
        }

        public static void ToggleMysteryKnife() =>
    MysteryTagNetwork.Send(MysteryTagEvents.ChangeWeaponState, 1d);

        public static void ToggleMysteryPistol() =>
            MysteryTagNetwork.Send(MysteryTagEvents.ChangeWeaponState, 2d);

        public static void MysteryWeaponRightHand()
        {
            if (PhotonNetwork.LocalPlayer != null)
                MysteryTagNetwork.Send(MysteryTagEvents.HandChanged,
                    (double)PhotonNetwork.LocalPlayer.ActorNumber, 0d);
        }

        public static void MysteryWeaponLeftHand()
        {
            if (PhotonNetwork.LocalPlayer != null)
                MysteryTagNetwork.Send(MysteryTagEvents.HandChanged,
                    (double)PhotonNetwork.LocalPlayer.ActorNumber, 1d);
        }

        public static void BecomeSheriff()
        {
            MysteryTagState.EnsureInitialized();
            if (PhotonNetwork.LocalPlayer != null)
                MysteryTagNetwork.Send(MysteryTagEvents.ChooseSheriff,
                    (double)PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public static void BecomeMurderer()
        {
            MysteryTagState.EnsureInitialized();
            if (!MysteryTagNetwork.RequireMasterClient() || PhotonNetwork.LocalPlayer == null) return;
            MysteryTagNetwork.Send(MysteryTagEvents.ChooseMurder,
                (double)PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public static void StartMysteryMap(int map)
        {
            if (MysteryTagNetwork.RequireMasterClient())
                MysteryTagNetwork.Send(MysteryTagEvents.StartGame, (double)map);
        }

        public static void ForceMurdererWin()
        {
            if (MysteryTagNetwork.RequireMasterClient())
                MysteryTagNetwork.Send(MysteryTagEvents.EndGame, 1d);
        }

        public static void ForceInnocentsWin()
        {
            if (MysteryTagNetwork.RequireMasterClient())
                MysteryTagNetwork.Send(MysteryTagEvents.EndGame, 2d);
        }

        private static float _spamRoundsDelay;
        private static bool _spamRoundsSendStart = true;
        private static int _spamRoundsMap = 1;
        private static int _spamRoundsWinner = 1;
        private static bool _wasShooting;

        public static void SpamMysteryRounds()
        {
            if (!PhotonNetwork.IsMasterClient) { MysteryTagNetwork.RequireMasterClient(); return; }
            if (Time.time < _spamRoundsDelay) return;
            _spamRoundsDelay = Time.time + 0.3f;

            if (_spamRoundsSendStart)
            {
                MysteryTagNetwork.Send(MysteryTagEvents.StartGame, (double)_spamRoundsMap);
                _spamRoundsMap = _spamRoundsMap % 3 + 1;
            }
            else
            {
                MysteryTagNetwork.Send(MysteryTagEvents.EndGame, (double)_spamRoundsWinner);
                _spamRoundsWinner = _spamRoundsWinner == 1 ? 2 : 1;
            }
            _spamRoundsSendStart = !_spamRoundsSendStart;
        }

    }
}

