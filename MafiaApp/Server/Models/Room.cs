using MafiaApp.Server.Hubs;
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
            Players = new List<Player>();
        }
        public string RoomId { get; set; }
        public List<Player> Players { get; set; }

        public IEnumerable<string> GetPlayers()
        {
            foreach (var p in Players)
            {
                yield return p.ConnectionId;
            }
        }

        public void RemovePlayerByConnectionId(string connectionId)
        {
            Players.Remove(Players.FirstOrDefault(m => m.ConnectionId == connectionId));
        }

        public void AddPlayer(string playerName, string connectionId)
        {
            Players.Add(new Player() { Name = playerName, ConnectionId = connectionId });
        }

        //public async IClientProxy UpdateRoomState(GameHub gameHub)
        //{
        //    return gameHub.Clients.Users(this.GetPlayers().ToList());
        //}

        public RoomDTO ToRoomDTO()
        {
            return new RoomDTO() { Players = this.Players, RoomId = this.RoomId };
        }

        public async Task UpdateRoomStateAsync(IHubCallerClients clients)
        {
            await clients.Clients(this.GetPlayers().ToList()).SendAsync("UpdateRoomState", this.ToRoomDTO());
        }
    }

}
