
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace LagMenu.Utilities
{
    public static class Tracker
    {
        private const string TrackerUrl =
            "https://tracker.lagmenu.lol/";

        private static readonly HttpClient Client =
            new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

        private static bool _wasInRoom;
        private static string _lastRoom = "";

        private static readonly HashSet<string> TrackedPlayers =
            new HashSet<string>();

        private static readonly HashSet<string> PendingPlayers =
            new HashSet<string>();

        private static string PlayerName =>
            PhotonNetwork.LocalPlayer?.NickName ?? "Unknown";

        private static string UserId =>
            PhotonNetwork.LocalPlayer?.UserId ?? "Unknown";

        public static void Update()
        {
            try
            {
                bool inRoom = PhotonNetwork.InRoom;

                if (!_wasInRoom && inRoom)
                {
                    HandleRoomJoined();
                }

                if (_wasInRoom && !inRoom)
                {
                    HandleRoomLeft();
                }

                if (inRoom)
                {
                    CheckPlayers();
                }

                _wasInRoom = inRoom;
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "[LagMenu Tracker] Update error: " + e
                );
            }
        }

        private static void HandleRoomJoined()
        {
            if (PhotonNetwork.CurrentRoom == null)
                return;

            _lastRoom =
                PhotonNetwork.CurrentRoom.Name ?? "";

            TrackedPlayers.Clear();
            PendingPlayers.Clear();

            SendEvent(
                "room_join",
                BuildRoomData()
            );
        }

        private static void HandleRoomLeft()
        {
            SendEvent(
                "room_leave",
                new Dictionary<string, object>
                {
                    {
                        "player",
                        BuildLocalPlayerData()
                    },
                    {
                        "roomCode",
                        _lastRoom
                    }
                }
            );

            _lastRoom = "";

            TrackedPlayers.Clear();
            PendingPlayers.Clear();
        }

        private static void CheckPlayers()
        {
            if (PhotonNetwork.CurrentRoom == null)
                return;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player == null)
                    continue;

                if (player.IsLocal)
                    continue;

                string userId =
                    player.UserId ?? "";

                if (string.IsNullOrEmpty(userId))
                    continue;

                if (TrackedPlayers.Contains(userId))
                    continue;

                if (PendingPlayers.Contains(userId))
                    continue;

                PendingPlayers.Add(userId);

                TryTrackPlayer(player);
            }
        }

        private static void TryTrackPlayer(Player player)
        {
            try
            {
                VRRig rig = FindRig(player);

                if (rig == null)
                {
                    PendingPlayers.Remove(
                        player.UserId
                    );

                    return;
                }

                TrackPlayerCoroutine(
                    rig,
                    player
                );
            }
            catch (Exception e)
            {
                PendingPlayers.Remove(
                    player.UserId
                );

                Debug.LogError(
                    "[LagMenu Tracker] Player tracking error: " +
                    e
                );
            }
        }

        private static VRRig FindRig(Player player)
        {
            if (player == null)
                return null;

            try
            {
                if (VRRigCache.m_activeRigs == null)
                    return null;

                return VRRigCache.m_activeRigs.Find(
                    rig =>
                        rig != null &&
                        rig.Creator != null &&
                        rig.Creator.ActorNumber ==
                        player.ActorNumber
                );
            }
            catch
            {
                return null;
            }
        }

        private static void TrackPlayerCoroutine(
            VRRig rig,
            Player player)
        {
            TrackerBehaviour behaviour =
                TrackerBehaviour.Instance;

            if (behaviour == null)
                return;

            behaviour.StartCoroutine(
                TrackPlayerRoutine(
                    rig,
                    player
                )
            );
        }

        private static IEnumerator TrackPlayerRoutine(
            VRRig rig,
            Player player)
        {
            if (rig == null || player == null)
            {
                if (player != null)
                    PendingPlayers.Remove(
                        player.UserId
                    );

                yield break;
            }

            Task<string> creationDateTask =
                GetCreationDate(
                    player.UserId
                );

            while (
                creationDateTask != null &&
                !creationDateTask.IsCompleted)
            {
                yield return null;
            }

            string creationDate = null;

            if (creationDateTask != null)
            {
                if (
                    creationDateTask.Status ==
                    TaskStatus.RanToCompletion
                )
                {
                    creationDate =
                        creationDateTask.Result;
                }
                else
                {
                    creationDate = "ERROR";
                }
            }

            Dictionary<string, object> data =
                BuildPlayerData(
                    player,
                    rig,
                    creationDate
                );

            SendEvent(
                "player_discovered",
                data
            );

            string userId =
                player.UserId ?? "";

            if (!string.IsNullOrEmpty(userId))
            {
                TrackedPlayers.Add(userId);
                PendingPlayers.Remove(userId);
            }
        }

        private static Task<string> GetCreationDate(
            string userId)
        {
            TaskCompletionSource<string> source =
                new TaskCompletionSource<string>();

            if (string.IsNullOrEmpty(userId))
            {
                source.SetResult("ERROR");
                return source.Task;
            }

            try
            {
                PlayFabClientAPI.GetAccountInfo(
                    new GetAccountInfoRequest
                    {
                        PlayFabId = userId
                    },
                    result =>
                    {
                        try
                        {
                            if (
                                result == null ||
                                result.AccountInfo == null
                            )
                            {
                                source.SetResult(
                                    "ERROR"
                                );

                                return;
                            }

                            source.SetResult(
                                result.AccountInfo.Created
                                    .ToString(
                                        "MMM dd, yyyy"
                                    )
                                    .ToUpper()
                            );
                        }
                        catch
                        {
                            source.SetResult(
                                "ERROR"
                            );
                        }
                    },
                    error =>
                    {
                        source.SetResult(
                            "ERROR"
                        );
                    }
                );
            }
            catch
            {
                source.SetResult(
                    "ERROR"
                );
            }

            return source.Task;
        }

        public static void SendRoomInfo()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (PhotonNetwork.CurrentRoom == null)
                return;

            SendEvent(
                "room_snapshot",
                BuildRoomData()
            );
        }

        private static Dictionary<string, object> BuildRoomData()
        {
            Dictionary<string, object> data =
                new Dictionary<string, object>();

            Room room =
                PhotonNetwork.CurrentRoom;

            data["roomCode"] =
                room?.Name ?? "Unknown";

            data["region"] =
                PhotonNetwork.CloudRegion ?? "Unknown";

            data["playersCount"] =
                PhotonNetwork.PlayerList?.Length ?? 0;

            data["maxPlayers"] =
                room?.MaxPlayers ?? 0;

            data["masterClient"] =
                PhotonNetwork.MasterClient?.NickName ??
                "Unknown";

            data["masterUserId"] =
                PhotonNetwork.MasterClient?.UserId ??
                "Unknown";

            data["masterActorNumber"] =
                PhotonNetwork.MasterClient?.ActorNumber ??
                -1;

            data["gameMode"] =
                GetGameMode();

            data["localPlayer"] =
                BuildLocalPlayerData();

            List<Dictionary<string, object>> players =
                new List<Dictionary<string, object>>();

            if (PhotonNetwork.PlayerList != null)
            {
                foreach (
                    Player player
                    in PhotonNetwork.PlayerList)
                {
                    if (player == null)
                        continue;

                    VRRig rig =
                        FindRig(player);

                    players.Add(
                        BuildPlayerData(
                            player,
                            rig,
                            null
                        )
                    );
                }
            }

            data["players"] =
                players;

            data["roomProperties"] =
                ConvertHashtable(
                    room?.CustomProperties
                );

            data["trackedTime"] =
                DateTimeOffset.UtcNow
                    .ToUnixTimeMilliseconds();

            return data;
        }

        private static Dictionary<string, object>
            BuildLocalPlayerData()
        {
            Player player =
                PhotonNetwork.LocalPlayer;

            if (player == null)
            {
                return new Dictionary<string, object>
                {
                    {
                        "userId",
                        "Unknown"
                    },
                    {
                        "userName",
                        "Unknown"
                    },
                    {
                        "actorNumber",
                        -1
                    }
                };
            }

            VRRig rig =
                FindRig(player);

            return BuildPlayerData(
                player,
                rig,
                null
            );
        }

        private static Dictionary<string, object>
            BuildPlayerData(
                Player player,
                VRRig rig,
                string creationDate)
        {
            Dictionary<string, object> data =
                new Dictionary<string, object>();

            if (player == null)
                return data;

            data["userId"] =
                player.UserId ?? "Unknown";

            data["userName"] =
                player.NickName ?? "Unknown";

            data["actorNumber"] =
                player.ActorNumber;

            data["isLocal"] =
                player.IsLocal;

            data["isMaster"] =
                player.IsMasterClient;

            data["customProperties"] =
                ConvertHashtable(
                    player.CustomProperties
                );

            if (!string.IsNullOrEmpty(creationDate))
            {
                data["userCreationDate"] =
                    creationDate;
            }

            if (rig != null)
            {
                data["isRigLoaded"] =
                    true;

                data["isSteam"] =
                    IsSteamPlayer(rig);

                data["rawCosmeticString"] =
                    GetCosmetics(rig);
            }
            else
            {
                data["isRigLoaded"] =
                    false;

                data["isSteam"] =
                    false;

                data["rawCosmeticString"] =
                    "";
            }

            if (PhotonNetwork.CurrentRoom != null)
            {
                data["roomCode"] =
                    PhotonNetwork.CurrentRoom.Name ??
                    "Unknown";

                data["playersInCode"] =
                    PhotonNetwork.PlayerList.Length;
            }

            data["region"] =
                PhotonNetwork.CloudRegion ??
                "Unknown";

            data["gameMode"] =
                GetGameMode();

            data["trackedTime"] =
                DateTimeOffset.UtcNow
                    .ToUnixTimeMilliseconds();

            return data;
        }

        private static string GetCosmetics(VRRig rig)
        {
            try
            {
                if (rig == null)
                    return "";

                if (rig._playerOwnedCosmetics == null)
                    return "";

                return rig
                    ._playerOwnedCosmetics
                    .Concat();
            }
            catch
            {
                return "";
            }
        }

        private static bool IsSteamPlayer(VRRig rig)
        {
            try
            {
                if (rig == null ||
                    rig.Creator == null)
                {
                    return false;
                }

                string cosmetics =
                    GetCosmetics(rig);

                Player player =
                    PhotonNetwork.PlayerList.FirstOrDefault(
                        p =>
                            p != null &&
                            p.ActorNumber ==
                            rig.Creator.ActorNumber
                    );

                int propertyCount =
                    player?.CustomProperties?.Count ?? 0;

                return
                    cosmetics.Contains(
                        "S. FIRST LOGIN"
                    ) ||
                    cosmetics.Contains(
                        "FIRST LOGIN"
                    ) ||
                    propertyCount >= 2;
            }
            catch
            {
                return false;
            }
        }

        private static string GetGameMode()
        {
            try
            {
                Type networkSystemType =
                    Type.GetType(
                        "NetworkSystem, Assembly-CSharp"
                    );

                if (networkSystemType != null)
                {
                    object instance =
                        networkSystemType
                            .GetProperty(
                                "Instance"
                            )
                            ?.GetValue(null);

                    if (instance != null)
                    {
                        object value =
                            networkSystemType
                                .GetProperty(
                                    "GameModeString"
                                )
                                ?.GetValue(instance);

                        if (value != null)
                            return value.ToString();
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (
                    PhotonNetwork.CurrentRoom != null &&
                    PhotonNetwork.CurrentRoom
                        .CustomProperties != null
                )
                {
                    Hashtable properties =
                        PhotonNetwork.CurrentRoom
                            .CustomProperties;

                    foreach (
                        object key
                        in properties.Keys)
                    {
                        string keyString =
                            key?.ToString() ?? "";

                        if (
                            keyString.IndexOf(
                                "game",
                                StringComparison
                                    .OrdinalIgnoreCase
                            ) >= 0
                        )
                        {
                            object value =
                                properties[key];

                            if (value != null)
                                return value.ToString();
                        }
                    }
                }
            }
            catch
            {
            }

            return "Unknown";
        }

        private static Dictionary<string, object>
            ConvertHashtable(
                Hashtable properties)
        {
            Dictionary<string, object> result =
                new Dictionary<string, object>();

            if (properties == null)
                return result;

            foreach (
                object key
                in properties.Keys)
            {
                if (key == null)
                    continue;

                string keyString =
                    key.ToString();

                result[keyString] =
                    ConvertPropertyValue(
                        properties[key]
                    );
            }

            return result;
        }

        private static object ConvertPropertyValue(
            object value)
        {
            if (value == null)
                return null;

            if (
                value is string ||
                value is bool ||
                value is byte ||
                value is sbyte ||
                value is short ||
                value is ushort ||
                value is int ||
                value is uint ||
                value is long ||
                value is ulong ||
                value is float ||
                value is double ||
                value is decimal
            )
            {
                return value;
            }

            if (value is Array array)
            {
                List<object> result =
                    new List<object>();

                foreach (object item in array)
                {
                    result.Add(
                        ConvertPropertyValue(
                            item
                        )
                    );
                }

                return result;
            }

            return value.ToString();
        }

        private static async void SendEvent(
            string eventType,
            Dictionary<string, object> data)
        {
            try
            {
                Dictionary<string, object> payload =
                    new Dictionary<string, object>();

                payload["event"] =
                    eventType;

                payload["timestamp"] =
                    DateTimeOffset.UtcNow
                        .ToUnixTimeMilliseconds();

                payload["player"] =
                    PlayerName;

                payload["userId"] =
                    UserId;

                payload["data"] =
                    data;

                string json =
                    JsonConvert.SerializeObject(
                        payload
                    );

                Debug.Log(
                    "[LagMenu Tracker] Sending " +
                    eventType
                );

                using (
                    StringContent content =
                        new StringContent(
                            json,
                            Encoding.UTF8,
                            "application/json"
                        )
                )
                {
                    HttpResponseMessage response =
                        await Client.PostAsync(
                            TrackerUrl,
                            content
                        );

                    if (!response.IsSuccessStatusCode)
                    {
                        string responseText =
                            await response.Content
                                .ReadAsStringAsync();

                        Debug.LogError(
                            "[LagMenu Tracker] HTTP " +
                            $"{(int)response.StatusCode}: " +
                            responseText
                        );

                        return;
                    }

                    Debug.Log(
                        "[LagMenu Tracker] " +
                        eventType +
                        " sent successfully"
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "[LagMenu Tracker] HTTP error: " +
                    e
                );
            }
        }

        private class TrackerBehaviour : MonoBehaviour
        {
            private static TrackerBehaviour _instance;

            public static TrackerBehaviour Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        GameObject obj =
                            new GameObject(
                                "LagMenu Tracker"
                            );

                        UnityEngine.Object.DontDestroyOnLoad(
                            obj
                        );

                        _instance =
                            obj.AddComponent<
                                TrackerBehaviour
                            >();
                    }

                    return _instance;
                }
            }
        }
    }
}
