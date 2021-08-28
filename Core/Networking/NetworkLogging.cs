using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public static class NetworkLogging
    {
        [Conditional("DEBUG")]
        public static void Client(string message)
        {
            Console.WriteLine(message);
        }
        
        public static void Server(string message)
        {
            Console.WriteLine(message);
        }
    }
}
