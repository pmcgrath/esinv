using System;
using System.Linq;


namespace ESInv.Domain
{
	public class OrderAggregate : Aggregate<OrderState>
	{
		public Guid Id { get { return base.State.Id; } }


		public OrderAggregate(
			OrderState state)
			: base(state)
		{
		}


		public static OrderAggregate CreateEmpty()
		{
			return new OrderAggregate(new OrderState(new ESInv.Messaging.IEvent[0]));
		}


		public void Create(
			Merchant merchant,
			ulong cardNumber,
			Money saleValue,
			ICardNumberResolutionService cardNumberResolutionService,
			IRateService rateService)
		{
			ESInv.DBC.Ensure.That(base.State.Id == Guid.Empty, "Order has already been created");

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
				Guid.NewGuid(),
				merchant.Id,
				saleValue.ToMessage(),
				_offers.Select(offer => offer.ToMessage()),
				DateTimeOffset.Now);

			base.Apply(_createdEvent);
		}


		public void MakePayment(
			Money value)
		{
			ESInv.DBC.Ensure.That(base.State.Id != Guid.Empty, "Order has not already been created");
			ESInv.DBC.Ensure.That(base.State.IsAnOfferCurrency(value.Currency), "Currency {0} is not an offer currency", value.Currency);
			if (base.State.PaymentsHaveBeenMade)
			{
				ESInv.DBC.Ensure.That(value.Currency == base.State.ExistingPaymentsCurrency, "Currency {0} is in conflict with existing payment(s) currency {1}", value.Currency, base.State.ExistingPaymentsCurrency);
			}

			var _orderPaymentMadeEvent = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				value.ToMessage(),
				DateTimeOffset.Now);

			base.Apply(_orderPaymentMadeEvent);
		}


		public void MakeRefund(
			Money value)
		{
			ESInv.DBC.Ensure.That(base.State.Id != Guid.Empty, "Order has not already been created");
			ESInv.DBC.Ensure.That(base.State.PaymentsHaveBeenMade, "Cannot make refund as no payments have been made");
			ESInv.DBC.Ensure.That(value.Currency == base.State.ExistingPaymentsCurrency, "Currency {0} is in conflict with payment(s) currency {1}", value.Currency, base.State.ExistingPaymentsCurrency);
			ESInv.DBC.Ensure.That(base.State.NetPaymentsValue >= value, "Refund value {0} exceeds net payments value", value.Amount);

			var _orderRefundMadeEvent = new ESInv.Messages.OrderRefundMade(
				Guid.NewGuid(),
				value.ToMessage(),
				DateTimeOffset.Now);

			base.Apply(_orderRefundMadeEvent);
		}
	}
}
