using ExitGames.Client.Photon;
using LagMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LagMenu.Menu
{
    internal class Seralizeing
    {
        public static void SendSerialize(PhotonView pv, RaiseEventOptions options = null, int timeOffset = 0, float delay = 0f)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (pv == null)
            {
                Debug.LogError("PhotonView is null. Cannot serialize.");
                return;
            }

            List<object> serializedData = PhotonNetwork.OnSerializeWrite(pv);
            if (serializedData == null || serializedData.Count == 0)
                return;

            PhotonNetwork.RaiseEventBatch raiseEventBatch = new PhotonNetwork.RaiseEventBatch();

            bool mixedReliable = pv.mixedModeIsReliable;
            raiseEventBatch.Reliable = pv.Synchronization == ViewSynchronization.ReliableDeltaCompressed || mixedReliable;
            raiseEventBatch.Group = pv.Group;

            IDictionary dictionary = PhotonNetwork.serializeViewBatches;

            PhotonNetwork.SerializeViewBatch serializeViewBatch = new PhotonNetwork.SerializeViewBatch(raiseEventBatch, 2);

            if (!dictionary.Contains(raiseEventBatch))
                dictionary[raiseEventBatch] = serializeViewBatch;

            serializeViewBatch.Add(serializedData);

            RaiseEventOptions sendOptions = PhotonNetwork.serializeRaiseEvOptions;
            RaiseEventOptions finalOptions = options != null ? new RaiseEventOptions
            {
                CachingOption = sendOptions.CachingOption,
                Flags = sendOptions.Flags,
                InterestGroup = sendOptions.InterestGroup,
                TargetActors = options.TargetActors,
                Receivers = options.Receivers
            } : sendOptions;

            bool reliable = serializeViewBatch.Batch.Reliable;
            List<object> objectUpdate = serializeViewBatch.ObjectUpdates;
            byte currentLevelPrefix = PhotonNetwork.currentLevelPrefix;

            objectUpdate[0] = PhotonNetwork.ServerTimestamp + timeOffset;
            objectUpdate[1] = currentLevelPrefix != 0 ? (object)currentLevelPrefix : null;

            if (delay <= 0f)
                PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? Photon.Pun.PunEvent.SendSerializeReliable : Photon.Pun.PunEvent.SendSerialize), objectUpdate, finalOptions,
                    reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            else
            {
                objectUpdate = new List<object>(objectUpdate);
                CoroutineManager.instance.StartCoroutine(SerializationDelay(() =>
                    PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? Photon.Pun.PunEvent.SendSerializeReliable : Photon.Pun.PunEvent.SendSerialize), objectUpdate, finalOptions,
                        reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable), delay));
            }

            serializeViewBatch.Clear();
        }

        public static IEnumerator SerializationDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
