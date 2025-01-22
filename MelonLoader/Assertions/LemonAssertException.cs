using System;

namespace MelonLoader.Assertions;

public class LemonAssertException : Exception
{
    private readonly string UserMessage;

    public LemonAssertException(string exceptionMsg, string userMessage) : base(exceptionMsg)
        => UserMessage = userMessage;

    public override string Message
    {
        get
        {
            return !string.IsNullOrEmpty(UserMessage) ? $"{base.Message}\n{UserMessage}" : base.Message;
        }
    }
}
