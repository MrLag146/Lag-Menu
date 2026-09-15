using System.Collections.Generic;
using UnityEngine;
using LagMenu.Menu;
using LagMenu.Utilities;

namespace LagMenu
{
    public class ButtonPress : MonoBehaviour
    {
        public ButtonInfo info;

        private const float COOLDOWN_TIME = 0.2f;

        private static readonly Dictionary<string, float> cooldowns = new Dictionary<string, float>();

        private static float globalLastPress = -999f;

        private void Update()
        {
            if (info == null) return;

            string key = info.buttonText;

            if (cooldowns.ContainsKey(key) && cooldowns[key] > 0f)
                cooldowns[key] -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (info == null) return;

            if (other.gameObject != Main.reference) return;

            string key = info.buttonText;

            if (cooldowns.ContainsKey(key) && cooldowns[key] > 0f) return;

            if (Time.time - globalLastPress < 0.1f) return;

            cooldowns[key] = COOLDOWN_TIME;
            globalLastPress = Time.time;
            switch (LagMenu.Mods.Settings.ActiveButtonSound)
            {
                case LagMenu.Mods.Settings.ButtonSound.ClickV2:
                    ResourceManager.PlayClickV2();
                    break;
                case LagMenu.Mods.Settings.ButtonSound.Steal:
                    ResourceManager.PlayStealSound();
                    break;
                case LagMenu.Mods.Settings.ButtonSound.Gmod:
                    ResourceManager.PlayGmodSound();
                    break;
                case LagMenu.Mods.Settings.ButtonSound.RobloxButton:
                    ResourceManager.PlayRobloxButtonSound();
                    break;
                default:
                    ResourceManager.PlayButtonSound();
                    break;
            }

            if (info.isTogglable)
            {
                info.enabled = !info.enabled;

                if (info.enabled)
                {
                    info.enableMethod?.Invoke();
                    info.method?.Invoke();
                }
                else
                {
                    info.disableMethod?.Invoke();
                }
            }
            else
            {
                info.method?.Invoke();
            }

            if (Main.ButtonClickAnimations)
            {
                StartCoroutine(Main.AnimateButtonClick(gameObject, info.buttonText));
            }
            else
            {
                Main.RecreateMenu();
            }
        }

        public static void ResetAllCooldowns()
        {
            cooldowns.Clear();
        }
    }
}
