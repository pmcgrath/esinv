using EventStore.ClientAPI;
using System;


namespace ESInv.Data.EventStore
{
	public class EventStoreDescriptor : IEvent
	{
		public Guid EventId { get; private set; }
		public string Type { get; private set; }
		public byte[] Data { get; private set; }
		public byte[] Metadata { get; private set; }


		public EventStoreDescriptor(
			Guid eventId,
			string type,
			byte[] data,
			byte[] metadata)
		{
			this.EventId = eventId;
			this.Type = type;
			this.Data = data;
			this.Metadata = metadata;
		}
	}
}
