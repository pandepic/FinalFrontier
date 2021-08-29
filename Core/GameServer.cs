using ElementEngine;
using FinalFrontier.Networking;

namespace FinalFrontier
{
    public class GameServer : BaseGame
    {
        public NetworkServer NetworkServer;

        public override void Load()
        {
            var windowRect = new ElementEngine.Rectangle()
            {
                X = 100,
                Y = 100,
                Width = 1600,
                Height = 900
            };

            SetupWindow(windowRect, "SERVER: Final Frontier", vsync: true, windowState: Veldrid.WindowState.Normal);
            Window.Resizable = false;

            NetworkServer = new NetworkServer();
            NetworkServer.Load();
        }

        public override void Update(GameTimer gameTimer)
        {
            NetworkServer.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

        public override void Exit()
        {
        }

    } // GameServer
}
