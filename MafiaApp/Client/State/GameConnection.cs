﻿using MafiaApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
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

    }
}
