using NUnit.Framework;
using System;
using System.Linq;


namespace ESInv.Tests
{
	[TestFixture]
	public class OrderAggregate
	{
		[Test]
		public void When_We_Have_Good_Data_Should_Result_In_Order_Creation()
		{
			// Start Billy and Noels build context
			// Parse and good
			var _cardNumber = 4242424242424242UL;
			var _saleValue = new ESInv.Domain.Money("EUR", 100.00M);
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");
			// Signature 
			// Create a creation context
			// End Billy and Noels build context

			var _cardNumberResolutionService = new ESInv.Services.CardNumberResolution();
			var _rateService = new ESInv.Services.Rate();

			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[0];
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			// Start pipeline
			_SUT.Create(
				_merchant,
				_cardNumber,
				_saleValue,
				_cardNumberResolutionService,
				_rateService);

			// Save evens to event stream
			//_SUT.Changes; 

			Assert.AreEqual(1, _SUT.Changes.Count());
			Assert.IsInstanceOf<ESInv.Messages.OrderCreated>(_SUT.Changes.First());
			var _event = _SUT.Changes.First() as ESInv.Messages.OrderCreated;
			Assert.AreEqual(_merchant.Id, _event.MerchantId);
			Assert.AreEqual(_saleValue.Currency, _event.SaleValue.Currency);
			Assert.AreEqual(_saleValue.Amount, _event.SaleValue.Amount);
			Assert.AreEqual(2, _event.Offers.Count());
			
			var _nonDCCOffer = _event.Offers.First();
			var _DCCOffer = _event.Offers.Skip(1).First();

			Assert.AreEqual(1.00M, _nonDCCOffer.ExchangeRateIncludingMargin);
			Assert.AreEqual(_saleValue.Currency, _nonDCCOffer.PaymentValue.Currency);
			Assert.AreEqual(_saleValue.Amount, _nonDCCOffer.PaymentValue.Amount);
			Assert.AreNotEqual(1.00M, _DCCOffer.ExchangeRateIncludingMargin);
			Assert.AreNotEqual(_saleValue.Currency, _DCCOffer.PaymentValue.Currency);
			Assert.AreNotEqual(_saleValue.Amount, _DCCOffer.PaymentValue.Amount);
		}
	}
}
