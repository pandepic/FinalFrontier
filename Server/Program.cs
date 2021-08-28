using FinalFrontier;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            try
            {
#endif
                using var game = new GameServer();
                game.Run();
#if DEBUG
            }
            catch (Exception ex)
            {
                ElementEngine.Logging.Error(ex.ToString());
            }
#endif
        }
    }
}
