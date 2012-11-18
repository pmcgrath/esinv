using System;
using System.Linq;
using System.Collections.Generic;


namespace ESInv.Domain
{
	public class OrderAggregate
	{
		private readonly OrderState c_state;
		private readonly IList<ESInv.Messaging.IEvent> c_uncommittedChanges;


		public IEnumerable<ESInv.Messaging.IEvent> UncommittedChanges { get { return this.c_uncommittedChanges; } }


		public OrderAggregate(
			OrderState state)
		{
			this.c_state = state;

			this.c_uncommittedChanges = new List<ESInv.Messaging.IEvent>();
		}


		public void Create(
			Merchant merchant,
			ulong cardNumber,
			Money saleValue,
			ICardNumberResolutionService cardNumberResolutionService,
			IRateService rateService)
		{
			ESInv.DBC.Ensure.That(this.c_state.Id == Guid.Empty, "Order has already been created");

			// Request data is already good at this stage ? i.e. At the stage where Billy and Noel said a lookup was going to get created
			// Merchant has already been resolved at the domain service layer command handler
			// Probably best to re-use Billy and Noel's context - that is a good place to start these aggregate methods

			// Offers
			var _nonDCCOffer = new PaymentOffer(1M, saleValue);
			var _offers = new[] { _nonDCCOffer };
			
			var _cardCurrency = cardNumberResolutionService.Resolve(cardNumber);
			if (_cardCurrency != saleValue.Currency)
			{
				var _DCCOfferExchangeRateIncludingMargin = rateService.GetRate(saleValue.Currency, _cardCurrency);
				var _DCCOfferAmount = saleValue.Amount * _DCCOfferExchangeRateIncludingMargin;
				var _DCCOffer = new PaymentOffer(_DCCOfferExchangeRateIncludingMargin, new Money(_cardCurrency, _DCCOfferAmount));

				_offers = new[] { _nonDCCOffer, _DCCOffer };
			}
			
			var _createdEvent = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				merchant.Id,
				saleValue.ToMessage(),
				_offers.Select(offer => offer.ToMessage()),
				DateTimeOffset.Now);
			this.Apply(_createdEvent);
		}


		public void MakePayment(
			Money value)
		{
			ESInv.DBC.Ensure.That(this.c_state.Id != Guid.Empty, "Order has not already been created");
			ESInv.DBC.Ensure.That(this.c_state.IsAnOfferCurrency(value.Currency), "Currency {0} is not an offer currency", value.Currency);
			if (this.c_state.PaymentsHaveBeenMade)
			{
				ESInv.DBC.Ensure.That(value.Currency == this.c_state.ExistingPaymentsCurrency, "Currency {0} is in conflict with existing payment(s) currency {1}", value.Currency, this.c_state.ExistingPaymentsCurrency);
			}

			var _orderPaymentMadeEvent = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				value.ToMessage(),
				DateTimeOffset.Now);

			this.Apply(_orderPaymentMadeEvent);
		}


		public void MakeRefund(
			Money value)
		{
			ESInv.DBC.Ensure.That(this.c_state.Id != Guid.Empty, "Order has not already been created");
			ESInv.DBC.Ensure.That(this.c_state.PaymentsHaveBeenMade, "Cannot make refund as no payments have been made");
			ESInv.DBC.Ensure.That(value.Currency == this.c_state.ExistingPaymentsCurrency, "Currency {0} is in conflict with payment(s) currency {1}", value.Currency, this.c_state.ExistingPaymentsCurrency);
			ESInv.DBC.Ensure.That(this.c_state.NetPaymentsValue >= value, "Refund value {0} exceeds net payments value", value.Amount);

			var _orderRefundMadeEvent = new ESInv.Messages.OrderRefundMade(
				Guid.NewGuid(),
				value.ToMessage(),
				DateTimeOffset.Now);

			this.Apply(_orderRefundMadeEvent);
		}


		private void Apply(
			ESInv.Messaging.IEvent @event)
		{
			this.c_uncommittedChanges.Add(@event);
			this.c_state.Mutate(@event);
		}
	}
}
