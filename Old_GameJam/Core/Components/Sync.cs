using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct TempSync<T> where T : struct { }
    public struct PlayerJoinedSync<T> where T : struct { }
    public struct EveryFrameSync<T> where T : struct { }
}
