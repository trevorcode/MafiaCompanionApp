using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaApp.Shared
{ 
    public class GameState
    {
        public GameState()
        {
            State = GameStates.Lobby;
            GameConfig = new GameConfiguration();
            Players = new List<Player>();
            DayCount = 1;
        }

        public GameStates State { get; set; }
        public int DayCount { get; set; }
        public List<Player> Players { get; set; }

        public GameConfiguration GameConfig { get; set; }
    }

    public enum GameStates
    {
        Lobby,
        Day,
        Voting,
        Night,
        Victory,
        Defeat
    }

    public class Player
    {
        public Player()
        {
            Role = RolesEnum.Spectator;
        }
        public RoomUser RoomUser { get; set; }
        public RolesEnum Role { get; set; }
        public bool IsAlive { get; set; } = true;
    }

}
