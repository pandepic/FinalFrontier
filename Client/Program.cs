using FinalFrontier;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
#if RELEASE
            try
            {
#endif
                using var game = new GameClient();
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
