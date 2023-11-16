using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    [Serializable]
    public class Axis
    {
        public int Vertical;
        public int Horizontal;
    }

    public class GameState
    {
        public List<Player> Players { get; set; }
        public List<Bullet> Bullets { get; set; }
        public GameState()
        {
            Players = new List<Player>();
            Bullets = new List<Bullet>();
        }
    }

    public class Plat 
    {
        public int pos_X, pos_Y, Scale_X,Scale_Y;
    }
    internal class Game
    {
        const int WorldWidth = 500;
        const int WorldHeigh = 400;
        const int LoopPeriod = 10;
        const int MaxCoins = 15;
        const int Gravity = 5;
        const int JumpForce = 10;
        public GameState State { get; set; }

        private  Dictionary<string, Axis> Axes;
        private Plat[] plats;
        public Game()
        {
            State = new GameState();
            Axes = new Dictionary<string, Axis>();
            plats = new Plat[5];
            plats[0] = new Plat {pos_X = 100, pos_Y = 50, Scale_X = 100, Scale_Y = 10 };
            plats[1] = new Plat { pos_X = 400, pos_Y = 100, Scale_X = 100, Scale_Y = 10 };
            plats[2] = new Plat { pos_X = 150, pos_Y = 150, Scale_X = 100, Scale_Y = 10 };
            plats[3] = new Plat { pos_X = 350, pos_Y = 275, Scale_X = 100, Scale_Y = 10 };
            plats[4] = new Plat { pos_X = 100, pos_Y = 300, Scale_X = 100, Scale_Y = 10 };

            StartGameLoop();
        }
        public void SpawnPlayer(string id,string username, int skin)
        {
            Random random = new Random();
            State.Players.Add(new Player()
            {
                Id = id,
                Username = username,
                x = random.Next(10, WorldWidth - 10),
                y = random.Next(10, WorldHeigh - 10),
                Speed = 2,
                Radius = 10,
                Skin = skin
            });

            Axes[id] = new Axis{Horizontal= 0,Vertical= 0 };

        }

        public void SetAxis(string id,Axis axis)
        {
            Axes[id]= axis;
        }

        public void Update()
        {
            List<string> takedCoinsIds= new List<string>();

            foreach (var player in State.Players)
            {
                if (player.Dead) continue;

                var axis = Axes[player.Id];

                if (axis.Horizontal > 0 && player.x < WorldWidth - player.Radius)
                {
                    player.x += player.Speed;
                    player.Dir_X = 1;
                }
                else if (axis.Horizontal < 0 && player.x > 0 + player.Radius)
                {
                    player.x -= player.Speed;
                    player.Dir_X = -1;
                }
                else 
                {
                    player.Dir_X= 0;
                }
                if (axis.Vertical > 0 && player.y < WorldHeigh - player.Radius)
                {
                    player.y += player.Speed;
                }
                else if (axis.Vertical < 0 && player.y > 0 + player.Radius)
                {
                    player.y -= player.Speed;
                }

                #region GRAVEDAD y Saltos
                if (!player.isJumping && player.y > 0 + player.Radius)
                {
                    player.y -= Gravity;
                }
                if (player.y < 0 + player.Radius)
                {
                    player.y = 0 + player.Radius;
                    player.isJumping = false; // El jugador ha vuelto al suelo y no está saltando
                }
                if (axis.Vertical > 0 && !player.isJumping)
                {
                    player.y += JumpForce;
                    player.isJumping = true;

                }
                else
                {
                    player.isJumping = false;
                }

                foreach (Plat platform in plats)
                {
                    if (axis.Vertical == 0 && player.y < platform.pos_Y + player.Radius && player.y > platform.pos_Y && player.x < platform.pos_X + platform.Scale_X / 2 && player.x > platform.pos_X - platform.Scale_X / 2)
                    {
                        player.y = platform.pos_Y + player.Radius;
                        player.isJumping = false;
                    }
                }

                #endregion

                //Console.WriteLine(State.Bullets.Count);

                if (State.Bullets.Count > 0) 
                {
                    foreach(Bullet bullet in State.Bullets) 
                    {
                        bullet.Move_Bullet();

                        if(bullet.x < 0 + bullet.Radius || bullet.x > WorldWidth - bullet.Radius || bullet.y < 0 + bullet.Radius || bullet.y > WorldHeigh-bullet.Radius) 
                        {
                            bullet.Taken = true;
                        }
                    }

                    State.Bullets = State.Bullets.Where(bullet => {
                        if (bullet.Take(player))
                        {
                            killPlayer(player);
                            bullet.pl.Score++;
                            bullet.Taken = true;
                            Console.WriteLine(player.Username + ":" + bullet.pl.Score);
                            return false;
                        }
                        else if (bullet.Taken)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }).ToList();
                }       
            }
        }

        public void RemovePlayer(string id)
        {
            State.Players = State.Players.Where(player => player.Id != id).ToList();
            Axes.Remove(id);
        }

        public void killPlayer(Player player)
        {
            player.Dead = true;
            Revive_Player(player);

        }

        async Task Revive_Player(Player player) 
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            Random random = new Random();
            player.x = random.Next(10, WorldWidth - 10);
            player.y = random.Next(10, WorldHeigh - 10);
            player.Dead = false;
        }

        async Task StartGameLoop()
        {
            while (true)
            {
                Update();
                await Task.Delay(TimeSpan.FromMilliseconds(LoopPeriod)); 
            }
        }

        public void SpawnBullet(Player player)
        {
            Random random = new Random();

            if (State.Bullets.Count <= MaxCoins) {
                Bullet coin = new Bullet {
                    pl = player,
                    id = Guid.NewGuid().ToString(),
                    x = player.x,
                    y = player.y,
                    Radius = 10,
                    Speed = 10,
                    Dir_X = player.Bullet_Dir_X,
                    Dir_Y = player.Bullet_Dir_Y
                };
                State.Bullets.Add(coin);

                Console.WriteLine(player.Username + " Super_XD");
            }
        }

    }
}
