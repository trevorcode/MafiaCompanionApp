using MafiaApp.Shared;
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

        public List<string> Test { get; set; }
        public RoomDTO Room { get; private set; }

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();


        public GameConnection(NavigationManager _navMan)
        {
            navMan = _navMan;
            Test = new List<string>();

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

            var result = await hubConnection.InvokeAsync<RoomDTO>("JoinRoom", roomId, playerName);

            if (!(result is null))
            {
                Room = result;
                navMan.NavigateTo("/play");
                return true;
            }
            else
            {
                await hubConnection.StopAsync();
                return false;
            }


        }

        private void SetupEvents()
        {
            hubConnection.On<RoomDTO>("UpdateRoomState", (room) =>
            {
                Console.WriteLine("Here3");
                Room = room;
                NotifyStateChanged();
            });
        }

    }
}
