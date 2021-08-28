using FinalFrontier;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
#if RELEASE
            try
            {
#endif
                using var game = new GameServer();
                game.Run();
#if RELEASE
            }
            catch (Exception ex)
            {
                ElementEngine.Logging.Error(ex.ToString());
            }
#endif
        }
    }
}
