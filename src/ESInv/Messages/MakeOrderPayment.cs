using System;


namespace ESInv.Messages
{
	public class MakeOrderPayment : ESInv.Messaging.ICommand
	{
		public readonly Guid Id;
		public readonly Guid OrderId;
		public readonly Money Value;


		public MakeOrderPayment(
			Guid id,
			Guid orderId,
			Money value)
		{
			this.Id = id;
			this.OrderId = orderId;
			this.Value = value;
		}
	}
}
