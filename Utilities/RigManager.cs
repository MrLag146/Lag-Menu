using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using LagMenu.Mods;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Modio.API.ModioAPI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Utilities
{

    internal class RigManager : MonoBehaviour
    {
        public static VRRig GetVRRigFromPlayer(NetPlayer p) =>
            GorillaGameManager.StaticFindRigForPlayer(p);

        public static Player GetPlayerFromVRRig(VRRig p)
        {
            return p.Creator.GetPlayerRef();
        }




        public static VRRig FindRig(NetPlayer netPlayer)
        {
            if (netPlayer != null)
            {
                return GorillaGameManager.StaticFindRigForPlayer(netPlayer);
            }
            return null;
        }



        public static Player GetPlayerFromVRRigSafe(VRRig p)
        {
            try
            {
                return p.Creator.GetPlayerRef();
            }
            catch
            {
                return null;
            }
        }


        public static PhotonView GetPhotonView(VRRig vrrig_0)
        {
            return ((NetworkView)Traverse.Create((object)vrrig_0).Field("netView").GetValue()).GetView;
        }
        public static Shader guiShader = Shader.Find("GUI/Text Shader");







        public static void ChangeColor(Color color, object target = null)
        {
            PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
            PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
            PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));
            GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
            PlayerPrefs.Save();
            if (target != null)
            {
                NetPlayer val = (NetPlayer)((target is NetPlayer) ? target : null);
                if (val == null)
                {
                    if (target is RpcTarget val2)
                    {
                        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", val2, new object[3] { color.r, color.g, color.b });
                    }
                }
                else
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", val, new object[3] { color.r, color.g, color.b });
                }
            }
            else
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", (RpcTarget)0, new object[3] { color.r, color.g, color.b });
            }
            Saftey.RPCProtection();
        }

        public static void ChangeName(string PlayerName, bool noColor = false)
        {
            ((GorillaComputer)GorillaComputer.instance).currentName = PlayerName;
            ((GorillaComputer)GorillaComputer.instance).SetLocalNameTagText(((GorillaComputer)GorillaComputer.instance).currentName);
            ((GorillaComputer)GorillaComputer.instance).savedName = ((GorillaComputer)GorillaComputer.instance).currentName;
            PlayerPrefs.SetString("playerName", ((GorillaComputer)GorillaComputer.instance).currentName);
            PlayerPrefs.Save();
            PhotonNetwork.LocalPlayer.NickName = PlayerName;
            if (!noColor && (((GorillaComputer)GorillaComputer.instance).friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) || CosmeticWardrobeProximityDetector.IsUserNearWardrobe(PhotonNetwork.LocalPlayer.ActorNumber)))
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", (RpcTarget)0, new object[3]
                {
                VRRig.LocalRig.playerColor.r,
                VRRig.LocalRig.playerColor.g,
                VRRig.LocalRig.playerColor.b
                });
               Saftey.RPCProtection();
            }
        }





        public static NetPlayer GetNetPlayerFromVRRig(VRRig vrrig)
        {
            return vrrig.Creator ?? vrrig.OwningNetPlayer ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(((Component)vrrig.rigSerializer).gameObject));
        }


        public static NetworkView GetNetworkFromRig(VRRig rig)
        {
            return (NetworkView)Traverse.Create(rig).Field("netView").GetValue();
        }
        public static Photon.Realtime.Player GetPlayerFromGun()
        {
            if (GunLib.gunLocked && GunLib.lockTarget != null)
            {
                return GetPlayerFromVRRig(GunLib.lockTarget);
            }

            if (GunLib.raycastHit.collider != null)
            {
                VRRig rig = GunLib.raycastHit.collider.GetComponentInParent<VRRig>();

                if (rig != null)
                {
                    return GetPlayerFromVRRig(rig);
                }
            }

            return null;
        }
        public static PhotonView GetPhotonViewFromVRRig(VRRig p)
        {
            return (PhotonView)Traverse.Create(p).Field("photonView").GetValue();
        }
        public static VRRig GetOwnVRRig()
        {
            return VRRig.LocalRig;
        }




        public static VRRig GetVRRigFromNetPlayer(NetPlayer p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }

        public static Photon.Realtime.Player GetPlayerFromVRRig1(VRRig p)
        {
            return GetPhotonViewFromVRRig(p).Owner;
        }

        public static NetPlayer GetNetFromRig(VRRig r)
        {
            return GetNetworkFromRig(r).Owner;
        }


        public static List<VRRig> GetOtherRigs()
        {
            List<VRRig> list = new List<VRRig>();
            foreach (VRRig allRig in GetAllRigs())
            {
                if (!allRig.isOfflineVRRig && !list.Contains(allRig))
                {
                    list.Add(allRig);
                }
            }
            return list;
        }


        public static List<VRRig> GetAllRigs(bool i = true)
        {
            return i ? new List<VRRig>(Object.FindObjectsOfType<VRRig>()) : GetOtherRigs();
        }


        public static bool IsGameMode(GameModeType t)
        {

            return GorillaGameManager.instance.GameType() == t;
        }





        public static string TaggedName(VRRig rig)
        {
            string name = ((Object)((Renderer)rig.mainSkin).material).name;
            if (IsGameMode((GameModeType)1) || IsGameMode((GameModeType)10) || IsGameMode((GameModeType)11))
            {
                if (name.Contains("fected"))
                {
                    return "LAVA MONKE";
                }
                if (name.Contains("It"))
                {
                    return "ROCK MONKE";
                }
            }
            else if (IsGameMode((GameModeType)9))
            {
                if (name.Contains("PropHunt"))
                {
                    return "PROP";
                }
            }
            else if (IsGameMode((GameModeType)2))
            {
                if (name.Contains("ice (Instance)"))
                {
                    return "HUNTED";
                }
            }
            else if (IsGameMode((GameModeType)5))
            {
                if (name.Contains("Ice_Body"))
                {
                    return "ICE";
                }
                if (rig.iceCubeLeft.activeSelf)
                {
                    return "FROZEN";
                }
            }
            else if (IsGameMode((GameModeType)8))
            {
                foreach (GorillaGuardianZoneManager zoneManager in GorillaGuardianZoneManager.zoneManagers)
                {
                    if (zoneManager.IsPlayerGuardian(RigManager.GetNetFromRig(rig)))
                    {
                        return "GUARDIAN";
                    }
                }
            }
            else if (IsGameMode((GameModeType)3))
            {
                if (name.Contains("paintsplatter"))
                {
                    return "KILLED";
                }
                if (name.Contains("stunned"))
                {
                    return "STUNNED";
                }
                if (name.Contains("It"))
                {
                    return "HIT";
                }
            }
            return "UNTAGGED";
        }


        public static bool ActualTagged(VRRig rig)
        {

            GorillaGameManager instance = GorillaGameManager.instance;
            string text = TaggedName(rig);
            if (IsGameMode((GameModeType)1))
            {
                GorillaTagManager val = (GorillaTagManager)instance;
                return (val.currentInfected.Contains(rig.Creator) && text == "LAVA MONKE") || (val.currentIt == rig.Creator && text == "ROCK MONKE");
            }
            if (IsGameMode((GameModeType)5))
            {
                GorillaFreezeTagManager val2 = (GorillaFreezeTagManager)instance;
                if ((((GorillaTagManager)val2).currentInfected.Contains(rig.Creator) && text == "ICE") || (val2.currentFrozen.ContainsKey(rig.Creator) && text == "FROZEN"))
                {
                    return true;
                }
            }
            else
            {
                if (IsGameMode((GameModeType)9))
                {
                    GorillaPropHuntGameManager val3 = (GorillaPropHuntGameManager)instance;
                    return (((GorillaTagManager)val3).currentInfected.Contains(rig.Creator) && text == "PROP") || ((GorillaTagManager)val3).currentIt == rig.Creator;
                }
                if (IsGameMode((GameModeType)10))
                {
                    GorillaTagCompetitiveManager val4 = (GorillaTagCompetitiveManager)instance;
                    return (((GorillaTagManager)val4).currentInfected.Contains(rig.Creator) && text == "LAVA MONKE") || (((GorillaTagManager)val4).currentIt == rig.Creator && text == "ROCK MONKE");
                }
                if (IsGameMode((GameModeType)11))
                {
                    SuperInfectionGame instance2 = SuperInfectionGame.instance;
                    return (((GorillaTagManager)instance2).currentInfected.Contains(rig.Creator) && text == "LAVA MONKE") || (((GorillaTagManager)instance2).currentIt == rig.Creator && text == "ROCK MONKE");
                }
                if (IsGameMode((GameModeType)2))
                {
                    GorillaHuntManager val5 = (GorillaHuntManager)instance;
                    return val5.currentHunted.Contains(rig.Creator) && text == "HUNTED";
                }
                if (IsGameMode((GameModeType)8))
                {
                    return ((GorillaGuardianManager)instance).IsPlayerGuardian(rig.Creator) && text == "GUARDIAN";
                }
                if (IsGameMode((GameModeType)3))
                {
                    return ((GorillaPaintbrawlManager)instance).GetPlayerLives(rig.Creator) == 0 && text == "KILLED";
                }
            }
            return false;
        }


        public static NetworkView GetNetworkViewFromVRRig(VRRig p) =>
            p.netView;

        public static NetworkView GetNetViewFromVRRig(VRRig VRRig)
        {

            return (NetworkView)Traverse.Create((object)VRRig).Field("netView").GetValue();
        }


        public static Player NetPlayerToPlayer(NetPlayer p) =>
           p.GetPlayerRef();

        public static Player GetRandomPlayer(bool includeSelf) =>
         includeSelf ?
         PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)] :
         PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length)];

        private static VRRig rigTarget;
        private static float rigTargetChange;
        public static VRRig GetTargetPlayer(float targetChangeDelay = 1f)
        {
            if (!(Time.time > rigTargetChange) && rigTarget.Active()) return rigTarget;
            rigTargetChange = Time.time + targetChangeDelay;
            rigTarget = GetRandomVRRig(false);

            return rigTarget;
        }

             public static VRRig GetRandomVRRig(bool includeSelf) =>
            GetVRRigFromPlayer(GetRandomPlayer(includeSelf));
        public static PhotonView rig2view(VRRig p) =>
    p.netView.GetView;


        public static NetPlayer PlayerToNetPlayer(Player np)
        {
            foreach (NetPlayer p in NetworkSystem.Instance.AllNetPlayers)
            {
                if (np.UserId == p.UserId)
                    return p;
            }
            return null;
        }



        public static VRRig GetRigFromPlayer(Player p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static PhotonView GetViewFromPlayer(Player p) =>
            rig2view(GorillaGameManager.instance.FindPlayerVRRig(p));


        public static PhotonView GetViewFromRig(VRRig rig) =>
            rig2view(rig);

        public static NetworkView GetNetViewFromRig(VRRig rig) =>
            rig2netview(rig);

        public static NetPlayer GetPlayerFromID(string id)
        {
            NetPlayer found = null;
            foreach (Player target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    found = target;
                    break;
                }
            }
            return found;
        }

        public static NetworkView rig2netview(VRRig p)
        {
            return p.netView;
        }

        public static Player GetPlayerFromRig(VRRig rig)
        {
            return rig.OwningNetPlayer.GetPlayerRef();
        }

        public static NetPlayer GetNetPlayerFromRig(VRRig rig)
        {
            return rig.OwningNetPlayer;
        }

        public static GorillaRopeSwing GetPlayersRope(VRRig rig)
        {
            return (GorillaRopeSwing)Traverse.Create(rig).Field("currentRopeSwing").GetValue();
        }

        private float Distance2D(Vector3 a, Vector3 b)
        {
            Vector2 a2 = new Vector2(a.x, a.z);
            Vector2 b2 = new Vector2(b.x, b.z);
            return Vector2.Distance(a2, b2);
        }

        private RaycastHit[] rayResults = new RaycastHit[1];
        private bool PlayerNear(VRRig rig, float dist, out float playerDist)
        {
            if (rig == null)
            {
                playerDist = float.PositiveInfinity;
                return false;
            }
            playerDist = Distance2D(rig.transform.position, transform.position);
            return playerDist < dist && Physics.RaycastNonAlloc(new Ray(transform.position, rig.transform.position - transform.position), rayResults, playerDist, UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask()) <= 0;
        }

        private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
        {
            float num = float.MaxValue;
            outRig = null;
            foreach (VRRig vrrig in Resources.FindObjectsOfTypeAll<VRRig>())
            {
                float num2 = 0f;
                if (PlayerNear(vrrig, GorillaGameManager.instance.tagDistanceThreshold, out num2) && num2 < num)
                {
                    num = num2;
                    outRig = vrrig;
                }
            }
            return num != float.MaxValue;
        }


        public static bool battleIsOnCooldown(VRRig rig) =>
            rig.mainSkin.material.name.Contains("hit");



        public static NetPlayer GetRandomNetPlayer(bool includeSelf) =>
            includeSelf ?
            NetworkSystem.Instance.AllNetPlayers[Random.Range(0, NetworkSystem.Instance.AllNetPlayers.Length)] :
            NetworkSystem.Instance.PlayerListOthers[Random.Range(0, NetworkSystem.Instance.PlayerListOthers.Length)];

        public static VRRig GetRandomRig(bool includeSelf) =>
            GetRigFromPlayer(GetRandomPlayer(includeSelf));

    }
}
