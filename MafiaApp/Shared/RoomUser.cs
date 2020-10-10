using MafiaApp.Shared.Roles;

namespace MafiaApp.Shared
{
    public class RoomUser
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public bool IsHost { get; set; } = false;
    }
}