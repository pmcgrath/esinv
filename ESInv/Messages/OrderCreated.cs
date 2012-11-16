using System;
using System.Collections.Generic;


namespace ESInv.Messages
{
	public class OrderCreated : ESInv.Messaging.IEvent
	{
		public Guid Id { get; private set; }
		public int MerchantId { get; private set; }
		public Money SaleValue { get; private set; }
		public IEnumerable<PaymentOffer> Offers { get; private set; }
		public DateTimeOffset Timestamp { get; private set; }


		public OrderCreated(
			Guid id,
			int merchantId,
			Money saleValue,
			IEnumerable<PaymentOffer> offers,
			DateTimeOffset timestamp)
		{
			this.Id = id;
			this.MerchantId = merchantId;
			this.SaleValue = saleValue;
			this.Offers = offers;
			this.Timestamp = timestamp;
		}
	}
}
