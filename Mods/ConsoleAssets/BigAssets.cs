using System;
using System.Collections.Generic;
using System.Text;

namespace LagMenu.Mods.ConsoleAssets
{

    public class BigAssets
    {
        public static bool isEnabled;

        public static void OnEnable() => isEnabled = true;
        public static void OnDisable() => isEnabled = false;
    }
}
