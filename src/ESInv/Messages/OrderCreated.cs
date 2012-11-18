using System;
using System.Collections.Generic;


namespace ESInv.Messages
{
	public class OrderCreated : ESInv.Messaging.IEvent
	{
		public readonly Guid Id;
		public readonly int MerchantId;
		public readonly Money SaleValue;
		public readonly IEnumerable<PaymentOffer> Offers;
		public readonly DateTimeOffset Timestamp;


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
