using MafiaApp.Server.Models;
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
            var room = hubState.GetRoomByConnectionId(Context.ConnectionId);
            room.RemovePlayerByConnectionId(Context.ConnectionId);

            await room.UpdateRoomStateAsync(Clients);
        }

        public async Task<RoomDTO> JoinRoom(string roomId, string playerName)
        {
            if (hubState.GetRoomByRoomId(roomId) != null)
            {
                var room = hubState.GetRoomByRoomId(roomId);
                room.AddPlayer(playerName, Context.ConnectionId);

                //await Clients.Clients(room.GetPlayers().ToList()).SendAsync("UpdateRoomState", room.ToRoomDTO());
                //await Clients.All.SendAsync("UpdateRoomState", room.ToRoomDTO());

                await room.UpdateRoomStateAsync(Clients);

                return room.ToRoomDTO();
            }
            else
            {
                return null;
            }
        }

        //public async Task<RoomDTO> CreateRoom()
        //{
        //    hubState.
        //}



    }
}
