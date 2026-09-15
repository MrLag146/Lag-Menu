/*
 * Seralyth Menu  Classes/Menu/ServerData.cs
 * A community driven mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Seralyth Software
 * https://github.com/Seralyth/Seralyth-Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static LagMenu.Utilities.VRRigExtensions;
using GorillaNetworking;
using LagMenu;
using LagMenu.Menu;
using LagMenu.Mods;
using LagMenu.Notifications;
using LagMenu.Utilities;
using MonoMod.Utils;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace LagMenu.Menu
{
    public class ServerData : MonoBehaviour
    {
        #region Configuration
        public static readonly bool ServerDataEnabled = true;
        public static bool DisableTelemetry = false;

        public const string ServerEndpoint = "https://lagmenu.lol/Console";
        public static readonly string ServerDataEndpoint = $"{ServerEndpoint}/serverdata.json";
        public static readonly string ServerWebsocket = "wss://menu.seralyth.software";

        public const string AssetURL = "https://raw.githubusercontent.com/Seralyth/Console/refs/heads/master/ServerData";



        public static readonly Dictionary<string, string> LocalAdmins = new Dictionary<string, string>()
        {

        };


        public static void SetupAdminPanel(string playerName)
        {
            NotifiLib.SendNotification($"[LagMenu] Admin detected: {playerName}");
            LagMenu.Utilities.ResourceManager.PlayAdminSound();
            if (!Mods.Admin.Admins.ContainsKey(Photon.Pun.PhotonNetwork.LocalPlayer.UserId))
            {
                Mods.Admin.Admins.Add(
                    Photon.Pun.PhotonNetwork.LocalPlayer.UserId,
                    playerName
                );
            }
        }








        #endregion

        #region Server Data Code
        private static ServerData instance;

        private static readonly List<string> DetectedModsLabelled = new List<string>();

        private static float DataLoadTime = -1f;
        private static float ReloadTime = -1f;

        private static int LoadAttempts;

        private static bool BetaBuildWarning;
        public static bool OutdatedVersion;

        private static bool GivenAdminMods;
        private static bool GivenPateronMods;

        private static string LastPollAnswered;

        private static string CurrentPoll = "What goes well with cheeseburgers?";
        private static string OptionA = "Fries";
        private static string OptionB = "Chips";

        public void Awake()
        {
            instance = this;
            DataLoadTime = Time.time + 5f;

            NetworkSystem.Instance.OnJoinedRoomEvent += OnJoinRoom;

            NetworkSystem.Instance.OnPlayerJoined += UpdatePlayerCount;
            NetworkSystem.Instance.OnPlayerLeft += UpdatePlayerCount;

        }

        public void Update()
        {
            if (DataLoadTime > 0f && Time.time > DataLoadTime && GorillaComputer.instance.isConnectedToMaster)
            {
                DataLoadTime = Time.time + 5f;

                LoadAttempts++;
                if (LoadAttempts >= 3)
                {
                    Console.Log("Server data could not be loaded");
                    DataLoadTime = -1f;
                    return;
                }

                Console.Log("Attempting to load web data");
                instance.StartCoroutine(RefreshServerData());
            }

            if (ReloadTime > 0f)
            {
                if (Time.time > ReloadTime)
                {
                    ReloadTime = Time.time + 30f;
                    instance.StartCoroutine(RefreshServerData());
                }
            }
            else
            {
                if (GorillaComputer.instance.isConnectedToMaster)
                    ReloadTime = Time.time + 5f;
            }

            if (!(Time.time > DataSyncDelay) && NetworkSystem.Instance.InRoom) return;
            if (NetworkSystem.Instance.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
            {
                instance.StartCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));
                NetworkSystem.Instance.PlayerListOthers.ForEach(p => ShouldWeReport(RigManager.NetPlayerToPlayer(p)));
            }

            PlayerCount = NetworkSystem.Instance.InRoom ? PhotonNetwork.PlayerList.Length : -1;
        }

        private IEnumerator RefreshServerData()
        {
            yield return LoadServerData();
            yield return GetSeralythCCU();
            yield return GetReportData();
        }

        public static void OnJoinRoom()
        {
            instance.StartCoroutine(TelemetryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.CurrentRoom.IsVisible, PhotonNetwork.PlayerList.Length, NetworkSystem.Instance.GameModeString));
            NetworkSystem.Instance.PlayerListOthers.ForEach(p => ShouldWeReport(RigManager.NetPlayerToPlayer(p)));
        }

        public static void ShouldWeReport(Player player)
        {
            if (!reportData.TryGetValue(player.UserId, out var entry))
                return;

            if (Administrators.ContainsKey(player.UserId))
                return;

            lock (entry.reportedIn)
            {
                if (!entry.reportedIn.Add(NetworkSystem.Instance.RoomName))
                    return;

                GorillaPlayerScoreboardLine.ReportPlayer(
                    player.UserId,
                    entry.ButtonType,
                    player.NickName
                );

                if (Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
                {
                    NotifiLib.SendNotification(
                         $"<color=grey>[</color><color=purple>ARS</color><color=grey>]</color> Player {player.NickName} (also known as {entry.KnownAs}) has been reported for {entry.Reason}. Added by {entry.Actor}"

                     );
                }
            }
        }

        public static string CleanString(string input, int maxLength = 12)
        {
            input = new string(Array.FindAll(input.ToCharArray(), Utils.IsASCIILetterOrDigit));

            if (input.Length > maxLength)
                input = input[..(maxLength - 1)];

            input = input.ToUpper();
            return input;
        }

        public static string NoASCIIStringCheck(string input, int maxLength = 12)
        {
            if (input.Length > maxLength)
                input = input[..(maxLength - 1)];

            input = input.ToUpper();
            return input;
        }

        public static int VersionToNumber(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 3)
                return -1;

            return int.Parse(parts[0]) * 100 + int.Parse(parts[1]) * 10 + int.Parse(parts[2]);
        }

        public static readonly Dictionary<string, string> Administrators = new Dictionary<string, string>();
        public static readonly List<string> SuperAdministrators = new List<string>();
        public static IEnumerator LoadServerData()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(ServerDataEndpoint))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Console.Log($"Failed to load server data:\nError: {request.error}\nResult: {request.result}\nResponse Code: {request.responseCode}\nBody (if any): {request.downloadHandler?.text}");
                    yield break;
                }

                string json = request.downloadHandler.text;
                DataLoadTime = -1f;

                JObject data = JObject.Parse(json);



                string minimumVersion = (string)data["min-version"];
                string version = (string)data["menu-version"];
                bool shownPrompt = false;

                if (VersionToNumber(version) > VersionToNumber(PluginInfo.Version))
                {
                    if (!OutdatedVersion)
                    {
                        OutdatedVersion = true;
                        Console.Log("Version is outdated");
                        Console.SendNotification($"<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to version {version}.", 10000);

                        shownPrompt = true;
                    }
                }

                else if (VersionToNumber(PluginInfo.Version) < VersionToNumber(minimumVersion))
                {
                    if (!OutdatedVersion)
                    {
                        OutdatedVersion = true;
                        Console.Log("Version is severely outdated");
                        GorillaComputer.instance.GeneralFailureMessage("Please update your menu. For safety purposes, you have been blocked from joining rooms.");
                        if (NetworkSystem.Instance.InRoom)
                            NetworkSystem.Instance.ReturnToSinglePlayer();
                        Console.SendNotification($"<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a severely outdated version of the menu. Please update your menu if available. For safety purposes, you have been blocked from joining rooms.", 10000);

                    }
                }
                else if (VersionToNumber(version) > VersionToNumber(PluginInfo.Version))
                {
                    if (!OutdatedVersion)
                    {
                        OutdatedVersion = true;
                        Console.Log("Version is outdated");
                        Console.SendNotification($"<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to version {version}.", 10000);

                        shownPrompt = true;
                    }
                }

                string minConsoleVersion = (string)data["min-console-version"];
                if (VersionToNumber(Console.ConsoleVersion) >= VersionToNumber(minConsoleVersion))
                {
                    Administrators.Clear();

                    JArray admins = (JArray)data["admins"];
                    foreach (var admin in admins)
                    {
                        string name = admin["name"].ToString();
                        string userId = admin["user-id"].ToString();
                        Administrators[userId] = name;
                    }

                    Administrators.AddRange(LocalAdmins);

                    SuperAdministrators.Clear();

                    JArray superAdmins = (JArray)data["super-admins"];
                    foreach (var superAdmin in superAdmins)
                        SuperAdministrators.Add(superAdmin.ToString());

                    if (!GivenAdminMods && PhotonNetwork.LocalPlayer.UserId != null && Administrators.TryGetValue(PhotonNetwork.LocalPlayer.UserId, out var administrator))
                    {
                        GivenAdminMods = true;
                        SetupAdminPanel(administrator);
                    }
                }
                else
                    Console.Log("On extreme outdated version of Console, not loading administrators");
            }
        }





        public static IEnumerator TelemetryRequest(string directory, string identity, string region, string userid, bool isPrivate, int playerCount, string gameMode)
        {
            if (DisableTelemetry)
                yield break;

            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/telemetry", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                directory = CleanString(directory),
                identity = CleanString(identity),
                region = CleanString(region, 3),
                userid = CleanString(userid, 20),
                isPrivate,
                playerCount,
                gameMode = CleanString(gameMode, 128),
                consoleVersion = Console.ConsoleVersion,
                menuName = Console.MenuName,
                menuVersion = Console.MenuVersion
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }

        private static float DataSyncDelay;
        public static int PlayerCount;

        public static void UpdatePlayerCount(NetPlayer Player)
        {
            NetworkSystem.Instance.PlayerListOthers.ForEach(p => ShouldWeReport(RigManager.NetPlayerToPlayer(p)));
            PlayerCount = -1;
        }


        public static bool IsPlayerSteam(VRRig Player)
        {
            string concat = Player.Cosmetics();
            int customPropsCount = Player.Creator.GetPlayerRef().CustomProperties.Count;

            return concat.Contains("S. FIRST LOGIN") ? true : concat.Contains("FIRST LOGIN") || customPropsCount >= 2;
        }

        public static IEnumerator PlayerDataSync(string directory, string region)
        {
            if (DisableTelemetry)
                yield break;

            DataSyncDelay = Time.time + 3f;
            yield return new WaitForSeconds(3f);

            if (!NetworkSystem.Instance.InRoom)
                yield break;

            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

            foreach (Player identification in PhotonNetwork.PlayerList)
            {
                VRRig rig = Console.GetVRRigFromPlayer(identification) ?? VRRig.LocalRig;
                data.Add(identification.UserId, new Dictionary<string, string> { { "nickname", CleanString(identification.NickName) }, { "cosmetics", rig.Cosmetics() }, { "color", $"{Math.Round(rig.playerColor.r * 255)} {Math.Round(rig.playerColor.g * 255)} {Math.Round(rig.playerColor.b * 255)}" }, { "platform", IsPlayerSteam(rig) ? "STEAM" : "QUEST" } });
            }

            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/syncdata", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                directory = CleanString(directory),
                region = CleanString(region, 3),
                data
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }
        #endregion

        #region Menu Specific
        public static IEnumerator ReportFailureMessage(string error)
        {
            if (DisableTelemetry)
                yield break;

            List<string> enabledMods = new List<string>();

            int categoryIndex = 0;
            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                enabledMods.AddRange(from button in category where button.enabled && !Main.categoryNames[categoryIndex].Contains("Settings") select NoASCIIStringCheck(Main.NoRichtextTags(button.overlapText ?? button.buttonText), 128));

                categoryIndex++;
            }


            UnityWebRequest request = new UnityWebRequest(ServerEndpoint + "/reportban", "POST");

            string json = JsonConvert.SerializeObject(new
            {
                error = NoASCIIStringCheck(error, 512),
                version = PluginInfo.Version,
                data = enabledMods
            });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
        }

        public static IEnumerator SendVote(string category)
        {
            UnityWebRequest request = new UnityWebRequest($"{ServerEndpoint}/vote", "POST");

            string json = JsonConvert.SerializeObject(new { option = category });

            byte[] raw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(raw);
            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) yield break;
            try
            {
                string responseText = request.downloadHandler.text;
                Dictionary<string, object> responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);

                int avotes = Convert.ToInt32(responseJson["a-votes"]);
                int bvotes = Convert.ToInt32(responseJson["b-votes"]);

                int total = avotes + bvotes;

                string result;
                if (total > 0)
                {
                    double aPercent = (double)avotes / total * 100;
                    double bPercent = (double)bvotes / total * 100;

                    result = $"Total Votes: {total}\n{OptionA}: {aPercent:F2}%\n{OptionB}: {bPercent:F2}%";
                }
                else
                    result = "No votes yet.";


            }
            catch { }
        }

        public static int onlineUsers = 0;
        private IEnumerator GetSeralythCCU()
        {
            UnityWebRequest request = new UnityWebRequest($"{ServerEndpoint}/usercount", "GET")
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                yield break;
            try
            {
                string responseText = request.downloadHandler.text;
                JObject json = JObject.Parse(responseText);

                onlineUsers = json["mods"]?["seralyth"]?["users"]?.Value<int>() ?? 0;
            }
            catch { }
        }

        public static readonly Dictionary<string, ReportEntry> reportData = new Dictionary<string, ReportEntry>();
        public class ReportEntry
        {
            public string KnownAs;
            public string Reason;
            public GorillaPlayerLineButton.ButtonType ButtonType;
            public string Actor;
            public HashSet<string> reportedIn = new HashSet<string>();
        }

        private IEnumerator GetReportData()
        {
            using UnityWebRequest request = UnityWebRequest.Get($"{ServerEndpoint}/reportdata");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                yield break;

            try
            {
                reportData.Clear();
                JObject json = JObject.Parse(request.downloadHandler.text);

                if (json["report"] is JObject report)
                {
                    foreach (var item in report.Properties())
                    {
                        JObject value = item.Value as JObject;
                        if (value == null) continue;

                        ReportEntry entry = new ReportEntry
                        {
                            KnownAs = value["known-as"].ToString() ?? "Unknown",
                            Reason = value["reason"].ToString() ?? "No reason found",
                            ButtonType = Enum.TryParse(value["ButtonType"]?.ToString(), out GorillaPlayerLineButton.ButtonType buttonType) ? buttonType : GorillaPlayerLineButton.ButtonType.Cheating,
                            Actor = value["actor"].ToString() ?? "Unknown"
                        };

                        if (reportData.TryGetValue(item.Name, out var existing))
                            entry.reportedIn = existing.reportedIn;

                        reportData[item.Name] = entry;
                    }
                }
                if (NetworkSystem.Instance.InRoom)
                    NetworkSystem.Instance.PlayerListOthers.ForEach(p => ShouldWeReport(RigManager.NetPlayerToPlayer(p)));
            }
            catch { }
        }
        #endregion
    }
}
