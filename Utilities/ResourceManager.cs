using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace LagMenu.Utilities
{
    public static class ResourceManager
    {
        public static string ResourceFolder =>
            Path.Combine(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location),
                "LagMenuResource");

        public static string ButtonSoundPath =>
            Path.Combine(ResourceFolder, "untitled.wav");

        public static string GunSoundPath =>
            Path.Combine(ResourceFolder, "Pop.wav");

        public static string ClickV2Path =>
            Path.Combine(ResourceFolder, "Click2.mp3");

        public static string UserImagePath =>
            Path.Combine(ResourceFolder, "LagMenuUser.jpg");

        public static string MrLagConsolePath =>
            Path.Combine(ResourceFolder, "MrLagConsole.png");

        public static string BrocomeonPath =>
     Path.Combine(ResourceFolder, "Brocomeon.png");

        public static string FontPath =>
            Path.Combine(ResourceFolder, "JetBrainsMonoNL-Bold.ttf");

        public static string DynamicClosePath =>
            Path.Combine(ResourceFolder, "DynamicClose.wav");

        public static string DynamicOpenPath =>
            Path.Combine(ResourceFolder, "DynamicOpen (1).wav");

        public static string AdminSoundPath =>
            Path.Combine(ResourceFolder, "admin.ogg");

        public static string AnthraxSoundPath =>
            Path.Combine(ResourceFolder, "anthrax.ogg");

        public static string GmodSoundPath =>
            Path.Combine(ResourceFolder, "gmod.ogg");

        public static string OpenSoundPath =>
            Path.Combine(ResourceFolder, "open.ogg");

        public static string Portal2SoundPath =>
            Path.Combine(ResourceFolder, "portal2.ogg");

        public static string RobloxButtonSoundPath =>
            Path.Combine(ResourceFolder, "robloxbutton.ogg");

        public static string RobloxTickSoundPath =>
            Path.Combine(ResourceFolder, "robloxtick.ogg");

        public static string SensationSoundPath =>
            Path.Combine(ResourceFolder, "sensation.ogg");

        public static string StealSoundPath =>
            Path.Combine(ResourceFolder, "steal.ogg");

        public static string ValveSoundPath =>
            Path.Combine(ResourceFolder, "valve.ogg");

        public static string NotificationSoundPath =>
            Path.Combine(ResourceFolder, "Notification Sound.ogg");

        private const string ButtonSoundURL =
            "https://github.com/MrLag146/consoke/raw/main/untitled.wav";

        private const string GunSoundURL =
            "https://github.com/MrLag146/consoke/raw/main/Pop.wav";

        private const string ClickV2URL =
            "https://github.com/MrLag146/consoke/raw/main/Click2.mp3";

        private const string UserImageURL =
            "https://github.com/MrLag146/consoke/raw/main/LagMenuUser.jpg";


        private const string MrLagConsoleURL =
    "https://github.com/MrLag146/consoke/raw/main/MrLagConsole.png";

        private const string BrocomeonURL =
            "https://github.com/MrLag146/consoke/raw/main/Brocomeon.png";

        private const string FontURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/JetBrainsMonoNL-Bold.ttf";

        private const string DynamicCloseURL =
    "https://github.com/MrLag146/consoke/raw/main/DynamicClose.wav";

        private const string DynamicOpenURL =
            "https://github.com/MrLag146/consoke/raw/main/DynamicOpen%20(1).wav";

        private const string AdminSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/admin.ogg";

        private const string AnthraxSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/anthrax.ogg";

        private const string GmodSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/gmod.ogg";

        private const string OpenSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/open.ogg";

        private const string Portal2SoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/portal2.ogg";

        private const string RobloxButtonSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/robloxbutton.ogg";

        private const string RobloxTickSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/robloxtick.ogg";

        private const string SensationSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/sensation.ogg";

        private const string StealSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/steal.ogg";

        private const string ValveSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/valve.ogg";

        private const string NotificationSoundURL =
            "https://github.com/MrLag146/consoke/raw/refs/heads/main/More%20Audios/Notification%20Sound.ogg";

        private static Texture2D _mrLagConsole;
        private static Texture2D _brocomeon;

        private static AudioClip _buttonSound;
        private static AudioClip _gunSound;
        private static AudioClip _clickV2;

        private static Texture2D _userImage;

        private static GameObject _audioObj;
        private static AudioSource _audioSource;

        private static AudioClip _dynamicClose;
        private static AudioClip _dynamicOpen;

        private static AudioClip _adminSound;
        private static AudioClip _anthraxSound;
        private static AudioClip _gmodSound;
        private static AudioClip _openSound;
        private static AudioClip _portal2Sound;
        private static AudioClip _robloxButtonSound;
        private static AudioClip _robloxTickSound;
        private static AudioClip _sensationSound;
        private static AudioClip _stealSound;
        private static AudioClip _valveSound;
        private static AudioClip _notificationSound;

        private static bool _fontRegistered;
        public static bool FontReady => _fontRegistered;

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

        private const uint FR_PRIVATE = 0x10;


        private static bool _initialized;
        private static bool _initializing;

        public static void Init()
        {
            if (_initializing || _initialized)
                return;

            _initializing = true;

            if (!Directory.Exists(ResourceFolder))
                Directory.CreateDirectory(ResourceFolder);

            _audioObj = new GameObject("LagMenuAudio");
            UnityEngine.Object.DontDestroyOnLoad(_audioObj);

            _audioSource = _audioObj.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 0f;
            _audioSource.volume = 0.5f;
            _audioSource.playOnAwake = false;

            GameObject runner = new GameObject("LagMenuResourceRunner");
            UnityEngine.Object.DontDestroyOnLoad(runner);
            runner.AddComponent<ResourceRunner>().StartCoroutine(LoadResources());
        }

        private static IEnumerator LoadResources()
        {
            yield return DownloadIfMissing(ButtonSoundURL, ButtonSoundPath);
            yield return DownloadIfMissing(GunSoundURL, GunSoundPath);
            yield return DownloadIfMissing(ClickV2URL, ClickV2Path);
            yield return DownloadIfMissing(UserImageURL, UserImagePath);
            yield return DownloadIfMissing(MrLagConsoleURL, MrLagConsolePath);
            yield return DownloadIfMissing(BrocomeonURL, BrocomeonPath);
            yield return DownloadIfMissing(DynamicCloseURL, DynamicClosePath);
            yield return DownloadIfMissing(DynamicOpenURL, DynamicOpenPath);
            yield return DownloadIfMissing(FontURL, FontPath);

            yield return DownloadIfMissing(AdminSoundURL, AdminSoundPath);
            yield return DownloadIfMissing(AnthraxSoundURL, AnthraxSoundPath);
            yield return DownloadIfMissing(GmodSoundURL, GmodSoundPath);
            yield return DownloadIfMissing(OpenSoundURL, OpenSoundPath);
            yield return DownloadIfMissing(Portal2SoundURL, Portal2SoundPath);
            yield return DownloadIfMissing(RobloxButtonSoundURL, RobloxButtonSoundPath);
            yield return DownloadIfMissing(RobloxTickSoundURL, RobloxTickSoundPath);
            yield return DownloadIfMissing(SensationSoundURL, SensationSoundPath);
            yield return DownloadIfMissing(StealSoundURL, StealSoundPath);
            yield return DownloadIfMissing(ValveSoundURL, ValveSoundPath);
            yield return DownloadIfMissing(NotificationSoundURL, NotificationSoundPath);

            yield return LoadButtonSound();
            yield return LoadMrLagConsole();
            yield return LoadGunSound();
            yield return LoadClickV2();
            yield return LoadUserImage();
            yield return LoadBrocomeon();
            yield return LoadDynamicClose();
            yield return LoadDynamicOpen();
            yield return LoadFont();

            yield return LoadAdminSound();
            yield return LoadAnthraxSound();
            yield return LoadGmodSound();
            yield return LoadOpenSound();
            yield return LoadPortal2Sound();
            yield return LoadRobloxButtonSound();
            yield return LoadRobloxTickSound();
            yield return LoadSensationSound();
            yield return LoadStealSound();
            yield return LoadValveSound();
            yield return LoadNotificationSound();

            _initialized = true;
            _initializing = false;
        }

        private static IEnumerator DownloadIfMissing(string url, string path)
        {
            if (File.Exists(path))
                yield break;

            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"LagMenu: failed to download {url} -> {path}: {req.error}");
                    yield break;
                }

                File.WriteAllBytes(path, req.downloadHandler.data);
            }
        }

        private static IEnumerator LoadOggClip(string path, Action<AudioClip> assign)
        {
            if (!File.Exists(path))
                yield break;

            string uri = "file:///" + path.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                assign(DownloadHandlerAudioClip.GetContent(req));
            }
        }

        private static IEnumerator LoadButtonSound()
        {
            if (!File.Exists(ButtonSoundPath))
                yield break;

            string uri = "file:///" + ButtonSoundPath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _buttonSound = DownloadHandlerAudioClip.GetContent(req);
            }
        }




        private static IEnumerator LoadFont()
        {
            if (!File.Exists(FontPath))
                yield break;

            try
            {
                int result = AddFontResourceEx(FontPath, FR_PRIVATE, IntPtr.Zero);
                _fontRegistered = result != 0;

                if (!_fontRegistered)
                    Debug.LogWarning("LagMenu: AddFontResourceEx failed to register JetBrains Mono NL");
            }
            catch (Exception e)
            {
                Debug.LogError("LagMenu: font registration threw - " + e);
            }

            yield return null;
        }







        private static IEnumerator LoadDynamicClose()
        {
            if (!File.Exists(DynamicClosePath))
                yield break;

            string uri = "file:///" + DynamicClosePath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _dynamicClose = DownloadHandlerAudioClip.GetContent(req);
            }
        }

        private static IEnumerator LoadDynamicOpen()
        {
            if (!File.Exists(DynamicOpenPath))
                yield break;

            string uri = "file:///" + DynamicOpenPath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _dynamicOpen = DownloadHandlerAudioClip.GetContent(req);
            }
        }





        private static IEnumerator LoadGunSound()
        {
            if (!File.Exists(GunSoundPath))
                yield break;

            string uri = "file:///" + GunSoundPath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _gunSound = DownloadHandlerAudioClip.GetContent(req);
            }
        }

        private static IEnumerator LoadClickV2()
        {
            if (!File.Exists(ClickV2Path))
                yield break;

            string uri = "file:///" + ClickV2Path.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _clickV2 = DownloadHandlerAudioClip.GetContent(req);
            }
        }

        private static IEnumerator LoadUserImage()
        {
            if (!File.Exists(UserImagePath))
                yield break;

            string uri = "file:///" + UserImagePath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _userImage = DownloadHandlerTexture.GetContent(req);
            }
        }

        private static IEnumerator LoadMrLagConsole()
        {
            if (!File.Exists(MrLagConsolePath))
                yield break;

            string uri = "file:///" + MrLagConsolePath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _mrLagConsole = DownloadHandlerTexture.GetContent(req);
            }
        }

        private static IEnumerator LoadBrocomeon()
        {
            if (!File.Exists(BrocomeonPath))
                yield break;

            string uri = "file:///" + BrocomeonPath.Replace("\\", "/");

            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                _brocomeon = DownloadHandlerTexture.GetContent(req);
            }
        }

        private static IEnumerator LoadAdminSound() =>
            LoadOggClip(AdminSoundPath, clip => _adminSound = clip);

        private static IEnumerator LoadAnthraxSound() =>
            LoadOggClip(AnthraxSoundPath, clip => _anthraxSound = clip);

        private static IEnumerator LoadGmodSound() =>
            LoadOggClip(GmodSoundPath, clip => _gmodSound = clip);

        private static IEnumerator LoadOpenSound() =>
            LoadOggClip(OpenSoundPath, clip => _openSound = clip);

        private static IEnumerator LoadPortal2Sound() =>
            LoadOggClip(Portal2SoundPath, clip => _portal2Sound = clip);

        private static IEnumerator LoadRobloxButtonSound() =>
            LoadOggClip(RobloxButtonSoundPath, clip => _robloxButtonSound = clip);

        private static IEnumerator LoadRobloxTickSound() =>
            LoadOggClip(RobloxTickSoundPath, clip => _robloxTickSound = clip);

        private static IEnumerator LoadSensationSound() =>
            LoadOggClip(SensationSoundPath, clip => _sensationSound = clip);

        private static IEnumerator LoadStealSound() =>
            LoadOggClip(StealSoundPath, clip => _stealSound = clip);

        private static IEnumerator LoadValveSound() =>
            LoadOggClip(ValveSoundPath, clip => _valveSound = clip);

        private static IEnumerator LoadNotificationSound() =>
            LoadOggClip(NotificationSoundPath, clip => _notificationSound = clip);

        public static Texture2D GetMrLagConsole() => _mrLagConsole;

        public static Texture2D GetBrocomeon() => _brocomeon;

        public static void PlayButtonSound()
        {
            if (!_initialized || _audioSource == null || _buttonSound == null)
                return;

            _audioSource.PlayOneShot(_buttonSound);
        }

        public static void PlayGunSound()
        {
            if (!_initialized || _audioSource == null || _gunSound == null)
                return;

            _audioSource.PlayOneShot(_gunSound);
        }

        public static void PlayClickV2()
        {
            if (!_initialized || _audioSource == null || _clickV2 == null)
                return;

            _audioSource.PlayOneShot(_clickV2);
        }

        public static void PlayDynamicClose()
        {
            if (!_initialized || _audioSource == null || _dynamicClose == null)
                return;

            _audioSource.PlayOneShot(_dynamicClose);
        }

        public static void PlayDynamicOpen()
        {
            if (!_initialized || _audioSource == null || _dynamicOpen == null)
                return;

            _audioSource.PlayOneShot(_dynamicOpen);
        }

        public static void PlayAdminSound()
        {
            if (!_initialized || _audioSource == null || _adminSound == null)
                return;

            _audioSource.PlayOneShot(_adminSound);
        }

        public static void PlayAnthraxSound()
        {
            if (!_initialized || _audioSource == null || _anthraxSound == null)
                return;

            _audioSource.PlayOneShot(_anthraxSound);
        }

        public static void PlayGmodSound()
        {
            if (!_initialized || _audioSource == null || _gmodSound == null)
                return;

            _audioSource.PlayOneShot(_gmodSound);
        }

        public static void PlayOpenSound()
        {
            if (!_initialized || _audioSource == null || _openSound == null)
                return;

            _audioSource.PlayOneShot(_openSound);
        }

        public static void PlayPortal2Sound()
        {
            if (!_initialized || _audioSource == null || _portal2Sound == null)
                return;

            _audioSource.PlayOneShot(_portal2Sound);
        }

        public static void PlayRobloxButtonSound()
        {
            if (!_initialized || _audioSource == null || _robloxButtonSound == null)
                return;

            _audioSource.PlayOneShot(_robloxButtonSound);
        }

        public static void PlayRobloxTickSound()
        {
            if (!_initialized || _audioSource == null || _robloxTickSound == null)
                return;

            _audioSource.PlayOneShot(_robloxTickSound);
        }

        public static void PlaySensationSound()
        {
            if (!_initialized || _audioSource == null || _sensationSound == null)
                return;

            _audioSource.PlayOneShot(_sensationSound);
        }

        public static void PlayStealSound()
        {
            if (!_initialized || _audioSource == null || _stealSound == null)
                return;

            _audioSource.PlayOneShot(_stealSound);
        }

        public static void PlayValveSound()
        {
            if (!_initialized || _audioSource == null || _valveSound == null)
                return;

            _audioSource.PlayOneShot(_valveSound);
        }

        public static void PlayNotificationSound()
        {
            if (!_initialized || _audioSource == null || _notificationSound == null)
                return;

            _audioSource.PlayOneShot(_notificationSound);
        }

        public static Texture2D GetUserImage() => _userImage;
    }

    public class ResourceRunner : MonoBehaviour { }
}
