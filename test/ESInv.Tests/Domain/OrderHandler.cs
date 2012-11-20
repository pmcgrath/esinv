using NSubstitute;
using NUnit.Framework;
using System;


namespace ESInv.Tests.Domain
{
	[TestFixture]
	public class OrderHandler
	{
		[Test]
		public void CreateOrder_Where_Good_Data_Results_In_Saved_Order()
		{
			var _command = new ESInv.Messages.CreateOrder(
				Guid.NewGuid(),
				"merchant_1",
				4242424242424242UL,
				new ESInv.Messages.Money("EUR", 100M));

			var _repository = Substitute.For<ESInv.Domain.IRepository<ESInv.Domain.OrderAggregate>>();
			
			var _cardNumberResolutionService = Substitute.For<ESInv.Domain.ICardNumberResolutionService>();
			_cardNumberResolutionService.Resolve(_command.CardNumber).Returns("USD");
			
			var _rateService = Substitute.For<ESInv.Domain.IRateService>();
			_rateService.GetRate(_command.SaleValue.Currency, "USD").Returns(1.27M);

			var _SUT = new ESInv.Domain.OrderHandler(_repository, _cardNumberResolutionService, _rateService);

			_SUT.Handle(_command);

			_repository.Received().Save(Arg.Any<ESInv.Domain.OrderAggregate>());
		}


		[Test]
		public void MakeOrderPayment_Where_Good_Data_Results_In_Saved_Order()
		{
			var _command = new ESInv.Messages.MakeOrderPayment(
				Guid.NewGuid(),
				Guid.NewGuid(),
				new ESInv.Messages.Money("EUR", 100M));

			var _aggregate = new ESInv.Domain.OrderAggregate(
				new ESInv.Messaging.IEvent[] 
					{
						new ESInv.Messages.OrderCreated(
							Guid.NewGuid(),
							_command.OrderId,
							1,
							new ESInv.Messages.Money("EUR", 100M),
							new []
								{
									new ESInv.Messages.PaymentOffer(1M, new ESInv.Messages.Money("EUR", 100M)),
									new ESInv.Messages.PaymentOffer(1.27M, new ESInv.Messages.Money("USD", 127M))
								},
							DateTimeOffset.Now)
					});
			var _repository = Substitute.For<ESInv.Domain.IRepository<ESInv.Domain.OrderAggregate>>();
			_repository.GetById(_command.OrderId).Returns(_aggregate);
			
			var _cardNumberResolutionService = Substitute.For<ESInv.Domain.ICardNumberResolutionService>();
			
			var _rateService = Substitute.For<ESInv.Domain.IRateService>();
			
			var _SUT = new ESInv.Domain.OrderHandler(_repository, _cardNumberResolutionService, _rateService);

			_SUT.Handle(_command);

			_repository.Received().Save(Arg.Any<ESInv.Domain.OrderAggregate>());
		}
	}
}
