using EventStore.ClientAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


namespace ESInv.Data.EventStore
{
	public class Store : IStore
	{
		private readonly IPEndPoint c_tcpEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113);


		public IEnumerable<ESInv.Messaging.IEvent> GetAll(
			string eventStream)
		{
			using (var _connection = new EventStoreConnection(this.c_tcpEndPoint))
			{
				var _eventStreamEventsSlice = _connection.ReadEventStreamForward(eventStream, 0, int.MaxValue);

				return _eventStreamEventsSlice.Events
					.Skip(1) 
					.Select(@event => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(@event.Data), Type.GetType(@event.EventType)))
					.Cast<ESInv.Messaging.IEvent>();
			}
		}


		public void Save(
			string eventStream,
			int expectedVersion,
			IEnumerable<ESInv.Messaging.IEvent> events)
		{
			using (var _connection = new EventStoreConnection(this.c_tcpEndPoint))
			{
				var _eventStoreDescriptors = events
					.Select(change =>
						new EventStoreDescriptor(
							Guid.NewGuid(),
							change.GetType().FullName,
 							Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(change)),
							null))
					.ToArray();

				if (expectedVersion <= 0) { expectedVersion = ExpectedVersion.Any; }

				_connection.AppendToStream(eventStream, expectedVersion, _eventStoreDescriptors);
			}
		}
	}
}
