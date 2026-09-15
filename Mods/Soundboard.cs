using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using LagMenu.Menu;

namespace LagMenu.Mods
{
    public static class Soundboard
    {
        public static bool IsPlaying = false;
        public static float RecoverTimer = -1f;
        public static bool HearSelf = false;
        public static bool LoopAudio = false;

        private static GameObject _audioObj;
        private static AudioSource _audioSource;
        private static readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();
        private static bool _loaded = false;

        private static int _soundboardCategoryIndex = -1;

        public static string SoundsFolder
        {
            get
            {
                string path = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Sounds");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public static void Init()
        {
            _soundboardCategoryIndex = -1;

            for (int i = 0; i < Buttons.buttons.Length; i++)
            {
                if (Buttons.buttons[i].Any(x => x.buttonText == "Back"))
                {
                    if (Buttons.buttons[i].Any(x => x.buttonText == "Stop All"))
                    {
                        _soundboardCategoryIndex = i;
                        break;
                    }
                }
            }
        }

        public static void UpdateSoundboard()
        {
            if (!_loaded)
                PopulateSounds();

            if (!LoopAudio && IsPlaying && RecoverTimer > 0f && Time.time >= RecoverTimer)
                HaltAudio();
        }

        public static void PopulateSounds()
        {
            _loaded = true;

            if (_soundboardCategoryIndex < 0)
                Init();

            string[] supportedExts = { ".wav", ".ogg", ".mp3" };

            string[] files = Directory.GetFiles(SoundsFolder, "*.*", SearchOption.AllDirectories)
                .Where(f => supportedExts.Contains(Path.GetExtension(f).ToLower()))
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .ToArray();

            List<ButtonInfo> soundButtons = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Stop All", isTogglable = false, method = () => HaltAudio() },

                new ButtonInfo
                {
                    buttonText = "Hear Self",
                    isTogglable = true,
                    enabled = HearSelf,
                    enableMethod = () => HearSelf = true,
                    disableMethod = () => HearSelf = false
                },

                new ButtonInfo
                {
                    buttonText = "Loop Audio",
                    isTogglable = true,
                    enabled = LoopAudio,
                    enableMethod = () => LoopAudio = true,
                    disableMethod = () => LoopAudio = false
                },

                new ButtonInfo { buttonText = "Open Folder", isTogglable = false, method = () => RevealSoundFolder() },

                new ButtonInfo
                {
                    buttonText = "Reload Sounds",
                    isTogglable = false,
                    method = () =>
                    {
                        _loaded = false;
                        _clipCache.Clear();
                        PopulateSounds();
                        Main.RecreateMenu();
                    }
                }
            };

            foreach (string file in files)
            {
                string pathCopy = file;
                string name = Path.GetFileNameWithoutExtension(file).Replace("_", " ");

                ButtonInfo btn = null;

                btn = new ButtonInfo
                {
                    buttonText = name,
                    isTogglable = true,

                    enableMethod = () =>
                    {
                        StopAllSoundButtons();
                        btn.enabled = true;
                        PlayFile(pathCopy);
                    },

                    disableMethod = () => HaltAudio()
                };

                soundButtons.Add(btn);
            }

            if (_soundboardCategoryIndex >= 0)
                Buttons.buttons[_soundboardCategoryIndex] = soundButtons.ToArray();

            if (Main.category == _soundboardCategoryIndex && Main.menu != null)
                Main.RecreateMenu();
        }

        private static void StopAllSoundButtons()
        {
            if (_soundboardCategoryIndex < 0) return;

            foreach (var b in Buttons.buttons[_soundboardCategoryIndex])
                if (b != null)
                    b.enabled = false;
        }

        public static void PlayFile(string filePath)
        {
            HaltAudio();

            AudioType type = AudioType.WAV;
            string ext = Path.GetExtension(filePath).ToLower();

            if (ext == ".ogg") type = AudioType.OGGVORBIS;
            else if (ext == ".mp3") type = AudioType.MPEG;

            EnsureAudioObj();
            _audioObj.GetComponent<MonoBehaviourHelper>()
                ?.StartCoroutine(LoadAndPlay(filePath, type));
        }

        private static IEnumerator LoadAndPlay(string filePath, AudioType type)
        {
            if (_clipCache.TryGetValue(filePath, out AudioClip cached))
            {
                PushToMic(cached);
                yield break;
            }

            using (var req = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath.Replace("\\", "/"), type))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    yield break;

                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                _clipCache[filePath] = clip;

                PushToMic(clip);
            }
        }

        private static void PushToMic(AudioClip clip)
        {
            if (clip == null) return;

            if (PhotonNetwork.InRoom)
            {
                if (HearSelf) PlayLocal(clip);

                try
                {
                    var recorder = GorillaTagger.Instance.myRecorder;
                    if (recorder != null)
                    {
                        recorder.SourceType = Recorder.InputSourceType.AudioClip;
                        recorder.AudioClip = clip;
                        recorder.LoopAudioClip = LoopAudio;
                        recorder.IsRecording = false;
                        recorder.RestartRecording(true);
                    }
                }
                catch
                {
                    PlayLocal(clip);
                }
            }
            else PlayLocal(clip);

            IsPlaying = true;
            RecoverTimer = Time.time + clip.length;
        }

        private static void PlayLocal(AudioClip clip)
        {
            EnsureAudioObj();
            _audioSource.clip = clip;
            _audioSource.loop = LoopAudio;
            _audioSource.Play();
        }

        public static void HaltAudio()
        {
            _audioSource?.Stop();

            if (PhotonNetwork.InRoom)
            {
                try
                {
                    var recorder = GorillaTagger.Instance.myRecorder;
                    if (recorder != null)
                    {
                        recorder.SourceType = Recorder.InputSourceType.Microphone;
                        recorder.AudioClip = null;
                        recorder.RestartRecording(true);
                    }
                }
                catch { }
            }

            StopAllSoundButtons();
            IsPlaying = false;
            RecoverTimer = -1f;
        }

        private static void EnsureAudioObj()
        {
            if (_audioObj == null)
            {
                _audioObj = new GameObject("LagMenuSoundboard");
                Object.DontDestroyOnLoad(_audioObj);
                _audioObj.AddComponent<MonoBehaviourHelper>();
                _audioSource = _audioObj.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 0f;
                _audioSource.playOnAwake = false;
            }
        }

        public static void RevealSoundFolder()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = SoundsFolder,
                    UseShellExecute = true
                });
            }
            catch { }
        }
    }

    public class MonoBehaviourHelper : MonoBehaviour { }
}
