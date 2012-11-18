using System;


namespace ESInv.Messages
{
	public class PaymentOffer
	{
		public readonly decimal ExchangeRateIncludingMargin;
		public readonly Money PaymentValue;


		public PaymentOffer(
			decimal exchangeRateIncludingMargin,
			Money paymentValue)
		{
			this.ExchangeRateIncludingMargin = exchangeRateIncludingMargin;
			this.PaymentValue = paymentValue;
		}
	}
}
