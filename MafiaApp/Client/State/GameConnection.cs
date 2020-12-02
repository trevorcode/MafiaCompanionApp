using MafiaApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MafiaApp.Client.State
{
    public class GameConnection
    {
        private HubConnection hubConnection;
        private NavigationManager navMan;

        public RoomPayload Room { get; private set; }

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        
        public GameConnection(NavigationManager _navMan)
        {
            navMan = _navMan;

            hubConnection = new HubConnectionBuilder()
                .WithUrl(navMan.ToAbsoluteUri($"/gameHub"))
                .Build();

            SetupEvents();
        }

        public async Task Reset()
        {
            Room = null;
            await hubConnection.StopAsync();
        }

        public async Task<bool> Connect(string roomId, string playerName)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            var result = await hubConnection.InvokeAsync<RoomPayload>("JoinRoom", roomId, playerName);

            if (!(result is null))
            {
                Room = result;
                
                return true;
            }
            else
            {
                await hubConnection.StopAsync();
                return false;
            }

        }

        public async Task<bool> CreateRoomAndConnect(string playerName)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            var result = await hubConnection.InvokeAsync<RoomPayload>("CreateRoom", playerName);

            if (!(result is null))
            {
                Room = result;
                return true;
            }
            else
            {
                await hubConnection.StopAsync();
                return false;
            }

        }

        public async Task UpdateGameConfig()
        {
            await hubConnection.InvokeAsync("UpdateGameConfig", Room.RoomId, Room.GameState.GameConfig);
        }

        public async Task StartNewGame()
        {
            await hubConnection.InvokeAsync("StartNewGame", Room.RoomId);
        }

        public async Task GoToNextGamePeriod()
        {
            await hubConnection.InvokeAsync("NextGamePeriod", Room.RoomId);
        }
        public async Task SelectPlayer(Player p)
        {
            await hubConnection.InvokeAsync("SelectPlayer", Room.RoomId, p);
        }

        public async Task ToggleDead(Player p)
        {
            await hubConnection.InvokeAsync("TogglePlayerDeadState", Room.RoomId, p);
        }

        public async Task EndGame()
        {
            await hubConnection.InvokeAsync("EndGame", Room.RoomId);
        }

        private void SetupEvents()
        {
            hubConnection.On<RoomPayload>("UpdateRoomState", (room) =>
            {
                Room = room;
                NotifyStateChanged();
            });
        }

        public string GetConnectionId()
        {
            return hubConnection.ConnectionId;
        }

        public Player GetUserSelf()
        {
            return this.Room.GameState.Players.FirstOrDefault(r => r.RoomUser.ConnectionId == this.GetConnectionId());
        }
    }
}
