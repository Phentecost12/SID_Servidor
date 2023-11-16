using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Player
    {
        internal bool isJumping;
        public string Id { get; set; }
        public string Username { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int Radius { get; set; }
        public int Speed { get; set; }
        public int Score { get; set; }
        public bool CanJump { get; internal set; }
        public int Bullet_Dir_X { get; set; }
        public int Bullet_Dir_Y { get; set; }
        public bool Dead { get; set; }
        public int Skin { get; set; }
        public int Dir_X { get; set; }
    }
}
