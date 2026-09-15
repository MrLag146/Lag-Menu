using System.Text;
using System;
using System.Collections.Generic;
using GorillaLocomotion;
using LagMenu.Menu;
using LagMenu.Mods;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Video;
using static Bindings;
using static UnityEngine.GridBrushBase;
using Console = LagMenu.Menu.Console;
using Random = UnityEngine.Random;
namespace LagMenu.Mods.ConsoleAssets
{
    public class Sword
    {
        private static int assetId;
        public static void OnEnable()
        {
            assetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "consolehamburburassets", nameof(Sword),
                assetId);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, assetId, 2);
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, assetId,
                new Vector3(0.1f, 0.1f, 0.2f));
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, assetId,
                Quaternion.Euler(0f, 90f, 90f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId, Vector3.one * 0.1f);
        }
        public static void OnDisable() =>
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
    }
}
