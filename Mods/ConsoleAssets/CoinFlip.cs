using System;
using System.Collections.Generic;
using System.Text;
using LagMenu.Utilities;
using Photon.Realtime;
using UnityEngine;
using static Bindings;
using static LagMenu.Menu.Console;
using Console = LagMenu.Menu.Console;
using Random = UnityEngine.Random;
namespace LagMenu.Mods.ConsoleAssets
{
    public class CoinFlip
    {
        private static int allocatedCoinId = -1;
        private static int coinChain;
        private static bool coinChainHeads;
        private static int coinHeads;
        private static int coinTails;
        private static bool lastFlipping;

        public static void Update()
        {
            bool rightGrab = InputManager.Instance.RightGrip.IsPressed;
            bool rightTrigger = InputManager.Instance.RightTrigger.IsPressed;
            bool rightPrimary = InputManager.Instance.RightPrimary.IsPressed;
            bool rightSecondary = InputManager.Instance.RightSecondary.IsPressed;

            if (rightGrab && rightTrigger)
            {
                if (allocatedCoinId == -1 && (rightPrimary || rightSecondary))
                {
                    allocatedCoinId = Console.GetFreeAssetID();

                    ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Coin",
                            allocatedCoinId);

                    ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedCoinId, 2);

                    Saftey.RPCProtection();
                }

                if (allocatedCoinId == -1) return;

                bool flipping = rightPrimary || rightSecondary;

                if (!flipping && lastFlipping)
                {
                    bool heads = Random.Range(0f, 1f) >= 0.5f;
                    if (heads != coinChainHeads)
                    {
                        coinChain = 0;
                        coinChainHeads = heads;
                    }

                    coinChain++;

                    if (heads) coinHeads++;
                    else coinTails++;

                    Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, allocatedCoinId,
                            "CoinHolder", heads ? "Heads" : "Tails");

                   Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedCoinId,
                            "CoinHolder", "Flip");
                }

                lastFlipping = flipping;
            }
            else
            {
                lastFlipping = false;

                if (allocatedCoinId != -1)
                {
                    Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedCoinId);
                    allocatedCoinId = -1;
                }
            }
        }

        public static void OnDisable()
        {
            if (allocatedCoinId != -1)
            {
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, allocatedCoinId);
                allocatedCoinId = -1;
            }
        }
    }
}
