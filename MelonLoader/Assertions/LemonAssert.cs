using System;

namespace MelonLoader.Assertions
{
    public static class LemonAssert
    {
        private static void Failure(string exceptionMsg, string userMessage, bool shouldThrowException)
        {
			if (string.IsNullOrEmpty(exceptionMsg))
				exceptionMsg = "Assertion has failed!";
			LemonAssertException exception = new LemonAssertException(exceptionMsg, userMessage);
			if (shouldThrowException)
				throw exception;
			else
				MelonLogger.Error(exception);
		}

		public static void IsNull<T>(T obj)
			=> IsNull(obj, null, true);
		public static void IsNull<T>(T obj, string userMessage)
			=> IsNull(obj, userMessage, true);
		public static void IsNull<T>(T obj, string userMessage, bool shouldThrowException)
        {
			bool result = false;
			if (LemonAssertMapping.IsNull.TryGetValue(typeof(T), out Delegate method))
				result = (bool)method.DynamicInvoke(new object[] { obj });
			else if (LemonAssertMapping.IsNull.TryGetValue(typeof(object), out Delegate method2))
				result = (bool)method2.DynamicInvoke(new object[] { obj });
			if (!result)
				Failure(NullFailureMessage(true), userMessage, shouldThrowException);
		}

		public static void IsNotNull<T>(T obj)
			=> IsNotNull(obj, null, true);
		public static void IsNotNull<T>(T obj, string userMessage)
			=> IsNotNull(obj, userMessage, true);
		public static void IsNotNull<T>(T obj, string userMessage, bool shouldThrowException)
		{
			bool result = false;
			if (LemonAssertMapping.IsNull.TryGetValue(typeof(T), out Delegate method))
				result = (bool)method.DynamicInvoke(new object[] { obj });
			else if (LemonAssertMapping.IsNull.TryGetValue(typeof(object), out Delegate method2))
				result = (bool)method2.DynamicInvoke(new object[] { obj });
			if (result)
				Failure(NullFailureMessage(false), userMessage, shouldThrowException);
		}

		public static void IsFalse(bool obj)
			=> IsFalse(obj, null, true);
		public static void IsFalse(bool obj, string userMessage)
			=> IsFalse(obj, userMessage, true);
		public static void IsFalse(bool obj, string userMessage, bool shouldThrowException)
		{
			if (!obj)
				return;
			Failure(BooleanFailureMessage(false), userMessage, shouldThrowException);
		}

		public static void IsTrue(bool obj)
			=> IsTrue(obj, null, true);
		public static void IsTrue(bool obj, string userMessage)
			=> IsTrue(obj, userMessage, true);
		public static void IsTrue(bool obj, string userMessage, bool shouldThrowException)
		{
			if (obj)
				return;
			Failure(BooleanFailureMessage(true), userMessage, shouldThrowException);
		}

		public static void IsEqual<T>(T obj, T obj2)
			=> IsEqual(obj, obj2, null, true);
		public static void IsEqual<T>(T obj, T obj2, string userMessage)
			=> IsEqual(obj, obj2, userMessage, true);
		public static void IsEqual<T>(T obj, T obj2, string userMessage, bool shouldThrowException)
		{
			bool result = false;
			if (LemonAssertMapping.IsEqual.TryGetValue(typeof(T), out Delegate method))
				result = (bool)method.DynamicInvoke(new object[] { obj, obj2 });
			else if (LemonAssertMapping.IsEqual.TryGetValue(typeof(object), out Delegate method2))
				result = (bool)method2.DynamicInvoke(new object[] { obj, obj2 });
			if (!result)
				Failure(EqualityFailureMessage(obj, obj2, true), userMessage, shouldThrowException);
		}

		public static void IsNotEqual<T>(T obj, T obj2)
			=> IsNotEqual(obj, obj2, null, true);
		public static void IsNotEqual<T>(T obj, T obj2, string userMessage)
			=> IsNotEqual(obj, obj2, userMessage, true);
		public static void IsNotEqual<T>(T obj, T obj2, string userMessage, bool shouldThrowException)
		{
			bool result = false;
			if (LemonAssertMapping.IsEqual.TryGetValue(typeof(T), out Delegate method))
				result = (bool)method.DynamicInvoke(new object[] { obj, obj2 });
			else if (LemonAssertMapping.IsEqual.TryGetValue(typeof(object), out Delegate method2))
				result = (bool)method2.DynamicInvoke(new object[] { obj, obj2 });
			if (result)
				Failure(EqualityFailureMessage(obj, obj2, false), userMessage, shouldThrowException);
		}

		private static string GetFailureMessage(string result, string expected)
			=> $"Assertion failure! {result}\nExpected: {expected}";
		private static string NullFailureMessage(bool expectedResult)
		{
			string result = $"Value was {(expectedResult ? "not " : "")}Null";
			string expected = $"Value was {(!expectedResult ? "not " : "")}Null";
			return GetFailureMessage(result, expected);
		}
		private static string BooleanFailureMessage(bool expectedResult)
		{
			string result = $"Value was {!expectedResult}";
			string expected = $"Value was {expectedResult}";
			return GetFailureMessage(result, expected);
		}
		private static string EqualityFailureMessage(object obj, object obj2, bool expectedResult)
		{
			string result = $"{obj} {(!expectedResult ? "==" : "!=")} {obj2}";
			string expected = $"{obj} {(expectedResult ? "==" : "!=")} {obj2}";
			return GetFailureMessage(result, expected);
		}
	}
}
