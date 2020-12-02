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

        public async Task StartNewGame(string roomId)
        {
            var room = hubState.GetRoomByRoomId(roomId);

            if (room != null)
            {
                room.GameState.State = GameStates.Day;
                foreach (var ru in room.RoomUsers)
                {
                    var p = new Player();
                    p.RoomUser = ru;
                    room.GameState.Players.Add(p);
                }

                room.GameState.Players.FirstOrDefault(p => p.RoomUser.IsHost).Role = RolesEnum.Host;

                var listOfRemainingPlayers = room.GameState.Players.Where(p => p.Role != RolesEnum.Host).ToList();

                foreach (var r in room.GameState.GameConfig.Roles)
                {
                    Random random = new Random();
                    int listCount = listOfRemainingPlayers.Count;
                    int newRandom = random.Next(0, listCount);
                    listOfRemainingPlayers[newRandom].Role = r;
                    listOfRemainingPlayers.RemoveAt(newRandom);
                }

                await room.UpdateRoomStateAsync(Clients);
            }
        }

        public async Task NextGamePeriod(string roomId)
        {
            var room = hubState.GetRoomByRoomId(roomId);

            if (room.GameState.State == GameStates.Day)
            {
                if (room.GameState.DayCount == 1)
                {
                    room.GameState.State = GameStates.Night;
                }
                else
                {
                    room.GameState.State = GameStates.Voting;
                }

                room.GameState.Players.ForEach(p =>
                {
                    p.SelectedPlayer = null;
                });
            }
            else if (room.GameState.State == GameStates.Voting)
            {
                room.GameState.State = GameStates.Night;

                room.GameState.Players.ForEach(p =>
                {
                    p.SelectedPlayer = null;
                });
            }
            else if (room.GameState.State == GameStates.Night)
            {
                room.GameState.DayCount += 1;
                room.GameState.State = GameStates.Day;
            }

            await room.UpdateRoomStateAsync(Clients);
        }

        public async Task SelectPlayer(string roomId, Player p)
        {
            var room = hubState.GetRoomByRoomId(roomId);

            var player = room.GameState.Players.FirstOrDefault(r => r.RoomUser.ConnectionId == Context.ConnectionId);
            player.SelectedPlayer = p;

            await room.UpdateRoomStateAsync(Clients);
        }

        public async Task TogglePlayerDeadState(string roomId, Player p)
        {
            var room = hubState.GetRoomByRoomId(roomId);

            var player = room.GameState.Players.FirstOrDefault(r => p.RoomUser.ConnectionId == r.RoomUser.ConnectionId);
            player.IsAlive = !player.IsAlive;

            await room.UpdateRoomStateAsync(Clients);
        }

        public async Task EndGame(string roomId)
        {
            var room = hubState.GetRoomByRoomId(roomId);

            if (room != null)
            {
                room.GameState = new GameState();

                for(int i = 0; i<room.RoomUsers.Count-1; i++)
                {
                    room.GameState.GameConfig.Roles.Add(RolesEnum.Citizen);
                }

                await room.UpdateRoomStateAsync(Clients);
            }
        }

    }
}
