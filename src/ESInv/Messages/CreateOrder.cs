using System;


namespace ESInv.Messages
{
	public class CreateOrder : ESInv.Messaging.ICommand
	{
		public readonly Guid Id;
		public readonly string MerchantIdentifier;
		public readonly ulong CardNumber;
		public readonly Money SaleValue;
		

		public CreateOrder(
			Guid id,
			string merchantIdentifier,
			ulong cardNumber,
			Money saleValue)
		{
			this.Id = id;
			this.MerchantIdentifier = merchantIdentifier;
			this.CardNumber = cardNumber;
			this.SaleValue = saleValue;
		}
	}
}
