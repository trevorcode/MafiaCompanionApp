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
            if (room != null)
            {
                room.RemoveRoomUserByConnectionId(Context.ConnectionId);
                room.GameState.GameConfig.Roles.RemoveAt(0);

                await room.UpdateRoomStateAsync(Clients);
            }
            

            
        }

        public async Task<RoomPayload> JoinRoom(string roomId, string playerName)
        {
            if (hubState.GetRoomByRoomId(roomId) != null)
            {
                var room = hubState.GetRoomByRoomId(roomId);
                room.AddRoomUser(playerName, Context.ConnectionId);
                room.GameState.GameConfig.Roles.Add(RolesEnum.Citizen);

                await room.UpdateRoomStateAsync(Clients);

                return room.ToRoomDTO();
            }
            else
            {
                return null;
            }
        }

        public async Task<RoomPayload> CreateRoom(string playerName)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            hubState.Rooms.Add(new Room() { RoomId = finalString });

            if (hubState.GetRoomByRoomId(finalString) != null)
            {
                var room = hubState.GetRoomByRoomId(finalString);
                room.AddRoomUser(playerName, Context.ConnectionId, true);

                await room.UpdateRoomStateAsync(Clients);

                return room.ToRoomDTO();
            }
            else
            {
                return null;
            }
        }

        public async Task UpdateGameConfig(string roomId, GameConfiguration gameConfiguration)
        {
            if (hubState.GetRoomByRoomId(roomId) != null)
            {
                var room = hubState.GetRoomByRoomId(roomId);
                room.GameState.GameConfig = gameConfiguration;

                await room.UpdateRoomStateAsync(Clients);
            }
        }


    }
}
