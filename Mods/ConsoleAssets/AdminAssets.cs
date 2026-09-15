using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using LagMenu.Menu;
using LagMenu.Mods;
using LagMenu.Mods.ConsoleAssets;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Video;
using static Bindings;
using static UnityEngine.GridBrushBase;
using Console = LagMenu.Menu.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    public static class AdminAssets
    {
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

        private static int iphoneNetId = -1;
        private static int travisNetId = -1;
        private static int skeleNetId = -1;
        private static int miniTravisNetId = -1;
        private static int flashEffectsNetId = -1;
        private static int concertNetId = -1;
        private static int donationNukeNetId = -1;
        private static int hamburburNetId = -1;
        private static int assetId;
        private static int swordNetId = -1;
        private static int banHammerNetId = -1;
        private static int rbSwordNetId = -1;
        private static int baldiNetId = -1;
        private static int enderScytheNetId = -1;
        private static int jukeboxNetId = -1;
        private static int jmanNetId = -1;
        private static int jailCellNetId = -1;
        private static int basketballNetId = -1;
        private static int beamNetId = -1;
        private static int blackRSwordNetId = -1;
        private static int blackStarNetId = -1;
        private static int blockNetId = -1;
        private static int bloodStarNetId = -1;
        private static int boomyWoomyNetId = -1;
        private static int boryRoseSwordNetId = -1;
        private static int bouncyHammerNetId = -1;
        private static int briteNetId = -1;
        private static int brSwordNetId = -1;
        private static int btoolsNetId = -1;
        private static int cageyesNetId = -1;
        private static int caseohNetId = -1;
        private static int cccNetId = -1;
        private static int cghNorbNetId = -1;
        private static int chairNetId = -1;
        private static int chasArenaNetId = -1;
        private static int cherryBombNetId = -1;
        private static int cityNetId = -1;
        private static int clickbaitMenuNetId = -1;
        private static int coliiNetId = -1;
        private static int commandBlockNetId = -1;
        private static int crosserNetId = -1;
        private static int cubeNetId = -1;
        private static int dancerNetId = -1;
        private static int dbzNetId = -1;
        private static int domainsNetId = -1;
        private static int enderBasketballNetId = -1;
        private static int enderHammerNetId = -1;
        private static int enderTravisNetId = -1;
        private static int endPortalStarNetId = -1;
        private static int errorStarNetId = -1;
        private static int eventsNetId = -1;
        private static int flameThingNetId = -1;
        private static int footballNetId = -1;
        private static int fullySetColiNetId = -1;
        private static int glitchesMapsNetId = -1;
        private static int iceRoseSwordNetId = -1;
        private static int iiMenuNetId = -1;
        private static int iiTombNetId = -1;
        private static int industryScavengerNetId = -1;
        private static int karambitNetId = -1;
        private static int karsNetId = -1;
        private static int knifeNetId = -1;
        private static int l115a33NetId = -1;
        private static int laCucaNetId = -1;
        private static int leviathanNetId = -1;
        private static int lonlyIslNetId = -1;
        private static int lowTaperNetId = -1;
        private static int mapNetId = -1;
        private static int mapsNetId = -1;
        private static int maps2NetId = -1;
        private static int mccSwordNetId = -1;
        private static int mcSwordNetId = -1;
        private static int mistScytheNetId = -1;
        private static int namelessLaserNetId = -1;
        private static int novaDomainNetId = -1;
        private static int novaIndicatahNetId = -1;
        private static int omegaNetId = -1;
        private static int pigeonNetId = -1;
        private static int portalGunNetId = -1;
        private static int portalSwordNetId = -1;
        private static int ravengerNetId = -1;
        private static int ravyRavyNetId = -1;
        private static int rblxCarpetNetId = -1;
        private static int reefBackReaperNetId = -1;
        private static int rgbEnderSwordNetId = -1;
        private static int saoPlayerIconNetId = -1;
        private static int scaryLarryNetId = -1;
        private static int shibaHoldableNetId = -1;
        private static int shotgunNetId = -1;
        private static int sisNetId = -1;
        private static int soggyNetId = -1;
        private static int sogSogNetId = -1;
        private static int sprayPaintNetId = -1;
        private static int starWinNetId = -1;
        private static int starGlitcherNetId = -1;
        private static int starPeNetId = -1;
        private static int starWandNetId = -1;
        private static int summonsNetId = -1;
        private static int travisV2NetId = -1;
        private static int triNetId = -1;
        private static int viltrumiteNetId = -1;
        private static int whiteRoseNetId = -1;
        private static int cghNetId = -1;
        private static int heavenNetId = -1;
        private static int hishibaNetId = -1;
        private static int minosNetId = -1;
        private static int netflixNetId = -1;
        private static int pizzaManNetId = -1;
        private static int realKnifeNetId = -1;
        private static int stormBladeNetId = -1;
        private static int zeldaSwordNetId = -1;
        private static int wingsNetId = -1;
        private static int wiiNetId = -1;
        private static int NukeNetId = -1;
        public static int tungTungTungSahurAssetId = -1;
        private static bool pistolWasShooting;
        private static bool swordEnabled;
        private static bool lastVelTooHigh;
        private static float swingDelay;
        private static bool banHammerSwinging;
        private static bool rbSwordEnabled;
        private static bool rbSwordLastVelTooHigh;
        private static float rbSwordSwingDelay;
        private static bool enderScytheSwinging;
        private static bool blackRSwordSwinging;
        private static bool boryRoseSwordSwinging;
        private static bool bouncyHammerSwinging;
        private static bool brSwordSwinging;
        private static bool enderHammerSwinging;
        private static bool iceRoseSwordSwinging;
        private static bool karambitSwinging;
        private static bool knifeSwinging;
        private static bool mccSwordSwinging;
        private static bool mcSwordSwinging;
        private static bool mistScytheSwinging;
        private static bool portalSwordSwinging;
        private static bool rgbEnderSwordSwinging;
        private static bool whiteRoseSwinging;
        private static bool realKnifeSwinging;
        private static bool stormBladeSwinging;
        private static bool zeldaSwordSwinging;
        private static bool jailWasShooting;
        private static float hamburburNextPlayTime;

        private static Vector3 WorldForward(float dist) =>
            GorillaTagger.Instance.bodyCollider.transform.position +
            GorillaTagger.Instance.bodyCollider.transform.forward * dist;

        private static bool SwingDetected(float threshold = 8f)
        {
            Vector3 h = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
            Vector3 b = GorillaTagger.Instance.rigidbody.linearVelocity;
            return (h - b).magnitude > threshold;
        }

        public static void SpawnIPhone()
        {
            iphoneNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "iphone", "iphone", iphoneNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, iphoneNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, iphoneNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, iphoneNetId, Quaternion.identity);
            string videoUrl = VideoLinks[Random.Range(0, VideoLinks.Length)];
            Console.ExecuteCommand("asset-setvideo", ReceiverGroup.All, iphoneNetId, videoUrl);
        }

        public static void DestroyIPhone()
        {
            if (iphoneNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, iphoneNetId);
            iphoneNetId = -1;
        }

        public static void SpawnTravis()
        {
            travisNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "travis", "travisscott", travisNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, travisNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, travisNetId, Vector3.one * 0.38f);
        }

        public static void DestroyTravis()
        {
            if (travisNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, travisNetId);
            travisNetId = -1;
        }



        public static void SpawnSkele()
        {
            skeleNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "skele", "skele", skeleNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, skeleNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, skeleNetId, Vector3.one * 0.38f);
        }


        public static void DestroySkele()
        {
            if (skeleNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, skeleNetId);
            skeleNetId = -1;
        }


        public static void TungTungTungSahur()
        {
            tungTungTungSahurAssetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1",
                    "tungtungtungsahur", tungTungTungSahurAssetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, tungTungTungSahurAssetId,
                    new Vector3(-57.1f, 5.6f, -37f));
            Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, tungTungTungSahurAssetId,
                    Quaternion.Euler(0f, 0f, 0f));
        }

        public static void TungTungTungSahur2()
        {
            tungTungTungSahurAssetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "tungtungtungsahur",
                    "tungtungtungsahur", tungTungTungSahurAssetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, tungTungTungSahurAssetId,
                    new Vector3(-57.1f, 5.6f, -37f));
            Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, tungTungTungSahurAssetId,
                    Quaternion.Euler(0f, 0f, 0f));
        }

        public static void DisableTungTungTungSahur()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, tungTungTungSahurAssetId);
        }








        public static void SpawnMiniTravis()
        {
            assetId = Console.GetFreeAssetID();

            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "minitravis", "travisscott",
                    assetId);

            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, assetId, 1);

            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, assetId,
                    new Vector3(-0.6f, 0.2f, 0f));

            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, assetId,
                    new Vector3(80f, 160f, 180f));
        }

        public static void DestroyMiniTravis()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId); Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
        }

        public static void SpawnFlashEffects()
        {
            flashEffectsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "flasheffects", EffectNames[flashTrailIndex], flashEffectsNetId);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, flashEffectsNetId, 3);
        }

        public static void DestroyFlashEffects()
        {
            if (flashEffectsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, flashEffectsNetId);
            flashEffectsNetId = -1;
        }

        public static void SpawnConcert()
        {
            bool inForest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest")?.activeInHierarchy ?? false;
            Vector3 position = inForest ? new Vector3(-27f, 2.4f, -49.9f) : new Vector3(-28.4873f, 15.5272f, -117.8634f);
            Quaternion rotation = inForest ? Quaternion.Euler(0f, 250f, 0f) : Quaternion.Euler(0f, 300f, 0f);
            Vector3 scale = inForest ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.8f, 0.8f, 0.8f);

            concertNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "concert", "concert", concertNetId);
            Console.ExecuteCommand("asset-settransform", ReceiverGroup.All, concertNetId, position, rotation);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, concertNetId, scale);
            Console.ExecuteCommand("asset-destroychild", ReceiverGroup.All, concertNetId, "stage/Targetphoto");
            Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, concertNetId, "audio", ConcertVideoNames[concertVideoIndex]);
        }

        public static void DestroyConcert()
        {
            if (concertNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, concertNetId);
            concertNetId = -1;
        }

        public static void SpawnDonationNuke()
        {
            assetId = Console.GetFreeAssetID();

            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "donationnuke",
                    "plsdonatenuke", assetId);

            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, assetId,
                    new Vector3(-64.16f, 2.99f, -82.07f));

            Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, assetId, "nuke", "nukesound");
        }

        public static void DestroyDonationNuke()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
        }


        public static void SpawnDonationNukeH() => SpawnDonationNuke();
        public static void DestroyDonationNukeH() => DestroyDonationNuke();

        public static void SpawnHamburbur()
        {
            hamburburNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "effects", "hamburgur", hamburburNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, hamburburNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, hamburburNetId, Vector3.zero);
            Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, hamburburNetId, "canihaveachezburger");
        }

        public static void UpdateHamburbur()
        {
            if (hamburburNetId < 0 || Time.time < hamburburNextPlayTime) return;
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (Vector3.Distance(rig.headMesh.transform.position,
                        GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.4f)
                {
                    Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, hamburburNetId, "mmmchezburger");
                    hamburburNextPlayTime = Time.time + 2f;
                    break;
                }
            }
        }

        public static void DestroyHamburbur()
        {
            if (hamburburNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, hamburburNetId);
            hamburburNetId = -1;
        }


        private static GameObject pistolCrosshair;

        public static void SpawnPistol()
        {
            assetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Pistol", assetId);
            if (BigAssets.isEnabled)
                Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, assetId, Vector3.one * 5);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, assetId, 2);

            if (pistolCrosshair == null)
            {
                pistolCrosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(pistolCrosshair.GetComponent<Collider>());
                Object.Destroy(pistolCrosshair.GetComponent<Rigidbody>());
                pistolCrosshair.transform.localScale = Vector3.one * 0.05f;
                pistolCrosshair.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                pistolCrosshair.GetComponent<Renderer>().material.color = Color.white;
                pistolCrosshair.layer = 2;
            }

            pistolCrosshair.SetActive(true);
        }


        private static bool lastTrigger;
        private static bool wasTriggerPressed;

        public static void UpdatePistol()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;

            bool trigger = ControllerInputPoller.instance.rightControllerTriggerButton;


            if (pistolCrosshair != null && Console.consoleAssets.TryGetValue(assetId, out Console.ConsoleAsset asset))
            {
                Transform rayPoint = asset.assetObject.transform.Find("Model");
                Transform arm = GTPlayer.Instance.RightHand.controllerTransform;

                Physics.Raycast(arm.position, -arm.up, out RaycastHit hit, 512f);
                pistolCrosshair.transform.position = hit.collider != null
                    ? hit.point
                    : arm.position + (-arm.up * 10f);


                if (trigger && !wasTriggerPressed)
                {
                    Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, assetId, "Model", "PistolShoot");
                    Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, assetId, "Model", "Shoot");

                    if (hit.collider != null)
                    {
                        VRRig rig = hit.collider.GetComponentInParent<VRRig>();
                        if (rig != null && !rig.isLocal)
                        {
                            Photon.Realtime.Player player = RigManager.GetPlayerFromVRRig(rig);
                            if (player != null)
                                Console.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
                        }
                    }
                }
            }

            if (!trigger && wasTriggerPressed)
                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, assetId, "Model", "Default");

            wasTriggerPressed = trigger;
        }


        public static void UpdatePistolVel()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;

            bool trigger = ControllerInputPoller.instance.rightControllerTriggerButton;


            if (pistolCrosshair != null && Console.consoleAssets.TryGetValue(assetId, out Console.ConsoleAsset asset))
            {
                Transform rayPoint = asset.assetObject.transform.Find("Model");
                Transform arm = GTPlayer.Instance.RightHand.controllerTransform;

                Physics.Raycast(arm.position, -arm.up, out RaycastHit hit, 512f);
                pistolCrosshair.transform.position = hit.collider != null
                    ? hit.point
                    : arm.position + (-arm.up * 10f);


                if (trigger && !wasTriggerPressed)
                {
                    Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, assetId, "Model", "PistolShoot");
                    Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, assetId, "Model", "Shoot");

                    if (hit.collider != null)
                    {
                        VRRig rig = hit.collider.GetComponentInParent<VRRig>();
                        if (rig != null && !rig.isLocal)
                        {
                            Photon.Realtime.Player player = RigManager.GetPlayerFromVRRig(rig);
                            if (player != null)
                                Console.ExecuteCommand("vel", player.ActorNumber, player.UserId);
                        }
                    }
                }
            }

            if (!trigger && wasTriggerPressed)
                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, assetId, "Model", "Default");

            wasTriggerPressed = trigger;
        }

        public static void DestroyPistol()
        {
            if (assetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
            assetId = -1;
            pistolWasShooting = false;

            if (pistolCrosshair != null)
            {
                Object.Destroy(pistolCrosshair);
                pistolCrosshair = null;
            }
        }

        public static void SpawnSword()
        {
            if (swordEnabled) return;
            swordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "sword", swordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, swordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, swordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, swordNetId, Quaternion.identity);
            Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, swordNetId, "Unsheath");
            swordEnabled = true;
            lastVelTooHigh = false;
            swingDelay = 0f;
        }

        public static void UpdateSword()
        {
            if (!swordEnabled || swordNetId < 0) return;
            bool velTooHigh =
     (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) -
      GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

            if (velTooHigh && !lastVelTooHighRS && Time.time > pauseSfx)
            {
                pauseSfx = Time.time + 0.3f;

                Console.ExecuteCommand("asset-playsound",
                    ReceiverGroup.All,
                    allocatedRSwordId,
                    "Sword/SFX",
                    $"Swing{Random.Range(1, 3)}");

                Vector3 handPos = GorillaTagger.Instance.rightHandTransform.position;

                foreach (VRRig rig in VRRigExtensions.ActiveRigs)
                {
                    if (rig.isLocal)
                        continue;

                    if (Vector3.Distance(handPos, rig.headMesh.transform.position) < 1.2f)
                    {
                        Console.ExecuteCommand("asset-playsound",
                            ReceiverGroup.All,
                            allocatedRSwordId,
                            "Sword/SFX",
                            $"Slash{Random.Range(1, 3)}");

                        Console.ExecuteCommand("asset-playanimation",
                            ReceiverGroup.All,
                            allocatedRSwordId,
                            "Sword",
                            "Particles");

                        NetPlayer player = rig.Creator;

                        Console.ExecuteCommand("silkick",
                            player.ActorNumber,
                            player.UserId);

                        break;
                    }
                }
            }

            lastVelTooHighRS = velTooHigh;
        }

        public static void DestroySword()
        {
            if (swordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, swordNetId);
            swordNetId = -1;
            swordEnabled = false;
            lastVelTooHigh = false;
            swingDelay = 0f;
        }

        public static void SpawnBanHammer()
        {
            if (banHammerNetId >= 0)
                return;

            banHammerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "banhammer", "BanHammer",
                    banHammerNetId);

            if (BigAssets.isEnabled)
                Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, banHammerNetId,
                        Vector3.one * 5);

            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, banHammerNetId, 2);

            Saftey.RPCProtection();
        }

        public static void UpdateBanHammer(Console.ConsoleAsset asset)
        {
            if (banHammerNetId < 0) return;
            if (!Console.ContainsKey(banHammerNetId)) return;
            Transform RayPoint = asset.assetObject.transform.Find("Model/HitBox");

            if (RayPoint == null)
            {
                Debug.LogError("[LagMenu] BanHammer: couldn't find 'Model/HitBox' on the spawned asset — check the actual prefab hierarchy.");
                return;
            }

            if (!RayPoint.TryGetComponent(out MeshCollider _))
                RayPoint.gameObject.AddComponent<MeshCollider>();

            Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit Ray, 0.4f,
                   Main.NoInvisLayerMask());

            Physics.SphereCast(RayPoint.position, 0.2f, RayPoint.forward, out RaycastHit ColliderRay, 0.4f,
                    GTPlayer.Instance.locomotionEnabledLayers);

            bool velTooHigh =
                    (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) -
                     GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

            if (Time.time > slashDelay)
            {
                if (Ray.collider != null)
                {
                    VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                    if (Target != null && !Target.isLocal)
                    {
                        slashDelay = Time.time + 1f;
                        pauseSfx = Time.time + 1f;

                        CoroutineManager.instance.StartCoroutine(KillFX());

                        NetPlayer player = Target.Creator;
                        Console.ExecuteCommand("kick", player.ActorNumber);
                    }
                }

                if (ColliderRay.collider != null)
                {
                    slashDelay = Time.time + 0.3f;
                    pauseSfx = Time.time + 0.5f;

                    Vector3 surfaceNormal = ColliderRay.normal;
                    Vector3 handVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0);
                    Vector3 bodyVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
                    float totalVelocity = handVelocity.magnitude + bodyVelocity.magnitude;
                    float pushStrength = Mathf.Clamp(totalVelocity, 1f, 14f);
                    GorillaTagger.Instance.rigidbody.linearVelocity += surfaceNormal * pushStrength;

                    CoroutineManager.instance.StartCoroutine(HitFX());
                }
            }

            if (velTooHigh && !lastVelTooHighRS && Time.time > pauseSfx)
            {
                pauseSfx = Time.time + 0.3f;
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, banHammerNetId,
                        "Model/SwingSFX", "Swing");
            }

            lastVelTooHighRS = velTooHigh;
        }


        public static void DestroyBanHammer()
        {
            if (banHammerNetId >= 0)
            {
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, banHammerNetId);
                banHammerNetId = -1;
            }
        }



        private static IEnumerator HitFX()
        {
            Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, banHammerNetId,
                    "Model", "Default");

            yield return null;
            yield return null;
            Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, banHammerNetId,
                    "Model/SwingSFX", "HammerHit");

            Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, banHammerNetId,
                    "Model", "HitGround");

            foreach (VRRig rig in VRRigCache.m_activeRigs.Where(rig => Vector3.Distance(
                                                                                     GorillaTagger.Instance
                                                                                            .rightHandTransform.position,
                                                                                     rig.transform.position) < 2f))
                Console.ExecuteCommand("vel", rig.Creator.ActorNumber,
                        (rig.transform.position - GorillaTagger.Instance.rightHandTransform.position).normalized * 5f);
        }

        private static IEnumerator KillFX()
        {
            Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, banHammerNetId,
                    "Model", "Default");

            yield return null;
            yield return null;
            Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, banHammerNetId,
                    "Model/KillSFX", "HammerKill");

            Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, banHammerNetId,
                    "Model", "HitPlayer");
        }





        public static void SpawnRbSword()
        {
            if (allocatedRSwordId >= 0) return;
            allocatedRSwordId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rbsword", "Sword", allocatedRSwordId);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedRSwordId, 2);
            Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, "Sword", "Music");
        }


        public static Dictionary<int, Console.ConsoleAsset> Assets =
    new Dictionary<int, Console.ConsoleAsset>();

        public static bool TryGetValue(int key, out Console.ConsoleAsset value)
        {
            if (Assets.ContainsKey(key))
            {
                value = Assets[key];
                return true;
            }

            value = null;
            return false;
        }


        private static float slashDelay;
        private static int allocatedRSwordId = -1;
        private static bool lastVelTooHighRS;
        private static float pauseSfx;

        public static void UpdateRBSword()
        {
            if (allocatedRSwordId < 0)
            {
                allocatedRSwordId = Console.GetFreeAssetID();
                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rbsword", nameof(Sword),
                        allocatedRSwordId);

                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, allocatedRSwordId, 2);
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId, nameof(Sword),
                        "Music");

                if (BigAssets.isEnabled)
                    Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, allocatedRSwordId,
                            Vector3.one * 5);

                Saftey.RPCProtection();
            }

            if (!TryGetValue(allocatedRSwordId,
                        out Console.ConsoleAsset asset))
                return;

            Transform rayPoint = asset.assetObject.transform.Find("Sword/HitBox");

            Physics.SphereCast(rayPoint.position, 0.1f, rayPoint.forward, out RaycastHit Ray, 0.7f,
                    Main.NoInvisLayerMask());

            if (Time.time > slashDelay && Ray.collider != null)
                try
                {
                    VRRig Target = Ray.collider.GetComponentInParent<VRRig>();
                    if (Target != null && !Target.isLocal)
                    {
                        slashDelay = Time.time + 0.5f;
                        pauseSfx = Time.time + 1f;
                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId,
                                "Sword/SFX", $"Slash{Random.Range(1, 3)}");

                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All,
                                allocatedRSwordId, nameof(Sword), "Particles");

                        NetPlayer player = Target.Creator;
                        Console.ExecuteCommand("silkick", player.ActorNumber, player.UserId);
                    }
                }
                catch { }

            bool velTooHigh = (GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0) -
                               GorillaTagger.Instance.rigidbody.linearVelocity).magnitude > 10f;

            if (velTooHigh && !lastVelTooHighRS && Time.time > pauseSfx)
            {
                pauseSfx = Time.time + 0.3f;
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, allocatedRSwordId,
                        "Sword/SFX", $"Swing{Random.Range(1, 3)}");
            }

            lastVelTooHighRS = velTooHigh;
        }

        public static void DestroyRbSword()
        {
            if (allocatedRSwordId < 0) return;

            Console.ExecuteCommand("asset-destroy",
                ReceiverGroup.All, allocatedRSwordId);

            allocatedRSwordId = -1;
            lastVelTooHighRS = false;
            pauseSfx = 0f;
        }







        public static int scaryLarryAssetId;

        public static void ScaryLarry()
        {
            scaryLarryAssetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1",
                    "scarylarry", scaryLarryAssetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, scaryLarryAssetId,
                    new Vector3(-57.1f, 5.6f, -37f));
            Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, scaryLarryAssetId,
                    Quaternion.Euler(0f, 0f, 0f));
        }

        public static void DisableScaryLarry()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, scaryLarryAssetId);
        }













        public static int TVassetId;
        public static int sofaAssetId;

        public static void TV()
        {
            assetId = Console.GetFreeAssetID();
            sofaAssetId = Console.GetFreeAssetID();

            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "consolehamburburassets",
                    "TV", assetId);

            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "consolehamburburassets",
                    "sofa", sofaAssetId);

            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, assetId,
                    new Vector3(-57.1f, 5.6f, -37f));

            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, sofaAssetId,
                    new Vector3(-51.8f, 4.2f, -37.4f));

            Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, assetId,
                    Quaternion.Euler(270f, 0f, 0f));

            Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, sofaAssetId,
                    Quaternion.Euler(270f, 270f, 0f));
            Console.ExecuteCommand("asset-setvideo", ReceiverGroup.All, assetId, nameof(VideoPlayer),
                    Mods.VideoPlayerType.CurrentUrl);
        }

        public static void DisableTV()
        {
           Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, assetId);
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, sofaAssetId);
        }





        public static void SpawnEnderScythe()
        {
            enderScytheNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "enderscythe", "enderscythe", enderScytheNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, enderScytheNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, enderScytheNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, enderScytheNetId, Quaternion.identity);
        }

        public static void UpdateEnderScythe()
        {
            if (enderScytheNetId < 0) return;
            bool swinging = SwingDetected(8f);
            if (swinging && !enderScytheSwinging)
                Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, enderScytheNetId, "swing");
            enderScytheSwinging = swinging;
        }

        public static void DestroyEnderScythe()
        {
            if (enderScytheNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, enderScytheNetId);
            enderScytheNetId = -1;
            enderScytheSwinging = false;
        }

        public static void SpawnJukebox()
        {
            jukeboxNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "jukebox", "jukebox", jukeboxNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, jukeboxNetId, WorldForward(1.5f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, jukeboxNetId, Vector3.one);
            Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, jukeboxNetId, "music");
        }

        public static void DestroyJukebox()
        {
            if (jukeboxNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, jukeboxNetId);
            jukeboxNetId = -1;
        }

        public static void SpawnJman()
        {
            jmanNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "jman", "jman", jmanNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, jmanNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, jmanNetId, Vector3.one);
        }

        public static void DestroyJman()
        {
            if (jmanNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, jmanNetId);
            jmanNetId = -1;
        }

        public static void SpawnJailCell()
        {
            jailCellNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "jailcell", "jail", jailCellNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, jailCellNetId, Vector3.zero);
        }

        public static void UpdateJailCellGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (jailCellNetId < 0) return;
                if (jailWasShooting) return;

                VRRig target = GunLib.GetTargetRig();
                if (target == null) return;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, jailCellNetId,
                    target.transform.position + new Vector3(-1f, -3f, -18f));
                jailWasShooting = true;
            },
            rightHand: true);

            if (!GunLib.isTrigger)
                jailWasShooting = false;
        }

        public static void DestroyJailCell()
        {
            if (jailCellNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, jailCellNetId);
            jailCellNetId = -1;
            jailWasShooting = false;
        }

        public static void SpawnBasketball()
        {
            basketballNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "basketball", "basketball", basketballNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, basketballNetId,
                GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, basketballNetId, Vector3.one);
        }

        public static void DestroyBasketball()
        {
            if (basketballNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, basketballNetId);
            basketballNetId = -1;
        }






        public static void SpawnBeam()
        {
            if (beamNetId >= 0)
                return;

            beamNetId = Console.GetFreeAssetID();

            Console.ExecuteCommand(
                "asset-spawn",
                ReceiverGroup.All,
                "beam",
                "beam",
                beamNetId);


            Console.ExecuteCommand(
                "asset-setanchor",
                ReceiverGroup.All,
                beamNetId,
                2);


            Console.ExecuteCommand(
                "asset-setlocalposition",
                ReceiverGroup.All,
                beamNetId,
                new Vector3(0f, 0f, 0.15f));


            Console.ExecuteCommand(
                "asset-setlocalrotation",
                ReceiverGroup.All,
                beamNetId,
                Quaternion.identity);
        }

        public static void DestroyBeam()
        {
            if (beamNetId < 0)
                return;

            Console.ExecuteCommand(
                "asset-destroy",
                ReceiverGroup.All,
                beamNetId);

            beamNetId = -1;
        }

        public static void SpawnBlackRSword()
        {
            blackRSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "blackrsword", "blackrsword", blackRSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, blackRSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, blackRSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, blackRSwordNetId, Quaternion.identity);
        }

        public static void UpdateBlackRSword()
        {
            if (blackRSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !blackRSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, blackRSwordNetId, "swing");
            blackRSwordSwinging = sw;
        }

        public static void DestroyBlackRSword()
        {
            if (blackRSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, blackRSwordNetId);
            blackRSwordNetId = -1;
            blackRSwordSwinging = false;
        }

        public static void SpawnBlackStar()
        {
            blackStarNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "blackstar", "blackstar", blackStarNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, blackStarNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, blackStarNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, blackStarNetId, Quaternion.identity);
        }

        public static void DestroyBlackStar()
        {
            if (blackStarNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, blackStarNetId);
            blackStarNetId = -1;
        }

        public static void SpawnBlock()
        {
            blockNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "block", "block", blockNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, blockNetId, WorldForward(1.5f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, blockNetId, Vector3.one);
        }

        public static void DestroyBlock()
        {
            if (blockNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, blockNetId);
            blockNetId = -1;
        }

        public static void SpawnBloodStar()
        {
            bloodStarNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "bloodstar", "bloodstar", bloodStarNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, bloodStarNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, bloodStarNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, bloodStarNetId, Quaternion.identity);
        }

        public static void DestroyBloodStar()
        {
            if (bloodStarNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, bloodStarNetId);
            bloodStarNetId = -1;
        }

        public static void SpawnBoomyWoomy()
        {
            boomyWoomyNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "boomywoomy", "boomywoomy", boomyWoomyNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, boomyWoomyNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, boomyWoomyNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, boomyWoomyNetId, Quaternion.identity);
        }

        public static void DestroyBoomyWoomy()
        {
            if (boomyWoomyNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, boomyWoomyNetId);
            boomyWoomyNetId = -1;
        }

        public static void SpawnBoryRoseSword()
        {
            boryRoseSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "boryrosesword", "boryrosesword", boryRoseSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, boryRoseSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, boryRoseSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, boryRoseSwordNetId, Quaternion.identity);
        }

        public static void UpdateBoryRoseSword()
        {
            if (boryRoseSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !boryRoseSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, boryRoseSwordNetId, "swing");
            boryRoseSwordSwinging = sw;
        }

        public static void DestroyBoryRoseSword()
        {
            if (boryRoseSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, boryRoseSwordNetId);
            boryRoseSwordNetId = -1;
            boryRoseSwordSwinging = false;
        }

        public static void SpawnBouncyHammer()
        {
            bouncyHammerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "banhammer", "banhammer", bouncyHammerNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, bouncyHammerNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, bouncyHammerNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, bouncyHammerNetId, Quaternion.identity);
        }

        public static void UpdateBouncyHammer()
        {
            if (bouncyHammerNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !bouncyHammerSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, bouncyHammerNetId, "swing");
            bouncyHammerSwinging = sw;
        }

        public static void DestroyBouncyHammer()
        {
            if (bouncyHammerNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, bouncyHammerNetId);
            bouncyHammerNetId = -1;
            bouncyHammerSwinging = false;
        }

        public static void SpawnBrite()
        {
            briteNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "brite", "brite", briteNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, briteNetId, "body");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, briteNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, briteNetId, Quaternion.identity);
        }

        public static void DestroyBrite()
        {
            if (briteNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, briteNetId);
            briteNetId = -1;
        }

        public static void SpawnBrSword()
        {
            brSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "brsword", "brsword", brSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, brSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, brSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, brSwordNetId, Quaternion.identity);
        }

        public static void UpdateBrSword()
        {
            if (brSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !brSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, brSwordNetId, "swing");
            brSwordSwinging = sw;
        }

        public static void DestroyBrSword()
        {
            if (brSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, brSwordNetId);
            brSwordNetId = -1;
            brSwordSwinging = false;
        }

        public static void SpawnBtools()
        {
            btoolsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "btools", "btools", btoolsNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, btoolsNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, btoolsNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, btoolsNetId, Quaternion.identity);
        }

        public static void DestroyBtools()
        {
            if (btoolsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, btoolsNetId);
            btoolsNetId = -1;
        }

        public static void SpawnCageyes()
        {
            cageyesNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cageyes", "cageyes", cageyesNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, cageyesNetId, "head");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, cageyesNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, cageyesNetId, Quaternion.identity);
        }

        public static void DestroyCageyes()
        {
            if (cageyesNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cageyesNetId);
            cageyesNetId = -1;
        }

        public static void SpawnCaseoh()
        {
            caseohNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "caseohf", "caseoh", caseohNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, caseohNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, caseohNetId, Vector3.one);
        }

        public static void DestroyCaseoh()
        {
            if (caseohNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, caseohNetId);
            caseohNetId = -1;
        }

        public static void SpawnCcc()
        {
            cccNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "ccc", "ccc", cccNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, cccNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, cccNetId, Vector3.one);
        }

        public static void DestroyCcc()
        {
            if (cccNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cccNetId);
            cccNetId = -1;
        }

        public static void SpawnCghNorb()
        {
            cghNorbNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cghnorb", "cghnorb", cghNorbNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, cghNorbNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, cghNorbNetId, Vector3.one);
        }

        public static void DestroyCghNorb()
        {
            if (cghNorbNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cghNorbNetId);
            cghNorbNetId = -1;
        }

        public static void SpawnChair()
        {
            chairNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "chair", "chair", chairNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, chairNetId,
                GorillaTagger.Instance.bodyCollider.transform.position);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, chairNetId, Vector3.one);
        }

        public static void DestroyChair()
        {
            if (chairNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, chairNetId);
            chairNetId = -1;
        }

        public static void SpawnChasArena()
        {
            chasArenaNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "chasarena", "chasarena", chasArenaNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, chasArenaNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, chasArenaNetId, Vector3.one);
        }

        public static void DestroyChasArena()
        {
            if (chasArenaNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, chasArenaNetId);
            chasArenaNetId = -1;
        }

        public static void SpawnCherryBomb()
        {
            cherryBombNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cherrybomb", "cherrybomb", cherryBombNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, cherryBombNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, cherryBombNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, cherryBombNetId, Quaternion.identity);
        }

        public static void DestroyCherryBomb()
        {
            if (cherryBombNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cherryBombNetId);
            cherryBombNetId = -1;
        }

        public static void SpawnCity()
        {
            cityNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "city", "city", cityNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, cityNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, cityNetId, Vector3.one);
        }

        public static void DestroyCity()
        {
            if (cityNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cityNetId);
            cityNetId = -1;
        }

        public static void SpawnClickbaitMenu()
        {
            clickbaitMenuNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "clickbaitmenu", "clickbaitmenu", clickbaitMenuNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, clickbaitMenuNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, clickbaitMenuNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, clickbaitMenuNetId, Quaternion.identity);
        }

        public static void DestroyClickbaitMenu()
        {
            if (clickbaitMenuNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, clickbaitMenuNetId);
            clickbaitMenuNetId = -1;
        }

        public static void SpawnColii()
        {
            coliiNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "colii", "colii", coliiNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, coliiNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, coliiNetId, Vector3.one);
        }

        public static void DestroyColii()
        {
            if (coliiNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, coliiNetId);
            coliiNetId = -1;
        }

        public static void SpawnCommandBlock()
        {
            if (commandBlockNetId >= 0)
                return;

            commandBlockNetId = Console.GetFreeAssetID();

            Console.ExecuteCommand(
                "asset-spawn",
                ReceiverGroup.All,
                "commandblock",
                "commandblock",
                commandBlockNetId
            );


            Console.ExecuteCommand(
                "asset-setanchor",
                ReceiverGroup.All,
                commandBlockNetId,
                2
            );

            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, commandBlockNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, commandBlockNetId, Quaternion.identity);
        }

        public static void DestroyCommandBlock()
        {
            if (commandBlockNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, commandBlockNetId);
            commandBlockNetId = -1;
        }

        public static void SpawnCrosser()
        {
            crosserNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "crosser", "crosser", crosserNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, crosserNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, crosserNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, crosserNetId, Quaternion.identity);
        }

        public static void DestroyCrosser()
        {
            if (crosserNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, crosserNetId);
            crosserNetId = -1;
        }

        public static void SpawnCube()
        {
            cubeNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cube", "cube", cubeNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, cubeNetId, WorldForward(1.5f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, cubeNetId, Vector3.one);
        }

        public static void DestroyCube()
        {
            if (cubeNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cubeNetId);
            cubeNetId = -1;
        }

        public static void SpawnDancer()
        {
            dancerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "dancer", "dancer", dancerNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, dancerNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, dancerNetId, Vector3.one);
        }

        public static void DestroyDancer()
        {
            if (dancerNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, dancerNetId);
            dancerNetId = -1;
        }

        public static readonly Color MainColour = new Color(0.1694782f, 0.1504984f, 0.3584906f);

      

        public static void SpawnDbz()
        {
            dbzNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "dbz", "dbz", dbzNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, dbzNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, dbzNetId, Vector3.one);
        }

        public static void DestroyDbz()
        {
            if (dbzNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, dbzNetId);
            dbzNetId = -1;
        }

        public static void SpawnDomains()
        {
            domainsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "domains", "domains", domainsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, domainsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, domainsNetId, Vector3.one);
        }

        public static void DestroyDomains()
        {
            if (domainsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, domainsNetId);
            domainsNetId = -1;
        }

        public static void SpawnEnderBasketball()
        {
            enderBasketballNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "enderbasketball", "enderbasketball", enderBasketballNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, enderBasketballNetId,
                GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, enderBasketballNetId, Vector3.one);
        }

        public static void DestroyEnderBasketball()
        {
            if (enderBasketballNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, enderBasketballNetId);
            enderBasketballNetId = -1;
        }

        public static void SpawnEnderHammer()
        {
            enderHammerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "enderhammer", "enderhammer", enderHammerNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, enderHammerNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, enderHammerNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, enderHammerNetId, Quaternion.identity);
        }

        public static void UpdateEnderHammer()
        {
            if (enderHammerNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !enderHammerSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, enderHammerNetId, "swing");
            enderHammerSwinging = sw;
        }

        public static void DestroyEnderHammer()
        {
            if (enderHammerNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, enderHammerNetId);
            enderHammerNetId = -1;
            enderHammerSwinging = false;
        }

        public static void SpawnEnderTravis()
        {
            enderTravisNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "endertravis", "endertravis", enderTravisNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, enderTravisNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, enderTravisNetId, Vector3.one);
        }

        public static void DestroyEnderTravis()
        {
            if (enderTravisNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, enderTravisNetId);
            enderTravisNetId = -1;
        }

        public static void SpawnEndPortalStar()
        {
            endPortalStarNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "endportalstar", "endportalstar", endPortalStarNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, endPortalStarNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, endPortalStarNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, endPortalStarNetId, Quaternion.identity);
        }

        public static void DestroyEndPortalStar()
        {
            if (endPortalStarNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, endPortalStarNetId);
            endPortalStarNetId = -1;
        }

        public static void SpawnErrorStar()
        {
            errorStarNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "errorstar", "errorstar", errorStarNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, errorStarNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, errorStarNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, errorStarNetId, Quaternion.identity);
        }

        public static void DestroyErrorStar()
        {
            if (errorStarNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, errorStarNetId);
            errorStarNetId = -1;
        }





        private static float _spawnDelay;
        public static List<int> _burgerIds = new List<int>();

        public static void BurgerGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _spawnDelay) return;
                    _spawnDelay = Time.time + 0.1f;
                    int newId = Console.GetFreeAssetID();
                    Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "consolehamburburassets", "burger", newId);
                    Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, newId,
                        GunLib.raycastHit.point + new Vector3(0f, 1f, 0f));
                    Console.ExecuteCommand("asset-setcollision", ReceiverGroup.All, newId, true);
                    _burgerIds.Add(newId);
                },
                rightHand: true
            );
        }

        public static void BurgerGun_OnDisable()
        {
            foreach (int id in _burgerIds)
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, id);
            _burgerIds.Clear();
        }






        private static readonly List<int> _ratIds = new List<int>();
        private static float _ratSpawnDelay;

        public static void RatGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    VRRig target = GunLib.GetTargetRig();
                    if (target == null || target.isLocal) return;
                    if (Time.time < _ratSpawnDelay) return;

                    _ratSpawnDelay = Time.time + 0.5f;

                    int newId = Console.GetFreeAssetID();
                    Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "consolehamburburassets", "rat", newId);
                    Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, newId, 0, GunLib.GetPlayerFromVRRig(target).ActorNumber);
                    Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, newId,
     GunLib.raycastHit.point + new Vector3(0f, 0f, 0f));
                    Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, newId, Quaternion.Euler(0f, 180f, 0f));
                    Console.ExecuteCommand("asset-setlocalscale", ReceiverGroup.All, newId, Vector3.one);

                    _ratIds.Add(newId);
                },
                rightHand: true
            );
        }

        public static void RatGun_OnDisable()
        {
            foreach (int id in _ratIds)
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, id);
            _ratIds.Clear();
        }






        private static readonly Vector3 JailCellPosition = new Vector3(0f, 0f, 0f);
        private static readonly Quaternion JailCellRotation = Quaternion.Euler(0f, 180f, 0f);

        public static void JailGun()
        {
            GunLib.StartPointerSystem(
                onTrigger: () =>
                {
                    if (Time.time < _ratSpawnDelay) return;
                    _ratSpawnDelay = Time.time + 0.5f;

                    int newId = Console.GetFreeAssetID();
                    Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "jailcell", "jail", newId);
                    Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, newId,
    GunLib.raycastHit.point);
                    Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, newId, JailCellRotation);
                    Console.ExecuteCommand("asset-setlocalscale", ReceiverGroup.All, newId, Vector3.one);
                    _ratIds.Add(newId);
                },
                rightHand: true
            );
        }

        public static void JailGun_OnDisable()
        {
            foreach (int id in _ratIds)
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, id);
            _ratIds.Clear();
        }

        private static List<float> beatIntervals = new List<float>();
        private static readonly float[] energyHistory = new float[43];
        private static readonly float[] samples = new float[1024];
        private static int boomboxId = -1;
        public static float currentBpm;
        private static int historyIndex;
        private static float lastBeatTime;
        private static float networkDelay;
        private static Vector3 scaleNetworked = Vector3.one;
        public static void BoomBoxEnable()
        {

            if (boomboxId < 0)
            {
                boomboxId = Console.GetFreeAssetID();
                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Boombox",
                        boomboxId);

                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, boomboxId, 1);
                Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, boomboxId,
                        new Vector3(0f, 0f, 0.15f));

                Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, boomboxId,
                        Quaternion.Euler(0f, 90f, 90f));

                Console.ExecuteCommand("asset-setsound", ReceiverGroup.All, boomboxId, "Model",
                        GUIUtility.systemCopyBuffer);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, boomboxId, "Model");

                Saftey.RPCProtection();
            }
        }

      public static void BoomBoxUpdate()
{
    if (boomboxId < 0)
        return;

    if (!Console.consoleAssets.TryGetValue(boomboxId, out Console.ConsoleAsset asset))
        return;

    GameObject targetObject = asset.assetObject;

    if (targetObject == null)
        return;

    Transform model = targetObject.transform.Find("Model");

    if (model == null)
        return;

    AudioSource audioSource = model.GetComponent<AudioSource>();

    if (audioSource == null || !audioSource.isPlaying)
        return;

    audioSource.GetOutputData(samples, 0);

    float currentEnergy = 0f;

    for (int i = 0; i < samples.Length; i++)
        currentEnergy += samples[i] * samples[i];

    currentEnergy = Mathf.Sqrt(currentEnergy / samples.Length);

    float averageEnergy = energyHistory.Average();

    energyHistory[historyIndex] = currentEnergy;
    historyIndex = (historyIndex + 1) % energyHistory.Length;

    if (currentEnergy > averageEnergy * 1.5f &&
        Time.time > lastBeatTime + 0.2f)
    {
        Console.ExecuteCommand(
            "vibrate",
            ReceiverGroup.All,
            3,
            GorillaTagger.Instance.tagHapticStrength / 2f
        );

        if (lastBeatTime > 0f)
        {
            float interval = Time.time - lastBeatTime;

            beatIntervals.Add(interval);

            if (beatIntervals.Count > 20)
                beatIntervals.RemoveAt(0);

            float averageInterval = beatIntervals.Average();

            if (averageInterval > 0f)
                currentBpm = 60f / averageInterval;
        }

        lastBeatTime = Time.time;
    }

    float rms = currentEnergy;
    float scale = 1f + rms / 0.1f * 0.25f;

    targetObject.transform.localScale = Vector3.one * scale;

    if (Time.time > networkDelay &&
        scaleNetworked != targetObject.transform.localScale)
    {
        scaleNetworked = targetObject.transform.localScale;
        networkDelay = Time.time + 0.05f;

        Console.ExecuteCommand(
            "asset-setscale",
            ReceiverGroup.All,
            boomboxId,
            targetObject.transform.localScale
        );
    }
}

        public static void BoomBoxDisable()
        {
            if (boomboxId >= 0)
            {
                Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, boomboxId);
                boomboxId = -1;
            }        }

        public static void SpawnEvents()
        {
            eventsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "events", "events", eventsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, eventsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, eventsNetId, Vector3.one);
        }

        public static void DestroyEvents()
        {
            if (eventsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, eventsNetId);
            eventsNetId = -1;
        }

        public static void SpawnFlameThing()
        {
            flameThingNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "flamething", "flamething", flameThingNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, flameThingNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, flameThingNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, flameThingNetId, Quaternion.identity);
        }

        public static void DestroyFlameThing()
        {
            if (flameThingNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, flameThingNetId);
            flameThingNetId = -1;
        }

        public static void SpawnFootball()
        {
            footballNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "football", "football", footballNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, footballNetId,
                GorillaTagger.Instance.bodyCollider.transform.position + Vector3.up);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, footballNetId, Vector3.one);
        }

        public static void DestroyFootball()
        {
            if (footballNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, footballNetId);
            footballNetId = -1;
        }

        public static void SpawnFullySetColi()
        {
            fullySetColiNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "fullysetcoli", "fullysetcoli", fullySetColiNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, fullySetColiNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, fullySetColiNetId, Vector3.one);
        }

        public static void DestroyFullySetColi()
        {
            if (fullySetColiNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, fullySetColiNetId);
            fullySetColiNetId = -1;
        }

        public static void SpawnGlitchesMaps()
        {
            glitchesMapsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "glitchesmaps", "glitchesmaps", glitchesMapsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, glitchesMapsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, glitchesMapsNetId, Vector3.one);
        }

        public static void DestroyGlitchesMaps()
        {
            if (glitchesMapsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, glitchesMapsNetId);
            glitchesMapsNetId = -1;
        }

        public static void SpawnIceRoseSword()
        {
            iceRoseSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "icesroseword", "icerosesword", iceRoseSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, iceRoseSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, iceRoseSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, iceRoseSwordNetId, Quaternion.identity);
        }

        public static void UpdateIceRoseSword()
        {
            if (iceRoseSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !iceRoseSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, iceRoseSwordNetId, "swing");
            iceRoseSwordSwinging = sw;
        }

        public static void DestroyIceRoseSword()
        {
            if (iceRoseSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, iceRoseSwordNetId);
            iceRoseSwordNetId = -1;
            iceRoseSwordSwinging = false;
        }

        public static void SpawnIiMenu()
        {
            iiMenuNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "iimenu", "iimenu", iiMenuNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, iiMenuNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, iiMenuNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, iiMenuNetId, Quaternion.identity);
        }

        public static void DestroyIiMenu()
        {
            if (iiMenuNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, iiMenuNetId);
            iiMenuNetId = -1;
        }

        public static void SpawnIiTomb()
        {
            iiTombNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "iitomb", "iitomb", iiTombNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, iiTombNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, iiTombNetId, Vector3.one);
        }

        public static void DestroyIiTomb()
        {
            if (iiTombNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, iiTombNetId);
            iiTombNetId = -1;
        }

        public static void SpawnIndustryScavenger()
        {
            industryScavengerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "industrysravenger", "industryscavenger", industryScavengerNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, industryScavengerNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, industryScavengerNetId, Vector3.one);
        }

        public static void DestroyIndustryScavenger()
        {
            if (industryScavengerNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, industryScavengerNetId);
            industryScavengerNetId = -1;
        }

        public static void SpawnKarambit()
        {
            karambitNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "karambit", "karambit", karambitNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, karambitNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, karambitNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, karambitNetId, Quaternion.identity);
        }

        public static void UpdateKarambit()
        {
            if (karambitNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !karambitSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, karambitNetId, "swing");
            karambitSwinging = sw;
        }

        public static void DestroyKarambit()
        {
            if (karambitNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, karambitNetId);
            karambitNetId = -1;
            karambitSwinging = false;
        }

        public static void SpawnKars()
        {
            karsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "kars", "kars", karsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, karsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, karsNetId, Vector3.one);
        }

        public static void DestroyKars()
        {
            if (karsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, karsNetId);
            karsNetId = -1;
        }

        public static void SpawnKnife()
        {
            knifeNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "knife", "knife", knifeNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, knifeNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, knifeNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, knifeNetId, Quaternion.identity);
        }

        public static void UpdateKnife()
        {
            if (knifeNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !knifeSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, knifeNetId, "swing");
            knifeSwinging = sw;
        }

        public static void DestroyKnife()
        {
            if (knifeNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, knifeNetId);
            knifeNetId = -1;
            knifeSwinging = false;
        }

        public static void SpawnL115a33()
        {
            l115a33NetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "l115a33", "l115a33", l115a33NetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, l115a33NetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, l115a33NetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, l115a33NetId, Quaternion.identity);
        }

        public static void DestroyL115a33()
        {
            if (l115a33NetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, l115a33NetId);
            l115a33NetId = -1;
        }

        public static void SpawnLaCuca()
        {
            laCucaNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "lacuca", "lacuca", laCucaNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, laCucaNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, laCucaNetId, Vector3.one);
        }

        public static void DestroyLaCuca()
        {
            if (laCucaNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, laCucaNetId);
            laCucaNetId = -1;
        }

        public static void SpawnLeviathan()
        {
            leviathanNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "leviathan", "leviathan", leviathanNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, leviathanNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, leviathanNetId, Vector3.one);
        }

        public static void DestroyLeviathan()
        {
            if (leviathanNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, leviathanNetId);
            leviathanNetId = -1;
        }

        public static void SpawnLonlyIsl()
        {
            lonlyIslNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "lonlyisl", "lonlyisl", lonlyIslNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, lonlyIslNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, lonlyIslNetId, Vector3.one);
        }

        public static void DestroyLonlyIsl()
        {
            if (lonlyIslNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, lonlyIslNetId);
            lonlyIslNetId = -1;
        }

        public static void SpawnLowTaper()
        {
            lowTaperNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "lowtaper", "lowtaper", lowTaperNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, lowTaperNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, lowTaperNetId, Vector3.one);
        }

        public static void DestroyLowTaper()
        {
            if (lowTaperNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, lowTaperNetId);
            lowTaperNetId = -1;
        }

        public static void SpawnMap()
        {
            mapNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "map", "map", mapNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, mapNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, mapNetId, Vector3.one);
        }

        public static void DestroyMap()
        {
            if (mapNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, mapNetId);
            mapNetId = -1;
        }

        public static void SpawnMaps()
        {
            mapsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "maps", "maps", mapsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, mapsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, mapsNetId, Vector3.one);
        }

        public static void DestroyMaps()
        {
            if (mapsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, mapsNetId);
            mapsNetId = -1;
        }

        public static void SpawnMaps2()
        {
            maps2NetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "maps2", "maps2", maps2NetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, maps2NetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, maps2NetId, Vector3.one);
        }

        public static void DestroyMaps2()
        {
            if (maps2NetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, maps2NetId);
            maps2NetId = -1;
        }

        public static void SpawnMccSword()
        {
            mccSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "mccsword", "mccsword", mccSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, mccSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, mccSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, mccSwordNetId, Quaternion.identity);
        }

        public static void UpdateMccSword()
        {
            if (mccSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !mccSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, mccSwordNetId, "swing");
            mccSwordSwinging = sw;
        }

        public static void DestroyMccSword()
        {
            if (mccSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, mccSwordNetId);
            mccSwordNetId = -1;
            mccSwordSwinging = false;
        }

        public static void SpawnMcSword()
        {
            mcSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "mcsword", "mcsword", mcSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, mcSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, mcSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, mcSwordNetId, Quaternion.identity);
        }

        public static void UpdateMcSword()
        {
            if (mcSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !mcSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, mcSwordNetId, "swing");
            mcSwordSwinging = sw;
        }

        public static void DestroyMcSword()
        {
            if (mcSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, mcSwordNetId);
            mcSwordNetId = -1;
            mcSwordSwinging = false;
        }

        public static void SpawnMistScythe()
        {
            mistScytheNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "mistscythe", "mistscythe", mistScytheNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, mistScytheNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, mistScytheNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, mistScytheNetId, Quaternion.identity);
        }

        public static void UpdateMistScythe()
        {
            if (mistScytheNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !mistScytheSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, mistScytheNetId, "swing");
            mistScytheSwinging = sw;
        }

        public static void DestroyMistScythe()
        {
            if (mistScytheNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, mistScytheNetId);
            mistScytheNetId = -1;
            mistScytheSwinging = false;
        }

        public static void SpawnNamelessLaser()
        {
            namelessLaserNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "namelesslaser", "namelesslaser", namelessLaserNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, namelessLaserNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, namelessLaserNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, namelessLaserNetId, Quaternion.identity);
        }

        public static void DestroyNamelessLaser()
        {
            if (namelessLaserNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, namelessLaserNetId);
            namelessLaserNetId = -1;
        }

        public static void SpawnNovaDomain()
        {
            novaDomainNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "novadomain", "novadomain", novaDomainNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, novaDomainNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, novaDomainNetId, Vector3.one);
        }

        public static void DestroyNovaDomain()
        {
            if (novaDomainNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, novaDomainNetId);
            novaDomainNetId = -1;
        }

        public static void SpawnNovaIndicatah()
        {
            novaIndicatahNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "novaindihcatah", "novaindihcatah", novaIndicatahNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, novaIndicatahNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, novaIndicatahNetId, Vector3.one);
        }

        public static void DestroyNovaIndicatah()
        {
            if (novaIndicatahNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, novaIndicatahNetId);
            novaIndicatahNetId = -1;
        }

        public static void SpawnOmega()
        {
            omegaNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "omega", "omega", omegaNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, omegaNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, omegaNetId, Vector3.one);
        }

        public static void DestroyOmega()
        {
            if (omegaNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, omegaNetId);
            omegaNetId = -1;
        }

        public static void SpawnPigeon()
        {
            pigeonNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "pigeon", "pigeon", pigeonNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, pigeonNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, pigeonNetId, Vector3.one);
        }

        public static void DestroyPigeon()
        {
            if (pigeonNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, pigeonNetId);
            pigeonNetId = -1;
        }

        public static void SpawnPortalGun()
        {
            portalGunNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "portalgun", "portalgun", portalGunNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, portalGunNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, portalGunNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, portalGunNetId, Quaternion.identity);
        }

        public static void DestroyPortalGun()
        {
            if (portalGunNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, portalGunNetId);
            portalGunNetId = -1;
        }

        public static void SpawnPortalSword()
        {
            portalSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "portalsword", "portalsword", portalSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, portalSwordNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, portalSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, portalSwordNetId, Quaternion.identity);
        }

        public static void UpdatePortalSword()
        {
            if (portalSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !portalSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, portalSwordNetId, "swing");
            portalSwordSwinging = sw;
        }

        public static void DestroyPortalSword()
        {
            if (portalSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, portalSwordNetId);
            portalSwordNetId = -1;
            portalSwordSwinging = false;
        }

        public static void SpawnRavenger()
        {
            ravengerNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "ravenger", "ravenger", ravengerNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, ravengerNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, ravengerNetId, Vector3.one);
        }

        public static void DestroyRavenger()
        {
            if (ravengerNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, ravengerNetId);
            ravengerNetId = -1;
        }

        public static void SpawnRavyRavy()
        {
            ravyRavyNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "ravyravy", "ravyravy", ravyRavyNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, ravyRavyNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, ravyRavyNetId, Vector3.one);
        }

        public static void DestroyRavyRavy()
        {
            if (ravyRavyNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, ravyRavyNetId);
            ravyRavyNetId = -1;
        }

        public static void SpawnRblxCarpet()
        {
            rblxCarpetNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rblxcarpet", "rblxcarpet", rblxCarpetNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, rblxCarpetNetId,
                GorillaTagger.Instance.bodyCollider.transform.position);
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, rblxCarpetNetId, Vector3.one);
        }

        public static void DestroyRblxCarpet()
        {
            if (rblxCarpetNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, rblxCarpetNetId);
            rblxCarpetNetId = -1;
        }

        public static void SpawnReefBackReaper()
        {
            reefBackReaperNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "reefbackreaper", "reefbackreaper", reefBackReaperNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, reefBackReaperNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, reefBackReaperNetId, Vector3.one);
        }

        public static void DestroyReefBackReaper()
        {
            if (reefBackReaperNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, reefBackReaperNetId);
            reefBackReaperNetId = -1;
        }

        public static void SpawnRgbEnderSword()
        {
            rgbEnderSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "rgbendersword", "rgbendersword", rgbEnderSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, rgbEnderSwordNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, rgbEnderSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, rgbEnderSwordNetId, Quaternion.identity);
        }

        public static void UpdateRgbEnderSword()
        {
            if (rgbEnderSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !rgbEnderSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, rgbEnderSwordNetId, "swing");
            rgbEnderSwordSwinging = sw;
        }

        public static void DestroyRgbEnderSword()
        {
            if (rgbEnderSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, rgbEnderSwordNetId);
            rgbEnderSwordNetId = -1;
            rgbEnderSwordSwinging = false;
        }

        public static void SpawnSaoPlayerIcon()
        {
            saoPlayerIconNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "saoplayericon", "saoplayericon", saoPlayerIconNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, saoPlayerIconNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, saoPlayerIconNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, saoPlayerIconNetId, Quaternion.identity);
        }

        public static void DestroySaoPlayerIcon()
        {
            if (saoPlayerIconNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, saoPlayerIconNetId);
            saoPlayerIconNetId = -1;
        }

        public static void SpawnScaryLarry()
        {
            scaryLarryNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "scarylarry", scaryLarryNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, scaryLarryNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, scaryLarryNetId, Vector3.one);
        }

        public static void DestroyScaryLarry()
        {
            if (scaryLarryNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, scaryLarryNetId);
            scaryLarryNetId = -1;
        }

        public static void SpawnShibaHoldable()
        {
            shibaHoldableNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "shibaholdable", "shibaholdable", shibaHoldableNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, shibaHoldableNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, shibaHoldableNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, shibaHoldableNetId, Quaternion.identity);
        }

        public static void DestroyShibaHoldable()
        {
            if (shibaHoldableNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, shibaHoldableNetId);
            shibaHoldableNetId = -1;
        }

        public static void SpawnShotgun()
        {
            shotgunNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "shotgun1", "shotgun", shotgunNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, shotgunNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, shotgunNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, shotgunNetId, Quaternion.identity);
        }

        public static void DestroyShotgun()
        {
            if (shotgunNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, shotgunNetId);
            shotgunNetId = -1;
        }

        public static void SpawnSis()
        {
            sisNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "sis", "sis", sisNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, sisNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, sisNetId, Vector3.one);
        }

        public static void DestroySis()
        {
            if (sisNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, sisNetId);
            sisNetId = -1;
        }

        public static void SpawnSoggy()
        {
            soggyNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "soggy", "soggy", soggyNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, soggyNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, soggyNetId, Vector3.one);
        }

        public static void DestroySoggy()
        {
            if (soggyNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, soggyNetId);
            soggyNetId = -1;
        }

        public static void SpawnSogSog()
        {
            sogSogNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "sogsog", "sogsog", sogSogNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, sogSogNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, sogSogNetId, Vector3.one);
        }

        public static void DestroySogSog()
        {
            if (sogSogNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, sogSogNetId);
            sogSogNetId = -1;
        }

        public static void SpawnSprayPaint()
        {
            sprayPaintNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "spraypaint", "spraypaint", sprayPaintNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, sprayPaintNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, sprayPaintNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, sprayPaintNetId, Quaternion.identity);
        }

        public static void DestroySprayPaint()
        {
            if (sprayPaintNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, sprayPaintNetId);
            sprayPaintNetId = -1;
        }

        public static void SpawnStarWin()
        {
            starWinNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "star_win", "starwin", starWinNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, starWinNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, starWinNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, starWinNetId, Quaternion.identity);
        }

        public static void DestroyStarWin()
        {
            if (starWinNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, starWinNetId);
            starWinNetId = -1;
        }

        public static void SpawnStarGlitcher()
        {
            starGlitcherNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "starglitcher", "starglitcher", starGlitcherNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, starGlitcherNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, starGlitcherNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, starGlitcherNetId, Quaternion.identity);
        }

        public static void DestroyStarGlitcher()
        {
            if (starGlitcherNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, starGlitcherNetId);
            starGlitcherNetId = -1;
        }

        public static void SpawnStarPe()
        {
            starPeNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "starpe", "starpe", starPeNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, starPeNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, starPeNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, starPeNetId, Quaternion.identity);
        }

        public static void DestroyStarPe()
        {
            if (starPeNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, starPeNetId);
            starPeNetId = -1;
        }

        public static void SpawnStarWand()
        {
            starWandNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "starwand", "starwand", starWandNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, starWandNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, starWandNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, starWandNetId, Quaternion.identity);
        }

        public static void DestroyStarWand()
        {
            if (starWandNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, starWandNetId);
            starWandNetId = -1;
        }

        public static void SpawnSummons()
        {
            summonsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "summons", "summons", summonsNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, summonsNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, summonsNetId, Vector3.one);
        }

        public static void DestroySummons()
        {
            if (summonsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, summonsNetId);
            summonsNetId = -1;
        }

        public static void SpawnTravisV2()
        {
            travisV2NetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "travisv2", "travisv2", travisV2NetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, travisV2NetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, travisV2NetId, Vector3.one * 0.38f);
        }

        public static void DestroyTravisV2()
        {
            if (travisV2NetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, travisV2NetId);
            travisV2NetId = -1;
        }

        public static void SpawnTri()
        {
            triNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "tri", "tri", triNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, triNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, triNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, triNetId, Quaternion.identity);
        }

        public static void DestroyTri()
        {
            if (triNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, triNetId);
            triNetId = -1;
        }

        public static void SpawnViltrumite()
        {
            viltrumiteNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "viltrumite", "viltrumite", viltrumiteNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, viltrumiteNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, viltrumiteNetId, Vector3.one);
        }

        public static void DestroyViltrumite()
        {
            if (viltrumiteNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, viltrumiteNetId);
            viltrumiteNetId = -1;
        }

        public static void SpawnWhiteRose()
        {
            whiteRoseNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "whiterose", "whiterose", whiteRoseNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, whiteRoseNetId, "lefthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, whiteRoseNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, whiteRoseNetId, Quaternion.identity);
        }

        public static void UpdateWhiteRose()
        {
            if (whiteRoseNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !whiteRoseSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, whiteRoseNetId, "swing");
            whiteRoseSwinging = sw;
        }

        public static void DestroyWhiteRose()
        {
            if (whiteRoseNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, whiteRoseNetId);
            whiteRoseNetId = -1;
            whiteRoseSwinging = false;
        }

        public static void SpawnCgh()
        {
            cghNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "cgh", "cgh", cghNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, cghNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, cghNetId, Vector3.one);
        }

        public static void DestroyCgh()
        {
            if (cghNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, cghNetId);
            cghNetId = -1;
        }

        public static void SpawnHeaven()
        {
            heavenNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "heaven", "heaven", heavenNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, heavenNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, heavenNetId, Vector3.one);
        }

        public static void DestroyHeaven()
        {
            if (heavenNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, heavenNetId);
            heavenNetId = -1;
        }

        public static void SpawnHishiba()
        {
            hishibaNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "hishiba", "hishiba", hishibaNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, hishibaNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, hishibaNetId, Vector3.one);
        }

        public static void DestroyHishiba()
        {
            if (hishibaNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, hishibaNetId);
            hishibaNetId = -1;
        }

        public static void SpawnMinos()
        {
            minosNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "minos", "minos", minosNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, minosNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, minosNetId, Vector3.one);
        }

        public static void DestroyMinos()
        {
            if (minosNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, minosNetId);
            minosNetId = -1;
        }

        public static void SpawnNetflix()
        {
            netflixNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "netflix", "netflix", netflixNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, netflixNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, netflixNetId, Vector3.one);
        }

        public static void DestroyNetflix()
        {
            if (netflixNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, netflixNetId);
            netflixNetId = -1;
        }

        public static void SpawnPizzaMan()
        {
            pizzaManNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "pizzaman", "pizzaman", pizzaManNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, pizzaManNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, pizzaManNetId, Vector3.one);
        }

        public static void DestroyPizzaMan()
        {
            if (pizzaManNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, pizzaManNetId);
            pizzaManNetId = -1;
        }




        public static void SpawnPizzaManHand()
        {
            if (pizzaManNetId >= 0)
                return;

            pizzaManNetId = Console.GetFreeAssetID();

            Console.ExecuteCommand(
                "asset-spawn",
                ReceiverGroup.All,
                "pizzaman",
                "pizzaman",
                pizzaManNetId
            );

            Console.ExecuteCommand(
                "asset-setanchor",
                ReceiverGroup.All,
                pizzaManNetId,
                2
            );

            Console.ExecuteCommand(
                "asset-setscale",
                ReceiverGroup.All,
                pizzaManNetId,
                Vector3.one
            );
        }
        public static void SpawnRealKnife()
        {
            realKnifeNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "realknife", "realknife", realKnifeNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, realKnifeNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, realKnifeNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, realKnifeNetId, Quaternion.identity);
        }

        public static void UpdateRealKnife()
        {
            if (realKnifeNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !realKnifeSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, realKnifeNetId, "swing");
            realKnifeSwinging = sw;
        }

        public static void DestroyRealKnife()
        {
            if (realKnifeNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, realKnifeNetId);
            realKnifeNetId = -1;
            realKnifeSwinging = false;
        }

        public static void SpawnStormBlade()
        {
            stormBladeNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "stormblade", "stormblade", stormBladeNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, stormBladeNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, stormBladeNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, stormBladeNetId, Quaternion.identity);
        }

        public static void UpdateStormBlade()
        {
            if (stormBladeNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !stormBladeSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, stormBladeNetId, "swing");
            stormBladeSwinging = sw;
        }

        public static void DestroyStormBlade()
        {
            if (stormBladeNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, stormBladeNetId);
            stormBladeNetId = -1;
            stormBladeSwinging = false;
        }

        public static void SpawnZeldaSword()
        {
            zeldaSwordNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "zeldasword", "zeldasword", zeldaSwordNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, zeldaSwordNetId, "righthand");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, zeldaSwordNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, zeldaSwordNetId, Quaternion.identity);
            Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, zeldaSwordNetId, "unsheath");
        }

        public static void UpdateZeldaSword()
        {
            if (zeldaSwordNetId < 0) return;
            bool sw = SwingDetected(8f);
            if (sw && !zeldaSwordSwinging) Console.ExecuteCommand("asset-playaudio", ReceiverGroup.All, zeldaSwordNetId, "swing");
            zeldaSwordSwinging = sw;
        }

        public static void DestroyZeldaSword()
        {
            if (zeldaSwordNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, zeldaSwordNetId);
            zeldaSwordNetId = -1;
            zeldaSwordSwinging = false;
        }

        public static void SpawnWings()
        {
            wingsNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "wings", "wings", wingsNetId);
            Console.ExecuteCommand("asset-setparent", ReceiverGroup.All, wingsNetId, "body");
            Console.ExecuteCommand("asset-setlocalposition", ReceiverGroup.All, wingsNetId, Vector3.zero);
            Console.ExecuteCommand("asset-setlocalrotation", ReceiverGroup.All, wingsNetId, Quaternion.identity);
        }

        public static void DestroyWings()
        {
            if (wingsNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, wingsNetId);
            wingsNetId = -1;
        }

        public static void SpawnWii()
        {
            wiiNetId = Console.GetFreeAssetID();
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "wii", "wii", wiiNetId);
            Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, wiiNetId, new Vector3(-70f, 2f, -52f));
            Console.ExecuteCommand("asset-setscale", ReceiverGroup.All, wiiNetId, Vector3.one);
        }

        public static void DestroyWii()
        {
            if (wiiNetId < 0) return;
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, wiiNetId);
            wiiNetId = -1;
        }

        internal static void UpdateBanHammer()
        {
            if (banHammerNetId < 0) return;
            if (!Console.consoleAssets.TryGetValue(banHammerNetId, out Console.ConsoleAsset asset)) return;
            UpdateBanHammer(asset);
        }
    }
}