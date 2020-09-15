using MafiaApp.Server.Models;
using MafiaApp.Shared;
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
            Rooms.Add(new Room { RoomId = "CART" });
        }

        public List<Room> Rooms { get; set; }

        public Room GetRoomByConnectionId(string connectionId)
        {
            return Rooms.FirstOrDefault(m => m.Players.Any(p=>p.ConnectionId == connectionId));
        }

        public Room GetRoomByRoomId(string roomId)
        {
            return Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }
    }


}
