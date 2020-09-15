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
        }

        public List<Room> Rooms { get; set; }

        
    }


}
