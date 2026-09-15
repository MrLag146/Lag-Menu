using GorillaLocomotion;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    public static class ConsoleAssetss
    {
        private const string AssetsUrl = "https://raw.githubusercontent.com/Seralyth/Console/refs/heads/master/ServerData";
        private const string HamburburAssetsUrl = "https://raw.githubusercontent.com/hamburbur-org/Console/refs/heads/master/ServerData";
       private const string LagMenuAssetsUrl = "https://raw.githubusercontent.com/hamburbur-org/Console/refs/heads/master/ServerData";
        private static readonly string[] VideoLinks =
        {
            "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/REmZhFKmOmo.mp4",
            "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/Playboi%20Cart%20-%20Sky.mp4",
            "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/monkeys_dancing.mp4",
            "https://drive.iidk.online/resources/iidk/shiba%20youtube.mp4",
            "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/hamburger.mp4",
        };

        public static readonly string[] EffectNames = { "Zoom Body Trail", "Ares Body Trail" };

        public static readonly string[] ConcertVideoNames =
        {
            "MOJO JOJO", "New Tank", "CRANK", "Over", "POP OUT", "OPM BABI",
            "Long Time", "Punk Monk", "R.I.P. Fredo (Notice Me)", "Foreign",
            "Sky", "FINE SHIT", "JumpOutTheHouse", "Lean 4 Real",
            "DIAMONDS SPECIAL", "RADAR", "Mileage", "Rockstar Made",
            "SOME MORE", "I SEEEE YOU BABY BOI", "DRUGS GOT ME NUMB",
            "OLYMPIAN", "F33l Lik3 Dyin", "BACKD00R",
        };

        public static int flashTrailIndex = 0;
        public static int concertVideoIndex = 0;

        private static readonly Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, GameObject> SpawnedAssets = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

        private static CoroutineRunner _runner;
        private static CoroutineRunner Runner
        {
            get
            {
                if (_runner != null) return _runner;
                GameObject go = new GameObject("LagMenu_ConsoleAssetRunner");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _runner = go.AddComponent<CoroutineRunner>();
                return _runner;
            }
        }

        private static void Spawn(string assetName, string displayName, Action<GameObject> onSpawned = null, bool useHamburbur = false, bool useLagMenu = false)
        {
            string url = useHamburbur ? HamburburAssetsUrl : useLagMenu ? LagMenuAssetsUrl : AssetsUrl;
            Runner.Run(SpawnRoutine(assetName, displayName, url, onSpawned));
        }

        private static void DestroyAsset(string assetName)
        {
            if (SpawnedAssets.TryGetValue(assetName, out GameObject asset) && asset != null)
                UnityEngine.Object.Destroy(asset);
            SpawnedAssets.Remove(assetName);
        }

        private static IEnumerator SpawnRoutine(string assetName, string displayName, string assetUrl, Action<GameObject> onSpawned)
        {
            DestroyAsset(assetName);

            if (!AssetBundles.ContainsKey(assetName))
            {
                using (UnityWebRequest request = UnityWebRequest.Get(assetUrl + "/" + assetName))
                {
                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("[ConsoleAssets] Failed to load " + assetName + ": " + request.error);
                        yield break;
                    }

                    AssetBundle bundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
                    if (bundle != null)
                        AssetBundles[assetName] = bundle;
                }
            }

            if (!AssetBundles.TryGetValue(assetName, out AssetBundle ab) || ab == null)
                yield break;

            AssetBundleRequest req = ab.LoadAssetAsync<GameObject>(assetName);
            yield return req;

            GameObject prefab = req.asset as GameObject;
            if (prefab == null)
            {
                AssetBundleRequest allReq = ab.LoadAllAssetsAsync<GameObject>();
                yield return allReq;
                foreach (UnityEngine.Object obj in allReq.allAssets)
                {
                    if (obj is GameObject go)
                    {
                        prefab = go;
                        if (string.Equals(go.name, assetName, StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                }
            }

            if (prefab == null) yield break;

            GameObject spawned = UnityEngine.Object.Instantiate(prefab);
            spawned.name = "LagMenu_" + assetName;
            if (spawned.transform.localScale == Vector3.zero)
                spawned.transform.localScale = Vector3.one;
            spawned.SetActive(true);
            ForceVisible(spawned);
            SpawnedAssets[assetName] = spawned;

            onSpawned?.Invoke(spawned);
        }

        private static void ForceVisible(GameObject go)
        {
            foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
                if (t != null && t.gameObject != null)
                    t.gameObject.SetActive(true);

            foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null) continue;
                if (r is SkinnedMeshRenderer smr) smr.updateWhenOffscreen = true;
                r.enabled = true;
                foreach (Material m in r.materials)
                {
                    if (m == null || !m.HasProperty("_Color")) continue;
                    Color c = m.color;
                    if (c.a < 0.15f) { c.a = 1f; m.color = c; }
                }
            }
        }

        public static void SpawnIPhone() => Spawn("iphone", "iPhone", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            VideoPlayer vp = go.GetComponentInChildren<VideoPlayer>();
            if (vp != null) vp.url = VideoLinks[Random.Range(0, VideoLinks.Length)];
        });

        public static void DestroyIPhone() => DestroyAsset("iphone");

        public static void SpawnTravis() => Spawn("travis", "Travis", go =>
        {
            go.transform.position = new Vector3(-70f, 2f, -52f);
            go.transform.localScale = Vector3.one * 0.38f;
        });

        public static void DestroyTravis() => DestroyAsset("travis");

        public static void SpawnMiniTravis() => Spawn("minitravis", "Mini Travis", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform, false);
            go.transform.localPosition = new Vector3(-0.6f, 0.2f, 0f);
            go.transform.localRotation = Quaternion.Euler(80f, 160f, 180f);
        });

        public static void DestroyMiniTravis() => DestroyAsset("minitravis");

        public static void SpawnFlashEffects() => Spawn("flasheffects", EffectNames[flashTrailIndex], go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.bodyTransform, false);
            go.transform.localPosition = Vector3.zero;
        });

        public static void DestroyFlashEffects() => DestroyAsset("flasheffects");

        public static void SpawnConcert()
        {
            bool inForest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest")?.activeInHierarchy ?? false;
            Vector3 position = inForest ? new Vector3(-27f, 2.4f, -49.9f) : new Vector3(-28.4873f, 15.5272f, -117.8634f);
            Quaternion rotation = inForest ? Quaternion.Euler(0f, 250f, 0f) : Quaternion.Euler(0f, 300f, 0f);
            Vector3 scale = inForest ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.8f, 0.8f, 0.8f);

            Spawn("concert", "Concert", go =>
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.transform.localScale = scale;
                Transform targetPhoto = go.transform.Find("stage/Targetphoto");
                if (targetPhoto != null) UnityEngine.Object.Destroy(targetPhoto.gameObject);
                AudioSource audio = go.transform.Find("audio")?.GetComponent<AudioSource>();
                if (audio != null)
                {
                    AudioClip clip = AssetBundles["concert"].LoadAsset<AudioClip>(ConcertVideoNames[concertVideoIndex]);
                    if (clip != null) { audio.clip = clip; audio.Play(); }
                }
            });
        }

        public static void DestroyConcert() => DestroyAsset("concert");

        public static void SpawnDonationNuke() => Spawn("donationnuke", "Donation Nuke", go =>
        {
            go.transform.position = new Vector3(-64.16f, 2.99f, -82.07f);
            AudioSource nuke = go.transform.Find("nuke")?.GetComponent<AudioSource>();
            if (nuke != null)
            {
                AudioClip clip = AssetBundles["donationnuke"].LoadAsset<AudioClip>("nukesound");
                if (clip != null) { nuke.clip = clip; nuke.Play(); }
            }
        });

        public static void DestroyDonationNuke() => DestroyAsset("donationnuke");

        public static void SpawnDonationNukeH() => Spawn("donationnuke", "Donation Nuke", go =>
        {
            go.transform.position = new Vector3(-64.16f, 2.99f, -82.07f);
            AudioSource nuke = go.transform.Find("nuke")?.GetComponent<AudioSource>();
            if (nuke != null)
            {
                AudioClip clip = AssetBundles["donationnuke"].LoadAsset<AudioClip>("nukesound");
                if (clip != null) { nuke.clip = clip; nuke.Play(); }
            }
        }, useHamburbur: true);

        public static void DestroyDonationNukeH() => DestroyAsset("donationnuke");

        private static float hamburburNextPlayTime;

        public static void SpawnHamburbur() => Spawn("effects", "Hamburgur", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            AudioSource sound = go.transform.Find("Sound")?.GetComponent<AudioSource>();
            if (sound != null)
            {
                AudioClip clip = AssetBundles["effects"].LoadAsset<AudioClip>("canihaveachezburger");
                if (clip != null) { sound.clip = clip; sound.Play(); }
            }
        });

        public static void UpdateHamburbur()
        {
            if (Time.time < hamburburNextPlayTime || !SpawnedAssets.ContainsKey("effects")) return;
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.4f)
                {
                    AudioSource sound = SpawnedAssets["effects"].transform.Find("Sound")?.GetComponent<AudioSource>();
                    if (sound != null)
                    {
                        AudioClip clip = AssetBundles["effects"].LoadAsset<AudioClip>("mmmchezburger");
                        if (clip != null) sound.PlayOneShot(clip);
                    }
                    break;
                }
            }
            hamburburNextPlayTime = Time.time + 2f;
        }

        public static void DestroyHamburbur() => DestroyAsset("effects");

        private static bool pistolWasShooting;

        public static void SpawnPistol() => Spawn("console.main1", "Pistol", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        });

        public static void UpdatePistol()
        {
            if (!SpawnedAssets.ContainsKey("console.main1")) return;
            bool shooting = ControllerInputPoller.instance.rightControllerIndexFloat > 0.7f;
            if (shooting && !pistolWasShooting)
            {
                AudioSource src = SpawnedAssets["console.main1"].transform.Find("Model")?.GetComponent<AudioSource>();
                if (src != null)
                {
                    AudioClip clip = AssetBundles["console.main1"].LoadAsset<AudioClip>("PistolShoot");
                    if (clip != null) src.PlayOneShot(clip);
                }
                Animator anim = SpawnedAssets["console.main1"].transform.Find("Model")?.GetComponent<Animator>();
                anim?.Play("Shoot");
            }
            else if (!shooting && pistolWasShooting)
            {
                Animator anim = SpawnedAssets["console.main1"].transform.Find("Model")?.GetComponent<Animator>();
                anim?.Play("Default");
            }
            pistolWasShooting = shooting;
        }

        public static void DestroyPistol() => DestroyAsset("console.main1");

        private static bool swordEnabled;
        private static bool lastVelTooHigh;
        private static float swingDelay;

        public static void SpawnSword()
        {
            if (swordEnabled) return;
            Spawn("console.main1", "Sword", go =>
            {
                go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                AudioSource src = go.GetComponentInChildren<AudioSource>();
                if (src != null)
                {
                    AudioClip clip = AssetBundles["console.main1"].LoadAsset<AudioClip>("Unsheath");
                    if (clip != null) src.PlayOneShot(clip);
                }
            });
            swordEnabled = true;
            lastVelTooHigh = false;
            swingDelay = 0f;
        }

        public static void UpdateSword()
        {
            if (!swordEnabled || !SpawnedAssets.ContainsKey("console.main1")) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool velTooHigh = (handVel - bodyVel).magnitude > 10f;
            if (velTooHigh && !lastVelTooHigh && Time.time > swingDelay)
            {
                swingDelay = Time.time + 0.3f;
                AudioSource src = SpawnedAssets["console.main1"].GetComponentInChildren<AudioSource>();
                if (src != null)
                {
                    AudioClip clip = AssetBundles["console.main1"].LoadAsset<AudioClip>("Slash");
                    if (clip != null) src.PlayOneShot(clip);
                }
            }
            lastVelTooHigh = velTooHigh;
        }

        public static void DestroySword()
        {
            DestroyAsset("console.main1");
            swordEnabled = false;
            lastVelTooHigh = false;
            swingDelay = 0f;
        }

        private static bool banHammerWasShooting;

        public static void SpawnBanHammer() => Spawn("banhammer", "Ban Hammer", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }, useHamburbur: true);

        public static void UpdateBanHammer()
        {
            if (!SpawnedAssets.TryGetValue("banhammer", out GameObject hammer) || hammer == null) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool swinging = (handVel - bodyVel).magnitude > 8f;
            if (swinging && !banHammerWasShooting)
            {
                AudioSource src = hammer.GetComponentInChildren<AudioSource>();
                if (src != null) src.Play();
            }
            banHammerWasShooting = swinging;
        }

        public static void DestroyBanHammer() => DestroyAsset("banhammer");

        private static bool rbSwordEnabled;
        private static bool rbSwordLastVelTooHigh;
        private static float rbSwordSwingDelay;

        public static void SpawnRbSword()
        {
            if (rbSwordEnabled) return;
            Spawn("rbsword", "RB Sword", go =>
            {
                go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                AudioSource src = go.GetComponentInChildren<AudioSource>();
                if (src != null)
                {
                    AudioClip clip = AssetBundles["rbsword"].LoadAsset<AudioClip>("Unsheath");
                    if (clip != null) src.PlayOneShot(clip);
                }
            }, useHamburbur: true);
            rbSwordEnabled = true;
            rbSwordLastVelTooHigh = false;
            rbSwordSwingDelay = 0f;
        }

        public static void UpdateRbSword()
        {
            if (!rbSwordEnabled || !SpawnedAssets.ContainsKey("rbsword")) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool velTooHigh = (handVel - bodyVel).magnitude > 10f;
            if (velTooHigh && !rbSwordLastVelTooHigh && Time.time > rbSwordSwingDelay)
            {
                rbSwordSwingDelay = Time.time + 0.3f;
                AudioSource src = SpawnedAssets["rbsword"].GetComponentInChildren<AudioSource>();
                if (src != null)
                {
                    AudioClip clip = AssetBundles["rbsword"].LoadAsset<AudioClip>("Slash");
                    if (clip != null) src.PlayOneShot(clip);
                }
            }
            rbSwordLastVelTooHigh = velTooHigh;
        }

        public static void DestroyRbSword()
        {
            DestroyAsset("rbsword");
            rbSwordEnabled = false;
            rbSwordLastVelTooHigh = false;
            rbSwordSwingDelay = 0f;
        }

        public static void SpawnBaldi() => Spawn("baldi", "Baldi", go =>
        {
            go.transform.position = new Vector3(-70f, 2f, -52f);
            go.transform.localScale = Vector3.one;
        });

        public static void DestroyBaldi() => DestroyAsset("baldi");

        private static bool enderScytheWasShooting;

        public static void SpawnEnderScythe() => Spawn("enderscythe", "Ender Scythe", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        });

        public static void UpdateEnderScythe()
        {
            if (!SpawnedAssets.TryGetValue("enderscythe", out GameObject scythe) || scythe == null) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool velTooHigh = (handVel - bodyVel).magnitude > 8f;
            if (velTooHigh && !enderScytheWasShooting)
            {
                AudioSource src = scythe.GetComponentInChildren<AudioSource>();
                if (src != null) src.Play();
            }
            enderScytheWasShooting = velTooHigh;
        }

        public static void DestroyEnderScythe() => DestroyAsset("enderscythe");

        public static void SpawnJukebox() => Spawn("jukebox", "Jukebox", go =>
        {
            go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1.5f;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            AudioSource audio = go.GetComponentInChildren<AudioSource>();
            if (audio != null) audio.Play();
        });

        public static void DestroyJukebox() => DestroyAsset("jukebox");

        public static void SpawnJman() => Spawn("jman", "Jman", go =>
        {
            go.transform.position = new Vector3(-70f, 2f, -52f);
            go.transform.localScale = Vector3.one;
        });

        public static void DestroyJman() => DestroyAsset("jman");

        private static bool jailWasShooting;

        public static void SpawnJailCell() => Spawn("jailcell", "Jail Cell", go =>
        {
            go.transform.position = Vector3.zero;
        });

        public static void UpdateJailCellGun(VRRig chosenRig, bool isShooting)
        {
            if (!SpawnedAssets.TryGetValue("jailcell", out GameObject jail) || jail == null) return;
            if (isShooting && chosenRig != null)
            {
                if (jailWasShooting) return;
                jail.transform.position = chosenRig.transform.position + new Vector3(-1f, -3f, -18f);
                jailWasShooting = true;
            }
            else jailWasShooting = false;
        }

        public static void DestroyJailCell() => DestroyAsset("jailcell");





        public static void SpawnBasketball() => Spawn("basketball", "Basketball", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up; go.transform.localScale = Vector3.one; });
        public static void DestroyBasketball() => DestroyAsset("basketball");

        public static void SpawnBeam() => Spawn("beam", "Beam", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBeam() => DestroyAsset("beam");

        private static bool blackRSwordSwinging;
        public static void SpawnBlackRSword() => Spawn("blackrsword", "Black RSword", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void UpdateBlackRSword() { if (!SpawnedAssets.TryGetValue("blackrsword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !blackRSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } blackRSwordSwinging = sw; }
        public static void DestroyBlackRSword() => DestroyAsset("blackrsword");

        public static void SpawnBlackStar() => Spawn("blackstar", "Black Star", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBlackStar() => DestroyAsset("blackstar");

        public static void SpawnBlock() => Spawn("block", "Block", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1.5f; go.transform.localScale = Vector3.one; });
        public static void DestroyBlock() => DestroyAsset("block");

        public static void SpawnBloodStar() => Spawn("bloodstar", "Blood Star", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBloodStar() => DestroyAsset("bloodstar");

        public static void SpawnBoomyWoomy() => Spawn("boomywoomy", "Boomy Woomy", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBoomyWoomy() => DestroyAsset("boomywoomy");

        private static bool boryRoseSwordSwinging;
        public static void SpawnBoryRoseSword() => Spawn("boryrosesword", "Bory Rose Sword", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void UpdateBoryRoseSword() { if (!SpawnedAssets.TryGetValue("boryrosesword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !boryRoseSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } boryRoseSwordSwinging = sw; }
        public static void DestroyBoryRoseSword() => DestroyAsset("boryrosesword");

        private static bool bouncyHammerSwinging;
        public static void SpawnBouncyHammer() => Spawn("bouncyhammer", "Bouncy Hammer", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void UpdateBouncyHammer() { if (!SpawnedAssets.TryGetValue("bouncyhammer", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !bouncyHammerSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } bouncyHammerSwinging = sw; }
        public static void DestroyBouncyHammer() => DestroyAsset("bouncyhammer");

        public static void SpawnBrite() => Spawn("brite", "Brite", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.bodyTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBrite() => DestroyAsset("brite");

        private static bool brSwordSwinging;
        public static void SpawnBrSword() => Spawn("brsword", "BR Sword", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void UpdateBrSword() { if (!SpawnedAssets.TryGetValue("brsword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !brSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } brSwordSwinging = sw; }
        public static void DestroyBrSword() => DestroyAsset("brsword");

        public static void SpawnBtools() => Spawn("btools", "Btools", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyBtools() => DestroyAsset("btools");

        public static void SpawnCageyes() => Spawn("cageyes", "Cage Eyes", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.headMesh.transform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyCageyes() => DestroyAsset("cageyes");

        public static void SpawnCaseoh() => Spawn("caseohf", "Caseoh", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyCaseoh() => DestroyAsset("caseohf");

        public static void SpawnCcc() => Spawn("ccc", "CCC", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyCcc() => DestroyAsset("ccc");

        public static void SpawnCghNorb() => Spawn("cghnorb", "CGH No RB", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyCghNorb() => DestroyAsset("cghnorb");

        public static void SpawnChair() => Spawn("chair", "Chair", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position; go.transform.localScale = Vector3.one; });
        public static void DestroyChair() => DestroyAsset("chair");

        public static void SpawnChasArena() => Spawn("chasarena", "Chas Arena", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyChasArena() => DestroyAsset("chasarena");

        public static void SpawnCherryBomb() => Spawn("cherrybomb", "Cherry Bomb", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyCherryBomb() => DestroyAsset("cherrybomb");

        public static void SpawnCity() => Spawn("city", "City", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyCity() => DestroyAsset("city");

        public static void SpawnClickbaitMenu() => Spawn("clickbaitmenu", "Clickbait Menu", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyClickbaitMenu() => DestroyAsset("clickbaitmenu");

        public static void SpawnColii() => Spawn("colii", "Colii", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyColii() => DestroyAsset("colii");

        public static void SpawnCommandBlock() => Spawn("commandblock", "Command Block", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyCommandBlock() => DestroyAsset("commandblock");

        public static void SpawnCrosser() => Spawn("crosser", "Crosser", go => { go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false); go.transform.localPosition = Vector3.zero; go.transform.localRotation = Quaternion.identity; });
        public static void DestroyCrosser() => DestroyAsset("crosser");

        public static void SpawnCube() => Spawn("cube", "Cube", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + GorillaTagger.Instance.bodyCollider.transform.forward * 1.5f; go.transform.localScale = Vector3.one; });
        public static void DestroyCube() => DestroyAsset("cube");

        public static void SpawnDancer() => Spawn("dancer", "Dancer", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyDancer() => DestroyAsset("dancer");

        public static void SpawnDbz() => Spawn("dbz", "DBZ", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyDbz() => DestroyAsset("dbz");

        public static void SpawnDomains() => Spawn("domains", "Domains", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyDomains() => DestroyAsset("domains");











        public static void SpawnEnderBasketball() => Spawn("enderbasketball", "Ender Basketball", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up; go.transform.localScale = Vector3.one; });
        public static void DestroyEnderBasketball() => DestroyAsset("enderbasketball");

        private static bool enderHammerSwinging;
        public static void SpawnEnderHammer() => Spawn("enderhammer", "Ender Hammer", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateEnderHammer() { if (!SpawnedAssets.TryGetValue("enderhammer", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !enderHammerSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } enderHammerSwinging = sw; }
        public static void DestroyEnderHammer() => DestroyAsset("enderhammer");

        public static void SpawnEnderTravis() => Spawn("endertravis", "Ender Travis", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyEnderTravis() => DestroyAsset("endertravis");

        public static void SpawnEndPortalStar() => Spawn("endportalstar", "End Portal Star", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyEndPortalStar() => DestroyAsset("endportalstar");

        public static void SpawnErrorStar() => Spawn("errorstar", "Error Star", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyErrorStar() => DestroyAsset("errorstar");

        public static void SpawnEvents() => Spawn("events", "Events", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyEvents() => DestroyAsset("events");

        public static void SpawnFlameThing() => Spawn("flamething", "Flame Thing", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyFlameThing() => DestroyAsset("flamething");

        public static void SpawnFootball() => Spawn("football", "Football", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up; go.transform.localScale = Vector3.one; });
        public static void DestroyFootball() => DestroyAsset("football");

        public static void SpawnFullySetColi() => Spawn("fullysetcoli", "Fully Set Coli", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyFullySetColi() => DestroyAsset("fullysetcoli");

        public static void SpawnGlitchesMaps() => Spawn("glitchesmaps", "Glitches Maps", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyGlitchesMaps() => DestroyAsset("glitchesmaps");

        private static bool iceRoseSwordSwinging;
        public static void SpawnIceRoseSword() => Spawn("icesroseword", "Ice Rose Sword", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateIceRoseSword() { if (!SpawnedAssets.TryGetValue("icesroseword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !iceRoseSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } iceRoseSwordSwinging = sw; }
        public static void DestroyIceRoseSword() => DestroyAsset("icesroseword");

        public static void SpawnIiMenu() => Spawn("iimenu", "II Menu", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyIiMenu() => DestroyAsset("iimenu");

        public static void SpawnIiTomb() => Spawn("iitomb", "II Tomb", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyIiTomb() => DestroyAsset("iitomb");

        public static void SpawnIndustryScavenger() => Spawn("industrysravenger", "Industry Scavenger", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyIndustryScavenger() => DestroyAsset("industrysravenger");

        private static bool karambitSwinging;
        public static void SpawnKarambit() => Spawn("karambit", "Karambit", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateKarambit() { if (!SpawnedAssets.TryGetValue("karambit", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !karambitSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } karambitSwinging = sw; }
        public static void DestroyKarambit() => DestroyAsset("karambit");

        public static void SpawnKars() => Spawn("kars", "Kars", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyKars() => DestroyAsset("kars");

        private static bool knifeSwinging;
        public static void SpawnKnife() => Spawn("knife", "Knife", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateKnife() { if (!SpawnedAssets.TryGetValue("knife", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !knifeSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } knifeSwinging = sw; }
        public static void DestroyKnife() => DestroyAsset("knife");

        public static void SpawnL115a33() => Spawn("l115a33", "L115A33", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyL115a33() => DestroyAsset("l115a33");

        public static void SpawnLaCuca() => Spawn("lacuca", "La Cuca", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyLaCuca() => DestroyAsset("lacuca");

        public static void SpawnLeviathan() => Spawn("leviathan", "Leviathan", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyLeviathan() => DestroyAsset("leviathan");

        public static void SpawnLonlyIsl() => Spawn("lonlyisl", "Lonely Island", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyLonlyIsl() => DestroyAsset("lonlyisl");

        public static void SpawnLowTaper() => Spawn("lowtaper", "Low Taper", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyLowTaper() => DestroyAsset("lowtaper");

        public static void SpawnMap() => Spawn("map", "Map", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyMap() => DestroyAsset("map");

        public static void SpawnMaps() => Spawn("maps", "Maps", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyMaps() => DestroyAsset("maps");

        public static void SpawnMaps2() => Spawn("maps2", "Maps 2", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyMaps2() => DestroyAsset("maps2");

        private static bool mccSwordSwinging;
        public static void SpawnMccSword() => Spawn("mccsword", "MCC Sword", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateMccSword() { if (!SpawnedAssets.TryGetValue("mccsword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !mccSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } mccSwordSwinging = sw; }
        public static void DestroyMccSword() => DestroyAsset("mccsword");

        private static bool mcSwordSwinging;
        public static void SpawnMcSword() => Spawn("mcsword", "MC Sword", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateMcSword() { if (!SpawnedAssets.TryGetValue("mcsword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !mcSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } mcSwordSwinging = sw; }
        public static void DestroyMcSword() => DestroyAsset("mcsword");

        private static bool mistScytheSwinging;
        public static void SpawnMistScythe() => Spawn("mistscythe", "Mist Scythe", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateMistScythe() { if (!SpawnedAssets.TryGetValue("mistscythe", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !mistScytheSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } mistScytheSwinging = sw; }
        public static void DestroyMistScythe() => DestroyAsset("mistscythe");

        public static void SpawnNamelessLaser() => Spawn("namelesslaser", "Nameless Laser", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyNamelessLaser() => DestroyAsset("namelesslaser");

        public static void SpawnNovaDomain() => Spawn("novadomain", "Nova Domain", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyNovaDomain() => DestroyAsset("novadomain");

        public static void SpawnNovaIndicatah() => Spawn("novaindihcatah", "Nova Indicatah", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyNovaIndicatah() => DestroyAsset("novaindihcatah");

        public static void SpawnOmega() => Spawn("omega", "Omega", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyOmega() => DestroyAsset("omega");

        public static void SpawnPigeon() => Spawn("pigeon", "Pigeon", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyPigeon() => DestroyAsset("pigeon");

        public static void SpawnPortalGun() => Spawn("portalgun", "Portal Gun", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyPortalGun() => DestroyAsset("portalgun");

        private static bool portalSwordSwinging;
        public static void SpawnPortalSword() => Spawn("portalsword", "Portal Sword", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdatePortalSword() { if (!SpawnedAssets.TryGetValue("portalsword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !portalSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } portalSwordSwinging = sw; }
        public static void DestroyPortalSword() => DestroyAsset("portalsword");

        public static void SpawnRavenger() => Spawn("ravenger", "Ravenger", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyRavenger() => DestroyAsset("ravenger");

        public static void SpawnRavyRavy() => Spawn("ravyravy", "Ravy Ravy", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyRavyRavy() => DestroyAsset("ravyravy");

        public static void SpawnRblxCarpet() => Spawn("rblxcarpet", "Rblx Carpet", go => { go.transform.position = GorillaTagger.Instance.bodyCollider.transform.position; go.transform.localScale = Vector3.one; });
        public static void DestroyRblxCarpet() => DestroyAsset("rblxcarpet");

        public static void SpawnReefBackReaper() => Spawn("reefbackreaper", "Reef Back Reaper", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyReefBackReaper() => DestroyAsset("reefbackreaper");

        private static bool rgbEnderSwordSwinging;
        public static void SpawnRgbEnderSword() => Spawn("rgbendersword", "RGB Ender Sword", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateRgbEnderSword() { if (!SpawnedAssets.TryGetValue("rgbendersword", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !rgbEnderSwordSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } rgbEnderSwordSwinging = sw; }
        public static void DestroyRgbEnderSword() => DestroyAsset("rgbendersword");

        public static void SpawnSaoPlayerIcon() => Spawn("saoplayericon", "SAO Player Icon", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroySaoPlayerIcon() => DestroyAsset("saoplayericon");

        public static void SpawnScaryLarry() => Spawn("scarylarry", "Scary Larry", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyScaryLarry() => DestroyAsset("scarylarry");

        public static void SpawnShibaHoldable() => Spawn("shibaholdable", "Shiba Holdable", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyShibaHoldable() => DestroyAsset("shibaholdable");

        public static void SpawnShotgun() => Spawn("shotgun1", "Shotgun", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyShotgun() => DestroyAsset("shotgun1");

        public static void SpawnSis() => Spawn("sis", "SIS", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroySis() => DestroyAsset("sis");

        public static void SpawnSoggy() => Spawn("soggy", "Soggy", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroySoggy() => DestroyAsset("soggy");

        public static void SpawnSogSog() => Spawn("sogsog", "Sog Sog", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroySogSog() => DestroyAsset("sogsog");

        public static void SpawnSprayPaint() => Spawn("spraypaint", "Spray Paint", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroySprayPaint() => DestroyAsset("spraypaint");

        public static void SpawnStarWin() => Spawn("star_win", "Star Win", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyStarWin() => DestroyAsset("star_win");

        public static void SpawnStarGlitcher() => Spawn("starglitcher", "Star Glitcher", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyStarGlitcher() => DestroyAsset("starglitcher");

        public static void SpawnStarPe() => Spawn("starpe", "Star PE", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyStarPe() => DestroyAsset("starpe");

        public static void SpawnStarWand() => Spawn("starwand", "Star Wand", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyStarWand() => DestroyAsset("starwand");

        public static void SpawnSummons() => Spawn("summons", "Summons", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroySummons() => DestroyAsset("summons");

        public static void SpawnTravisV2() => Spawn("travisv2", "Travis V2", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one * 0.38f; });
        public static void DestroyTravisV2() => DestroyAsset("travisv2");

        public static void SpawnTri() => Spawn("tri", "Tri", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void DestroyTri() => DestroyAsset("tri");

        public static void SpawnViltrumite() => Spawn("viltrumite", "Viltrumite", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyViltrumite() => DestroyAsset("viltrumite");

        private static bool whiteRoseSwinging;
        public static void SpawnWhiteRose() => Spawn("whiterose", "White Rose", go => { go.transform.position = GorillaTagger.Instance.leftHandTransform.position; go.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation; });
        public static void UpdateWhiteRose() { if (!SpawnedAssets.TryGetValue("whiterose", out GameObject s) || s == null) return; Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0); bool sw = (h - GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 8f; if (sw && !whiteRoseSwinging) { AudioSource a = s.GetComponentInChildren<AudioSource>(); if (a != null) a.Play(); } whiteRoseSwinging = sw; }
        public static void DestroyWhiteRose() => DestroyAsset("whiterose");





        public static void SpawnCgh() => Spawn("cgh", "CGH", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyCgh() => DestroyAsset("cgh");

        public static void SpawnHeaven() => Spawn("heaven", "Heaven", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyHeaven() => DestroyAsset("heaven");

        public static void SpawnHishiba() => Spawn("hishiba", "Hishiba", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyHishiba() => DestroyAsset("hishiba");

        public static void SpawnMinos() => Spawn("minos", "Minos", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyMinos() => DestroyAsset("minos");

        public static void SpawnNetflix() => Spawn("netflix", "Netflix", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyNetflix() => DestroyAsset("netflix");

        public static void SpawnPizzaMan() => Spawn("pizzaman", "Pizza Man", go => { go.transform.position = new Vector3(-70f, 2f, -52f); go.transform.localScale = Vector3.one; });
        public static void DestroyPizzaMan() => DestroyAsset("pizzaman");

        private static bool realKnifeWasShooting;
        public static void SpawnRealKnife() => Spawn("realknife", "Real Knife", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        });
        public static void UpdateRealKnife()
        {
            if (!SpawnedAssets.TryGetValue("realknife", out GameObject knife) || knife == null) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool swinging = (handVel - bodyVel).magnitude > 8f;
            if (swinging && !realKnifeWasShooting)
            {
                AudioSource src = knife.GetComponentInChildren<AudioSource>();
                if (src != null) src.Play();
            }
            realKnifeWasShooting = swinging;
        }
        public static void DestroyRealKnife() => DestroyAsset("realknife");

        private static bool stormBladeWasShooting;
        public static void SpawnStormBlade() => Spawn("stormblade", "Storm Blade", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        });
        public static void UpdateStormBlade()
        {
            if (!SpawnedAssets.TryGetValue("stormblade", out GameObject blade) || blade == null) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool swinging = (handVel - bodyVel).magnitude > 8f;
            if (swinging && !stormBladeWasShooting)
            {
                AudioSource src = blade.GetComponentInChildren<AudioSource>();
                if (src != null) src.Play();
            }
            stormBladeWasShooting = swinging;
        }
        public static void DestroyStormBlade() => DestroyAsset("stormblade");

        private static bool zeldaSwordWasShooting;
        public static void SpawnZeldaSword() => Spawn("zeldasword", "Zelda Sword", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            AudioSource src = go.GetComponentInChildren<AudioSource>();
            if (src != null) src.Play();
        });
        public static void UpdateZeldaSword()
        {
            if (!SpawnedAssets.TryGetValue("zeldasword", out GameObject sword) || sword == null) return;
            Vector3 handVel = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 bodyVel = GorillaTagger.Instance.rigidbody.linearVelocity;
            bool swinging = (handVel - bodyVel).magnitude > 8f;
            if (swinging && !zeldaSwordWasShooting)
            {
                AudioSource src = sword.GetComponentInChildren<AudioSource>();
                if (src != null) src.Play();
            }
            zeldaSwordWasShooting = swinging;
        }
        public static void DestroyZeldaSword() => DestroyAsset("zeldasword");

        public static void SpawnWings() => Spawn("wings", "Wings", go =>
        {
            go.transform.SetParent(GorillaTagger.Instance.offlineVRRig.bodyTransform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        });
        public static void DestroyWings() => DestroyAsset("wings");

        public static void SpawnWii() => Spawn("wii", "Wii", go =>
        {
            go.transform.position = new Vector3(-70f, 2f, -52f);
            go.transform.localScale = Vector3.one;
        });
        public static void DestroyWii() => DestroyAsset("wii");
    }
}




    public class CoroutineRunner : MonoBehaviour
    {
        public void Run(IEnumerator routine) => StartCoroutine(routine);
    }
