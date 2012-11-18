using System;
using System.Collections.Generic;


namespace ESInv.Messages
{
	public class CreateOrder : ESInv.Messaging.ICommand
	{
		public readonly Guid Id;
		public readonly string MerchantIdentifier;
		public readonly Money SaleValue;
		

		public CreateOrder(
			Guid id,
			string merchantIdentifier,
			Money saleValue)
		{
			this.Id = id;
			this.MerchantIdentifier = merchantIdentifier;
			this.SaleValue = saleValue;
		}
	}
}
