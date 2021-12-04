using System;

namespace MelonLoader.Assertions
{
	public class LemonAssertException : Exception
	{
		private string UserMessage;

		public LemonAssertException(string exceptionMsg, string userMessage) : base(exceptionMsg)
			=> UserMessage = userMessage;

		public override string Message
		{
			get
			{
				if (!string.IsNullOrEmpty(UserMessage))
					return $"{base.Message}\n{UserMessage}";
				return base.Message;
			}
		}
	}
}
