using GorillaNetworking;
using LagMenu.Utilities;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LagMenu.Mods
{
    internal class Room
    {
        public static int reconnectDelay = 1;

        public static IEnumerator QueueRoomCoroutine(string roomName)
        {
            NetworkSystemPUN instance = (NetworkSystemPUN)NetworkSystem.Instance;

            instance.ReturnToSinglePlayer();
            yield return new WaitUntil(() => instance.netState == NetSystemState.Idle);
            yield return new WaitForSeconds(0.5f);

            while (!instance.InRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, JoinType.Solo);
                yield return new WaitForSeconds(reconnectDelay);
            }
        }

        public static void Reconnect()
        {
            string roomName = NetworkSystem.Instance.RoomName;
            NetworkSystem.Instance.ReturnToSinglePlayer();
            QueueRoom(roomName);
        }



        public static void JoinRandom()
        {
            if (PhotonNetwork.InRoom)
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();
                CoroutineManager.instance.StartCoroutine(JoinRandomDelay());
                return;
            }

            GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger ?? GorillaComputer.instance.GetJoinTriggerForZone("forest");
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(trigger);
        }

        public static IEnumerator JoinRandomDelay()
        {
            yield return new WaitForSeconds(1.5f);
            JoinRandom();
        }



        public static void QueueRoom(string roomName)
        {
            if (queueCoroutine != null)
                CoroutineManager.instance.StopCoroutine(queueCoroutine);

            queueCoroutine = CoroutineManager.instance.StartCoroutine(QueueRoomCoroutine(roomName));
        }


        public static void JoinRoom(string roomName)
        {
            if (string.IsNullOrWhiteSpace(roomName))
                return;

            roomName = roomName.ToUpper().Trim();


            if (NetworkSystem.Instance.InRoom &&
                string.Equals(NetworkSystem.Instance.RoomName, roomName,
                    StringComparison.OrdinalIgnoreCase))
                return;

            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, 0);
        }

        public static Coroutine queueCoroutine;

        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }



        public static void JoinLag12()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("LAG12", JoinType.Solo);
        }





    }
}
