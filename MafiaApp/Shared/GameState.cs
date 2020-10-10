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
        }

        public GameStates State { get; set; }

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
}
