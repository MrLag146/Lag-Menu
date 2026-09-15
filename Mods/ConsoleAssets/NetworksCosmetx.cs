using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaNetworking;
using LagMenu.Menu;
using Photon.Pun;
using Photon.Realtime;
using static Bindings;
using static LagMenu.Menu.Console;
using Console = LagMenu.Menu.Console;
namespace LagMenu.Mods.ConsoleAssets
{
    public class NetworksCosmetx
    {
        private static bool Enabled;
        private static int[] oldCosmetics;
        private static int[] oldTryOn;
        public static void Start()
        {
            NetworkSystem.Instance.OnPlayerJoined += (Action<NetPlayer>)OnPlayerJoinSpoof;
        }
        public static void Update()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;
            if (oldCosmetics == CosmeticsController.instance.currentWornSet.ToPackedIDArray())
                return;
            oldCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
            string concat = CosmeticsController.instance.currentWornSet.ToDisplayNameArray()
                                               .Aggregate("", (current, cosmetic) => current + cosmetic);
            if (string.IsNullOrEmpty(concat))
                return;
            Console.ExecuteCommand("cosmetic", ReceiverGroup.Others, concat);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others,
                    CosmeticsController.instance.activeMergedSet.ToPackedIDArray(),
                    CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
        }
        public static void OnDisable()
        {
            Enabled = false;
        }
        public static void OnEnable()
        {
            Enabled = true;
            if (!NetworkSystem.Instance.InRoom)
                return;
            oldCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
            string concat = CosmeticsController.instance.currentWornSet.ToDisplayNameArray()
                                               .Aggregate("", (current, cosmetic) => current + cosmetic);
            if (string.IsNullOrEmpty(concat))
                return;
            Console.ExecuteCommand("cosmetic", ReceiverGroup.Others, concat);
            CosmeticsController.instance.UpdateWornCosmetics(true);
        }
        public static void OnPlayerJoinSpoof(NetPlayer player)
        {
            if (!Enabled)
                return;
            string concat = CosmeticsController.instance.currentWornSet.ToDisplayNameArray()
                                               .Aggregate("", (current, cosmetic) => current + cosmetic);
            if (string.IsNullOrEmpty(concat))
                return;
            ExecuteCommand("cosmetic", player.ActorNumber, concat);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others,
                    CosmeticsController.instance.activeMergedSet.ToPackedIDArray(),
                    CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
        }
    }
}
