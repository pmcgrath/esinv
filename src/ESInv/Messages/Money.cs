using System;


namespace ESInv.Messages
{
	public class Money
	{
		public readonly string Currency;
		public readonly decimal Amount;


		public Money(
			string currency,
			decimal amount)
		{
			this.Currency = currency;
			this.Amount = amount;
		}
	}
}
