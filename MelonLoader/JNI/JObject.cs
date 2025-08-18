#if ANDROID
using System;

namespace MelonLoader.Java;

public class JObject : IDisposable
{
    private bool Disposed { get; set; }

    public IntPtr Handle { get; set;}

    internal JNI.ReferenceType ReferenceType { get; set;}

    public JObject() { }

    public JObject(IntPtr handle, JNI.ReferenceType referenceType)
    {
        this.Handle = handle;
        this.ReferenceType = referenceType;
    }

    public JObject(JObject obj) : this(obj.Handle, obj.ReferenceType) 
    {
        this.Disposed = obj.Disposed;
        obj.Disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed || this.Handle == IntPtr.Zero)
            return;

        switch (this.ReferenceType)
        {
            case JNI.ReferenceType.Local:
                JNI.DeleteLocalRef(this);
                break;

            case JNI.ReferenceType.Global:
                JNI.DeleteGlobalRef(this);
                break;

            case JNI.ReferenceType.WeakGlobal:
                JNI.DeleteWeakGlobalRef(this);
                break;
        }

        Disposed = true;
    }

    public bool Valid()
    {
        return this.Handle != IntPtr.Zero;
    }

    ~JObject()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
#endif
