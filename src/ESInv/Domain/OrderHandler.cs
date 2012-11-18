using System;


namespace ESInv.Domain
{
	public class OrderHandler : ESInv.Messaging.ICommandHandler<ESInv.Messages.CreateOrder>
	{
		public OrderHandler()
		{
		}


		public void Handle(
			ESInv.Messages.CreateOrder command)
		{
			var _merchant = new ESInv.Domain.Merchant(12, "The merchant name");

			var _cardNumberResolutionService = new ESInv.Services.CardNumberResolution();
			var _rateService = new ESInv.Services.Rate();

			var _aggregateState = new ESInv.Domain.OrderState(new ESInv.Messaging.IEvent[0]);
			var _aggregate = new ESInv.Domain.OrderAggregate(_aggregateState);

			_aggregate.Create(
				_merchant,
				command.CardNumber,
				command.SaleValue.FromMessage(),
				_cardNumberResolutionService,
				_rateService);

			//_aggregate.UncommittedChanges
		}
	}
}
