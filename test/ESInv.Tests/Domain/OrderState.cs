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
		public void When_Order_Created_Results_In_An_Order_With_Order_Attributes()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
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


		[Test, ExpectedException()]
		public void When_Order_Created_Twice_Results_In_An_Exception()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				1001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated, _orderCreated });
		}


		[Test, ExpectedException()]
		public void When_Payment_Made_For_Order_Not_Already_Created_Results_In_An_Exception()
		{
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 127M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[0]);

			_SUT.Mutate(_orderPaymentMade);
		}

	
		[Test]
		public void When_Full_DCC_Payment_Made_Results_In_An_Order_With_Payments()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 127M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual(_orderPaymentMade.Value.Currency, _SUT.NetPayments.Currency);
			Assert.AreEqual(_orderPaymentMade.Value.Amount, _SUT.NetPayments.Amount);
			Assert.AreEqual(1, _SUT.Entries.Count());
		}


		[Test]
		public void When_Full_Non_DCC_Payment_Made_Results_In_An_Order_With_Payments()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("EUR", 100M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual(_orderPaymentMade.Value.Currency, _SUT.NetPayments.Currency);
			Assert.AreEqual(_orderPaymentMade.Value.Amount, _SUT.NetPayments.Amount);
			Assert.AreEqual(1, _SUT.Entries.Count());
		}


		[Test]
		public void When_Partial_DCC_Payment_Made_Results_In_An_Order_With_Payments()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 27.56M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new[] { _orderCreated });

			_SUT.Mutate(_orderPaymentMade);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual(_orderPaymentMade.Value.Currency, _SUT.NetPayments.Currency);
			Assert.AreEqual(_orderPaymentMade.Value.Amount, _SUT.NetPayments.Amount);
			Assert.AreEqual(1, _SUT.Entries.Count());
		}


		[Test]
		public void When_Second_Partial_DCC_Payment_Made_Results_In_An_Order_With_Payments_Made()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPayment1Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107M),
				DateTimeOffset.Now);
			var _orderPayment2Made = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 20M),
				DateTimeOffset.Now);

			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPayment1Made });

			_SUT.Mutate(_orderPayment2Made);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual(_orderPayment1Made.Value.Currency, _SUT.NetPayments.Currency);
			Assert.AreEqual(_orderPayment1Made.Value.Amount + _orderPayment2Made.Value.Amount, _SUT.NetPayments.Amount);
			Assert.AreEqual(2, _SUT.Entries.Count());
		}


		[Test]
		public void When_A_Partial_Refund_Made_After_Payment_Results_In_An_Order_With_A_Net_Payment()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107M),
				DateTimeOffset.Now);
			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPaymentMade });

			var _orderRefundMade = new ESInv.Messages.OrderRefundMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 100M),
				DateTimeOffset.Now);

			_SUT.Mutate(_orderRefundMade);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual("USD", _SUT.NetPayments.Currency);
			Assert.AreEqual(7M, _SUT.NetPayments.Amount);
			Assert.AreEqual(2, _SUT.Entries.Count());
		}


		[Test]
		public void When_A_Full_Refund_Made_After_Payment_Results_In_An_Order_With_A_Net_Payment_Of_Zero()
		{
			var _orderCreated = new ESInv.Messages.OrderCreated(
				Guid.NewGuid(),
				10001,
				new ESInv.Messages.Money("EUR", 100M),
				new[]
					{
						new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
						new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
					},
				DateTimeOffset.Now);
			var _orderPaymentMade = new ESInv.Messages.OrderPaymentMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107M),
				DateTimeOffset.Now);
			var _SUT = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[] { _orderCreated, _orderPaymentMade });

			var _orderRefundMade = new ESInv.Messages.OrderRefundMade(
				Guid.NewGuid(),
				new ESInv.Messages.Money("USD", 107M),
				DateTimeOffset.Now);

			_SUT.Mutate(_orderRefundMade);

			Assert.IsTrue(_SUT.PaymentsHaveBeenMade);
			Assert.AreEqual("USD", _SUT.NetPayments.Currency);
			Assert.AreEqual(0M, _SUT.NetPayments.Amount);
			Assert.AreEqual(2, _SUT.Entries.Count());
		}
	}
}
