using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LagMenu.Mods
{
    internal class Networking
    {
        private const float PropertyChangeCooldown = 0.1f;
        private const byte EventCode = 176;

        private static readonly int EventId = ComputeHash("GorillaShirts");

        private static readonly List<Hashtable> Presets = new List<Hashtable>
        {
            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/Abstracted Gorilla" } }
            },

            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/Pibby Gorilla" } }
            },

            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/Polygon Figher" } }
            },

            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/Scummy Gorilla" } }
            },

            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/OJ's StyledSnail" } }
            },

            new Hashtable
            {
                { "TagOffset", 0 },
                { "Fallbacks", new int[] { 0 } },
                { "Colours", new int[] { -1 } },
                { "Shirts", new string[] { "Custom/Animatronic Suit" } }
            }
        };

        private static int currentPreset;
        private static float lastTime;

        public static void EnableConsoleSpoof()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        public static void DisableConsoleSpoof()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        }

        private static void OnEventReceived(EventData eventData)
        {
            if (eventData.Code != 68)
                return;

            object rawData;

            if (!eventData.Parameters.TryGetValue(
                    ParameterCode.Data,
                    out rawData))
            {
                return;
            }

            object[] data = rawData as object[];

            if (data == null || data.Length == 0)
                return;

            string command = data[0] as string;

            if (command != "isusing")
                return;

            string rainbowText = GetRainbowText("YOU FUCKING BITCH");

            object[] response =
            {
                "confirmusing",
                "69.67",
                "<size=300%>" + rainbowText + "</size>"
            };

            PhotonNetwork.RaiseEvent(
                68,
                response,
                new RaiseEventOptions
                {
                    TargetActors = new int[]
                    {
                        eventData.Sender
                    }
                },
                SendOptions.SendReliable
            );
        }

        private static string GetRainbowText(string input)
        {
            string result = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                float hue = (float)i / input.Length;

                Color color = Color.HSVToRGB(
                    hue,
                    1f,
                    1f
                );

                string hex =
                    ColorUtility.ToHtmlStringRGB(color);

                result +=
                    "<color=#" + hex + ">" +
                    input[i] +
                    "</color>";
            }

            return result;
        }

        public static void FuckGShirts()
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (Time.time - lastTime < PropertyChangeCooldown)
                return;

            currentPreset++;

            if (currentPreset >= Presets.Count)
                currentPreset = 0;

            lastTime = Time.time;

            Send(Presets[currentPreset]);
        }

        private static void Send(Hashtable properties)
        {
            object[] content = new object[]
            {
                EventId,
                properties
            };

            RaiseEventOptions options = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others
            };

            PhotonNetwork.RaiseEvent(
                EventCode,
                content,
                options,
                SendOptions.SendReliable
            );
        }

        public static void DisableFuckGShirts()
        {
            if (!PhotonNetwork.InRoom)
                return;

            Send(new Hashtable
            {
                { "Shirts", new string[0] },
                { "Colours", new int[0] },
                { "Fallbacks", new int[0] },
                { "TagOffset", 0 }
            });
        }

        private static int ComputeHash(string input)
        {
            unchecked
            {
                return input.Aggregate(
                    23,
                    (current, c) => current * 31 + c
                );
            }
        }
    }
}
