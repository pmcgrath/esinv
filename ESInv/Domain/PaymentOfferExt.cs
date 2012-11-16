using System;


namespace ESInv.Domain
{
	public static class PaymentOfferExt
	{
		public static PaymentOffer FromMessage(
			this ESInv.Messages.PaymentOffer subject)
		{
			return new PaymentOffer(subject.ExchangeRateIncludingMargin, subject.PaymentValue.FromMessage());
		}


		public static ESInv.Messages.PaymentOffer ToMessage(
			this PaymentOffer subject)
		{
			return new ESInv.Messages.PaymentOffer(subject.ExchangeRateIncludingMargin, subject.PaymentValue.ToMessage());
		}
	}
}
