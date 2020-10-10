using System.Collections.Generic;
using System.Linq;

namespace MafiaApp.Shared
{
    public class RoomPayload
    {
        public RoomPayload()
        {

        }

        public string RoomId { get; set; }
        public List<RoomUser> RoomUsers { get; set; }
        public GameState GameState { get; set; }

        public bool IsHost(string connectionId)
        {
            return RoomUsers.FirstOrDefault(m => m.ConnectionId == connectionId).IsHost;
        }
    }


}