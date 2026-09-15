using ExitGames.Client.Photon;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LagMenu
{
    internal class Serialize2
    {
        private static MethodInfo _onSerializeWrite;
        private static FieldInfo _serializeViewBatches;
        private static FieldInfo _serializeRaiseEvOptions;
        private static FieldInfo _currentLevelPrefix;
        private static Type _raiseEventBatchType;
        private static Type _serializeViewBatchType;
        private static FieldInfo _mixedModeIsReliable;
        private static ConstructorInfo _serializeViewBatchCtor;
        private static FieldInfo _objectUpdates;
        private static FieldInfo _batchField;
        private static FieldInfo _reliableField;
        private static MethodInfo _clearMethod;
        private static MethodInfo _addMethod;
        private static bool _initialized;

        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            Type pnType = typeof(PhotonNetwork);
            Type pvType = typeof(PhotonView);

            BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic |
                               BindingFlags.Static | BindingFlags.Instance;

            _onSerializeWrite = pnType.GetMethod("OnSerializeWrite", all);
            _serializeViewBatches = pnType.GetField("serializeViewBatches", all);
            _serializeRaiseEvOptions = pnType.GetField("serializeRaiseEvOptions", all);
            _currentLevelPrefix = pnType.GetField("currentLevelPrefix", all);
            _mixedModeIsReliable = pvType.GetField("mixedModeIsReliable", all);

            foreach (Type nested in pnType.GetNestedTypes(all))
            {
                if (nested.Name == "RaiseEventBatch")
                    _raiseEventBatchType = nested;
                if (nested.Name == "SerializeViewBatch")
                    _serializeViewBatchType = nested;
            }

            if (_serializeViewBatchType != null)
            {
                _serializeViewBatchCtor = _serializeViewBatchType.GetConstructors(all)[0];
                _objectUpdates = _serializeViewBatchType.GetField("ObjectUpdates", all);
                _batchField = _serializeViewBatchType.GetField("Batch", all);
                _clearMethod = _serializeViewBatchType.GetMethod("Clear", all);
                _addMethod = _serializeViewBatchType.GetMethod("Add", all);
            }

            if (_raiseEventBatchType != null)
                _reliableField = _raiseEventBatchType.GetField("Reliable", all);
        }

        public static void Serialize(PhotonView pv, RaiseEventOptions options = null,
                                     int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom || pv == null) return;

            try
            {
                Init();

                List<object> serializedData =
                    _onSerializeWrite?.Invoke(null, new object[] { pv }) as List<object>;
                if (serializedData == null) return;

                bool isMixedReliable = (bool)(_mixedModeIsReliable?.GetValue(pv) ?? false);
                bool isReliable = pv.Synchronization == ViewSynchronization.ReliableDeltaCompressed
                                       || isMixedReliable;

                object eventBatch = Activator.CreateInstance(_raiseEventBatchType);
                _reliableField?.SetValue(eventBatch, isReliable);

                FieldInfo groupField = _raiseEventBatchType.GetField("Group",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                groupField?.SetValue(eventBatch, pv.Group);

                System.Collections.IDictionary batchDict =
                    _serializeViewBatches?.GetValue(null) as System.Collections.IDictionary;
                if (batchDict == null) return;

                object viewBatch;
                if (!batchDict.Contains(eventBatch))
                {
                    viewBatch = _serializeViewBatchCtor.Invoke(new object[] { eventBatch, 2 });
                    batchDict[eventBatch] = viewBatch;
                }
                else
                {
                    viewBatch = batchDict[eventBatch];
                }

                _addMethod?.Invoke(viewBatch, new object[] { serializedData });

                RaiseEventOptions defaultOptions =
                    _serializeRaiseEvOptions?.GetValue(null) as RaiseEventOptions
                    ?? new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                RaiseEventOptions finalOptions = options != null
                    ? new RaiseEventOptions
                    {
                        CachingOption = defaultOptions.CachingOption,
                        Flags = defaultOptions.Flags,
                        InterestGroup = defaultOptions.InterestGroup,
                        TargetActors = options.TargetActors,
                        Receivers = options.Receivers
                    }
                    : defaultOptions;

                List<object> updateData = _objectUpdates?.GetValue(viewBatch) as List<object>;
                if (updateData == null) return;

                int levelPrefix = (int)(_currentLevelPrefix?.GetValue(null) ?? 0);
                updateData[0] = PhotonNetwork.ServerTimestamp + timeOffset;
                updateData[1] = levelPrefix != 0 ? (object)levelPrefix : null;

                object batchObj = _batchField?.GetValue(viewBatch);
                bool reliable = batchObj != null && (bool)(_reliableField?.GetValue(batchObj) ?? false);
                byte eventCode = reliable ? (byte)206 : (byte)201;
                SendOptions send = reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable;

                if (delay <= 0f)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, updateData, finalOptions, send);
                }
                else
                {
                    CoroutineManager.instance.StartCoroutine(
                        SerializeDelay(() =>
                            PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, updateData, finalOptions, send),
                        delay));
                }

                _clearMethod?.Invoke(viewBatch, null);
            }
            catch (Exception e)
            {
                Debug.LogError("[LagMenu] Serialize2 error: " + e);
            }
        }

        public static IEnumerator SerializeDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
