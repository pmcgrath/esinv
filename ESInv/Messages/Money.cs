using System;


namespace ESInv.Messages
{
	public class Money
	{
		public string Currency { get; private set; }
		public decimal Amount { get; private set; }


		public Money(
			string currency,
			decimal amount)
		{
			this.Currency = currency;
			this.Amount = amount;
		}
	}
}
