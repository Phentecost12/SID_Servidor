
using CS_SocketIO;
using GameServer;

const int SERVER_TIME_STEP = 30;
ServerUdp io = new ServerUdp(11000);
Game game = new Game();


io.On("connection",onConnection);


UpdateState();

void onConnection(object _client)
{
    Client client = (Client)_client;
    string username = ((dynamic)client.Data)?.Username;
    string Skin = ((dynamic)client.Data)?.Skin;

    if (string.IsNullOrEmpty(username))
    {
        client.Disconnect("Debe enviar el username en la data inicial");
        return;
    }

    Console.WriteLine("Cliente conectado " + username);
    
    game.SpawnPlayer(client.Id,username, int.Parse(Skin));

    client.Emit("welcome", new {
            Message="Bienvenido al juego",
            Id = client.Id,
            State =game.State,
        });
    client.Broadcast("newPlayer",new { Id=client.Id , Username=username });
    client.On("move", (axis) => {
        int  horizontal = ((dynamic)axis).Horizontal;
        int vertical = ((dynamic)axis).Vertical;

        game.SetAxis(client.Id,new Axis {Horizontal=horizontal,Vertical = vertical  });
    });

    client.On("Shoot", (Shoot) =>
    {
        int horizontal = ((dynamic)Shoot).x;
        int vertical = ((dynamic)Shoot).y;
        string username = ((dynamic)Shoot).username;

        if (horizontal < -50) 
        {
            horizontal = -1;
        }
        else if (horizontal > 50) 
        {
            horizontal = 1;
        }
        else 
        {
            horizontal = 0;
        }

        if (vertical < -50)
        {
            vertical = -1;
        }
        else if (vertical > 50)
        {
            vertical = 1;
        }
        else
        {
            vertical = 0;
        }


        Player p = new Player();

        foreach(Player player in game.State.Players) 
        {
            if(Equals(player.Username,username)) 
            {
                p = player;
                break;
            }
        }

        p.Bullet_Dir_X = horizontal;
        p.Bullet_Dir_Y = vertical;
        game.SpawnBullet(p);
    });



    client.On("disconnect", (_client) =>
    {
        game.RemovePlayer(client.Id);

         Console.WriteLine("usuario desconectado " + client.Id);
    });
}
io.Listen();



 async Task UpdateState()
{
    while (true)
    {
        io.Emit("updateState", new { State = game.State });
        await Task.Delay(TimeSpan.FromMilliseconds(SERVER_TIME_STEP));
    }
}
