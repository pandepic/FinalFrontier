using FinalFrontier;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            try
            {
#endif
                using var game = new GameClient();
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
