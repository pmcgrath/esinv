using System;


namespace ESInv.Messages
{
	public class PaymentOffer
	{
		public decimal ExchangeRateIncludingMargin { get; private set; }
		public Money PaymentValue { get; private set; }


		public PaymentOffer(
			decimal exchangeRateIncludingMargin,
			Money paymentValue)
		{
			this.ExchangeRateIncludingMargin = exchangeRateIncludingMargin;
			this.PaymentValue = paymentValue;
		}
	}
}
