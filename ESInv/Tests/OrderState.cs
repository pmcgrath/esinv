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
		public void When_Order_Confirmed_Should_Result_In_An_Order_With_A_Confirmed_Order()
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
			var _orderConfirmed = new ESInv.Messages.OrderConfirmed(
				Guid.NewGuid(),
				"USD",
				100.00M,
				127.00M,
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderConfirmed);

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
	}
}
