#if ANDROID
using System;

namespace MelonLoader.Java;

public class JThrowableException : Exception
{
    public JThrowable? Throwable { get; set;}

    public JThrowableException() { }

    public JThrowableException(JThrowable throwable) : base()
    {
        this.Throwable = throwable;
    }
}
#endif
