using System;
using System.Linq;
using System.Collections.Generic;


namespace ESInv.Domain
{
	public class OrderAggregate
	{
		private readonly OrderState c_state;
		private readonly IList<ESInv.Messaging.IEvent> c_changes;


		public IEnumerable<ESInv.Messaging.IEvent> Changes { get { return this.c_changes; } }


		public OrderAggregate(
			OrderState state)
		{
			this.c_state = state;

			this.c_changes = new List<ESInv.Messaging.IEvent>();
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
			Money @value)
		{
			ESInv.DBC.Ensure.That(this.c_state.Id != Guid.Empty, "Order has not already been created");
			// Skipping other checks

			var _saleValue = @value;
			var _paymentValue = @value;
			if (@value.Currency != this.c_state.SaleValue.Currency)
			{
				var _matchingDCCOffer = this.c_state.Offers.FirstOrDefault(offer => offer.PaymentValue.Currency == @value.Currency);
				// Assumes it is found for now
				var _reverseEngineeredSaleValue = @value.Amount / _matchingDCCOffer.ExchangeRateIncludingMargin; // Ignoring rounding
				_saleValue = new Money(this.c_state.SaleValue.Currency, _reverseEngineeredSaleValue);

				_paymentValue = @value;
			}

			var _confirmedEvent = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				@value.ToMessage(),
				DateTimeOffset.Now);
			this.Apply(_confirmedEvent);
		}


		private void Apply(
			ESInv.Messaging.IEvent @event)
		{
			this.c_changes.Add(@event);
			this.c_state.Mutate(@event);
			// bus.Publish for external subscribers?
		}
	}
}
