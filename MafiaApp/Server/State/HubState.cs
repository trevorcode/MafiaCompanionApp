using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaApp.Server.State
{
    public class HubState
    {
        public HubState()
        {
            Rooms = new List<Room>();
            Rooms.Add(new Room { RoomId = "GARF" });
        }

        public List<Room> Rooms { get; set; }

        
    }

    public class Room
    {
        public Room()
        {
            Players = new List<Player>();
        }
        public string RoomId { get; set; }
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}
