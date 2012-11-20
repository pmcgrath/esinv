using System;
using System.Collections.Generic;


namespace ESInv.Domain
{
	public interface IAggregate
	{
		Guid Id { get; }
		int Version { get; }
		IEnumerable<ESInv.Messaging.IEvent> UncommittedChanges { get; }


		void ClearUncommitedChanges();
	}
}
