using LagMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using HarmonyLib;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using LagMenu.Menu;

namespace LagMenu.Mods
{
    public static class UsefulManager
    {
        private const int StumpLeaderboardIndex = 3;

        private static readonly Dictionary<string, BoardInformation> BoardInformations =
            new Dictionary<string, BoardInformation>
            {
                ["Canyon2"] = new BoardInformation(
                    "Canyon/CanyonScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-24.5019f, -28.7746f, 0.1f),
                    new Vector3(270f, 0f, 0f),
                    new Vector3(21.5946f, 1f, 22.1782f)),
                ["Skyjungle"] = new BoardInformation(
                    "skyjungle/UI/Scoreboard/GorillaScoreBoard",
                    new Vector3(-21.2764f, -32.1928f, 0f),
                    new Vector3(270.2987f, 0.2f, 359.9f),
                    new Vector3(21.6f, 0.1f, 20.4909f)),
                ["Mountain"] = new BoardInformation(
                    "Mountain/MountainScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-21.2764f, -32.1928f, 0f),
                    new Vector3(270.2987f, 0.2f, 359.9f),
                    new Vector3(21.6f, 0.1f, 20.4909f)),
                ["Metropolis"] = new BoardInformation(
                    "MetroMain/ComputerArea/Scoreboard/GorillaScoreBoard",
                    new Vector3(-25.1f, -31f, 0.1502f),
                    new Vector3(270.1958f, 0.2086f, 0f),
                    new Vector3(21f, 102.9727f, 21.4f)),
                ["Bayou"] = new BoardInformation(
                    "BayouMain/ComputerArea/GorillaScoreBoardPhysical",
                    new Vector3(-28.3419f, -26.851f, 0.3f),
                    new Vector3(270f, 0f, 0f),
                    new Vector3(21.3636f, 38f, 21f)),
                ["Beach"] = new BoardInformation(
                    "BeachScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-22.1964f, -33.7126f, 0.1f),
                    new Vector3(270.056f, 0f, 0f),
                    new Vector3(21.2f, 2f, 21.6f)),
                ["Cave"] = new BoardInformation(
                    "Cave_Main_Prefab/CrystalCaveScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-22.1964f, -33.7126f, 0.1f),
                    new Vector3(270.056f, 0f, 0f),
                    new Vector3(21.2f, 2f, 21.6f)),
                ["Rotating"] = new BoardInformation(
                    "RotatingPermanentEntrance/UI (1)/RotatingScoreboard/RotatingScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-22.1964f, -33.7126f, 0.1f),
                    new Vector3(270.056f, 0f, 0f),
                    new Vector3(21.2f, 2f, 21.6f)),
                ["MonkeBlocks"] = new BoardInformation(
                    "Environment Objects/MonkeBlocksRoomPersistent/AtticScoreBoard/AtticScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-22.1964f, -24.5091f, 0.57f),
                    new Vector3(270.1856f, 0.1f, 0f),
                    new Vector3(21.6f, 1.2f, 20.8f)),
                ["Basement"] = new BoardInformation(
                    "Basement/BasementScoreboardAnchor/GorillaScoreBoard/",
                    new Vector3(-22.1964f, -24.5091f, 0.57f),
                    new Vector3(270.1856f, 0.1f, 0f),
                    new Vector3(21.6f, 1.2f, 20.8f)),
                ["City"] = new BoardInformation(
                    "City_Pretty/CosmeticsScoreboardAnchor/GorillaScoreBoard",
                    new Vector3(-22.1964f, -34.9f, 0.57f),
                    new Vector3(270f, 0f, 0f),
                    new Vector3(21.6f, 2.4f, 22f)),
            };

        private static readonly Dictionary<string, GameObject> objectBoards = new Dictionary<string, GameObject>();

        private static GameObject mainBoard;
        private static bool initialized;
        private static bool hooked;


        private static bool gtvChecked = false;


        private static VideoPlayer videoPlayer;
        private static bool hasSetupVideo;

        public static bool custom = true;
        public static string[] CustomBoardTexts = new string[]
        {
            "LagMenu",
            "LagMenu",
            "if you see (MrLag Styled) that means its a Joystick click because i lowk perfer it like that if you have any issues jst tell me and yuh",
            "Uh yeah this is LagMenu the WORST menu of all time why are you using this any other menu would be better, also i am NOT responsible for any bans if you get banned that is your fault for using a cheat in a gorilla game"
        };


        public static void ChangeMapInfoText()
        {
            try
            {
                var mapInfo = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP")?.GetComponent<TextMeshPro>();
                if (mapInfo != null)
                {
                    mapInfo.richText = true;
                    mapInfo.text = "Lagmenu Is Better";
                    mapInfo.color = Color.white;
                    mapInfo.fontSize = 42;
                    mapInfo.alignment = TextAlignmentOptions.Center;
                }
            }
            catch { }
        }

        public static Material BoardMat;
        public static GameObject BoardGradientObject = null;
        public static bool IsBoardGradientEnabled = true;

        public static Color BoardColor = new Color(0.2f, 0f, 0.3f);


        public static void Update()
        {
            if (!initialized)
                Initialize();


            UpdateVideo();
            ChangeMapInfoText();

            if (custom)
                UpdateMOTD();
        }


        public static void Enable()
        {
            if (!hooked)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                hooked = true;
            }

            Initialize();
        }


        public static void Disable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            hooked = false;

            if (mainBoard != null)
            {
                Object.Destroy(mainBoard);
                mainBoard = null;
            }

            foreach (var kvp in objectBoards)
                if (kvp.Value != null)
                    Object.Destroy(kvp.Value);

            objectBoards.Clear();

            if (videoPlayer != null && videoPlayer.targetTexture != null)
            {
                videoPlayer.targetTexture.Release();
                videoPlayer.targetTexture = null;
            }
            videoPlayer = null;
            hasSetupVideo = false;

            gtvChecked = false;
            custom = true;
            initialized = false;
        }


        private static void Initialize()
        {
            try
            {

                if (mainBoard == null)
                {
                    GameObject anchor = GameObject.Find(
                        "Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard");

                    if (anchor != null)
                        mainBoard = CreateBoard(anchor.transform,
                            new Vector3(-22.1964f, -34.9f, 0.57f),
                            new Vector3(270f, 0f, 0f),
                            new Vector3(21.2f, 2f, 21.6f));
                }



                try
                {
                    GameObject mapInfoText = GameObject.Find(
                        "Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");

                    GameObject loadingText = GameObject.Find(
                        "Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");

                    if (mapInfoText != null)
                    {
                        TextMeshPro tmp = mapInfoText.GetComponent<TextMeshPro>();
                        if (tmp != null)
                            tmp.text = "<color=black>LagMenu!</color>";
                    }

                    if (loadingText != null)
                        Object.Destroy(loadingText);
                }
                catch { }

                initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] CustomBoardManager.Initialize error: " + e);
            }
        }


        public static void UpdateMOTD()
        {
            try
            {
                GameObject motdHeading = GameObject.Find(
                    "Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
                if (motdHeading != null)
                    motdHeading.GetComponent<TextMeshPro>().text = CustomBoardTexts[0];

                GameObject cocHeading = GameObject.Find(
                    "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");
                if (cocHeading != null)
                    cocHeading.GetComponent<TextMeshPro>().text = CustomBoardTexts[1];

                GameObject cocBody = GameObject.Find(
                    "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
                if (cocBody != null)
                    cocBody.GetComponent<TextMeshPro>().text = CustomBoardTexts[2];

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    GameObject motdBody = GameObject.Find(
                        "Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
                    if (motdBody != null)
                        motdBody.GetComponent<TextMeshPro>().text = CustomBoardTexts[3];

                    custom = false;
                }
            }
            catch { }
        }


        private static void UpdateVideo()
        {
            if (hasSetupVideo)
            {
                if (videoPlayer != null
                    && !videoPlayer.isPlaying
                    && videoPlayer.gameObject.activeInHierarchy
                    && videoPlayer.enabled)
                    videoPlayer.Play();
                return;
            }

            TrySetupVideo();
        }

        private static void TrySetupVideo()
        {
            try
            {
                GameObject featuredMaps = GameObject.Find(
                    "Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/");

                if (featuredMaps == null) return;


                GameObject displayTextObj = featuredMaps.transform.Find("DisplayText")?.gameObject;
                if (displayTextObj != null)
                    foreach (Transform child in displayTextObj.transform)
                        if (child.name.ToLower().EndsWith("tmp"))
                            child.gameObject.SetActive(!child.gameObject.activeSelf);

                GameObject featuredMapImage = featuredMaps.transform.Find("FeaturedMapImage")?.gameObject;
                if (featuredMapImage == null) return;


                SpriteRenderer sr = featuredMapImage.GetComponent<SpriteRenderer>();
                if (sr != null) Object.Destroy(sr);


                MeshFilter mf = featuredMapImage.GetComponent<MeshFilter>()
                             ?? featuredMapImage.AddComponent<MeshFilter>();
                mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

                MeshRenderer mr = featuredMapImage.GetComponent<MeshRenderer>()
                               ?? featuredMapImage.AddComponent<MeshRenderer>();

                Material videoMat = new Material(Shader.Find("Unlit/Texture"));
                mr.material = videoMat;


                videoPlayer = featuredMapImage.AddComponent<VideoPlayer>();
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.url = "https://files.catbox.moe/xeiy7t.mp4";
                videoPlayer.isLooping = true;

                RenderTexture rt = new RenderTexture(512, 512, 0);
                videoPlayer.targetTexture = rt;
                mr.material.mainTexture = rt;

                featuredMapImage.transform.localScale = new Vector3(0.845f, 0.445f, 1f);

                videoPlayer.Play();
                featuredMapImage.SetActive(true);

                hasSetupVideo = true;
            }
            catch { }
        }


        private static void TryChangeVideo()
        {
            if (gtvChecked) return;
            gtvChecked = true;

            GameObject vodObj = GameObject.Find("Miscellaneous Scripts/VOD");
            if (vodObj == null)
            {
                Debug.LogWarning("[LagMenu] VOD object not found");
            }
        }


        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            initialized = false;
            hasSetupVideo = false;
            videoPlayer = null;
            custom = true;
            gtvChecked = false;

            Initialize();
            Menu.Console.LoadConsole();


            if (scene.name == "City")
                TryChangeVideo();

            if (!BoardInformations.TryGetValue(scene.name, out BoardInformation info))
                return;

            CreateObjectBoard(scene.name, info.GameObjectPath,
                info.Position, info.Rotation, info.Scale);
        }


        private static void CreateObjectBoard(string scene, string path,
            Vector3 position, Vector3 rotation, Vector3 scale)
        {
            try
            {
                if (objectBoards.TryGetValue(scene, out GameObject old))
                {
                    if (old != null) Object.Destroy(old);
                    objectBoards.Remove(scene);
                }

                GameObject parent = GameObject.Find(path);
                if (parent == null) return;

                GameObject board = CreateBoard(parent.transform, position, rotation, scale);
                objectBoards[scene] = board;
            }
            catch { }
        }


        private static GameObject CreateBoard(Transform parent, Vector3 position,
            Vector3 rotation, Vector3 scale)
        {
            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Plane);
            board.transform.parent = parent;
            board.transform.localPosition = position;
            board.transform.localRotation = Quaternion.Euler(rotation);
            board.transform.localScale = scale;

            Object.Destroy(board.GetComponent<Collider>());
            ApplyColor(board.GetComponent<Renderer>());

            return board;
        }

        private static void ApplyColor(Renderer r)
        {
            if (r == null) return;
            r.material.color = BoardColor;
        }



        private struct BoardInformation
        {
            public readonly string GameObjectPath;
            public readonly Vector3 Position;
            public readonly Vector3 Rotation;
            public readonly Vector3 Scale;

            public BoardInformation(string path, Vector3 pos, Vector3 rot, Vector3 scale)
            {
                GameObjectPath = path;
                Position = pos;
                Rotation = rot;
                Scale = scale;
            }
        }
    }
}
