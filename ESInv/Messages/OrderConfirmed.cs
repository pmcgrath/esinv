using System;


namespace ESInv.Messages
{
	public class OrderConfirmed : ESInv.Messaging.IEvent
	{
		public Guid Id { get; private set; }
		public string PaymentCurrency { get; private set; }
		public decimal SaleCurrencyAmount { get; private set; }
		public decimal PaymentCurrencyAmount { get; private set; }
		public DateTimeOffset Timestamp { get; private set; }


		public OrderConfirmed(
			Guid id,
			string paymentCurrency,
			decimal saleCurrencyAmount,
			decimal paymentCurrencyAmount,
			DateTimeOffset timestamp)
		{
			this.Id = id;
			this.PaymentCurrency = paymentCurrency;
			this.SaleCurrencyAmount = saleCurrencyAmount;
			this.PaymentCurrencyAmount = paymentCurrencyAmount;
			this.Timestamp = timestamp;
		}
	}
}
