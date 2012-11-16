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
		public Money CumulativePaymentValue { get; private set; }
		

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
			ESInv.DBC.Ensure.That(this.Id == Guid.Empty, "Order has already been created");

			this.Id = @event.Id;
			this.MerchantId = @event.MerchantId;
			this.SaleValue = @event.SaleValue.FromMessage();
			this.Offers = @event.Offers.Select(offer => offer.FromMessage());
			this.CreationTimestamp = @event.Timestamp;
		}


		private void When(
			ESInv.Messages.OrderPaymentMade @event)
		{
			ESInv.DBC.Ensure.That(this.Id != Guid.Empty, "Order has NOT already been created");

			var _paymentValue = @event.Value.FromMessage();

			var _matchingOffer = this.Offers.FirstOrDefault(offer => offer.PaymentValue.Currency == _paymentValue.Currency);
			ESInv.DBC.Ensure.That(_matchingOffer != null, "Currency {0} is not an offer currency", _paymentValue.Currency);

			var _cumulativePaymentValue = this.CumulativePaymentValue != null ? this.CumulativePaymentValue : new Money(_paymentValue.Currency, 0.00M);
			ESInv.DBC.Ensure.That(_paymentValue.Currency == _cumulativePaymentValue.Currency, "Currency {0} is in conflict with an existing payment currency {1}", _paymentValue.Currency, _cumulativePaymentValue.Currency);

			_cumulativePaymentValue += _paymentValue;
			ESInv.DBC.Ensure.That(_cumulativePaymentValue <= _matchingOffer.PaymentValue, "Payment exceeds balance");

			this.CumulativePaymentValue = _cumulativePaymentValue;
		}
	}
}
