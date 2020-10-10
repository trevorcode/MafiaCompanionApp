using MafiaApp.Server.Hubs;
using MafiaApp.Server.State;
using MafiaApp.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaApp.Server.Models
{
    public class Room
    {
        public Room()
        {
            RoomUsers = new List<RoomUser>();
            GameState = new GameState();
        }
        public string RoomId { get; set; }
        public List<RoomUser> RoomUsers { get; set; }
        public GameState GameState { get; set; }

        public IEnumerable<string> GetPlayers()
        {
            foreach (var p in RoomUsers)
            {
                yield return p.ConnectionId;
            }
        }

        public void RemoveRoomUserByConnectionId(string connectionId)
        {
            RoomUsers.Remove(RoomUsers.FirstOrDefault(m => m.ConnectionId == connectionId));
        }

        public void AddRoomUser(string playerName, string connectionId, bool isHost = false)
        {
            RoomUsers.Add(new RoomUser() { Name = playerName, ConnectionId = connectionId, IsHost = isHost });
        }

        //public async IClientProxy UpdateRoomState(GameHub gameHub)
        //{
        //    return gameHub.Clients.Users(this.GetPlayers().ToList());
        //}

        public RoomPayload ToRoomDTO()
        {
            return new RoomPayload() { RoomUsers = this.RoomUsers, RoomId = this.RoomId, GameState = this.GameState };
        }

        public async Task UpdateRoomStateAsync(IHubCallerClients clients)
        {
            await clients.Clients(this.GetPlayers().ToList()).SendAsync("UpdateRoomState", this.ToRoomDTO());
        }
    }

}
