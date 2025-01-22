using System;

namespace MelonLoader.Assertions;

public class LemonAssertException(string exceptionMsg, string userMessage) : Exception(exceptionMsg)
{
    public override string Message
    {
        get
        {
            return !string.IsNullOrEmpty(userMessage) ? $"{base.Message}\n{userMessage}" : base.Message;
        }
    }
}
