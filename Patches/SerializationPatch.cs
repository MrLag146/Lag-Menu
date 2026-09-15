using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using HarmonyLib;
using Photon.Pun;

namespace LagMenu.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
    public class SerializationPatch
    {
        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Action SerializationPatch_Callback_01;

        public static Func<bool> SerializationPatch_State_01;

        public static event Action OnSerialization
        {
            [CompilerGenerated]
            add
            {
                Action oUHPQZPU = SerializationPatch_Callback_01;
                Action action = oUHPQZPU;
                Action action2 = action;
                Action value2 = (Action)Delegate.Combine(action2, value);
                oUHPQZPU = Interlocked.CompareExchange(ref SerializationPatch_Callback_01, value2, action2);
                action = oUHPQZPU;
                if ((object)action != action2)
                {
                    do
                    {
                        action2 = action;
                        value2 = (Action)Delegate.Combine(action2, value);
                        action = Interlocked.CompareExchange(ref SerializationPatch_Callback_01, value2, action2);
                    }
                    while ((object)action != action2);
                }
            }
            [CompilerGenerated]
            remove
            {
                Action oUHPQZPU = SerializationPatch_Callback_01;
                Action action = oUHPQZPU;
                Action action2 = action;
                Action value2 = (Action)Delegate.Remove(action2, value);
                oUHPQZPU = Interlocked.CompareExchange(ref SerializationPatch_Callback_01, value2, action2);
                action = oUHPQZPU;
                if ((object)action != action2)
                {
                    do
                    {
                        action2 = action;
                        value2 = (Action)Delegate.Remove(action2, value);
                        action = Interlocked.CompareExchange(ref SerializationPatch_Callback_01, value2, action2);
                    }
                    while ((object)action != action2);
                }
            }
        }
    }
}

