using System;


namespace ESInv.DBC
{
	public static class Ensure
	{
		public static void That(
			bool condition,
			string message)
		{
			if (!condition) { throw new PreConditionFailureException(message); }
		}


		public static void That(
			bool condition,
			string format,
			params object[] args)
		{
			if (!condition) { throw new PreConditionFailureException(string.Format(format, args)); }
		}
	}
}
