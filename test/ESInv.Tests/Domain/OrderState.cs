using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ESInv.Tests
{
	[TestFixture]
	public class OrderState
	{
		[Test]
		public void When_Order_Created_Should_Result_In_An_Order_With_Order_Attributes()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			Assert.AreEqual(_orderCreated.Id, _SUT.Id);
			Assert.AreEqual(_orderCreated.MerchantId, _SUT.MerchantId);
			Assert.AreEqual(_orderCreated.SaleValue.Currency, _SUT.SaleValue.Currency);
			Assert.AreEqual(_orderCreated.SaleValue.Amount, _SUT.SaleValue.Amount);
			Assert.AreEqual(_orderCreated.Offers.Count(), _SUT.Offers.Count());
			for (int _index = 0; _index < _orderCreated.Offers.Count(); _index++)
			{
				var _expected = _orderCreated.Offers.Skip(_index).First();
				var _actual = _SUT.Offers.Skip(_index).First();

				Assert.AreEqual(_expected.ExchangeRateIncludingMargin, _actual.ExchangeRateIncludingMargin);
				Assert.AreEqual(_expected.PaymentValue.Currency, _actual.PaymentValue.Currency);
				Assert.AreEqual(_expected.PaymentValue.Amount, _actual.PaymentValue.Amount);
			}
			Assert.AreEqual(_orderCreated.Timestamp, _SUT.CreationTimestamp);
		}


		[Test, ExpectedException(typeof(ApplicationException))]
		public void When_Order_Created_Twice_Should_Result_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				1001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated, _orderCreated });
		}


		[Test]
		public void When_Full_DCC_Order_Payment_Made_Should_Result_In_An_Order_With_An_Outstanding_Balance_Of_Zero()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 127.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);

			Assert.AreEqual(_orderPaymentMade.Value.Currency, _SUT.CumulativePaymentValue.Currency);
			Assert.AreEqual(_orderPaymentMade.Value.Amount, _SUT.CumulativePaymentValue.Amount);
		}


		[Test]
		public void When_Second_Partial_DCC_Order_Payment_Made_Should_Result_In_An_Order_With_An_Outstanding_Balance_Of_Zero()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPayment1Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107.00M),
				DateTimeOffset.Now);
			var _orderPayment2Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 20.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPayment1Made });

			_SUT.Mutate(_orderPayment2Made);

			Assert.AreEqual(_orderPayment2Made.Value.Currency, _SUT.CumulativePaymentValue.Currency);
			Assert.AreEqual((_orderPayment1Made.Value.Amount + _orderPayment2Made.Value.Amount), _SUT.CumulativePaymentValue.Amount);
		}


		[Test, ExpectedException]
		public void When_DCC_Order_Payment_For_Non_Offer_Currency_Made_Should_Result_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("GBP", 100.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);
		}


		[Test, ExpectedException]
		public void When_DCC_Order_Payment_Made_That_Exceeds_The_Offer_Should_Result_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 10000.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);
		}


		[Test, ExpectedException]
		public void When_Second_Payment_Is_For_A_Different_Currency_Should_Result_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPayment1Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107.00M),
				DateTimeOffset.Now);
			var _orderPayment2Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("EUR", 1.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPayment1Made });

			_SUT.Mutate(_orderPayment2Made);
		}


		[Test, ExpectedException]
		public void When_Second_Payment_Is_For_Same_Currency_But_Exceeds_The_Balance_Should_Result_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100.00M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1.00M, new ESInv.Messages.Money("EUR", 100.00M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127.00M))
					},
				DateTimeOffset.Now);
			var _orderPayment1Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107.00M),
				DateTimeOffset.Now);
			var _orderPayment2Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 100000.00M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPayment1Made });

			_SUT.Mutate(_orderPayment2Made);
		}
	}
}
