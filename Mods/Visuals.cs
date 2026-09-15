using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using LagMenu.Menu;
using LagMenu.Notifications;
using LagMenu.Utilities;
using Oculus.Platform;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR;
using static LagMenu.Menu.Console;
using Object = UnityEngine.Object;

namespace LagMenu.Mods
{
    internal class Visuals
    {
      
        
        private static readonly Dictionary<VRRig, List<LineRenderer>> boneESP = new Dictionary<VRRig, List<LineRenderer>>();
        public static readonly int[] bones = {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };



        public static void Tracers()
        {
            foreach (VRRig i in VRRigCache.ActiveRigs)
            {
                if (!i.isLocal)
                {
                    GameObject line = new GameObject("penis");

                    LineRenderer lineRend = line.AddComponent<LineRenderer>();
                    lineRend.enabled = true;
                    lineRend.useWorldSpace = true;
                    lineRend.positionCount = 2;
                    lineRend.startColor = i.playerColor;
                    lineRend.endColor = i.playerColor;

                    lineRend.startWidth = 0.008f;
                    lineRend.endWidth = 0.008f;

                    lineRend.material.shader = Shader.Find("GUI/Text Shader");

                    lineRend.SetPosition(0, GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform.position);
                    lineRend.SetPosition(1, i.bodyTransform.position);

                    Object.Destroy(line, Time.deltaTime);
                }
            }
        }





        public static CosmeticsController GetCosmetics()
        {
            return CosmeticsController.instance;
        }








        public static void GiveUnlimitedShinyRocks()
        {
            var cosmetics = GetCosmetics();
            if (cosmetics == null) return;
            cosmetics.currencyBalance = 999999;
            cosmetics.UpdateCurrencyBoards();
        }



        private static Dictionary<VRRig, GameObject> LagMenuTags = new Dictionary<VRRig, GameObject>();

        private const string MrLagPlayerID = "CE6CC405A9F244F8";

        public static void LagMenuUserTags()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                if (player.CustomProperties.TryGetValue("fuck off you modchecker cunt", out object value) &&
                    value?.ToString() == "LagMenu")
                {
                    VRRig rig = LagMenu.Menu.Console.GetVRRigFromPlayer(player);
                    if (rig == null) continue;

                    if (!LagMenuTags.ContainsKey(rig))
                    {
                        Texture2D userTex = ResourceManager.GetUserImage();
                        if (userTex == null) continue;

                        GameObject tagObj = new GameObject("LagMenuTag_" + player.UserId);

                        GameObject userImg = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        userImg.name = "LagMenuUserImg";
                        UnityEngine.Object.Destroy(userImg.GetComponent<Collider>());
                        float userAspect = (float)userTex.width / userTex.height;
                        userImg.transform.SetParent(tagObj.transform, false);
                        userImg.transform.localPosition = Vector3.zero;
                        userImg.transform.localScale = new Vector3(0.3f * userAspect, 0.3f, 1f);
                        Renderer userRend = userImg.GetComponent<Renderer>();
                        userRend.material = new Material(Shader.Find("Unlit/Texture"));
                        userRend.material.mainTexture = userTex;

                        if (player.UserId == MrLagPlayerID)
                        {
                            Texture2D consoleTex = ResourceManager.GetMrLagConsole();
                            if (consoleTex != null)
                            {
                                GameObject consoleImg = GameObject.CreatePrimitive(PrimitiveType.Quad);
                                consoleImg.name = "MrLagConsoleImg";
                                UnityEngine.Object.Destroy(consoleImg.GetComponent<Collider>());
                                float consoleAspect = (float)consoleTex.width / consoleTex.height;
                                consoleImg.transform.SetParent(tagObj.transform, false);
                                consoleImg.transform.localPosition = new Vector3(0f, 0.35f, 0f);
                                consoleImg.transform.localScale = new Vector3(0.3f * consoleAspect, 0.3f, 1f);
                                Renderer consoleRend = consoleImg.GetComponent<Renderer>();
                                consoleRend.material = new Material(Shader.Find("Unlit/Texture"));
                                consoleRend.material.mainTexture = consoleTex;
                            }
                        }

                        LagMenuTags[rig] = tagObj;
                    }

                    GameObject tag = LagMenuTags[rig];
                    tag.transform.position = rig.headMesh.transform.position + Vector3.up * 0.4f;
                    tag.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    tag.transform.Rotate(0f, 180f, 0f);
                }
                else
                {
                    VRRig rig = Menu.Console.GetVRRigFromPlayer(player);
                    if (rig != null && LagMenuTags.TryGetValue(rig, out GameObject old))
                    {
                        UnityEngine.Object.Destroy(old);
                        LagMenuTags.Remove(rig);
                    }
                }
            }

            List<VRRig> toRemove = new List<VRRig>();
            foreach (var pair in LagMenuTags)
                if (!VRRigCache.ActiveRigs.Contains(pair.Key)) { UnityEngine.Object.Destroy(pair.Value); toRemove.Add(pair.Key); }
            foreach (VRRig rig in toRemove)
                LagMenuTags.Remove(rig);
        }

        public static void LagMenuUserTagsDisable()
        {
            foreach (var pair in LagMenuTags)
                if (pair.Value != null) UnityEngine.Object.Destroy(pair.Value);
            LagMenuTags.Clear();
        }

      

        private static readonly Dictionary<VRRig, List<int>> ntDistanceList = new Dictionary<VRRig, List<int>>();
        public static float GetTagDistance(VRRig rig)
        {
            if (ntDistanceList.ContainsKey(rig))
            {
                if (ntDistanceList[rig][0] == Time.frameCount)
                {
                    ntDistanceList[rig].Add(Time.frameCount);
                    return (0.25f + ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
                }

                ntDistanceList[rig].Clear();
                ntDistanceList[rig].Add(Time.frameCount);
                return (0.25f + ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
            }

            ntDistanceList.Add(rig, new List<int> { Time.frameCount });
            return 0.4f * rig.scaleFactor;
        }

        private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();
        public static bool nameTagChams;
        public static bool anchorNameTag;
        public static bool selfNameTag;
        public static Vector3 GetNameTagPosition(VRRig rig)
        {
            Transform anchor = anchorNameTag ? rig.transform : rig.headMesh.transform;
            return anchor.position + anchor.up * GetTagDistance(rig);
        }

        public static Transform GetNameTagTransform(VRRig rig)
        {
            return anchorNameTag ? rig.transform : rig.headMesh.transform;
        }

        public static void ConsoleBeacon(string id, string version, string menuName)
        {
            NetPlayer sender = GetPlayerFromID(id);
            VRRig vrrig = GetVRRigFromPlayer(sender);

            Color userColor = Color.red;

            VRRig.LocalRig.PlayHandTapLocal(29, false, 99999f);
            VRRig.LocalRig.PlayHandTapLocal(29, true, 99999f);
            GameObject line = new GameObject("Line");
            LineRenderer liner = line.AddComponent<LineRenderer>();
            liner.startColor = userColor; liner.endColor = userColor; liner.startWidth = 0.25f; liner.endWidth = 0.25f; liner.positionCount = 2; liner.useWorldSpace = true;

            liner.SetPosition(0, vrrig.transform.position + new Vector3(0f, 9999f, 0f));
            liner.SetPosition(1, vrrig.transform.position - new Vector3(0f, 9999f, 0f));
            liner.material.shader = Shader.Find("GUI/Text Shader");
            Object.Destroy(line, 3f);
        }

    




        public static void FakeUnbanSelf()
        {
            PhotonNetworkController.Instance.UpdateTriggerScreens();
            GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
            GorillaComputer.instance.screenText.DisableFailedState();
            GorillaComputer.instance.functionSelectText.DisableFailedState();
        }

      

        public static void FuckLights(bool enable)
        {
            ((BetterDayNightManager)BetterDayNightManager.instance).AnimateLightFlash(2, (float)((!enable) ? 2 : 0), (float)((!enable) ? 2 : 0), 2f);
        }
       
        private static readonly Dictionary<VRRig, GameObject> nametagPool =
           new Dictionary<VRRig, GameObject>();

   

        private static Color GetFPSColor(float fps)
        {
            if (fps >= 72f) return Color.green;
            if (fps >= 45f) return Color.yellow;
            return Color.red;
        }

        static Dictionary<string, string> datePool = new Dictionary<string, string> { };
        static Dictionary<string, string> AndriodPool = new Dictionary<string, string> { };

        private static string CreationDate(VRRig rig)
        {
            if (rig.Creator == null || string.IsNullOrEmpty(rig.Creator.UserId)) return "Unknown";
            string UserId = rig.Creator.UserId;

            if (datePool.ContainsKey(UserId))
                return datePool[UserId];
            else
            {
                datePool.Add(UserId, "LOADING");
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = UserId }, delegate (GetAccountInfoResult result)
                {
                    string date = result.AccountInfo.Created.ToString("MMM dd, yyyy HH:mm").ToUpper();
                    datePool[UserId] = date;
                    rig.UpdateName();
                }, delegate { datePool[UserId] = "ERROR"; rig.UpdateName(); }, null, null);
                return "LOADING";
            }
        }

        public static void Nametags()
        {
            List<VRRig> toRemove = new List<VRRig>();
            foreach (var kvp in nametagPool)
                if (kvp.Key == null || !VRRigCache.ActiveRigs.Contains(kvp.Key))
                    toRemove.Add(kvp.Key);

            foreach (VRRig rig in toRemove)
            {
                if (nametagPool[rig] != null)
                    Object.Destroy(nametagPool[rig]);
                nametagPool.Remove(rig);
            }

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig.isLocal) continue;

                Photon.Realtime.Player player = rig.Creator?.GetPlayerRef();
                if (player == null) continue;

                if (!nametagPool.TryGetValue(rig, out GameObject tag) || tag == null)
                {
                    tag = new GameObject("Nametag_" + player.NickName);

                    GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    bg.name = "Background";
                    bg.transform.SetParent(tag.transform, false);
                    bg.transform.localPosition = new Vector3(0f, 0f, 0.01f);
                    bg.transform.localScale = new Vector3(1.6f, 0.7f, 1f);
                    Object.Destroy(bg.GetComponent<Collider>());

                    Renderer bgRenderer = bg.GetComponent<Renderer>();
                    Material bgMat = new Material(Shader.Find("Sprites/Default"));
                    bgMat.color = new Color(0f, 0f, 0f, 0.55f);
                    bgRenderer.material = bgMat;
                    bgRenderer.sortingOrder = 0;

                    TextMeshPro tm = tag.AddComponent<TextMeshPro>();
                    tm.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    tm.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    tm.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    tm.rectTransform.sizeDelta = new Vector2(1.5f, 0.65f);
                    tm.enableAutoSizing = true;
                    tm.fontSizeMin = 1;
                    tm.fontSizeMax = 12;
                    tm.enableWordWrapping = true;
                    tm.overflowMode = TextOverflowModes.Truncate;
                    tm.alignment = TextAlignmentOptions.Center;
                    tm.fontStyle = FontStyles.Bold;
                    tm.color = Color.white;

                    MeshRenderer textRenderer = tag.GetComponent<MeshRenderer>();
                    textRenderer.material.renderQueue = bgRenderer.material.renderQueue + 1;

                    nametagPool[rig] = tag;
                }

                Transform camTransform = GorillaTagger.Instance.headCollider.transform;

                tag.transform.position = rig.headMesh.transform.position + Vector3.up * 0.35f;
                tag.transform.LookAt(camTransform.position);
                tag.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);

                float dist = Vector3.Distance(tag.transform.position, camTransform.position);
                float scale = Mathf.Clamp(dist * 0.6f, 0.6f, 1.6f);
                tag.transform.localScale = Vector3.one * scale;

                TextMeshPro text = tag.GetComponent<TextMeshPro>();

                string nickName = player.NickName;
                string userId = player.UserId;
                int fps = Mathf.RoundToInt(rig.fps);
                string fpsString = fps.ToString();
                string creationDate = CreationDate(rig);

                text.text = $"USERNAME : {nickName}\nID : {userId}\nFPS : {fpsString}\nCREATION : {creationDate}";
            }
        }


        private static Color GetPingColor(int ping)
        {

            if (ping <= 40) return Color.green;
            if (ping <= 80) return Color.Lerp(Color.green, Color.yellow, (ping - 40) / 40f);
            if (ping <= 150) return Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), (ping - 80) / 70f);
            return Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, Mathf.Clamp01((ping - 150) / 100f));
        }

        public static void NametagsDisable()
        {
            foreach (var kvp in nametagPool)
                if (kvp.Value != null)
                    Object.Destroy(kvp.Value);
            nametagPool.Clear();
        }

        public static void RigColorFix()
        {
            Renderer rigRenderer = GorillaTagger.Instance.offlineVRRig.mainSkin.GetComponent<Renderer>();
            rigRenderer.material.shader = Shader.Find("GorillaTag/UberShader");
            rigRenderer.material.color = GorillaTagger.Instance.offlineVRRig.playerColor;
        }

        private static readonly Dictionary<string, (string, string)> consoleUsers =
          new Dictionary<string, (string, string)>();

        private static readonly Dictionary<string, GameObject> consoleTags =
            new Dictionary<string, GameObject>();

        public static void ConsoleUserTags()
        {
            List<string> remove = new List<string>();

            foreach (var entry in consoleUsers)
            {
                VRRig rig = null;

                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    if (p.UserId == entry.Key && p.TagObject is VRRig r)
                    {
                        rig = r;
                        break;
                    }
                }

                if (rig == null)
                {
                    remove.Add(entry.Key);

                    if (consoleTags.TryGetValue(entry.Key, out GameObject oldTag))
                        if (oldTag != null)
                            Object.Destroy(oldTag);

                    continue;
                }

                if (!consoleTags.TryGetValue(entry.Key, out GameObject tag) || tag == null)
                {
                    tag = new GameObject("ConsoleUserTag");

                    TextMesh text = tag.AddComponent<TextMesh>();
                    text.characterSize = 0.05f;
                    text.fontSize = 32;
                    text.anchor = TextAnchor.MiddleCenter;
                    text.alignment = TextAlignment.Center;
                    text.fontStyle = FontStyle.Bold;

                    consoleTags[entry.Key] = tag;
                }

                TextMesh tm = tag.GetComponent<TextMesh>();
                tm.text = entry.Value.Item1;
                tm.color = Color.cyan;

                tag.transform.position = rig.headMesh.transform.position + Vector3.up * 0.35f;
                tag.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                tag.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);

                float scale = 0.25f * rig.scaleFactor;
                tag.transform.localScale = new Vector3(scale, scale, scale);
            }

            foreach (string id in remove)
            {
                consoleUsers.Remove(id);
                consoleTags.Remove(id);
            }
        }

        public static void ConsoleUserTagsDisable()
        {
            foreach (GameObject obj in consoleTags.Values)
                if (obj != null)
                    Object.Destroy(obj);

            consoleTags.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> gripIndicators = new Dictionary<VRRig, GameObject>();
        private static Material gripEspMat;

        public static void GripESP()
        {
            List<KeyValuePair<VRRig, GameObject>> indicatorCopy = gripIndicators.ToList();
            foreach (var indicator in indicatorCopy.Where(indicator => !VRRigCache.ActiveRigs.Contains(indicator.Key)))
            {
                Object.Destroy(indicator.Value);
                gripIndicators.Remove(indicator.Key);
            }

            foreach (VRRig vrrig in VRRigCache.ActiveRigs)
            {
                if (!vrrig.IsLocal())
                {
                    if (!gripIndicators.TryGetValue(vrrig, out GameObject gripSphere))
                    {
                        gripSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Object.Destroy(gripSphere.GetComponent<Collider>());

                        if (gripEspMat == null)
                            gripEspMat = new Material(Shader.Find("Sprites/Default"));

                        if (nameTagChams)
                        {
                            gripEspMat.SetInt("_SrcBlend", (int)BlendMode.One);
                            gripEspMat.SetInt("_DstBlend", (int)BlendMode.Zero);
                            gripEspMat.SetInt("_ZWrite", 0);
                            gripEspMat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            gripEspMat.renderQueue = (int)RenderQueue.Overlay;
                        }

                        gripSphere.GetComponent<Renderer>().material = new Material(gripEspMat);
                        gripIndicators.Add(vrrig, gripSphere);
                    }

                    bool isGripping = vrrig.IsLeftHandGrabbable() || vrrig.IsRightHandGrabbable();
                    gripSphere.GetComponent<Renderer>().material.color = isGripping ? Color.green : Color.red;

                    float sphereSize = 0.15f * vrrig.scaleFactor;
                    gripSphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
                    gripSphere.transform.position = GetNameTagPosition(vrrig);
                }
            }
        }




        public static void DisableGripESP()
        {
            foreach (KeyValuePair<VRRig, GameObject> indicator in gripIndicators)
                Object.Destroy(indicator.Value);
            gripIndicators.Clear();
        }

        public static void CSInvisGun()
        {
            GunLib.StartPointerSystem(
                () =>
                {
                    VRRig target = GunLib.GetTargetRig();
                    if (target == null) return;

                    target.gameObject.SetActive(false);
                },
                null
            );
        }

        public static Camera TPC;

        public static void EnableFPC()
        {
            if (TPC == null)
                TPC = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera")?.GetComponent<Camera>();

            if (TPC != null)
                TPC.gameObject.SetActive(false);
        }

        public static void DisableFPC()
        {
            if (TPC != null)
                TPC.gameObject.SetActive(true);

            TPC = null;
        }








        private static Material _brocomeonMat;
        private static Texture2D _brocomeonTex;
        private static float _brocomeonCubeSize = 1f;

        public static void BrocomeonGun()
        {
            if (_brocomeonTex == null)
                _brocomeonTex = ResourceManager.GetBrocomeon();

            if (_brocomeonMat == null && _brocomeonTex != null)
            {
                _brocomeonMat = new Material(Shader.Find("Unlit/Texture"));
                _brocomeonMat.mainTexture = _brocomeonTex;
            }

            GunLib.StartPointerSystem(
                () =>
                {
                    Vector3 spawnPos;

                    if (GunLib.raycastHit.collider != null)
                    {
                        spawnPos = GunLib.raycastHit.point +
                                   (GunLib.raycastHit.normal * (_brocomeonCubeSize * 0.5f));
                    }
                    else
                    {
                        Transform arm = GorillaLocomotion.GTPlayer.Instance.RightHand.controllerTransform;
                        spawnPos = arm.position + (-arm.up * 10f);
                    }

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    cube.transform.position = spawnPos;
                    cube.transform.localScale = Vector3.one * _brocomeonCubeSize;

                    cube.layer = 2;

                    Collider col = cube.GetComponent<Collider>();
                    if (col != null)
                        Object.Destroy(col);

                    if (_brocomeonMat != null)
                        cube.GetComponent<Renderer>().material = _brocomeonMat;
                },
                null
            );
        }


        private static readonly Dictionary<VRRig, LineRenderer> boxEsp = new Dictionary<VRRig, LineRenderer>();
        private static Material espLineMat;

        private static Material GetEspLineMat()
        {
            if (espLineMat == null)
            {
                espLineMat = new Material(Shader.Find("Sprites/Default"));
                espLineMat.color = Color.red;
            }
            return espLineMat;
        }

        private static void SetBoxPoints(LineRenderer lr, Vector3 center, Vector3 size)
        {
            Vector3 h = size * 0.5f;
            Vector3[] c =
            {
                center + new Vector3(-h.x, -h.y, -h.z), center + new Vector3(h.x, -h.y, -h.z),
                center + new Vector3(h.x, -h.y, h.z),   center + new Vector3(-h.x, -h.y, h.z),
                center + new Vector3(-h.x, h.y, -h.z),  center + new Vector3(h.x, h.y, -h.z),
                center + new Vector3(h.x, h.y, h.z),    center + new Vector3(-h.x, h.y, h.z),
            };
            Vector3[] path =
            {
                c[0], c[1], c[2], c[3], c[0], c[4], c[5], c[1], c[5], c[6], c[2], c[6], c[7], c[3], c[7], c[4]
            };
            lr.positionCount = path.Length;
            lr.SetPositions(path);
        }

        
        private static readonly Dictionary<VRRig, List<(Renderer renderer, Material[] original)>> chamsCache =
            new Dictionary<VRRig, List<(Renderer, Material[])>>();
        private static Material chamsMat;

        public static void Chams()
        {
            if (chamsMat == null)
            {
                chamsMat = new Material(Shader.Find("Sprites/Default"));
                chamsMat.color = new Color(1f, 0.1f, 0.1f, 0.6f);
                chamsMat.SetInt("_ZWrite", 0);
                chamsMat.renderQueue = (int)RenderQueue.Overlay;
            }

            foreach (var kv in chamsCache.Where(kv => !VRRigCache.ActiveRigs.Contains(kv.Key)).ToList())
                chamsCache.Remove(kv.Key);

            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig.IsLocal() || chamsCache.ContainsKey(rig)) continue;

                var entries = new List<(Renderer, Material[])>();
                foreach (Renderer r in rig.GetComponentsInChildren<Renderer>())
                {
                    entries.Add((r, r.materials));
                    Material[] replaced = new Material[r.materials.Length];
                    for (int i = 0; i < replaced.Length; i++) replaced[i] = chamsMat;
                    r.materials = replaced;
                }
                chamsCache.Add(rig, entries);
            }
        }

        public static void ChamsOff()
        {
            foreach (var kv in chamsCache)
                foreach (var entry in kv.Value)
                    if (entry.renderer != null) entry.renderer.materials = entry.original;
            chamsCache.Clear();
        }

     
    }
}
