using System;
using System.Runtime.Serialization;


namespace ESInv.DBC
{
	public class PreConditionFailureException : Exception
	{
		public PreConditionFailureException()
		{
		}


		public PreConditionFailureException(
			string message)
			: base(message)
		{
		}


		public PreConditionFailureException(
			string message,
			Exception inner)
			: base(message, inner)
		{
		}


		protected PreConditionFailureException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}
