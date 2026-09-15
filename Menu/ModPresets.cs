using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LagMenu.Menu
{
    public static class ModPresets
    {
        private static readonly string FolderPath = Path.Combine(
            Directory.GetCurrentDirectory(), "LagMenu");

        private static readonly string SavePath = Path.Combine(FolderPath, "save.txt");
        private static readonly string ThemeSavePath = Path.Combine(FolderPath, "theme.txt");

        public static bool HasSave => File.Exists(SavePath);
        public static bool HasThemeSave => File.Exists(ThemeSavePath);

        public static void Init()
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Failed to create folder: " + e);
            }
        }

        public static void Save()
        {
            try
            {
                Init();

                List<string> list = new List<string>();

                foreach (ButtonInfo[] cat in Buttons.buttons)
                    foreach (ButtonInfo btn in cat)
                        if (btn.isTogglable && btn.enabled)
                            list.Add(btn.buttonText);

                File.WriteAllLines(SavePath, list);
                Debug.Log("[LagMenu] Saved " + list.Count + " mod(s) to " + SavePath);
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Failed to save mods: " + e);
            }
        }

        public static void LoadSave()
        {
            try
            {
                if (!HasSave)
                {
                    Debug.LogWarning("[LagMenu] No save file found at " + SavePath);
                    return;
                }

                string[] saved = File.ReadAllLines(SavePath);

                foreach (string b in saved)
                    foreach (ButtonInfo[] cat in Buttons.buttons)
                        foreach (ButtonInfo btn in cat)
                            if (btn.buttonText == b)
                            {
                                btn.enabled = true;
                                btn.enableMethod?.Invoke();
                            }

                Main.RecreateMenu();
                Debug.Log("[LagMenu] Loaded " + saved.Length + " mod(s).");
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Failed to load mods: " + e);
            }
        }


        public static void SaveTheme()
        {
            try
            {
                Init();
                File.WriteAllText(ThemeSavePath, Mods.ThemeChanger.CurrentName);
                Debug.Log("[LagMenu] Saved theme: " + Mods.ThemeChanger.CurrentName);
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Failed to save theme: " + e);
            }
        }

        public static void LoadTheme()
        {
            try
            {
                if (!HasThemeSave)
                {
                    Debug.Log("[LagMenu] No theme save found – using default.");
                    return;
                }

                string name = File.ReadAllText(ThemeSavePath).Trim();

                int index = Mods.ThemeChanger.Presets.FindIndex(
                    t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (index < 0)
                {
                    Debug.LogWarning("[LagMenu] Saved theme '" + name + "' not found – using default.");
                    return;
                }

                Mods.ThemeChanger.Apply(index);
                Debug.Log("[LagMenu] Loaded theme: " + name);
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Failed to load theme: " + e);
            }
        }

        public static void SaveAll()
        {
            Save();
            SaveTheme();
        }

        public static void LoadAll()
        {
            LoadSave();
            LoadTheme();
        }
    }
}
