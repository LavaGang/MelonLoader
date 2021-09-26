using System;

namespace UnhollowerMini
{
    internal class Il2CppObjectBase
    {
        public IntPtr Pointer
        {
            get
            {
                var handleTarget = IL2CPP.il2cpp_gchandle_get_target(myGcHandle);
                if (handleTarget == IntPtr.Zero) throw new ObjectCollectedException("Object was garbage collected in IL2CPP domain");
                return handleTarget;
            }
        }

        protected uint myGcHandle;

        protected Il2CppObjectBase() { }

        public Il2CppObjectBase(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                throw new NullReferenceException();

            myGcHandle = IL2CPP.il2cpp_gchandle_new(pointer, false);
        }

        public bool WasCollected =>
            IL2CPP.il2cpp_gchandle_get_target(myGcHandle) == IntPtr.Zero;


        ~Il2CppObjectBase()
        {
            IL2CPP.il2cpp_gchandle_free(myGcHandle);
        }
    }
}
