using System;


namespace ESInv.Messages
{
	public class OrderRefundMade : ESInv.Messaging.IEvent
	{
		public readonly Guid Id;
		public readonly Money Value;
		public readonly DateTimeOffset Timestamp;


		public OrderRefundMade(
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
