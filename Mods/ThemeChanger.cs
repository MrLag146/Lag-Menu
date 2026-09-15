using LagMenu.Menu;
using System.Collections.Generic;
using UnityEngine;

namespace LagMenu.Mods
{
    public static class ThemeChanger
    {
        public struct Theme
        {
            public string Name;
            public Color MenuBg;
            public Color Accent;
            public Color ButtonOff;
            public Color ButtonOn;
            public Color HomeCol;
        }

        public static readonly List<Theme> Presets = new List<Theme>
        {
            new Theme { Name = "Default",       MenuBg = new Color(0.08f, 0.08f, 0.40f), Accent = new Color(0.45f, 0.00f, 0.90f), ButtonOff = new Color(0.18f, 0.00f, 0.38f), ButtonOn = new Color(0.42f, 0.00f, 0.88f), HomeCol = new Color(0.28f, 0.00f, 0.58f) },
            new Theme { Name = "Midnight",      MenuBg = new Color(0.05f, 0.05f, 0.05f), Accent = new Color(0.10f, 0.10f, 0.10f), ButtonOff = new Color(0.12f, 0.12f, 0.12f), ButtonOn = new Color(0.30f, 0.30f, 0.30f), HomeCol = new Color(0.20f, 0.20f, 0.20f) },
            new Theme { Name = "Crimson",       MenuBg = new Color(0.25f, 0.03f, 0.03f), Accent = new Color(0.80f, 0.00f, 0.00f), ButtonOff = new Color(0.35f, 0.05f, 0.05f), ButtonOn = new Color(0.75f, 0.05f, 0.05f), HomeCol = new Color(0.55f, 0.03f, 0.03f) },
            new Theme { Name = "Forest",        MenuBg = new Color(0.04f, 0.18f, 0.04f), Accent = new Color(0.00f, 0.60f, 0.10f), ButtonOff = new Color(0.06f, 0.22f, 0.06f), ButtonOn = new Color(0.05f, 0.55f, 0.10f), HomeCol = new Color(0.05f, 0.35f, 0.07f) },
            new Theme { Name = "Ocean",         MenuBg = new Color(0.04f, 0.12f, 0.25f), Accent = new Color(0.00f, 0.45f, 0.85f), ButtonOff = new Color(0.05f, 0.18f, 0.35f), ButtonOn = new Color(0.02f, 0.42f, 0.80f), HomeCol = new Color(0.04f, 0.28f, 0.55f) },
            new Theme { Name = "Sunset",        MenuBg = new Color(0.22f, 0.08f, 0.02f), Accent = new Color(0.95f, 0.40f, 0.00f), ButtonOff = new Color(0.30f, 0.10f, 0.02f), ButtonOn = new Color(0.90f, 0.38f, 0.00f), HomeCol = new Color(0.55f, 0.20f, 0.01f) },
            new Theme { Name = "Sakura",        MenuBg = new Color(0.25f, 0.05f, 0.12f), Accent = new Color(0.95f, 0.30f, 0.55f), ButtonOff = new Color(0.32f, 0.07f, 0.15f), ButtonOn = new Color(0.88f, 0.28f, 0.50f), HomeCol = new Color(0.55f, 0.12f, 0.28f) },
            new Theme { Name = "Arctic",        MenuBg = new Color(0.08f, 0.18f, 0.28f), Accent = new Color(0.55f, 0.85f, 1.00f), ButtonOff = new Color(0.10f, 0.22f, 0.35f), ButtonOn = new Color(0.45f, 0.78f, 0.95f), HomeCol = new Color(0.20f, 0.45f, 0.65f) },
            new Theme { Name = "Toxic",         MenuBg = new Color(0.05f, 0.15f, 0.02f), Accent = new Color(0.30f, 1.00f, 0.00f), ButtonOff = new Color(0.07f, 0.20f, 0.03f), ButtonOn = new Color(0.25f, 0.90f, 0.00f), HomeCol = new Color(0.12f, 0.45f, 0.02f) },
            new Theme { Name = "Gold",          MenuBg = new Color(0.18f, 0.12f, 0.00f), Accent = new Color(1.00f, 0.78f, 0.00f), ButtonOff = new Color(0.25f, 0.17f, 0.01f), ButtonOn = new Color(0.90f, 0.70f, 0.00f), HomeCol = new Color(0.50f, 0.35f, 0.01f) },
            new Theme { Name = "Void",          MenuBg = new Color(0.02f, 0.00f, 0.06f), Accent = new Color(0.40f, 0.00f, 0.60f), ButtonOff = new Color(0.05f, 0.00f, 0.10f), ButtonOn = new Color(0.35f, 0.00f, 0.55f), HomeCol = new Color(0.15f, 0.00f, 0.25f) },
            new Theme { Name = "Rust",          MenuBg = new Color(0.18f, 0.07f, 0.02f), Accent = new Color(0.75f, 0.28f, 0.05f), ButtonOff = new Color(0.25f, 0.10f, 0.03f), ButtonOn = new Color(0.68f, 0.25f, 0.04f), HomeCol = new Color(0.40f, 0.15f, 0.03f) },
            new Theme { Name = "Candy",         MenuBg = new Color(0.28f, 0.03f, 0.18f), Accent = new Color(1.00f, 0.20f, 0.60f), ButtonOff = new Color(0.35f, 0.05f, 0.22f), ButtonOn = new Color(0.92f, 0.18f, 0.55f), HomeCol = new Color(0.58f, 0.08f, 0.35f) },
            new Theme { Name = "Slate",         MenuBg = new Color(0.10f, 0.13f, 0.18f), Accent = new Color(0.40f, 0.55f, 0.75f), ButtonOff = new Color(0.14f, 0.18f, 0.24f), ButtonOn = new Color(0.35f, 0.50f, 0.70f), HomeCol = new Color(0.22f, 0.30f, 0.42f) },
            new Theme { Name = "Lava",          MenuBg = new Color(0.12f, 0.02f, 0.00f), Accent = new Color(1.00f, 0.22f, 0.00f), ButtonOff = new Color(0.20f, 0.04f, 0.00f), ButtonOn = new Color(0.95f, 0.20f, 0.00f), HomeCol = new Color(0.50f, 0.08f, 0.00f) },
            new Theme { Name = "Pastel",        MenuBg = new Color(0.22f, 0.18f, 0.28f), Accent = new Color(0.80f, 0.65f, 0.95f), ButtonOff = new Color(0.28f, 0.22f, 0.35f), ButtonOn = new Color(0.72f, 0.58f, 0.88f), HomeCol = new Color(0.48f, 0.38f, 0.60f) },
            new Theme { Name = "Military",      MenuBg = new Color(0.08f, 0.10f, 0.04f), Accent = new Color(0.35f, 0.42f, 0.15f), ButtonOff = new Color(0.12f, 0.14f, 0.06f), ButtonOn = new Color(0.30f, 0.38f, 0.12f), HomeCol = new Color(0.20f, 0.24f, 0.09f) },
            new Theme { Name = "Neon",          MenuBg = new Color(0.02f, 0.02f, 0.02f), Accent = new Color(0.00f, 1.00f, 0.80f), ButtonOff = new Color(0.05f, 0.05f, 0.05f), ButtonOn = new Color(0.00f, 0.90f, 0.72f), HomeCol = new Color(0.02f, 0.35f, 0.28f) },
            new Theme { Name = "Rose Gold",     MenuBg = new Color(0.22f, 0.10f, 0.10f), Accent = new Color(0.95f, 0.60f, 0.55f), ButtonOff = new Color(0.28f, 0.13f, 0.13f), ButtonOn = new Color(0.88f, 0.55f, 0.50f), HomeCol = new Color(0.52f, 0.25f, 0.25f) },
            new Theme { Name = "Galaxy",        MenuBg = new Color(0.05f, 0.02f, 0.15f), Accent = new Color(0.55f, 0.20f, 0.95f), ButtonOff = new Color(0.08f, 0.04f, 0.20f), ButtonOn = new Color(0.50f, 0.18f, 0.88f), HomeCol = new Color(0.22f, 0.08f, 0.45f) },
            new Theme { Name = "Ice",           MenuBg = new Color(0.12f, 0.20f, 0.30f), Accent = new Color(0.70f, 0.90f, 1.00f), ButtonOff = new Color(0.15f, 0.25f, 0.38f), ButtonOn = new Color(0.62f, 0.84f, 0.96f), HomeCol = new Color(0.28f, 0.48f, 0.65f) },

            new Theme { Name = "Blood Moon",    MenuBg = new Color(0.15f, 0.00f, 0.00f), Accent = new Color(0.90f, 0.05f, 0.05f), ButtonOff = new Color(0.22f, 0.02f, 0.02f), ButtonOn = new Color(0.85f, 0.04f, 0.04f), HomeCol = new Color(0.45f, 0.01f, 0.01f) },
            new Theme { Name = "Hot Pink",      MenuBg = new Color(0.20f, 0.02f, 0.10f), Accent = new Color(1.00f, 0.08f, 0.50f), ButtonOff = new Color(0.28f, 0.04f, 0.14f), ButtonOn = new Color(0.92f, 0.06f, 0.45f), HomeCol = new Color(0.52f, 0.04f, 0.24f) },
            new Theme { Name = "Flamingo",      MenuBg = new Color(0.28f, 0.08f, 0.14f), Accent = new Color(1.00f, 0.45f, 0.65f), ButtonOff = new Color(0.35f, 0.10f, 0.18f), ButtonOn = new Color(0.92f, 0.40f, 0.60f), HomeCol = new Color(0.58f, 0.20f, 0.32f) },
            new Theme { Name = "Cherry",        MenuBg = new Color(0.22f, 0.02f, 0.06f), Accent = new Color(0.88f, 0.10f, 0.22f), ButtonOff = new Color(0.30f, 0.04f, 0.08f), ButtonOn = new Color(0.80f, 0.08f, 0.18f), HomeCol = new Color(0.48f, 0.05f, 0.12f) },
            new Theme { Name = "Strawberry",    MenuBg = new Color(0.25f, 0.04f, 0.04f), Accent = new Color(0.98f, 0.25f, 0.28f), ButtonOff = new Color(0.32f, 0.06f, 0.06f), ButtonOn = new Color(0.90f, 0.22f, 0.25f), HomeCol = new Color(0.55f, 0.12f, 0.12f) },
            new Theme { Name = "Magenta",       MenuBg = new Color(0.18f, 0.00f, 0.18f), Accent = new Color(0.90f, 0.00f, 0.90f), ButtonOff = new Color(0.25f, 0.02f, 0.25f), ButtonOn = new Color(0.82f, 0.00f, 0.82f), HomeCol = new Color(0.44f, 0.01f, 0.44f) },
            new Theme { Name = "Bubblegum",     MenuBg = new Color(0.30f, 0.06f, 0.20f), Accent = new Color(1.00f, 0.50f, 0.80f), ButtonOff = new Color(0.38f, 0.08f, 0.25f), ButtonOn = new Color(0.95f, 0.45f, 0.75f), HomeCol = new Color(0.60f, 0.18f, 0.42f) },

            new Theme { Name = "Amber",         MenuBg = new Color(0.20f, 0.10f, 0.00f), Accent = new Color(1.00f, 0.60f, 0.00f), ButtonOff = new Color(0.28f, 0.14f, 0.01f), ButtonOn = new Color(0.92f, 0.55f, 0.00f), HomeCol = new Color(0.52f, 0.28f, 0.01f) },
            new Theme { Name = "Honey",         MenuBg = new Color(0.22f, 0.14f, 0.00f), Accent = new Color(1.00f, 0.72f, 0.10f), ButtonOff = new Color(0.30f, 0.18f, 0.01f), ButtonOn = new Color(0.92f, 0.65f, 0.08f), HomeCol = new Color(0.54f, 0.32f, 0.02f) },
            new Theme { Name = "Citrus",        MenuBg = new Color(0.15f, 0.18f, 0.00f), Accent = new Color(0.75f, 1.00f, 0.00f), ButtonOff = new Color(0.20f, 0.24f, 0.01f), ButtonOn = new Color(0.68f, 0.92f, 0.00f), HomeCol = new Color(0.35f, 0.46f, 0.01f) },
            new Theme { Name = "Solar",         MenuBg = new Color(0.18f, 0.10f, 0.00f), Accent = new Color(1.00f, 0.85f, 0.00f), ButtonOff = new Color(0.26f, 0.14f, 0.01f), ButtonOn = new Color(0.95f, 0.78f, 0.00f), HomeCol = new Color(0.50f, 0.38f, 0.01f) },
            new Theme { Name = "Pumpkin",       MenuBg = new Color(0.22f, 0.08f, 0.00f), Accent = new Color(0.90f, 0.45f, 0.05f), ButtonOff = new Color(0.30f, 0.11f, 0.01f), ButtonOn = new Color(0.82f, 0.40f, 0.04f), HomeCol = new Color(0.50f, 0.20f, 0.02f) },
            new Theme { Name = "Marigold",      MenuBg = new Color(0.20f, 0.12f, 0.00f), Accent = new Color(0.98f, 0.65f, 0.05f), ButtonOff = new Color(0.28f, 0.16f, 0.01f), ButtonOn = new Color(0.90f, 0.58f, 0.04f), HomeCol = new Color(0.50f, 0.30f, 0.02f) },

            new Theme { Name = "Slime",         MenuBg = new Color(0.06f, 0.18f, 0.02f), Accent = new Color(0.20f, 0.95f, 0.05f), ButtonOff = new Color(0.08f, 0.24f, 0.03f), ButtonOn = new Color(0.18f, 0.85f, 0.04f), HomeCol = new Color(0.10f, 0.48f, 0.04f) },
            new Theme { Name = "Jade",          MenuBg = new Color(0.02f, 0.16f, 0.10f), Accent = new Color(0.00f, 0.66f, 0.42f), ButtonOff = new Color(0.03f, 0.22f, 0.14f), ButtonOn = new Color(0.00f, 0.58f, 0.38f), HomeCol = new Color(0.02f, 0.36f, 0.22f) },
            new Theme { Name = "Emerald",       MenuBg = new Color(0.02f, 0.18f, 0.08f), Accent = new Color(0.05f, 0.80f, 0.35f), ButtonOff = new Color(0.03f, 0.24f, 0.11f), ButtonOn = new Color(0.04f, 0.72f, 0.30f), HomeCol = new Color(0.03f, 0.42f, 0.18f) },
            new Theme { Name = "Jungle",        MenuBg = new Color(0.03f, 0.14f, 0.03f), Accent = new Color(0.15f, 0.70f, 0.20f), ButtonOff = new Color(0.04f, 0.18f, 0.04f), ButtonOn = new Color(0.12f, 0.62f, 0.17f), HomeCol = new Color(0.06f, 0.32f, 0.08f) },
            new Theme { Name = "Moss",          MenuBg = new Color(0.08f, 0.12f, 0.04f), Accent = new Color(0.45f, 0.58f, 0.18f), ButtonOff = new Color(0.11f, 0.16f, 0.05f), ButtonOn = new Color(0.40f, 0.52f, 0.15f), HomeCol = new Color(0.22f, 0.28f, 0.08f) },
            new Theme { Name = "Seafoam",       MenuBg = new Color(0.04f, 0.18f, 0.14f), Accent = new Color(0.20f, 0.90f, 0.70f), ButtonOff = new Color(0.05f, 0.24f, 0.18f), ButtonOn = new Color(0.17f, 0.82f, 0.62f), HomeCol = new Color(0.08f, 0.44f, 0.33f) },
            new Theme { Name = "Poison",        MenuBg = new Color(0.04f, 0.12f, 0.00f), Accent = new Color(0.55f, 1.00f, 0.10f), ButtonOff = new Color(0.06f, 0.17f, 0.01f), ButtonOn = new Color(0.48f, 0.92f, 0.08f), HomeCol = new Color(0.15f, 0.40f, 0.03f) },
            new Theme { Name = "Camo",          MenuBg = new Color(0.10f, 0.12f, 0.05f), Accent = new Color(0.40f, 0.50f, 0.12f), ButtonOff = new Color(0.14f, 0.16f, 0.07f), ButtonOn = new Color(0.34f, 0.44f, 0.10f), HomeCol = new Color(0.22f, 0.26f, 0.08f) },

            new Theme { Name = "Cobalt",        MenuBg = new Color(0.02f, 0.06f, 0.28f), Accent = new Color(0.10f, 0.35f, 1.00f), ButtonOff = new Color(0.03f, 0.09f, 0.36f), ButtonOn = new Color(0.08f, 0.30f, 0.95f), HomeCol = new Color(0.04f, 0.17f, 0.55f) },
            new Theme { Name = "Navy",          MenuBg = new Color(0.02f, 0.04f, 0.18f), Accent = new Color(0.15f, 0.30f, 0.75f), ButtonOff = new Color(0.03f, 0.06f, 0.25f), ButtonOn = new Color(0.12f, 0.26f, 0.68f), HomeCol = new Color(0.05f, 0.12f, 0.38f) },
            new Theme { Name = "Sapphire",      MenuBg = new Color(0.03f, 0.05f, 0.22f), Accent = new Color(0.08f, 0.28f, 0.92f), ButtonOff = new Color(0.04f, 0.08f, 0.30f), ButtonOn = new Color(0.06f, 0.24f, 0.84f), HomeCol = new Color(0.04f, 0.14f, 0.48f) },
            new Theme { Name = "Sky",           MenuBg = new Color(0.05f, 0.14f, 0.28f), Accent = new Color(0.30f, 0.72f, 1.00f), ButtonOff = new Color(0.07f, 0.18f, 0.36f), ButtonOn = new Color(0.25f, 0.65f, 0.95f), HomeCol = new Color(0.12f, 0.36f, 0.62f) },
            new Theme { Name = "Dusk",          MenuBg = new Color(0.06f, 0.06f, 0.22f), Accent = new Color(0.38f, 0.42f, 0.95f), ButtonOff = new Color(0.09f, 0.09f, 0.30f), ButtonOn = new Color(0.32f, 0.36f, 0.88f), HomeCol = new Color(0.18f, 0.18f, 0.52f) },
            new Theme { Name = "Abyss",         MenuBg = new Color(0.00f, 0.04f, 0.14f), Accent = new Color(0.00f, 0.30f, 0.80f), ButtonOff = new Color(0.01f, 0.06f, 0.20f), ButtonOn = new Color(0.00f, 0.26f, 0.72f), HomeCol = new Color(0.01f, 0.14f, 0.38f) },
            new Theme { Name = "Glacier",       MenuBg = new Color(0.08f, 0.16f, 0.26f), Accent = new Color(0.50f, 0.80f, 0.98f), ButtonOff = new Color(0.11f, 0.20f, 0.32f), ButtonOn = new Color(0.44f, 0.73f, 0.92f), HomeCol = new Color(0.22f, 0.40f, 0.58f) },

            new Theme { Name = "Amethyst",      MenuBg = new Color(0.10f, 0.04f, 0.20f), Accent = new Color(0.65f, 0.30f, 1.00f), ButtonOff = new Color(0.14f, 0.06f, 0.28f), ButtonOn = new Color(0.58f, 0.26f, 0.92f), HomeCol = new Color(0.30f, 0.12f, 0.50f) },
            new Theme { Name = "Ultraviolet",   MenuBg = new Color(0.06f, 0.00f, 0.18f), Accent = new Color(0.50f, 0.00f, 1.00f), ButtonOff = new Color(0.10f, 0.01f, 0.25f), ButtonOn = new Color(0.44f, 0.00f, 0.92f), HomeCol = new Color(0.22f, 0.01f, 0.46f) },
            new Theme { Name = "Lavender",      MenuBg = new Color(0.16f, 0.10f, 0.28f), Accent = new Color(0.75f, 0.60f, 1.00f), ButtonOff = new Color(0.22f, 0.14f, 0.36f), ButtonOn = new Color(0.68f, 0.54f, 0.94f), HomeCol = new Color(0.40f, 0.28f, 0.60f) },
            new Theme { Name = "Plum",          MenuBg = new Color(0.18f, 0.04f, 0.18f), Accent = new Color(0.72f, 0.15f, 0.72f), ButtonOff = new Color(0.24f, 0.06f, 0.24f), ButtonOn = new Color(0.65f, 0.12f, 0.65f), HomeCol = new Color(0.40f, 0.08f, 0.40f) },
            new Theme { Name = "Grape",         MenuBg = new Color(0.14f, 0.04f, 0.22f), Accent = new Color(0.60f, 0.18f, 0.90f), ButtonOff = new Color(0.19f, 0.06f, 0.30f), ButtonOn = new Color(0.54f, 0.15f, 0.82f), HomeCol = new Color(0.30f, 0.08f, 0.48f) },
            new Theme { Name = "Plasma",        MenuBg = new Color(0.08f, 0.02f, 0.20f), Accent = new Color(0.80f, 0.10f, 1.00f), ButtonOff = new Color(0.12f, 0.03f, 0.28f), ButtonOn = new Color(0.72f, 0.08f, 0.92f), HomeCol = new Color(0.32f, 0.04f, 0.48f) },
            new Theme { Name = "Twilight",      MenuBg = new Color(0.08f, 0.04f, 0.18f), Accent = new Color(0.55f, 0.35f, 0.85f), ButtonOff = new Color(0.12f, 0.06f, 0.24f), ButtonOn = new Color(0.48f, 0.30f, 0.78f), HomeCol = new Color(0.25f, 0.14f, 0.42f) },

            new Theme { Name = "Teal",          MenuBg = new Color(0.02f, 0.16f, 0.16f), Accent = new Color(0.00f, 0.75f, 0.75f), ButtonOff = new Color(0.03f, 0.22f, 0.22f), ButtonOn = new Color(0.00f, 0.68f, 0.68f), HomeCol = new Color(0.02f, 0.38f, 0.38f) },
            new Theme { Name = "Aqua",          MenuBg = new Color(0.02f, 0.14f, 0.20f), Accent = new Color(0.00f, 0.80f, 1.00f), ButtonOff = new Color(0.03f, 0.19f, 0.27f), ButtonOn = new Color(0.00f, 0.72f, 0.92f), HomeCol = new Color(0.02f, 0.38f, 0.50f) },
            new Theme { Name = "Cyber",         MenuBg = new Color(0.00f, 0.10f, 0.12f), Accent = new Color(0.00f, 0.95f, 0.88f), ButtonOff = new Color(0.01f, 0.14f, 0.17f), ButtonOn = new Color(0.00f, 0.86f, 0.80f), HomeCol = new Color(0.01f, 0.38f, 0.42f) },
            new Theme { Name = "Matrix",        MenuBg = new Color(0.00f, 0.08f, 0.02f), Accent = new Color(0.00f, 1.00f, 0.25f), ButtonOff = new Color(0.01f, 0.12f, 0.03f), ButtonOn = new Color(0.00f, 0.90f, 0.22f), HomeCol = new Color(0.01f, 0.35f, 0.08f) },
            new Theme { Name = "Deep Sea",      MenuBg = new Color(0.00f, 0.08f, 0.16f), Accent = new Color(0.00f, 0.55f, 0.80f), ButtonOff = new Color(0.01f, 0.11f, 0.22f), ButtonOn = new Color(0.00f, 0.48f, 0.72f), HomeCol = new Color(0.01f, 0.25f, 0.42f) },

            new Theme { Name = "Ash",           MenuBg = new Color(0.12f, 0.12f, 0.14f), Accent = new Color(0.65f, 0.65f, 0.70f), ButtonOff = new Color(0.17f, 0.17f, 0.19f), ButtonOn = new Color(0.58f, 0.58f, 0.63f), HomeCol = new Color(0.32f, 0.32f, 0.35f) },
            new Theme { Name = "Obsidian",      MenuBg = new Color(0.04f, 0.04f, 0.05f), Accent = new Color(0.35f, 0.35f, 0.38f), ButtonOff = new Color(0.07f, 0.07f, 0.08f), ButtonOn = new Color(0.28f, 0.28f, 0.30f), HomeCol = new Color(0.15f, 0.15f, 0.16f) },
            new Theme { Name = "Silver",        MenuBg = new Color(0.14f, 0.16f, 0.18f), Accent = new Color(0.75f, 0.80f, 0.85f), ButtonOff = new Color(0.19f, 0.21f, 0.24f), ButtonOn = new Color(0.68f, 0.73f, 0.78f), HomeCol = new Color(0.36f, 0.40f, 0.44f) },
            new Theme { Name = "Phantom",       MenuBg = new Color(0.06f, 0.06f, 0.08f), Accent = new Color(0.50f, 0.52f, 0.55f), ButtonOff = new Color(0.09f, 0.09f, 0.11f), ButtonOn = new Color(0.44f, 0.46f, 0.50f), HomeCol = new Color(0.22f, 0.22f, 0.25f) },
            new Theme { Name = "Stealth",       MenuBg = new Color(0.05f, 0.05f, 0.06f), Accent = new Color(0.22f, 0.22f, 0.24f), ButtonOff = new Color(0.08f, 0.08f, 0.09f), ButtonOn = new Color(0.18f, 0.18f, 0.20f), HomeCol = new Color(0.12f, 0.12f, 0.13f) },

            new Theme { Name = "Sand",          MenuBg = new Color(0.18f, 0.14f, 0.08f), Accent = new Color(0.80f, 0.68f, 0.42f), ButtonOff = new Color(0.24f, 0.19f, 0.11f), ButtonOn = new Color(0.72f, 0.61f, 0.36f), HomeCol = new Color(0.44f, 0.34f, 0.18f) },
            new Theme { Name = "Desert",        MenuBg = new Color(0.20f, 0.12f, 0.04f), Accent = new Color(0.90f, 0.62f, 0.25f), ButtonOff = new Color(0.27f, 0.16f, 0.05f), ButtonOn = new Color(0.82f, 0.55f, 0.20f), HomeCol = new Color(0.48f, 0.28f, 0.10f) },
            new Theme { Name = "Terracotta",    MenuBg = new Color(0.24f, 0.10f, 0.06f), Accent = new Color(0.85f, 0.38f, 0.20f), ButtonOff = new Color(0.32f, 0.14f, 0.08f), ButtonOn = new Color(0.76f, 0.33f, 0.16f), HomeCol = new Color(0.52f, 0.20f, 0.10f) },
            new Theme { Name = "Coffee",        MenuBg = new Color(0.14f, 0.08f, 0.04f), Accent = new Color(0.60f, 0.38f, 0.18f), ButtonOff = new Color(0.20f, 0.11f, 0.05f), ButtonOn = new Color(0.54f, 0.33f, 0.14f), HomeCol = new Color(0.32f, 0.18f, 0.08f) },
            new Theme { Name = "Walnut",        MenuBg = new Color(0.12f, 0.07f, 0.03f), Accent = new Color(0.52f, 0.30f, 0.12f), ButtonOff = new Color(0.17f, 0.10f, 0.04f), ButtonOn = new Color(0.46f, 0.26f, 0.10f), HomeCol = new Color(0.26f, 0.14f, 0.06f) },
            new Theme { Name = "Sahara",        MenuBg = new Color(0.22f, 0.16f, 0.05f), Accent = new Color(0.95f, 0.75f, 0.30f), ButtonOff = new Color(0.29f, 0.21f, 0.07f), ButtonOn = new Color(0.88f, 0.68f, 0.25f), HomeCol = new Color(0.52f, 0.36f, 0.12f) },

            new Theme { Name = "Inferno",       MenuBg = new Color(0.10f, 0.00f, 0.00f), Accent = new Color(1.00f, 0.35f, 0.00f), ButtonOff = new Color(0.17f, 0.01f, 0.00f), ButtonOn = new Color(0.95f, 0.30f, 0.00f), HomeCol = new Color(0.44f, 0.05f, 0.00f) },
            new Theme { Name = "Nuclear",       MenuBg = new Color(0.06f, 0.12f, 0.00f), Accent = new Color(0.65f, 1.00f, 0.00f), ButtonOff = new Color(0.09f, 0.17f, 0.01f), ButtonOn = new Color(0.58f, 0.92f, 0.00f), HomeCol = new Color(0.20f, 0.42f, 0.01f) },
            new Theme { Name = "Aurora",        MenuBg = new Color(0.02f, 0.10f, 0.14f), Accent = new Color(0.20f, 1.00f, 0.65f), ButtonOff = new Color(0.03f, 0.14f, 0.19f), ButtonOn = new Color(0.17f, 0.92f, 0.58f), HomeCol = new Color(0.06f, 0.42f, 0.32f) },
            new Theme { Name = "Prism",         MenuBg = new Color(0.05f, 0.05f, 0.10f), Accent = new Color(0.85f, 0.50f, 1.00f), ButtonOff = new Color(0.08f, 0.08f, 0.15f), ButtonOn = new Color(0.78f, 0.44f, 0.94f), HomeCol = new Color(0.28f, 0.20f, 0.45f) },
            new Theme { Name = "Heatwave",      MenuBg = new Color(0.18f, 0.04f, 0.00f), Accent = new Color(1.00f, 0.55f, 0.10f), ButtonOff = new Color(0.25f, 0.06f, 0.01f), ButtonOn = new Color(0.95f, 0.50f, 0.08f), HomeCol = new Color(0.52f, 0.18f, 0.02f) },
            new Theme { Name = "Bioluminescent",MenuBg = new Color(0.00f, 0.06f, 0.12f), Accent = new Color(0.10f, 0.95f, 0.80f), ButtonOff = new Color(0.01f, 0.09f, 0.17f), ButtonOn = new Color(0.08f, 0.86f, 0.72f), HomeCol = new Color(0.02f, 0.35f, 0.38f) },
            new Theme { Name = "Ethereal",      MenuBg = new Color(0.10f, 0.08f, 0.18f), Accent = new Color(0.80f, 0.70f, 1.00f), ButtonOff = new Color(0.14f, 0.11f, 0.24f), ButtonOn = new Color(0.72f, 0.63f, 0.94f), HomeCol = new Color(0.36f, 0.28f, 0.55f) },
            new Theme { Name = "Retrowave",     MenuBg = new Color(0.08f, 0.02f, 0.14f), Accent = new Color(1.00f, 0.05f, 0.65f), ButtonOff = new Color(0.13f, 0.03f, 0.20f), ButtonOn = new Color(0.92f, 0.04f, 0.58f), HomeCol = new Color(0.35f, 0.04f, 0.42f) },
            new Theme { Name = "Vaporwave",     MenuBg = new Color(0.14f, 0.04f, 0.22f), Accent = new Color(1.00f, 0.45f, 0.85f), ButtonOff = new Color(0.20f, 0.06f, 0.30f), ButtonOn = new Color(0.92f, 0.40f, 0.78f), HomeCol = new Color(0.45f, 0.14f, 0.55f) },
            new Theme { Name = "Holographic",   MenuBg = new Color(0.08f, 0.08f, 0.14f), Accent = new Color(0.60f, 0.90f, 1.00f), ButtonOff = new Color(0.12f, 0.12f, 0.20f), ButtonOn = new Color(0.54f, 0.82f, 0.96f), HomeCol = new Color(0.28f, 0.38f, 0.52f) },
            new Theme { Name = "Glitch",        MenuBg = new Color(0.02f, 0.02f, 0.06f), Accent = new Color(0.00f, 1.00f, 0.50f), ButtonOff = new Color(0.04f, 0.04f, 0.10f), ButtonOn = new Color(0.00f, 0.90f, 0.44f), HomeCol = new Color(0.02f, 0.30f, 0.18f) },
            new Theme { Name = "Stardust",      MenuBg = new Color(0.06f, 0.04f, 0.14f), Accent = new Color(0.90f, 0.85f, 1.00f), ButtonOff = new Color(0.09f, 0.06f, 0.20f), ButtonOn = new Color(0.82f, 0.78f, 0.96f), HomeCol = new Color(0.30f, 0.22f, 0.50f) },
            new Theme { Name = "Nebula",        MenuBg = new Color(0.04f, 0.02f, 0.12f), Accent = new Color(0.70f, 0.30f, 0.90f), ButtonOff = new Color(0.07f, 0.04f, 0.18f), ButtonOn = new Color(0.63f, 0.26f, 0.82f), HomeCol = new Color(0.22f, 0.10f, 0.40f) },
            new Theme { Name = "Supernova",     MenuBg = new Color(0.15f, 0.04f, 0.00f), Accent = new Color(1.00f, 0.70f, 0.10f), ButtonOff = new Color(0.22f, 0.06f, 0.01f), ButtonOn = new Color(0.95f, 0.64f, 0.08f), HomeCol = new Color(0.50f, 0.20f, 0.02f) },
            new Theme { Name = "Black Hole",    MenuBg = new Color(0.00f, 0.00f, 0.02f), Accent = new Color(0.25f, 0.05f, 0.40f), ButtonOff = new Color(0.02f, 0.00f, 0.04f), ButtonOn = new Color(0.20f, 0.04f, 0.35f), HomeCol = new Color(0.08f, 0.01f, 0.14f) },

            new Theme { Name = "Spring",        MenuBg = new Color(0.12f, 0.18f, 0.08f), Accent = new Color(0.55f, 0.90f, 0.30f), ButtonOff = new Color(0.17f, 0.24f, 0.11f), ButtonOn = new Color(0.48f, 0.82f, 0.25f), HomeCol = new Color(0.28f, 0.46f, 0.16f) },
            new Theme { Name = "Summer",        MenuBg = new Color(0.20f, 0.10f, 0.00f), Accent = new Color(1.00f, 0.80f, 0.10f), ButtonOff = new Color(0.27f, 0.14f, 0.01f), ButtonOn = new Color(0.94f, 0.73f, 0.08f), HomeCol = new Color(0.52f, 0.32f, 0.02f) },
            new Theme { Name = "Autumn",        MenuBg = new Color(0.20f, 0.08f, 0.02f), Accent = new Color(0.85f, 0.42f, 0.08f), ButtonOff = new Color(0.27f, 0.11f, 0.03f), ButtonOn = new Color(0.78f, 0.37f, 0.06f), HomeCol = new Color(0.48f, 0.20f, 0.04f) },
            new Theme { Name = "Winter",        MenuBg = new Color(0.08f, 0.12f, 0.20f), Accent = new Color(0.65f, 0.80f, 1.00f), ButtonOff = new Color(0.11f, 0.16f, 0.27f), ButtonOn = new Color(0.58f, 0.73f, 0.95f), HomeCol = new Color(0.24f, 0.34f, 0.52f) },
            new Theme { Name = "Blizzard",      MenuBg = new Color(0.10f, 0.14f, 0.22f), Accent = new Color(0.85f, 0.92f, 1.00f), ButtonOff = new Color(0.14f, 0.18f, 0.29f), ButtonOn = new Color(0.78f, 0.86f, 0.98f), HomeCol = new Color(0.28f, 0.38f, 0.55f) },
            new Theme { Name = "Monsoon",       MenuBg = new Color(0.05f, 0.10f, 0.18f), Accent = new Color(0.25f, 0.55f, 0.88f), ButtonOff = new Color(0.07f, 0.14f, 0.24f), ButtonOn = new Color(0.20f, 0.48f, 0.80f), HomeCol = new Color(0.11f, 0.26f, 0.44f) },

            new Theme { Name = "Chrome",        MenuBg = new Color(0.12f, 0.14f, 0.16f), Accent = new Color(0.82f, 0.86f, 0.90f), ButtonOff = new Color(0.17f, 0.19f, 0.22f), ButtonOn = new Color(0.75f, 0.79f, 0.84f), HomeCol = new Color(0.36f, 0.40f, 0.45f) },
            new Theme { Name = "Bronze",        MenuBg = new Color(0.16f, 0.09f, 0.02f), Accent = new Color(0.70f, 0.44f, 0.18f), ButtonOff = new Color(0.22f, 0.12f, 0.03f), ButtonOn = new Color(0.63f, 0.39f, 0.14f), HomeCol = new Color(0.38f, 0.20f, 0.06f) },
            new Theme { Name = "Titanium",      MenuBg = new Color(0.10f, 0.11f, 0.13f), Accent = new Color(0.55f, 0.60f, 0.65f), ButtonOff = new Color(0.14f, 0.16f, 0.18f), ButtonOn = new Color(0.48f, 0.53f, 0.58f), HomeCol = new Color(0.26f, 0.28f, 0.32f) },
            new Theme { Name = "Copper",        MenuBg = new Color(0.18f, 0.08f, 0.04f), Accent = new Color(0.78f, 0.42f, 0.22f), ButtonOff = new Color(0.24f, 0.11f, 0.05f), ButtonOn = new Color(0.70f, 0.37f, 0.18f), HomeCol = new Color(0.42f, 0.18f, 0.08f) },
            new Theme { Name = "Platinum",      MenuBg = new Color(0.14f, 0.15f, 0.17f), Accent = new Color(0.88f, 0.90f, 0.92f), ButtonOff = new Color(0.19f, 0.20f, 0.22f), ButtonOn = new Color(0.80f, 0.83f, 0.86f), HomeCol = new Color(0.40f, 0.42f, 0.46f) },

            new Theme { Name = "Destiny",           MenuBg = new Color(0.05f, 0.06f, 0.12f), Accent = new Color(0.18f, 0.91f, 0.89f), ButtonOff = new Color(0.08f, 0.10f, 0.20f), ButtonOn = new Color(0.14f, 0.82f, 0.80f), HomeCol = new Color(0.10f, 0.40f, 0.45f) },
            new Theme { Name = "Void Walker",       MenuBg = new Color(0.06f, 0.00f, 0.14f), Accent = new Color(0.55f, 0.10f, 0.95f), ButtonOff = new Color(0.10f, 0.01f, 0.22f), ButtonOn = new Color(0.48f, 0.08f, 0.88f), HomeCol = new Color(0.24f, 0.03f, 0.45f) },
            new Theme { Name = "Solar Warlock",     MenuBg = new Color(0.18f, 0.05f, 0.00f), Accent = new Color(1.00f, 0.50f, 0.05f), ButtonOff = new Color(0.26f, 0.07f, 0.01f), ButtonOn = new Color(0.95f, 0.44f, 0.04f), HomeCol = new Color(0.52f, 0.18f, 0.01f) },
            new Theme { Name = "Arc Titan",         MenuBg = new Color(0.02f, 0.06f, 0.18f), Accent = new Color(0.25f, 0.65f, 1.00f), ButtonOff = new Color(0.04f, 0.09f, 0.26f), ButtonOn = new Color(0.20f, 0.58f, 0.95f), HomeCol = new Color(0.08f, 0.25f, 0.52f) },
            new Theme { Name = "Strand",            MenuBg = new Color(0.02f, 0.12f, 0.06f), Accent = new Color(0.15f, 0.95f, 0.45f), ButtonOff = new Color(0.03f, 0.17f, 0.09f), ButtonOn = new Color(0.12f, 0.86f, 0.40f), HomeCol = new Color(0.05f, 0.42f, 0.20f) },
            new Theme { Name = "Stasis",            MenuBg = new Color(0.04f, 0.10f, 0.20f), Accent = new Color(0.45f, 0.78f, 1.00f), ButtonOff = new Color(0.06f, 0.14f, 0.28f), ButtonOn = new Color(0.38f, 0.70f, 0.95f), HomeCol = new Color(0.14f, 0.34f, 0.55f) },
            new Theme { Name = "The Traveler",      MenuBg = new Color(0.14f, 0.14f, 0.18f), Accent = new Color(0.92f, 0.92f, 1.00f), ButtonOff = new Color(0.19f, 0.19f, 0.24f), ButtonOn = new Color(0.84f, 0.84f, 0.96f), HomeCol = new Color(0.40f, 0.40f, 0.52f) },
            new Theme { Name = "Darkness",          MenuBg = new Color(0.03f, 0.02f, 0.06f), Accent = new Color(0.50f, 0.20f, 0.70f), ButtonOff = new Color(0.06f, 0.04f, 0.10f), ButtonOn = new Color(0.44f, 0.16f, 0.62f), HomeCol = new Color(0.18f, 0.08f, 0.28f) },

            new Theme { Name = "Halo",              MenuBg = new Color(0.04f, 0.10f, 0.04f), Accent = new Color(0.20f, 0.85f, 0.20f), ButtonOff = new Color(0.06f, 0.14f, 0.06f), ButtonOn = new Color(0.16f, 0.76f, 0.16f), HomeCol = new Color(0.08f, 0.36f, 0.08f) },
            new Theme { Name = "Master Chief",      MenuBg = new Color(0.06f, 0.12f, 0.05f), Accent = new Color(0.35f, 0.72f, 0.22f), ButtonOff = new Color(0.09f, 0.16f, 0.07f), ButtonOn = new Color(0.30f, 0.64f, 0.18f), HomeCol = new Color(0.15f, 0.32f, 0.10f) },
            new Theme { Name = "UNSC",              MenuBg = new Color(0.08f, 0.10f, 0.14f), Accent = new Color(0.40f, 0.55f, 0.80f), ButtonOff = new Color(0.12f, 0.15f, 0.20f), ButtonOn = new Color(0.34f, 0.48f, 0.72f), HomeCol = new Color(0.22f, 0.28f, 0.42f) },
            new Theme { Name = "Covenant",          MenuBg = new Color(0.10f, 0.02f, 0.16f), Accent = new Color(0.65f, 0.18f, 0.92f), ButtonOff = new Color(0.15f, 0.03f, 0.22f), ButtonOn = new Color(0.58f, 0.14f, 0.84f), HomeCol = new Color(0.30f, 0.06f, 0.44f) },
            new Theme { Name = "Flood",             MenuBg = new Color(0.04f, 0.12f, 0.04f), Accent = new Color(0.28f, 0.88f, 0.28f), ButtonOff = new Color(0.06f, 0.17f, 0.06f), ButtonOn = new Color(0.22f, 0.78f, 0.22f), HomeCol = new Color(0.10f, 0.38f, 0.10f) },
            new Theme { Name = "Forerunner",        MenuBg = new Color(0.10f, 0.08f, 0.02f), Accent = new Color(0.90f, 0.75f, 0.20f), ButtonOff = new Color(0.15f, 0.12f, 0.03f), ButtonOn = new Color(0.82f, 0.68f, 0.16f), HomeCol = new Color(0.38f, 0.28f, 0.06f) },

            new Theme { Name = "Warzone",           MenuBg = new Color(0.06f, 0.07f, 0.05f), Accent = new Color(0.65f, 0.72f, 0.35f), ButtonOff = new Color(0.09f, 0.10f, 0.07f), ButtonOn = new Color(0.58f, 0.65f, 0.28f), HomeCol = new Color(0.22f, 0.24f, 0.12f) },
            new Theme { Name = "Ghost",             MenuBg = new Color(0.06f, 0.06f, 0.07f), Accent = new Color(0.70f, 0.72f, 0.75f), ButtonOff = new Color(0.09f, 0.09f, 0.10f), ButtonOn = new Color(0.62f, 0.65f, 0.68f), HomeCol = new Color(0.24f, 0.24f, 0.26f) },
            new Theme { Name = "Nuke Town",         MenuBg = new Color(0.12f, 0.06f, 0.00f), Accent = new Color(0.95f, 0.55f, 0.10f), ButtonOff = new Color(0.18f, 0.09f, 0.01f), ButtonOn = new Color(0.86f, 0.48f, 0.08f), HomeCol = new Color(0.42f, 0.20f, 0.02f) },
            new Theme { Name = "Shadow Ops",        MenuBg = new Color(0.04f, 0.04f, 0.04f), Accent = new Color(0.85f, 0.15f, 0.15f), ButtonOff = new Color(0.07f, 0.07f, 0.07f), ButtonOn = new Color(0.76f, 0.12f, 0.12f), HomeCol = new Color(0.20f, 0.06f, 0.06f) },

            new Theme { Name = "Minecraft",         MenuBg = new Color(0.22f, 0.14f, 0.08f), Accent = new Color(0.40f, 0.72f, 0.18f), ButtonOff = new Color(0.30f, 0.19f, 0.10f), ButtonOn = new Color(0.34f, 0.64f, 0.14f), HomeCol = new Color(0.30f, 0.44f, 0.12f) },
            new Theme { Name = "Creeper",           MenuBg = new Color(0.04f, 0.16f, 0.04f), Accent = new Color(0.18f, 0.80f, 0.18f), ButtonOff = new Color(0.06f, 0.22f, 0.06f), ButtonOn = new Color(0.14f, 0.72f, 0.14f), HomeCol = new Color(0.08f, 0.40f, 0.08f) },
            new Theme { Name = "Nether",            MenuBg = new Color(0.20f, 0.04f, 0.00f), Accent = new Color(0.95f, 0.35f, 0.00f), ButtonOff = new Color(0.28f, 0.06f, 0.01f), ButtonOn = new Color(0.86f, 0.30f, 0.00f), HomeCol = new Color(0.50f, 0.12f, 0.01f) },
            new Theme { Name = "End",               MenuBg = new Color(0.06f, 0.04f, 0.12f), Accent = new Color(0.78f, 0.55f, 0.95f), ButtonOff = new Color(0.09f, 0.06f, 0.17f), ButtonOn = new Color(0.70f, 0.48f, 0.88f), HomeCol = new Color(0.28f, 0.18f, 0.42f) },
            new Theme { Name = "Diamond",           MenuBg = new Color(0.04f, 0.18f, 0.22f), Accent = new Color(0.20f, 0.88f, 0.95f), ButtonOff = new Color(0.06f, 0.24f, 0.30f), ButtonOn = new Color(0.16f, 0.80f, 0.88f), HomeCol = new Color(0.08f, 0.44f, 0.52f) },
            new Theme { Name = "Netherite",         MenuBg = new Color(0.10f, 0.08f, 0.09f), Accent = new Color(0.55f, 0.42f, 0.48f), ButtonOff = new Color(0.15f, 0.12f, 0.13f), ButtonOn = new Color(0.48f, 0.36f, 0.42f), HomeCol = new Color(0.28f, 0.20f, 0.23f) },

            new Theme { Name = "GTA",               MenuBg = new Color(0.00f, 0.08f, 0.00f), Accent = new Color(0.15f, 0.90f, 0.15f), ButtonOff = new Color(0.01f, 0.12f, 0.01f), ButtonOn = new Color(0.12f, 0.82f, 0.12f), HomeCol = new Color(0.03f, 0.38f, 0.03f) },
            new Theme { Name = "Los Santos",        MenuBg = new Color(0.12f, 0.10f, 0.04f), Accent = new Color(0.95f, 0.78f, 0.20f), ButtonOff = new Color(0.17f, 0.14f, 0.05f), ButtonOn = new Color(0.86f, 0.70f, 0.16f), HomeCol = new Color(0.40f, 0.30f, 0.08f) },
            new Theme { Name = "Wanted",            MenuBg = new Color(0.18f, 0.02f, 0.00f), Accent = new Color(0.95f, 0.18f, 0.05f), ButtonOff = new Color(0.25f, 0.04f, 0.01f), ButtonOn = new Color(0.88f, 0.14f, 0.04f), HomeCol = new Color(0.48f, 0.06f, 0.01f) },
            new Theme { Name = "Vice City",         MenuBg = new Color(0.18f, 0.04f, 0.14f), Accent = new Color(0.95f, 0.30f, 0.75f), ButtonOff = new Color(0.25f, 0.06f, 0.20f), ButtonOn = new Color(0.86f, 0.24f, 0.68f), HomeCol = new Color(0.48f, 0.10f, 0.36f) },

            new Theme { Name = "Among Us",          MenuBg = new Color(0.06f, 0.06f, 0.20f), Accent = new Color(0.80f, 0.12f, 0.12f), ButtonOff = new Color(0.09f, 0.09f, 0.28f), ButtonOn = new Color(0.72f, 0.10f, 0.10f), HomeCol = new Color(0.20f, 0.10f, 0.30f) },
            new Theme { Name = "Impostor",          MenuBg = new Color(0.08f, 0.00f, 0.00f), Accent = new Color(0.90f, 0.05f, 0.05f), ButtonOff = new Color(0.14f, 0.01f, 0.01f), ButtonOn = new Color(0.82f, 0.04f, 0.04f), HomeCol = new Color(0.35f, 0.02f, 0.02f) },

            new Theme { Name = "Fortnite",          MenuBg = new Color(0.04f, 0.10f, 0.20f), Accent = new Color(0.35f, 0.78f, 1.00f), ButtonOff = new Color(0.06f, 0.14f, 0.28f), ButtonOn = new Color(0.28f, 0.70f, 0.95f), HomeCol = new Color(0.10f, 0.32f, 0.55f) },
            new Theme { Name = "Tilted",            MenuBg = new Color(0.10f, 0.06f, 0.00f), Accent = new Color(1.00f, 0.65f, 0.00f), ButtonOff = new Color(0.16f, 0.10f, 0.01f), ButtonOn = new Color(0.92f, 0.58f, 0.00f), HomeCol = new Color(0.40f, 0.24f, 0.02f) },
            new Theme { Name = "Peely",             MenuBg = new Color(0.20f, 0.14f, 0.00f), Accent = new Color(0.98f, 0.82f, 0.10f), ButtonOff = new Color(0.27f, 0.19f, 0.01f), ButtonOn = new Color(0.90f, 0.74f, 0.08f), HomeCol = new Color(0.50f, 0.36f, 0.02f) },

            new Theme { Name = "Pokemon",           MenuBg = new Color(0.18f, 0.02f, 0.02f), Accent = new Color(0.95f, 0.85f, 0.10f), ButtonOff = new Color(0.25f, 0.04f, 0.04f), ButtonOn = new Color(0.86f, 0.76f, 0.08f), HomeCol = new Color(0.50f, 0.10f, 0.10f) },
            new Theme { Name = "Charizard",         MenuBg = new Color(0.22f, 0.06f, 0.00f), Accent = new Color(1.00f, 0.45f, 0.05f), ButtonOff = new Color(0.30f, 0.08f, 0.01f), ButtonOn = new Color(0.92f, 0.40f, 0.04f), HomeCol = new Color(0.55f, 0.16f, 0.02f) },
            new Theme { Name = "Mewtwo",            MenuBg = new Color(0.16f, 0.06f, 0.20f), Accent = new Color(0.80f, 0.55f, 0.95f), ButtonOff = new Color(0.22f, 0.08f, 0.28f), ButtonOn = new Color(0.72f, 0.48f, 0.88f), HomeCol = new Color(0.42f, 0.18f, 0.50f) },
            new Theme { Name = "Umbreon",           MenuBg = new Color(0.02f, 0.02f, 0.06f), Accent = new Color(0.95f, 0.78f, 0.05f), ButtonOff = new Color(0.04f, 0.04f, 0.10f), ButtonOn = new Color(0.86f, 0.70f, 0.04f), HomeCol = new Color(0.12f, 0.10f, 0.18f) },
            new Theme { Name = "Sylveon",           MenuBg = new Color(0.22f, 0.08f, 0.14f), Accent = new Color(0.98f, 0.65f, 0.80f), ButtonOff = new Color(0.30f, 0.11f, 0.19f), ButtonOn = new Color(0.90f, 0.58f, 0.72f), HomeCol = new Color(0.55f, 0.22f, 0.36f) },
            new Theme { Name = "Gengar",            MenuBg = new Color(0.14f, 0.05f, 0.20f), Accent = new Color(0.62f, 0.30f, 0.85f), ButtonOff = new Color(0.20f, 0.07f, 0.28f), ButtonOn = new Color(0.55f, 0.25f, 0.76f), HomeCol = new Color(0.32f, 0.12f, 0.44f) },

            new Theme { Name = "Jedi",              MenuBg = new Color(0.02f, 0.06f, 0.02f), Accent = new Color(0.20f, 0.95f, 0.20f), ButtonOff = new Color(0.03f, 0.09f, 0.03f), ButtonOn = new Color(0.16f, 0.86f, 0.16f), HomeCol = new Color(0.06f, 0.36f, 0.06f) },
            new Theme { Name = "Sith",              MenuBg = new Color(0.10f, 0.00f, 0.00f), Accent = new Color(0.95f, 0.05f, 0.05f), ButtonOff = new Color(0.16f, 0.01f, 0.01f), ButtonOn = new Color(0.86f, 0.04f, 0.04f), HomeCol = new Color(0.38f, 0.02f, 0.02f) },
            new Theme { Name = "Mandalorian",       MenuBg = new Color(0.10f, 0.10f, 0.12f), Accent = new Color(0.65f, 0.68f, 0.72f), ButtonOff = new Color(0.14f, 0.14f, 0.17f), ButtonOn = new Color(0.58f, 0.61f, 0.65f), HomeCol = new Color(0.28f, 0.28f, 0.32f) },
            new Theme { Name = "Empire",            MenuBg = new Color(0.04f, 0.04f, 0.05f), Accent = new Color(0.75f, 0.75f, 0.78f), ButtonOff = new Color(0.07f, 0.07f, 0.08f), ButtonOn = new Color(0.68f, 0.68f, 0.70f), HomeCol = new Color(0.18f, 0.18f, 0.20f) },
            new Theme { Name = "Rebellion",         MenuBg = new Color(0.16f, 0.04f, 0.00f), Accent = new Color(0.90f, 0.32f, 0.08f), ButtonOff = new Color(0.22f, 0.06f, 0.01f), ButtonOn = new Color(0.82f, 0.28f, 0.06f), HomeCol = new Color(0.44f, 0.12f, 0.02f) },

            new Theme { Name = "Iron Man",          MenuBg = new Color(0.20f, 0.02f, 0.02f), Accent = new Color(0.98f, 0.72f, 0.08f), ButtonOff = new Color(0.28f, 0.04f, 0.04f), ButtonOn = new Color(0.90f, 0.64f, 0.06f), HomeCol = new Color(0.52f, 0.10f, 0.04f) },
            new Theme { Name = "Spider-Man",        MenuBg = new Color(0.16f, 0.02f, 0.02f), Accent = new Color(0.20f, 0.35f, 0.95f), ButtonOff = new Color(0.22f, 0.04f, 0.04f), ButtonOn = new Color(0.16f, 0.28f, 0.88f), HomeCol = new Color(0.38f, 0.06f, 0.06f) },
            new Theme { Name = "Batman",            MenuBg = new Color(0.04f, 0.04f, 0.04f), Accent = new Color(0.90f, 0.72f, 0.08f), ButtonOff = new Color(0.07f, 0.07f, 0.07f), ButtonOn = new Color(0.82f, 0.64f, 0.06f), HomeCol = new Color(0.16f, 0.14f, 0.04f) },
            new Theme { Name = "Hulk",              MenuBg = new Color(0.04f, 0.18f, 0.04f), Accent = new Color(0.25f, 0.95f, 0.10f), ButtonOff = new Color(0.06f, 0.24f, 0.06f), ButtonOn = new Color(0.20f, 0.86f, 0.08f), HomeCol = new Color(0.08f, 0.44f, 0.08f) },
            new Theme { Name = "Thanos",            MenuBg = new Color(0.14f, 0.05f, 0.18f), Accent = new Color(0.70f, 0.35f, 0.85f), ButtonOff = new Color(0.20f, 0.07f, 0.25f), ButtonOn = new Color(0.62f, 0.30f, 0.76f), HomeCol = new Color(0.38f, 0.14f, 0.46f) },
            new Theme { Name = "Infinity",          MenuBg = new Color(0.06f, 0.04f, 0.14f), Accent = new Color(0.95f, 0.72f, 0.10f), ButtonOff = new Color(0.10f, 0.06f, 0.20f), ButtonOn = new Color(0.86f, 0.64f, 0.08f), HomeCol = new Color(0.28f, 0.16f, 0.44f) },

            new Theme { Name = "Cyberpunk",         MenuBg = new Color(0.04f, 0.04f, 0.10f), Accent = new Color(0.95f, 0.85f, 0.00f), ButtonOff = new Color(0.07f, 0.07f, 0.15f), ButtonOn = new Color(0.86f, 0.76f, 0.00f), HomeCol = new Color(0.20f, 0.14f, 0.35f) },
            new Theme { Name = "Night City",        MenuBg = new Color(0.05f, 0.02f, 0.10f), Accent = new Color(1.00f, 0.20f, 0.55f), ButtonOff = new Color(0.08f, 0.04f, 0.15f), ButtonOn = new Color(0.92f, 0.16f, 0.48f), HomeCol = new Color(0.24f, 0.06f, 0.30f) },
            new Theme { Name = "Samurai",           MenuBg = new Color(0.14f, 0.02f, 0.02f), Accent = new Color(0.95f, 0.22f, 0.10f), ButtonOff = new Color(0.20f, 0.04f, 0.04f), ButtonOn = new Color(0.86f, 0.18f, 0.08f), HomeCol = new Color(0.44f, 0.06f, 0.04f) },
            new Theme { Name = "Netrunner",         MenuBg = new Color(0.00f, 0.08f, 0.08f), Accent = new Color(0.00f, 0.95f, 0.75f), ButtonOff = new Color(0.01f, 0.12f, 0.12f), ButtonOn = new Color(0.00f, 0.86f, 0.68f), HomeCol = new Color(0.02f, 0.38f, 0.35f) },

            new Theme { Name = "Apex",              MenuBg = new Color(0.10f, 0.04f, 0.00f), Accent = new Color(0.95f, 0.35f, 0.05f), ButtonOff = new Color(0.15f, 0.06f, 0.01f), ButtonOn = new Color(0.86f, 0.30f, 0.04f), HomeCol = new Color(0.38f, 0.12f, 0.02f) },
            new Theme { Name = "Wraith",            MenuBg = new Color(0.08f, 0.02f, 0.14f), Accent = new Color(0.65f, 0.40f, 0.95f), ButtonOff = new Color(0.12f, 0.04f, 0.20f), ButtonOn = new Color(0.58f, 0.34f, 0.88f), HomeCol = new Color(0.28f, 0.10f, 0.42f) },
            new Theme { Name = "Bloodhound",        MenuBg = new Color(0.06f, 0.06f, 0.08f), Accent = new Color(0.85f, 0.35f, 0.10f), ButtonOff = new Color(0.10f, 0.10f, 0.12f), ButtonOn = new Color(0.76f, 0.30f, 0.08f), HomeCol = new Color(0.24f, 0.16f, 0.08f) },

            new Theme { Name = "LoL Gold",          MenuBg = new Color(0.12f, 0.08f, 0.00f), Accent = new Color(0.95f, 0.76f, 0.15f), ButtonOff = new Color(0.18f, 0.12f, 0.01f), ButtonOn = new Color(0.86f, 0.68f, 0.12f), HomeCol = new Color(0.40f, 0.28f, 0.04f) },
            new Theme { Name = "Void (LoL)",        MenuBg = new Color(0.08f, 0.00f, 0.14f), Accent = new Color(0.70f, 0.15f, 0.95f), ButtonOff = new Color(0.12f, 0.01f, 0.20f), ButtonOn = new Color(0.62f, 0.12f, 0.86f), HomeCol = new Color(0.28f, 0.03f, 0.44f) },
            new Theme { Name = "Arcane",            MenuBg = new Color(0.06f, 0.04f, 0.16f), Accent = new Color(0.55f, 0.35f, 0.95f), ButtonOff = new Color(0.09f, 0.06f, 0.22f), ButtonOn = new Color(0.48f, 0.30f, 0.86f), HomeCol = new Color(0.22f, 0.12f, 0.45f) },
            new Theme { Name = "Jinx",              MenuBg = new Color(0.04f, 0.06f, 0.16f), Accent = new Color(0.40f, 0.90f, 0.95f), ButtonOff = new Color(0.06f, 0.09f, 0.22f), ButtonOn = new Color(0.34f, 0.82f, 0.88f), HomeCol = new Color(0.10f, 0.32f, 0.44f) },

            new Theme { Name = "Naruto",            MenuBg = new Color(0.20f, 0.08f, 0.00f), Accent = new Color(0.98f, 0.55f, 0.05f), ButtonOff = new Color(0.28f, 0.11f, 0.01f), ButtonOn = new Color(0.90f, 0.48f, 0.04f), HomeCol = new Color(0.52f, 0.20f, 0.02f) },
            new Theme { Name = "Sasuke",            MenuBg = new Color(0.06f, 0.02f, 0.14f), Accent = new Color(0.55f, 0.20f, 0.90f), ButtonOff = new Color(0.10f, 0.04f, 0.20f), ButtonOn = new Color(0.48f, 0.16f, 0.82f), HomeCol = new Color(0.22f, 0.06f, 0.42f) },
            new Theme { Name = "Dragon Ball",       MenuBg = new Color(0.14f, 0.06f, 0.00f), Accent = new Color(0.98f, 0.62f, 0.05f), ButtonOff = new Color(0.20f, 0.09f, 0.01f), ButtonOn = new Color(0.90f, 0.55f, 0.04f), HomeCol = new Color(0.46f, 0.20f, 0.02f) },
            new Theme { Name = "Super Saiyan",      MenuBg = new Color(0.10f, 0.08f, 0.00f), Accent = new Color(0.95f, 0.92f, 0.05f), ButtonOff = new Color(0.15f, 0.12f, 0.01f), ButtonOn = new Color(0.86f, 0.84f, 0.04f), HomeCol = new Color(0.36f, 0.32f, 0.02f) },
            new Theme { Name = "Demon Slayer",      MenuBg = new Color(0.04f, 0.08f, 0.18f), Accent = new Color(0.90f, 0.25f, 0.25f), ButtonOff = new Color(0.06f, 0.12f, 0.25f), ButtonOn = new Color(0.82f, 0.20f, 0.20f), HomeCol = new Color(0.14f, 0.20f, 0.42f) },
            new Theme { Name = "Tanjiro",           MenuBg = new Color(0.04f, 0.10f, 0.16f), Accent = new Color(0.20f, 0.65f, 0.90f), ButtonOff = new Color(0.06f, 0.14f, 0.22f), ButtonOn = new Color(0.16f, 0.58f, 0.82f), HomeCol = new Color(0.08f, 0.28f, 0.44f) },
            new Theme { Name = "One Piece",         MenuBg = new Color(0.04f, 0.08f, 0.20f), Accent = new Color(0.95f, 0.20f, 0.10f), ButtonOff = new Color(0.06f, 0.12f, 0.28f), ButtonOn = new Color(0.86f, 0.16f, 0.08f), HomeCol = new Color(0.12f, 0.20f, 0.44f) },
            new Theme { Name = "Luffy",             MenuBg = new Color(0.16f, 0.02f, 0.02f), Accent = new Color(0.95f, 0.85f, 0.05f), ButtonOff = new Color(0.22f, 0.04f, 0.04f), ButtonOn = new Color(0.86f, 0.76f, 0.04f), HomeCol = new Color(0.44f, 0.08f, 0.04f) },
            new Theme { Name = "Attack on Titan",   MenuBg = new Color(0.10f, 0.08f, 0.04f), Accent = new Color(0.55f, 0.50f, 0.22f), ButtonOff = new Color(0.15f, 0.12f, 0.05f), ButtonOn = new Color(0.48f, 0.44f, 0.18f), HomeCol = new Color(0.28f, 0.22f, 0.08f) },
            new Theme { Name = "Evangelion",        MenuBg = new Color(0.04f, 0.10f, 0.04f), Accent = new Color(0.85f, 0.18f, 0.18f), ButtonOff = new Color(0.06f, 0.14f, 0.06f), ButtonOn = new Color(0.76f, 0.14f, 0.14f), HomeCol = new Color(0.12f, 0.30f, 0.12f) },

            new Theme { Name = "Roblox",            MenuBg = new Color(0.14f, 0.02f, 0.02f), Accent = new Color(0.92f, 0.92f, 0.92f), ButtonOff = new Color(0.20f, 0.04f, 0.04f), ButtonOn = new Color(0.82f, 0.82f, 0.82f), HomeCol = new Color(0.46f, 0.06f, 0.06f) },
            new Theme { Name = "Adopt Me",          MenuBg = new Color(0.18f, 0.10f, 0.22f), Accent = new Color(0.90f, 0.65f, 0.95f), ButtonOff = new Color(0.24f, 0.14f, 0.30f), ButtonOn = new Color(0.82f, 0.58f, 0.88f), HomeCol = new Color(0.48f, 0.26f, 0.55f) },

            new Theme { Name = "Overwatch",         MenuBg = new Color(0.02f, 0.08f, 0.16f), Accent = new Color(0.95f, 0.72f, 0.08f), ButtonOff = new Color(0.04f, 0.12f, 0.22f), ButtonOn = new Color(0.86f, 0.64f, 0.06f), HomeCol = new Color(0.08f, 0.24f, 0.42f) },
            new Theme { Name = "Valorant",          MenuBg = new Color(0.10f, 0.04f, 0.04f), Accent = new Color(0.95f, 0.30f, 0.30f), ButtonOff = new Color(0.15f, 0.06f, 0.06f), ButtonOn = new Color(0.86f, 0.25f, 0.25f), HomeCol = new Color(0.36f, 0.10f, 0.10f) },
            new Theme { Name = "Terraria",          MenuBg = new Color(0.04f, 0.14f, 0.22f), Accent = new Color(0.45f, 0.88f, 0.30f), ButtonOff = new Color(0.06f, 0.19f, 0.30f), ButtonOn = new Color(0.38f, 0.80f, 0.25f), HomeCol = new Color(0.10f, 0.38f, 0.44f) },
            new Theme { Name = "Subnautica",        MenuBg = new Color(0.00f, 0.08f, 0.20f), Accent = new Color(0.10f, 0.75f, 0.95f), ButtonOff = new Color(0.01f, 0.12f, 0.28f), ButtonOn = new Color(0.08f, 0.68f, 0.86f), HomeCol = new Color(0.02f, 0.28f, 0.48f) },
            new Theme { Name = "Hollow Knight",     MenuBg = new Color(0.04f, 0.04f, 0.10f), Accent = new Color(0.55f, 0.88f, 0.95f), ButtonOff = new Color(0.07f, 0.07f, 0.15f), ButtonOn = new Color(0.48f, 0.80f, 0.88f), HomeCol = new Color(0.14f, 0.22f, 0.38f) },
            new Theme { Name = "Undertale",         MenuBg = new Color(0.00f, 0.00f, 0.00f), Accent = new Color(0.28f, 0.70f, 0.95f), ButtonOff = new Color(0.04f, 0.04f, 0.04f), ButtonOn = new Color(0.22f, 0.62f, 0.86f), HomeCol = new Color(0.06f, 0.22f, 0.35f) },
            new Theme { Name = "Fnaf",              MenuBg = new Color(0.04f, 0.02f, 0.00f), Accent = new Color(0.80f, 0.28f, 0.05f), ButtonOff = new Color(0.07f, 0.04f, 0.01f), ButtonOn = new Color(0.72f, 0.24f, 0.04f), HomeCol = new Color(0.22f, 0.08f, 0.02f) },
            new Theme { Name = "Sonic",             MenuBg = new Color(0.02f, 0.06f, 0.22f), Accent = new Color(0.15f, 0.45f, 0.95f), ButtonOff = new Color(0.04f, 0.09f, 0.30f), ButtonOn = new Color(0.12f, 0.38f, 0.86f), HomeCol = new Color(0.06f, 0.18f, 0.50f) },
            new Theme { Name = "Shadow",            MenuBg = new Color(0.04f, 0.00f, 0.00f), Accent = new Color(0.80f, 0.05f, 0.05f), ButtonOff = new Color(0.08f, 0.01f, 0.01f), ButtonOn = new Color(0.72f, 0.04f, 0.04f), HomeCol = new Color(0.22f, 0.02f, 0.02f) },
            new Theme { Name = "Mario",             MenuBg = new Color(0.20f, 0.02f, 0.02f), Accent = new Color(0.95f, 0.78f, 0.10f), ButtonOff = new Color(0.28f, 0.04f, 0.04f), ButtonOn = new Color(0.86f, 0.70f, 0.08f), HomeCol = new Color(0.52f, 0.06f, 0.06f) },
            new Theme { Name = "Zelda",             MenuBg = new Color(0.10f, 0.08f, 0.00f), Accent = new Color(0.95f, 0.82f, 0.08f), ButtonOff = new Color(0.15f, 0.12f, 0.01f), ButtonOn = new Color(0.86f, 0.74f, 0.06f), HomeCol = new Color(0.36f, 0.28f, 0.02f) },
            new Theme { Name = "Resident Evil",     MenuBg = new Color(0.08f, 0.00f, 0.00f), Accent = new Color(0.88f, 0.12f, 0.05f), ButtonOff = new Color(0.13f, 0.01f, 0.01f), ButtonOn = new Color(0.80f, 0.10f, 0.04f), HomeCol = new Color(0.30f, 0.03f, 0.02f) },
            new Theme { Name = "Silent Hill",       MenuBg = new Color(0.08f, 0.06f, 0.06f), Accent = new Color(0.55f, 0.45f, 0.40f), ButtonOff = new Color(0.12f, 0.09f, 0.09f), ButtonOn = new Color(0.48f, 0.38f, 0.34f), HomeCol = new Color(0.24f, 0.18f, 0.16f) },

            new Theme { Name = "Purple", MenuBg = new Color(0.294f, 0.098f, 0.765f), Accent = new Color(0.542f, 0.398f, 0.880f), ButtonOff = new Color(0.294f, 0.098f, 0.765f), ButtonOn = new Color(0.502f, 0.369f, 0.816f), HomeCol = new Color(0.398f, 0.233f, 0.790f) },
            new Theme { Name = "Red", MenuBg = new Color(0.706f, 0.071f, 0.071f), Accent = new Color(0.880f, 0.088f, 0.088f), ButtonOff = new Color(0.706f, 0.071f, 0.071f), ButtonOn = new Color(0.412f, 0.000f, 0.000f), HomeCol = new Color(0.559f, 0.035f, 0.035f) },
            new Theme { Name = "Yellow", MenuBg = new Color(0.941f, 0.745f, 0.004f), Accent = new Color(0.880f, 0.697f, 0.004f), ButtonOff = new Color(0.941f, 0.745f, 0.004f), ButtonOn = new Color(0.824f, 0.706f, 0.290f), HomeCol = new Color(0.882f, 0.725f, 0.147f) },
            new Theme { Name = "Sage", MenuBg = new Color(0.518f, 0.722f, 0.518f), Accent = new Color(0.631f, 0.880f, 0.631f), ButtonOff = new Color(0.518f, 0.722f, 0.518f), ButtonOn = new Color(0.412f, 0.569f, 0.412f), HomeCol = new Color(0.465f, 0.645f, 0.465f) },
            new Theme { Name = "Sky (CM)", MenuBg = new Color(0.176f, 0.451f, 0.686f), Accent = new Color(0.330f, 0.616f, 0.880f), ButtonOff = new Color(0.176f, 0.451f, 0.686f), ButtonOn = new Color(0.294f, 0.549f, 0.784f), HomeCol = new Color(0.235f, 0.500f, 0.735f) },
            new Theme { Name = "Perrywinkle", MenuBg = new Color(0.824f, 0.588f, 0.882f), Accent = new Color(0.821f, 0.587f, 0.880f), ButtonOff = new Color(0.824f, 0.588f, 0.882f), ButtonOn = new Color(0.706f, 0.510f, 0.765f), HomeCol = new Color(0.765f, 0.549f, 0.824f) },
            new Theme { Name = "Blush", MenuBg = new Color(0.804f, 0.424f, 0.906f), Accent = new Color(0.781f, 0.411f, 0.880f), ButtonOff = new Color(0.804f, 0.424f, 0.906f), ButtonOn = new Color(0.584f, 0.298f, 0.667f), HomeCol = new Color(0.694f, 0.361f, 0.786f) },
            new Theme { Name = "Blood", MenuBg = new Color(0.235f, 0.051f, 0.082f), Accent = new Color(0.880f, 0.293f, 0.386f), ButtonOff = new Color(0.235f, 0.051f, 0.082f), ButtonOn = new Color(0.447f, 0.149f, 0.196f), HomeCol = new Color(0.341f, 0.100f, 0.139f) },
            new Theme { Name = "Sand (CM)", MenuBg = new Color(0.490f, 0.451f, 0.333f), Accent = new Color(0.880f, 0.815f, 0.616f), ButtonOff = new Color(0.490f, 0.451f, 0.333f), ButtonOn = new Color(0.588f, 0.545f, 0.412f), HomeCol = new Color(0.539f, 0.498f, 0.373f) },
            new Theme { Name = "Apricot", MenuBg = new Color(0.788f, 0.490f, 0.016f), Accent = new Color(0.880f, 0.547f, 0.018f), ButtonOff = new Color(0.788f, 0.490f, 0.016f), ButtonOn = new Color(0.576f, 0.349f, 0.039f), HomeCol = new Color(0.682f, 0.420f, 0.027f) },
            new Theme { Name = "Water", MenuBg = new Color(0.000f, 0.000f, 0.275f), Accent = new Color(0.000f, 0.000f, 0.880f), ButtonOff = new Color(0.000f, 0.000f, 0.275f), ButtonOn = new Color(0.000f, 0.000f, 0.314f), HomeCol = new Color(0.000f, 0.000f, 0.294f) },
            new Theme { Name = "Bonsai", MenuBg = new Color(0.255f, 0.000f, 0.294f), Accent = new Color(0.776f, 0.000f, 0.880f), ButtonOff = new Color(0.255f, 0.000f, 0.294f), ButtonOn = new Color(0.294f, 0.000f, 0.333f), HomeCol = new Color(0.275f, 0.000f, 0.314f) },
            new Theme { Name = "Shockwave", MenuBg = new Color(0.353f, 0.059f, 0.608f), Accent = new Color(0.511f, 0.085f, 0.880f), ButtonOff = new Color(0.353f, 0.059f, 0.608f), ButtonOn = new Color(0.098f, 0.098f, 0.098f), HomeCol = new Color(0.225f, 0.078f, 0.353f) },
            new Theme { Name = "Lime", MenuBg = new Color(0.435f, 0.737f, 0.447f), Accent = new Color(0.520f, 0.880f, 0.534f), ButtonOff = new Color(0.435f, 0.737f, 0.447f), ButtonOn = new Color(0.020f, 0.706f, 0.431f), HomeCol = new Color(0.227f, 0.722f, 0.439f) },
            new Theme { Name = "Magma", MenuBg = new Color(0.039f, 0.039f, 0.039f), Accent = new Color(0.880f, 0.448f, 0.072f), ButtonOff = new Color(0.039f, 0.039f, 0.039f), ButtonOn = new Color(0.816f, 0.416f, 0.067f), HomeCol = new Color(0.427f, 0.227f, 0.053f) },
            new Theme { Name = "Summer (CM)", MenuBg = new Color(0.082f, 0.655f, 0.573f), Accent = new Color(0.585f, 0.880f, 0.880f), ButtonOff = new Color(0.082f, 0.655f, 0.573f), ButtonOn = new Color(0.388f, 0.584f, 0.584f), HomeCol = new Color(0.235f, 0.620f, 0.578f) },
            new Theme { Name = "Ink", MenuBg = new Color(0.122f, 0.110f, 0.114f), Accent = new Color(0.003f, 0.338f, 0.880f), ButtonOff = new Color(0.122f, 0.110f, 0.114f), ButtonOn = new Color(0.004f, 0.384f, 1.000f), HomeCol = new Color(0.063f, 0.247f, 0.557f) },
            new Theme { Name = "Dusk (CM)", MenuBg = new Color(0.220f, 0.220f, 0.220f), Accent = new Color(0.880f, 0.880f, 0.880f), ButtonOff = new Color(0.220f, 0.220f, 0.220f), ButtonOn = new Color(0.310f, 0.310f, 0.310f), HomeCol = new Color(0.265f, 0.265f, 0.265f) },
            new Theme { Name = "Magenta (CM)", MenuBg = new Color(0.824f, 0.027f, 0.580f), Accent = new Color(0.880f, 0.029f, 0.620f), ButtonOff = new Color(0.824f, 0.027f, 0.580f), ButtonOn = new Color(0.039f, 0.039f, 0.039f), HomeCol = new Color(0.431f, 0.033f, 0.310f) },
            new Theme { Name = "Twilight (CM)", MenuBg = new Color(0.078f, 0.078f, 0.137f), Accent = new Color(0.572f, 0.572f, 0.880f), ButtonOff = new Color(0.078f, 0.078f, 0.137f), ButtonOn = new Color(0.153f, 0.153f, 0.235f), HomeCol = new Color(0.116f, 0.116f, 0.186f) },
            new Theme { Name = "Night", MenuBg = new Color(0.024f, 0.024f, 0.024f), Accent = new Color(0.880f, 0.880f, 0.880f), ButtonOff = new Color(0.024f, 0.024f, 0.024f), ButtonOn = new Color(0.055f, 0.055f, 0.055f), HomeCol = new Color(0.039f, 0.039f, 0.039f) },
            new Theme { Name = "Frost", MenuBg = new Color(0.706f, 0.863f, 1.000f), Accent = new Color(0.621f, 0.759f, 0.880f), ButtonOff = new Color(0.706f, 0.863f, 1.000f), ButtonOn = new Color(0.471f, 0.667f, 0.863f), HomeCol = new Color(0.588f, 0.765f, 0.931f) },
            new Theme { Name = "Emerald (CM)", MenuBg = new Color(0.000f, 0.471f, 0.353f), Accent = new Color(0.000f, 0.880f, 0.660f), ButtonOff = new Color(0.000f, 0.471f, 0.353f), ButtonOn = new Color(0.000f, 0.784f, 0.588f), HomeCol = new Color(0.000f, 0.627f, 0.471f) },
            new Theme { Name = "Sunset2", MenuBg = new Color(1.000f, 0.369f, 0.227f), Accent = new Color(0.880f, 0.673f, 0.390f), ButtonOff = new Color(1.000f, 0.369f, 0.227f), ButtonOn = new Color(1.000f, 0.765f, 0.443f), HomeCol = new Color(1.000f, 0.567f, 0.335f) },
            new Theme { Name = "Royal", MenuBg = new Color(0.176f, 0.000f, 0.471f), Accent = new Color(0.449f, 0.000f, 0.880f), ButtonOff = new Color(0.176f, 0.000f, 0.471f), ButtonOn = new Color(0.510f, 0.000f, 1.000f), HomeCol = new Color(0.343f, 0.000f, 0.735f) },
            new Theme { Name = "Cyberpunk (CM)", MenuBg = new Color(0.039f, 0.039f, 0.039f), Accent = new Color(0.880f, 0.000f, 0.518f), ButtonOff = new Color(0.039f, 0.039f, 0.039f), ButtonOn = new Color(1.000f, 0.000f, 0.588f), HomeCol = new Color(0.520f, 0.020f, 0.314f) },
            new Theme { Name = "Ocean2", MenuBg = new Color(0.000f, 0.235f, 0.431f), Accent = new Color(0.000f, 0.616f, 0.880f), ButtonOff = new Color(0.000f, 0.235f, 0.431f), ButtonOn = new Color(0.000f, 0.549f, 0.784f), HomeCol = new Color(0.000f, 0.392f, 0.608f) },
            new Theme { Name = "Obsidian (CM)", MenuBg = new Color(0.059f, 0.059f, 0.078f), Accent = new Color(0.566f, 0.000f, 0.880f), ButtonOff = new Color(0.059f, 0.059f, 0.078f), ButtonOn = new Color(0.176f, 0.000f, 0.275f), HomeCol = new Color(0.118f, 0.029f, 0.176f) },
            new Theme { Name = "Toxic2", MenuBg = new Color(0.078f, 0.157f, 0.000f), Accent = new Color(0.587f, 0.880f, 0.000f), ButtonOff = new Color(0.078f, 0.157f, 0.000f), ButtonOn = new Color(0.667f, 1.000f, 0.000f), HomeCol = new Color(0.373f, 0.578f, 0.000f) },
            new Theme { Name = "Cotton Candy", MenuBg = new Color(1.000f, 0.667f, 0.863f), Accent = new Color(0.587f, 0.759f, 0.880f), ButtonOff = new Color(1.000f, 0.667f, 0.863f), ButtonOn = new Color(0.667f, 0.863f, 1.000f), HomeCol = new Color(0.833f, 0.765f, 0.931f) },
            new Theme { Name = "Silver (CM)", MenuBg = new Color(0.471f, 0.471f, 0.471f), Accent = new Color(0.880f, 0.880f, 0.880f), ButtonOff = new Color(0.471f, 0.471f, 0.471f), ButtonOn = new Color(0.784f, 0.784f, 0.784f), HomeCol = new Color(0.627f, 0.627f, 0.627f) },
            new Theme { Name = "Vampire", MenuBg = new Color(0.157f, 0.000f, 0.000f), Accent = new Color(0.880f, 0.000f, 0.440f), ButtonOff = new Color(0.157f, 0.000f, 0.000f), ButtonOn = new Color(0.471f, 0.000f, 0.235f), HomeCol = new Color(0.314f, 0.000f, 0.118f) },
            new Theme { Name = "Aurora (CM)", MenuBg = new Color(0.039f, 0.157f, 0.235f), Accent = new Color(0.000f, 0.880f, 0.528f), ButtonOff = new Color(0.039f, 0.157f, 0.235f), ButtonOn = new Color(0.000f, 0.784f, 0.471f), HomeCol = new Color(0.020f, 0.471f, 0.353f) },
            new Theme { Name = "Rust2", MenuBg = new Color(0.471f, 0.196f, 0.078f), Accent = new Color(0.880f, 0.367f, 0.147f), ButtonOff = new Color(0.471f, 0.196f, 0.078f), ButtonOn = new Color(0.314f, 0.118f, 0.039f), HomeCol = new Color(0.392f, 0.157f, 0.059f) },
            new Theme { Name = "Void2", MenuBg = new Color(0.020f, 0.000f, 0.059f), Accent = new Color(0.528f, 0.000f, 0.880f), ButtonOff = new Color(0.020f, 0.000f, 0.059f), ButtonOn = new Color(0.118f, 0.000f, 0.196f), HomeCol = new Color(0.069f, 0.000f, 0.127f) },
            new Theme { Name = "Cherry2", MenuBg = new Color(0.706f, 0.078f, 0.235f), Accent = new Color(0.880f, 0.345f, 0.449f), ButtonOff = new Color(0.706f, 0.078f, 0.235f), ButtonOn = new Color(1.000f, 0.392f, 0.510f), HomeCol = new Color(0.853f, 0.235f, 0.373f) },
            new Theme { Name = "Glacier (CM)", MenuBg = new Color(0.392f, 0.706f, 0.863f), Accent = new Color(0.690f, 0.828f, 0.880f), ButtonOff = new Color(0.392f, 0.706f, 0.863f), ButtonOn = new Color(0.784f, 0.941f, 1.000f), HomeCol = new Color(0.588f, 0.824f, 0.931f) },
            new Theme { Name = "Neon2", MenuBg = new Color(0.000f, 1.000f, 0.392f), Accent = new Color(0.000f, 0.880f, 0.345f), ButtonOff = new Color(0.000f, 1.000f, 0.392f), ButtonOn = new Color(0.000f, 0.196f, 0.078f), HomeCol = new Color(0.000f, 0.598f, 0.235f) },
            new Theme { Name = "Lavender (CM)", MenuBg = new Color(0.588f, 0.471f, 0.784f), Accent = new Color(0.759f, 0.690f, 0.880f), ButtonOff = new Color(0.588f, 0.471f, 0.784f), ButtonOn = new Color(0.863f, 0.784f, 1.000f), HomeCol = new Color(0.725f, 0.627f, 0.892f) },
            new Theme { Name = "Inferno (CM)", MenuBg = new Color(0.784f, 0.235f, 0.000f), Accent = new Color(0.880f, 0.690f, 0.000f), ButtonOff = new Color(0.784f, 0.235f, 0.000f), ButtonOn = new Color(1.000f, 0.784f, 0.000f), HomeCol = new Color(0.892f, 0.510f, 0.000f) },
            new Theme { Name = "Abyss (CM)", MenuBg = new Color(0.000f, 0.039f, 0.118f), Accent = new Color(0.000f, 0.489f, 0.880f), ButtonOff = new Color(0.000f, 0.039f, 0.118f), ButtonOn = new Color(0.000f, 0.196f, 0.353f), HomeCol = new Color(0.000f, 0.118f, 0.235f) },
            new Theme { Name = "Clay", MenuBg = new Color(0.116f, 0.063f, 0.046f), Accent = new Color(0.723f, 0.397f, 0.290f), ButtonOff = new Color(0.116f, 0.063f, 0.046f), ButtonOn = new Color(0.304f, 0.167f, 0.122f), HomeCol = new Color(0.210f, 0.115f, 0.084f) },
            new Theme { Name = "Slate2", MenuBg = new Color(0.057f, 0.075f, 0.101f), Accent = new Color(0.354f, 0.469f, 0.630f), ButtonOff = new Color(0.057f, 0.075f, 0.101f), ButtonOn = new Color(0.149f, 0.197f, 0.265f), HomeCol = new Color(0.103f, 0.136f, 0.183f) },
            new Theme { Name = "Forest2", MenuBg = new Color(0.057f, 0.090f, 0.055f), Accent = new Color(0.357f, 0.560f, 0.341f), ButtonOff = new Color(0.057f, 0.090f, 0.055f), ButtonOn = new Color(0.150f, 0.235f, 0.143f), HomeCol = new Color(0.104f, 0.162f, 0.099f) },
            new Theme { Name = "Rose", MenuBg = new Color(0.110f, 0.055f, 0.075f), Accent = new Color(0.688f, 0.346f, 0.471f), ButtonOff = new Color(0.110f, 0.055f, 0.075f), ButtonOn = new Color(0.289f, 0.145f, 0.198f), HomeCol = new Color(0.200f, 0.100f, 0.137f) },
            new Theme { Name = "Mono", MenuBg = new Color(0.093f, 0.090f, 0.086f), Accent = new Color(0.580f, 0.565f, 0.540f), ButtonOff = new Color(0.093f, 0.090f, 0.086f), ButtonOn = new Color(0.244f, 0.237f, 0.227f), HomeCol = new Color(0.168f, 0.164f, 0.156f) },
            new Theme { Name = "Midnight2", MenuBg = new Color(0.054f, 0.049f, 0.083f), Accent = new Color(0.336f, 0.304f, 0.520f), ButtonOff = new Color(0.054f, 0.049f, 0.083f), ButtonOn = new Color(0.141f, 0.128f, 0.219f), HomeCol = new Color(0.097f, 0.088f, 0.151f) },
        };

        public static int ActiveIndex = 0;

        public static Theme Current => Presets[ActiveIndex];

        public static Color CurrentAccent => Presets[ActiveIndex].Accent;

        public static void Apply(int index)
        {
            if (index < 0 || index >= Presets.Count) return;
            ActiveIndex = index;
            Theme th = Presets[index];




        Menu.PanelAsset.RetintMaterial("shell_2a2437", th.MenuBg);
            Menu.PanelAsset.RetintMaterial("violet_9d6bf5", th.HomeCol);

            if (Main.menu != null)
            {
                Main.menu.GetComponent<UnityEngine.Renderer>()
                    ?.material.SetColor("_Color", th.MenuBg);
                Main.RecreateMenu();
            }


            if (LagMenuOnGUI.Instance != null)
                LagMenuOnGUI.Instance.ApplyTheme(th.Accent, th.MenuBg, th.ButtonOn, th.ButtonOff);


            Menu.ModPresets.SaveTheme();
        }

        public static void CycleNext()
        {
            Apply((ActiveIndex + 1) % Presets.Count);
        }

        public static void CyclePrev()
        {
            Apply((ActiveIndex - 1 + Presets.Count) % Presets.Count);
        }

        public static string CurrentName =>
            Presets[ActiveIndex].Name;
    }
}
