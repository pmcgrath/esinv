using System;


namespace ESInv.DBC
{
	public static class Ensure
	{
		public static void That(
			bool condition,
			string message)
		{
			if (!condition) { throw new ApplicationException(message); }
		}


		public static void That(
			bool condition,
			string format,
			params string[] args)
		{
			if (!condition) { throw new ApplicationException(string.Format(format, args)); }
		}
	}
}
