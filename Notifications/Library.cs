using BepInEx;
using LagMenu.Mods;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace LagMenu.Notifications
{
    [BepInPlugin("org.gorillatag.lars.notifications2", "NotificationLibrary", "2.0.0")]
    public class NotifiLib : BaseUnityPlugin, IInRoomCallbacks, IMatchmakingCallbacks
    {
        private static NotifiLib instance;
        private GameObject HUDParent;
        private GameObject HUDCanvas;
        private Camera mainCamera;
        private bool hasInit;

        private static Texture2D bgTexture;
        private static Texture2D cardBorderTexture;
        private static Texture2D progressBarBg;
        private static Texture2D progressBarFill;
        private static Texture2D glowTexture;
        private static Texture2D iconBadgeTexture;
        private static Texture2D solidTexture;

        private const int BG_W = 380;
        private const int BG_H = 90;
        private const int BG_R = 16;
        private const int PF_W = 344;
        private const int PF_H = 6;
        private const int BADGE_SIZE = 32;

        private static readonly Color textWhite = new Color(0.98f, 0.98f, 0.98f, 1f);
        private static readonly Color textGrey = new Color(0.82f, 0.82f, 0.85f, 1f);
        private static readonly List<NotificationUI> activeNotifications = new List<NotificationUI>();
        private static readonly List<ScreenNotification> screenNotifications = new List<ScreenNotification>();
        private const int MAX_NOTIFICATIONS = 2;
        private const int MAX_SCREEN_NOTIFICATIONS = 5;
        private const float NOTIFICATION_WIDTH = 380f;
        private const float NOTIFICATION_HEIGHT = 90f;
        private const float NOTIFICATION_SPACING = 100f;
        public static bool IsEnabled = true;
        public static bool disableNotifications = false;
        private static GUIStyle titleStyle;
        private static GUIStyle messageStyle;
        private static GUIStyle iconStyle;

        private static Color GetAccent(NotifiReason reason)
        {
            switch (reason)
            {
                case NotifiReason.Error: return new Color(1f, 0.28f, 0.35f);
                case NotifiReason.Success: return new Color(0.25f, 1f, 0.55f);
                case NotifiReason.Warning: return new Color(1f, 0.72f, 0.15f);
                default: return ThemeChanger.CurrentAccent;
            }
        }

        private static readonly Dictionary<NotifiReason, string> Icons = new Dictionary<NotifiReason, string>
        {
            { NotifiReason.Info, "i" },
            { NotifiReason.Success, "\u2713" },
            { NotifiReason.Error, "\u2715" },
            { NotifiReason.Warning, "!" },
            { NotifiReason.RoomJoined, "\u2192" },
            { NotifiReason.RoomLeft, "\u2190" },
            { NotifiReason.Button, "\u25CF" },
            { NotifiReason.MasterClientChange, "\u2605" },
        };

        private void Awake()
        {
            instance = this;
            Logger.LogInfo("Plugin NotificationLibrary is loaded!");
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer == null) return;
            SendNotification(newPlayer.NickName, "Joined the room", 3f, NotifiReason.RoomJoined);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer == null) return;
            SendNotification(otherPlayer.NickName, "Left the room", 3f, NotifiReason.RoomLeft);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            if (newMasterClient == null || !newMasterClient.IsLocal) return;
            if (PhotonNetwork.IsMasterClient)
            {
                SendNotification("You are now master client", NotifiReason.MasterClientChange);
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

        public void OnJoinedRoom()
        {
            string code = PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "";
            SendNotification($"Joined Room ({code})", NotifiReason.RoomJoined);
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public void OnCreatedRoom() { }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnJoinRandomFailed(short returnCode, string message) { }
        public void OnLeftRoom() { }
        public void OnPreLeavingRoom() { }

        private void Init()
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
            InitializeTextures();
            if (XRSettings.isDeviceActive) CreateVRHUD();
            hasInit = true;
        }

        private void InitializeTextures()
        {
            bgTexture = CreateRoundedRect(BG_W, BG_H, new Color(0.07f, 0.07f, 0.085f, 0.96f), BG_R);
            cardBorderTexture = CreateRoundedRectRing(BG_W, BG_H, BG_R, 1.5f);
            progressBarBg = CreateRoundedRect(PF_W, PF_H, new Color(0.16f, 0.16f, 0.18f, 1f), 3);
            progressBarFill = CreateRoundedRect(PF_W, PF_H, Color.white, 3);
            glowTexture = CreateGlow(BG_W + 24, BG_H + 24);
            iconBadgeTexture = CreateRoundedRect(BADGE_SIZE, BADGE_SIZE, Color.white, BADGE_SIZE / 2);
            solidTexture = CreateSolid(8, 8, Color.white);
        }

        private void CreateVRHUD()
        {
            HUDParent = new GameObject("NOTIFICATIONLIB_VR_PARENT");
            HUDParent.transform.SetParent(mainCamera.transform, false);
            HUDParent.transform.localPosition = new Vector3(-0.15f, 0f, 0.8f);
            HUDParent.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);

            HUDCanvas = new GameObject("NOTIFICATIONLIB_CANVAS");
            HUDCanvas.transform.SetParent(HUDParent.transform, false);
            Canvas canvas = HUDCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;
            CanvasScaler scaler = HUDCanvas.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;
            HUDCanvas.AddComponent<GraphicRaycaster>();
            RectTransform rect = HUDCanvas.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500f, 500f);
            rect.localScale = Vector3.one * 0.001f;
            rect.localPosition = Vector3.zero;
        }

        private void Update()
        {
            if (!hasInit && Camera.main != null && GorillaTagger.Instance != null) Init();
            if (!hasInit) return;

            if (XRSettings.isDeviceActive)
            {
                for (int i = activeNotifications.Count - 1; i >= 0; i--)
                {
                    if (activeNotifications[i] != null) activeNotifications[i].Update();
                    else activeNotifications.RemoveAt(i);
                }
            }

            for (int i = screenNotifications.Count - 1; i >= 0; i--)
            {
                screenNotifications[i].Update();
                if (screenNotifications[i].State == NotificationState.Completed)
                {
                    screenNotifications.RemoveAt(i);
                    UpdateScreenNotificationPositions();
                }
            }
        }

        private void OnGUI()
        {
            if (XRSettings.isDeviceActive) return;
            if (screenNotifications.Count == 0) return;
            InitializeStyles();
            float x = 15f;
            float baseY = Screen.height - 15f - NOTIFICATION_HEIGHT;
            for (int i = screenNotifications.Count - 1; i >= 0; i--)
            {
                ScreenNotification notif = screenNotifications[i];
                int displayIndex = screenNotifications.Count - 1 - i;
                notif.TargetY = baseY - (displayIndex * (NOTIFICATION_HEIGHT + 10f));
                notif.CurrentY = Mathf.Lerp(notif.CurrentY, notif.TargetY, Time.deltaTime * 8f);
                DrawScreenNotification(notif, x + notif.OffsetX, notif.CurrentY);
            }
        }

        private void DrawScreenNotification(ScreenNotification notif, float x, float y)
        {
            float scale = notif.Scale;
            if (scale <= 0.01f) return;

            Color accent = GetAccent(notif.Reason);

            float scaledWidth = NOTIFICATION_WIDTH * scale;
            float scaledHeight = NOTIFICATION_HEIGHT * scale;
            float offsetX = (NOTIFICATION_WIDTH - scaledWidth) * 0.5f;
            float offsetY = (NOTIFICATION_HEIGHT - scaledHeight) * 0.5f;
            Rect rect = new Rect(x + offsetX, y + offsetY, scaledWidth, scaledHeight);

            Color orig = GUI.color;

            if (notif.Alpha > 0.01f)
            {
                GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha * 0.55f);
                GUI.DrawTexture(new Rect(x + offsetX - 12f * scale, y + offsetY - 12f * scale,
                    (NOTIFICATION_WIDTH + 24f) * scale, (NOTIFICATION_HEIGHT + 24f) * scale), glowTexture);
            }

            GUI.color = new Color(1f, 1f, 1f, notif.Alpha);
            GUI.DrawTexture(rect, bgTexture);

            GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha * 0.9f);
            GUI.DrawTexture(rect, cardBorderTexture);

            GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha);
            GUI.DrawTexture(new Rect(x + offsetX + 6f * scale, y + offsetY + 10f * scale, 3.5f * scale, (NOTIFICATION_HEIGHT - 20f) * scale), solidTexture);

            float badgeX = x + offsetX + 16f * scale;
            float badgeY = y + offsetY + 16f * scale;
            float badgeSize = BADGE_SIZE * scale;
            GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha * 0.22f);
            GUI.DrawTexture(new Rect(badgeX, badgeY, badgeSize, badgeSize), iconBadgeTexture);
            GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha);
            iconStyle.fontSize = Mathf.RoundToInt(16 * scale);
            GUI.Label(new Rect(badgeX, badgeY, badgeSize, badgeSize), Icons.TryGetValue(notif.Reason, out var icon) ? icon : "i", iconStyle);

            GUI.color = new Color(textWhite.r, textWhite.g, textWhite.b, notif.Alpha);
            GUI.Label(new Rect(x + offsetX + 58f * scale, y + offsetY + 15f * scale, 302f * scale, 25f * scale), notif.Title, titleStyle);

            GUI.color = new Color(textGrey.r, textGrey.g, textGrey.b, notif.Alpha);
            GUI.Label(new Rect(x + offsetX + 58f * scale, y + offsetY + 38f * scale, 302f * scale, 30f * scale), notif.Message, messageStyle);

            GUI.color = new Color(1f, 1f, 1f, notif.Alpha * 0.5f);
            GUI.DrawTexture(new Rect(x + offsetX + 18f * scale, y + offsetY + (NOTIFICATION_HEIGHT - 14f) * scale, PF_W * scale, PF_H * scale), progressBarBg);

            float progress = Mathf.Clamp01(1f - ((Time.time - notif.StartTime) / notif.Duration));
            float fillWidth = PF_W * progress * scale;
            if (fillWidth > 0f)
            {
                GUI.color = new Color(accent.r, accent.g, accent.b, notif.Alpha);
                GUI.DrawTexture(new Rect(x + offsetX + 18f * scale, y + offsetY + (NOTIFICATION_HEIGHT - 14f) * scale, fillWidth, PF_H * scale), progressBarFill);
            }

            GUI.color = orig;
        }

        private void InitializeStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = textWhite },
                    alignment = TextAnchor.UpperLeft,
                    wordWrap = false,
                    richText = true
                };
            }
            if (messageStyle == null)
            {
                messageStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    normal = { textColor = textGrey },
                    alignment = TextAnchor.UpperLeft,
                    wordWrap = true,
                    richText = true
                };
            }
            if (iconStyle == null)
            {
                iconStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = false,
                    richText = false
                };
            }
        }

        public enum NotifiReason
        {
            Info, Button, RoomJoined, RoomLeft, MasterClientChange, Error, Success, Warning
        }

        public static void SendNotification(string message, NotifiReason reason = NotifiReason.Info)
        {
            SendNotification(GetReasonTitle(reason), message, 3f, reason);
        }

        public static void SendNotification(string title, string message, float duration = 3f, NotifiReason reason = NotifiReason.Info)
        {
            if (disableNotifications || !IsEnabled || instance == null || !instance.hasInit) return;
            try
            {
                if (XRSettings.isDeviceActive)
                {
                    activeNotifications.RemoveAll(n => n == null || n.notifObject == null);
                    while (activeNotifications.Count >= MAX_NOTIFICATIONS)
                    {
                        var oldest = activeNotifications[0];
                        if (oldest != null && oldest.notifObject != null)
                            instance.StartCoroutine(oldest.FadeOut(true));
                        else
                            activeNotifications.RemoveAt(0);
                        activeNotifications.RemoveAt(0);
                    }
                    var notif = new NotificationUI(title, message, duration, reason);
                    activeNotifications.Add(notif);
                    instance.StartCoroutine(notif.Show());
                    instance.StartCoroutine(notif.LifeCycle());
                    UpdateNotificationPositions();
                }

                if (!XRSettings.isDeviceActive)
                {
                    while (screenNotifications.Count >= MAX_SCREEN_NOTIFICATIONS)
                        screenNotifications.RemoveAt(0);
                    screenNotifications.Add(new ScreenNotification
                    {
                        Title = title,
                        Message = message,
                        Duration = duration,
                        Reason = reason,
                        StartTime = Time.time,
                        State = NotificationState.FadingIn,
                        Alpha = 0f,
                        Scale = 0f,
                        OffsetX = 0f,
                        TargetY = 0f,
                        CurrentY = Screen.height - 15f - NOTIFICATION_HEIGHT
                    });
                    UpdateScreenNotificationPositions();
                }

                ResourceManager.PlayNotificationSound();
            }
            catch (Exception) { }
        }

        private static void UpdateNotificationPositions()
        {
            for (int i = 0; i < activeNotifications.Count; i++)
            {
                if (activeNotifications[i] != null)
                    activeNotifications[i].targetY = (activeNotifications.Count - 1 - i) * NOTIFICATION_SPACING;
            }
        }

        private static void UpdateScreenNotificationPositions()
        {
            float baseY = Screen.height - 15f - NOTIFICATION_HEIGHT;
            for (int i = screenNotifications.Count - 1; i >= 0; i--)
                screenNotifications[i].TargetY = baseY - ((screenNotifications.Count - 1 - i) * (NOTIFICATION_HEIGHT + 10f));
        }

        private static string GetReasonTitle(NotifiReason reason)
        {
            return reason switch
            {
                NotifiReason.Error => "<color=#ff4444>ERROR</color>",
                NotifiReason.Success => "<color=#44ff44>SUCCESS</color>",
                NotifiReason.Warning => "<color=#ffaa44>WARNING</color>",
                NotifiReason.RoomJoined => "<color=#44aaff>JOINED</color>",
                NotifiReason.RoomLeft => "<color=#ff8844>LEFT</color>",
                NotifiReason.Button => "<color=#b87aff>ACTION</color>",
                NotifiReason.MasterClientChange => "<color=#ff66c8>MASTER CLIENT CHANGED</color>",
                _ => "<color=#ffffff>INFO</color>",
            };
        }

        public static void ClearAllNotifications()
        {
            if (instance == null) return;
            foreach (var n in activeNotifications)
                if (n != null && n.notifObject != null) n.ForceDestroy();
            activeNotifications.Clear();
            screenNotifications.Clear();
        }

        public static void ClearPastNotifications(int amount)
        {
            if (instance == null) return;
            amount = Math.Min(amount, activeNotifications.Count);
            for (int i = 0; i < amount; i++)
                if (activeNotifications[i] != null && activeNotifications[i].notifObject != null)
                    activeNotifications[i].ForceDestroy();
            activeNotifications.RemoveRange(0, amount);
            UpdateNotificationPositions();
            screenNotifications.RemoveRange(0, Math.Min(amount, screenNotifications.Count));
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            ClearAllNotifications();
            if (bgTexture != null) Destroy(bgTexture);
            if (cardBorderTexture != null) Destroy(cardBorderTexture);
            if (progressBarBg != null) Destroy(progressBarBg);
            if (progressBarFill != null) Destroy(progressBarFill);
            if (glowTexture != null) Destroy(glowTexture);
            if (iconBadgeTexture != null) Destroy(iconBadgeTexture);
            if (solidTexture != null) Destroy(solidTexture);
        }

        private static float RoundedRectSDF(float px, float py, float w, float h, float r)
        {
            float cx = w * 0.5f, cy = h * 0.5f;
            float qx = Mathf.Abs(px - cx) - (cx - r);
            float qy = Mathf.Abs(py - cy) - (cy - r);
            float outside = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f) + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
            float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
            return outside + inside - r;
        }

        private static Color[] BuildSolidRoundedRectPixels(int width, int height, Color color, int radius)
        {
            var pixels = new Color[width * height];
            float r = radius;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dist = RoundedRectSDF(x + 0.5f, y + 0.5f, width, height, r);
                    float alpha = Mathf.Clamp01(-(dist - 0.5f) / 1.5f);
                    if (alpha <= 0f)
                        pixels[x + y * width] = Color.clear;
                    else
                    {
                        var c = color;
                        c.a *= alpha;
                        pixels[x + y * width] = c;
                    }
                }
            }
            return pixels;
        }

        private static Color[] BuildRoundedRectRingPixels(int width, int height, int radius, float thickness)
        {
            var pixels = new Color[width * height];
            float r = radius;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dist = RoundedRectSDF(x + 0.5f, y + 0.5f, width, height, r);
                    float ring = 1f - Mathf.Abs(dist + thickness * 0.5f) / (thickness * 0.5f + 0.75f);
                    float alpha = Mathf.Clamp01(ring);
                    pixels[x + y * width] = new Color(1f, 1f, 1f, alpha);
                }
            }
            return pixels;
        }

        private static Texture2D CreateRoundedRect(int width, int height, Color color, int radius)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(BuildSolidRoundedRectPixels(width, height, color, radius));
            tex.Apply(false);
            return tex;
        }

        private static Texture2D CreateRoundedRectRing(int width, int height, int radius, float thickness)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(BuildRoundedRectRingPixels(width, height, radius, thickness));
            tex.Apply(false);
            return tex;
        }

        private static Texture2D CreateSolid(int width, int height, Color color)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply(false);
            return tex;
        }

        private static Texture2D CreateGlow(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
            float rx = center.x, ry = center.y;
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x) / rx;
                    float dy = Mathf.Abs(y - center.y) / ry;
                    float d = Mathf.Sqrt(dx * dx * dx * dx + dy * dy * dy * dy);
                    float alpha = 1f - Mathf.Clamp01(d);
                    pixels[x + y * width] = new Color(1f, 1f, 1f, alpha);
                }
            }
            tex.SetPixels(pixels);
            tex.Apply(false);
            return tex;
        }

        private enum NotificationState { FadingIn, Displaying, FadingOut, Completed }

        private class ScreenNotification
        {
            public string Title, Message;
            public float Duration, StartTime, Alpha, Scale, OffsetX, TargetY, CurrentY;
            public NotifiReason Reason;
            public NotificationState State;

            public void Update()
            {
                float elapsed = Time.time - StartTime;
                switch (State)
                {
                    case NotificationState.FadingIn:
                        Alpha = Mathf.Lerp(Alpha, 1f, Time.deltaTime * 10f);
                        Scale = Mathf.Lerp(Scale, 1f, Time.deltaTime * 12f);
                        if (Alpha > 0.98f && Scale > 0.98f) { State = NotificationState.Displaying; Alpha = 1f; Scale = 1f; }
                        break;
                    case NotificationState.Displaying:
                        if (elapsed >= Duration - 0.5f) State = NotificationState.FadingOut;
                        break;
                    case NotificationState.FadingOut:
                        Alpha = Mathf.Lerp(Alpha, 0f, Time.deltaTime * 8f);
                        Scale = Mathf.Lerp(Scale, 0f, Time.deltaTime * 10f);
                        if (Alpha < 0.02f && Scale < 0.02f) State = NotificationState.Completed;
                        break;
                }
            }
        }

        private class NotificationUI
        {
            public GameObject notifObject;
            public string title, message;
            public float duration, startTime, targetY, currentAlpha;
            public NotifiReason reason;
            private Image background, border, glow, stripe, badge, progressBg, progressFill;
            private TextMeshProUGUI titleText, messageText, iconText;
            private RectTransform rectTransform;
            private Color accent;
            private enum State { FadingIn, Displaying, FadingOut, Completed }
            private State currentState = State.FadingIn;
            private float lastUpdateTime;
            private const float UPDATE_INTERVAL = 0.016f;

            public NotificationUI(string title, string message, float duration, NotifiReason reason)
            {
                this.title = title; this.message = message;
                this.duration = duration; this.reason = reason;
                this.accent = GetAccent(reason);
                this.startTime = Time.time; this.lastUpdateTime = Time.time;
                CreateUI();
            }

            private void CreateUI()
            {
                notifObject = new GameObject("Notification");
                notifObject.transform.SetParent(instance.HUDCanvas.transform, false);
                rectTransform = notifObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(NOTIFICATION_WIDTH, NOTIFICATION_HEIGHT);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.zero;

                var glowObj = new GameObject("Glow");
                glowObj.transform.SetParent(notifObject.transform, false);
                glow = glowObj.AddComponent<Image>();
                glow.sprite = Sprite.Create(glowTexture, new Rect(0, 0, glowTexture.width, glowTexture.height), new Vector2(0.5f, 0.5f));
                glow.raycastTarget = false;
                var glowRect = glowObj.GetComponent<RectTransform>();
                glowRect.sizeDelta = new Vector2(NOTIFICATION_WIDTH + 24, NOTIFICATION_HEIGHT + 24);
                glowRect.anchoredPosition = Vector2.zero;
                glow.color = new Color(accent.r, accent.g, accent.b, 0f);

                var bgObj = new GameObject("Background");
                bgObj.transform.SetParent(notifObject.transform, false);
                background = bgObj.AddComponent<Image>();
                background.sprite = Sprite.Create(bgTexture, new Rect(0, 0, bgTexture.width, bgTexture.height), new Vector2(0.5f, 0.5f));
                background.raycastTarget = false;
                var bgRect = bgObj.GetComponent<RectTransform>();
                bgRect.sizeDelta = new Vector2(NOTIFICATION_WIDTH, NOTIFICATION_HEIGHT);
                bgRect.anchoredPosition = Vector2.zero;

                var borderObj = new GameObject("Border");
                borderObj.transform.SetParent(notifObject.transform, false);
                border = borderObj.AddComponent<Image>();
                border.sprite = Sprite.Create(cardBorderTexture, new Rect(0, 0, cardBorderTexture.width, cardBorderTexture.height), new Vector2(0.5f, 0.5f));
                border.raycastTarget = false;
                var borderRect = borderObj.GetComponent<RectTransform>();
                borderRect.sizeDelta = new Vector2(NOTIFICATION_WIDTH, NOTIFICATION_HEIGHT);
                borderRect.anchoredPosition = Vector2.zero;
                border.color = new Color(accent.r, accent.g, accent.b, 0f);

                var stripeObj = new GameObject("AccentStripe");
                stripeObj.transform.SetParent(notifObject.transform, false);
                stripe = stripeObj.AddComponent<Image>();
                stripe.sprite = Sprite.Create(solidTexture, new Rect(0, 0, solidTexture.width, solidTexture.height), new Vector2(0.5f, 0.5f));
                stripe.raycastTarget = false;
                var stripeRect = stripeObj.GetComponent<RectTransform>();
                stripeRect.sizeDelta = new Vector2(3.5f, NOTIFICATION_HEIGHT - 20f);
                stripeRect.anchoredPosition = new Vector2(-NOTIFICATION_WIDTH / 2f + 8f, 0f);
                stripe.color = new Color(accent.r, accent.g, accent.b, 0f);

                var badgeObj = new GameObject("IconBadge");
                badgeObj.transform.SetParent(notifObject.transform, false);
                badge = badgeObj.AddComponent<Image>();
                badge.sprite = Sprite.Create(iconBadgeTexture, new Rect(0, 0, iconBadgeTexture.width, iconBadgeTexture.height), new Vector2(0.5f, 0.5f));
                badge.raycastTarget = false;
                var badgeRect = badgeObj.GetComponent<RectTransform>();
                badgeRect.sizeDelta = new Vector2(BADGE_SIZE, BADGE_SIZE);
                badgeRect.anchoredPosition = new Vector2(-NOTIFICATION_WIDTH / 2f + 32f, 12f);
                badge.color = new Color(accent.r, accent.g, accent.b, 0f);

                var iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(notifObject.transform, false);
                var ic = iconObj.AddComponent<Canvas>(); ic.overrideSorting = true; ic.sortingOrder = 11;
                iconText = iconObj.AddComponent<TextMeshProUGUI>();
                iconText.text = Icons.TryGetValue(reason, out var glyph) ? glyph : "i";
                iconText.fontSize = 18; iconText.fontStyle = FontStyles.Bold;
                iconText.color = accent; iconText.alignment = TextAlignmentOptions.Center;
                iconText.raycastTarget = false;
                var iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.sizeDelta = new Vector2(BADGE_SIZE, BADGE_SIZE);
                iconRect.anchoredPosition = new Vector2(-NOTIFICATION_WIDTH / 2f + 32f, 12f);

                var progressBgObj = new GameObject("ProgressBg");
                progressBgObj.transform.SetParent(notifObject.transform, false);
                progressBg = progressBgObj.AddComponent<Image>();
                progressBg.sprite = Sprite.Create(progressBarBg, new Rect(0, 0, progressBarBg.width, progressBarBg.height), new Vector2(0.5f, 0.5f));
                progressBg.raycastTarget = false;
                var progressBgRect = progressBgObj.GetComponent<RectTransform>();
                progressBgRect.sizeDelta = new Vector2(PF_W, PF_H);
                progressBgRect.anchoredPosition = new Vector2(2, -40);

                var progressFillObj = new GameObject("ProgressFill");
                progressFillObj.transform.SetParent(notifObject.transform, false);
                progressFill = progressFillObj.AddComponent<Image>();
                progressFill.sprite = Sprite.Create(progressBarFill, new Rect(0, 0, progressBarFill.width, progressBarFill.height), new Vector2(0f, 0.5f));
                progressFill.type = Image.Type.Filled;
                progressFill.fillMethod = Image.FillMethod.Horizontal;
                progressFill.fillAmount = 1f;
                progressFill.raycastTarget = false;
                progressFill.color = accent;
                var progressFillRect = progressFillObj.GetComponent<RectTransform>();
                progressFillRect.sizeDelta = new Vector2(PF_W, PF_H);
                progressFillRect.pivot = new Vector2(0f, 0.5f);
                progressFillRect.anchoredPosition = new Vector2(2 - PF_W / 2f, -40);

                var titleObj = new GameObject("Title");
                titleObj.transform.SetParent(notifObject.transform, false);
                var tc = titleObj.AddComponent<Canvas>(); tc.overrideSorting = true; tc.sortingOrder = 10;
                titleText = titleObj.AddComponent<TextMeshProUGUI>();
                titleText.text = title; titleText.fontSize = 18; titleText.fontStyle = FontStyles.Bold;
                titleText.color = textWhite; titleText.alignment = TextAlignmentOptions.Left;
                titleText.enableWordWrapping = false; titleText.richText = true; titleText.raycastTarget = false;
                var titleRect = titleObj.GetComponent<RectTransform>();
                titleRect.sizeDelta = new Vector2(300f, 30f); titleRect.anchoredPosition = new Vector2(20, 15);

                var msgObj = new GameObject("Message");
                msgObj.transform.SetParent(notifObject.transform, false);
                var mc = msgObj.AddComponent<Canvas>(); mc.overrideSorting = true; mc.sortingOrder = 10;
                messageText = msgObj.AddComponent<TextMeshProUGUI>();
                messageText.text = message; messageText.fontSize = 14;
                messageText.color = textGrey; messageText.alignment = TextAlignmentOptions.Left;
                messageText.enableWordWrapping = true; messageText.richText = true; messageText.raycastTarget = false;
                var msgRect = msgObj.GetComponent<RectTransform>();
                msgRect.sizeDelta = new Vector2(300f, 35f); msgRect.anchoredPosition = new Vector2(20, -10);
            }

            public IEnumerator Show()
            {
                float elapsed = 0f, fadeTime = 0.3f;
                while (elapsed < fadeTime)
                {
                    if (notifObject == null || rectTransform == null) { currentState = State.Completed; yield break; }
                    float t = elapsed / fadeTime;
                    rectTransform.localScale = Vector3.one * EaseOutBack(t);
                    UpdateAlpha(t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                if (rectTransform != null) rectTransform.localScale = Vector3.one;
                UpdateAlpha(1f);
                currentState = State.Displaying;
            }

            private float EaseOutBack(float t)
            {
                float c1 = 1.70158f, c3 = c1 + 1f;
                return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
            }

            public IEnumerator LifeCycle()
            {
                yield return new WaitForSeconds(duration - 0.5f);
                yield return FadeOut(false);
            }

            public IEnumerator FadeOut(bool immediate)
            {
                currentState = State.FadingOut;
                float fadeTime = immediate ? 0.15f : 0.25f, elapsed = 0f;
                while (elapsed < fadeTime)
                {
                    if (notifObject == null || rectTransform == null) { currentState = State.Completed; yield break; }
                    float t = 1f - (elapsed / fadeTime);
                    rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                    UpdateAlpha(t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                ForceDestroy();
                currentState = State.Completed;
            }

            public void ForceDestroy()
            {
                if (notifObject != null) { UnityEngine.Object.Destroy(notifObject); notifObject = null; }
            }

            private void UpdateAlpha(float alpha)
            {
                if (notifObject == null) return;
                currentAlpha = alpha;
                if (background != null) background.color = new Color(1f, 1f, 1f, alpha);
                if (border != null) border.color = new Color(accent.r, accent.g, accent.b, alpha * 0.9f);
                if (stripe != null) stripe.color = new Color(accent.r, accent.g, accent.b, alpha);
                if (badge != null) badge.color = new Color(accent.r, accent.g, accent.b, alpha * 0.22f);
                if (iconText != null) iconText.color = new Color(accent.r, accent.g, accent.b, alpha);
                if (titleText != null) titleText.color = new Color(textWhite.r, textWhite.g, textWhite.b, alpha);
                if (messageText != null) messageText.color = new Color(textGrey.r, textGrey.g, textGrey.b, alpha * 0.98f);
                if (progressBg != null) progressBg.color = new Color(1f, 1f, 1f, alpha * 0.5f);
                if (progressFill != null) progressFill.color = new Color(accent.r, accent.g, accent.b, alpha);
                if (glow != null) glow.color = new Color(accent.r, accent.g, accent.b, alpha * 0.55f);
            }

            public void Update()
            {
                if (notifObject == null || rectTransform == null) return;
                if (Time.time - lastUpdateTime < UPDATE_INTERVAL) return;
                lastUpdateTime = Time.time;

                var cur = rectTransform.anchoredPosition;
                var tar = new Vector2(0, targetY);
                if (Vector2.Distance(cur, tar) > 0.1f)
                    rectTransform.anchoredPosition = Vector2.Lerp(cur, tar, Time.deltaTime * 8f);

                if (currentState == State.Displaying && progressFill != null)
                    progressFill.fillAmount = Mathf.Clamp01(1f - ((Time.time - startTime) / duration));
            }
        }
    }
}
