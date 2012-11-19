using EventStore.ClientAPI;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Text;


namespace ESInv.Data
{
	public class OrderRepository : ESInv.Domain.IRepository<ESInv.Domain.OrderAggregate>
	{
		public ESInv.Domain.OrderAggregate GetById(
			Guid id)
		{
			var _endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113);
			using (var _connection = new EventStoreConnection(_endpoint))
			{
				var _eventStream = string.Format("order/{0}", id);
				var _task = _connection.ReadEventStreamForwardAsync(_eventStream, 0, int.MaxValue);
				_task.Wait();

				var _eventStreamEvents = _task.Result.Events;
				var _events = _eventStreamEvents
					.Skip(1) 
					.Select(@event => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(@event.Data), Type.GetType(@event.EventType)))
					.Cast<ESInv.Messaging.IEvent>();

				var _state = new ESInv.Domain.OrderState(_events);

				return new ESInv.Domain.OrderAggregate(_state);
			}
		}


		public void Save(
			ESInv.Domain.OrderAggregate aggregate)
		{
			var _endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113);
			using (var _connection = new EventStoreConnection(_endpoint))
			{
				var _eventStream = string.Format("order/{0}", aggregate.Id);

				// Should re-use event Id directly
				var _events = aggregate.UncommittedChanges
					.Select(change => 
						new Event(
							Guid.NewGuid(),
							change.GetType().FullName,
 							Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(change)),
							null))
					.ToArray();

				_connection.AppendToStream(_eventStream, -1, _events);

				aggregate.ClearUncommitedChanges();
			}
		}
	}



	public class TT
	{

		public void T()
		{
			var _originalEvent = new ESInv.Messages.OrderPaymentMade(Guid.NewGuid(), new ESInv.Messages.Money("EUR", 100M), DateTimeOffset.Now);

			var _eventJson = JsonConvert.SerializeObject(_originalEvent);

			var _deserialisedEvent = JsonConvert.DeserializeObject<ESInv.Messages.OrderPaymentMade>(_eventJson);

		}
	}


	public class Event : IEvent
	{
		public Guid EventId { get; private set; }
		public string Type { get; private set; }
		public byte[] Data { get; private set; }
		public byte[] Metadata { get; private set; }


		public Event(
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
