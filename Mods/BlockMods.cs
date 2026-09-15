using GorillaLocomotion;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LagMenu.Mods
{
    public static class BlockMods
    {
        public static string currentBlock = "random";
        public static bool floatingBlocks = false;
        public static Vector3 sphereOrigin = Vector3.zero;
        private static float blockDelay = 0f;
        private static int cmdId = 0;

        private static readonly Dictionary<Type, float> cacheTime = new Dictionary<Type, float>();
        private static readonly Dictionary<Type, object[]> cacheData = new Dictionary<Type, object[]>();
        private static readonly Dictionary<int, float> delayDict = new Dictionary<int, float>();

        public static void BlockGun()
        {
            GunLib.StartPointerSystem(
                color: Color.cyan,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    DropBlock(
                        currentBlock,
                        GunLib.raycastHit.point + new Vector3(0f, 0.3f, 0f),
                        Vector3.zero,
                        Quaternion.identity,
                        floatingBlocks
                    );
                }
            );
        }

        public static void SphereGun()
        {
            GunLib.StartPointerSystem(
                color: Color.magenta,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    if (sphereOrigin == Vector3.zero)
                        sphereOrigin = GunLib.raycastHit.point + new Vector3(0f, 1f, 0f);

                    Vector3 pos = sphereOrigin + UnityEngine.Random.onUnitSphere * 0.7f;

                    DropBlock(
                        currentBlock,
                        pos,
                        Vector3.zero,
                        Quaternion.LookRotation(sphereOrigin - pos),
                        true
                    );
                },
                onGrip: () =>
                {
                    sphereOrigin = Vector3.zero;
                }
            );
        }

        public static void DestroyBlockGun()
        {
            GunLib.StartPointerSystem(
                color: Color.red,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("[LagMenu] Must be master to destroy blocks");
                        return;
                    }

                    if (GunLib.raycastHit.collider == null) return;

                    BuilderPiece piece = GunLib.raycastHit.collider.GetComponentInParent<BuilderPiece>();
                    if (piece == null) return;

                    D(0.079f, () =>
                    {
                        GetBuilderTable()?.RequestRecyclePiece(piece, false, 2);
                    });
                }
            );
        }

        public static void CrashGun()
        {
            GunLib.StartPointerSystem(
                color: Color.red,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("[LagMenu] Must be master to crash");
                        return;
                    }

                    VRRig target = GunLib.GetTargetRig();
                    if (target == null) return;

                    DropBlock("crash", Vector3.zero, Vector3.zero, Quaternion.identity, false, target);
                }
            );
        }

        public static void LagGun()
        {
            GunLib.StartPointerSystem(
                color: Color.yellow,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Debug.Log("[LagMenu] Must be master to lag");
                        return;
                    }

                    VRRig target = GunLib.GetTargetRig();
                    if (target == null) return;

                    DropBlock("lag", Vector3.zero, Vector3.zero, Quaternion.identity, false, target);
                }
            );
        }

        public static void FlingGun()
        {
            GunLib.StartPointerSystem(
                color: Color.green,
                pointerSize: new Vector3(0.15f, 0.15f, 0.15f),
                pointerShape: PrimitiveType.Sphere,
                rightHand: true,
                onTrigger: () =>
                {
                    VRRig target = GunLib.GetTargetRig();
                    if (target == null) return;

                    DropBlock("fling",
                        target.transform.position,
                        Vector3.zero,
                        Quaternion.identity,
                        false,
                        target
                    );
                }
            );
        }

        public static void ToggleFloatBlocks()
        {
            floatingBlocks = !floatingBlocks;
            Debug.Log("[LagMenu] Floating blocks: " + floatingBlocks);
        }

        private static readonly string[] blockTypes = { "random", "custom" };
        private static int blockTypeIndex = 0;
        public static void CycleBlockType()
        {
            blockTypeIndex = (blockTypeIndex + 1) % blockTypes.Length;
            currentBlock = blockTypes[blockTypeIndex];
            Debug.Log("[LagMenu] Block type: " + currentBlock);
        }

        public static void BlockSpam()
        {
            if (Time.time > blockDelay)
            {
                blockDelay = Time.time + 0.075f;
                DropBlock(
                    currentBlock,
                    VRRig.LocalRig.transform.position,
                    Vector3.zero,
                    Quaternion.identity,
                    floatingBlocks
                );
            }
        }

        public static void DropBlock(string blockName, Vector3 position, Vector3 velocity,
                                      Quaternion rotation, bool floating,
                                      VRRig targetRig = null)
        {
            BuilderTable table = GetBuilderTable();
            if (table == null)
            {
                Debug.Log("[LagMenu] Must be in MonkeBlocks map!");
                return;
            }

            if (!floating && blockName == "SnapPieceArmShelf(Clone)")
            {
                Debug.Log("[LagMenu] Enable floating blocks for arm block mods");
                return;
            }

            float delay = 0.025f;

            int[] crashBlocks = { -741198570, -1447051713, -1961073389, 577606505 };

            Vector3 crashRoomPos = FindGO(
                "Environment Objects/MonkeBlocksRoomPersistent/RoomGeo/" +
                "BuilderFactory (1)/BuilderFactory_FloorCollision/Cube"
            )?.transform.position + new Vector3(0f, 3.255f, 0f) ?? Vector3.zero;

            switch (blockName)
            {
                case "crash":
                    {
                        Photon.Realtime.Player target = targetRig != null
                            ? GetPlayerFromRig(targetRig)
                            : null;
                        PlaceBlock(null, crashRoomPos, velocity, rotation,
                                   false, 0.019f,
                                   crashBlocks[UnityEngine.Random.Range(0, crashBlocks.Length)],
                                   RpcTarget.All, target);
                        return;
                    }

                case "lag":
                    {
                        Photon.Realtime.Player target = targetRig != null
                            ? GetPlayerFromRig(targetRig)
                            : null;
                        PlaceBlock(null, crashRoomPos, velocity, rotation,
                                   false, 0.075f,
                                   crashBlocks[UnityEngine.Random.Range(0, crashBlocks.Length)],
                                   RpcTarget.All, target);
                        return;
                    }

                case "fling":
                    {
                        Photon.Realtime.Player target = targetRig != null
                            ? GetPlayerFromRig(targetRig)
                            : null;
                        PlaceBlock(null, position, velocity, rotation,
                                   false, 0.03f, -566818631,
                                   RpcTarget.All, target);
                        return;
                    }

                case "random":
                    {
                        List<BuilderPiece> unique = new List<BuilderPiece>();
                        foreach (BuilderPiece bp in GetPieces())
                            if (!unique.Contains(bp) && !bp.isArmShelf)
                                unique.Add(bp);

                        BuilderPiece pick = unique[UnityEngine.Random.Range(0, unique.Count)];
                        PlaceBlock(pick, position, velocity, rotation, floating, delay);
                        return;
                    }

                default:
                    {
                        GameObject found = FindGO(blockName);
                        if (found == null) return;
                        BuilderPiece piece = found.GetComponent<BuilderPiece>();
                        if (piece == null) return;
                        PlaceBlock(piece, position, velocity, rotation, floating, delay);
                        return;
                    }
            }
        }

        private static void PlaceBlock(BuilderPiece piece, Vector3 position,
                                        Vector3 velocity, Quaternion rotation,
                                        bool floating, float delay,
                                        int typeOverride = -1,
                                        RpcTarget target = RpcTarget.All,
                                        Photon.Realtime.Player targetPlayer = null)
        {
            D(delay, () =>
            {
                BuilderTable table = GetBuilderTable();
                if (table == null) return;

                if (!PhotonNetwork.IsMasterClient)
                {
                    BuilderPiece nearby = FindNearbyPiece(piece);
                    if (nearby == null) return;

                    table.builderNetworking.RequestGrabPiece(nearby, true, Vector3.zero, Quaternion.identity);
                    table.builderNetworking.RequestDropPiece(nearby, position, rotation, velocity, velocity);
                    return;
                }

                int pieceType = typeOverride != -1
                    ? typeOverride
                    : (piece != null ? piece.pieceType : -1);

                if (pieceType == -1) return;

                int id = table.CreatePieceId();

                PhotonView view = ((MonoBehaviourPun)table.builderNetworking).photonView;

                object[] createParams = new object[]
                {
                    pieceType,
                    id,
                    BitPackUtils.PackWorldPosForNetwork(position),
                    BitPackUtils.PackQuaternionForNetwork(rotation),
                    0,
                    (byte)4,
                    1,
                    PhotonNetwork.LocalPlayer
                };

                if (floating)
                {
                    if (targetPlayer != null)
                        view.RPC("PieceCreatedByShelfRPC", targetPlayer, createParams);
                    else
                        view.RPC("PieceCreatedByShelfRPC", target, createParams);
                }
                else
                {
                    object[] grabParams = new object[]
                    {
                        cmdId++,
                        id,
                        true,
                        BitPackUtils.PackHandPosRotForNetwork(
                            GTPlayer.Instance.LeftHand.controllerTransform.position,
                            GTPlayer.Instance.LeftHand.controllerTransform.rotation),
                        PhotonNetwork.LocalPlayer
                    };

                    object[] dropParams = new object[]
                    {
                        cmdId++,
                        id,
                        position,
                        rotation,
                        velocity,
                        velocity,
                        PhotonNetwork.LocalPlayer
                    };

                    if (targetPlayer != null)
                    {
                        view.RPC("PieceCreatedByShelfRPC", targetPlayer, createParams);
                        view.RPC("PieceGrabbedRPC", targetPlayer, grabParams);
                        view.RPC("PieceDroppedRPC", targetPlayer, dropParams);
                    }
                    else
                    {
                        view.RPC("PieceCreatedByShelfRPC", target, createParams);
                        view.RPC("PieceGrabbedRPC", target, grabParams);
                        view.RPC("PieceDroppedRPC", target, dropParams);
                    }
                }
            });
        }


        private static BuilderTable GetBuilderTable()
        {
            try
            {
                BuilderTable result;
                BuilderTable.TryGetBuilderTableForZone(
                    VRRig.LocalRig.zoneEntity.currentZone, out result);
                return result;
            }
            catch { return null; }
        }

        private static List<BuilderPiece> GetPieces()
        {
            BuilderTable table = GetBuilderTable();
            return table?.pieces ?? new List<BuilderPiece>();
        }

        private static BuilderPiece FindNearbyPiece(BuilderPiece preferred)
        {
            return FindObjects<BuilderPiece>()
                .Where(bp => bp.gameObject.activeInHierarchy
                          && !bp.isBuiltIntoTable
                          && (preferred == null || bp.pieceType == preferred.pieceType)
                          && bp.CanPlayerGrabPiece(
                                PhotonNetwork.LocalPlayer.ActorNumber,
                                bp.transform.position)
                          && Vector3.Distance(
                                bp.transform.position,
                                GTPlayer.Instance.LeftHand.controllerTransform.position) < 2.5f)
                .OrderBy(bp => Vector3.Distance(
                                bp.transform.position,
                                GTPlayer.Instance.LeftHand.controllerTransform.position))
                .FirstOrDefault();
        }

        private static T[] FindObjects<T>() where T : Object
        {
            return Object.FindObjectsByType<T>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
        }

        private static Photon.Realtime.Player GetPlayerFromRig(VRRig rig)
        {
            if (rig == null) return null;
            foreach (var p in PhotonNetwork.PlayerList)
                if (p != null && p.TagObject == rig)
                    return p;
            return null;
        }

        private static GameObject FindGO(string path)
        {
            try { return GameObject.Find(path); }
            catch { return null; }
        }

        private static void D(float time, Action action)
        {
            int key = action.Method.MetadataToken;
            if (!delayDict.ContainsKey(key) || Time.time > delayDict[key])
            {
                action();
                delayDict[key] = Time.time + time;
            }
        }
    }
}
