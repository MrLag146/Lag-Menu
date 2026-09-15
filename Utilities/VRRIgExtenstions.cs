using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using LagMenu;
using LagMenu.Utilities;
using static LagMenu.Utilities.RigManager;
using System;
using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;


namespace LagMenu.Utilities
{


    public static class VRRigExtensions
    {
        public static bool IsTagged(this VRRig rig)
        {
            if (rig == null) return false;
            List<NetPlayer> infectedPlayers = GameModeUtilities.InfectedList();
            NetPlayer targetPlayer = rig.GetPlayer();
            return infectedPlayers.Contains(targetPlayer);
        }

        public static NetPlayer GetPlayer(this VRRig rig) =>
            RigManager.GetPlayerFromVRRig(rig);


        public static VRRig VRRig(this NetPlayer self) =>
            RigManager.GetVRRigFromPlayer(self);

        public static PhotonView GetPhotonView(this VRRig rig) =>
            rig.netView.GetView;
        public static ProjectileWeapon GetSlingshot(this VRRig rig) =>
            rig.projectileWeapon;


        public static bool Active(this VRRig rig) =>
           rig != null && VRRigCache.ActiveRigs.Contains(rig);

        public static void Obliterate(this GameObject obj) => Object.Destroy(obj);

        public static readonly Dictionary<VRRig, int> PlayerPing = new Dictionary<VRRig, int>();

        public static int GetPing(this VRRig rig) =>
                PlayerPing.TryGetValue(rig, out int ping) ? ping : PhotonNetwork.GetPing();


        public static Photon.Realtime.Player GetPhotonPlayer(this VRRig rig) =>
           RigManager.NetPlayerToPlayer(RigManager.GetPlayerFromVRRig(rig));

        public static bool IsLocal(this VRRig rig) =>
            rig != null && (rig.isLocal);

        public static bool IsLeftHandGrabbable(this VRRig rig) =>
            rig != null && (rig.leftMiddle.calcT > 0.8f || rig.leftIndex.calcT > 0.8f) && rig.reliableState.transferrablePosStates.All(s => s != TransferrableObject.PositionState.InLeftHand);

        public static bool IsRightHandGrabbable(this VRRig rig) =>
           rig != null && (rig.rightMiddle.calcT > 0.8f || rig.rightIndex.calcT > 0.8f) && rig.reliableState.transferrablePosStates.All(s => s != TransferrableObject.PositionState.InRightHand);

        public static string Cosmetics(this VRRig rig) =>
           rig._playerOwnedCosmetics.Concat();


        private static readonly List<VRRig> _rigs = new List<VRRig>();
        private static int _lastFrame = -1;
        private static readonly object _lock = new object();




        public static CallLimiter GetCallLimiter(this FXSystemSettings settings, int index) =>
       settings.callSettings[index].CallLimitSettings;

        public static CallLimitType<CallLimiter> GetCallLimitType(this FXSystemSettings settings, int index) =>
            settings.callSettings[index];

        public static bool CanCallNow(this CallLimiter limiter, float? time = null, bool useNetworkTime = false)
        {
            float currentTime = time ?? (useNetworkTime
                ? (float)PhotonNetwork.Time
                : Time.time);

            Type type = limiter.GetType();


            string[] names =
            {
        "CanCall",
        "CanCallNow",
        "Check",
        "CheckCall",
        "IsReady",
        "TryCall"
    };

            foreach (string name in names)
            {
                MethodInfo method = type.GetMethod(name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method == null)
                    continue;

                object result = method.GetParameters().Length switch
                {
                    0 => method.Invoke(limiter, null),
                    1 => method.Invoke(limiter, new object[] { currentTime }),
                    _ => null
                };

                if (result is bool b)
                    return b;
            }

            return true;
        }


        public static List<VRRig> ActiveRigs
        {
            get
            {
                int frame = Time.frameCount;
                if (frame == _lastFrame)
                    return _rigs;

                lock (_lock)
                {
                    if (frame == _lastFrame)
                        return _rigs;

                    _lastFrame = frame;
                    _rigs.Clear();

                    foreach (var rig in VRRigCache.ActiveRigs)
                    {
                        if (rig != null)
                            _rigs.Add(rig);
                    }
                }

                return _rigs;


            }
        }
    }
}









