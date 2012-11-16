using System;


namespace ESInv.Domain
{
	public static class MoneyExt
	{
		public static Money FromMessage(
			this ESInv.Messages.Money subject)
		{
			return new Money(subject.Currency, subject.Amount);
		}


		public static ESInv.Messages.Money ToMessage(
			this Money subject)
		{
			return new ESInv.Messages.Money(subject.Currency, subject.Amount);
		}
	}
}
