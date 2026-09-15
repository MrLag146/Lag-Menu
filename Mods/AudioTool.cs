using System.Linq;
using GorillaNetworking;
using UnityEngine;

namespace LagMenu.Mods
{
    public static class AudioTool
    {
        public static int micIndex;
        public static bool muted;
        public static float masterVolume = 1f;

        private static Photon.Voice.Unity.Recorder GetRecorder()
        {
            try
            {
                return NetworkSystem.Instance?.VoiceConnection?.PrimaryRecorder;
            }
            catch
            {
                return null;
            }
        }

        private static Photon.Voice.DeviceInfo[] GetMicrophones()
        {
            try
            {
                var recorder = GetRecorder();
                var enumerator = recorder?.MicrophonesEnumerator;

                if (enumerator == null)
                    return new Photon.Voice.DeviceInfo[0];

                return enumerator.ToArray();
            }
            catch
            {
                return new Photon.Voice.DeviceInfo[0];
            }
        }

        private static int MicCount()
        {
            return GetMicrophones().Length;
        }

        public static string CurrentMicName
        {
            get
            {
                var devices = GetMicrophones();

                if (devices.Length == 0)
                    return "No mic found";

                int idx = ((micIndex % devices.Length) + devices.Length) % devices.Length;

                return devices[idx].Name;
            }
        }

        public static void NextMic()
        {
            var devices = GetMicrophones();

            if (devices.Length == 0)
                return;

            micIndex = (micIndex + 1) % devices.Length;
            ApplyMic();
        }

        public static void PreviousMic()
        {
            var devices = GetMicrophones();

            if (devices.Length == 0)
                return;

            micIndex = ((micIndex - 1) % devices.Length + devices.Length) % devices.Length;
            ApplyMic();
        }

        public static void ApplyMic()
        {
            try
            {
                var recorder = GetRecorder();

                if (recorder == null)
                    return;

                var devices = GetMicrophones();

                if (devices.Length == 0)
                    return;

                int idx = ((micIndex % devices.Length) + devices.Length) % devices.Length;

                recorder.MicrophoneDevice = devices[idx];
                recorder.RestartRecording(true);
            }
            catch
            {
            }
        }

        public static void ToggleMute()
        {
            muted = !muted;
            AudioListener.volume = muted ? 0f : masterVolume;
        }

        public static void SetMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);

            if (!muted)
                AudioListener.volume = masterVolume;
        }
    }
}
