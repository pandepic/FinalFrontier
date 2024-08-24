using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public interface INetworkGameState
    {
        public void OnServerDisconnected();
    }
}
