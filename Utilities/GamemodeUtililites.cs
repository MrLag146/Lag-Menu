using GorillaGameModes;
using GorillaTagScripts;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

namespace LagMenu.Utilities
{
    public class GameModeUtilities
    {
        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer>();

            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return infected;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        infected.Add(tagManager.currentIt);
                    else
                        infected.AddRange(tagManager.currentInfected);
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        infected.Add(ghostManager.currentIt);
                    else
                        infected.AddRange(ghostManager.currentInfected);
                    break;

                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;

                    infected.AddRange(
                        paintbrawlManager.playerLives
                        .Where(e => e.Value <= 0)
                        .Select(e => PhotonNetwork.CurrentRoom.GetPlayer(e.Key))
                        .Cast<NetPlayer>()
                    );

                    if (!infected.Contains(NetworkSystem.Instance.LocalPlayer))
                        infected.Add(NetworkSystem.Instance.LocalPlayer);

                    break;
            }

            return infected;
        }

        public static void AddInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;

                    if (tagManager.isCurrentlyTag)
                        tagManager.currentIt = plr;
                    else if (!tagManager.currentInfected.Contains(plr))
                        tagManager.AddInfectedPlayer(plr);
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;

                    if (ghostManager.isCurrentlyTag)
                        ghostManager.currentIt = plr;
                    else if (!ghostManager.currentInfected.Contains(plr))
                        ghostManager.AddInfectedPlayer(plr);
                    break;

                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 0;
                    break;
            }
        }

        public static void RemoveInfected(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;

                    if (tagManager.isCurrentlyTag)
                    {
                        if (tagManager.currentIt == plr)
                            tagManager.currentIt = null;
                    }
                    else if (tagManager.currentInfected.Contains(plr))
                    {
                        tagManager.currentInfected.Remove(plr);
                    }
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;

                    if (ghostManager.isCurrentlyTag)
                    {
                        if (ghostManager.currentIt == plr)
                            ghostManager.currentIt = null;
                    }
                    else if (ghostManager.currentInfected.Contains(plr))
                    {
                        ghostManager.currentInfected.Remove(plr);
                    }
                    break;

                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;
                    paintbrawlManager.playerLives[plr.ActorNumber] = 3;
                    break;
            }
        }

        public static void AddRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    ((GorillaTagManager)GorillaGameManager.instance).currentIt = plr;
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    ((GorillaAmbushManager)GorillaGameManager.instance).currentIt = plr;
                    break;

                case GameModeType.Paintbrawl:
                    ((GorillaPaintbrawlManager)GorillaGameManager.instance).playerLives[plr.ActorNumber] = 0;
                    break;
            }
        }

        public static void RemoveRock(NetPlayer plr)
        {
            if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
                return;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.currentIt == plr)
                        tagManager.currentIt = null;
                    break;

                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.currentIt == plr)
                        ghostManager.currentIt = null;
                    break;

                case GameModeType.Paintbrawl:
                    ((GorillaPaintbrawlManager)GorillaGameManager.instance).playerLives[plr.ActorNumber] = 3;
                    break;
            }
        }
    }
}
