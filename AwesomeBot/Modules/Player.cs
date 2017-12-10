using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeBot.Modules
{
    public class Player
    {
        public SocketGuildUser User { get; set; }
        public int Points = 0;

        public Player(SocketGuildUser user)
        {
            User = user;
        }

        public void AddPoints(int points)
        {
            Points += points;
            //Save Points
        }

        public int CurrentPoints
        {
            get
            {
                return Points;
            }
        }

        public int GetPlayerPoints(Player player)
        {
            return player.Points;
        }
    }
}
