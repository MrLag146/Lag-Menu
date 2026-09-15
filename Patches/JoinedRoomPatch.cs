using GorillaNetworking;
using HarmonyLib;
using System.Reflection;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController), nameof(PhotonNetworkController.OnJoinedRoom))]
    public class JoinedRoomPatch
    {
        public static bool enabled;

        private static void Prefix()
        {
            if (!enabled) return;

            try
            {
                var field = typeof(PhotonNetworkController).GetField("currentJoinType",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    field.SetValue(PhotonNetworkController.Instance, JoinType.FollowingParty);
                    return;
                }

                var prop = typeof(PhotonNetworkController).GetProperty("currentJoinType",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (prop != null)
                {
                    prop.SetValue(PhotonNetworkController.Instance, JoinType.FollowingParty);
                    return;
                }

                foreach (var f in typeof(PhotonNetworkController).GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (f.FieldType == typeof(JoinType))
                        UnityEngine.Debug.Log("[LagMenu] JoinType field: " + f.Name);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("[LagMenu] JoinedRoomPatch error: " + e);
            }
        }
    }
}
