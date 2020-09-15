using MafiaApp.Server.State;
using MafiaApp.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaApp.Server.Hubs
{
    public class GameHub : Hub
    {
        private readonly HubState hubState;

        public GameHub(HubState _hubState)
        {
            hubState = _hubState;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var room = hubState.Rooms.FirstOrDefault(m => m.Players.Any(p => p.ConnectionId == Context.ConnectionId));
            room.Players.Remove(room.Players.FirstOrDefault(m => m.ConnectionId == Context.ConnectionId));

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomId);

            await Clients.Group(room.RoomId).SendAsync("UpdateRoomState", room);
        }

        public async Task<Room> JoinRoom(string roomId, string playerName)
        {
            if (hubState.Rooms.Any(r => r.RoomId == roomId))
            {
                var room = hubState.Rooms.FirstOrDefault(r => r.RoomId == roomId);
                room.Players.Add(new Player { ConnectionId = Context.ConnectionId, Name = playerName });
                await Clients.Group(room.RoomId).SendAsync("UpdateRoomState", room);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                return room;
            }
            else
            {
                return null;
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
