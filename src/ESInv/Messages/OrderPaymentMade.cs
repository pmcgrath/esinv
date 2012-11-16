using System;


namespace ESInv.Messages
{
	public class OrderPaymentMade : ESInv.Messaging.IEvent
	{
		public Guid Id { get; private set; }
		public Money Value { get; private set; }
		public DateTimeOffset Timestamp { get; private set; }


		public OrderPaymentMade(
			Guid id,
			Money value,
			DateTimeOffset timestamp)
		{
			this.Id = id;
			this.Value = value;
			this.Timestamp = timestamp;
		}
	}
}
