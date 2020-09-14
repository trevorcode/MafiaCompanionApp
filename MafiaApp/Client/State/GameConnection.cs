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
        private string RoomId { get; set; }

        public GameConnection(NavigationManager _navMan)
        {
            navMan = _navMan;
            Test = new List<string>();

            hubConnection = new HubConnectionBuilder()
                .WithUrl(navMan.ToAbsoluteUri($"/gameHub"))
                .Build();

            SetupEvents();
        }

        public async Task Connect(string roomId, string playerName)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            await hubConnection.SendAsync("JoinRoom", roomId, playerName);


        }

        private void SetupEvents()
        {
            hubConnection.On("IncorrectRoomError", () =>
            {
                Console.WriteLine("Here3");
                Test.Add("Error: No room with that code");
                //hubConnection.StopAsync();
            });

            hubConnection.On<string>("RoomJoined", (roomId) =>
            {
                RoomId = roomId;
            });
        }

        public bool IsConnectedWithRoomId()
        {
            if (!string.IsNullOrEmpty(RoomId) && hubConnection.State == HubConnectionState.Connected)
            {
                Console.WriteLine("Here4");
                navMan.NavigateTo("play");
                return true;
            }
            return false;
        }
    }
}
