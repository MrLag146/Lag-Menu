using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LagMenu.Menu.ButtonInfo;
using static LagMenu.Menu.Buttons;

namespace LagMenu.Mods
{
    public static class VideoPlayerType
    {
        private static readonly Dictionary<string, string> Videos = new Dictionary<string, string>()
        {
            { "Mr Bean Edit", "https://github.com/MrLag146/consoke/raw/refs/heads/main/Mr%20Bean%20Compressed.mp4" },
            { "Mr Bean Edit 2", "https://github.com/MrLag146/consoke/raw/refs/heads/main/MediaPlayer/mr%20bean%20edit%202%20compressed.mp4" },
            { "Scott", "https://files.catbox.moe/xeiy7t.mp4" },
            { "Spider Man Polyester Edit", "https://github.com/MrLag146/consoke/raw/refs/heads/main/MediaPlayer/spiderman%20polyester.mp4" },

        };

        private static readonly List<string> Keys = Videos.Keys.ToList();

        private static int currentIndex = 0;

        public static string CurrentName => Keys[currentIndex];

        public static string CurrentUrl => Videos[Keys[currentIndex]];



        public static void RefreshVideoButton()
        {
            foreach (var b in buttons[15])
            {
                if (b.buttonText.StartsWith("Video:"))
                {
                    b.buttonText = "Video: " + VideoPlayerType.CurrentName;
                    break;
                }
            }
        }

        public static void CycleNext()
        {
            currentIndex++;
            if (currentIndex >= Keys.Count)
                currentIndex = 0;
        }











        public static void CyclePrev()
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = Keys.Count - 1;
        }
    }
}
