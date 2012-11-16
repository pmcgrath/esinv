using System;
using System.Collections.Generic;
using System.Linq;


namespace ESInv.Domain
{
	public class OrderState
	{
		public Guid Id { get; private set; }
		public int MerchantId { get; private set; }
		public Money SaleValue { get; private set; }
		public IEnumerable<PaymentOffer> Offers { get; private set; }
		public DateTimeOffset CreationTimestamp { get; private set; }
		public string PaymentCurrency { get; private set; }
		public decimal SaleCurrencyBalance { get; private set; }
		public decimal PaymentCurrencyBalance { get; private set; }
		

		public OrderState(
			IEnumerable<ESInv.Messaging.IEvent> events)
		{
			foreach (var @event in events) { this.Mutate(@event); }
		}


		public void Mutate(
			ESInv.Messaging.IEvent @event)
		{
			((dynamic)this).When((dynamic)@event);
		}


		private void When(
			ESInv.Messages.OrderCreated @event)
		{
			ESInv.DBC.Ensure.NotSet(this.Id, "Order has already been created");

			this.Id = @event.Id;
			this.MerchantId = @event.MerchantId;
			this.SaleValue = @event.SaleValue.FromMessage();
			this.Offers = @event.Offers.Select(offer => offer.FromMessage());
			this.CreationTimestamp = @event.Timestamp;
		}


		private void When(
			ESInv.Messages.OrderConfirmed @event)
		{
			ESInv.DBC.Ensure.That(this.Id != Guid.Empty, "Order has NOT already been created");
			// Verify currencies etc

			this.PaymentCurrency = @event.PaymentCurrency;
			this.SaleCurrencyBalance += @event.SaleCurrencyAmount;
			this.PaymentCurrencyBalance += @event.PaymentCurrencyAmount;
		}
	}
}
