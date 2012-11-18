using System;


namespace ESInv.Domain
{
	public class OrderEntry
	{
		public readonly OrderEntryType Type;
		public readonly Money Value;
		public readonly DateTimeOffset Timestamp;


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
