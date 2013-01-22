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
			using (var _connection = EventStoreConnection.Create())
			{
				_connection.Connect(this.c_tcpEndPoint);

				var _eventStreamEventsSlice = _connection.ReadStreamEventsForward(eventStream, 0, int.MaxValue, false);

				return _eventStreamEventsSlice.Events
					.Skip(1)
					.Select(eventStoreRecordedEvent => 
						JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventStoreRecordedEvent.Event.Data), Type.GetType(eventStoreRecordedEvent.Event.EventType)))
					.Cast<ESInv.Messaging.IEvent>();
			}
		}


		public void Save(
			string eventStream,
			int expectedVersion,
			IEnumerable<ESInv.Messaging.IEvent> events)
		{
			using (var _connection = EventStoreConnection.Create())
			{
				_connection.Connect(this.c_tcpEndPoint);

				var _eventStoreEventDataItems = events
					.Select(change =>
						new EventData(
							eventId: Guid.NewGuid(),
							type: change.GetType().FullName,
 							data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(change)),
							isJson: true,
							metadata: null))
					.ToArray();

				if (expectedVersion <= 0) { expectedVersion = ExpectedVersion.Any; }

				_connection.AppendToStream(eventStream, expectedVersion, _eventStoreEventDataItems);
			}
		}
	}
}
