using NSubstitute;
using NUnit.Framework;
using System;


namespace ESInv.Tests.Data
{
	[TestFixture]
	public class OrderRepository
	{
		[Test]
		public void RoundTrip()
		{
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");
			var _cardNumber = 4242424242424242UL;
			var _saleValue = new ESInv.Domain.Money("EUR", 100M);

			var _cardNumberResolutionService = Substitute.For<ESInv.Domain.ICardNumberResolutionService>();
			_cardNumberResolutionService.Resolve(_cardNumber).Returns("USD");

			var _rateService = Substitute.For<ESInv.Domain.IRateService>();
			_rateService.GetRate(_saleValue.Currency, "USD").Returns(1.27M);

			var _order = ESInv.Domain.OrderAggregate.CreateEmpty();

			_order.Create(
				_merchant,
				_cardNumber,
				_saleValue,
				_cardNumberResolutionService,
				_rateService);

			_order.MakePayment(
				new ESInv.Domain.Money("EUR", 50M));

			var _SUT = new ESInv.Data.Repository<ESInv.Domain.OrderAggregate>();
			_SUT.Save(_order);

			var _retreivedOrder = _SUT.GetById(_order.Id);

			_retreivedOrder.MakePayment(
				new ESInv.Domain.Money("EUR", 50M));

			_retreivedOrder.MakePayment(
				new ESInv.Domain.Money("EUR", 50M));

			_SUT.Save(_retreivedOrder);
		}
	}
}
