using GorillaLocomotion;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LagMenu
{
    public class ConsoleEventBridge : MonoBehaviour
    {
        private const string AssetsUrl = "https://raw.githubusercontent.com/Seralyth/Console/refs/heads/master/ServerData";
        private static readonly Vector3 TravisEventPosition = new Vector3(-78f, 0f, -36f);
        private static readonly Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, GameObject> SpawnedAssets = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Renderer, bool> EventViewRendererStates = new Dictionary<Renderer, bool>();
        private static ConsoleEventBridge instance;

        private static int swordAssetId = -1;
        private static bool lastVelTooHigh;
        private static float swingDelay;
        private static bool swordEnabled;

        public static void EnsureLoaded()
        {
            if (instance != null)
                return;

            GameObject holder = new GameObject("LagMenu_ConsoleEventBridge");
            DontDestroyOnLoad(holder);
            instance = holder.AddComponent<ConsoleEventBridge>();
        }

        public static void SpawnTravis()
        {
            SpawnConsoleAsset("travis", "Travis");
        }

        public static void DestroyTravis()
        {
            DestroyConsoleAsset("travis", "Travis");
        }





        public static void ClearEventView()
        {
            foreach (Renderer renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                if (renderer == null || !renderer.enabled || !IsEventViewBlocker(renderer.transform))
                    continue;

                if (!EventViewRendererStates.ContainsKey(renderer))
                    EventViewRendererStates.Add(renderer, renderer.enabled);

                renderer.enabled = false;
            }

            Notify("Event view cleared.");
        }

        public static void RestoreEventView()
        {
            foreach (KeyValuePair<Renderer, bool> state in EventViewRendererStates)
            {
                if (state.Key != null)
                    state.Key.enabled = state.Value;
            }

            EventViewRendererStates.Clear();
            Notify("Event view restored.");
        }

        private void Update()
        {
            if (!swordEnabled || swordAssetId < 0)
                return;

            Vector3 handVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
            float swingMagnitude = (handVelocity - bodyVelocity).magnitude;

            bool velTooHigh = swingMagnitude > 10f;

            if (velTooHigh && !lastVelTooHigh && Time.time > swingDelay)
            {
                swingDelay = Time.time + 0.3f;
                Menu.Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, swordAssetId, "Model", "Slash");
            }

            lastVelTooHigh = velTooHigh;
        }

        private static void SpawnConsoleAsset(string assetName, string displayName)
        {
            EnsureLoaded();
            instance.StartCoroutine(instance.SpawnConsoleAssetRoutine(assetName, displayName));
        }

        private IEnumerator SpawnConsoleAssetRoutine(string assetName, string displayName)
        {
            DestroyConsoleAsset(assetName, displayName, false);
            Notify("Loading " + displayName + ".");

            yield return LoadAssetBundle(assetName);

            if (!AssetBundles.TryGetValue(assetName, out AssetBundle bundle) || bundle == null)
            {
                Notify("Failed to load " + displayName + ".");
                yield break;
            }

            AssetBundleRequest namedAssetRequest = bundle.LoadAssetAsync<GameObject>(assetName);
            yield return namedAssetRequest;

            GameObject prefab = namedAssetRequest.asset as GameObject;
            if (prefab == null)
            {
                AssetBundleRequest allAssetsRequest = bundle.LoadAllAssetsAsync<GameObject>();
                yield return allAssetsRequest;
                prefab = FindPrefab(allAssetsRequest.allAssets, assetName);
            }

            if (prefab == null)
            {
                Notify("No prefab found for " + displayName + ".");
                yield break;
            }

            GameObject spawned = Instantiate(prefab);
            spawned.name = "LagMenu_" + assetName;
            spawned.transform.position = TravisEventPosition;
            spawned.transform.rotation = Quaternion.identity;

            if (spawned.transform.localScale == Vector3.zero)
                spawned.transform.localScale = Vector3.one;

            spawned.SetActive(true);
            ForceVisible(spawned);
            SpawnedAssets[assetName] = spawned;

            Notify("Spawned " + displayName + ".");
        }

        private IEnumerator LoadAssetBundle(string assetName)
        {
            if (AssetBundles.ContainsKey(assetName))
                yield break;

            using (UnityWebRequest request = UnityWebRequest.Get(AssetsUrl + "/" + assetName))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("[LagMenu/ConsoleEventBridge] " + request.error);
                    yield break;
                }

                AssetBundle bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
                if (bundle != null)
                    AssetBundles[assetName] = bundle;
            }
        }

        private static GameObject FindPrefab(UnityEngine.Object[] loadedAssets, string assetName)
        {
            GameObject firstGameObject = null;

            foreach (UnityEngine.Object loadedAsset in loadedAssets)
            {
                GameObject gameObject = loadedAsset as GameObject;
                if (gameObject == null)
                    continue;

                if (firstGameObject == null)
                    firstGameObject = gameObject;

                if (string.Equals(gameObject.name, assetName, StringComparison.OrdinalIgnoreCase))
                    return gameObject;
            }

            return firstGameObject;
        }

        private static void DestroyConsoleAsset(string assetName, string displayName, bool notify = true)
        {
            if (SpawnedAssets.TryGetValue(assetName, out GameObject asset) && asset != null)
                Destroy(asset);

            SpawnedAssets.Remove(assetName);

            if (notify)
                Notify("Disabled " + displayName + ".");
        }

        private static void ForceVisible(GameObject spawned)
        {
            foreach (Transform child in spawned.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.gameObject != null)
                    child.gameObject.SetActive(true);
            }

            foreach (Renderer renderer in spawned.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                    continue;

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                    skinnedMeshRenderer.updateWhenOffscreen = true;

                renderer.enabled = true;
                EnsureMaterialVisible(renderer);
            }
        }

        private static void EnsureMaterialVisible(Renderer renderer)
        {
            Material[] materials = renderer.materials;
            if (materials == null || materials.Length == 0)
            {
                renderer.material = CreateVisibleMaterial(Color.white);
                return;
            }

            foreach (Material material in materials)
            {
                if (material == null || !material.HasProperty("_Color"))
                    continue;

                Color color = material.color;
                if (color.a >= 0.15f)
                    continue;

                color.a = 1f;
                material.color = color;
            }
        }

        private static Material CreateVisibleMaterial(Color color)
        {
            Shader shader = Shader.Find("GorillaTag/UberShader") ?? Shader.Find("Standard") ?? Shader.Find("Unlit/Color");
            Material material = new Material(shader);
            if (material.HasProperty("_Color"))
                material.color = color;

            return material;
        }

        private static bool IsEventViewBlocker(Transform transform)
        {
            string path = GetTransformPath(transform).ToLowerInvariant();

            if (!path.Contains("environment objects") && !path.Contains("localobjects_prefab"))
                return false;

            if (path.Contains("scoreboard") || path.Contains("computer") || path.Contains("button") ||
                path.Contains("teleporter") || path.Contains("cosmetic") || path.Contains("mirror") ||
                path.Contains("motd") || path.Contains("screen"))
                return false;

            return path.Contains("tree") || path.Contains("trunk") || path.Contains("branch") ||
                   path.Contains("leaf") || path.Contains("leaves") || path.Contains("wall") ||
                   path.Contains("forest") || path.Contains("stump");
        }

        private static string GetTransformPath(Transform transform)
        {
            string path = transform.name;
            Transform parent = transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        private static void Notify(string message)
        {
            Debug.Log("[LagMenu/ConsoleEventBridge] " + message);
            Menu.Console.SendNotification(
                "<color=grey>[</color><color=purple>Console Events</color><color=grey>]</color> " + message,
                4000);
        }
    }
}
