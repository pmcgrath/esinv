using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;


namespace ESInv.Tests
{
	[TestFixture]
	public class OrderAggregate
	{
		[Test]
		public void Create_Where_Good_Data_Results_In_Order_Creation()
		{
			// Start Billy and Noels build context
			// Parse and good
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");
			var _cardNumber = 4242424242424242UL;
			var _saleValue = new ESInv.Domain.Money("EUR", 100M);
			// Signature 
			// Create a creation context
			// End Billy and Noels build context

			var _cardNumberResolutionService = Substitute.For<ESInv.Domain.ICardNumberResolutionService>();
			_cardNumberResolutionService.Resolve(_cardNumber).Returns("USD");

			var _rateService = Substitute.For<ESInv.Domain.IRateService>();
			_rateService.GetRate(_saleValue.Currency, "USD").Returns(1.27M);

			var _SUT = ESInv.Domain.OrderAggregate.CreateEmpty();

			// Start pipeline
			_SUT.Create(
				_merchant,
				_cardNumber,
				_saleValue,
				_cardNumberResolutionService,
				_rateService);

			Assert.AreEqual(1, _SUT.UncommittedChanges.Count());
			Assert.IsInstanceOf<ESInv.Messages.OrderCreated>(_SUT.UncommittedChanges.First());
			var _event = _SUT.UncommittedChanges.First() as ESInv.Messages.OrderCreated;
			Assert.AreEqual(_merchant.Id, _event.MerchantId);
			Assert.AreEqual(_saleValue.Currency, _event.SaleValue.Currency);
			Assert.AreEqual(_saleValue.Amount, _event.SaleValue.Amount);
			Assert.AreEqual(2, _event.Offers.Count());
			
			var _nonDCCOffer = _event.Offers.First();
			var _DCCOffer = _event.Offers.Skip(1).First();

			Assert.AreEqual(1M, _nonDCCOffer.ExchangeRateIncludingMargin);
			Assert.AreEqual(_saleValue.Currency, _nonDCCOffer.PaymentValue.Currency);
			Assert.AreEqual(_saleValue.Amount, _nonDCCOffer.PaymentValue.Amount);
			Assert.AreNotEqual(1M, _DCCOffer.ExchangeRateIncludingMargin);
			Assert.AreNotEqual(_saleValue.Currency, _DCCOffer.PaymentValue.Currency);
			Assert.AreNotEqual(_saleValue.Amount, _DCCOffer.PaymentValue.Amount);
			Assert.AreEqual(1, _SUT.UncommittedChanges.Count());
		}


		[Test]
		public void ClearUncommitedChanges_Where_Order_Created_Event_Existed_Results_In_Order_With_No_Uncommited_Changes()
		{
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");
			var _cardNumber = 4242424242424242UL;
			var _saleValue = new ESInv.Domain.Money("EUR", 100M);

			var _cardNumberResolutionService = Substitute.For<ESInv.Domain.ICardNumberResolutionService>();
			_cardNumberResolutionService.Resolve(_cardNumber).Returns("USD");

			var _rateService = Substitute.For<ESInv.Domain.IRateService>();
			_rateService.GetRate(_saleValue.Currency, "USD").Returns(1.27M);

			var _SUT = ESInv.Domain.OrderAggregate.CreateEmpty();
			_SUT.Create(
				_merchant,
				_cardNumber,
				_saleValue,
				_cardNumberResolutionService,
				_rateService);

			_SUT.ClearUncommitedChanges();

			Assert.AreEqual(0, _SUT.UncommittedChanges.Count());
		}


		[Test, ExpectedException]
		public void MakePayment_Where_Non_Offer_Currency_Results_In_An_Exception()
		{
			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[]
				{
					new ESInv.Messages.OrderCreated(
						Guid.NewGuid(),
						Guid.NewGuid(),
						1,
						new ESInv.Messages.Money("EUR", 100M),
						new []
							{
								new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
								new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
							},
						DateTimeOffset.Now)
				};
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			_SUT.MakePayment(new ESInv.Domain.Money("GBP", 20M));
		}


		[Test]
		public void MakePayment_Where_Second_Payment_With_Same_Currency_But_Exceeds_The_Offer_Results_In_Accepted_Payment()
		{
			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[]
				{
					new ESInv.Messages.OrderCreated(
						Guid.NewGuid(),
						Guid.NewGuid(),
						1,
						new ESInv.Messages.Money("EUR", 100M),
						new []
							{
								new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
								new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
							},
						DateTimeOffset.Now),
					new ESInv.Messages.OrderPaymentMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("USD", 107M),
						DateTimeOffset.Now)
				};
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			_SUT.MakePayment(new ESInv.Domain.Money("USD", 100000M));

			Assert.AreEqual(1, _SUT.UncommittedChanges.Count());
			Assert.IsInstanceOf<ESInv.Messages.OrderPaymentMade>(_SUT.UncommittedChanges.First());
		}


		[Test, ExpectedException]
		public void MakePayment_Where_This_Is_Second_Payment_And_Is_Different_Currency_To_Previous_Payment_Results_In_An_Exception()
		{
			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[]
				{
					new ESInv.Messages.OrderCreated(
						Guid.NewGuid(),
						Guid.NewGuid(),
						1,
						new ESInv.Messages.Money("EUR", 100M),
						new []
							{
								new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
								new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
							},
						DateTimeOffset.Now),
					new ESInv.Messages.OrderPaymentMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("USD", 15M),
						DateTimeOffset.Now)
				};
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			_SUT.MakePayment(new ESInv.Domain.Money("EUR", 100000M));
		}


		[Test]
		public void MakeRefund_Where_Being_Made_After_A_Payment_And_Partial_Refund_Results_In_Accepted_Refund()
		{
			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[]
				{
					new ESInv.Messages.OrderCreated(
						Guid.NewGuid(),
						Guid.NewGuid(),
						1,
						new ESInv.Messages.Money("EUR", 100M),
						new []
							{
								new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
								new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
							},
						DateTimeOffset.Now),
					new ESInv.Messages.OrderPaymentMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("EUR", 90M),
						DateTimeOffset.Now),
					new ESInv.Messages.OrderRefundMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("EUR", 80M),
						DateTimeOffset.Now)
				};
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			_SUT.MakeRefund(new ESInv.Domain.Money("EUR", 5M));

			Assert.AreEqual(1, _SUT.UncommittedChanges.Count());
			Assert.IsInstanceOf<ESInv.Messages.OrderRefundMade>(_SUT.UncommittedChanges.First());
		}


		[Test, ExpectedException]
		public void MakeRefund_Where_Being_Made_After_A_Payment_And_Partial_Refund_But_Value_Exceeds_Net_Payments_Results_In_An_Exception()
		{
			var _SUTStateEventStreamEvents = new ESInv.Messaging.IEvent[]
				{
					new ESInv.Messages.OrderCreated(
						Guid.NewGuid(),
						Guid.NewGuid(),
						1,
						new ESInv.Messages.Money("EUR", 100M),
						new []
							{
								new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
								new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
							},
						DateTimeOffset.Now),
					new ESInv.Messages.OrderPaymentMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("EUR", 90M),
						DateTimeOffset.Now),
					new ESInv.Messages.OrderRefundMade(
						Guid.NewGuid(),
						new ESInv.Messages.Money("EUR", 80M),
						DateTimeOffset.Now)
				};
			var _SUTState = new ESInv.Domain.OrderState(_SUTStateEventStreamEvents);
			var _SUT = new ESInv.Domain.OrderAggregate(_SUTState);

			_SUT.MakeRefund(new ESInv.Domain.Money("EUR", 25M));
		}
	}
}
