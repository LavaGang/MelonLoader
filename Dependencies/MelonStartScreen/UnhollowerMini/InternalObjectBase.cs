using System;

namespace UnhollowerMini
{
    internal class InternalObjectBase
    {
        public IntPtr Pointer
        {
            get
            {
                var handleTarget = UnityInternals.gchandle_get_target(myGcHandle);
                if (handleTarget == IntPtr.Zero) throw new ObjectCollectedException("Object was garbage collected");
                return handleTarget;
            }
        }

        protected uint myGcHandle;

        protected InternalObjectBase() { }

        public InternalObjectBase(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new NullReferenceException();

            myGcHandle = UnityInternals.gchandle_new(pointer, false);
        }

        ~InternalObjectBase()
        {
            UnityInternals.gchandle_free(myGcHandle);
        }
    }
}
