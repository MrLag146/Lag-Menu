using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace LagMenu.Utilities
{
    public class Useful
    {
        public static string GetGameMode()
        {
            Room currentRoom = PhotonNetwork.CurrentRoom;
            if (currentRoom?.CustomProperties == null || !((Dictionary<object, object>)(object)currentRoom.CustomProperties).ContainsKey("gameMode"))
            {
                return "ERROR";
            }
            return currentRoom.CustomProperties["gameMode"].ToString();
        }
    }
}