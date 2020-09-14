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
        public string RoomId { get; private set; }

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
            RoomId = "";
            await hubConnection.StopAsync();
        }

        public async Task<bool> Connect(string roomId, string playerName)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            var result = await hubConnection.InvokeAsync<string>("JoinRoom", roomId, playerName);

            if (!string.IsNullOrEmpty(result))
            {
                RoomId = result;
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
            //hubConnection.On("IncorrectRoomError", () =>
            //{
                
            //    //hubConnection.StopAsync();
            //});
        }

    }
}
