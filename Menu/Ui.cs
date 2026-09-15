using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaNetworking;
using LagMenu.Menu;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LagMenu.Menu
{
    public class LagMenuOnGUI : MonoBehaviour
    {
        public static bool IsVisible = false;
        public static bool CmdVisible = false;
        public static bool ArrayListVisible = true;
        public static LagMenuOnGUI Instance { get; private set; }

        private const float PAD = 10f;
        private const float BTN_H = 24f;
        private const float BTN_GAP = 3f;
        private const float TAB_H = 22f;
        private const float HEADER_H = 30f;
        private const float WIN_W = 640f;
        private const float MOD_H = 20f;

        private static Color32 C_BG = new Color32(10, 10, 22, 210);
        private static Color32 C_HEADER = new Color32(18, 0, 45, 230);
        private static Color32 C_ACCENT = new Color32(110, 0, 220, 255);
        private static Color32 C_BTN = new Color32(28, 0, 58, 200);
        private static Color32 C_BTN_HOV = new Color32(50, 0, 100, 220);
        private static Color32 C_BTN_ON = new Color32(80, 0, 170, 220);
        private static Color32 C_BTN_ON_H = new Color32(100, 0, 200, 240);
        private static Color32 C_TAB = new Color32(18, 0, 40, 200);
        private static Color32 C_TAB_SEL = new Color32(90, 0, 180, 230);
        private static Color32 C_MOD_BG = new Color32(0, 0, 0, 175);
        private static Color32 C_WM_BG = new Color32(0, 0, 0, 145);
        private static Color32 C_STRIP = new Color32(110, 0, 220, 200);
        private static Color32 C_FIELD = new Color32(15, 5, 35, 200);
        private static Color32 C_SEP = new Color32(80, 0, 160, 120);

        private readonly Dictionary<string, Texture2D> _tex = new Dictionary<string, Texture2D>();
        private Texture2D _pixel;

        private GUIStyle _sBtnOff, _sBtnOn, _sLabel, _sHeader, _sTab, _sTabSel;
        private GUIStyle _sWatermark, _sMod, _sField, _sTooltip, _sSmall;
        private bool _stylesBuilt;

        private Rect _win;
        private Rect _cmdWin;
        private Vector2 _scroll;
        private bool _dirty = true;
        private int _lastW, _lastH;

        private float _scrollY = 0f;
        private float _scrollTarget = 0f;

        private int _tabScroll = 0;
        private const int TABS_VISIBLE = 5;

        private float _smoothFps;
        private float _fpsTimer;
        private int _displayFps;
        private string _fpsText = "FPS: --";
        private string _pingText = "Ping: --";

        private string _wmText;
        private float _wmW;
        private readonly Dictionary<string, ModAnimNode> _modAnim = new Dictionary<string, ModAnimNode>();
        private string _modListTitle = "LagMenu";
        private GUIStyle _sModTitle;
        private Texture2D _gradTex;
        private Color _gradC1, _gradC2;
        private float _headW, _headV;
        private Texture2D _wmTex;
        private int _lastWmFps = -1;

        private static readonly List<string> _mods = new List<string>();
        private static readonly HashSet<string> _modSet = new HashSet<string>();
        private static readonly Dictionary<string, float> _modIn = new Dictionary<string, float>();
        private static readonly Dictionary<string, float> _modOut = new Dictionary<string, float>();
        private static readonly List<string> _modBuf = new List<string>();
        private static readonly List<(string m, float w)> _sorted = new List<(string, float)>(32);
        private readonly Dictionary<int, Texture2D> _modTex = new Dictionary<int, Texture2D>();

        private float _animT = 1f;
        private bool _animIn = false;
        private bool _animOut = false;
        private bool _wasVis = true;

        private string _tooltipFrame = "";
        private string _tooltipShow = "";
        private float _tooltipAnim = 0f;

        private string _inputText = "";
        private string _nameInput = "";

        private Player _selectedPlayer;
        private Vector2 _playerScroll;

        private string _cmdInput = "";
        private string _cmdOutput = "Type 'help' for a list of commands.";

        private static int _origW, _origH;
        private static bool _resCap;



        private Rect _tabBarScreenRect;



        private class ModAnimNode
        {
            public float x, dest, v;
            public void Update(float smoothTime) => x = Mathf.SmoothDamp(x, dest, ref v, smoothTime);
        }




        private void Awake()
        {
            Instance = this;

            if (!_resCap)
            {
                _origW = Screen.currentResolution.width * 2;
                _origH = Screen.currentResolution.height * 2;
                _resCap = true;
            }

            _pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            _pixel.SetPixels32(new Color32[] { new Color32(255, 255, 255, 255) });
            _pixel.Apply(false, true);
            _gradTex = new Texture2D(64, 1) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };

            _win = new Rect(_origW / 4f - WIN_W / 2f, _origH / 4f - 200f, WIN_W, 400f);
            _cmdWin = new Rect(_win.x, _win.y + 410f, WIN_W, 70f);
        }

        private void OnDestroy()
        {
            foreach (var kv in _tex)
                if (kv.Value) Destroy(kv.Value);
            _tex.Clear();

            foreach (var kv in _modTex)
                if (kv.Value) Destroy(kv.Value);
            _modTex.Clear();

            if (_pixel) Destroy(_pixel);
            if (_wmTex) Destroy(_wmTex);
            if (_gradTex) Destroy(_gradTex);
        }

        private void Update()
        {
            if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame || Keyboard.current[Key.RightAlt].wasPressedThisFrame)
            {
                IsVisible = !IsVisible;
                if (IsVisible) { _animIn = true; _animOut = false; _animT = 0f; }
                else { _animOut = true; _animIn = false; _animT = 1f; }
            }

            if (Keyboard.current[Key.Backslash].wasPressedThisFrame)
                ArrayListVisible = !ArrayListVisible;

            if (Keyboard.current[Key.Semicolon].wasPressedThisFrame)
                CmdVisible = !CmdVisible;

            if (IsVisible)
            {
                if (Keyboard.current[Key.Comma].wasPressedThisFrame)
                    _tabScroll = Mathf.Max(0, _tabScroll - 1);
                if (Keyboard.current[Key.Period].wasPressedThisFrame)
                    _tabScroll = Mathf.Min(Main.categoryNames.Length - TABS_VISIBLE, _tabScroll + 1);
            }

            _smoothFps = Mathf.Lerp(_smoothFps, 1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime), Time.unscaledDeltaTime * 4f);
            _fpsTimer -= Time.unscaledDeltaTime;
            if (_fpsTimer <= 0f)
            {
                _displayFps = Mathf.RoundToInt(_smoothFps);
                _fpsText = $"FPS: {_displayFps}";
                _pingText = $"Ping: {PhotonNetwork.GetPing()} ms";
                _fpsTimer = 0.5f;
            }

            float now = Time.unscaledTime;
            _modBuf.Clear();
            foreach (var kv in _modOut)
                if (now - kv.Value >= 0.4f) _modBuf.Add(kv.Key);
            foreach (var k in _modBuf)
            {
                _mods.Remove(k); _modSet.Remove(k);
                _modIn.Remove(k); _modOut.Remove(k);
            }
            foreach (var k in _modBuf)
            {
                _mods.Remove(k); _modSet.Remove(k);
                _modIn.Remove(k); _modOut.Remove(k);
                _modAnim.Remove(k);
            }
            int w = Screen.width, h = Screen.height;
            if (w != _lastW || h != _lastH) { _lastW = w; _lastH = h; _dirty = true; }

            foreach (var cat in Buttons.buttons)
                foreach (var btn in cat)
                    if (btn.enabled && btn.isTogglable)
                        btn.method?.Invoke();
        }

        private void OnGUI()
        {
            bool menuActive = IsVisible || _animOut;
            bool arrayListActive = ArrayListVisible && (_mods.Count > 0 || _modAnim.Count > 0);
            if (!menuActive && !arrayListActive) return;

            if (_dirty || !_stylesBuilt)
            {
                RebuildTextures();
                BuildStyles();
                _dirty = false;
            }

            float scale = (float)Screen.width / (_origW / 2f);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);

            if (_animIn)
            {
                _animT = Mathf.MoveTowards(_animT, 1f, Time.unscaledDeltaTime * 8f);
                if (_animT >= 1f) { _animT = 1f; _animIn = false; }
            }
            if (_animOut)
            {
                _animT = Mathf.MoveTowards(_animT, 0f, Time.unscaledDeltaTime * 8f);
                if (_animT <= 0f) { _animT = 0f; _animOut = false; }
            }
            _wasVis = IsVisible;

            menuActive = IsVisible || _animOut;
            if (!menuActive && !arrayListActive) return;

            Matrix4x4 mat = GUI.matrix;

            if (arrayListActive)
            {
                GUI.color = Color.white;
                GUI.matrix = mat;
                DrawModList();
            }

            if (menuActive)
            {
                float ease = 1f - Mathf.Pow(1f - _animT, 3f);
                GUI.color = new Color(1, 1, 1, ease);

                float slideY = Mathf.Lerp(-50f, 0f, ease);
                GUI.matrix = mat * Matrix4x4.Translate(new Vector3(0, slideY, 0));
                DrawWatermark();

                _tooltipAnim = Mathf.Lerp(_tooltipAnim, string.IsNullOrEmpty(_tooltipFrame) ? 0f : 1f, Time.unscaledDeltaTime * 14f);
                if (!string.IsNullOrEmpty(_tooltipFrame)) _tooltipShow = _tooltipFrame;
                if (_tooltipAnim > 0.01f) DrawTooltip(_tooltipAnim * ease);
                _tooltipFrame = "";

                GUI.matrix = mat;
                if (_win.width < 1f) _win = new Rect(_origW / 4f - WIN_W / 2f, _origH / 4f - 200f, WIN_W, 400f);
                _win = GUI.Window(42001, _win, DrawWin, "", GUIStyle.none);

                if (CmdVisible)
                {
                    if (_cmdWin.width < 1f) _cmdWin = new Rect(_win.x, _win.y + _win.height + 10f, WIN_W, 70f);
                    _cmdWin = GUI.Window(42003, _cmdWin, DrawCmdWin, "", GUIStyle.none);
                }
            }
            else
            {
                _tooltipFrame = "";
            }

            GUI.matrix = mat;
            GUI.color = Color.white;
        }

        private void DrawWin(int id)
        {
            float w = _win.width;
            float h = _win.height;

            DrawRect(new Rect(0, 0, w, h), C_BG, 8);

            DrawRect(new Rect(0, 0, 3, h), C_ACCENT, 0);

            float y = 0f;

            DrawRect(new Rect(0, y, w, HEADER_H), C_HEADER, 0);
            DrawRect(new Rect(0, y + HEADER_H - 1.5f, w, 1.5f), C_ACCENT, 0);

            string catName = Main.category == -1 ? "Players"
                : (Main.category >= 0 && Main.category < Main.categoryNames.Length
                    ? Main.categoryNames[Main.category] : "LagMenu");
            GUI.Label(new Rect(PAD + 4, y, w - PAD * 2, HEADER_H),
                $"<b>LagMenu</b>  <color=#AA66FF>{catName}</color>", _sHeader);

            GUI.Label(new Rect(w - 110f, y, 105f, HEADER_H),
                $"<color=#66FF88>{_fpsText}</color>  <color=#6699FF>{_pingText}</color>", _sSmall);

            y += HEADER_H;

            DrawRect(new Rect(0, y, w, TAB_H), new Color32(8, 0, 20, 220), 0);

            float playersTabW = 64f;
            bool playersSel = Main.category == -1;
            Rect playersTr = new Rect(PAD, y, playersTabW, TAB_H);
            DrawRect(playersTr, playersSel ? C_TAB_SEL : C_TAB, 4);
            if (playersSel) DrawRect(new Rect(PAD, y + TAB_H - 2f, playersTabW, 2f), C_ACCENT, 0);
            if (GUI.Button(playersTr, "Players", playersSel ? _sTabSel : _sTab))
            {
                Main.category = -1;
                _scrollY = 0f; _scrollTarget = 0f;
            }

            float tabsStartX = PAD + playersTabW + BTN_GAP;
            int tabCount = Main.categoryNames.Length;
            int visTabs = Mathf.Min(TABS_VISIBLE, tabCount);
            _tabScroll = Mathf.Clamp(_tabScroll, 0, Mathf.Max(0, tabCount - visTabs));
            float tabW = (w - tabsStartX - PAD) / visTabs;

            for (int i = 0; i < visTabs; i++)
            {
                int idx = i + _tabScroll;
                if (idx >= tabCount) break;

                bool sel = Main.category == idx;
                float tx = tabsStartX + i * tabW;
                Rect tr = new Rect(tx, y, tabW - 1f, TAB_H);

                DrawRect(tr, sel ? C_TAB_SEL : C_TAB, 4);
                if (sel) DrawRect(new Rect(tx, y + TAB_H - 2f, tabW - 1f, 2f), C_ACCENT, 0);

                GUIStyle st = sel ? _sTabSel : _sTab;
                if (GUI.Button(tr, Main.categoryNames[idx], st))
                {
                    Main.category = idx;
                    Main.page = 0;
                    _scrollY = 0f; _scrollTarget = 0f;
                }
            }

            _tabBarScreenRect = new Rect(0, y - TAB_H, w, TAB_H);

            if (_tabScroll > 0)
                GUI.Label(new Rect(tabsStartX - 8f, y - TAB_H + 2f, 8f, TAB_H - 4f), "‹", _sSmall);
            if (_tabScroll + visTabs < tabCount)
                GUI.Label(new Rect(w - 10f, y - TAB_H + 2f, 8f, TAB_H - 4f), "›", _sSmall);

            y += TAB_H;
            DrawRect(new Rect(0, y, w, 1f), C_SEP, 0);
            y += 1f;

            DrawRect(new Rect(PAD, y + 2f, w - PAD * 2f, BTN_H - 4f), C_FIELD, 5);
            DrawRect(new Rect(PAD, y + BTN_H - 2f, w - PAD * 2f, 1f), C_ACCENT, 0);
            string newInput = GUI.TextField(
                new Rect(PAD + 4f, y + 3f, w - PAD * 2f - 8f, BTN_H - 6f),
                _inputText, _sField);
            if (newInput != _inputText) _inputText = newInput.ToUpper();
            y += BTN_H + BTN_GAP;

            float qw = (w - PAD * 2f - BTN_GAP) / 2f;
            DrawRect(new Rect(PAD, y + 2f, w - PAD * 2f - 60f - BTN_GAP, BTN_H - 4f), C_FIELD, 5);
            DrawRect(new Rect(PAD, y + BTN_H - 2f, w - PAD * 2f - 60f - BTN_GAP, 1f), C_ACCENT, 0);
            _nameInput = GUI.TextField(
                new Rect(PAD + 4f, y + 3f, w - PAD * 2f - 60f - BTN_GAP - 8f, BTN_H - 6f),
                _nameInput, _sField);
            if (DrawBtn(new Rect(w - PAD - 56f, y, 56f, BTN_H - 2f), "Set Name", false))
            {
                if (!string.IsNullOrEmpty(_nameInput))
                {
                    GorillaComputer.instance.currentName = _nameInput;
                    GorillaComputer.instance.SetLocalNameTagText(_nameInput);
                    GorillaComputer.instance.savedName = _nameInput;
                    PlayerPrefs.SetString("playerName", _nameInput);
                    PlayerPrefs.Save();
                    Photon.Pun.PhotonNetwork.LocalPlayer.NickName = _nameInput;
                }
            }
            y += BTN_H + BTN_GAP;

            if (DrawBtn(new Rect(PAD, y, qw, BTN_H - 2f), "Join Room", false))
            {
                if (!string.IsNullOrEmpty(_inputText))
                    PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(_inputText, GorillaNetworking.JoinType.Solo);
            }
            if (DrawBtn(new Rect(PAD + qw + BTN_GAP, y, qw, BTN_H - 2f), "Disconnect", false))
                PhotonNetwork.Disconnect();
            y += BTN_H + BTN_GAP * 2f;

            if (Main.category == -1)
            {
                DrawPlayerListContent(y, w);
                GUI.DragWindow(new Rect(0, 0, w, HEADER_H));
                return;
            }

            int cat = Main.category;
            if (cat < 0 || cat >= Buttons.buttons.Length) cat = 0;

            var visible = Buttons.buttons[cat]
                .Where(b => b.shouldShow == null || b.shouldShow())
                .ToArray();

            float listH = visible.Length * (BTN_H + BTN_GAP) + 6f;
            float viewH = Mathf.Max(80f, _win.height - y - PAD);

            _scrollY = Mathf.Lerp(_scrollY, _scrollTarget, Time.unscaledDeltaTime * 14f);
            _scrollY = Mathf.Clamp(_scrollY, 0f, Mathf.Max(0f, listH - viewH));

            if (Event.current.type == EventType.ScrollWheel)
            {
                Vector2 mp = Event.current.mousePosition;
                if (_tabBarScreenRect.Contains(mp))
                {
                    _tabScroll = Mathf.Clamp(
                        _tabScroll + (int)Mathf.Sign(Event.current.delta.y),
                        0, Mathf.Max(0, tabCount - visTabs));
                    Main.category = Mathf.Clamp(Main.category, _tabScroll, _tabScroll + visTabs - 1);
                }
                else
                {
                    _scrollTarget = Mathf.Clamp(
                        _scrollTarget + Event.current.delta.y * 18f,
                        0f, Mathf.Max(0f, listH - viewH));
                }
                Event.current.Use();
            }

            GUI.BeginGroup(new Rect(0, y, w, viewH));
            float by = -_scrollY + 2f;

            foreach (var info in visible)
            {
                var cap = info;
                Rect br = new Rect(PAD, by, w - PAD * 2f, BTN_H - 2f);

                if (by + BTN_H > 0 && by < viewH)
                {
                    bool isOn = cap.isTogglable && cap.enabled;
                    bool clicked = DrawModBtn(br, cap.overlapText ?? cap.buttonText, isOn, cap.toolTip);

                    if (clicked)
                    {
                        if (cap.isTogglable)
                        {
                            cap.enabled = !cap.enabled;
                            if (cap.enabled) { cap.enableMethod?.Invoke(); cap.method?.Invoke(); }
                            else cap.disableMethod?.Invoke();
                            UpdateModList(cap);
                        }
                        else
                        {
                            cap.method?.Invoke();
                        }
                    }
                }
                by += BTN_H + BTN_GAP;
            }

            GUI.EndGroup();

            float totalH = y + viewH + PAD;
            if (Mathf.Abs(_win.height - totalH) > 1f)
                _win.height = Mathf.Lerp(_win.height, totalH, Time.unscaledDeltaTime * 10f);

            if (listH > viewH)
            {
                float sbH = viewH * (viewH / listH);
                float sbY = y + (_scrollY / Mathf.Max(1f, listH - viewH)) * (viewH - sbH);
                DrawRect(new Rect(w - 3f, sbY, 3f, sbH), C_ACCENT, 0);
            }

            DrawRect(new Rect(0, _win.height - 16f, w, 16f), new Color32(6, 0, 15, 200), 0);
            string room = PhotonNetwork.InRoom
                ? $"  {PhotonNetwork.CurrentRoom.Name}  ·  {PhotonNetwork.CurrentRoom.PlayerCount}p"
                : "  not connected";
            GUI.Label(new Rect(PAD, _win.height - 16f, w - PAD * 2f, 16f), room, _sSmall);

            GUI.DragWindow(new Rect(0, 0, w, HEADER_H));
        }

        private void DrawPlayerListContent(float y, float w)
        {
            float viewH = Mathf.Max(80f, _win.height - y - PAD);
            GUI.BeginGroup(new Rect(0, y, w, viewH));

            if (!PhotonNetwork.InRoom)
            {
                GUI.Label(new Rect(PAD, 8f, w - PAD * 2f, 22f), "Not in a room.", _sSmall);
                GUI.EndGroup();
                return;
            }

            if (_selectedPlayer != null)
            {
                bool stillHere = PhotonNetwork.PlayerList.Any(p => p.ActorNumber == _selectedPlayer.ActorNumber);
                if (!stillHere) _selectedPlayer = null;
            }

            if (_selectedPlayer == null)
            {
                Player[] players = PhotonNetwork.PlayerList;
                float rh = BTN_H + 14f, gap = BTN_GAP;
                float total = players.Length * (rh + gap) + 6f;

                _playerScroll = GUI.BeginScrollView(new Rect(0, 0, w, viewH), _playerScroll, new Rect(0, 0, w - 20f, total));
                float py = 2f;
                foreach (Player p in players)
                {
                    Rect r = new Rect(PAD, py, w - PAD * 2f - 20f, rh - 2f);
                    bool isLocal = p.IsLocal;
                    bool isMaster = p.IsMasterClient;

                    DrawRect(r, isLocal ? C_TAB_SEL : C_FIELD, 5);
                    if (isLocal) DrawRect(new Rect(r.x, r.y + 2f, 3f, r.height - 4f), C_ACCENT, 0);

                    string displayName = string.IsNullOrEmpty(p.NickName) ? "Player " + p.ActorNumber : p.NickName;
                    GUI.Label(new Rect(r.x + 12f, r.y + 3f, r.width - 30f, r.height * 0.55f), displayName, _sLabel);

                    string sub = isLocal ? "you" : "";
                    if (isMaster) sub += (sub.Length > 0 ? " · " : "") + "host";
                    sub += (sub.Length > 0 ? " · " : "") + "id:" + p.ActorNumber;
                    GUI.Label(new Rect(r.x + 12f, r.y + r.height * 0.5f, r.width - 30f, r.height * 0.45f), sub, _sSmall);

                    if (!isLocal)
                    {
                        GUI.Label(new Rect(r.xMax - 20f, r.y, 16f, r.height), "›", _sSmall);
                        if (GUI.Button(r, "", GUIStyle.none))
                        {
                            _selectedPlayer = p;
                            _playerScroll = Vector2.zero;
                        }
                    }
                    py += rh + gap;
                }
                GUI.EndScrollView();
            }
            else
            {
                Player sp = _selectedPlayer;
                string nm = string.IsNullOrEmpty(sp.NickName) ? "Player " + sp.ActorNumber : sp.NickName;

                if (DrawBtn(new Rect(PAD, 2f, 60f, BTN_H - 2f), "Back", false))
                {
                    _selectedPlayer = null;
                }

                GUI.Label(new Rect(PAD, BTN_H + 6f, w - PAD * 2f, 22f), $"<b>{nm}</b>", _sHeader);

                float ay = BTN_H + 34f;
                if (DrawBtn(new Rect(PAD, ay, w - PAD * 2f, BTN_H - 2f), "Copy PlayerID", false))
                {
                    GUIUtility.systemCopyBuffer = sp.UserId;
                }
                ay += BTN_H + BTN_GAP;

                if (DrawBtn(new Rect(PAD, ay, w - PAD * 2f, BTN_H - 2f), "Teleport To Player", false))
                {
                    VRRig rig = RigManager.GetRigFromPlayer(sp);
                    if (rig != null)
                        GTPlayer.Instance.TeleportTo(rig.transform.position, GTPlayer.Instance.transform.rotation);
                }
            }

            GUI.EndGroup();
        }

        private void DrawCmdWin(int id)
        {
            float w = _cmdWin.width;
            float h = _cmdWin.height;

            DrawRect(new Rect(0, 0, w, h), C_BG, 8);
            DrawRect(new Rect(0, 0, 3, h), C_ACCENT, 0);
            DrawRect(new Rect(0, 0, w, HEADER_H * 0.7f), C_HEADER, 0);
            DrawRect(new Rect(0, HEADER_H * 0.7f - 1.5f, w, 1.5f), C_ACCENT, 0);
            GUI.Label(new Rect(PAD, 0, w - PAD * 2f, HEADER_H * 0.7f), "<b>Command Prompt</b>", _sHeader);

            float y = HEADER_H * 0.7f + 6f;
            float cmdBtnW = 26f;

            DrawRect(new Rect(PAD, y, w - PAD * 2f - cmdBtnW - BTN_GAP, BTN_H - 2f), C_FIELD, 5);
            DrawRect(new Rect(PAD, y + BTN_H - 3f, w - PAD * 2f - cmdBtnW - BTN_GAP, 1f), C_ACCENT, 0);
            GUI.SetNextControlName("lagmenu_cmd_field");
            _cmdInput = GUI.TextField(
                new Rect(PAD + 4f, y + 1f, w - PAD * 2f - cmdBtnW - BTN_GAP - 8f, BTN_H - 4f),
                _cmdInput, _sField);

            bool cmdSubmit = DrawBtn(new Rect(w - PAD - cmdBtnW, y, cmdBtnW, BTN_H - 2f), ">", false);
            bool cmdEnter = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return
                            && GUI.GetNameOfFocusedControl() == "lagmenu_cmd_field";

            if (cmdSubmit || cmdEnter)
            {
                if (!string.IsNullOrEmpty(_cmdInput))
                {
                    RunCommand(_cmdInput.Trim());
                    _cmdInput = "";
                }
                if (cmdEnter) Event.current.Use();
            }
            y += BTN_H + 4f;

            GUI.Label(new Rect(PAD + 2f, y, w - PAD * 2f, MOD_H),
                $"<color=#AA66FF>›</color> {_cmdOutput}", _sSmall);

            GUI.DragWindow(new Rect(0, 0, w, HEADER_H * 0.7f));
        }

        private void DrawModList()
        {
            float canvasW = _origW / 2f;
            float y = 12f;

            Color titleFlow = FlowColor();
            _sModTitle.normal.textColor = Color.Lerp(titleFlow, Color.white, 0.5f);
            var titleContent = new GUIContent(_modListTitle);
            Vector2 ts = _sModTitle.CalcSize(titleContent);
            Rect titleRect = new Rect(canvasW - ts.x - 20f, y, ts.x + 10f, ts.y + 6f);
            GUI.Label(titleRect, _modListTitle, _sModTitle);
            y += ts.y + 10f;

            _headW = Mathf.SmoothDamp(_headW, ts.x + 5f, ref _headV, 0.3f);
            Rect underline = new Rect(canvasW - 15f - _headW, y, _headW, 2f);
            DrawGradient(underline, C_ACCENT, C_STRIP);
            y += 10f;

            _sorted.Clear();
            for (int i = 0; i < _mods.Count; i++)
            {
                var gc = new GUIContent(_mods[i]);
                _sorted.Add((_mods[i], _sMod.CalcSize(gc).x));
            }
            _sorted.Sort((a, b) => b.w.CompareTo(a.w));

            int i2 = 0;
            foreach (var entry in _sorted)
            {
                string mod = entry.m;
                float tw = entry.w;
                float w = tw + 24f;
                float h = 26f;

                bool removing = _modOut.ContainsKey(mod);

                if (!_modAnim.TryGetValue(mod, out var node))
                {
                    node = new ModAnimNode { x = w + 20f, dest = 0f };
                    _modAnim[mod] = node;
                }
                node.dest = removing ? w + 20f : 0f;
                node.Update(0.15f);

                float dx = canvasW - w - 10f + node.x;
                Rect box = new Rect(dx, y, w, h);

                DrawRect(box, C_MOD_BG, 3);

                Color flow = FlowColor(i2 * 0.2f);
                DrawRect(new Rect(box.x + box.width - 2f, box.y, 2f, h), flow, 0);

                _sMod.normal.textColor = flow;
                GUI.Label(new Rect(box.x + 8f, box.y + 1f, box.width - 14f, h), mod, _sMod);

                y += h + 3f;
                i2++;
            }
            _sMod.normal.textColor = Color.white;
        }

        private void DrawGradient(Rect rect, Color c1, Color c2)
        {
            if (_gradC1 != c1 || _gradC2 != c2)
            {
                _gradC1 = c1;
                _gradC2 = c2;
                for (int i = 0; i < 64; i++)
                    _gradTex.SetPixel(i, 0, Color.Lerp(c1, c2, i / 63f));
                _gradTex.Apply();
            }
            Color prev = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(rect, _gradTex);
            GUI.color = prev;
        }

        private void DrawWatermark()
        {
            if (_displayFps != _lastWmFps)
            {
                _lastWmFps = _displayFps;
                _wmText = $"LagMenu  |  FPS: {_displayFps}  |  \\ to toggle";
                var gc = new GUIContent(_wmText);
                _wmW = _sWatermark.CalcSize(gc).x + 20f;
            }

            float wx = _origW / 4f - _wmW / 2f;

            if (!_wmTex || ((Texture)_wmTex).width != (int)_wmW)
            {
                if (_wmTex) Destroy(_wmTex);
                _wmTex = MakeRoundRect((int)_wmW, 26, C_WM_BG, default, 0, 8);
            }

            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(wx, 8f, _wmW, 26f), _wmTex);

            _sWatermark.normal.textColor = FlowColor();
            GUI.Label(new Rect(wx + 10f, 8f, _wmW, 26f), _wmText, _sWatermark);
            _sWatermark.normal.textColor = Color.white;
        }

        private void DrawTooltip(float alpha)
        {
            if (string.IsNullOrEmpty(_tooltipShow)) return;
            var gc = new GUIContent(_tooltipShow);
            float tw = _sTooltip.CalcSize(gc).x + 16f;
            float th = 22f;

            Vector2 mp = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : Vector2.zero;
            float scale = (float)Screen.width / (_origW / 2f);
            float tx = Mathf.Clamp(mp.x / scale + 14f, 0f, _origW / 2f - tw - 4f);
            float ty = Mathf.Clamp(_origH / 2f - mp.y / scale - th - 10f, 0f, _origH / 2f - th - 4f);

            Color prev = GUI.color;
            GUI.color = new Color(1, 1, 1, alpha);
            DrawRect(new Rect(tx, ty, tw, th), new Color32(10, 0, 28, 220), 5);
            DrawRect(new Rect(tx, ty, 2.5f, th), C_ACCENT, 0);
            _sTooltip.normal.textColor = new Color(1, 1, 1, alpha);
            GUI.Label(new Rect(tx + 8f, ty, tw - 8f, th), _tooltipShow, _sTooltip);
            GUI.color = prev;
        }

        private void RunCommand(string raw)
        {
            string cmd = raw.ToLowerInvariant();
            switch (cmd)
            {
                case "help":
                    _cmdOutput = "help, clear, id, beta, exit";
                    break;
                case "clear":
                    _cmdOutput = "";
                    break;
                case "id":
                    string id = PhotonNetwork.LocalPlayer != null ? PhotonNetwork.LocalPlayer.UserId : "unknown";
                    GUIUtility.systemCopyBuffer = id;
                    _cmdOutput = "Copied to clipboard: " + id;
                    break;
                case "beta":
                    LagMenu.Mods.Settings.betaUnlocked = true;
                    _cmdOutput = "Beta category unlocked — check the main menu.";
                    break;
                case "exit":
                    IsVisible = false;
                    _cmdOutput = "";
                    break;
                default:
                    _cmdOutput = $"Unknown command: '{raw}'. Type 'help' for a list.";
                    break;
            }
        }

        private bool DrawBtn(Rect r, string label, bool on)
        {
            bool hov = r.Contains(Event.current.mousePosition);
            Color32 bg = on
     ? (hov ? Lighten(C_ACCENT, 0.15f) : C_ACCENT)
     : (hov ? C_BTN_HOV : C_BTN);
            DrawRect(r, bg, 5);
            DrawRect(new Rect(r.x, r.y, 3f, r.height), C_STRIP, 0);
            if (hov) _tooltipFrame = label;
            GUI.Label(r, label, on ? _sBtnOn : _sBtnOff);
            return GUI.Button(r, GUIContent.none, GUIStyle.none);
        }


        private static Color32 Lighten(Color32 c, float amt)
        {
            return new Color32(
                (byte)Mathf.Min(c.r + (int)(amt * 255), 255),
                (byte)Mathf.Min(c.g + (int)(amt * 255), 255),
                (byte)Mathf.Min(c.b + (int)(amt * 255), 255),
                c.a);
        }

        private bool DrawModBtn(Rect r, string label, bool on, string tooltip)
        {
            bool hov = r.Contains(Event.current.mousePosition);
            Color32 bg = on
                ? (hov ? C_BTN_ON_H : C_BTN_ON)
                : (hov ? C_BTN_HOV : C_BTN);
            DrawRect(r, bg, 5);
            DrawRect(new Rect(r.x, r.y, 3f, r.height), on ? C_ACCENT : C_STRIP, 0);



            if (hov && !string.IsNullOrEmpty(tooltip)) _tooltipFrame = tooltip;
            else if (hov) _tooltipFrame = label;

            GUI.Label(r, (on ? "● " : "○ ") + label, on ? _sBtnOn : _sBtnOff);


            return GUI.Button(r, GUIContent.none, GUIStyle.none);
        }

        private void DrawRect(Rect r, Color32 col, int radius)
        {
            if (radius <= 1)
            {
                Color prev = GUI.color;
                GUI.color = col;
                GUI.DrawTexture(r, _pixel);
                GUI.color = prev;
                return;
            }

            string key = $"{(int)r.width}x{(int)r.height}_{col.r}_{col.g}_{col.b}_{col.a}_{radius}";
            if (!_tex.TryGetValue(key, out Texture2D t) || !t)
            {
                t = MakeRoundRect((int)Mathf.Max(1, r.width), (int)Mathf.Max(1, r.height), col, default, 0, radius);
                _tex[key] = t;
            }
            GUI.DrawTexture(r, t);
        }

        private static Texture2D MakeRoundRect(int w, int h, Color32 fill, Color32 border, int bw, int r)
        {
            w = Mathf.Max(1, w);
            h = Mathf.Max(1, h);
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear };
            var px = new Color32[w * h];
            Color32 clear = new Color32(0, 0, 0, 0);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    int cx = x, cy = y;
                    bool round = false;

                    if (x < r && y >= h - r) { cx = r; cy = h - r; round = true; }
                    else if (x >= w - r && y >= h - r) { cx = w - r - 1; cy = h - r; round = true; }
                    else if (x < r && y < r) { cx = r; cy = r; round = true; }
                    else if (x >= w - r && y < r) { cx = w - r - 1; cy = r; round = true; }

                    if (round)
                    {
                        float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                        px[idx] = dist > r ? clear : (dist > r - bw && bw > 0) ? border : fill;
                    }
                    else
                    {
                        px[idx] = (bw > 0 && (x < bw || x >= w - bw || y < bw || y >= h - bw)) ? border : fill;
                    }
                }
            }
            t.SetPixels32(px);
            t.Apply(false, true);
            return t;
        }

        private Texture2D GetModTex(int bw)
        {
            if (_modTex.TryGetValue(bw, out var t) && t) return t;
            t = MakeRoundRect(Mathf.Max(1, bw), 20, C_MOD_BG, default, 0, 6);
            _modTex[bw] = t;
            return t;
        }

        private void RebuildTextures()
        {
            foreach (var kv in _tex) if (kv.Value) Destroy(kv.Value);
            _tex.Clear();
            foreach (var kv in _modTex) if (kv.Value) Destroy(kv.Value);
            _modTex.Clear();
            if (_wmTex) { Destroy(_wmTex); _wmTex = null; _lastWmFps = -1; }
        }

        private void BuildStyles()
        {
            _stylesBuilt = true;
            Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            _sBtnOff = MakeStyle(f, 11, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            _sBtnOn = MakeStyle(f, 11, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            _sLabel = MakeStyle(f, 11, FontStyle.Normal, TextAnchor.MiddleLeft, Color.white);
            _sHeader = MakeStyle(f, 13, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            _sHeader.richText = true;
            _sTab = MakeStyle(f, 9, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.7f, 0.6f, 0.9f));
            _sTabSel = MakeStyle(f, 9, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            _sWatermark = MakeStyle(f, 12, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            _sWatermark.richText = true;
            _sMod = MakeStyle(f, 11, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            _sModTitle = MakeStyle(f, 18, FontStyle.Bold, TextAnchor.MiddleRight, Color.white);
            _sField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 10,
                font = f,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            SetAllStates(_sField, MakeRoundRect(4, 4, C_FIELD, default, 0, 2));
            _sTooltip = MakeStyle(f, 10, FontStyle.Normal, TextAnchor.MiddleLeft, Color.white);
            _sSmall = MakeStyle(f, 8, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.7f, 0.7f, 0.8f));
            _sSmall.richText = true;
        }

        private static GUIStyle MakeStyle(Font f, int size, FontStyle fs, TextAnchor anchor, Color col)
        {
            var s = new GUIStyle { fontSize = size, font = f, fontStyle = fs, alignment = anchor };
            s.normal.textColor = col;
            s.hover.textColor = col;
            s.active.textColor = col;
            return s;
        }




        public void ApplyTheme(Color accent, Color bg, Color btnOn, Color btnOff)
        {
            C_ACCENT = accent;
            C_BG = bg;
            C_BTN_ON = btnOn;
            C_BTN = btnOff;
            C_STRIP = accent;
            C_BTN_ON_H = new Color(Mathf.Min(btnOn.r + 0.1f, 1f), Mathf.Min(btnOn.g + 0.1f, 1f), Mathf.Min(btnOn.b + 0.1f, 1f));
            C_BTN_HOV = new Color(Mathf.Min(btnOff.r + 0.08f, 1f), Mathf.Min(btnOff.g + 0.08f, 1f), Mathf.Min(btnOff.b + 0.08f, 1f));
            C_HEADER = Color.Lerp(bg, Color.black, 0.35f);
            _dirty = true;
        }


        private static void SetAllStates(GUIStyle s, Texture2D t)
        {
            s.normal.background = t;
            s.hover.background = t;
            s.active.background = t;
            s.focused.background = t;
            s.normal.textColor = s.hover.textColor = s.active.textColor = s.focused.textColor = Color.white;
        }

        public static void AddMod(string name)
        {
            if (_modSet.Contains(name)) return;
            _mods.Add(name);
            _modSet.Add(name);
            _modIn[name] = Time.unscaledTime;
        }

        public static void RemoveMod(string name)
        {
            if (!_modSet.Contains(name)) return;
            _modOut[name] = Time.unscaledTime;
        }

        private static void UpdateModList(ButtonInfo info)
        {
            if (!info.isTogglable) return;
            if (info.enabled) AddMod(info.overlapText ?? info.buttonText);
            else RemoveMod(info.overlapText ?? info.buttonText);
        }

        private static Color FlowColor(float offset = 0f)
        {
            float t = Mathf.PingPong(Time.unscaledTime * 1.4f - offset, 1f);
            return Color.Lerp(new Color(0.55f, 0.1f, 1f), new Color(0.2f, 0.7f, 1f), t);
        }
    }
}
