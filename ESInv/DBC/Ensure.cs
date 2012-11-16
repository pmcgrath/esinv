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


		public static void NotSet(
			Guid subject,
			string message = "Guid value not set")
		{
			if (subject != Guid.Empty) { throw new ApplicationException(message); }
		}
	}
}
