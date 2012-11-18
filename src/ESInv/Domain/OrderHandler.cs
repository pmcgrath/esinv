using System;


namespace ESInv.Domain
{
	public class OrderHandler :
		ESInv.Messaging.ICommandHandler<ESInv.Messages.CreateOrder>,
		ESInv.Messaging.ICommandHandler<ESInv.Messages.MakeOrderPayment>,
		ESInv.Messaging.ICommandHandler<ESInv.Messages.MakeOrderRefund>
	{
		private readonly ESInv.Domain.IRepository<ESInv.Domain.OrderAggregate> c_repository;
		private readonly ESInv.Domain.ICardNumberResolutionService c_cardNumberResolutionService;
		private readonly ESInv.Domain.IRateService c_rateService;


		public OrderHandler(
			ESInv.Domain.IRepository<ESInv.Domain.OrderAggregate> repository,
			ESInv.Domain.ICardNumberResolutionService cardNumberResolutionService,
			ESInv.Domain.IRateService rateService)
		{
			this.c_repository = repository;
			this.c_cardNumberResolutionService = cardNumberResolutionService;
			this.c_rateService = rateService;
		}


		public void Handle(
			ESInv.Messages.CreateOrder command)
		{
			// Pending 
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");

			var _aggregate = ESInv.Domain.OrderAggregate.CreateEmpty();

			_aggregate.Create(
				_merchant,
				command.CardNumber,
				command.SaleValue.FromMessage(),
				this.c_cardNumberResolutionService,
				this.c_rateService);

			this.c_repository.Save(_aggregate);
		}


		public void Handle(
			ESInv.Messages.MakeOrderPayment command)
		{
			var _aggregate = this.c_repository.GetById(command.OrderId);

			_aggregate.MakePayment(
				command.Value.FromMessage());

			this.c_repository.Save(_aggregate);
		}


		public void Handle(
			ESInv.Messages.MakeOrderRefund command)
		{
			var _aggregate = this.c_repository.GetById(command.OrderId);

			_aggregate.MakeRefund(
				command.Value.FromMessage());

			this.c_repository.Save(_aggregate);
		}
	}
}
