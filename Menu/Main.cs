using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using HarmonyLib;
using LagMenu.Menu;
using LagMenu.Mods;
using LagMenu.Utilities;
using Oculus.Interaction.Samples;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;
using WebSocketSharp;
using CommonUsages = UnityEngine.XR.CommonUsages;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace LagMenu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Main
    {
        public static GameObject menu;
        public static GameObject reference;
        public static SphereCollider buttonCollider;

        public static int category = 0;
        public static int page = 0;

        private static int lastCategory = -1;
        private static int lastPage = -1;
        public static bool closing = false;


        private static float fps;
        private static float fpsUpdateTimer;
        private static TextMeshPro fpsText;
        private static TextMeshPro title;
        public static bool useRoundedMenu = false;

        public static bool animatedTitle = true;
        public static bool doCustomName = true;
        public static string customMenuName = "LagMenu";

        public const int BUTTONS_PER_PAGE = 8;

        public static readonly string[] categoryNames =
        {
            "Main",
            "Settings",
            "Movement",
            "Rig",
            "Room",
            "Advantage",
            "Visual",
            "Saftey",
            "World",
            "OP",
            "Block",
            "Proj",
            "Experimental",
            "Admin",
            "Soundboard",
            "Admin Assets",
            "Detected",
            "Master",
            "Nothin For Now",
            "Credits",
            "Virtual Stump",
            "Networking",
            "Beta",
        };


        private static Color accentCol => ThemeChanger.CurrentAccent;
        private static Color buttonOn => ThemeChanger.Current.ButtonOn;



        public static Color BaseColor = Color.purple;


        public static void Prefix()
        {
            try
            {
                if (CoroutineRunner == null)
                {
                    GameObject runnerObj = new GameObject("LagMenu_CoroutineRunner");
                    Object.DontDestroyOnLoad(runnerObj);
                    CoroutineRunner = runnerObj.AddComponent<Utilities.ResourceRunner>();
                }
                Utilities.Tracker.Update();
                Utilities.ResourceManager.Init();
                UsefulManager.Update();
                Soundboard.Init();
                if (LagMenuOnGUI.Instance == null)
                {
                    GameObject guiObj = new GameObject("LagMenuOnGUI");
                    Object.DontDestroyOnLoad(guiObj);
                    guiObj.AddComponent<LagMenuOnGUI>();
                }


                if (Time.time > fpsUpdateTimer)
                {
                    fps = 1f / Time.unscaledDeltaTime;
                    fpsUpdateTimer = Time.time + 0.5f;

                    if (fpsText != null)
                    {
                        fpsText.text = "FPS: " + Mathf.CeilToInt(fps);
                        fpsText.color = fps >= 60f ? Color.green
                                      : fps >= 30f ? Color.yellow
                                      : Color.red;
                    }
                }


                if (fontSwapPending && Utilities.ResourceManager.FontReady && menu != null)
                {
                    fontSwapPending = false;
                    RecreateMenu();
                }

                if (animatedTitle && title != null)
                {
                    string targetString = doCustomName ? NoRichtextTags(customMenuName) : "LagMenu";
                    int length = (int)Mathf.PingPong(Time.time / 0.25f, targetString.Length + 1);
                    title.text = length > 0 ? targetString[..length] : "";
                }

                bool open = ControllerInputPoller.instance.leftControllerSecondaryButton;

                foreach (ButtonInfo[] cat in Buttons.buttons)
                    foreach (ButtonInfo btn in cat)
                        if (btn.enabled && btn.isTogglable)
                            btn.method?.Invoke();

                if (menu == null)
                {
                    if (open)
                    {
                        CreateMenu();
                        RecenterMenu();
                        if (reference == null)
                            CreateReference();
                    }
                }
                else
                {
                    if (open)
                    {
                        closing = false;
                        RecenterMenu();

                        if (category != lastCategory || page != lastPage)
                        {
                            lastCategory = category;
                            lastPage = page;
                            RecreateMenu();
                        }
                    }
                    else
                    {
                        if (!closing)
                        {
                            closing = true;



                            if (useShrinkClose)
                            {
                                CoroutineRunner.StartCoroutine(ShrinkOutCoroutine(menu));
                            }
                            else
                            {
                                Rigidbody rb = menu.AddComponent<Rigidbody>();
                                rb.linearVelocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0);
                                Object.Destroy(menu, 2f);
                            }

                            menu = null;
                            fpsText = null;
                            Object.Destroy(reference);
                            reference = null;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static readonly Quaternion LeftHandBasis =
            Quaternion.LookRotation(new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, 1f));
        private static readonly Vector3 ReadableFace = Vector3.back;


        private static Vector3 panelPivotOffset;

        public static void CreateMenu()
        {
            menu = new GameObject("LagMenu_Root");
            menu.transform.localScale = Vector3.one;

            PanelAsset.BuildPanel(menu.transform);

            ThemeChanger.Theme curTheme = ThemeChanger.Current;
            PanelAsset.RetintMaterial("shell_2a2437", curTheme.MenuBg);
            PanelAsset.RetintMaterial("violet_9d6bf5", curTheme.HomeCol);


            panelPivotOffset = Vector3.zero;
            if (PanelAsset.TryGetLocalBounds(menu.transform, out Bounds rawBounds))
            {
                panelPivotOffset = -rawBounds.center;
                foreach (Transform child in menu.transform)
                    child.localPosition += panelPivotOffset;
            }


            RefreshMenuScale();


            Transform startHand = GorillaTagger.Instance.leftHandTransform;
            Quaternion startRot = startHand.rotation * LeftHandBasis;
            menu.transform.position = startHand.position + startRot * ReadableFace * (MENU_FORWARD_OFFSET * Settings.menuDistance);
            menu.transform.rotation = startRot;

            BuildMenu();

            if (useShrinkClose)
                CoroutineRunner.StartCoroutine(GrowInCoroutine());
        }



        private static IEnumerator ShrinkOutCoroutine(GameObject menuObj)
        {
            if (menuObj == null) yield break;

            Vector3 start = menuObj.transform.localScale;
            float time = 0f;
            float duration = 0.10f;

            while (time < duration)
            {
                if (menuObj == null) yield break;
                menuObj.transform.localScale = Vector3.Lerp(start, Vector3.zero, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            if (menuObj != null)
                Object.Destroy(menuObj);

            Utilities.ResourceManager.PlayDynamicClose();
        }


        private static IEnumerator GrowInCoroutine()
        {
            Vector3 target = menu.transform.localScale;
            menu.transform.localScale = Vector3.zero;

            float time = 0f;
            float duration = 0.12f;

            while (time < duration)
            {
                if (menu == null) yield break;
                menu.transform.localScale = Vector3.Lerp(Vector3.zero, target, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            if (menu != null)
                menu.transform.localScale = target;

            Utilities.ResourceManager.PlayDynamicOpen();
        }



        public static void RefreshMenuScale()
        {
            if (menu == null) return;
            if (PanelAsset.TryGetLocalBounds(menu.transform, out Bounds panelBounds) && panelBounds.size.x > 0.0001f)
            {
                float scaleFactor = (TARGET_PANEL_WIDTH * Settings.menuScale) / panelBounds.size.x;
                menu.transform.localScale = Vector3.one * scaleFactor;
            }
        }
        public static void RecreateMenu()
        {
            if (menu == null) return;
            fpsText = null;

            foreach (Transform child in menu.transform)
            {
                if (child.name == "LagMenu_PanelMesh") continue;
                Object.Destroy(child.gameObject);
            }
            BuildMenu();
        }


       private static void BuildMenu()
        {
            ClampState();
            BuildBorder();
            BuildHeader();
            BuildDisconnect();
            BuildPageButtons();
            BuildModButtons();
            BuildHome();

        }














        private static void ClampState()
        {
            if (category < -1 || category >= Buttons.buttons.Length) category = 0;
            int total = GetTotalPages();
            if (page >= total) page = total - 1;
            if (page < 0) page = 0;
        }


        private static void BuildBorder()
        {

        }


        private static void BuildHeader()
        {
            string catName = category == -1 ? "Players"
                : (category >= 0 && category < categoryNames.Length)
                    ? categoryNames[category] : "LagMenu";
            int total = GetTotalPages();
            string txt = total > 1
                ? "LagMenu  " + catName + "  [" + (page + 1) + "/" + total + "]"
                : "LagMenu  " + catName;

            GameObject headerObj = MakeTextAtAnchor("title_lag_menu", txt, Color.white);
            if (headerObj != null) title = headerObj.GetComponent<TextMeshPro>();


            if (PanelAsset.TryGetPartBounds(menu.transform, "title_lag_menu", out Bounds titleBounds))
            {
                GameObject fpsObj = new GameObject("FPS");
                fpsObj.transform.SetParent(menu.transform, false);
                fpsObj.transform.localRotation = Quaternion.identity;
                fpsObj.transform.localPosition = titleBounds.center + new Vector3(0f, titleBounds.size.y * 0.9f, 0f);
                fpsObj.transform.localScale = Vector3.one;

                fpsText = fpsObj.AddComponent<TextMeshPro>();
                fpsText.font = activeFont;
                fpsText.text = "FPS: " + Mathf.CeilToInt(fps);

                const float CapPerEm = 0.7f;
                fpsText.fontSize = (titleBounds.size.y * 0.25f) / CapPerEm;
                fpsText.fontStyle = FontStyles.Bold;
                fpsText.alignment = TextAlignmentOptions.Center;
                fpsText.enableWordWrapping = false;
                fpsText.color = fps >= 60f ? Color.green :
                                fps >= 30f ? Color.yellow :
                                Color.red;

                Renderer fpsRend = fpsObj.GetComponent<Renderer>();
                if (fpsRend != null && fpsText.font != null)
                    fpsRend.material = fpsText.font.material;
            }
        }


        private static void BuildDisconnect()
        {
            if (!Settings.showDisconnect)
            {
                SetPartVisible("btn_disconnect_body", false);
                return;
            }
            SetPartVisible("btn_disconnect_body", true);

            MakeButtonAtAnchor("btn_disconnect_body", new ButtonInfo
            {
                buttonText = "Disconnect",
                isTogglable = false,
                method = () => PhotonNetwork.Disconnect()
            });
            MakeTextAtAnchor("label_disconnect", "Disconnect", Color.white);
        }


        private static void BuildPageButtons()
        {
            int total = GetTotalPages();
            bool hasPrev = page > 0;
            bool hasNext = page < total - 1;

            MakeButtonAtAnchor("rail_prev_body", new ButtonInfo
            {
                buttonText = "PrevPage",
                isTogglable = false,
                method = () => { if (page > 0) page--; }
            });
            MakeTextAtAnchor("rail_prev_arrow", "<", hasPrev ? Color.white : Color.grey);

            MakeButtonAtAnchor("rail_next_body", new ButtonInfo
            {
                buttonText = "NextPage",
                isTogglable = false,
                method = () => { if (page < GetTotalPages() - 1) page++; }
            });
            MakeTextAtAnchor("rail_next_arrow", ">", hasNext ? Color.white : Color.grey);
        }


        private static void TintPart(string partName, Color color)
        {
            Transform part = PanelAsset.FindPart(partName);
            if (part == null) return;
            Renderer r = part.GetComponent<Renderer>();
            if (r != null) r.material.color = color;
        }


        private static void SetPartVisible(string partName, bool visible)
        {
            Transform part = PanelAsset.FindPart(partName);
            if (part != null) part.gameObject.SetActive(visible);
        }


        private static Player selectedPlayer;
        private static bool showModCheck;

        public static void OpenPlayers()
        {
            category = -1;
            page = 0;
            selectedPlayer = null;
        }

        private static ButtonInfo[] GetPlayerCategoryButtons()
        {
            var list = new List<ButtonInfo>();

            list.Add(new ButtonInfo
            {
                buttonText = "Back",
                isTogglable = false,
                method = () =>
                {
                    if (selectedPlayer != null) { selectedPlayer = null; page = 0; showModCheck = false; }
                    else { category = 0; page = 0; }
                }
            });

            if (!PhotonNetwork.InRoom)
            {
                list.Add(new ButtonInfo { buttonText = "Not In A Room", isTogglable = false, method = () => { } });
                return list.ToArray();
            }

            if (selectedPlayer != null && !PhotonNetwork.PlayerList.Any(p => p.ActorNumber == selectedPlayer.ActorNumber))
                selectedPlayer = null;

            if (selectedPlayer == null)
            {
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    Player captured = p;
                    bool isLocal = p.IsLocal;
                    string label = (string.IsNullOrEmpty(p.NickName) ? "Player " + p.ActorNumber : p.NickName)
                                   + (isLocal ? " (You)" : "") + (p.IsMasterClient ? " [Master]" : "");

                    list.Add(new ButtonInfo
                    {
                        buttonText = label,
                        isTogglable = false,
                        method = () => { if (!isLocal) { selectedPlayer = captured; page = 0; showModCheck = false; } }
                    });
                }
            }
            else
            {
                Player sp = selectedPlayer;
                string nm = string.IsNullOrEmpty(sp.NickName) ? "Player " + sp.ActorNumber : sp.NickName;
                VRRig spRig = RigManager.GetRigFromPlayer(sp);

                list.Add(new ButtonInfo { buttonText = "» " + nm, isTogglable = false, method = () => { } });
                list.Add(new ButtonInfo
                {
                    buttonText = "Teleport To Player",
                    isTogglable = false,
                    method = () =>
                    {
                        VRRig rig = RigManager.GetRigFromPlayer(sp);
                        if (rig != null)
                            GTPlayer.Instance.TeleportTo(rig.transform.position, GTPlayer.Instance.transform.rotation);
                    }
                });
                list.Add(new ButtonInfo
                {
                    buttonText = string.IsNullOrEmpty(sp.UserId) ? "Unknown ID" : sp.UserId,
                    isTogglable = false,
                    method = () => { GUIUtility.systemCopyBuffer = sp.UserId; }
                });
                list.Add(new ButtonInfo
                {
                    buttonText = "Platform: " + GetPlatform(sp),
                    isTogglable = false,
                    method = () => { }
                });
                if (spRig != null)
                {
                    string hex = ColorUtility.ToHtmlStringRGB(spRig.playerColor);
                    list.Add(new ButtonInfo
                    {
                        buttonText = "Color: #" + hex,
                        isTogglable = false,
                        method = () =>
                        {
                            VRRig liveRig = RigManager.GetRigFromPlayer(sp);
                            if (liveRig != null)
                                RigManager.ChangeColor(liveRig.playerColor);
                        }
                    });
                }
                list.Add(new ButtonInfo
                {
                    buttonText = showModCheck ? "Hide Mod Check" : "Check Mods",
                    isTogglable = false,
                    method = () => { showModCheck = !showModCheck; }
                });

                if (showModCheck)
                {
                    List<ModInfo> detected = ModDatabase.GetDetectedMods(sp.CustomProperties);
                    if (detected.Count == 0)
                    {
                        list.Add(new ButtonInfo { buttonText = "No known mods detected", isTogglable = false, method = () => { } });
                    }
                    else
                    {
                        foreach (ModInfo mod in detected)
                        {
                            string prefix = mod.legal ? "[OK] " : "[!] ";
                            list.Add(new ButtonInfo { buttonText = prefix + mod.name, isTogglable = false, method = () => { } });
                        }
                    }
                }
            }

            return list.ToArray();
        }

        private static string GetPlatform(Player p)
        {
            if (!string.IsNullOrEmpty(p.UserId) && p.UserId.Length == 17 && p.UserId.All(char.IsDigit))
                return "Steam";

            return "Quest";
        }

        private static void BuildModButtons()
        {
            ButtonInfo[] all = category == -1 ? GetPlayerCategoryButtons() : Buttons.buttons[category];

            ButtonInfo[] visibleButtons = all
                .Where(x => x.shouldShow == null || x.shouldShow())
                .ToArray();

            if (visibleButtons.Length == 0) return;


            MakeButtonAtAnchor("row_back_face", visibleButtons[0]);
            TintPart("row_back_face", Lift(buttonOff, 0.05f));
            MakeTextAtAnchor("back_credit", "Back", Color.white);
            MakeTextAtAnchor("row_0_label", visibleButtons[0].buttonText, Color.white,
                alignment: TextAlignmentOptions.MidlineLeft);


            int contentCount = Mathf.Max(0, visibleButtons.Length - 1);
            int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)contentCount / BUTTONS_PER_PAGE));
            if (page >= totalPages) page = totalPages - 1;

            int start = 1 + page * BUTTONS_PER_PAGE;
            int count = Mathf.Min(BUTTONS_PER_PAGE, visibleButtons.Length - start);

            for (int i = 0; i < count; i++)
            {
                BuildModButton(visibleButtons[start + i], i + 1);
                SetPartVisible($"row_{i + 1:00}_face", true);
                SetPartVisible($"row_{i + 1:00}_outline", true);
            }


            for (int i = count; i < BUTTONS_PER_PAGE; i++)
            {
                SetPartVisible($"row_{i + 1}_chevron", false);
                SetPartVisible($"row_{i + 1:00}_face", false);
                SetPartVisible($"row_{i + 1:00}_outline", false);
            }
        }


        private static Color buttonOff => ThemeChanger.Current.ButtonOff;



        private static Color Lift(Color c, float amount) => new Color(
            Mathf.Clamp01(c.r + amount), Mathf.Clamp01(c.g + amount), Mathf.Clamp01(c.b + amount), c.a);

        private static void BuildModButton(ButtonInfo info, int rowNum)
        {
            MakeButtonAtAnchor($"row_{rowNum:00}_face", info);
            MakeTextAtAnchor($"row_{rowNum}_label", info.buttonText, Color.white,
                alignment: TextAlignmentOptions.MidlineLeft);

            bool active = info.isTogglable && info.enabled;
            TintPart($"row_{rowNum:00}_face", active ? accentCol : Lift(buttonOff, 0.05f));

            SetPartVisible($"row_{rowNum}_chevron", active);
        }


        private static void BuildHome()
        {
            if (!Settings.showHome)
            {
                SetPartVisible("btn_home_body", false);
                return;
            }
            SetPartVisible("btn_home_body", true);

            MakeButtonAtAnchor("btn_home_body", new ButtonInfo
            {
                buttonText = "Home",
                isTogglable = false,
                method = () => { category = 0; page = 0; }
            });
            MakeTextAtAnchor("label_home", "Home", Color.white);
        }

        private static GameObject MakeButtonAtAnchor(string faceAnchorName, ButtonInfo info)
        {
            if (!PanelAsset.TryGetPartBounds(menu.transform, faceAnchorName, out Bounds b))
            {
                Debug.LogError($"[LagMenu] No panel part named '{faceAnchorName}' found on the built model.");
                return null;
            }

            GameObject btn = new GameObject(faceAnchorName + "_Btn");
            btn.transform.SetParent(menu.transform, false);
            btn.transform.localPosition = b.center;
            btn.layer = LayerMask.NameToLayer("Ignore Raycast");

            BoxCollider col = btn.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(Mathf.Max(b.size.x, 0.01f), Mathf.Max(b.size.y, 0.01f), 0.02f);

            btn.AddComponent<ButtonPress>().info = info;
            return btn;
        }

        private static GameObject MakeTextAtAnchor(string anchorName, string text, Color? color = null,
            float fontSizeOverride = -1f, TextAlignmentOptions alignment = TextAlignmentOptions.Midline)
        {
            if (!PanelAsset.TryGetPartBounds(menu.transform, anchorName, out Bounds b))
            {
                Debug.LogError($"[LagMenu] No panel part named '{anchorName}' found on the built model.");
                return null;
            }

            GameObject obj = new GameObject(anchorName + "_Text");
            obj.transform.SetParent(menu.transform, false);

            obj.transform.localRotation = Settings.mirrorText ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;
            obj.transform.localScale = Vector3.one;


            bool isLeft = alignment == TextAlignmentOptions.MidlineLeft || alignment == TextAlignmentOptions.Left;
            float width = isLeft ? b.size.x * 1.12f : b.size.x;
            float xOffset = isLeft ? (width - b.size.x) * 0.5f : 0f;

            obj.transform.localPosition = b.center + new Vector3(xOffset, 0f, -0.0004f);

            TextMeshPro tm = obj.AddComponent<TextMeshPro>();
            tm.font = activeFont;
            tm.text = text;
            tm.rectTransform.sizeDelta = new Vector2(width, b.size.y * 2.5f);


            const float CapPerEm = 0.7f;
            const float BakedCapFraction = 0.62f;
            float targetCap = b.size.y * BakedCapFraction;
            float baseFontSize = targetCap / CapPerEm;
            tm.fontSize = (fontSizeOverride > 0f ? fontSizeOverride : baseFontSize) * Settings.textScale;

            tm.color = color ?? Color.white;
            tm.fontStyle = FontStyles.Bold;
            tm.alignment = alignment;
            tm.enableWordWrapping = false;
            tm.overflowMode = TextOverflowModes.Ellipsis;

            return obj;
        }

        public static int GetTotalPages()
        {
            int cnt;
            if (category == -1)
            {
                cnt = GetPlayerCategoryButtons().Length;
            }
            else if (category < 0 || category >= Buttons.buttons.Length)
            {
                return 1;
            }
            else
            {
                cnt = Buttons.buttons[category].Count(x => x.shouldShow == null || x.shouldShow());
            }


            int contentCount = Mathf.Max(0, cnt - 1);
            return Mathf.Max(1, Mathf.CeilToInt((float)contentCount / BUTTONS_PER_PAGE));
        }

        private const float MENU_FORWARD_OFFSET = 0.12f;

        private const float MENU_SMOOTH_SPEED = 14f;


        private const float TARGET_PANEL_WIDTH = 0.45f;

        public static void RecenterMenu()
        {
            Transform hand = GorillaTagger.Instance.leftHandTransform;
            Quaternion targetRot = hand.rotation * LeftHandBasis;
            Vector3 targetPos = hand.position + targetRot * ReadableFace * (MENU_FORWARD_OFFSET * Settings.menuDistance);

            float t = 1f - Mathf.Exp(-MENU_SMOOTH_SPEED * Time.deltaTime);
            menu.transform.position = Vector3.Lerp(menu.transform.position, targetPos, t);
            menu.transform.rotation = Quaternion.Slerp(menu.transform.rotation, targetRot, t);
        }

        public static void CreateReference()
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.SetParent(GorillaTagger.Instance.rightHandTransform);
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            SetColor(reference, accentCol);

            Object.Destroy(reference.GetComponent<SphereCollider>());
            SphereCollider sc = reference.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 1f;
            buttonCollider = sc;
        }

        public static void SetColor(GameObject obj, Color color)
        {
            Renderer r = obj.GetComponent<Renderer>();
            r.material.shader = Shader.Find("GorillaTag/UberShader");
            r.material.color = color;
        }





        public static bool A_Button;
        public static bool B_Button;
        public static bool RightTrigger;
        public static bool RightGrip;
        public static Vector2 RightJoystick;
        public static bool RightStickClick;
        public static float sensitivity = 0.5f;
        public static bool X_Button;
        public static bool Y_Button;
        public static bool LeftGrip;
        public static bool LeftTrigger;
        public static Vector2 LeftJoystick;
        public static bool LeftStickClick;
        public static UnityEngine.XR.InputDevice leftController;
        public static UnityEngine.XR.InputDevice rightController;
        private static bool CalculateGripState(float grabValue, float grabThreshold)
        {
            return grabValue >= grabThreshold;
        }
        public static void Controllerinputs()
        {
            var Poller = ControllerInputPoller.instance;
            A_Button = Poller.rightControllerPrimaryButton;
            B_Button = Poller.rightControllerSecondaryButton;
            RightTrigger = CalculateGripState(Poller.rightControllerIndexFloat, sensitivity);
            RightGrip = CalculateGripState(Poller.rightControllerGripFloat, sensitivity);
            RightJoystick = Poller.rightControllerPrimary2DAxis;
            RightStickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);



            X_Button = Poller.leftControllerPrimaryButton;
            Y_Button = Poller.leftControllerSecondaryButton;
            LeftTrigger = CalculateGripState(Poller.leftControllerIndexFloat, sensitivity);
            LeftGrip = CalculateGripState(Poller.leftControllerGripFloat, sensitivity);
            LeftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
            LeftStickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            return;


                rightController.TryGetFeatureValue(CommonUsages.primaryButton, out A_Button);
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out B_Button);
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out RightTrigger);
                rightController.TryGetFeatureValue(CommonUsages.gripButton, out RightGrip);
                rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out RightStickClick);
                rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out RightJoystick);

                leftController.TryGetFeatureValue(CommonUsages.primaryButton, out X_Button);
                leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out Y_Button);
                leftController.TryGetFeatureValue(CommonUsages.triggerButton, out LeftTrigger);
                leftController.TryGetFeatureValue(CommonUsages.gripButton, out LeftGrip);
                leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out LeftStickClick);
                leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out LeftJoystick);
        }










        public static IEnumerator AnimateButtonClick(GameObject btn, string buttonText)
        {
            Vector3 originalScale = btn.transform.localScale;
            Vector3 shrunkScale = originalScale * 0.97f;

            Renderer rend = btn.GetComponent<Renderer>();
            Color originalColor = rend != null ? rend.material.color : Color.white;

            Color flashColor = new Color(
                Mathf.Clamp01(originalColor.r + 0.15f),
                Mathf.Clamp01(originalColor.g + 0.15f),
                Mathf.Clamp01(originalColor.b + 0.15f),
                1f
            );

            Transform textTransform = null;
            if (menu != null)
            {
                Transform found = menu.transform.Find(buttonText + "_Text");
                if (found != null) textTransform = found;
            }

            Vector3 textOriginalScale = textTransform != null ? textTransform.localScale : Vector3.one;
            Vector3 textShrunkScale = textOriginalScale * 0.8f;

            float shrinkDuration = 0.05f;
            float growDuration = 0.08f;
            float time = 0f;

            while (time < shrinkDuration)
            {
                if (btn == null) yield break;

                float t = time / shrinkDuration;
                float easeT = Mathf.SmoothStep(0f, 1f, t);

                btn.transform.localScale = Vector3.Lerp(originalScale, shrunkScale, easeT);
                if (textTransform != null)
                    textTransform.localScale = Vector3.Lerp(textOriginalScale, textShrunkScale, easeT);
                if (rend != null)
                    rend.material.color = Color.Lerp(originalColor, flashColor, easeT);

                time += Time.deltaTime;
                yield return null;
            }

            if (btn != null)
            {
                btn.transform.localScale = shrunkScale;
                if (rend != null) rend.material.color = flashColor;
            }
            if (textTransform != null)
                textTransform.localScale = textShrunkScale;

            time = 0f;
            while (time < growDuration)
            {
                if (btn == null) yield break;

                float t = time / growDuration;
                float easeT = Mathf.SmoothStep(0f, 1f, t);

                btn.transform.localScale = Vector3.Lerp(shrunkScale, originalScale, easeT);
                if (textTransform != null)
                    textTransform.localScale = Vector3.Lerp(textShrunkScale, textOriginalScale, easeT);
                if (rend != null)
                    rend.material.color = Color.Lerp(flashColor, originalColor, easeT);

                time += Time.deltaTime;
                yield return null;
            }

            if (btn != null)
            {
                btn.transform.localScale = originalScale;
                if (rend != null) rend.material.color = originalColor;
            }
            if (textTransform != null)
                textTransform.localScale = textOriginalScale;


            Main.RecreateMenu();
        }















        private static bool fontSwapPending = true;




        public static readonly int TransparentFX = LayerMask.NameToLayer(nameof(TransparentFX));
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Zone = LayerMask.NameToLayer(nameof(Zone));
        public static readonly int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static readonly int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static readonly int GorillaCosmetics = LayerMask.NameToLayer(nameof(GorillaCosmetics));
        public static readonly int GorillaParticle = LayerMask.NameToLayer(nameof(GorillaParticle));




        public static int NoInvisLayerMask() =>
         ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone | 1 << GorillaTrigger | 1 << GorillaBoundary |
           1 << GorillaCosmetics | 1 << GorillaParticle);



        public T GetObject<T>()
        {
            string json = GetMessage();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string GetMessage()
        {
            return "yoyoyo";
        }

        public static readonly Dictionary<Type, object[]> typePool = new Dictionary<Type, object[]>();
        private static readonly Dictionary<Type, float> receiveTypeDelay = new Dictionary<Type, float>();

        public static T[] GetAllType<T>(float decayTime = 5f) where T : Object
        {
            Type type = typeof(T);

            float lastReceivedTime = receiveTypeDelay.GetValueOrDefault(type, -1f);

            if (Time.time > lastReceivedTime)
            {
                typePool.Remove(type);
                receiveTypeDelay[type] = Time.time + decayTime;
            }

            if (!typePool.ContainsKey(type))
                typePool.Add(type, Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None));

            return (T[])typePool[type];
        }

        private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();
        public static bool ButtonClickAnimations = true;
        public static ButtonInfo animatingButton;
        private static MonoBehaviour CoroutineRunner;
        public static GameObject GetObject(string find)
        {
            if (objectPool.TryGetValue(find, out GameObject go))
                return go;

            GameObject tgo = GameObject.Find(find);
            if (!tgo && find.Contains("/"))
            {
                var split = find.Split('/');
                var rootName = split[0];
                var root = GameObject.Find(rootName);

                if (root != null)
                {
                    var path = find[(rootName.Length + 1)..];
                    var tr = root.transform.Find(path);
                    if (tr != null)
                        tgo = tr.gameObject;
                }
            }

            if (tgo != null)
                objectPool.Add(find, tgo);

            return tgo;
        }

        public static bool useShrinkClose = false;









        public static void InitializeFonts()
        {
            Fonts = new TMP_FontAsset[]
            {
        AgencyFB,
        FreeSans,
        Candara,
        ComicSans,
        CascadiaMono,
        Anton,
        Minecraft,
        MSGothic,
        OpenDyslexic,
        SimSun,
        Taiko,
        Terminal,
        Utopium,
        DejaVuSans
            };

            activeFont = Fonts[0];
        }

        public static void IncreaseFont()
        {
            fontCycle++;

            if (fontCycle >= Fonts.Length)
                fontCycle = 0;

            activeFont = Fonts[fontCycle];
            RecreateMenu();
        }

        public static void DecreaseFont()
        {
            fontCycle--;

            if (fontCycle < 0)
                fontCycle = Fonts.Length - 1;

            activeFont = Fonts[fontCycle];
            RecreateMenu();
        }


        public static int fontCycle;


        private static Font customFont;
        private static bool fontGaveUp = false;
        private static bool fontRegistered = false;

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, out uint pcFonts);

        private static Font GetFont()
        {
            if (customFont != null || fontGaveUp)
                return customFont;

            if (!Utilities.ResourceManager.FontReady)
                return null;

            try
            {
                if (!fontRegistered)
                {
                    fontRegistered = true;

                    byte[] fontData = LoadEmbeddedFontBytes("JetBrainsMonoNL-Bold");
                    if (fontData == null || fontData.Length == 0)
                    {
                        Debug.LogWarning("LagMenu: Embedded resource 'JetBrainsMonoNL-Bold' not found in assembly.");
                    }
                    else
                    {
                        GCHandle pinned = GCHandle.Alloc(fontData, GCHandleType.Pinned);
                        try
                        {
                            bool added = AddFontMemResourceEx(pinned.AddrOfPinnedObject(), (uint)fontData.Length, IntPtr.Zero, out uint fontCount) != IntPtr.Zero;
                            Debug.Log($"LagMenu: AddFontMemResourceEx success={added}, fontCount={fontCount}, bytes={fontData.Length}");
                        }
                        finally
                        {
                            pinned.Free();
                        }
                    }
                }

                string[] candidateNames = { "JetBrains Mono NL Bold", "JetBrains Mono NL" };

                customFont = Font.CreateDynamicFontFromOSFont(candidateNames, 80);
                if (customFont != null)
                {
                    Debug.Log(customFont.name);
                    Debug.Log(customFont.material);
                }

                Debug.Log($"LagMenu: CreateDynamicFontFromOSFont result: {(customFont != null ? customFont.name : "null")}");
            }

            catch (Exception e)
            {
                Debug.LogWarning("LagMenu: Failed to load embedded font JetBrainsMonoNL-Bold: " + e);
            }

            if (customFont == null)
            {
                fontGaveUp = true;
                Debug.LogWarning("LagMenu: JetBrainsMonoNL-Bold not available, falling back to default font");
            }

            return customFont;
        }

        private static byte[] LoadEmbeddedFontBytes(string nameFragment)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.IndexOf(nameFragment, StringComparison.OrdinalIgnoreCase) >= 0);

            if (resourceName == null)
                return null;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;
                byte[] buffer = new byte[stream.Length];
                int total = 0;
                while (total < buffer.Length)
                {
                    int read = stream.Read(buffer, total, buffer.Length - total);
                    if (read <= 0) break;
                    total += read;
                }
                return buffer;
            }
        }







        public static TMP_FontAsset activeFont;
        public static TMP_FontAsset[] Fonts;


        public static TMP_FontAsset FreeSans;
        public static TMP_FontAsset Candara;
        public static TMP_FontAsset CascadiaMono;
        public static TMP_FontAsset Anton;
        public static TMP_FontAsset Minecraft;
        public static TMP_FontAsset MSGothic;
        public static TMP_FontAsset OpenDyslexic;
        public static TMP_FontAsset SimSun;
        public static TMP_FontAsset Taiko;
        public static TMP_FontAsset Terminal;
        public static TMP_FontAsset Utopium;
        public static TMP_FontAsset DejaVuSans;
        public static TMP_FontAsset AgencyFB;
        public static TMP_FontAsset ComicSans;
        public static TMP_FontAsset JetBrainsMono;

        public static void ApplyFontType(int index)
        {
            fontCycle = index;
            switch (index)
            {
                case 0:
                    activeFont = FreeSans;
                    break;
                case 1:
                    activeFont = Candara;
                    break;
                case 2:
                    activeFont = CascadiaMono;
                    break;
                case 3:
                    activeFont = Anton;
                    break;
                case 4:
                    activeFont = Minecraft;
                    break;
                case 5:
                    activeFont = MSGothic;
                    break;
                case 6:
                    activeFont = OpenDyslexic;
                    break;
                case 7:
                    activeFont = SimSun;
                    break;
                case 8:
                    activeFont = Taiko;
                    break;
                case 9:
                    activeFont = Terminal;
                    break;
                case 10:
                    activeFont = Utopium;
                    break;
                case 11:
                    activeFont = DejaVuSans;
                    break;
                case 12:
                    activeFont = AgencyFB;
                    break;
                case 13:
                    activeFont = ComicSans;
                    break;
                case 14:
                    activeFont = JetBrainsMono;
                    break;
                default:
                    activeFont = AgencyFB;
                    break;
            }
            RecreateMenu();
        }
        public static string NoRichtextTags(string input, string replace = "")
        {
            Regex notags = new Regex("<.*?>", RegexOptions.IgnoreCase);
            return notags.Replace(input, replace);
        }
    }
}