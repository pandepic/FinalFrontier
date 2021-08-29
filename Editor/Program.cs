using System;

namespace Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            using var game = new GameEditor();
            game.Run();
        }
    }
}
