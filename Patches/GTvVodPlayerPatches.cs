using System;
using HarmonyLib;
using UnityEngine.Video;




namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(VODPlayer))]
    public class GTvVodPlayerPatches
    {
        private static readonly string[] VideoOptions = new string[]
               {
            "https://mrlag.lol/LagFlix/AdminLaggy/Mr%20Lag%20On%20Christmas.mp4",
            "https://mrlag.lol/LagFlix/AdminLaggy/YAAI",
               };

        public static int videoIndex = 0;

        public static string ForcedVideoURL => VideoOptions[videoIndex];

        public static string GetVideoName()
        {

            return $"Video {videoIndex + 1}/{VideoOptions.Length}";
        }

        public static void IncreaseVideoIndex()
        {
            videoIndex++;
            if (videoIndex >= VideoOptions.Length)
            {
                videoIndex = 0;
            }
        }

        public static void DecreaseVideoIndex()
        {
            videoIndex--;
            if (videoIndex < 0)
            {
                videoIndex = VideoOptions.Length - 1;
            }
        }

        private static VODPlayer.VODStream.VODStreamChannel DefaultChannel => VODPlayer.VODStream.VODStreamChannel.DEFAULT;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(VODPlayer.OnEnable))]
        public static void OnEnable_Postfix(VODPlayer __instance)
        {
            VODPlayer.state = VODPlayer.State.RUNNING;
            __instance.StartVideoPlayback(
                    ForcedVideoURL,
                    ForcedVideoURL,
                    DefaultChannel,
                    0.0,
                    null
            );
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(VODPlayer.Player_loopPointReached))]
        public static bool PlayerLoopPointReachedPrefix(VODPlayer __instance, VideoPlayer source)
        {
            __instance.StartVideoPlayback(
                    ForcedVideoURL,
                    ForcedVideoURL,
                    DefaultChannel,
                    0.0,
                    null
            );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("IGorillaSliceableSimple.SliceUpdate")]
        public static bool SliceUpdatePrefix(VODPlayer __instance)
        {
            if (VODPlayer.state != VODPlayer.State.RUNNING)
                VODPlayer.state = VODPlayer.State.RUNNING;

            if (__instance.player != null && __instance.player.isPlaying)
                __instance.PositionAudio();
            else if (!__instance.playerBusy)
                __instance.StartVideoPlayback(
                        ForcedVideoURL,
                        ForcedVideoURL,
                        DefaultChannel,
                        0.0,
                        null
                );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(VODPlayer.StartPlayback))]
        public static bool StartPlaybackPrefix(VODPlayer __instance, ref VODPlayer.VODStream str, ref double time)
        {
            __instance.StartVideoPlayback(
                    ForcedVideoURL,
                    ForcedVideoURL,
                    DefaultChannel,
                    0.0,
                    null
            );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(VODPlayer.PlayPreviouStream))]
        public static bool PlayPreviouStreamPrefix(VODPlayer __instance)
        {
            __instance.StartVideoPlayback(
                    ForcedVideoURL,
                    ForcedVideoURL,
                    DefaultChannel,
                    0.0,
                    null
            );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VODPlayer), nameof(VODPlayer.GetNextStream), typeof(VODPlayer.VODStream.VODStreamChannel[]))]
        public static bool GetNextStreamPrefix(ref VODPlayer.VODNextStreamData __result)
        {
            __result = new VODPlayer.VODNextStreamData("", DateTime.MinValue);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(VODPlayer.StartImagePlayback))]
        public static bool StartImagePlaybackPrefix(VODPlayer __instance)
        {
            __instance.StartVideoPlayback(
                    ForcedVideoURL,
                    ForcedVideoURL,
                    DefaultChannel,
                    0.0,
                    null
            );

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(VODPlayer.StartVideoPlayback))]
        public static void StartVideoPlaybackPrefix(VODPlayer __instance)
        {
            if (__instance.player != null)
                __instance.player.aspectRatio = VideoAspectRatio.Stretch;
        }
    }
}
