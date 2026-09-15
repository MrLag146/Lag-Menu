
using LagMenu;

namespace LagMenu.Mods
{



    public class TravisEvent
    {
        public static void OnEnable() => ConsoleEventBridge.SpawnTravis();
        public static void OnDisable() => ConsoleEventBridge.DestroyTravis();
    }

    public class EventView
    {
        public static void OnEnable() => ConsoleEventBridge.ClearEventView();
        public static void OnDisable() => ConsoleEventBridge.RestoreEventView();
    }
}
