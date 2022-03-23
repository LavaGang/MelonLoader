using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<void> PreStart;
        public delegate* unmanaged[Stdcall]<void> Start;
    }
}
