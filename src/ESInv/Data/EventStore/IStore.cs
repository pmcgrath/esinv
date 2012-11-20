using System;
using System.Collections.Generic;


namespace ESInv.Data.EventStore
{
	public interface IStore
	{
		IEnumerable<ESInv.Messaging.IEvent> GetAll(
			string eventStream);


		void Save(
			string eventStream,
			int expectedVersion,
			IEnumerable<ESInv.Messaging.IEvent> events);
	}
}
