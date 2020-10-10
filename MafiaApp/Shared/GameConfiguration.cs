using MafiaApp.Shared.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MafiaApp.Shared
{
    public class GameConfiguration
    {
        public GameConfiguration()
        {
            Roles = new List<RolesEnum>();
        }
        public List<RolesEnum> Roles { get; set; }
    }
}
