using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public interface IPacketData
    {
        public void Write(NetworkPacket packet);
    }
}
