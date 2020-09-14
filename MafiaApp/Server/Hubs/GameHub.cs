using MafiaApp.Server.State;
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

        public async Task JoinRoom(string roomId, string playerName)
        {
            if (hubState.Rooms.Any(r => r.RoomId == roomId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                hubState.Rooms.FirstOrDefault(r=>r.RoomId == roomId).Players.Add(new Player { ConnectionId = Context.ConnectionId, Name = playerName });
                await Clients.Caller.SendAsync("RoomJoined", roomId);
            }
            else
            {
                await Clients.Caller.SendAsync("IncorrectRoomError");
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
