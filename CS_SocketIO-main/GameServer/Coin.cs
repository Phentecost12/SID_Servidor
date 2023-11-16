using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Bullet
    {
        public Player pl { get; set; }
        public string id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int WorldWidth { get; set; }
        public int Dir_X { get;set; }
        public int Dir_Y { get;set; }
        public int Radius { get; set; }
        public int Speed { get; set; }
        public bool Taken { get; set; }

        public void Move_Bullet() 
        {
            x += Speed * Dir_X;
            y += Speed * Dir_Y;
        }

        public bool Take(Player player)
        {
            if (!Taken)
            {
                if (pl == player) return false;
                var dx = player.x - x;
                var dy = player.y - y;
                var rSum = Radius + player.Radius;
                
                return dx*dx + dy*dy <= rSum*rSum;
            }
            else
            { return false; }
        }
    }
}
