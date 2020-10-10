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
        }

        public List<Room> Rooms { get; set; }

        public Room GetRoomByConnectionId(string connectionId)
        {
            return Rooms.FirstOrDefault(m => m.RoomUsers.Any(p=>p.ConnectionId == connectionId));
        }

        public Room GetRoomByRoomId(string roomId)
        {
            return Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }
    }


}
