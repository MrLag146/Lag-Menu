
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace LagMenu.Mods
{
    internal class Settings
    {
        public static float flyspeed = 14f;
        public static bool mediaIntUI = true;


        public static float menuScale = 1f;
        public static float menuDistance = 1f;
        public static bool mirrorText = false;
        public static float textScale = 2.7f;
        public static bool showHome = true;
        public static bool showDisconnect = true;
        public static bool betaUnlocked = false;
        public static float armsLength = 1.5f;
        public static float speedBoost = 1.3f;
        public static float speedBoostMax = 1.4f;
        public static float pullPower = 0.5f;

        public static bool useClickV2 = false;

        public static bool Rounding = false;
        public static bool Outline = false;
        public static int throwableIndex = 0;
        public enum ButtonSound
        {
            Default,
            ClickV2,
            Steal,
            Gmod,
            RobloxButton
        }
        public static bool randomizecolorproj = false;
        public static int ButtonSoundCycle;
        public static ButtonSound ActiveButtonSound = ButtonSound.Default;

        public static void ToggleButtonSound()
        {
            int count = Enum.GetValues(typeof(ButtonSound)).Length;
            ApplyButtonSoundType((ButtonSoundCycle + 1) % count);
        }
        public static int LagPackets = 450;
        public static float LagCooldown = 1f;
        private static float _lagCooldown = 0f;
        public static void ApplyButtonSoundType(int index)
        {
            int count = Enum.GetValues(typeof(ButtonSound)).Length;
            if (index < 0 || index >= count) index = 0;

            ButtonSoundCycle = index;
            ActiveButtonSound = (ButtonSound)index;
        }
        public static void IncreaseFlySpeed()
        {
            flyspeed += 2f;
        }
        public static void DecreaseFlySpeed()
        {
            flyspeed -= 1f;
            if (flyspeed < 1f)
            {
                flyspeed = 1f;
            }
        }
        public static Font AgencyFB;
        public static Font FreeSans;
        public static Font DejaVuSans;
        public static Font Utopium;
        public static Font ComicSans;
        public static Font CascadiaMono;
        public static Font Candara;
        public static Font MSGothic;
        public static Font Anton;
        public static Font SimSun;
        public static Font Minecraft;
        public static Font Terminal;
        public static Font OpenDyslexic;
        public static Font Taiko;
        public static Font LiberationSans;
        public static Font JetBrainsMonoNLBold;
        public static Font activeFont;
        public static int fontCycle;
        public static void ApplyFontType(int index)
        {
            fontCycle = index;
            switch (index)
            {
                case 0:
                    activeFont = AgencyFB;
                    break;
                case 1:
                    activeFont = FreeSans;
                    break;
                case 2:
                    activeFont = DejaVuSans;
                    break;
                case 3:
                    activeFont = Utopium;
                    break;
                case 4:
                    activeFont = ComicSans;
                    break;
                case 5:
                    activeFont = CascadiaMono;
                    break;
                case 6:
                    activeFont = Candara;
                    break;
                case 7:
                    activeFont = MSGothic;
                    break;
                case 8:
                    activeFont = Anton;
                    break;
                case 9:
                    activeFont = SimSun;
                    break;
                case 10:
                    activeFont = Minecraft;
                    break;
                case 11:
                    activeFont = Terminal;
                    break;
                case 12:
                    activeFont = OpenDyslexic;
                    break;
                case 13:
                    activeFont = Taiko;
                    break;
                case 14:
                    activeFont = LiberationSans;
                    break;
                case 15:
                    activeFont = JetBrainsMonoNLBold;
                    break;
                default:
                    activeFont = AgencyFB;
                    fontCycle = 0;
                    break;
            }
        }


        public static void SetFPS(int fps)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = fps;
        }

        public static void UnlockFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }

        public static void DisableNetworkTriggers(bool disable)
        {
            GameObject triggers = GameObject.Find("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
            if (triggers != null) triggers.SetActive(!disable);
        }
    }
}
