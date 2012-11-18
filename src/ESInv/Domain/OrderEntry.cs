using System;


namespace ESInv.Domain
{
	public class OrderEntry
	{
		public OrderEntryType Type { get; private set; }
		public Money Value { get; private set; }
		public DateTimeOffset Timestamp { get; private set; }


		public OrderEntry(
			OrderEntryType type,
			Money value,
			DateTimeOffset timestamp)
		{
			this.Type = type;
			this.Value = value;
			this.Timestamp = timestamp;
		}
	}
}
