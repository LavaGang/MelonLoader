using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public enum IL2CPP_ProfileFlags
    {
        NONE = 0,
        APPDOMAIN_EVENTS = 1 << 0,
        ASSEMBLY_EVENTS = 1 << 1,
        MODULE_EVENTS = 1 << 2,
        CLASS_EVENTS = 1 << 3,
        JIT_COMPILATION = 1 << 4,
        INLINING = 1 << 5,
        EXCEPTIONS = 1 << 6,
        ALLOCATIONS = 1 << 7,
        GC = 1 << 8,
        THREADS = 1 << 9,
        REMOTING = 1 << 10,
        TRANSITIONS = 1 << 11,
        ENTER_LEAVE = 1 << 12,
        COVERAGE = 1 << 13,
        INS_COVERAGE = 1 << 14,
        STATISTICAL = 1 << 15,
        METHOD_EVENTS = 1 << 16,
        MONITOR_EVENTS = 1 << 17,
        IOMAP_EVENTS = 1 << 18, /* this should likely be removed, too */
        GC_MOVES = 1 << 19,
        FILEIO = 1 << 20
    }
}
