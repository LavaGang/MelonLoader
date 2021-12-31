using System;

namespace UnhollowerMini
{
    internal class ObjectCollectedException : Exception
    {
        public ObjectCollectedException(string message) : base(message) { }
    }
}
