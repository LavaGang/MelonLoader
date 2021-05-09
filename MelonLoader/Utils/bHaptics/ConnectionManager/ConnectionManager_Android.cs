#if __ANDROID__
using System;
using System.Collections.Generic;
using MelonLoader.bHapticsExtra;

namespace MelonLoader.ConnectionManager
{
    public class ConnectionManager : BaseConnectionManager
    {
        public new static bool IsConnectionManagerSupported => true;
    }
}
#endif