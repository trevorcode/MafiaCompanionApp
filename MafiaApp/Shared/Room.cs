﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaApp.Shared
{
    public class Room
    {
        public Room()
        {
            Players = new List<Player>();
        }
        public string RoomId { get; set; }
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}
