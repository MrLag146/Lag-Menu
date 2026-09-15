using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using LagMenu.Menu;
using LagMenu.Notifications;
using LagMenu.Patches;
using LagMenu.Utilities;
using Liv.Lck.Tablet;
using Photon.Pun;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;
using static GorillaNetworking.CosmeticsController;
using static LagMenu.Main;
using static LagMenu.Mods.Saftey;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using CosmeticItem = GorillaNetworking.CosmeticsController.CosmeticItem;
using CosmeticSet = GorillaNetworking.CosmeticsController.CosmeticSet;

namespace LagMenu.Mods
{
    internal class World
    {
        public static GameObject physicsObj;
        static bool grabbingBug;
        public static void BugGun()
        {
            GetBugOwnership();
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    ThrowableBug bug = GetBugList()[bugIndex];
                    bug.transform.position = GunLib.raycastHit.point;
                    if (physicsObj != null)
                    {
                        physicsObj.transform.position = GunLib.raycastHit.point;
                        physicsObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        physicsObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    }
                },
                rightHand: true
            );
        }







        public static void EnableNetworkTriggers()
        {

                GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(true);
                return;
            }

        public static void DisableNetworkTriggers()
        {
            GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab").SetActive(false);
        }

        public static void BecomeBug()
        {
            GetBugOwnership();
            ThrowableBug bug = GetBugList()[bugIndex];

            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(0, 0, 0);

            bug.transform.position = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.position;
            bug.transform.rotation = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.rotation;
        }




        static List<ThrowableBug> bugList = new List<ThrowableBug>();
        static float bugDelay;
        public static List<ThrowableBug> GetBugList()
        {
            if (Time.time < bugDelay)
                return bugList;
            bugList.Clear();
            foreach (var bug in UnityEngine.Object.FindObjectsByType<ThrowableBug>(UnityEngine.FindObjectsSortMode.None))
            {
                if (!bugList.Contains(bug) && !bug.name.Contains("Cave"))
                    bugList.Add(bug);
            }
            bugDelay = Time.time + 5f;
            return bugList;
        }

        private static readonly Dictionary<string[], int[]> cachePacked = new Dictionary<string[], int[]>();
        public static int[] PackCosmetics(string[] unpackedCosmetics)
        {
            if (cachePacked.TryGetValue(unpackedCosmetics, out var cosmetics))
                return cosmetics;

            CosmeticsController.CosmeticSet Set = new CosmeticsController.CosmeticSet(unpackedCosmetics, CosmeticsController.instance);
            int[] packedIDs = Set.ToPackedIDArray();
            cachePacked.Add(unpackedCosmetics, packedIDs);
            return packedIDs;
        }
        public static void RGBMonkey()
        {
            float time = Time.time * 1.8f;
            var R = Mathf.Sin(time) * 0.5f + 0.5f;
            var G = Mathf.Sin(time + 2f * Mathf.PI / 3f) * 0.5f + 0.5f;
            var B = Mathf.Sin(time + 4f * Mathf.PI / 3f) * 0.5f + 0.5f;
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { R, G, B });
        }

        public static float getOwnershipDelay;
        public static Coroutine BugCoroutine;
        public static IEnumerator ReturnRig()
        {
            yield return new WaitForSeconds(0.2f);
            VRRig.LocalRig.enabled = true;
            BugCoroutine = null;
        }








        public static int bugIndex;



        public static void GetBugOwnership()
        {
            ThrowableBug bug = GetBugList()[bugIndex];

            if (bug == null || !PhotonNetwork.InRoom)
                return;

            GameObject bugObject = bug.gameObject;

            RequestableOwnershipGuard guard = bug.worldShareableInstance.guard;
            if (guard == null)
                return;

            if (!bug.IsMyItem())
            {
                if (bug.currentState != TransferrableObject.PositionState.Dropped &&
                    bug.currentState != TransferrableObject.PositionState.None)
                    return;

                VRRig.LocalRig.enabled = true;

                if (Vector3.SqrMagnitude(bugObject.transform.position - GorillaTagger.Instance.bodyCollider.transform.position) > 15f)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = bugObject.transform.position;

                    if (BugCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(BugCoroutine);

                    BugCoroutine = CoroutineManager.instance.StartCoroutine(ReturnRig());
                }

                if (Vector3.SqrMagnitude(bugObject.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position) > 15f)
                    return;

                if (Time.time < getOwnershipDelay)
                    return;

                getOwnershipDelay = Time.time + 0.5f;

                guard.ownershipRequestNonce = Guid.NewGuid().ToString();
                guard.currentState = (NetworkingState)3;

                guard.netViews[0].SendRPC(
                    "OwnershipRequested",
                    guard.actualOwner,
                    new object[] { guard.ownershipRequestNonce });

                guard.netViews[0].SendRPC(
                    "TransferOwnershipFromToRPC",
                    RpcTarget.All,
                    new object[]
                    {
                PhotonNetwork.LocalPlayer,
                guard.ownershipRequestNonce
                    });

                guard.netViews[0].SendRPC(
                    "SetOwnershipFromMasterClient",
                    RpcTarget.All,
                    new object[]
                    {
                PhotonNetwork.LocalPlayer
                    });

                guard.SetOwnership(VRRig.LocalRig.Creator, false, false);

                return;
            }

            if (BugCoroutine != null)
            {
                CoroutineManager.instance.StopCoroutine(BugCoroutine);
                VRRig.LocalRig.enabled = true;
            }

            bug.worldShareableInstance.transferableObjectState = TransferrableObject.PositionState.Dropped;
        }


        public static float Timde = 60f;


        public static void StackCosmetics()
        {
            Timde += 1f;
            if (Timde > 60f)
            {
                string[] cosmeticArray = { "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI." };

                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(new string[] { "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI." }, CosmeticsController.instance);
                GorillaTagger.Instance.offlineVRRig.cosmeticSet = new CosmeticsController.CosmeticSet(new string[] { "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI.", "LBAAI." }, CosmeticsController.instance);

                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(cosmeticArray), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);

                NotifiLib.SendNotification("Successfully spoofed cosmetics!");
                GorillaTagger.Instance.offlineVRRig.enabled = true;

                cosmeticsToggle = true;

                foreach (CheckoutCartButton m in GameObject.FindObjectsOfType<CheckoutCartButton>())
                {
                    if (m.isOn)
                    {
                        m.ButtonActivationWithHand(false);
                    }
                }
                foreach (TryOnBundleButton m in GameObject.FindObjectsOfType<TryOnBundleButton>())
                {
                    if (m.isOn)
                    {
                        m.ButtonActivationWithHand(false);
                    }
                }
                foreach (FittingRoomButton m in GameObject.FindObjectsOfType<FittingRoomButton>())
                {
                    if (m.isOn)
                    {
                        m.ButtonActivationWithHand(false);
                    }
                }
                Timde = 0f;
            }
        }

        public static async void KickModders()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer != NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = report.position;
                        VRRig.LocalRig.leftHand.rigTarget.transform.position = report.position;
                        VRRig.LocalRig.rightHand.rigTarget.transform.position = report.position;
                        await Task.Delay(250);
                    }
                }
            }
            catch { }

            VRRig.LocalRig.enabled = true;
        }


        private static int[] archiveCosmetics;
        public static bool cosmeticsToggle;
        public static async void CosmeticsSpoof()
        {
            if (!cosmeticsToggle)
            {
                if (!GorillaTagger.Instance.offlineVRRig.inTryOnRoom)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.1707f, 15.1462f, -120.1849f);
                    cosmeticsToggle = true;
                    PhotonNetwork.RunViewUpdate();
                    await Task.Delay(150);
                }
                string[] cosmeticArray = { "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU." };
                archiveCosmetics = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
                CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(cosmeticArray, CosmeticsController.instance);
                VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(cosmeticArray, CosmeticsController.instance);
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, PackCosmetics(cosmeticArray), CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
                NotifiLib.SendNotification("Successfully spoofed cosmetics!");
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                cosmeticsToggle = true;
            }
        }

        public static async void UnCosmeticsSpoof()
        {
            if (!GorillaTagger.Instance.offlineVRRig.inTryOnRoom)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(-51.1707f, 15.1462f, -120.1849f);
                PhotonNetwork.RunViewUpdate();
                await Task.Delay(150);
            }
            CosmeticsController.instance.currentWornSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            VRRig.LocalRig.cosmeticSet = new CosmeticsController.CosmeticSet(archiveCosmetics, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, archiveCosmetics, CosmeticsController.instance.tryOnSet.ToPackedIDArray(), false);
            GorillaTagger.Instance.offlineVRRig.enabled = true;
            cosmeticsToggle = false;
        }






        public static IEnumerator EnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }




        public static Coroutine dropBoard;

        public static void BetaDropBoard(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
        {
            if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, position) > 5f)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = position + Vector3.down * 4f;

                if (dropBoard != null)
                    CoroutineManager.instance.StopCoroutine(dropBoard);

                dropBoard = CoroutineManager.instance.StartCoroutine(EnableRig());
            }

            FreeHoverboardManager.instance.SendDropBoardRPC(position, rotation, velocity, avelocity, boardColor);

        }

        private static float hoverboardGunDelay;



        public static void ShowerGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target == null) return;

                Water(
                    target.transform.position + new Vector3(0f, 0.6f, 0f),
                    Quaternion.LookRotation(Vector3.down)
                );

            },
            onGrip: () =>
            {
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }


        private static void Water(Vector3 position, Quaternion rotation)
        {
            if (!PhotonNetwork.InRoom) return;

            if (Vector3.Distance(position, VRRig.LocalRig.transform.position) > 3f)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = position + new Vector3(0f, 0.8f, 0f);
                return;
            }

            GorillaTagger.Instance.myVRRig.SendRPC(
                "RPC_PlaySplashEffect",
                RpcTarget.All,
                position,
                rotation,
                4f,
                100f,
                true,
                false
            );
        }


        public static void HoverboardGun()
        {
            GunLib.StartPointerSystem(
                Color.magenta,
                new Vector3(0.15f, 0.15f, 0.15f),
                PrimitiveType.Sphere,
                GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform,

                delegate
                {
                    if (Time.time > hoverboardGunDelay)
                    {
                        hoverboardGunDelay = Time.time + 0.25f;

                        Vector3 spawnPos = GunLib.raycastHit.collider != null
                            ? GunLib.raycastHit.point + Vector3.up
                            : GunLib.pointer.transform.position + Vector3.up;

                        BetaDropBoard(
                            spawnPos,
                            RandomQuaternion(),
                            Vector3.zero,
                            Vector3.zero,
                            RandomColor()
                        );
                    }
                },

                delegate { }
            );
        }

        public static Quaternion RandomQuaternion(float range = 360f) =>
          Quaternion.Euler(Random.Range(0f, range),
                      Random.Range(0f, range),
                      Random.Range(0f, range));








        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
         new Color32((byte)Random.Range(0, range),
                     (byte)Random.Range(0, range),
                     (byte)Random.Range(0, range),
                     alpha);

        public static void ChangeColor(Color color, object target = null)
        {
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));

            GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
            PlayerPrefs.Save();

            try
            {
                switch (target)
                {
                    case null:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, color.r, color.g, color.b);
                        break;
                    case NetPlayer player:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", player, color.r, color.g, color.b);
                        break;
                    case RpcTarget targets:
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", targets, color.r, color.g, color.b);
                        break;
                }


            }
            catch { }
        }


        public static void SpawnHoverboard()
        {
            BetaDropBoard(VRRig.LocalRig.transform.position, VRRig.LocalRig.transform.rotation, Vector3.zero, Vector3.zero, RandomColor());
            GTPlayer.Instance.SetHoverActive(true);
        }


        public static Coroutine RopeCoroutine;
        public static IEnumerator RopeEnableRig()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }




        public static void BetaSetRopeVelocity(int RopeId, Vector3 Velocity)
        {
            Velocity = Velocity.ClampMagnitudeSafe(100f);

            GorillaRopeSwing[] ropes = UnityEngine.Object.FindObjectsOfType<GorillaRopeSwing>();

            GorillaRopeSwing Rope = ropes.FirstOrDefault(x => x.ropeId == RopeId);

            if (Rope != null)
            {
                var ClosestNode = Rope.nodes
                    .Skip(1)
                    .Select((v, i) => new
                    {
                        index = i,
                        transform = v,
                        distance = Vector3.Distance(
                            GorillaTagger.Instance.bodyCollider.transform.position,
                            v.transform.position
                        )
                    })
                    .OrderBy(x => x.distance)
                    .First();

                if (ClosestNode.distance > 5f)
                {
                    if (RopeCoroutine != null)
                        CoroutineManager.instance.StopCoroutine(RopeCoroutine);

                    RopeCoroutine = CoroutineManager.instance.StartCoroutine(RopeEnableRig());

                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = ClosestNode.transform.position;
                }

                Vector3 ServerPos = GorillaTagger.Instance.bodyCollider.transform.position;

                if (Vector3.Distance(ServerPos, ClosestNode.transform.position) < 5f)
                {
                    RopeSwingManager.instance.SendSetVelocity_RPC(
                        RopeId,
                        ClosestNode.index,
                        Velocity.ClampMagnitudeSafe(100f),
                        true
                    );
                }
                else
                {
                    RopeDelay = 0f;
                }

                Saftey.RPCProtection();
            }
        }

        public static void BetaSetRopeVelocity(GorillaRopeSwing Rope, Vector3 Velocity)
        {
            BetaSetRopeVelocity(Rope.ropeId, Velocity);
        }



        public static float RopeDelay;
        private static float ropeTeleportDelay;

        public static bool ropeSpazMode;






        private static float randomRopeDelay;
        private static GorillaRopeSwing randomRope;
        public static GorillaRopeSwing GetRandomRope()
        {
            if (Time.time > randomRopeDelay)
            {
                randomRopeDelay = Time.time + 0.5f;

                randomRope = Object.FindObjectsOfType<GorillaRopeSwing>()
                    .OrderBy(_ => Random.value)
                    .FirstOrDefault();
            }

            return randomRope;
        }


        public static void SpazAllRopes()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f && Time.time > RopeDelay)
            {
                RopeDelay = Time.time + 0.125f;

                GorillaRopeSwing rope = GetRandomRope();
                BetaSetRopeVelocity(rope, RandomVector3(100f));
            }
        }


        public static float ropeCooldown;

        public static void RopeGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                if (Time.time < ropeCooldown) return;
                if (GunLib.raycastHit.collider == null) return;

                Transform target = GunLib.raycastHit.collider.transform;
                if (!target.name.Contains("RopeBone")) return;

                ropeCooldown = Time.time + 0.1f;

                GorillaRopeSwing g = target.GetComponentInParent<GorillaRopeSwing>();
                if (g == null) return;

                Transform n = target;
                Vector3 v = (target.position - GorillaTagger.Instance.bodyCollider.transform.position).normalized * 10f + Vector3.up * 5f;

                n = n ?? g.nodes.Last();

                Vector3 SP = GorillaTagger.Instance.bodyCollider.transform.position;

                if (Vector3.Distance(SP, n.position) > 5f)
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = n.position;

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = n.position;
                }
                else
                {
                    if (!VRRig.LocalRig.enabled)
                    {
                        VRRig.LocalRig.transform.position = n.position;
                        GorillaTagger.Instance.offlineVRRig.transform.position = n.position;
                    }

                    if (Vector3.Distance(SP, n.position) < 5f)
                    {
                        RopeSwingManager.instance.SendSetVelocity_RPC(
                            g.ropeId,
                            g.GetBoneIndex(n),
                            v,
                            true
                        );
                    }
                }

                RPCProtection();

                if (Vector3.Distance(VRRig.LocalRig.transform.position, n.position) > 2f)
                {
                    VRRig.LocalRig.enabled = true;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            },
            onGrip: () =>
            {
                VRRig.LocalRig.enabled = true;
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            },
            rightHand: true);
        }












        public static Vector3 RandomVector3(float range = 1f) =>
         new Vector3(Random.Range(-range, range),
                     Random.Range(-range, range),
                     Random.Range(-range, range));




        private static float spamDelay;
        private static bool returnOrTeleport;
        public static void ArcadeTeleporterEffectSpam()
        {
            if (!PhotonNetwork.InRoom) return;
            if (Time.time > spamDelay)
            {
                spamDelay = Time.time + 0.1f;
                returnOrTeleport = !returnOrTeleport;

                GetObject("City_Pretty/CosmeticsScoreboardAnchor/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", RpcTarget.All, returnOrTeleport, (short)Random.Range(0, 7));
                RPCProtection();
            }
        }

        public static void StumpTeleporterEffectSpam()
        {
            if (!PhotonNetwork.InRoom) return;
            if (Time.time > spamDelay)
            {
                spamDelay = Time.time + 0.1f;
                returnOrTeleport = !returnOrTeleport;

                GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/StumpVRHeadset/VirtualStump_StumpTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", RpcTarget.All, returnOrTeleport, (short)0);
                RPCProtection();
            }
        }













        public static bool leftGrab = ControllerInputPoller.instance.leftGrab;


        public static bool rightGrab = ControllerInputPoller.instance.rightGrab;





        public static void SetBraceletState(bool enable, bool isLeftHand) =>
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, enable, isLeftHand);

        public static void GetBracelet(bool state)
        {
            if (leftGrab)
            {
                SetBraceletState(false, false);
                SetBraceletState(state, true);
            }

            if (rightGrab)
            {
                SetBraceletState(state, false);
                SetBraceletState(false, true);
            }

            if (leftGrab || rightGrab)
                RPCProtection();
        }

        private static bool previousBraceletSpamState;
        private static float braceletSpamDelay;
        public static void BraceletSpam()
        {
            if (Time.time > braceletSpamDelay)
            {
                GetBracelet(Time.frameCount % 2 == 0);
                braceletSpamDelay = Time.time + 0.1f;

                previousBraceletSpamState = !previousBraceletSpamState;
            }
        }

        public static void RemoveBracelet()
        {
            SetBraceletState(false, true);
            SetBraceletState(false, false);
            RPCProtection();
        }

        public static float isDirtyDelay;
        public static void RainbowBracelet()
        {
            BraceletPatch.enabled = true;
            if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
            {
                SetBraceletState(true, false);
                RPCProtection();

                VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(true);
            }
            List<Color> rgbColors = new List<Color>();
            for (int i = 0; i < 10; i++)
                rgbColors.Add(Color.HSVToRGB((Time.frameCount / 180f + i / 10f) % 1f, 1f, 1f));

            VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
            VRRig.LocalRig.reliableState.braceletSelfIndex = 99;
            VRRig.LocalRig.reliableState.braceletBeadColors = rgbColors;
            VRRig.LocalRig.friendshipBraceletRightHand.UpdateBeads(rgbColors, 99);

            if (Time.time > isDirtyDelay)
            {
                isDirtyDelay = Time.time + 0.1f;
                VRRig.LocalRig.reliableState.SetIsDirty();
            }
        }

        public static void RemoveRainbowBracelet()
        {
            BraceletPatch.enabled = false;
            if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
            {
                SetBraceletState(false, false);
                RPCProtection();

                VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(false);
            }

            VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
            VRRig.LocalRig.reliableState.braceletSelfIndex = 0;
            VRRig.LocalRig.reliableState.braceletBeadColors.Clear();
            VRRig.LocalRig.UpdateFriendshipBracelet();

            VRRig.LocalRig.reliableState.SetIsDirty();
        }




















        

        private static float hoverboardScreenGunDelay;
        public static void HoverboardScreenGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null && Time.time > hoverboardScreenGunDelay)
                {
                    hoverboardScreenGunDelay = Time.time + 0.3f;
                    ScreenTarget(GunLib.lockTarget, Color.black);
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
            },
            rightHand: true);
        }

        private static Coroutine hoverboardScreenCoroutine;
        private static IEnumerator ReleaseHoverboardScreen()
        {
            yield return new WaitForSeconds(0.3f);
            VRRig.LocalRig.enabled = true;
        }

        private static void ScreenTarget(VRRig target, Color color)
        {
            if (hoverboardScreenCoroutine != null)
                CoroutineManager.instance.StopCoroutine(hoverboardScreenCoroutine);

            hoverboardScreenCoroutine = CoroutineManager.instance.StartCoroutine(ReleaseHoverboardScreen());

            Vector3 screenPosition = target.headMesh.transform.position + target.headMesh.transform.forward * 0.35f;
            Quaternion screenRotation = target.headMesh.transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = screenPosition - Vector3.up * 0.5f;

            HoverboardVisual visual = VRRig.LocalRig.hoverboardVisual;
            visual.SetIsHeld(true, visual.NominalParentTransform.InverseTransformPoint(screenPosition), visual.NominalParentTransform.InverseTransformRotation(screenRotation), color);

            visual.interpolatedLocalPosition = visual.NominalLocalPosition;
            visual.interpolatedLocalRotation = visual.NominalLocalRotation;

            RPCProtection();
        }

        public static void HoverboardScreenAll()
        {
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
            {
                VRRig target = RigManager.FindRig(player);
                if (target != null)
                    ScreenTarget(target, Color.black);
            }
        }

        public static void BecomeHoverboard()
        {
            Vector3 boardPosition = GorillaTagger.Instance.bodyCollider.transform.position;
            Quaternion boardRotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = boardPosition - Vector3.up * 1f;
            
            GTPlayer.Instance.SetHoverActive(true);

            HoverboardVisual visual = VRRig.LocalRig.hoverboardVisual;
            visual.SetIsHeld(true, visual.NominalParentTransform.InverseTransformPoint(boardPosition), visual.NominalParentTransform.InverseTransformRotation(boardRotation), VRRig.LocalRig.playerColor);

            visual.interpolatedLocalPosition = visual.NominalLocalPosition;
            visual.interpolatedLocalRotation = visual.NominalLocalRotation;
        }

        public static void DisableBecomeHoverboard()
        {
            VRRig.LocalRig.enabled = true;
            GTPlayer.Instance.SetHoverActive(false);
        }

        private static string animatedNameBase;
        public static void AnimatedName()
        {
            if (string.IsNullOrEmpty(animatedNameBase))
                animatedNameBase = PhotonNetwork.LocalPlayer.NickName;

            int length = Mathf.Clamp((int)Mathf.PingPong(Time.time / 0.25f, animatedNameBase.Length) + 1, 1, animatedNameBase.Length);
            RigManager.ChangeName(animatedNameBase.Substring(0, length));
        }

        public static void DisableAnimatedName()
        {
            if (!string.IsNullOrEmpty(animatedNameBase))
                RigManager.ChangeName(animatedNameBase);

            animatedNameBase = null;
        }

        private static float copyIdentityDelay;
        public static void CopyIdentityGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();

                if (target != null && !target.isLocal && Time.time > copyIdentityDelay)
                {
                    copyIdentityDelay = Time.time + 0.5f;

                    Photon.Realtime.Player player = GunLib.GetPlayerFromVRRig(target);
                    if (player != null)
                        RigManager.ChangeName(player.NickName);

                    ChangeColor(target.playerColor);
                }
            },
            onGrip: () => { },
            rightHand: true);
        }

        private static float nameCycleDelay;
        private static int nameCycleIndex;
        public static void NameCycle(string[] names)
        {
            if (names == null || names.Length == 0)
                return;

            if (Time.time > nameCycleDelay)
            {
                nameCycleIndex = (nameCycleIndex + 1) % names.Length;
                RigManager.ChangeName(names[nameCycleIndex]);
                nameCycleDelay = Time.time + 1f;
            }
        }

        public static void RandomNameCycle() =>
            NameCycle(new[] { "Lag", "Menu", "User", "Guest", "????" });

        public static void FlashNameTag() =>
            VRRig.LocalRig.ShowGoldNameTag = (Time.time % 0.2f) > 0.1f;

        public static void DisableFlashNameTag() =>
            VRRig.LocalRig.ShowGoldNameTag = false;

        private static Photon.Voice.Unity.Recorder GetVoiceRecorder()
        {
            try
            {
                return NetworkSystem.Instance?.VoiceConnection?.PrimaryRecorder;
            }
            catch
            {
                return null;
            }
        }

        private class LagMicProcessor : IProcessor<float>
        {
            public float DropChance = 0.2f;

            public float[] Process(float[] buffer)
            {
                if (Random.value < DropChance)
                    Array.Clear(buffer, 0, buffer.Length);

                return buffer;
            }

            public void Dispose() { }
        }

        private class GlitchMicProcessor : IProcessor<float>
        {
            private float[] heldChunk;
            private int holdFramesLeft;

            public float[] Process(float[] buffer)
            {
                if (holdFramesLeft > 0)
                {
                    Array.Copy(heldChunk, buffer, Mathf.Min(heldChunk.Length, buffer.Length));
                    holdFramesLeft--;
                }
                else if (Random.value < 0.02f)
                {
                    heldChunk = (float[])buffer.Clone();
                    holdFramesLeft = Random.Range(4, 12);
                }

                return buffer;
            }

            public void Dispose() { }
        }

        private class EchoMicProcessor : IProcessor<float>
        {
            private readonly float[] delayLine;
            private int writeHead;

            public EchoMicProcessor(int sampleRate)
            {
                delayLine = new float[Mathf.Max(1, sampleRate / 3)];
            }

            public float[] Process(float[] buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    float delayed = delayLine[writeHead];
                    float mixed = Mathf.Clamp(buffer[i] + delayed * 0.5f, -1f, 1f);

                    delayLine[writeHead] = mixed;
                    buffer[i] = mixed;

                    writeHead = (writeHead + 1) % delayLine.Length;
                }

                return buffer;
            }

            public void Dispose() { }
        }

        private class MicEffectHook : Photon.Voice.Unity.VoiceComponent
        {
            public IProcessor<float> Processor;

            private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
            {
                if (Processor != null && p.Voice is LocalVoiceAudioFloat floatVoice)
                    floatVoice.AddPostProcessor(Processor);
            }
        }

        private static MicEffectHook micEffectHook;
        private static IProcessor<float> activeMicProcessor;

        private static void SetMicProcessor(IProcessor<float> processor)
        {
            var recorder = GetVoiceRecorder();
            if (recorder == null)
                return;

            if (micEffectHook == null)
                micEffectHook = recorder.gameObject.AddComponent<MicEffectHook>();

            activeMicProcessor?.Dispose();
            activeMicProcessor = processor;
            micEffectHook.Processor = processor;

            recorder.RestartRecording(true);
        }

        private static void ClearMicProcessor()
        {
            if (micEffectHook != null)
                micEffectHook.Processor = null;

            activeMicProcessor?.Dispose();
            activeMicProcessor = null;

            var recorder = GetVoiceRecorder();
            recorder?.RestartRecording(true);
        }

        public static void LaggyMic(bool enabled)
        {
            if (enabled)
                SetMicProcessor(new LagMicProcessor());
            else
                ClearMicProcessor();
        }

        public static void GlitchyMic(bool enabled)
        {
            if (enabled)
                SetMicProcessor(new GlitchMicProcessor());
            else
                ClearMicProcessor();
        }

        public static void EchoMic(bool enabled)
        {
            var recorder = GetVoiceRecorder();
            int sampleRate = recorder != null ? (int)recorder.SamplingRate : 16000;

            if (enabled)
                SetMicProcessor(new EchoMicProcessor(sampleRate > 0 ? sampleRate : 16000));
            else
                ClearMicProcessor();
        }

        public static void MuteMic(bool mute)
        {
            var recorder = GetVoiceRecorder();
            if (recorder != null)
                recorder.IsRecording = !mute;
        }

    }
}
